# 🩺 Task Guide: Ela (Doctor) - Phase 1 Specs

**Mission:** Build the Diagnosis Interface.

## 📂 Your Workspace
*   **Directory**: `HealthCenter.Desktop/Features/Doctor/`
*   **You Own**: `DiagnosisView.axaml`, `DiagnosisViewModel.cs`
*   **Forbidden**: Do NOT touch `Features/Reception`.

## 1. View Layout (`DiagnosisView`)
The screen must be divided into two main areas (Split View):

### Left Panel (Patient Context)
*   **Background**: Light Gray (distinct from main area).
*   **Content**:
    *   Patient Name (Bold, Large).
    *   Age / Gender.
    *   Button: "View History".

### Right Panel (Consultation)
*   **Symptom Entry**:
    *   Control: `TextBox`
    *   Height: Large (multi-line).
    *   Binding: `SymptomsText`.
*   **Prescription Entry**:
    *   Control: `TextBox` (or a List for advanced version).
    *   Binding: `PrescriptionText`.
*   **Actions**:
    *   Button: "Save & Finish" (Bottom Right alignment).

## 2. Functional Rules
*   The "Save" button should clear the inputs after saving.
*   The text boxes must support Arabic text entry naturally.

## 3. Acceptance Criteria
*   The layout must use a `Grid` with Column Definitions (e.g., `1*, 3*`).
*   The "Patient Context" sidebar must remain visible while scrolling the notes (if we add scrolling).
