import React, { useState, useEffect, useRef } from 'react';
import styled from 'styled-components';
import ReactMarkdown from 'react-markdown';
import { theme } from '../theme';
import { hrApi, ChatRequest, ChatResponse, ChatHistoryResponse, Employee } from '../api';

interface Message {
  id: string;
  text: string;
  isUser: boolean;
  timestamp: Date;
  followups?: string[];
}

const ChatContainer = styled.div`
  display: flex;
  flex-direction: column;
  height: 100vh;
  max-width: 900px;
  margin: 0 auto;
  background: ${theme.colors.background.primary};
`;

const Header = styled.div`
  background: linear-gradient(135deg, ${theme.colors.primary.main}, ${theme.colors.secondary.main});
  color: white;
  padding: ${theme.spacing.md} ${theme.spacing.lg};
  box-shadow: ${theme.shadows.medium};
  
  @media (max-width: 768px) {
    padding: ${theme.spacing.sm};
  }
`;

const HeaderContent = styled.div`
  max-width: 900px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: ${theme.spacing.lg};
  
  @media (max-width: 768px) {
    flex-direction: column;
    gap: ${theme.spacing.md};
    align-items: stretch;
  }
`;

const TitleSection = styled.div`
  flex-shrink: 0;
  
  @media (max-width: 768px) {
    text-align: center;
  }
`;

const Title = styled.h1`
  margin: 0;
  font-size: ${theme.typography.h3.fontSize};
  font-weight: ${theme.typography.h3.fontWeight};
  
  @media (max-width: 768px) {
    font-size: ${theme.typography.body.fontSize};
  }
`;

const Subtitle = styled.p`
  margin: ${theme.spacing.xs} 0 0;
  opacity: 0.9;
  font-size: ${theme.typography.caption.fontSize};
`;

const EmployeeSelectionHeader = styled.div`
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: ${theme.borderRadius.medium};
  padding: ${theme.spacing.md};
  border: 1px solid rgba(255, 255, 255, 0.2);
  min-width: 300px;
  flex-shrink: 0;
  
  @media (max-width: 768px) {
    min-width: unset;
    width: 100%;
    padding: ${theme.spacing.sm};
  }
`;

const HeaderLabel = styled.label`
  display: block;
  font-size: ${theme.typography.small.fontSize};
  color: white;
  font-weight: 600;
  margin-bottom: ${theme.spacing.xs};
  text-align: center;
`;

const HeaderSelect = styled.select`
  width: 100%;
  padding: ${theme.spacing.sm};
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: ${theme.borderRadius.small};
  font-size: ${theme.typography.small.fontSize};
  background: rgba(255, 255, 255, 0.9);
  color: ${theme.colors.text.primary};
  
  &:focus {
    outline: none;
    border-color: white;
    box-shadow: 0 0 0 2px rgba(255, 255, 255, 0.3);
  }
  
  &:disabled {
    background: rgba(255, 255, 255, 0.5);
    color: ${theme.colors.text.light};
    cursor: not-allowed;
  }
  
  @media (max-width: 768px) {
    font-size: ${theme.typography.caption.fontSize};
    padding: ${theme.spacing.xs};
  }
`;

const HeaderSelectedEmployee = styled.div`
  padding: ${theme.spacing.sm};
  background: rgba(255, 255, 255, 0.15);
  border: 2px solid rgba(255, 255, 255, 0.4);
  border-radius: ${theme.borderRadius.small};
  color: white;
  font-weight: 600;
  text-align: center;
  font-size: ${theme.typography.small.fontSize};
  
  .employee-name {
    font-size: ${theme.typography.body.fontSize};
    margin-bottom: 2px;
  }
  
  .employee-details {
    opacity: 0.9;
    font-weight: 400;
    font-size: ${theme.typography.caption.fontSize};
  }
  
  @media (max-width: 768px) {
    padding: ${theme.spacing.xs};
    
    .employee-name {
      font-size: ${theme.typography.small.fontSize};
    }
    
    .employee-details {
      font-size: 11px;
    }
  }
`;

const MessagesContainer = styled.div`
  flex: 1;
  padding: ${theme.spacing.lg};
  overflow-y: auto;
  background: ${theme.colors.background.secondary};
`;

const MessageBubble = styled.div<{ isUser: boolean }>`
  max-width: 75%;
  margin: ${theme.spacing.md} 0;
  padding: ${theme.spacing.lg};
  border-radius: ${theme.borderRadius.large};
  align-self: ${props => props.isUser ? 'flex-end' : 'flex-start'};
  margin-left: ${props => props.isUser ? 'auto' : '0'};
  margin-right: ${props => props.isUser ? '0' : 'auto'};
  
  ${props => props.isUser ? `
    background: linear-gradient(135deg, ${theme.colors.primary.main}, ${theme.colors.primary.dark});
    color: white;
    border-bottom-right-radius: ${theme.borderRadius.small};
  ` : `
    background: ${theme.colors.background.card};
    color: ${theme.colors.text.primary};
    border: 1px solid ${theme.colors.border.light};
    border-bottom-left-radius: ${theme.borderRadius.small};
    box-shadow: ${theme.shadows.small};
  `}
`;

const MessageContent = styled.div`
  line-height: ${theme.typography.body.lineHeight};
  
  h1, h2, h3, h4, h5, h6 {
    margin: ${theme.spacing.md} 0 ${theme.spacing.sm};
    color: ${theme.colors.primary.main};
  }
  
  p {
    margin: ${theme.spacing.sm} 0;
  }
  
  ul, ol {
    margin: ${theme.spacing.sm} 0;
    padding-left: ${theme.spacing.lg};
  }
  
  li {
    margin: ${theme.spacing.xs} 0;
  }
  
  strong {
    font-weight: 600;
    color: ${theme.colors.primary.main};
  }
  
  em {
    font-style: italic;
    opacity: 0.9;
  }
  
  code {
    background: ${theme.colors.background.accent};
    padding: 2px 4px;
    border-radius: ${theme.borderRadius.small};
    font-family: 'Courier New', monospace;
  }
`;

const Timestamp = styled.div`
  font-size: ${theme.typography.caption.fontSize};
  opacity: 0.7;
  margin-top: ${theme.spacing.sm};
`;

const Followups = styled.div`
  margin-top: ${theme.spacing.lg};
  display: flex;
  flex-wrap: wrap;
  gap: ${theme.spacing.sm};
`;

const FollowupButton = styled.button`
  background: ${theme.colors.secondary.lighter};
  color: ${theme.colors.text.primary};
  border: 1px solid ${theme.colors.secondary.light};
  border-radius: ${theme.borderRadius.large};
  padding: ${theme.spacing.sm} ${theme.spacing.md};
  font-size: ${theme.typography.small.fontSize};
  cursor: pointer;
  transition: all ${theme.transitions.normal};
  
  &:hover {
    background: ${theme.colors.secondary.light};
    color: white;
    transform: translateY(-1px);
    box-shadow: ${theme.shadows.small};
  }
`;

const InputContainer = styled.div`
  padding: ${theme.spacing.lg};
  background: ${theme.colors.background.card};
  border-top: 1px solid ${theme.colors.border.light};
  box-shadow: ${theme.shadows.medium};
`;

const EmployeeSection = styled.div`
  margin-bottom: ${theme.spacing.lg};
  padding: ${theme.spacing.md};
  background: ${theme.colors.background.accent};
  border-radius: ${theme.borderRadius.medium};
  border: 1px solid ${theme.colors.border.accent};
`;

const Label = styled.label`
  display: block;
  font-size: ${theme.typography.small.fontSize};
  color: ${theme.colors.text.secondary};
  font-weight: 600;
  margin-bottom: ${theme.spacing.sm};
`;

const Select = styled.select`
  width: 100%;
  padding: ${theme.spacing.md};
  border: 1px solid ${theme.colors.border.medium};
  border-radius: ${theme.borderRadius.medium};
  font-size: ${theme.typography.body.fontSize};
  background: white;
  
  &:focus {
    outline: none;
    border-color: ${theme.colors.primary.main};
    box-shadow: 0 0 0 3px ${theme.colors.primary.lighter};
  }
  
  &:disabled {
    background: ${theme.colors.background.secondary};
    color: ${theme.colors.text.light};
    cursor: not-allowed;
  }
`;

const SelectedEmployee = styled.div`
  padding: ${theme.spacing.md};
  background: white;
  border: 2px solid ${theme.colors.primary.light};
  border-radius: ${theme.borderRadius.medium};
  color: ${theme.colors.primary.dark};
  font-weight: 600;
  text-align: center;
`;

const MessageInputContainer = styled.div`
  display: flex;
  gap: ${theme.spacing.sm};
`;

const MessageInput = styled.input`
  flex: 1;
  padding: ${theme.spacing.md};
  border: 1px solid ${theme.colors.border.medium};
  border-radius: ${theme.borderRadius.medium};
  font-size: ${theme.typography.body.fontSize};
  
  &:focus {
    outline: none;
    border-color: ${theme.colors.primary.main};
    box-shadow: 0 0 0 3px ${theme.colors.primary.lighter};
  }
`;

const SendButton = styled.button`
  background: linear-gradient(135deg, ${theme.colors.primary.main}, ${theme.colors.primary.dark});
  color: white;
  border: none;
  border-radius: ${theme.borderRadius.medium};
  padding: ${theme.spacing.md} ${theme.spacing.xl};
  font-weight: 600;
  cursor: pointer;
  transition: all ${theme.transitions.normal};
  
  &:hover:not(:disabled) {
    transform: translateY(-1px);
    box-shadow: ${theme.shadows.medium};
  }
  
  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
`;

const LoadingIndicator = styled.div`
  display: flex;
  align-items: center;
  gap: ${theme.spacing.sm};
  padding: ${theme.spacing.md};
  color: ${theme.colors.text.secondary};
  font-style: italic;
`;

export const ChatInterface: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputMessage, setInputMessage] = useState('');
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(null);
  const [isEmployeeSelected, setIsEmployeeSelected] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [chatId, setChatId] = useState<string | null>(null);
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
        const employeeData = await hrApi.getEmployees();
        setEmployees(employeeData);
      } catch (error) {
        console.error('Error loading employees:', error);
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
        // Load existing chat history
        const cleanEmployeeId = employee.id.replace('employees/', '');
        const historyResponse = await hrApi.getChatHistory(cleanEmployeeId);

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

  const handleSendMessage = async () => {
    if (!inputMessage.trim() || isLoading || !selectedEmployee) return;

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
        employeeId: selectedEmployee.id.replace('employees/', ''),
      };

      const response: ChatResponse = await hrApi.chat(request);

      const botMessage: Message = {
        id: (Date.now() + 1).toString(),
        text: response.answer,
        isUser: false,
        timestamp: new Date(),
        followups: response.followups,
      };

      setMessages(prev => [...prev, botMessage]);

      // Update chat ID from the response to maintain conversation continuity
      if (response.chatId) {
        setChatId(response.chatId);
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
    <ChatContainer>
      <Header>
        <HeaderContent>
          <TitleSection>
            <Title>Human Resources Assistant</Title>
            <Subtitle>Your dedicated HR support companion</Subtitle>
          </TitleSection>


          {!isEmployeeSelected ? (
            <HeaderSelect
              id="employeeSelect"
              value={selectedEmployee?.id || ''}
              onChange={(e) => handleEmployeeSelect(e.target.value)}
            >
              <option value="">Choose your name to begin...</option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>
                  {employee.name} - {employee.jobTitle} ({employee.department})
                </option>
              ))}
            </HeaderSelect>
          ) : (
            <HeaderSelectedEmployee>
              <div className="employee-name">{selectedEmployee?.name}</div>
              <div className="employee-details">
                {selectedEmployee?.jobTitle} • {selectedEmployee?.department}
              </div>
            </HeaderSelectedEmployee>
          )}
        </HeaderContent>
      </Header>

      <MessagesContainer>
        {messages.map((message) => (
          <MessageBubble key={message.id} isUser={message.isUser}>
            <MessageContent>
              {message.isUser ? (
                message.text
              ) : (
                <ReactMarkdown>{message.text}</ReactMarkdown>
              )}
            </MessageContent>

            <Timestamp>
              {message.timestamp.toLocaleTimeString([], {
                hour: '2-digit',
                minute: '2-digit'
              })}
            </Timestamp>

            {message.followups && message.followups.length > 0 && (
              <Followups>
                {message.followups.map((followup, index) => (
                  <FollowupButton
                    key={index}
                    onClick={() => handleFollowupClick(followup)}
                    disabled={!selectedEmployee}
                  >
                    {followup}
                  </FollowupButton>
                ))}
              </Followups>
            )}
          </MessageBubble>
        ))}

        {isLoading && (
          <LoadingIndicator>
            <div>🤔 Processing your request...</div>
          </LoadingIndicator>
        )}

        <div ref={messagesEndRef} />
      </MessagesContainer>

      <InputContainer>
        <MessageInputContainer>
          <MessageInput
            type="text"
            value={inputMessage}
            onChange={(e) => setInputMessage(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder={selectedEmployee ? "Ask me anything about HR..." : "Please select your employee profile above to get started"}
            disabled={isLoading || !selectedEmployee}
          />
          <SendButton
            onClick={handleSendMessage}
            disabled={isLoading || !inputMessage.trim() || !selectedEmployee}
          >
            Send
          </SendButton>
        </MessageInputContainer>
      </InputContainer>
    </ChatContainer>
  );
};
