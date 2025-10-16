# Course Tracker App

A mobile course management application developed with **.NET MAUI** for a course at WGU. The app allows students to manage academic terms, courses, and assessments while receiving local notifications for important dates.

---

## Overview
This project simulates a lightweight student course tracking system where users can:
- Manage **academic terms** with start and end dates
- Add and view up to **six courses** per term
- For each course:
  - View and edit **details** (title, dates, instructor info, status, and notes)
  - Add up to **two assessments** (one Objective and one Performance)
  - Toggle **start/end notifications** for upcoming deadlines
  - Share notes externally via system share features
- Automatically validate dates
- Persist all data locally via SQLite

---

## Features

| Feature | Description |
|----------|-------------|
| **Terms Management** | Add, edit, and delete academic terms with validation. |
| **Courses** | Add up to six courses per term with instructor info and notes. |
| **Assessments** | Add up to two assessments per course (Objective & Performance). |
| **Local Notifications** | Android notifications for course start and end dates. |
| **Data Persistence** | Uses SQLite via async CRUD operations. |
| **Sharing** | Share notes via native OS share features. |
| **Validation** | Prevents empty fields or invalid dates. |

## Tech Stack

- **Framework:** .NET 9 / .NET MAUI
- **Language:** C#
- **Database:** SQLite (via 'sqlite-net-pcl')
- **Notifications:** Plugin.LocalNotification
- **Platform Tested:** Android Emulator (Pixel 5, API 34)
- **IDE:** Visual Studio 2022 Community Edition

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with **.NET MAUI workload** installed.

### Setup
1. Clone this repository:
   ```bash
   git clone https://github.com/artdude47/mobile-app-wgu-2025.git
   cd wgu-c971
   ```
2. Open the solution in Visual Studio
3. Set the startup project to **WGU.C971** and target **Android or Windows**
4. Run the app

## Testing
Seeed data is automatically created for one term, one course, and two assessments. You can delete or modify these freely to test CRUD functions.

## Screens
- **Terms Page** - List of all academic terms and option to add new ones
- **Term Detail** - Show courses within the term
- **Course Detail** - Displays course information, instructor details, notes, and toggles for alerts
- **Assessment Detail** - Displays assessment details for the selected course



