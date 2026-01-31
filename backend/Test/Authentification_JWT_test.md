# Postman Testing Guide for JWT Authentication

## Prerequisites
1. Start your ASP.NET Core API:
   ```bash
   cd backend/CreditCardAPI
   dotnet run
   ```
2. The API will be available at:
   - **HTTP**: `http://localhost:5249`
   - **HTTPS**: `https://localhost:7015`

---

## Step 1: Test User Signup (Register)

### Request Setup:
- **Method**: `POST`
- **URL**: `http://localhost:5249/api/auth/signup`
- **Headers**:
  - `Content-Type: application/json`

### Request Body (raw JSON):
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

### Expected Response (201 Created):
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 1,
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "expiresAt": "2024-01-31T15:30:00Z"
  }
}
```

### Postman Steps:
1. Create a new request
2. Set method to **POST**
3. Enter URL: `http://localhost:5249/api/auth/signup`
4. Go to **Headers** tab, add:
   - Key: `Content-Type`
   - Value: `application/json`
5. Go to **Body** tab
6. Select **raw** and choose **JSON** from dropdown
7. Paste the JSON body above
8. Click **Send**

### Error Cases to Test:
- **400 Bad Request**: Missing required fields or invalid email format
- **409 Conflict**: Email already exists (try signing up with the same email twice)

---

## Step 2: Test User Login

### Request Setup:
- **Method**: `POST`
- **URL**: `http://localhost:5249/api/auth/login`
- **Headers**:
  - `Content-Type: application/json`

### Request Body (raw JSON):
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

### Expected Response (200 OK):
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 1,
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "expiresAt": "2024-01-31T15:30:00Z"
  }
}
```

### Postman Steps:
1. Create a new request
2. Set method to **POST**
3. Enter URL: `http://localhost:5249/api/auth/login`
4. Go to **Headers** tab, add:
   - Key: `Content-Type`
   - Value: `application/json`
5. Go to **Body** tab
6. Select **raw** and choose **JSON** from dropdown
7. Paste the JSON body above (use the email/password from signup)
8. Click **Send**
9. **IMPORTANT**: Copy the `token` from the response - you'll need it for authenticated requests!

### Error Cases to Test:
- **400 Bad Request**: Missing email or password
- **401 Unauthorized**: Wrong email or password

---

## Step 3: Test User Logout

### Request Setup:
- **Method**: `POST`
- **URL**: `http://localhost:5249/api/auth/logout`
- **Headers**: None required (no body needed)

### Expected Response (200 OK):
```json
{
  "success": true,
  "message": "Logout successful. Please discard your token on the client side.",
  "data": null
}
```

### Postman Steps:
1. Create a new request
2. Set method to **POST**
3. Enter URL: `http://localhost:5249/api/auth/logout`
4. Click **Send** (no body or special headers needed)

---

## Step 4: Using JWT Token for Authenticated Requests

### How to Use the Token:
After login, you'll receive a JWT token. To use it in subsequent requests:

1. **Copy the token** from the login/signup response
2. In Postman, go to the **Authorization** tab
3. Select **Bearer Token** from the Type dropdown
4. Paste your token in the **Token** field
5. OR manually add it in Headers:
   - Key: `Authorization`
   - Value: `Bearer YOUR_TOKEN_HERE`

### Example: Testing a Protected Endpoint
If you have protected endpoints that require authentication:

- **Method**: `GET` (or any method)
- **URL**: `http://localhost:5249/api/your-protected-endpoint`
- **Headers**:
  - `Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

---

## Postman Collection Setup (Optional)

### Create a Postman Environment:
1. Click **Environments** → **+** to create new
2. Add variables:
   - `base_url`: `http://localhost:5249`
   - `token`: (leave empty, will be set automatically)

### Using Environment Variables:
- URL: `{{base_url}}/api/auth/login`
- Token: `{{token}}`

### Auto-save Token Script (Tests Tab):
After login request, add this in the **Tests** tab:
```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.data && jsonData.data.token) {
        pm.environment.set("token", jsonData.data.token);
        console.log("Token saved to environment");
    }
}