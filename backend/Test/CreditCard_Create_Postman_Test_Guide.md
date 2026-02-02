# Comprehensive Credit Card Creation Testing Guide

## 🚀 Quick Start (5-Minute Setup)

### 1. Start API & Get Token
```bash
# Terminal 1: Start API
cd backend/CreditCardAPI
dotnet run

# Terminal 2: Get JWT Token (using curl)
curl -X POST "http://localhost:5249/api/Auth/register" \
-H "Content-Type: application/json" \
-d '{
  "email": "test@example.com",
  "password": "Test123!@#",
  "firstName": "Test",
  "lastName": "User"
}'

# Copy the token from the response
```

### 2. Quick Test in Postman
- **URL**: `POST http://localhost:5249/api/CreditCard`
- **Auth**: Bearer Token (paste JWT)
- **Headers**: `Content-Type: application/json`
- **Body**: 
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

---

## 📋 Detailed Prerequisites

1. **Start your ASP.NET Core API:**
   ```bash
   cd backend/CreditCardAPI
   dotnet run
   ```
   The API will be available at:
   - **HTTP**: `http://localhost:5249`
   - **HTTPS**: `https://localhost:7015`

2. **Authenticate and Get JWT Token:**
   - First, sign up or login using the Auth endpoints
   - Copy the JWT token from the response
   - You'll need this token for all credit card creation requests

---

## Endpoint Information

- **Method**: `POST`
- **URL**: `http://localhost:5249/api/CreditCard`
- **Authentication**: Required (JWT Bearer Token)
- **Content-Type**: `application/json`

---

## Postman Setup

### Step 1: Configure Authorization
1. Create a new request in Postman
2. Set method to `POST`
3. Enter URL: `http://localhost:5249/api/CreditCard`
4. Go to **Authorization** tab
5. Select **Type**: `Bearer Token`
6. Paste your JWT token in the **Token** field

### Step 2: Configure Headers
1. Go to **Headers** tab
2. Add header:
   - **Key**: `Content-Type`
   - **Value**: `application/json`

---

## Test Cases

### ✅ Test Case 1: Valid Visa Card Creation

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (201 Created):**
```json
{
  "success": true,
  "message": "Credit card created successfully",
  "data": {
    "id": 1,
    "cardNumberPartial": "1111",
    "creditLimit": 5000.00,
    "currentBalance": 0.00,
    "isActive": false,
    "message": "Credit card created successfully"
  }
}
```

**Postman Steps:**
1. Set request body to **raw** and **JSON**
2. Paste the JSON above
3. Click **Send**
4. Verify status code: `201 Created`
5. Verify response structure matches above

---

### ✅ Test Case 2: Valid Mastercard Creation

**Request Body:**
```json
{
  "cardNumber": "5555555555554444",
  "expirationDate": "2027-06-30T00:00:00",
  "creditLimit": 10000.00,
  "currantBalance": 2500.50,
  "type": "Mastercard"
}
```

**Expected Response (201 Created):**
- Status: `201 Created`
- Response should contain the created card with masked card number (last 4 digits)

---

### ✅ Test Case 3: Valid Amex Card Creation

**Request Body:**
```json
{
  "cardNumber": "378282246310005",
  "expirationDate": "2028-03-15T00:00:00",
  "creditLimit": 15000.00,
  "currentBalance": 0.00,
  "type": "Amex"
}
```

**Note:** If your system requires exactly 16 digits, use a valid 16-digit Amex number that starts with 34 or 37 and passes Luhn validation.

---

### ❌ Test Case 4: Invalid Card Number - Luhn Check Failure

**Request Body:**
```json
{
  "cardNumber": "4111111111111112",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Credit card creation failed. Please check: card number uniqueness, Luhn validation, expiration date (future, max 10 years), credit limit > 0, balance within limit, card type/prefix match, active card limit (max 5), total card limit (max 10), and ensure card number is not blacklisted.",
  "data": null
}
```

---

### ❌ Test Case 5: Invalid Card Number - Wrong Length

**Request Body:**
```json
{
  "cardNumber": "41111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Message: "Invalid input data. Please check your input fields."

---

### ❌ Test Case 6: Duplicate Card Number

**Steps:**
1. First, create a card with number `4111111111111111` (Test Case 1)
2. Try to create another card with the same number

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2027-12-31T00:00:00",
  "creditLimit": 3000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure message

---

### ❌ Test Case 7: Invalid Expiration Date - Past Date

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2020-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (expiration date must be in the future)

---

### ❌ Test Case 8: Invalid Expiration Date - More Than 10 Years

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2035-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (expiration date must be ≤ 10 years)

---

### ❌ Test Case 9: Invalid Credit Limit - Zero or Negative

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 0.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Either model validation error or business validation failure

---

### ❌ Test Case 10: Invalid Current Balance - Negative

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currantBalance": -100.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Model validation or business validation failure

---

### ❌ Test Case 11: Invalid Current Balance - Exceeds Credit Limit

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currantBalance": 6000.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (balance must be ≤ credit limit)

---

### ❌ Test Case 12: Invalid Card Type - Wrong Prefix (Visa with Mastercard Number)

**Request Body:**
```json
{
  "cardNumber": "5555555555554444",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (card type prefix mismatch)

---

### ❌ Test Case 13: Invalid Card Type - Invalid Type Value

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Discover"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Model validation error (type must be Visa, Mastercard, or Amex)

---

### ❌ Test Case 14: Blacklisted Card Number

**Request Body:**
```json
{
  "cardNumber": "1111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (card number is blacklisted)

**Other Blacklisted Numbers to Test:**
- `2222222222222222`
- `0000000000000000`
- `1234567890123456`

---

### ❌ Test Case 15: Missing Required Fields

**Request Body (Missing cardNumber):**
```json
{
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Message: "Invalid input data. Please check your input fields."

**Test with other missing fields:**
- Missing `expirationDate`
- Missing `creditLimit`
- Missing `currantBalance`
- Missing `type`

---

### ❌ Test Case 16: Invalid Format - Non-Numeric Card Number

**Request Body:**
```json
{
  "cardNumber": "4111-1111-1111-1111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Model validation error (card number must contain only digits)

---

### ❌ Test Case 17: Unauthorized Request - Missing Token

**Steps:**
1. Remove the Bearer token from Authorization tab
2. Send the request

**Expected Response (401 Unauthorized):**
- Status: `401 Unauthorized`
- Authentication required error

---

### ❌ Test Case 18: Unauthorized Request - Invalid Token

**Steps:**
1. Set an invalid/expired token in Authorization tab
2. Send the request

**Expected Response (401 Unauthorized):**
- Status: `401 Unauthorized`

---

### ⚠️ Test Case 21: Edge Case - Maximum Credit Limit

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 999999999.99,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (201 Created):**
- Should succeed if no upper limit is enforced
- Verify the large credit limit is stored correctly

---

### ⚠️ Test Case 22: Edge Case - Minimum Valid Credit Limit

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 0.01,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (201 Created):**
- Should succeed with minimum valid credit limit

---

### ⚠️ Test Case 23: Edge Case - Current Balance Equals Credit Limit

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 5000.00,
  "type": "Visa"
}
```

**Expected Response (201 Created):**
- Should succeed (balance = credit limit is typically allowed)

---

### ❌ Test Case 24: Edge Case - Credit Limit with Too Many Decimals

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.123,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Should fail if decimal precision is limited to 2 places

---

### ❌ Test Case 25: Edge Case - Future Expiration Date (Exactly 10 Years)

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2035-02-01T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (201 Created):**
- Should succeed (exactly 10 years should be valid)

---

### ❌ Test Case 26: Edge Case - Invalid Date Format

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026/12/31",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Should fail with invalid date format

---

### ❌ Test Case 27: Edge Case - Empty String Values

**Request Body:**
```json
{
  "cardNumber": "",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": ""
}
```

**Expected Response (400 Bad Request):**
- Should fail with validation errors for empty required fields

---

### ❌ Test Case 28: Edge Case - Negative Credit Limit

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": -1000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Should fail with negative credit limit validation

---

### ❌ Test Case 29: Edge Case - Special Characters in Card Type

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa@#$"
}
```

**Expected Response (400 Bad Request):**
- Should fail with invalid card type validation

---

### ❌ Test Case 30: Edge Case - SQL Injection Attempt

**Request Body:**
```json
{
  "cardNumber": "4111111111111111'; DROP TABLE CreditCards; --",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Should fail with card number format validation
- Verify no SQL injection occurred

---

## 🎯 Performance Testing

### Test Case 31: Load Testing - Rapid Sequential Requests

**Steps:**
1. Send 100 valid credit card creation requests sequentially
2. Measure response times
3. Verify all requests succeed

**Expected Results:**
- All requests should return 201 Created
- Response times should be consistent (< 2 seconds)
- No database connection issues

---

### Test Case 32: Concurrent Requests

**Steps:**
1. Send 10 valid credit card creation requests simultaneously
2. Verify all succeed without conflicts

**Expected Results:**
- All requests should return 201 Created
- No race conditions or deadlocks
- All cards should be properly stored

---

## 🔍 Security Testing

### Test Case 33: XSS Attempt in Card Type

**Request Body:**
```json
{
  "cardNumber": "4111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "<script>alert('xss')</script>"
}
```

**Expected Response (400 Bad Request):**
- Should fail with validation error
- Verify no script execution occurs

---

### Test Case 34: Very Long Card Number

**Request Body:**
```json
{
  "cardNumber": "4111111111111111111111111111111111111111",
  "expirationDate": "2026-12-31T00:00:00",
  "creditLimit": 5000.00,
  "currentBalance": 0.00,
  "type": "Visa"
}
```

**Expected Response (400 Bad Request):**
- Should fail with length validation

---

**Steps:**
1. Create 5 active credit cards (you may need to manually set `isActive = true` in database or have an activate endpoint)
2. Try to create a 6th active card

**Note:** This test requires cards to be active. If your system doesn't have an activate endpoint yet, you may need to test this by:
- Manually updating the database to set 5 cards as active
- Or wait for an activate card endpoint to be implemented

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (user already has 5 active cards)

---

### ⚠️ Test Case 20: Maximum Total Cards Limit (10 Cards)

**Steps:**
1. Create 10 credit cards for the same user
2. Try to create an 11th card

**Expected Response (400 Bad Request):**
- Status: `400 Bad Request`
- Business validation failure (user already has maximum 10 cards)

---

## Valid Test Card Numbers (Luhn Valid)

Use these numbers for testing valid scenarios:

- **Visa**: `4111111111111111`, `4012888888881881`, `4222222222222` (13 digits - adjust if needed)
- **Mastercard**: `5555555555554444`, `5105105105105100`
- **Amex**: `378282246310005` (15 digits), `371449635398431` (15 digits)

**Note:** Ensure card numbers match the required length (16 digits) and pass Luhn validation.

---

## 📋 Comprehensive Testing Checklist

### ✅ Basic Functionality Tests
- ✅ Valid Visa card creation
- [ ] Valid Mastercard creation
- [ ] Valid Amex card creation
- [ ] Invalid card number (Luhn failure)
- [ ] Invalid card number (wrong length)
- [ ] Duplicate card number
- [ ] Past expiration date
- [ ] Expiration date > 10 years
- [ ] Zero/negative credit limit
- [ ] Negative current balance
- [ ] Current balance exceeds credit limit
- [ ] Card type/prefix mismatch
- [ ] Invalid card type value
- [ ] Blacklisted card number
- [ ] Missing required fields
- [ ] Invalid card number format
- [ ] Missing authentication token
- [ ] Invalid authentication token
- [ ] Maximum active cards limit (5)
- [ ] Maximum total cards limit (10)

### ⚠️ Edge Case Tests
- [ ] Maximum credit limit (boundary test)
- [ ] Minimum valid credit limit (0.01)
- [ ] Current balance equals credit limit
- [ ] Credit limit with too many decimals
- [ ] Future expiration date (exactly 10 years)
- [ ] Invalid date format
- [ ] Empty string values
- [ ] Negative credit limit
- [ ] Special characters in card type
- [ ] SQL injection attempt

### 🎯 Performance Tests
- [ ] Load testing - 100 sequential requests
- [ ] Concurrent requests - 10 simultaneous
- [ ] Response time validation (< 2 seconds)

### 🔍 Security Tests
- [ ] XSS attempt in card type
- [ ] Very long card number (buffer overflow)
- [ ] SQL injection protection
- [ ] Authentication bypass attempts

**Total Test Cases: 34**

---

## Common Issues and Solutions

### Issue: 401 Unauthorized
**Solution:** Ensure you have a valid JWT token in the Authorization header as Bearer token.

### Issue: 400 Bad Request with "Invalid input data"
**Solution:** Check that all required fields are present and in correct format (dates, numbers, etc.).

### Issue: 400 Bad Request with business validation message
**Solution:** Review the business rules:
- Card number must be unique per user
- Card number must pass Luhn validation
- Expiration date must be in future and ≤ 10 years
- Credit limit > 0
- Balance ≥ 0 and ≤ credit limit
- Card type must match card number prefix
- Card number not blacklisted
- User limits: max 5 active, max 10 total

### Issue: Card number validation fails
**Solution:** 
- Ensure card number is exactly 16 digits
- Use a card number that passes Luhn algorithm
- Remove spaces and dashes from card number
- Ensure card type prefix matches the card number

---

## Postman Collection Tips

1. **Create a Collection:** Organize all test cases in a Postman collection
2. **Use Variables:** Store base URL and JWT token as collection variables
3. **Environment Variables:** Create environments for different API endpoints (dev, staging, prod)
4. **Tests Tab:** Add assertions to automatically verify responses
5. **Pre-request Scripts:** Automatically get and set JWT token before requests

---

## Example Postman Test Script

Add this to the **Tests** tab in Postman:

```javascript
pm.test("Status code is 201", function () {
    pm.response.to.have.status(201);
});

pm.test("Response has success true", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.success).to.eql(true);
});

pm.test("Response contains card data", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.data).to.have.property('id');
    pm.expect(jsonData.data).to.have.property('cardNumberPartial');
    pm.expect(jsonData.data.cardNumberPartial).to.have.lengthOf(4);
});
```

---

## 📊 Test Results Summary Template

Use this template to track your test results:

| Test Case | Status | Response Code | Notes |
|-----------|--------|---------------|-------|
| Valid Visa Card Creation | ✅/❌ | 201/400 | |
| Invalid Card Number (Luhn) | ✅/❌ | 400/200 | |
| Duplicate Card Number | ✅/❌ | 400/201 | |
| SQL Injection Attempt | ✅/❌ | 400/201 | |
| XSS Attempt | ✅/❌ | 400/201 | |
| Load Test (100 requests) | ✅/❌ | All 201 | Avg response time: |
| Concurrent Test (10 req) | ✅/❌ | All 201 | No conflicts: |

**Pass Rate:** ___/34 (___%)

---

## 🎯 Testing Best Practices

### 1. **Test Organization**
- Create a Postman collection with all test cases
- Use folders to group related tests (Basic, Edge Cases, Security, Performance)
- Use environment variables for base URL and JWT token

### 2. **Data Management**
- Use unique card numbers for each test to avoid conflicts
- Clean up test data after testing sessions
- Use different user accounts for isolation testing

### 3. **Automation**
- Use Postman's pre-request scripts for automatic token refresh
- Add test scripts to automatically verify responses
- Consider using Newman for CI/CD integration

### 4. **Performance Monitoring**
- Record response times for each test category
- Monitor database performance during load testing
- Check for memory leaks during extended testing

### 5. **Security Validation**
- Always verify that malicious inputs are rejected
- Check that error messages don't leak sensitive information
- Validate that all inputs are properly sanitized

---

## 🚨 Critical Test Cases (Must Pass)

These test cases are critical for production readiness:

1. **✅ Valid Card Creation** - Core functionality
2. **❌ SQL Injection** - Security vulnerability
3. **❌ XSS Attempts** - Security vulnerability  
4. **❌ Authentication Bypass** - Security vulnerability
5. **⚠️ Load Testing** - Performance and scalability
6. **⚠️ Concurrent Requests** - Data integrity
7. **❌ Duplicate Prevention** - Business logic integrity

---

## 📈 Success Criteria

Your API is production-ready when:

- **✅ All 34 test cases pass**
- **✅ Response times < 2 seconds for single requests**
- **✅ Load test completes without errors**
- **✅ No security vulnerabilities detected**
- **✅ All business rules are enforced**
- **✅ Error handling is comprehensive**
- **✅ Data integrity is maintained**

---

## 🔄 Regression Testing

After any code changes, always run:

1. **Quick Regression** (5 minutes):
   - Valid Visa card creation
   - Invalid card number (Luhn)
   - Authentication test
   - SQL injection test

2. **Full Regression** (30 minutes):
   - All 34 test cases
   - Performance testing
   - Security testing


