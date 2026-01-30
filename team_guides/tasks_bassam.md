# 🧠 Task Guide: Bassam (Logic Core) - Phase 1 Specs

**Mission:** Design the Data Layer.

## 📂 Your Workspace
*   **Primary Directory**: `HealthCenter.Desktop/Database`
*   **Secondary Directory**: `HealthCenter.Desktop/Features/Shared`
*   **Forbidden**: Do NOT touch the `Views` or `Styles` folders.

## 1. Entity Specifications: `Patient.cs`
Create a class representing a Patient. It **must** have these exact property names (case-sensitive) so the frontend team can bind to them:

| Property Name | Type | Constraints |
|---|---|---|
| `Id` | `Guid` | Initialize with `Guid.NewGuid()` |
| `FullName` | `string` | Cannot be null/empty |
| `PhoneNumber` | `string` | - |
| `Age` | `int` | - |
| `CreatedAt` | `DateTime`| Default to `DateTime.Now` |
| `MedicalHistory`| `string` | Default to empty string |

## 2. Database Context Rules
*   **Context Class Name**: `AppDbContext`
*   **Table Name**: `Patients`
*   **Database Engine**: SQLite
*   **File Location**: The database file (`healthcenter.db`) must be saved in the User's **LocalApplicationData** folder (e.g., `%AppData%`). **Do not** save it in the program folder (it will fail on installed machines).

## 3. Acceptance Criteria
*   You must provide a method (or script) that creates the database file automatically if it doesn't exist on startup.
