# AI Prompt: Create a Theater Show Planning and Seating Management Application

## Application Overview

Design and implement a comprehensive application for educative theater for elementary schools coordination. The coordinator needs to plan shows, manage seating layouts, and optimize group assignments to seats. The application should help enter valid data and generate optimal seating arrangements. They will work on 1 show at the time, but a show will have multiple showings.

## Core Requirements

### Architecture

- Implement a clean architecture pattern with clear separation of concerns:
  - Application layer: Models, Business logic and services
  - Infrastructure layer: Data access, serialization and deserialization
  - Presentation layer: User interface simple async. no MVVM pattern
- Use local files for data persistence
- Use dependency injection for loose coupling

### User Interface

- Build a modern WPF desktop application
- Create a main navigation interface with sections for:
  - Shows management
  - Groups management
  - Spaces/venues management
  - Planning tools
- Implement data validation throughout the UI
- Include loading indicators for asynchronous operations
- Provide clear feedback for user actions

## Feature Specifications

### 1. Space Management

Design a system for modeling and visualizing theater seating layouts:

- Spaces/Venues:
  - Properties: Name, Description
  - Contains multiple Sections
  - Visual editor for layout management

- Sections (e.g., Main Hall, Balcony):
  - Properties: Name, Priority (for preferred seating), IsBalcony flag
  - Contains multiple Rows
  - Sorting/ordering by priority

- Rows:
  - Properties: Name, RowNumber, StartSeatNumber, EndSeatNumber
  - Accessibility flags (HasAccessFromLeft, HasAccessFromRight)
  - Calculate SeatCount from start/end seat numbers

- Seats:
  - Properties: SeatNumber, IsOccupied
  - Track assignments and availability
  - Generate dynamically based on row configuration

- Space Editor:
  - Visual editor to manage sections and rows
  - Interactive controls for adding/editing/removing components
  - Support for zooming and panning the layout

### 2. Show Management

Implement tools for managing theater performances:

- Shows:
  - Properties: Name, List of ShowInstances
  - CRUD operations for shows

- Show Instances:
  - Properties: Date, Time, assigned Space
  - Track group assignments for each instance
  - Link to specific venue space for seating

- Interfaces:
  - List view of all shows with filtering options
  - Detail view for show instances

### 3. Group Management

Create functionality for managing attendee groups:

- Groups:
  - Properties: Name, Size, Preferences
  - Track assignments to show instances

- Group Preferences:
  - PreferFrontRow flag
  - List of preferred show days/times
  - UI for adding/removing preferred shows

- Assignments:
  - Link groups to specific show instances
  - Track assigned seats for each group
  - Maintain group cohesion in seating

### 4. Planning & Optimization

Develop algorithms for optimal seating assignment:

- Find suitable seats for a group:
  - Consider group size and preferences
  - Ensure group members sit together
  - Respect accessibility requirements

- Generate optimal distribution:
  - Assign multiple groups across show instances
  - Maximize preference satisfaction
  - Ensure efficient space utilization
  - Ensure groups only go to one show instance

- Seat assignment validation:
  - Verify group can access their seats
  - Check for seat availability
  - Maintain group integrity

### 5. Data Persistence

Implement data storage and retrieval:

- Workspace save/load functionality for projects
- based on JSON files
- Import/export capabilities

### 6. output

- PDF with a visual map of each show instance, color coded per group, and seat assignment in text summarized by row, start/end seat

## Technical Requirements

- Target .NET 9.0 framework
- Local files for data access (JSON)
- Async programming for responsive UI
- Error handling and logging

## Optional Enhancements

- Reporting functionality for bookings and occupancy
- Drag-and-drop interface for manual seat assignments
- Advanced filtering and search capabilities

## Deliverables

1. Complete source code with clean architecture
2. User documentation
3. Installation guide
4. Example workspace with sample data

The final application should provide the coordinator with an intuitive, powerful tool to design venue spaces, manage shows, and optimize group seating arrangements while respecting preferences and constraints. 
