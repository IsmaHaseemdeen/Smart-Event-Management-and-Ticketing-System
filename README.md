# Smart Event Management and Ticketing System

## Project Overview

The Smart Event Management and Ticketing System is a full-stack web application developed using **ASP.NET Core MVC, C#, Microsoft SQL Server, and Entity Framework Core**.

The project addresses the digital event management requirements of a metropolitan cultural council, providing a centralized platform for discovering cultural events, booking tickets, managing event operations, and verifying attendee admission.

The system supports four distinct user roles — **Guest, Member, Staff, and Administrator** — with role-based functionality and access restrictions.

**Project Type:** Full-Stack Web Application  
**Architecture:** Model-View-Controller (MVC)  
**Backend:** C# and ASP.NET Core MVC  
**Database:** Microsoft SQL Server  
**ORM:** Entity Framework Core  
**Frontend:** HTML, CSS, Bootstrap, and JavaScript  
**Status:** Academic web application project

---

## 1. Business Problem

Cultural organizations require efficient systems to manage events, ticket availability, reservations, and attendee verification.

Traditional or fragmented event management processes can create operational challenges, including:

- Inefficient event administration
- Difficulty managing venues and schedules
- Limited visibility into ticket availability
- Manual booking and attendance verification
- Inconsistent communication with attendees
- Difficulty gathering and moderating event feedback

This project provides a centralized web application designed to address these challenges.

## 2. Application Features

### Guest Module

Guests can:

- Browse available and upcoming events
- Search events by category, city, venue, and date
- View event information and approved reviews
- Submit general inquiries
- Register for a member account

Ticket pricing and booking functionality are restricted to authenticated members.

### Member Module

Registered members can:

- Register and sign in
- Browse and filter cultural events
- View event details and available ticket types
- Select ticket quantities
- Create ticket bookings
- View booking and payment statuses
- Access booking history
- Update permitted profile information
- Submit reviews for attended events
- Send inquiries to administrators

### Administrator Module

Administrators can:

- Access the administrative dashboard
- Create, update, and manage events
- Manage venues and event categories
- Manage event sessions and availability
- Moderate user-submitted reviews
- Review and respond to inquiries
- Manage relevant administrative operations

### Staff Module

Staff members can:

- Access event check-in functionality
- Verify attendee booking information
- Validate booking codes
- Confirm identifying phone-number digits
- Check booking and payment eligibility
- Record verified attendance

The application separates these responsibilities through role-based access control.

---

## 3. Technologies and Tools

| Technology | Purpose |
|---|---|
| C# | Backend programming and application logic |
| ASP.NET Core MVC | Web application development |
| Entity Framework Core | Object-relational mapping and data access |
| Microsoft SQL Server | Relational database management |
| SQL Server Management Studio | Database administration and testing |
| Oracle SQL Developer Data Modeler | Logical and relational database design |
| HTML | Interface structure |
| CSS | Interface styling |
| Bootstrap | Responsive UI components |
| JavaScript | Client-side interactions |
| Visual Studio | Application development environment |

---

## 4. System Architecture

The application follows a three-tier architecture.

### Presentation Layer

ASP.NET MVC views, HTML, CSS, Bootstrap, and JavaScript provide the user-facing interfaces for event browsing, booking, administration, and ticket verification.

### Application Logic Layer

ASP.NET MVC controllers handle requests, perform business operations, apply validation rules, and enforce role-based permissions.

### Data Layer

Microsoft SQL Server stores event records, users, bookings, payments, reviews, inquiries, and related information.

Entity Framework Core connects the application to the database and manages entity relationships.

---

## 5. Database Design and Implementation

The database was designed using Oracle SQL Developer Data Modeler and implemented using Microsoft SQL Server.

The project includes:

- Entity-Relationship Diagram (ERD)
- Logical and relational database models
- Data dictionary
- Primary and foreign keys
- Unique and check constraints
- Entity Framework Core models
- Database migrations
- Sample data seeding
- SQL INSERT statements
- SQL SELECT queries and validation

### Main Database Entities

1. User
2. Venue
3. Category
4. Event
5. Event Category
6. Event Session
7. Ticket Type
8. Booking
9. Booked Ticket
10. Payment
11. Review
12. Inquiry

### Entity-Relationship Diagram

![Database ER Diagram](images/database-erd.png)

### Relational Database Model

![Relational Database Model](images/database-relational-model.png)

---

## 6. Application Screenshots

The following screenshots demonstrate the interfaces developed for the project.

### Homepage and Upcoming Events

![Application Homepage](images/homepage.png)

### Event Browsing and Filtering

![Event Browsing](images/events.png)

### Ticket Booking

![Ticket Booking](images/ticket-booking.png)

### Booking Confirmation

![Booking Confirmation](images/booking-confirmation.png)

### Administrator Dashboard

![Administrator Dashboard](images/admin-dashboard.png)

### Staff Ticket Verification

![Staff Check-in Verification](images/staff-checkin.png)

---

## 7. Ticket Booking Workflow

The application provides a structured ticket booking process.

1. A member signs in.
2. The member browses available events.
3. An event and session are selected.
4. Available ticket types and prices are displayed.
5. The member selects the required ticket quantities.
6. The application validates availability and calculates the booking amount.
7. A booking record and unique booking code are generated.
8. Booking and payment status information can be reviewed.
9. Eligible bookings can be verified by authorized event staff.

**Payment limitation:** The academic application demonstrates booking and payment-status workflows but does not integrate a live payment gateway.

---

## 8. Authentication and Authorization

The system provides role-based authentication and access control.

| User Role | Primary Permissions |
|---|---|
| Guest | Browse events, read approved reviews, submit inquiries, register |
| Member | Book tickets, manage bookings, submit reviews, update profile |
| Staff | Verify attendee check-ins |
| Administrator | Manage events, venues, categories, reviews, and inquiries |

This design separates public-facing functionality from restricted member, staff, and administrator operations.

---

## 9. Database Operations

The project demonstrates practical database development and manipulation.

### Database Implementation

- Relational table creation
- Referential integrity constraints
- Entity relationships
- Database migrations
- Seed data generation

### SQL Operations

- Creating and updating database records
- Inserting sample data
- Retrieving event and venue information
- Retrieving member booking records
- Checking ticket availability
- Reviewing payment statuses
- Retrieving approved event reviews
- Identifying unanswered inquiries

---

## 10. Testing and Validation

The academic project documentation includes functional testing, database testing, and test cases.

Testing scenarios cover:

- User registration and authentication
- Event browsing and search
- Ticket selection and booking
- Ticket availability validation
- Booking and payment statuses
- Review eligibility and submission
- Guest inquiry submission
- Administrative event management
- Staff ticket verification
- Database integrity and constraints

The testing documentation provides evidence of development-time validation. The repository does not claim production-level security or load testing.

---

## 11. Project Structure

The existing repository follows the ASP.NET MVC project structure.

- `Areas/Identity/Pages/` — identity-related pages
- `Controllers/` — request handling and application logic
- `Data/` — database context and data-related classes
- `Migrations/` — Entity Framework Core database migrations
- `Models/` — domain and application models
- `Properties/` — development configuration
- `Views/` — Razor views and page layouts
- `wwwroot/` — static CSS, JavaScript, images, and other assets
- `Program.cs` — application startup and configuration
- `appsettings.json` — application configuration
- `EventTicketingSystem.csproj` — .NET project definition
- `EventTicketingSystem.slnx` — Visual Studio solution

---

## 12. Local Setup

### Prerequisites

- .NET SDK compatible with the project target framework
- Microsoft SQL Server
- Visual Studio or another compatible .NET development environment

### Running the Application

1. Clone the repository.
2. Open the solution in Visual Studio or navigate to the project directory using a terminal.
3. Restore the project dependencies.
4. Configure a local SQL Server database connection in the application's local configuration.
5. Apply the Entity Framework Core migrations according to the project's configuration.
6. Build and run the ASP.NET Core application.

Basic terminal commands:

```bash
dotnet restore
dotnet build
dotnet run
```

Database initialization requires an appropriate local SQL Server connection and the EF Core migration tooling used by the project.

Do not use production credentials or publicly committed database passwords.

---

## 13. Skills Demonstrated

### Full-Stack Web Development

- C# Programming
- ASP.NET Core MVC
- Backend Development
- HTML, CSS, and JavaScript
- Bootstrap
- Authentication and Authorization
- Role-Based Access Control
- Form Validation

### Database Development

- Microsoft SQL Server
- Entity Framework Core
- Database Schema Design
- Entity-Relationship Modelling
- Database Normalization
- SQL Queries
- Data Seeding
- Database Migrations
- Primary Key and Foreign Key Constraints

### Software Engineering

- Requirements Analysis
- MVC Architecture
- Modular Application Design
- Business Logic Implementation
- Functional Testing
- Database Testing
- Debugging
- Technical Documentation

---

## 14. Project Documentation

The academic project report documents the system's requirements, database models, implementation, application interfaces, testing, and development reflections.

[View Project Documentation](docs/Smart_Event_Management_Report.pdf)

---

## 15. Academic Context

This project was developed as an academic coursework application based on a metropolitan cultural council's event management and ticketing requirements.

It demonstrates the application of full-stack web development, relational database design, MVC architecture, role-based access control, and software testing in an integrated software project.

## Disclaimer

This is an academic demonstration project and is not a production ticketing platform.

Live payment gateway integration and production deployment are outside the demonstrated project scope. Sample events, accounts, and booking records are intended for testing and demonstration.
