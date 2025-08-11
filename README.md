# HR Assistant Application

A modern web application that provides an AI-powered HR chat assistant for employees to query HR information, check vacation balances, find department contacts, and access HR policies.

## Architecture

- **Frontend**: React with TypeScript and styled-components
- **Backend**: C# ASP.NET Core Web API
- **Database**: RavenDB for document storage
- **Theme**: Pastel purple and green color scheme on white background

## Features

### Chat Bot Functionality
- Natural language processing for HR queries
- Vacation balance and time-off information
- Department and employee directory lookup
- HR policy document search
- Vacation request assistance

### Data Models
- **Employees**: Personal info, vacation tracking, department assignments
- **Departments**: Team structure, managers, building locations
- **Vacation Requests**: Request tracking with approval workflow
- **HR Policies**: Comprehensive policy documents in markdown format

### Sample Queries
- "How much leave do I have?"
- "Who is responsible for IT in building C?"
- "How do I file an expense report?"
- "What is the attendance policy?"
- "I want to request vacation time"

## Project Structure

```
/backend/HRAssistant/          # C# Web API
├── Controllers/               # API controllers
├── Models/                   # Data models
└── Program.cs               # Application configuration

/frontend/hr-assistant/       # React TypeScript app
├── src/
│   ├── components/          # React components
│   ├── api.ts              # API client
│   ├── theme.ts            # Design system
│   └── App.tsx             # Main application
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ and npm
- RavenDB (optional - can use RavenDB Cloud or Docker)

### Backend Setup
1. Navigate to the backend directory:
   ```bash
   cd backend/HRAssistant
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Start RavenDB (if running locally):
   - Download from https://ravendb.net/
   - Run on default port 8080
   - Or use Docker: `docker run -p 8080:8080 ravendb/ravendb`

4. Build and run the API:
   ```bash
   dotnet run
   ```

5. Seed sample data (one-time):
   ```bash
   curl -X POST http://localhost:5000/api/HR/seed
   ```

### Frontend Setup
1. Navigate to the frontend directory:
   ```bash
   cd frontend/hr-assistant
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

4. Open http://localhost:3000 in your browser

## Sample Data

The application includes comprehensive sample data:

### Employees
- Alice Johnson (IT Manager) - Building A
- Bob Smith (HR Manager) - Building B  
- Carol Wilson (Finance Manager) - Building C
- David Brown (Marketing Manager) - Building A
- John Doe (Senior Developer) - Building A

### HR Policies
- Code of Conduct Policy
- Attendance and Leave Policy
- Anti-Harassment and Discrimination Policy
- Remote Work and Telecommuting Policy

### Department Structure
- Information Technology (Building A, Floor 3)
- Human Resources (Building B, Floor 2)
- Finance (Building C, Floor 1)
- Marketing (Building A, Floor 2)

## Design Features

### Color Palette
- **Primary Purple**: #B19CD9 (soft lavender)
- **Secondary Green**: #A8D8B9 (soft mint)
- **Background**: Pure white (#FFFFFF)
- **Accent Colors**: Coordinated pastels for UI elements

### User Experience
- Modern chat interface with message bubbles
- Suggested questions for easy interaction
- Real-time typing indicators
- Responsive design for desktop and mobile
- Accessibility-focused design patterns

## API Endpoints

- `POST /api/HR/chat` - Send chat message
- `POST /api/HR/seed` - Seed sample data
- `GET /api/HR/employees` - Get all employees
- `GET /api/HR/departments` - Get all departments
- `GET /api/HR/policies` - Get all HR policies

## Technologies Used

### Backend
- ASP.NET Core 9.0
- RavenDB.Client
- C# 12.0 with minimal APIs

### Frontend  
- React 18
- TypeScript 5
- styled-components
- Axios for API calls

## Future Enhancements

- User authentication and authorization
- Real-time notifications for vacation approvals
- Advanced search and filtering
- Document upload for HR policies
- Integration with calendar systems
- Mobile application
- Multi-language support

## Development Notes

- All backend logic resides in a single controller file as requested
- Shallow architecture with no separate service layers
- Document-based storage for flexible data modeling
- Comprehensive error handling and user feedback
- Seed data includes realistic HR scenarios and policies
