# OBCS (OpenBioCard Server) API 文档 - 完整版

**数据格式**: `application/json`

## 目录

1. [快速开始](#1-快速开始)
2. [API概述](#2-api概述)
3. [现代API接口](#3-现代api接口)
4. [Classic API接口](#4-classic-api接口)
5. [数据模型](#5-数据模型)
6. [错误代码](#6-错误代码)
7. [配置说明](#7-配置说明)

---

## 1. 快速开始

### 环境要求
- .NET 8.0
- PostgreSQL (生产环境) 或 SQLite (开发环境)
- 内存: 最低512MB，推荐1GB

### 部署步骤
```bash
# 1. 克隆项目
git clone <repository-url>

# 2. 配置数据库连接
# 修改 appsettings.json 中的连接字符串

# 3. 运行应用
dotnet run

# 4. 初始化管理员 (首次运行)
curl -X GET "http://localhost:5000/classic/init-admin"
```

### API基地址
```
现代API: http://localhost:5000/api/
Classic API: http://localhost:5000/classic/
```

---

## 2. API概述

### 认证方式
两种认证方式都支持：

1. **Bearer Token (推荐)**
```http
Authorization: Bearer <token>
```

2. **请求体Token (Classic API兼容)**
```json
{
    "username": "user",
    "token": "your-token"
}
```

### 响应格式
```json
{
    // 成功响应
    "data": { ... },
    "success": true,
  
    // 错误响应
    "error": "错误信息",
    "errorCode": "ERROR_CODE",
    "success": false
}
```

### 通用头部
```http
Content-Type: application/json
Accept: application/json
Authorization: Bearer <token> (可选)
```

---

## 3. 现代API接口

### 3.1 认证管理
| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| POST | `/api/auth/signup` | 用户注册 | 公开 |
| POST | `/api/auth/signin` | 用户登录 | 公开 |
| POST | `/api/auth/delete` | 删除账户 | 用户自己 |
| POST | `/api/auth/logout` | 登出当前会话 | 用户自己 |

#### 用户注册
```http
POST /api/auth/signup
Content-Type: application/json

{
    "username": "newuser",
    "password": "Password123",
    "userType": "user"
}
```

**响应**:
```json
{
    "token": "guid-token-here"
}
```

#### 用户登录
```http
POST /api/auth/signin
Content-Type: application/json

{
    "username": "existinguser",
    "password": "Password123"
}
```

### 3.2 个人资料管理
| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | `/api/profile/{username}` | 获取公开资料 | 公开 |
| GET | `/api/profile/me` | 获取个人资料 | 用户自己 |
| PUT | `/api/profile/{username}` | 更新个人资料 | 用户自己 |

#### 获取公开资料
```http
GET /api/profile/johndoe
```

**响应**:
```json
{
    "username": "johndoe",
    "nickName": "John Doe",
    "avatar": {
        "type": "image",
        "text": null,
        "dataBase64": "base64-image-data"
    },
    "description": "Full-stack developer",
    "contacts": [...],
    "socialLinks": [...],
    "projects": [...]
}
```

#### 更新个人资料
```http
PUT /api/profile/johndoe
Authorization: Bearer <token>
Content-Type: application/json

{
    "username": "johndoe",
    "nickName": "John Updated",
    "description": "Updated bio",
    "avatar": {
        "type": "remote",
        "text": "https://example.com/avatar.jpg",
        "dataBase64": null
    }
}
```

### 3.3 管理员接口
| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | `/api/admin/permission` | 检查管理员权限 | admin/root |
| GET | `/api/admin/users` | 获取用户列表 | admin/root |
| POST | `/api/admin/users` | 创建新用户 | admin/root |
| DELETE | `/api/admin/users/{username}` | 删除用户 | admin/root |

#### 创建用户 (管理员)
```http
POST /api/admin/users
Authorization: Bearer <admin-token>
Content-Type: application/json

{
    "newUsername": "newemployee",
    "password": "TempPass123",
    "type": "user"
}
```

---

## 4. Classic API接口

### 4.1 Classic认证
| 方法 | 端点 | 描述 |
|------|------|------|
| POST | `/classic/signup/create` | 用户注册 |
| POST | `/classic/signin` | 用户登录 |
| POST | `/classic/delete` | 删除账户 |
| GET | `/classic/init-admin` | 初始化管理员 |

#### 注册 (Classic)
```http
POST /classic/signup/create
Content-Type: application/json

{
    "userName": "newuser",
    "password": "password123",
    "type": "user"
}
```

### 4.2 Classic用户资料
| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/classic/user/{username}` | 获取用户资料 |
| POST | `/classic/user/{username}` | 更新用户资料 |

**Classic格式示例**:
```json
{
    "username": "johndoe",
    "name": "John Doe",
    "avatar": "data:image/png;base64,...",
    "bio": "Developer",
    "contacts": [
        {
            "type": "email",
            "value": "john@example.com"
        }
    ]
}
```

### 4.3 Classic管理员
| 方法 | 端点 | 描述 |
|------|------|------|
| POST | `/classic/admin/check-permission` | 权限检查 |
| POST | `/classic/admin/users/list` | 获取用户列表 |
| GET | `/classic/admin/users` | 获取用户列表 |
| POST | `/classic/admin/users` | 创建用户 |
| DELETE | `/classic/admin/users/{username}` | 删除用户 |

---

## 5. 数据模型

### 5.1 Asset (媒体资产)
```json
{
    "type": "text|image|remote|style",
    "text": "文本内容/URL",
    "dataBase64": "base64编码的图像数据"
}
```

### 5.2 联系方式 (Contact)
```json
{
    "type": "email|phone|address",
    "text": "联系信息",
    "image": {
        "type": "image",
        "dataBase64": "base64图标"
    }
}
```

### 5.3 社交链接
```json
{
    "type": "github|twitter|linkedin",
    "value": "用户名或URL",
    "attributes": {
        "repos": "45",
        "followers": "1200"
    }
}
```

### 5.4 工作经历
```json
{
    "company": "Tech Corp",
    "companyUrl": "https://techcorp.com",
    "position": "Senior Developer",
    "startDate": "2020-01-01",
    "endDate": "2023-12-31",
    "description": "负责后端开发",
    "logo": {
        "type": "remote",
        "text": "https://techcorp.com/logo.png"
    }
}
```

---

## 6. 错误代码

| 状态码 | 错误码 | 描述 |
|--------|--------|------|
| 400 | `INVALID_REQUEST` | 请求参数无效 |
| 401 | `UNAUTHORIZED` | 认证失败 |
| 403 | `FORBIDDEN` | 权限不足 |
| 404 | `USER_NOT_FOUND` | 用户不存在 |
| 409 | `USER_EXISTS` | 用户名已存在 |
| 429 | `RATE_LIMITED` | 请求过于频繁 |
| 500 | `INTERNAL_ERROR` | 服务器内部错误 |

---

## 7. 配置说明

### appsettings.json 关键配置
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=openbiocard;Username=postgres;Password=postgres"
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:8787"
    ]
  },
  "AuthSettings": {
    "RootUsername": "root",
    "RootPassword": "change_me_in_production"
  },
  "AssetSettings": {
    "MaxFileSizeMB": 5,
    "AllowedImageTypes": [
      "image/jpeg",
      "image/png",
      "image/gif",
      "image/webp"
    ]
  }
}
```

### 环境变量
```bash
# 生产环境变量
export ConnectionStrings__DefaultConnection="Host=db;Database=openbiocard"
export AuthSettings__RootPassword="secure_password_here"
```

---

## 8. 安全注意事项

### 8.1 认证安全
1. 所有密码使用Argon2id哈希存储
2. Root账户有特殊Token格式 (`root-<guid>`)
3. 建议生产环境修改Root密码

### 8.2 CORS配置
- 开发环境允许 `localhost:*`
- 生产环境应限制为前端域名
- 支持证书验证 (`AllowCredentials`)

### 8.3 文件上传
- 最大文件大小: 5MB
- 允许的图片类型: JPEG, PNG, GIF, WebP
- 所有图像存储为Base64编码

---

## 9. 迁移指南

### 从Classic迁移到现代API
1. 认证方式从Body Token迁移到Bearer Token
2. 数据格式从扁平化迁移到结构化
3. 新增了更细粒度的权限控制

### 向后兼容性
- Classic API完全保留
- 数据格式自动转换
- 认证系统互通

---
