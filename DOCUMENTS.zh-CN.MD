# OBCS (OpenBioCard Server) API 文档

**数据格式**: `application/json`

## 目录

1. [通用说明](#1-通用说明)
2. [Classic 认证接口](#2-classic-认证接口)
3. [Classic 用户接口](#3-classic-用户接口)
4. [Classic 管理员接口](#4-classic-管理员接口)
5. [Classic 数据模型参考](#5-classic-数据模型参考)

---

## 1. 通用说明

### 接口命名规范
- **Classic 接口** (`/classic/...`)：
  - 请求与响应的 JSON 字段均采用 **小写驼峰命名 (CamelCase)**（例如：`userName` -> `username`）。
- **其它接口** (如有)：
  - 默认采用 **大写驼峰命名 (PascalCase)**（例如：`UserName`），除非另有说明。

### 认证方式
- **Header 方式**: 在 HTTP 请求头中添加 `Authorization: Bearer <your_token>`。
- **Body 方式**: 部分 Classic 接口（如删除账号、管理员操作）需要在请求体 JSON 中包含 `token` 字段。

### 错误处理
- **200 OK**: 请求成功。
- **400 Bad Request**: 请求参数错误或校验失败。
- **401 Unauthorized**: Token 无效、过期或用户名不匹配。
- **403 Forbidden**: 权限不足（例如非管理员尝试执行管理操作，或被防火墙阻止）。
- **404 Not Found**: 资源（用户或档案）不存在。
- **409 Conflict**: 资源冲突（如用户名已存在）。
- **429 Too Many Requests**: 请求速率过快（达到速率上限）。 
- **500 Internal Server Error**: 服务器内部错误。

---

## 2. Classic 认证接口

### 初始化管理员 (首次安装)
如果系统中没有任何用户，此接口将创建一个默认管理员账号。

- **接口**: `GET /classic/init-admin`
- **权限**: 公开 (仅在无用户时有效)

**响应**: `Admin initialized` 或 `Admin already initialized`

---

### 用户注册
注册一个新的普通用户或管理员账号。

- **接口**: `POST /classic/signup/create`
- **权限**: 公开

**请求参数**:

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| username | string | 是 | 用户名 (唯一) |
| password | string | 是 | 密码 |
| type | string | 是 | 用户类型: `user` 或 `admin`。**不可注册 root**。 |

**请求示例**:
```json
{
    "username": "newuser",
    "password": "secretpassword",
    "type": "user"
}
```

**响应示例**:
```json
{
    "token": "guid-token-string..."
}
```

### 用户登录
- **接口**: `POST /classic/signin`
- **权限**: 公开

**请求参数**:

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| username | string | 是 | 用户名 |
| password | string | 是 | 密码 |

**响应示例**:
```json
{
    "token": "guid-token-string..."
}
```

### 删除账号 (注销)
用户删除自己的账号。

- **接口**: `POST /classic/delete`
- **权限**: 需提供 Token

**请求参数**:

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| username | string | 是 | 要删除的用户名 |
| token | string | 是 | 当前用户的 Token |

**响应示例**:
```json
{
    "message": "Account deleted successfully"
}
```

---

## 3. Classic 用户接口

### 获取用户资料
获取指定用户的公开资料信息。

- **接口**: `GET /classic/user/{username}`
- **权限**: 公开

**响应示例**:
见 [数据模型参考 - User Profile](#5-classic-数据模型参考)

### 更新用户资料
更新当前用户的个人资料。此操作会全量替换列表类型的数据（如联系方式、经历等）。

- **接口**: `POST /classic/user/{username}`
- **权限**: 需要认证 (Header)
- **Header**: `Authorization: Bearer <token>`

**请求参数**:
请求体为完整的 `ClassicProfile` 对象结构。

**请求示例**:
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

**响应示例**:
```json
{
    "success": true
}
```

---

## 4. Classic 管理员接口

**注意**: 所有管理员接口均验证发起请求的用户是否有 `admin` 或 `root` 权限。

### 检查管理员权限
验证当前 Token 是否拥有管理员权限。

- **接口**: `POST /classic/admin/check-permission`

**请求参数**:
```json
{
    "username": "admin_user",
    "token": "admin_token..."
}
```

**响应示例**:
```json
{
    "success": true,
    "type": "admin"
}
```

### 获取用户列表 (POST 方式)
- **接口**: `POST /classic/admin/users/list`

**请求参数**:
```json
{
    "username": "admin_user",
    "token": "admin_token..."
}
```

**响应示例**:
```json
{
    "users": [
        { "username": "user1", "type": "user" },
        { "username": "admin1", "type": "admin" }
    ]
}
```

### 获取用户列表 (GET 方式)
- **接口**: `GET /classic/admin/users`
- **注意**: 尽管是 GET 请求，但目前实现仍需在 Body 中传递 JSON 参数，或者在 Header 中携带 Token。

**请求参数 (Body)**:
```json
{
    "username": "admin_user",
    "token": "admin_token..." 
}
```
*注：如果在 Header 中携带了 Authorization Token，Body 中的 token 可省略，但 username 仍需匹配。*

### 创建用户 (管理员操作)
管理员直接创建新用户，无需注册流程。

- **接口**: `POST /classic/admin/users`

**请求参数**:

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| username | string | 是 | 操作管理员的用户名 |
| token | string | 是 | 操作管理员的 Token |
| newUsername | string | 是 | 新用户的用户名 |
| password | string | 是 | 新用户的密码 |
| type | string | 是 | `user` 或 `admin` |

**响应示例**:
```json
{
    "message": "User created",
    "token": "new-user-token..."
}
```

### 删除用户 (管理员操作)
管理员删除指定用户。

- **接口**: `DELETE /classic/admin/users/{targetUsername}`

**请求参数 (Body)**:
```json
{
    "username": "admin_user",
    "token": "admin_token..."
}
```

**响应示例**:
```json
{
    "message": "User deleted"
}
```

---

## 5. Classic 数据模型参考

所有 `/classic` 接口涉及的 JSON 对象结构如下：

### User Profile (用户资料)
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

### Contact (联系方式)
```json
{
    "type": "string",
    "value": "string"
}
```

### SocialLink (社交链接)
```json
{
    "type": "string",
    "value": "string",
    "githubData": {
        "key": "value (object)"
    }
}
```

### Project (项目)
```json
{
    "name": "string",
    "url": "string",
    "description": "string",
    "logo": "string (Base64 / URL)"
}
```

### WorkExperience (工作经历)
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

### SchoolExperience (教育经历)
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

### GalleryItem (相册项)
```json
{
    "image": "string",
    "caption": "string"
}
```