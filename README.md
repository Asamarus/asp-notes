# AspNotes

This is a work in progress note-taking application. This will be an new version of [Notes](https://github.com/Asamarus/notes) that will use ASP.Net Core and .Net 8 for the back-end instead of Node.js

### Building and running with Docker

`docker compose up --build` \
Application will be available at http://localhost:8080

## Back-end NuGet Packages

- **Entity Framework Core**: For object-relational mapping, specifically `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`.
- **HtmlAgilityPack**: To parse and manipulate HTML documents.
- **SqlKata**: An SQL query builder, along with `SqlKata.Execution` for executing queries.
- **Ardalis.GuardClauses**: For guarding against improper inputs.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: To support JWT bearer authentication.
- **Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore**: For EF Core diagnostics.
- **Serilog.AspNetCore**: For logging in ASP.NET Core applications.
- **Swashbuckle.AspNetCore**: To generate Swagger documents for API documentation.

## Front-end NPM Packages

- **React** and **React DOM**: For building the UI components.
- **@mantine/**: A suite of Mantine packages for UI design, including `@mantine/core`, `@mantine/hooks`, and more for charts, notifications, and date pickers.
- **@hello-pangea/dnd**: Drag and drop library for React.
- **lodash**: A modern JavaScript utility library delivering modularity, performance, & extras.
- **react-router-dom**: For routing in React applications.
- **react-hook-form**: For managing forms and validation.
- **react-icons**: To include icons from popular icon packs.
- **zustand**: For state management

### Testing Libraries

- **@testing-library/dom**: Provides a set of helpers to test DOM nodes.
- **@testing-library/jest-dom**: Offers custom Jest matchers to test the state of the DOM.
- **@testing-library/react**: Allows testing React components in a more user-centric way.
- **@testing-library/user-event**: Simulates user events for testing purposes.

### Build and Development Tools

- **vite**: A modern build tool for faster and leaner development experiences.
- **vitest**: A Vite-native test runner with a focus on performance and developer experience.
- **vite-plugin-mkcert**: Vite plugin for HTTPS development with locally trusted certificates.

## End-to-End Testing

[Playwright](https://playwright.dev/) is used for end-to-end testing
