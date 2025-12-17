# OBCS (OpenBioCard Server) API Documentation

**Data Format**: `application/json`

## Table of Contents

1. [General Information](#1-general-information)
2. [Classic Authentication Endpoints](#2-classic-authentication-endpoints)
3. [Classic User Endpoints](#3-classic-user-endpoints)
4. [Classic Administrator Endpoints](#4-classic-administrator-endpoints)
5. [Classic Data Model Reference](#5-classic-data-model-reference)

---

## 1. General Information

### Naming Convention
- **Classic Endpoints** (`/classic/...`):
  - JSON request and response fields use **lower camelCase** (e.g., `userName` -> `username`).
- **Other Endpoints** (if any):
  - Default to **PascalCase** (e.g., `UserName`), unless otherwise specified.

### Authentication Methods
- **Header Method**: Include `Authorization: Bearer <your_token>` in the HTTP request header.
- **Body Method**: Some Classic endpoints (e.g., account deletion, admin operations) require a `token` field in the JSON request body.

### Error Handling
- **200 OK**: Request succeeded.
- **400 Bad Request**: Invalid request parameters or validation failure.
- **401 Unauthorized**: Invalid, expired token or username mismatch.
- **403 Forbidden**: Insufficient permissions (e.g., non-admin attempting admin actions, or blocked by firewall).
- **404 Not Found**: Resource (user or profile) does not exist.
- **409 Conflict**: Resource conflict (e.g., username already exists).
- **429 Too Many Requests**: Rate limit exceeded.
- **500 Internal Server Error**: Internal server error.

---

## 2. Classic Authentication Endpoints

### Initialize Admin (First Installation)
If no users exist in the system, this endpoint creates a default admin account.

- **Endpoint**: `GET /classic/init-admin`
- **Permission**: Public (only valid when no users exist)

**Response**: `Admin initialized` or `Admin already initialized`

---

### User Registration
Register a new regular user or admin account.

- **Endpoint**: `POST /classic/signup/create`
- **Permission**: Public

**Request Parameters**:

| Field | Type | Required | Description |
|---|---|---|---|
| username | string | Yes | Username (unique) |
| password | string | Yes | Password |
| type | string | Yes | User type: `user` or `admin`. **Cannot register root**. |

**Request Example**:
```json
{
    "username": "newuser",
    "password": "secretpassword",
    "type": "user"
}
```

**Response Example**:
```json
{
    "token": "guid-token-string..."
}
```

### User Login
- **Endpoint**: `POST /classic/signin`
- **Permission**: Public

**Request Parameters**:

| Field | Type | Required | Description |
|---|---|---|---|
| username | string | Yes | Username |
| password | string | Yes | Password |

**Response Example**:
```json
{
    "token": "guid-token-string..."
}
```

### Delete Account (Deactivation)
Delete the current user's account.

- **Endpoint**: `POST /classic/delete`
- **Permission**: Token required

**Request Parameters**:

| Field | Type | Required | Description |
|---|---|---|---|
| username | string | Yes | Username to delete |
| token | string | Yes | Current user's Token |

**Response Example**:
```json
{
    "message": "Account deleted successfully"
}
```

---

## 3. Classic User Endpoints

### Get User Profile
Retrieve public profile information of a specified user.

- **Endpoint**: `GET /classic/user/{username}`
- **Permission**: Public

**Response Example**:
See [Data Model Reference - User Profile](#5-classic-data-model-reference)

### Update User Profile
Update the current user's profile. This operation fully replaces list-type data (e.g., contacts, experiences).

- **Endpoint**: `POST /classic/user/{username}`
- **Permission**: Authentication required (Header)
- **Header**: `Authorization: Bearer <token>`

**Request Parameters**:
Request body must be a complete `ClassicProfile` object structure.

**Request Example**:
```json
{
    "username": "johndoe",
    "name": "John Doe Updated",
    "bio": "New Bio Content",
    "location": "New York",
    "workExperiences": [
        {
            "company": "Tech Corp",
            "position": "Senior Dev",
            "startDate": "2020-01-01"
        }
    ],
    "contacts": [
        { "type": "email", "value": "john@example.com" }
    ]
}
```

**Response Example**:
```json
{
    "success": true
}
```

---

## 4. Classic Administrator Endpoints

**Note**: All admin endpoints verify whether the requesting user has `admin` or `root` privileges.

### Check Admin Permission
Verify if the current token has admin permissions.

- **Endpoint**: `POST /classic/admin/check-permission`

**Request Parameters**:
```json
{
    "username": "admin_user",
    "token": "admin_token..."
}
```

**Response Example**:
```json
{
    "success": true,
    "type": "admin"
}
```

### Get User List (POST method)
- **Endpoint**: `POST /classic/admin/users/list`

**Request Parameters**:
```json
{
    "username": "admin_user",
    "token": "admin_token..."
}
```

**Response Example**:
```json
{
    "users": [
        { "username": "user1", "type": "user" },
        { "username": "admin1", "type": "admin" }
    ]
}
```

### Get User List (GET method)
- **Endpoint**: `GET /classic/admin/users`
- **Note**: Although it's a GET request, current implementation still requires passing JSON parameters in the body, or providing Token in the header.

**Request Parameters (Body)**:
```json
{
    "username": "admin_user",
    "token": "admin_token..." 
}
```
*Note: If Authorization Token is provided in the header, the token in the body can be omitted, but username must still match.*

### Create User (Admin operation)
Admin creates a new user directly, bypassing the registration process.

- **Endpoint**: `POST /classic/admin/users`

**Request Parameters**:

| Field | Type | Required | Description |
|---|---|---|---|
| username | string | Yes | Admin's username |
| token | string | Yes | Admin's Token |
| newUsername | string | Yes | New user's username |
| password | string | Yes | New user's password |
| type | string | Yes | `user` or `admin` |

**Response Example**:
```json
{
    "message": "User created",
    "token": "new-user-token..."
}
```

### Delete User (Admin operation)
Admin deletes a specified user.

- **Endpoint**: `DELETE /classic/admin/users/{targetUsername}`

**Request Parameters (Body)**:
```json
{
    "username": "admin_user",
    "token": "admin_token..."
}
```

**Response Example**:
```json
{
    "message": "User deleted"
}
```

---

## 5. Classic Data Model Reference

All JSON object structures used in `/classic` endpoints are as follows:

### User Profile
```json
{
    "username": "string",
    "name": "string",
    "pronouns": "string",
    "avatar": "string (Base64 Data URI / URL / Text)",
    "bio": "string",
    "location": "string",
    "website": "string",
    "background": "string",
    "currentCompany": "string",
    "currentCompanyLink": "string",
    "currentSchool": "string",
    "currentSchoolLink": "string",
    "contacts": [ Contact ],
    "socialLinks": [ SocialLink ],
    "projects": [ Project ],
    "workExperiences": [ WorkExperience ],
    "schoolExperiences": [ SchoolExperience ],
    "gallery": [ GalleryItem ]
}
```

### Contact
```json
{
    "type": "string",
    "value": "string"
}
```

### SocialLink
```json
{
    "type": "string",
    "value": "string",
    "githubData": {
        "key": "value (object)"
    }
}
```

### Project
```json
{
    "name": "string",
    "url": "string",
    "description": "string",
    "logo": "string (Base64 / URL)"
}
```

### WorkExperience
```json
{
    "position": "string",
    "company": "string",
    "companyLink": "string",
    "startDate": "yyyy-MM-dd",
    "endDate": "yyyy-MM-dd",
    "description": "string",
    "logo": "string"
}
```

### SchoolExperience
```json
{
    "school": "string",
    "schoolLink": "string",
    "degree": "string",
    "major": "string",
    "startDate": "yyyy-MM-dd",
    "endDate": "yyyy-MM-dd",
    "description": "string",
    "logo": "string"
}
```

### GalleryItem
```json
{
    "image": "string",
    "caption": "string"
}
```