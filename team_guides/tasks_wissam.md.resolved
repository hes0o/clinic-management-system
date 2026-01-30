# 🏨 Task Guide: Wissam (Reception) - Phase 1 Specs

**Mission:** Build the "Add Patient" Form.

## 📂 Your Workspace
*   **Directory**: `HealthCenter.Desktop/Features/Reception/`
*   **You Own**: `AddPatientView.axaml`, `AddPatientViewModel.cs`
*   **Forbidden**: Do NOT touch `Features/Doctor` or `Styles/`.

## 1. View Requirements (`AddPatientView`)
You need to build a UserControl with the following input fields. Use a `StackPanel` or `Grid`.

*   **Title Header**: "Add New Patient" (Large Font).
*   **Name Input**:
    *   Control: `TextBox`
    *   Watermark: "Enter full name..."
    *   Binding: Bind to a String property `NewPatientName`.
*   **Phone Input**:
    *   Control: `TextBox`
    *   Binding: Bind to `NewPatientPhone`.
*   **Age Input**:
    *   Control: `NumericUpDown`
    *   Range: 0 to 120.
*   **Action Button**:
    *   Text: "Register"
    *   Style: Use Merve's `primary` class.
    *   Command: Bind to `RegisterCommand`.

## 2. ViewModel Requirements
*   Implement `INotifyPropertyChanged` (or inherit `ViewModelBase`).
*   Ensure the "Register" button is **Disabled** if the Name is empty (Command Validation).

## 3. Acceptance Criteria
*   The UI must align Right-to-Left (FlowDirection).
*   Tabbing through fields must work in logical order (Name -> Phone -> Age -> Button).
