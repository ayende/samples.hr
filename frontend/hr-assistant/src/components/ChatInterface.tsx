import React, { useState, useEffect, useRef } from 'react';
import ReactMarkdown from 'react-markdown';
import { hrApi, ChatRequest, ChatResponse, EmployeeDropdown } from '../api';
import { SignatureDialog } from './SignatureDialog';
import './ChatInterface.css';

interface Message {
  id: string;
  text: string;
  isUser: boolean;
  timestamp: Date;
  followups?: string[];
}

export const ChatInterface: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputMessage, setInputMessage] = useState('');
  const [employees, setEmployees] = useState<EmployeeDropdown[]>([]);
  const [selectedEmployee, setSelectedEmployee] = useState<EmployeeDropdown | null>(null);
  const [isEmployeeSelected, setIsEmployeeSelected] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingEmployees, setIsLoadingEmployees] = useState(true);
  const [chatId, setChatId] = useState<string | null>(null);
  const [signatureDialog, setSignatureDialog] = useState<{
    isOpen: boolean;
    title: string;
    description: string;
    resolve?: (result: { signature: string | null; confirmed: boolean }) => void;
  }>({
    isOpen: false,
    title: '',
    description: ''
  });
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    // Load employees for dropdown
    const loadEmployees = async () => {
      try {
        setIsLoadingEmployees(true);
        const employeeData = await hrApi.getEmployeesForDropdown();
        setEmployees(employeeData);
      } catch (error) {
        console.error('Error loading employees:', error);
      } finally {
        setIsLoadingEmployees(false);
      }
    };

    loadEmployees();

    // Add welcome message
    const welcomeMessage: Message = {
      id: 'welcome',
      text: `# Welcome to HR Assistant! 👋

Please select your employee profile from the dropdown in the header above to get started.`,
      isUser: false,
      timestamp: new Date(),
      followups: [
      ]
    };
    setMessages([welcomeMessage]);
  }, []);

  const handleEmployeeSelect = async (employeeId: string) => {
    if (isEmployeeSelected) return; // Prevent changing once selected

    const employee = employees.find(emp => emp.id === employeeId);
    if (employee) {
      setSelectedEmployee(employee);
      setIsEmployeeSelected(true);
      setIsLoading(true);

      try {
        const historyResponse = await hrApi.getChatHistory(employee.id);

        // Set the chat ID from the response
        setChatId(historyResponse.chatId);

        // Convert API messages to frontend Message format
        const historyMessages: Message[] = historyResponse.messages.map(msg => ({
          id: msg.id,
          text: msg.text,
          isUser: msg.isUser,
          timestamp: new Date(msg.timestamp),
        }));

        // If there's no history, add welcome message
        if (historyMessages.length === 0) {
          const welcomeMessage: Message = {
            id: Date.now().toString(),
            text: `# Welcome to HR Assistant! 👋
Hello, **${employee.name}**, how can I help you today?`,
            isUser: false,
            timestamp: new Date(),
            followups: [
              "How much vacation time do I have?",
              "Onboarding materials?",
              "Reimbursement policies?"
            ]
          };
          setMessages([welcomeMessage]);
        } else {
          // Load existing chat history
          setMessages(historyMessages);
        }
      } catch (error) {
        console.error('Error loading chat history:', error);
        // Fallback to welcome message if history loading fails
        const welcomeMessage: Message = {
          id: Date.now().toString(),
          text: `# Welcome to HR Assistant! 👋
Hello, **${employee.name}**, how can I help you today?`,
          isUser: false,
          timestamp: new Date(),
          followups: [
            "How much vacation time do I have?",
            "Onboarding materials?",
            "Reimbursement policies?"
          ]
        };
        setMessages([welcomeMessage]);
      } finally {
        setIsLoading(false);
      }
    }
  };

  // Function to request signature - can be called from handleSendMessage
  // Usage: const result = await requestSignature("Title", "Description");
  // Returns: { signature: string | null, confirmed: boolean }
  const requestSignature = async (title: string, description: string): Promise<{ signature: string | null; confirmed: boolean }> => {
    return new Promise((resolve) => {
      setSignatureDialog({
        isOpen: true,
        title,
        description,
        resolve
      });
    });
  };

  const handleSignatureConfirm = (signature: string) => {
    setSignatureDialog(prev => {
      if (prev.resolve) {
        prev.resolve({ signature, confirmed: true });
      }
      return { ...prev, isOpen: false };
    });
  };

  const handleSignatureCancel = () => {
    setSignatureDialog(prev => {
      if (prev.resolve) {
        prev.resolve({ signature: null, confirmed: false });
      }
      return { ...prev, isOpen: false };
    });
  };

  const handleSendMessage = async () => {
    if (!inputMessage.trim() || isLoading || !selectedEmployee) return;

    // Normal message handling
    const userMessage: Message = {
      id: Date.now().toString(),
      text: inputMessage,
      isUser: true,
      timestamp: new Date(),
    };

    setMessages(prev => [...prev, userMessage]);
    setInputMessage('');
    setIsLoading(true);

    try {
      const request: ChatRequest = {
        chatId: chatId || undefined,
        message: inputMessage,
        employeeId: selectedEmployee.id,
        signatures: []
      };

      const response: ChatResponse = await hrApi.chat(request);
      var signatures = [];

      for (const signature of response.documentsToSign) {
        const signatureResult = await requestSignature(
          signature.title,
          signature.content
        );
        if (signatureResult.confirmed) {
          await hrApi.signDocument({
            chatId: response.chatId,
            employeeId: selectedEmployee.id,
            toolId: signature.toolId,
            documentId: signature.documentId,
            confirmed: signatureResult.confirmed,
            signatureBlob: signatureResult.signature || undefined
          });
          signatures.push({ toolId: signature.toolId, content: 'Signed by employee' });
        }
        else {
          signatures.push({ toolId: signature.toolId, content: 'Employee declined to sign' });
        }
      }
      // if there are signatures, send them back to the model
      const finalResponse = signatures.length === 0 ? response :
        await hrApi.chat({
          chatId: request.chatId,
          signatures: signatures,
          message: '',
          employeeId: selectedEmployee.id,
        });

      const botMessage: Message = {
        id: (Date.now() + 1).toString(),
        text: finalResponse.answer || "Document processing completed.",
        isUser: false,
        timestamp: new Date(),
        followups: finalResponse.followups,
      };

      setMessages(prev => [...prev, botMessage]);

      if (finalResponse.chatId) {
        setChatId(finalResponse.chatId);
      }

    } catch (error) {
      console.error('Error sending message:', error);
      const errorMessage: Message = {
        id: (Date.now() + 1).toString(),
        text: "I'm sorry, I'm having trouble connecting right now. Please try again in a moment, or contact IT support if the problem persists.",
        isUser: false,
        timestamp: new Date(),
      };
      setMessages(prev => [...prev, errorMessage]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleFollowupClick = (followup: string) => {
    setInputMessage(followup);
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  return (
    <div className="chat-container">
      <header className="header">
        <div className="header-content">
          <div className="title-section">
            <h1 className="title">Human Resources Assistant</h1>
            <p className="subtitle">Your dedicated HR support companion</p>
          </div>

          {!isEmployeeSelected ? (
            <select
              className="header-select"
              id="employeeSelect"
              value={selectedEmployee?.id || ''}
              onChange={(e: React.ChangeEvent<HTMLSelectElement>) => handleEmployeeSelect(e.target.value)}
              disabled={isLoadingEmployees}
            >
              <option value="">
                {isLoadingEmployees ? "Loading employees..." : "Choose your name to begin..."}
              </option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>
                  {employee.name} - {employee.jobTitle} ({employee.department})
                </option>
              ))}
            </select>
          ) : (
            <div className="header-selected-employee">
              <div className="employee-name">{selectedEmployee?.name}</div>
              <div className="employee-details">
                {selectedEmployee?.jobTitle} • {selectedEmployee?.department}
              </div>
            </div>
          )}
        </div>
      </header>

      <div className="messages-container">
        {messages.map((message) => (
          <div key={message.id} className={`message-bubble ${message.isUser ? 'user' : 'bot'}`}>
            <div className="message-content">
              {message.isUser ? (
                message.text
              ) : (
                <ReactMarkdown>{message.text}</ReactMarkdown>
              )}
            </div>

            <div className="timestamp">
              {message.timestamp.toLocaleTimeString([], {
                hour: '2-digit',
                minute: '2-digit'
              })}
            </div>

            {message.followups && message.followups.length > 0 && (
              <div className="followups">
                {message.followups.map((followup, index) => (
                  <button
                    key={index}
                    className="followup-button"
                    onClick={() => handleFollowupClick(followup)}
                    disabled={!selectedEmployee}
                  >
                    {followup}
                  </button>
                ))}
              </div>
            )}
          </div>
        ))}

        {isLoading && (
          <div className="loading-indicator">
            <div>🤔 Processing your request...</div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      <div className="input-container">
        <div className="message-input-container">
          <input
            className="message-input"
            type="text"
            value={inputMessage}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setInputMessage(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder={selectedEmployee ? "Ask me anything about HR..." : "Please select your employee profile above to get started"}
            disabled={isLoading || !selectedEmployee}
          />
          <button
            className="send-button"
            onClick={handleSendMessage}
            disabled={isLoading || !inputMessage.trim() || !selectedEmployee}
          >
            Send
          </button>
        </div>
      </div>

      <SignatureDialog
        isOpen={signatureDialog.isOpen}
        title={signatureDialog.title}
        description={signatureDialog.description}
        onConfirm={handleSignatureConfirm}
        onCancel={handleSignatureCancel}
      />
    </div>
  );
}
