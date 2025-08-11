# HR Assistant Application

This workspace contains a TypeScript frontend and C# backend HR assistant application using RavenDB.

## Project Structure
- `/frontend/hr-assistant` - TypeScript React application with pastel purple/green theme
- `/backend/HRAssistant` - C# Web API with RavenDB integration
- Contains HR chatbot functionality for employee queries
- Supports vacation requests, employee lookup, department info, and HR policies

## Key Features
- Employee chat interface for HR queries
- Vacation request management
- Department and employee directory
- HR policy document system
- Data seeding capabilities

## Progress Tracking
- [x] Project requirements clarified
- [x] Scaffold the project structure
- [x] Customize the project with HR-specific features
- [x] Install required extensions (not needed)
- [x] Compile the project
- [x] Create and run tasks
- [x] Launch configuration ready
- [x] Ensure documentation is complete

## Completed Features
- ✅ C# Web API with RavenDB integration
- ✅ Complete data models (Employee, Department, VacationRequest, HRPolicy)
- ✅ Single-file controller with all HR actions
- ✅ React TypeScript frontend with styled-components
- ✅ Pastel purple/green theme design
- ✅ Chat interface with message bubbles
- ✅ API client with TypeScript interfaces
- ✅ Comprehensive HR policies (10+ policies included)
- ✅ Sample data seeding functionality
- ✅ VS Code tasks and launch configurations
- ✅ Complete documentation and README

## Development Workflow

### Quick Start
1. **Start Backend**: Run task "Start Backend" or press `Ctrl+Shift+P` → "Tasks: Run Task" → "Start Backend"
2. **Start Frontend**: Run task "Start Frontend" in a separate terminal
3. **Seed Data**: Run task "Seed Database" (one-time setup)
4. **Open Browser**: Navigate to http://localhost:3000

### Available Tasks
- `Start Full Application` - Starts both backend and frontend
- `Start Backend` - Runs the C# API on port 5000
- `Start Frontend` - Runs React dev server on port 3000
- `Seed Database` - Populates RavenDB with sample data
- `Build Backend` - Compiles the C# project
- `Build Frontend` - Creates production React build

### Prerequisites
- .NET 9.0 SDK installed
- Node.js 18+ and npm installed  
- RavenDB running on localhost:8080 (or adjust connection in Program.cs)

## Project Complete ✅
The HR Assistant application is fully functional and ready for development!
