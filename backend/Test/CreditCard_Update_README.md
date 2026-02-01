# Credit Card Update Operation

This document explains (in detail) how the **Update Credit Card** feature is implemented in the backend (`backend/CreditCardAPI`) using clean architecture.

## 1) What the Update operation does

The Update operation allows an **authenticated user** to modify an existing credit card record that belongs to them.

It updates only these fields:

- **Id**
- **CardNumber**
- **ExpirationDate**
- **CreditLimit**
- **CurrentBalance**
- **Type**

It also enforces **all business rules** (Luhn, uniqueness, limits, etc.) in the **Service** layer.

## 2) Clean Architecture flow (who does what)

The project follows:

```
Controller → Service → Repository → EF Core Entity
```

### 2.1 Controller responsibilities (no business logic)

File: `backend/CreditCardAPI/Controllers/CreditCardController.cs`

The controller:

- Validates the request DTO using `ModelState` (DataAnnotations validation).
- Extracts `userId` from the JWT claim (`ClaimTypes.NameIdentifier`).
- Ensures the URL `id` matches `dto.Id`.
- Calls the service (`UpdateCreditCardAsync`) to run all business rules.
- Returns:
  - `200 OK` with `CreditCardResponseDTO` on success
  - `400 Bad Request` on validation/business rule failure
  - `401 Unauthorized` when token/user id is invalid
  - `500 InternalServerError` on unexpected exceptions

### 2.2 Service responsibilities (all business rules + mapping)

File: `backend/CreditCardAPI/Services/CreditCardService.cs`

The service:

- Loads the existing entity via repository (`FindByIdAsync`).
- Checks ownership (card belongs to the current user).
- Enforces all update business rules.
- Maps `UpdateCreditCardDTO` → `CreditCard` entity (mapping happens here only).
- Calls repository `UpdateAsync`.
- Maps `CreditCard` → `CreditCardResponseDTO`.

### 2.3 Repository responsibilities (database only)

Files:

- `backend/CreditCardAPI/Repositories/ICreditCardRepository.cs`
- `backend/CreditCardAPI/Repositories/CreditCardRepository.cs`

The repository:

- Uses EF Core to:
  - `FindByIdAsync(id)`
  - `UpdateAsync(entity)`
- Contains **no DTO mapping**.
- Contains **no business rules**.

## 3) API contract

### 3.1 Endpoint

`PUT /api/CreditCard/{id}`

### 3.2 Required headers

- `Authorization: Bearer {JWT_TOKEN}`
- `Content-Type: application/json`

### 3.3 Request body (`UpdateCreditCardDTO`)

Example:

```json
{
  "id": 1,
  "cardNumber": "4111111111111111",
  "expirationDate": "2027-12-31T00:00:00Z",
  "creditLimit": 5000.00,
  "currentBalance": 1500.00,
  "type": "Visa"
}
```

Important:

- The `id` in URL **must match** the `id` in the JSON body.
- `cardNumber` must be exactly **16 digits** (only digits, no spaces).

### 3.4 Success response (`200 OK`)

Example:

```json
{
  "success": true,
  "message": "Credit card updated successfully",
  "data": {
    "id": 1,
    "cardNumberPartial": "1111",
    "creditLimit": 5000.00,
    "currentBalance": 1500.00,
    "isActive": false,
    "message": "Credit card updated successfully"
  }
}
```

Notes:

- `cardNumberPartial` is the **last 4 digits** of the card.
- In this project, `IsActive` is currently **not persisted** (entity uses `[NotMapped]`).

### 3.5 Failure responses

#### `400 Bad Request` (DTO validation or business rule failure)

Returned when:

- DTO validation fails (`ModelState.IsValid == false`), or
- Service rejects the update due to business rules (service returns `null`).

Example:

```json
{
  "success": false,
  "message": "Credit card update failed. Please check: card ownership, card number uniqueness (except current record), Luhn validation, expiration date (future, max 10 years), credit limit > 0, balance within limit, card type/prefix match, and ensure card number is not blacklisted.",
  "data": null
}
```

#### `401 Unauthorized`

Returned when the JWT is missing/invalid or the `NameIdentifier` claim cannot be parsed.

## 4) Business rules enforced by the Service (Update)

All rules below are enforced inside `CreditCardService.UpdateCreditCardAsync`.

### Rule 1: Card must exist and belong to the current user

- The service loads the card by `Id`.
- If card does not exist or the `UserId` does not match the authenticated user, the update is rejected.

### Rule 2: Card number must be unique per user (except the current record)

- If you change the card number, the service checks for duplicates using:
  - `FindByCardNumberAndUserIdAsync(newCardNumber, userId)`
- If another card exists with the same number, the update is rejected.

### Rule 3: Card number must be 16 digits and pass Luhn validation

- Must be exactly 16 numeric characters.
- Must pass the Luhn algorithm.

Why this exists:

- Prevents storing invalid card numbers.

### Rule 4: Expiration date must be in the future and <= 10 years

- `ExpirationDate` must be strictly greater than `DateTime.UtcNow`.
- `ExpirationDate` must be less than or equal to `UtcNow + 10 years`.

### Rule 5: Credit limit must be > 0

- `CreditLimit <= 0` is rejected.

### Rule 6: CurrentBalance must be between 0 and CreditLimit

- `CurrentBalance < 0` is rejected.
- `CurrentBalance > CreditLimit` is rejected.

### Rule 7: Type must be Visa / Mastercard / Amex AND prefix must match the type

Accepted values for `type`:

- `Visa`
- `Mastercard`
- `Amex`

Prefix rules (examples):

- Visa: starts with `4` (e.g. `4xxxxxxxxxxxxxxx`)
- Mastercard: starts with `51`-`55` OR `2221`-`2720` ranges (project includes a set of allowed prefixes)
- Amex: starts with `34` or `37`

If the prefix doesn’t match the chosen type, the update is rejected.

### Rule 8: Reject blacklisted/simple numbers

The service rejects known simple/blacklisted numbers such as:

- Repeated digits: `1111111111111111`, `0000000000000000`, etc.
- Sequential: `1234567890123456`, `0123456789012345`

## 5) Where the code lives

- DTO:
  - `backend/CreditCardAPI/DTOS/UpdateCreditCardDTO.cs`
  - `backend/CreditCardAPI/DTOS/CreditCardResponseDTO.cs`
- Controller:
  - `backend/CreditCardAPI/Controllers/CreditCardController.cs`
- Service:
  - `backend/CreditCardAPI/Services/CreditCardService.cs`
  - `backend/CreditCardAPI/Services/ICreditCardService.cs`
- Repository:
  - `backend/CreditCardAPI/Repositories/CreditCardRepository.cs`
  - `backend/CreditCardAPI/Repositories/ICreditCardRepository.cs`
- Entity:
  - `backend/CreditCardAPI/Models/CreditCard.cs`

## 6) Example curl request

```bash
curl -X PUT "https://localhost:5001/api/CreditCard/1" \
  -H "Authorization: Bearer {jwt_token}" \
  -H "Content-Type: application/json" \
  -d "{\"id\":1,\"cardNumber\":\"4111111111111111\",\"expirationDate\":\"2027-12-31T00:00:00Z\",\"creditLimit\":5000.00,\"currentBalance\":1500.00,\"type\":\"Visa\"}"
```

## 7) Testing

Use the Postman collection (in the same `backend/Test` folder) to test:

- Successful update
- DTO validation failures
- Luhn failures
- Prefix/type mismatch
- Past expiration date
- Balance exceeding limit
- Unauthorized access
