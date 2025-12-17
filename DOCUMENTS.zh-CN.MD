# OBCS API 文档

## 目录
- [认证与授权](#认证与授权)
- [个人资料管理](#个人资料管理)
- [管理员操作](#管理员操作)
- [数据结构](#数据结构)

## 认证与授权

### 用户注册
```http
POST /api/auth/signup
Content-Type: application/json
```

**请求体**:
```json
{
    "Username": "string", // 必须，唯一
    "Password": "string", // 必须
    "UserType": "string" // 必须，值: "user" 或 "admin"
}
```

**响应**:
```json
{
    "Token": "string"
}
```

**状态码**:
- 200: 注册成功
- 400: 请求参数错误
- 409: 用户名已存在
- 403: 尝试创建root用户

### 用户登录
```http
POST /api/auth/signin
Content-Type: application/json
```

**请求体**:
```json
{
    "Username": "string", // 必须
    "Password": "string" // 必须
}
```

**响应**: 同注册接口

**状态码**:
- 200: 登录成功
- 401: 用户名或密码错误

### 删除账户
```http
POST /api/auth/delete
Authorization: Bearer <token>
Content-Type: application/json
```

**请求体**:
```json
{
    "Username": "string" // 必须
}
```

**状态码**:
- 200: 删除成功
- 401: Token无效或不匹配
- 403: 不能删除root账户

### 用户登出
```http
POST /api/auth/logout
Authorization: Bearer <token>
```

**状态码**:
- 200: 登出成功
- 401: Token无效

## 个人资料管理

### 获取公开资料
```http
GET /api/profile/{username}
```

**响应**: ProfileDto 对象

**状态码**:
- 200: 成功
- 404: 用户不存在

### 更新个人资料
```http
PUT /api/profile/{username}
Authorization: Bearer <token>
Content-Type: application/json
```

**请求体**: ProfileDto 对象

**响应**:
```json
{
    "success": true
}
```

**状态码**:
- 200: 成功
- 401: Token无效或不匹配
- 404: 用户不存在

### 获取个人资料（私有）
```http
GET /api/profile/me
Authorization: Bearer <token>
```

**响应**: ProfileDto 对象

**状态码**:
- 200: 成功
- 401: Token无效
- 404: 资料不存在

## 管理员操作

### 检查管理员权限
```http
GET /api/admin/permission
Authorization: Bearer <token>
```

**响应**:
```json
{
    "success": true,
    "type": "string" // "admin" 或 "root"
}
```

**状态码**:
- 200: 成功
- 401: Token无效
- 403: 权限不足

### 获取用户列表
```http
GET /api/admin/users
Authorization: Bearer <token>
```

**响应**:
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

**状态码**:
- 200: 成功
- 401: Token无效
- 403: 权限不足

### 创建用户（管理员）

```http
POST /api/admin/users
Authorization: Bearer <token>
Content-Type: application/json
```

**请求体**:
```json
{
    "NewUsername": "string", // 必须
    "Password": "string", // 必须
    "Type": "string" // 必须，值: "user" 或 "admin"
}
```

**响应**:
```json
{
    "Token": "string"
}
```

**状态码**:
- 200: 成功
- 400: 请求参数错误
- 401: Token无效
- 403: 权限不足
- 409: 用户名已存在

### 删除用户（管理员）
```http
DELETE /api/admin/users/{username}
Authorization: Bearer <token>
```

**状态码**:
- 200: 成功
- 400: 不能删除自己的账户
- 401: Token无效
- 403: 权限不足或不能删除root账户
- 404: 用户不存在

---

## Classic API (小写驼峰)

### Classic 用户注册
```http
POST /classic/signup/create
Content-Type: application/json
```

**请求体**:
```json
{
    "userName": "string", // 必须，唯一
    "password": "string", // 必须
    "type": "string" // 必须，值: "user" 或 "admin"
}
```

**响应**:
```json
{
    "token": "string"
}
```

### Classic 用户登录
```http
POST /classic/signin
Content-Type: application/json
```

**请求体**:
```json
{
    "userName": "string", // 必须
    "password": "string" // 必须
}
```

**响应**: 同注册接口

### Classic 删除账户
```http
POST /classic/delete
Content-Type: application/json
```

**请求体**:
```json
{
    "userName": "string", // 必须
    "token": "string" // 必须
}
```

**响应**:
```json
{
    "message": "string"
}
```

### Classic 获取用户资料
```http
GET /classic/user/{username}
```

**响应**: ClassicProfile 对象

### Classic 更新用户资料
```http
POST /classic/user/{username}
Authorization: Bearer <token>
Content-Type: application/json
```

**请求体**: ClassicProfile 对象

**响应**:
```json
{
    "success": true
}
```

### Classic 检查管理员权限
```http
POST /classic/admin/check-permission
Content-Type: application/json
```

**请求体**:
```json
{
    "userName": "string", // 必须
    "token": "string" // 必须
}
```

**响应**:
```json
{
    "success": true,
    "type": "string" // "admin" 或 "root"
}
```

### Classic 获取用户列表
```http
GET /classic/admin/users
```

**注意**: 需要在Header或Body中传递token和username

### Classic 创建用户（管理员）
```http
POST /classic/admin/users
Content-Type: application/json
```

**请求体**:
```json
{
    "userName": "string", // 管理员用户名
    "token": "string", // 管理员令牌
    "newUserName": "string", // 新用户名
    "password": "string", // 新用户密码
    "type": "string" // 新用户类型，值: "user" 或 "admin"
}
```

**响应**:
```json
{
    "message": "string",
    "token": "string" // 新用户的令牌
}
```

### Classic 删除用户（管理员）
```http
DELETE /classic/admin/users/{targetUsername}
Content-Type: application/json
```

**请求体**:
```json
{
    "userName": "string", // 管理员用户名
    "token": "string" // 管理员令牌
}
```

**响应**:
```json
{
    "message": "string"
}
```

### Classic 初始化管理员
```http
GET /classic/init-admin
```

**响应**: 字符串 "Admin initialized" 或 "Admin already initialized"

---

## 数据结构

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

### Classic 兼容性数据结构 (小写驼峰)

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
    "value": "string" // 文本或Base64 Data URI
}
```

#### ClassicSocialLink
```json
{
    "type": "string",
    "value": "string",
    "githubData": {
        // 任意键值对
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