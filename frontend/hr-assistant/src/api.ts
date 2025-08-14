import axios from 'axios';

const API_BASE_URL = 'http://localhost:5258/api';


export interface ChatRequest {
  chatId?: string;
  message: string;
  employeeId: string;
  signatures: { toolId: string; content: string }[];
}

export interface RequiredAction {
  name: string;
  toolId: string;
  payload?: any;
}

export interface ChatResponse {
  chatId: string;
  answer: string;
  followups: string[];
  generatedAt: string;
  requiredActions: RequiredAction[];
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
  chatId: string;
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
  chatId: string;
  messages: ChatMessage[];
  employeeId: string;
}

export interface Employee {
  id: string;
  name: string;
  department: string;
  employmentType: string;
  hireDate: string;
  criticalRole: boolean;
  jobTitle: string;
  email: string;
  building: string;
  vacation: VacationInfo;
  signedDocuments: SignedDocument[];
}

export interface EmployeeDropdown {
  id: string;
  name: string;
  department: string;
  jobTitle: string;
  email: string;
}

export interface VacationInfo {
  annualEntitlement: number;
  accruedDays: number;
  carryOverDays: number;
  balance: number;
  cap: number;
  history: VacationHistory[];
}

export interface VacationHistory {
  year: number;
  usedDays: number;
  carryOverUsed: number;
  requests: VacationRequestInfo[];
}

export interface VacationRequestInfo {
  id: string;
  startDate: string;
  endDate: string;
  days: number;
  reason: string;
  status: string;
  replacement?: Replacement;
  submittedDate: string;
  approvedDate?: string;
  approvedBy?: string;
}

export interface Replacement {
  employeeId: string;
  name: string;
}

export interface Department {
  id: string;
  name: string;
  manager: string;
  managerId: string;
  building: string;
  floor: string;
  description: string;
  responsibleFor: string[];
}

export interface HRPolicy {
  id: string;
  title: string;
  category: string;
  content: string;
  lastUpdated: string;
  updatedBy: string;
  version: number;
  tags: string[];
}

export interface SignatureDocument {
  id: string;
  title: string;
  type: string;
  content: string;
  version: number;
  createdDate: string;
  createdBy: string;
  lastUpdated?: string;
  updatedBy?: string;
  isActive: boolean;
  requiresSignature: boolean;
  tags: string[];
  description?: string;
  expirationDays?: number;
}

export interface SignedDocument {
  documentId: string;
  documentTitle: string;
  documentVersion: number;
  signedDate: string;
  signatureAttachmentName?: string;
  signedBy: string;
  signatureMethod?: string;
  expirationDate?: string;
  notes?: string;
}

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const hrApi = {
  chat: (request: ChatRequest): Promise<ChatResponse> =>
    api.post('/HumanResourcesAgent/chat', request).then(response => response.data),

  signDocument: (request: SignDocumentRequest): Promise<any> =>
    api.post('/HumanResourcesAgent/sign-document', request).then(response => response.data),

  getChatHistory: (employeeId: string): Promise<ChatHistoryResponse> =>
    api.get(`/HumanResourcesAgent/chat/today/${employeeId}`).then(response => response.data),

  seedData: (): Promise<any> =>
    api.post('/HumanResourcesAgent/seed').then(response => response.data),

  getEmployees: (): Promise<Employee[]> =>
    api.get('/HumanResourcesAgent/employees').then(response => response.data),

  getEmployeesForDropdown: (): Promise<EmployeeDropdown[]> =>
    api.get('/HumanResourcesAgent/employees/dropdown').then(response => response.data),

  getDepartments: (): Promise<Department[]> =>
    api.get('/HumanResourcesAgent/departments').then(response => response.data),

  getPolicies: (): Promise<HRPolicy[]> =>
    api.get('/HumanResourcesAgent/policies').then(response => response.data),

  getSignatureDocuments: (): Promise<SignatureDocument[]> =>
    api.get('/HumanResourcesAgent/signature-documents').then(response => response.data),

  getSignatureDocument: (documentId: string): Promise<SignatureDocument> =>
    api.get(`/HumanResourcesAgent/signature-documents/${documentId}`).then(response => response.data),

  getEmployeeSignedDocuments: (employeeId: string): Promise<SignedDocument[]> =>
    api.get(`/HumanResourcesAgent/employees/${employeeId}/signed-documents`).then(response => response.data),

  seedSignatureDocuments: (): Promise<any> =>
    api.post('/HumanResourcesAgent/signature-documents').then(response => response.data),
};