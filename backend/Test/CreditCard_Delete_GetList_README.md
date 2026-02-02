# Credit Card Get/List & Delete Operations

This document explains how **Get/List** and **Delete** operations for `CreditCard` are implemented in `backend/CreditCardAPI` using clean architecture.

## 1) Clean Architecture flow

```
Controller → Service → Repository → EF Core Entity
```

### Controller (API layer)
File: `backend/CreditCardAPI/Controllers/CreditCardController.cs`

- Validates incoming DTOs using `ModelState`.
- Extracts the authenticated `userId` from JWT (`ClaimTypes.NameIdentifier`).
- Delegates all business logic to the service.
- Returns responses using `ApiResponse<T>`.

### Service (Business + Mapping)
File: `backend/CreditCardAPI/Services/CreditCardService.cs`

- Enforces business rules.
- Ensures the card belongs to the authenticated user.
- Maps **Entity → DTO** and **DTO → Entity**.

### Repository (EF Core only)
File: `backend/CreditCardAPI/Repositories/CreditCardRepository.cs`

- Executes EF Core operations only:
  - Find by id
  - List by user
  - Remove entity
- No mapping and no business logic.

## 2) Get/List Credit Cards

### 2.1 List endpoint

`GET /api/CreditCard`

#### Query parameters
- `isActive` (optional): `true` / `false`
- `userId` (optional): **must match the authenticated user**, otherwise the API returns `400`.

> Note: In the current project the `CreditCard.IsActive` field is `[NotMapped]` and not persisted in DB. Filtering by `isActive` only filters the in-memory results.

#### Response (200 OK)

The controller returns `ApiResponse<CreditCardListDTO>`.

Example:

```json
{
  "success": true,
  "message": "Credit cards retrieved successfully",
  "data": {
    "cards": [
      {
        "id": 7,
        "cardNumberPartial": "5100",
        "creditLimit": 7000.00,
        "currentBalance": 900.00,
        "isActive": false,
        "message": ""
      }
    ],
    "message": "Credit cards retrieved successfully"
  }
}
```

### 2.2 Get by Id endpoint

`GET /api/CreditCard/{id}`

Returns one credit card if it exists and belongs to the authenticated user.

- On success: `200 OK`
- If not found / not owned: `400 Bad Request` (project pattern uses a generic message)

## 3) Delete Credit Card

### 3.1 Delete endpoint

`DELETE /api/CreditCard/{id}`

#### Body (`DeleteCreditCardDTO`)

```json
{
  "id": 7
}
```

Important:

- The URL `{id}` must match the body `id`.
- The card must belong to the authenticated user.

### 3.2 Delete business rule

The service rejects deletion when:

- The credit card does not exist
- The credit card does not belong to the authenticated user
- `CurrentBalance > 0`

This is implemented in:

- `CreditCardService.DeleteCreditCardAsync`

### 3.3 Success response (200 OK)

```json
{
  "success": true,
  "message": "Credit card deleted successfully",
  "data": {
    "id": 7,
    "cardNumberPartial": "5100",
    "creditLimit": 7000.00,
    "currentBalance": 0.00,
    "isActive": false,
    "message": "Credit card deleted successfully"
  }
}
```

### 3.4 Failure response (400 Bad Request)

Example:

```json
{
  "success": false,
  "message": "Credit card delete failed. Ensure the card exists, belongs to you, and has a current balance of 0.",
  "data": null
}
```

## 4) Postman testing

Import the collection:

- `backend/Test/CreditCard_Delete_GetList_Postman_Collection.json`

Set variables:

- `base_url` (example: `http://localhost:5249`)
- `jwt_token`
- `card_id`

Suggested test flow:

1. Create a card (use your existing create request/guide)
2. List cards (GET)
3. Update the card to set `currentBalance = 0`
4. Delete the card (DELETE)
