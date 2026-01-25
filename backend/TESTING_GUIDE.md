# 🧪 Patient Management API - Testing Guide

## Quick Start

### 1. Start the Application
```bash
cd HealthCenter.API
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
      Now listening on: http://localhost:5000
```

### 2. Open Swagger UI
Navigate to: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

---

## 📋 Test Scenarios

### ✅ Scenario 1: Create a Patient (POST)

**Endpoint**: `POST /api/patients`

**Steps**:
1. Expand the POST endpoint in Swagger
2. Click "Try it out"
3. Enter request body:
```json
{
  "fullName": "John Doe",
  "phone": "+1234567890"
}
```
4. Click "Execute"

**Expected Response**: `201 Created`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "John Doe",
  "phone": "+1234567890"
}
```

---

### ✅ Scenario 2: Get All Patients (GET)

**Endpoint**: `GET /api/patients`

**Steps**:
1. Expand the GET endpoint in Swagger
2. Click "Try it out"
3. Click "Execute"

**Expected Response**: `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "John Doe",
    "phone": "+1234567890"
  }
]
```

**If no patients exist**: Returns empty array `[]`

---

### ✅ Scenario 3: Validation Error (POST)

**Endpoint**: `POST /api/patients`

**Test Case 1: Missing FullName**
```json
{
  "fullName": "",
  "phone": "+1234567890"
}
```

**Expected Response**: `400 Bad Request`
```json
{
  "error": "Validation error: Name is required"
}
```

**Test Case 2: Invalid Phone (too short)**
```json
{
  "fullName": "John Doe",
  "phone": "123"
}
```

**Expected Response**: `400 Bad Request`
```json
{
  "error": "Validation error: Phone number must be at least 10 digits"
}
```

**Test Case 3: Duplicate Phone**
1. Create a patient with phone "+1234567890"
2. Try to create another patient with the same phone

**Expected Response**: `400 Bad Request`
```json
{
  "error": "A patient with this phone number already exists"
}
```

---

### ✅ Scenario 4: Get Patient by ID (GET)

**Endpoint**: `GET /api/patients/{id}`

**Steps**:
1. First, create a patient and note the returned ID
2. Expand the GET by ID endpoint
3. Click "Try it out"
4. Enter the patient ID
5. Click "Execute"

**Expected Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "John Doe",
  "phone": "+1234567890"
}
```

**If ID doesn't exist**: `404 Not Found`
```json
{
  "error": "Patient not found"
}
```

---

### ✅ Scenario 5: Update Patient Contact (PUT)

**Endpoint**: `PUT /api/patients/{id}/contact`

**Steps**:
1. Create a patient and note the ID
2. Expand the PUT endpoint
3. Click "Try it out"
4. Enter the patient ID
5. Enter new phone number:
```json
{
  "phone": "+9876543210"
}
```
6. Click "Execute"

**Expected Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "John Doe",
  "phone": "+9876543210"
}
```

---

### ✅ Scenario 6: Delete Patient (DELETE)

**Endpoint**: `DELETE /api/patients/{id}`

**Steps**:
1. Create a patient and note the ID
2. Expand the DELETE endpoint
3. Click "Try it out"
4. Enter the patient ID
5. Click "Execute"

**Expected Response**: `204 No Content` (empty response body)

**Verify deletion**:
- Try to GET the same patient by ID
- Should return `404 Not Found`

---

## 🎯 Complete Test Flow

### End-to-End Test Sequence:

1. **GET /api/patients** → Should return empty array `[]`

2. **POST /api/patients** → Create "John Doe"
   ```json
   {
     "fullName": "John Doe",
     "phone": "+1234567890"
   }
   ```
   → Returns 201 with patient data

3. **POST /api/patients** → Create "Jane Smith"
   ```json
   {
     "fullName": "Jane Smith",
     "phone": "+9876543210"
   }
   ```
   → Returns 201 with patient data

4. **GET /api/patients** → Should return array with 2 patients

5. **GET /api/patients/{john-id}** → Should return John's data

6. **PUT /api/patients/{john-id}/contact** → Update John's phone
   ```json
   {
     "phone": "+1111111111"
   }
   ```
   → Returns 200 with updated data

7. **GET /api/patients/{john-id}** → Verify phone was updated

8. **DELETE /api/patients/{jane-id}** → Delete Jane
   → Returns 204

9. **GET /api/patients** → Should return array with 1 patient (John only)

10. **POST /api/patients** → Try to create duplicate phone
    ```json
    {
      "fullName": "Another Person",
      "phone": "+1111111111"
    }
    ```
    → Returns 400 (duplicate phone error)

---

## 🔍 Validation Rules

### FullName Validation:
- ✅ Required (not null or empty)
- ✅ Minimum 2 characters
- ✅ Trimmed automatically

### Phone Validation:
- ✅ Required (not null or empty)
- ✅ Minimum 10 characters
- ✅ Must be unique (no duplicates)
- ✅ Trimmed automatically

---

## 📊 Expected HTTP Status Codes

| Endpoint | Success | Validation Error | Not Found | Server Error |
|----------|---------|------------------|-----------|--------------|
| POST /api/patients | 201 Created | 400 Bad Request | - | 500 Internal Server Error |
| GET /api/patients | 200 OK | - | - | 500 Internal Server Error |
| GET /api/patients/{id} | 200 OK | 400 Bad Request | 404 Not Found | - |
| PUT /api/patients/{id}/contact | 200 OK | 400 Bad Request | 404 Not Found | - |
| DELETE /api/patients/{id} | 204 No Content | 400 Bad Request | 404 Not Found | - |

---

## 🐛 Troubleshooting

### Issue: Port already in use
**Solution**: Change port in `launchSettings.json` or kill the process using the port

### Issue: Swagger UI not loading
**Solution**: Ensure you're in Development environment and Swagger is configured in Program.cs

### Issue: 500 Internal Server Error
**Solution**: Check application logs for detailed error messages

### Issue: Validation not working
**Solution**: Ensure request body matches the DTO structure exactly

---

## ✅ Definition of Done Checklist

After testing, verify:
- [ ] POST /api/patients creates a patient successfully (201 Created)
- [ ] GET /api/patients returns a list of patients (200 OK)
- [ ] Empty list returns 200 OK with empty array
- [ ] Invalid data returns 400 Bad Request with error message
- [ ] Duplicate phone returns 400 Bad Request
- [ ] Domain entity invariants are preserved (no public setters)
- [ ] Swagger shows all endpoints correctly
- [ ] All endpoints tested via Swagger UI

---

## 🎉 Success Criteria

**The feature is complete when**:
1. All test scenarios pass
2. Validation works correctly
3. Error messages are clear and helpful
4. Swagger documentation is accurate
5. No build errors or warnings
6. Code follows SOLID principles

**Status**: ✅ READY FOR TESTING
