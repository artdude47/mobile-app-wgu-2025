# Academic Planner - .NET MAUI Course Tracker

A cross-platform mobile application designed to help students track academic terms, courses, and assessments. Built with **.NET MAUI** and **C#**, featuring local notifications and a relational SQLite database.

---

## Features

### Academic Management
- **Term & Course Tracking:** Manage multiple academic terms, with support for up to **six courses** per term.
- **Assessment Tracking:** Track objective and performance assessments for each course with start and end dates.
- **Detailed Notes:** Store and edit instructor contact information (email, phone) and course notes.

### Mobile Engineering
- **Native Notifications:** Android-integrated push notifications for assessment start/end dates using platform-specific permission handling.
- **Asynchronous Data Layer:** Non-blocking SQLite database operations to ensure a smooth, lag-free UI during data saves and loads.
- **Data Persistence:** All user data is locally persisted on the device, ensuring offline accessibility.

---

## Tech Stack

- **Framework:** .NET MAUI (.NET 9)
- **Language:** C#
- **Database:** SQLite (via `sqlite-net-pcl`)
- **Notifications:** Plugin.LocalNotification
- **Architecture:** Event-Driven UI with Service-Repository Pattern
- **Platform:** Android (Primary), Windows (Compatible)

---

## Architecture Highlights

### Asynchronous Database Service
Unlike basic synchronous apps, this project implements a fully asynchronous database layer using `SQLiteAsyncConnection`.
- **Why it matters:** It prevents the Main Thread (UI) from freezing during I/O operations.
- **Implementation:** All CRUD operations (e.g., `GetTermsAsync`, `SaveCourseAsync`) return `Task<T>` and are awaited by the UI.

### Platform-Specific Conditional Compilation
The app handles platform differences intelligently using preprocessor directives (`#if ANDROID`).
- **Android:** Requests specific `PostNotifications` permissions at runtime before scheduling alerts.
- **Windows:** Falls back gracefully with UI alerts for unsupported features, ensuring the app doesn't crash on different platforms.

### Relational Data Modeling
The internal SQLite database uses a relational schema to maintain data integrity:
- **Foreign Keys:** Courses are linked to Terms via `TermId`, and Assessments are linked to Courses via `CourseId`.
- **Indexed Lookups:** Fields like `CourseId` are decorated with `[Indexed]` attributes to optimize query performance.

---

## Getting Started

### 1. Prerequisites
- .NET 9 SDK
- Visual Studio 2022 (with .NET MAUI workload installed)

### 2. Installation
```bash
git clone [https://github.com/artdude47/mobile-app-wgu-2025.git](https://github.com/artdude47/mobile-app-wgu-2025.git)
cd wgu-c971
```

### 3. Running the App
1. Open `WGU.C971.sln` in Visual Studio
2. Select an **Android Emulator** (e.g., Pixel 5 - API 34) as the debug target.
3. Press **F5** to build and deploy.

*Note: The app automatically initializes the SQLite database at `FileSystem.AppDataDirectory` on the first launch.*

