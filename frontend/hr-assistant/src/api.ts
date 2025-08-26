import axios from 'axios';
import { fetchEventSource, EventSourceMessage } from '@microsoft/fetch-event-source';

const API_BASE_URL = 'http://localhost:5258/api';

export interface ChatRequest {
  conversationId?: string;
  message: string;
  employeeId: string;
  signatures: { toolId: string; content: string }[];
}

export interface ChatResponse {
  conversationId: string;
  answer: string;
  followups: string[];
  generatedAt: string;
  documentsToSign: SignatureDocumentRequest[];
}

export interface SignatureDocumentRequest {
  toolId: string;
  documentId: string;
  title: string;
  content: string;
  version: number;
}

export interface SignDocumentRequest {
  conversationId: string;
  employeeId: string;
  toolId: string;
  documentId: string;
  confirmed: boolean;
  signatureBlob?: string;
}

export interface ChatMessage {
  id: string;
  text: string;
  isUser: boolean;
  timestamp: string;
}

export interface ChatHistoryResponse {
  conversationId: string;
  messages: ChatMessage[];
  employeeId: string;
}

export interface EmployeeDropdown {
  id: string;
  name: string;
  department: string;
  jobTitle: string;
  email: string;
}

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const hrApi = {

  chat: (
    request: ChatRequest,
    onChunk: (chunk: string) => void
  ): Promise<ChatResponse> => {
    return new Promise<ChatResponse>((resolve, reject) => {
      fetchEventSource(`${API_BASE_URL}/HumanResourcesAgent/chat`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
        onmessage(ev: EventSourceMessage) {
          if (ev.event === 'final') {
            resolve(JSON.parse(ev.data));
          } else {
            onChunk(ev.data);
          }
        },
        onerror(err: any) {
          reject(err);
        },
      });
    });
  },

  signDocument: (request: SignDocumentRequest): Promise<any> =>
    api.post('/HumanResourcesAgent/sign-document', request).then(response => response.data),

  getChatHistory: (employeeId: string): Promise<ChatHistoryResponse> =>
    api.get(`/HumanResourcesAgent/chat/today/${employeeId}`).then(response => response.data),

  getEmployeesForDropdown: (): Promise<EmployeeDropdown[]> =>
    api.get('/HumanResourcesAgent/employees/dropdown').then(response => response.data),
};