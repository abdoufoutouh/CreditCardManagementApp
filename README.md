# Credit Card Management Web Application

## Project Overview

This is a full-stack web application designed to enable users to securely manage multiple credit cards through an intuitive dashboard interface. The application provides comprehensive credit card lifecycle management—from creation and activation to updates and deletion—while maintaining strict data isolation and security standards.

The system serves individual users who need a centralized platform to track, organize, and manage their credit card portfolios with real-time balance monitoring and card status visibility.

## Key Features

- **User Authentication**: Secure registration and login with JWT-based stateless authentication
- **Credit Card Management**: Full CRUD operations for credit card records with validation
- **Card Generation**: Automatic generation of valid card numbers with Luhn algorithm validation
- **User Data Isolation**: Each user can only access and manage their own credit cards
- **Search and Filter**: Advanced search capabilities by card type and card number
- **Dashboard Analytics**: Real-time statistics including total cards, expiring cards, and balance tracking
- **Responsive UI**: Professional, component-based dashboard with card preview and management interface
- **Backend Validation**: Comprehensive business rule enforcement including credit limits, balance constraints, and card uniqueness

## Global Architecture

The application follows a clean separation of concerns with a frontend-backend architecture communicating via REST API.

**Frontend** (Vue.js 3) runs independently and communicates with the backend through HTTP requests. The frontend handles user interface rendering, form validation, and state management.

**Backend** (ASP.NET Core) exposes REST API endpoints that handle business logic, data persistence, and security enforcement. All endpoints require JWT authentication except for signup and login.

**Communication**: The frontend sends JSON payloads to the backend API and receives standardized JSON responses. Authentication is stateless—the backend validates JWT tokens on each request without maintaining session state.

**Responsibility Boundaries**: The backend enforces all critical business rules and data validation. The frontend provides user experience enhancements and client-side validation for responsiveness. User ownership of data is verified on the backend for every operation.

## Backend Design Overview

The backend is built with ASP.NET Core 10 and Entity Framework Core, implementing a layered architecture that separates concerns across multiple tiers.

**Architecture Layers**:
- **Controllers**: Handle HTTP requests, validate input, extract user context from JWT claims, and return standardized API responses
- **Services**: Implement business logic including card validation, balance constraints, card limits per user, and data transformation
- **Repositories**: Manage database operations and queries, providing a clean abstraction over Entity Framework
- **Models**: Define database entities with relationships and constraints
- **DTOs**: Transfer data between layers with only necessary fields exposed to clients

**Key Design Patterns**:
- Dependency injection for loose coupling between layers
- Repository pattern for data access abstraction
- Service layer for business rule enforcement
- DTO pattern to control API contracts and prevent over-exposure of internal data

**Security Implementation**:
- JWT authentication with configurable issuer, audience, and signing key
- User ID extraction from JWT claims for every protected operation
- Backend verification that users can only access their own credit cards
- Validation of card ownership before any modification or deletion

**Business Rules Enforced**:
- Card numbers must pass Luhn algorithm validation
- Card numbers must be globally unique across all users
- Users can maintain maximum 5 active cards and 10 total cards
- Credit limit must be positive
- Current balance must be non-negative and not exceed credit limit
- Cards can only be deleted when balance is zero
- Expiration dates must be within 10 years from current date
- Blacklisted card numbers are rejected

## Frontend Design Overview

The frontend is built with Vue.js 3 using the Composition API and Vite as the build tool.

**Architecture**:
- **Pages**: Full-page components representing routes (Login, Signup, Dashboard, Create/Edit Card)
- **Components**: Reusable UI components organized by domain (forms, cards, sections, layout)
- **Composables**: Encapsulated logic for API communication and state management
- **Store**: Pinia store for centralized authentication state
- **Router**: Vue Router for client-side navigation

**State Management**:
- Pinia store manages authentication state including JWT token and user information
- Token is persisted in localStorage for session continuity
- Component-level state for form data and UI interactions
- API composables manage loading and error states

**API Integration**:
- Centralized composable for all credit card API operations
- Automatic JWT token injection in request headers
- Standardized error handling and response parsing
- Loading states for async operations

**Component Design**:
- Separation between page-level components and reusable UI components
- Props-based data flow for component composition
- Event emission for parent-child communication
- Computed properties for derived state and filtering

## Credit Card Management Flow

**Create Card**: User navigates to the create card page, enters card details (number, expiration, credit limit, initial balance), and optionally generates a valid card number. The backend validates all inputs, checks uniqueness, and enforces user card limits before persisting.

**View Cards**: Dashboard displays all user cards with last four digits visible, card type, expiration date, and current balance. Users can filter by active status or search by card type and number.

**Update Card**: Users can modify credit limit and current balance for existing cards. The backend validates that balance remains within the new limit and persists changes.

**Delete Card**: Users can delete cards only when the current balance is zero. The backend enforces this constraint and removes the card record.

**Search**: Users can search their cards by card type (Visa, Mastercard, Amex) or by card number (last four digits). Results are filtered on the backend and returned to the frontend.

## Security and Best Practices

**Authentication**: JWT tokens are issued upon successful login and required for all protected endpoints. Tokens contain user ID and email claims used to verify user identity and enforce data isolation.

**User Isolation**: Every protected endpoint extracts the user ID from the JWT token and verifies that the requested resource belongs to that user. This prevents users from accessing or modifying other users' data.

**Validation Strategy**: Frontend validation provides immediate user feedback. Backend validation enforces all business rules and security constraints. The backend never trusts frontend validation.

**Error Handling**: API responses follow a consistent structure with success flag, message, and optional data. Errors are caught at the controller level and returned with appropriate HTTP status codes and descriptive messages.

**Clean Code Principles**: Code is organized into logical layers with single responsibility. Dependencies are injected rather than created. Business logic is separated from infrastructure concerns. DTOs prevent internal model exposure.

**Data Protection**: Card numbers are stored in the database but only last four digits are returned to the frontend. Passwords are stored securely. Sensitive operations require authentication.

## Testing Approach

The application has been validated through manual testing using Postman for API endpoints and browser testing for the frontend interface.

**API Testing**: All endpoints have been tested with valid and invalid inputs to verify correct behavior, error handling, and response formats. Authentication flows have been validated to ensure JWT tokens are properly issued and validated.

**Flow Validation**: Complete user workflows have been tested including signup, login, card creation, updates, deletion, and search operations. Data isolation has been verified to ensure users cannot access other users' cards.

**Stability**: The application has been tested for consistency across multiple operations and edge cases including boundary conditions for card limits, balance constraints, and validation rules.

## Project Structure Overview

**Backend** (`backend/CreditCardAPI/`): Contains the ASP.NET Core API with controllers handling HTTP requests, services implementing business logic, repositories managing data access, models defining database schema, and DTOs controlling API contracts. Configuration files specify database connection and JWT settings.

**Frontend** (`frontend/vite-project/src/`): Contains Vue components organized by domain, pages representing routes, composables for API integration, Pinia store for state management, and router configuration. Assets and styles are organized separately.

**Separation of Concerns**: Backend and frontend are completely independent. The backend exposes a REST API that any client can consume. The frontend is a pure consumer of the API with no direct database access. This separation enables independent scaling, testing, and deployment.

## Conclusion

This project demonstrates solid engineering practices through clean architecture, proper separation of concerns, and comprehensive business rule enforcement. The layered backend design with repositories and services provides maintainability and testability. The component-based frontend with centralized state management ensures scalability and code reuse.

The application is production-ready in terms of architecture and security practices. It successfully implements user authentication, data isolation, and comprehensive validation. The codebase reflects disciplined software engineering with clear responsibility boundaries, dependency injection, and consistent error handling patterns.
