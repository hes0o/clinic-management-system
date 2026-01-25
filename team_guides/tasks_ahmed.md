# 🛡️ Task Guide: Ahmed (Ops) - Phase 1 Specs

**Mission:** Implement Robust Logging (So we know why it crashed).

## 📂 Your Workspace
*   **Directory**: `HealthCenter.Desktop/Infrastructure/` (Create this folder)
*   **You Own**: `LoggingConfig.cs`, `BackupService.cs`
*   **File Config**: You will modify `Program.cs` to inject logging.

## 1. Library Requirement
*   Use **Serilog**. It is the industry standard for .NET Structured Logging.

## 2. Logging Configuration Rules
*   **Console Sink**: All logs must appear in the Debug output (for devs).
*   **File Sink**:
    *   Path: `Logs/healthcenter-.log`
    *   Rolling Interval: **Daily** (Create a new file every day).
    *   Retention: Keep logs for **30 days**, then delete old ones.

## 3. Crash Handling
*   Wrap the application startup (`BuildAvaloniaApp`) in a `try/catch` block.
*   If a fatal exception occurs (App Crash), it **must** be written to the log file as "Fatal".

## 4. Acceptance Criteria
*   Start the app.
*   Verify a `Logs` folder is created.
*   Verify the log file contains the timestamp and the message "Application Starting...".
