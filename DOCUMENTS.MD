# OBCS API Documentation

## Table of Contents
- [Authentication and Authorization](#authentication-and-authorization)
- [Profile Management](#profile-management)
- [Admin Operations](#admin-operations)
- [Data Structures](#data-structures)

## Authentication and Authorization

### User Registration
```http
POST /api/auth/signup
Content-Type: application/json
```

**Request Body**:
```json
{
    "Username": "string", // Required, unique
    "Password": "string", // Required
    "UserType": "string" // Required, values: "user" or "admin"
}
```

**Response**:
```json
{
    "Token": "string"
}
```

**Status Codes**:
- 200: Registration successful
- 400: Invalid request parameters
- 409: Username already exists
- 403: Attempt to create root user

### User Login
```http
POST /api/auth/signin
Content-Type: application/json
```

**Request Body**:
```json
{
    "Username": "string", // Required
    "Password": "string" // Required
}
```

**Response**: Same as registration endpoint

**Status Codes**:
- 200: Login successful
- 401: Invalid username or password

### Delete Account
```http
POST /api/auth/delete
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body**:
```json
{
    "Username": "string" // Required
}
```

**Status Codes**:
- 200: Deletion successful
- 401: Token invalid or mismatched
- 403: Cannot delete root account

### User Logout
```http
POST /api/auth/logout
Authorization: Bearer <token>
```

**Status Codes**:
- 200: Logout successful
- 401: Token invalid

## Profile Management

### Retrieve Public Profile
```http
GET /api/profile/{username}
```

**Response**: ProfileDto object

**Status Codes**:
- 200: Success
- 404: User does not exist

### Update Profile
```http
PUT /api/profile/{username}
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body**: ProfileDto object

**Response**:
```json
{
    "success": true
}
```

**Status Codes**:
- 200: Success
- 401: Token invalid or mismatched
- 404: User does not exist

### Retrieve Private Profile
```http
GET /api/profile/me
Authorization: Bearer <token>
```

**Response**: ProfileDto object

**Status Codes**:
- 200: Success
- 401: Token invalid
- 404: Profile does not exist

## Admin Operations

### Check Admin Permission
```http
GET /api/admin/permission
Authorization: Bearer <token>
```

**Response**:
```json
{
    "success": true,
    "type": "string" // "admin" or "root"
}
```

**Status Codes**:
- 200: Success
- 401: Token invalid
- 403: Insufficient permission

### Retrieve User List
```http
GET /api/admin/users
Authorization: Bearer <token>
```

**Response**:
```json
{
    "Users": [
        {
            "Username": "string",
            "Type": "string"
        }
    ]
}
```

**Status Codes**:
- 200: Success
- 401: Token invalid
- 403: Insufficient permission

### Create User (Admin)

```http
POST /api/admin/users
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body**:
```json
{
    "NewUsername": "string", // Required
    "Password": "string", // Required
    "Type": "string" // Required, values: "user" or "admin"
}
```

**Response**:
```json
{
    "Token": "string"
}
```

**Status Codes**:
- 200: Success
- 400: Invalid request parameters
- 401: Token invalid
- 403: Insufficient permission
- 409: Username already exists

### Delete User (Admin)
```http
DELETE /api/admin/users/{username}
Authorization: Bearer <token>
```

**Status Codes**:
- 200: Success
- 400: Cannot delete own account
- 401: Token invalid
- 403: Insufficient permission or cannot delete root account
- 404: User does not exist

---

## Classic API (lower camelCase)

### Classic User Registration
```http
POST /classic/signup/create
Content-Type: application/json
```

**Request Body**:
```json
{
    "userName": "string", // Required, unique
    "password": "string", // Required
    "type": "string" // Required, values: "user" or "admin"
}
```

**Response**:
```json
{
    "token": "string"
}
```

### Classic User Login
```http
POST /classic/signin
Content-Type: application/json
```

**Request Body**:
```json
{
    "userName": "string", // Required
    "password": "string" // Required
}
```

**Response**: Same as registration endpoint

### Classic Delete Account
```http
POST /classic/delete
Content-Type: application/json
```

**Request Body**:
```json
{
    "userName": "string", // Required
    "token": "string" // Required
}
```

**Response**:
```json
{
    "message": "string"
}
```

### Classic Retrieve User Profile
```http
GET /classic/user/{username}
```

**Response**: ClassicProfile object

### Classic Update User Profile
```http
POST /classic/user/{username}
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body**: ClassicProfile object

**Response**:
```json
{
    "success": true
}
```

### Classic Check Admin Permission
```http
POST /classic/admin/check-permission
Content-Type: application/json
```

**Request Body**:
```json
{
    "userName": "string", // Required
    "token": "string" // Required
}
```

**Response**:
```json
{
    "success": true,
    "type": "string" // "admin" or "root"
}
```

### Classic Retrieve User List
```http
GET /classic/admin/users
```

**Note**: Token and username must be passed in Header or Body

### Classic Create User (Admin)
```http
POST /classic/admin/users
Content-Type: application/json
```

**Request Body**:
```json
{
    "userName": "string", // Admin username
    "token": "string", // Admin token
    "newUserName": "string", // New username
    "password": "string", // New user password
    "type": "string" // New user type, values: "user" or "admin"
}
```

**Response**:
```json
{
    "message": "string",
    "token": "string" // Token for new user
}
```

### Classic Delete User (Admin)
```http
DELETE /classic/admin/users/{targetUsername}
Content-Type: application/json
```

**Request Body**:
```json
{
    "userName": "string", // Admin username
    "token": "string" // Admin token
}
```

**Response**:
```json
{
    "message": "string"
}
```

### Classic Initialize Admin
```http
GET /classic/init-admin
```

**Response**: String "Admin initialized" or "Admin already initialized"

---

## Data Structures

### SignUpRequest
```json
{
    "Username": "string",
    "Password": "string",
    "UserType": "string"
}
```

### SignInRequest
```json
{
    "Username": "string",
    "Password": "string"
}
```

### DeleteRequest
```json
{
    "Username": "string"
}
```

### TokenResponse
```json
{
    "Token": "string"
}
```

### ProfileDto
```json
{
    "Username": "string",
    "Avatar": AssetDto,
    "NickName": "string",
    "Pronouns": "string",
    "Description": "string",
    "Location": "string",
    "Website": "string",
    "Background": AssetDto,
    "CurrentCompany": "string",
    "CurrentCompanyLink": "string",
    "CurrentSchool": "string",
    "CurrentSchoolLink": "string",
    "Contacts": [ContactItemDto],
    "SocialLinks": [SocialLinkItemDto],
    "Projects": [ProjectItemDto],
    "WorkExperiences": [WorkExperienceItemDto],
    "SchoolExperiences": [SchoolExperienceItemDto],
    "Gallery": [GalleryItemDto]
}
```

### AssetDto
```json
{
    "Type": "string", // "Text", "Image", "Remote", "Style"
    "Text": "string",
    "DataBase64": "string"
}
```

### ContactItemDto
```json
{
    "Type": "string",
    "Text": "string",
    "Image": AssetDto
}
```

### SocialLinkItemDto
```json
{
    "Type": "string",
    "Value": "string",
    "Attributes": {
        "key": "value"
    }
}
```

### ProjectItemDto
```json
{
    "Name": "string",
    "Url": "string",
    "Description": "string",
    "Logo": AssetDto
}
```

### WorkExperienceItemDto
```json
{
    "Company": "string",
    "CompanyUrl": "string",
    "Position": "string",
    "StartDate": "string", // yyyy-MM-dd
    "EndDate": "string", // yyyy-MM-dd
    "Description": "string",
    "Logo": AssetDto
}
```

### SchoolExperienceItemDto
```json
{
    "School": "string",
    "SchoolLink": "string",
    "Degree": "string",
    "Major": "string",
    "StartDate": "string", // yyyy-MM-dd
    "EndDate": "string", // yyyy-MM-dd
    "Description": "string",
    "Logo": AssetDto
}
```

### GalleryItemDto
```json
{
    "Image": AssetDto,
    "Caption": "string"
}
```

### CreateUserRequest
```json
{
    "NewUsername": "string",
    "Password": "string",
    "Type": "string"
}
```

### UserInfoDto
```json
{
    "Username": "string",
    "Type": "string"
}
```

### UserListResponse
```json
{
    "Users": [UserInfoDto]
}
```

### Classic Compatibility Data Structures (lower camelCase)

#### ClassicSignUpRequest
```json
{
    "userName": "string",
    "password": "string",
    "type": "string"
}
```

#### ClassicSignInRequest
```json
{
    "userName": "string",
    "password": "string"
}
```

#### ClassicDeleteRequest
```json
{
    "userName": "string",
    "token": "string"
}
```

#### ClassicTokenResponse
```json
{
    "token": "string"
}
```

#### ClassicProfile
```json
{
    "username": "string",
    "name": "string",
    "pronouns": "string",
    "avatar": "string", // Base64 Data URI / URL / Text
    "bio": "string",
    "location": "string",
    "website": "string",
    "background": "string", // Base64 Data URI / URL / Text
    "currentCompany": "string",
    "currentCompanyLink": "string",
    "currentSchool": "string",
    "currentSchoolLink": "string",
    "contacts": [ClassicContact],
    "socialLinks": [ClassicSocialLink],
    "projects": [ClassicProject],
    "workExperiences": [ClassicWorkExperience],
    "schoolExperiences": [ClassicSchoolExperience],
    "gallery": [ClassicGalleryItem]
}
```

#### ClassicContact
```json
{
    "type": "string",
    "value": "string" // Text or Base64 Data URI
}
```

#### ClassicSocialLink
```json
{
    "type": "string",
    "value": "string",
    "githubData": {
        // Arbitrary key-value pairs
    }
}
```

#### ClassicProject
```json
{
    "name": "string",
    "url": "string",
    "description": "string",
    "logo": "string" // Base64 Data URI / URL
}
```

#### ClassicWorkExperience
```json
{
    "position": "string",
    "company": "string",
    "companyLink": "string",
    "startDate": "string", // yyyy-MM-dd
    "endDate": "string", // yyyy-MM-dd
    "description": "string",
    "logo": "string" // Base64 Data URI / URL
}
```

#### ClassicSchoolExperience
```json
{
    "school": "string",
    "schoolLink": "string",
    "degree": "string",
    "major": "string",
    "startDate": "string", // yyyy-MM-dd
    "endDate": "string", // yyyy-MM-dd
    "description": "string",
    "logo": "string" // Base64 Data URI / URL
}
```

#### ClassicGalleryItem
```json
{
    "image": "string", // Base64 Data URI
    "caption": "string"
}
```

#### ClassicAdminRequest
```json
{
    "userName": "string",
    "token": "string"
}
```

#### ClassicCreateUserRequest
```json
{
    "userName": "string",
    "token": "string",
    "newUserName": "string",
    "password": "string",
    "type": "string"
}
```

#### ClassicUserInfo
```json
{
    "userName": "string",
    "type": "string"
}
```

#### ClassicUserListResponse
```json
{
  "users": [ClassicUserInfo]
}
```