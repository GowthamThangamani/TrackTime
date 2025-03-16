# Activity Tracker Console Application

This project is a **Console-Based Activity Tracker** that monitors and logs active and idle time for users. It provides functionality to record, track, and view total active time for each day.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [How It Works](#how-it-works)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Contribution Guidelines](#contribution-guidelines)
- [Future Enhancements](#future-enhancements)

---

## Overview
The **Activity Tracker** application monitors user activity by detecting input events (e.g., mouse/keyboard) and logs active time intervals. It also tracks idle occurrences based on a configurable threshold. The application supports viewing total logged time for any given date.

---

## Features
1. **Track Active Time**:
   - Logs the duration when the user is actively using the system.
   - Stops tracking after detecting prolonged inactivity.

2. **Idle Monitoring**:
   - Detects idle time based on a specified threshold (default: 5 minutes).

3. **Time Log Reports**:
   - Displays total active time per day.

4. **Log Storage**:
   - Records all active sessions in a text file (`activity_log.txt`) for persistent storage.

---

## How It Works
1. **Activity Detection**:
   - Uses the `GetLastInputInfo` function from the `user32.dll` library to detect the time of the last input event.
   - Calculates idle time by comparing the current time with the last input time.

2. **Session Tracking**:
   - Logs active time intervals and accumulates the total duration in memory.
   - Writes session data to `activity_log.txt` for each session.

3. **Command Interface**:
   - Options to **start tracking** and **view summary reports** in a simple text-based menu.

4. **Idle Count**:
   - Keeps track of how many times the user was idle beyond the threshold.

---

## Getting Started

### Prerequisites
- **.NET Framework or .NET Core SDK**: Required to compile and run the code.
- **Windows OS**: The program relies on `user32.dll`, which is available on Windows systems.

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/ActivityTracker.git
   cd ActivityTracker
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the project:
   ```bash
   dotnet run
   ```

---

## Usage

### Start Tracking Time
1. Select option **2: Record time** from the main menu.
2. Monitor active and idle time in real-time.
3. Press **'S'** to stop tracking and log the session.

### View Total Time
1. Select option **1: Show total time taken based on date** from the main menu.
2. View daily summaries of active time.

---

## Contribution Guidelines

We welcome community contributions to enhance this project. Here’s how you can contribute:

### Reporting Issues
- Use the [GitHub Issues](https://github.com/YourUsername/ActivityTracker/issues) section to report bugs or suggest features.
- Provide detailed descriptions, steps to reproduce issues, and environment specifications.

### Contributing Code
1. Fork the repository and create a new branch:
   ```bash
   git checkout -b feature/YourFeature
   ```

2. Make your changes, ensure the code compiles, and write tests if necessary.

3. Commit changes:
   ```bash
   git commit -m "Add YourFeature"
   ```

4. Push to your fork and submit a pull request:
   ```bash
   git push origin feature/YourFeature
   ```

5. Provide a clear description of your changes in the PR.

### Suggestions for Contributions
- Add **unit tests** for validating functionality.
- Improve the UI with more detailed menus or colors.
- Extend idle detection to include **CPU or process monitoring**.
- Refactor code for modularity and readability.
- Add support for **cross-platform compatibility**.
- Implement **CSV or JSON file export** for reports.

---

## Future Enhancements
Here are potential features to improve the project:
1. **Graphical User Interface (GUI)**.
2. **Support for custom idle thresholds** via configuration files or command-line arguments.
3. **Multi-language support** for the interface.
4. **Advanced analytics**: Charts and insights on activity trends over time.
5. Cloud sync for activity logs to ensure access across devices.

---

## License
This project is licensed under the [MIT License](LICENSE).

Feel free to contribute and help make this project even better!

---

Let me know if you need any adjustments or would like to discuss enhancements further!
