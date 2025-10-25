# HTTP Testing Project Template

This README serves as a complete guide for setting up and using this modular HTTP testing structure in future projects.

---

## 📁 Project Structure

```
ApiTest.Testing/
├── http-client.env.json              # Environment variables (shared)
├── http-client.private.env.json      # Personal overrides (gitignored)
├── .gitignore                        # Ignore private settings
├── Module1/                          # Each API module gets a folder
│   ├── Action1/                      # Each action gets a subfolder
│   │   ├── Action1.success.http      # Success test scenarios
│   │   └── Action1.fail.http         # Failure test scenarios
│   ├── Action2/
│   │   ├── Action2.success.http
│   │   └── Action2.fail.http
│   └── ...
├── Module2/
│   └── ...
└── Auth/                             # Authentication module
    ├── Login.success.http
    ├── Login.fail.http
    ├── Register/
    │   ├── Register.success.http
    │   └── Register.fail.http
    └── ...
```

### Structure Rules

1. **Module** = One API resource (Products, Orders, Users, etc.)
2. **Action** = One operation (Create, Update, Delete, GetAll, etc.)
3. **Files**: Always two files per action:
   - `.success.http` - Success scenarios
   - `.fail.http` - Failure scenarios

---

## 🌍 Environment Configuration

### File: `http-client.env.json`

```json
{
  "$schema": "http://json.schemastore.org/http-client.env",
  "dev": {
    "baseUrl": "https://localhost:5001",
    "apiPath": "/api",
    "description": "Local development (default)"
  },
  "staging": {
    "baseUrl": "https://staging.yourapi.com",
    "apiPath": "/api",
    "description": "Staging environment"
  },
  "production": {
    "baseUrl": "https://api.yourapi.com",
    "apiPath": "/api",
    "description": "Production environment"
  }
}
```

### File: `http-client.private.env.json` (Optional)

```json
{
  "$schema": "http://json.schemastore.org/http-client.env",
  "dev": {
    "baseUrl": "https://localhost:7001",
    "apiPath": "/api/v2"
  }
}
```

### File: `.gitignore`

```
http-client.private.env.json
*.private.http
```

**Usage:**
- Variables automatically load in Visual Studio, VS Code (REST Client), Rider
- Reference: `{{baseUrl}}{{apiPath}}/endpoint`
- Switch environments using the environment picker in your IDE

---

## 🔐 Authentication Pattern

### For Authorized Endpoints

Every `.success.http` and `.fail.http` file that tests authorized actions must follow this pattern:

### SUCCESS File Template

```http
### [Module] [Action] - SUCCESS Scenarios
### Variables loaded from: http-client.env.json
### NOTE: [Action] requires authentication!

###############################################################################
### STEP 1: Login to Get Token
### This request is NAMED "loginFor[Action]" so the token can be reused
###############################################################################

# @name loginFor[Action]
POST {{baseUrl}}{{apiPath}}/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

###############################################################################
### SUCCESS SCENARIO 1: [Description]
### Expected: [HTTP Status Code] [Status Message]
### Uses token from loginFor[Action] automatically!
###############################################################################

[METHOD] {{baseUrl}}{{apiPath}}/[endpoint]
Authorization: Bearer {{loginFor[Action].response.body.$.token}}
Content-Type: application/json

{
  // request body
}

###

### Add more scenarios - all reuse the same token
```

### FAILURE File Template

```http
### [Module] [Action] - FAILURE Scenarios
### Variables loaded from: http-client.env.json

###############################################################################
### STEP 1: Login to Get Token (for authorized failure tests)
###############################################################################

# @name loginFor[Action]Fail
POST {{baseUrl}}{{apiPath}}/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

###############################################################################
### FAILURE SCENARIO 1: [Action] Without Authentication
### Expected: 401 Unauthorized
###############################################################################

[METHOD] {{baseUrl}}{{apiPath}}/[endpoint]
Content-Type: application/json

{
  // request body
}

###

###############################################################################
### FAILURE SCENARIO 2: [Other Validation Error]
### Expected: [HTTP Status Code] [Status Message]
###############################################################################

[METHOD] {{baseUrl}}{{apiPath}}/[endpoint]
Authorization: Bearer {{loginFor[Action]Fail.response.body.$.token}}
Content-Type: application/json

{
  // invalid request body
}

###

### Add more validation scenarios - all include the token
```

---

## 🎯 Key Syntax Rules

### 1. Named Requests (Token Capture)
```http
# @name requestName
POST {{baseUrl}}{{apiPath}}/endpoint
```

### 2. Access Response Values (JSONPath)
```http
# Token from response
{{requestName.response.body.$.token}}

# Field from response
{{requestName.response.body.$.id}}

# Nested field
{{requestName.response.body.$.user.email}}

# Array item
{{requestName.response.body.$.items[0].name}}

# Response header
{{requestName.response.headers.location}}
```

**IMPORTANT**: Always use `$.` prefix for JSON body fields (Visual Studio requirement)

### 3. Authorization Header
```http
Authorization: Bearer {{loginName.response.body.$.token}}
```

### 4. Environment Variables
```http
{{baseUrl}}        # From http-client.env.json
{{apiPath}}        # From http-client.env.json
```

---

## 📋 Complete Example

### File: `Products/CreateProduct/CreateProduct.success.http`

```http
### Products Create - SUCCESS Scenarios
### Variables loaded from: http-client.env.json
### NOTE: Product creation requires authentication!

###############################################################################
### STEP 1: Login to Get Token
###############################################################################

# @name loginForProduct
POST {{baseUrl}}{{apiPath}}/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

###############################################################################
### SUCCESS SCENARIO 1: Create Product with All Fields
### Expected: 201 Created
###############################################################################

POST {{baseUrl}}{{apiPath}}/products
Authorization: Bearer {{loginForProduct.response.body.$.token}}
Content-Type: application/json

{
  "name": "Test Product",
  "description": "Product description",
  "price": 99.99,
  "stockQuantity": 50
}

###

###############################################################################
### SUCCESS SCENARIO 2: Create Product with Minimal Fields
### Expected: 201 Created
###############################################################################

POST {{baseUrl}}{{apiPath}}/products
Authorization: Bearer {{loginForProduct.response.body.$.token}}
Content-Type: application/json

{
  "name": "Minimal Product",
  "price": 19.99,
  "stockQuantity": 10
}
```

### File: `Products/CreateProduct/CreateProduct.fail.http`

```http
### Products Create - FAILURE Scenarios
### Variables loaded from: http-client.env.json

###############################################################################
### STEP 1: Login to Get Token
###############################################################################

# @name loginForProductFail
POST {{baseUrl}}{{apiPath}}/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

###############################################################################
### FAILURE SCENARIO 1: Create Without Authentication
### Expected: 401 Unauthorized
###############################################################################

POST {{baseUrl}}{{apiPath}}/products
Content-Type: application/json

{
  "name": "Unauthorized Product",
  "price": 99.99,
  "stockQuantity": 50
}

###

###############################################################################
### FAILURE SCENARIO 2: Missing Required Field - Name
### Expected: 400 Bad Request
###############################################################################

POST {{baseUrl}}{{apiPath}}/products
Authorization: Bearer {{loginForProductFail.response.body.$.token}}
Content-Type: application/json

{
  "price": 99.99,
  "stockQuantity": 50
}

###

###############################################################################
### FAILURE SCENARIO 3: Negative Price
### Expected: 400 Bad Request
###############################################################################

POST {{baseUrl}}{{apiPath}}/products
Authorization: Bearer {{loginForProductFail.response.body.$.token}}
Content-Type: application/json

{
  "name": "Invalid Product",
  "price": -50.00,
  "stockQuantity": 50
}
```

---

## 🤖 AI Prompt for Generating Test Files

Use this prompt when asking AI to generate HTTP test files for your project:

### Prompt Template

```
Create HTTP test files for the [MODULE_NAME] module, [ACTION_NAME] action.

CONTEXT:
- API Base URL: {{baseUrl}}{{apiPath}}
- Endpoint: [HTTP_METHOD] {{baseUrl}}{{apiPath}}/[endpoint]
- Authentication: Required (uses Bearer token)
- Request body structure: [PROVIDE_REQUEST_MODEL]
- Response structure: [PROVIDE_RESPONSE_MODEL]

REQUIREMENTS:

1. PROJECT STRUCTURE:
   - Create two files in folder: [MODULE]/[ACTION]/
   - File 1: [ACTION].success.http (success scenarios)
   - File 2: [ACTION].fail.http (failure scenarios)

2. ENVIRONMENT SETUP:
   - Variables loaded from: http-client.env.json
   - Use {{baseUrl}}{{apiPath}} for all URLs
   - Never hardcode URLs

3. AUTHENTICATION PATTERN:
   - Start each file with login request: # @name loginFor[Action]
   - Login endpoint: POST {{baseUrl}}{{apiPath}}/auth/login
   - Login credentials: {"username": "testuser", "password": "password123"}
   - Use token in all requests: Authorization: Bearer {{loginFor[Action].response.body.$.token}}
   - IMPORTANT: Use JSONPath syntax with $.  prefix for response fields

4. SUCCESS FILE REQUIREMENTS:
   - Include 5-8 different success scenarios
   - Each scenario tests different valid inputs
   - All scenarios reuse the same login token
   - Format: ### SUCCESS SCENARIO N: [Description]
   - Always include: Authorization: Bearer {{loginFor[Action].response.body.$.token}}

5. FAILURE FILE REQUIREMENTS:
   - FIRST scenario: Test 401 Unauthorized (no token)
   - Login after first scenario: # @name loginFor[Action]Fail
   - Remaining scenarios: Test validation errors (WITH token)
   - Include: Missing fields, invalid formats, edge cases
   - Format: ### FAILURE SCENARIO N: [Description]

6. RESPONSE FIELD ACCESS:
   - Use JSONPath: {{requestName.response.body.$.fieldName}}
   - Always include $. prefix for JSON fields
   - Example: {{login.response.body.$.token}}

7. REQUEST NAMING:
   - Use # @name to capture responses
   - Example: # @name createProduct

GENERATE:
- Complete [ACTION].success.http file with 5-8 scenarios
- Complete [ACTION].fail.http file with validation tests
- Follow the exact pattern and syntax shown above
```

### Example Usage

```
Create HTTP test files for the Users module, UpdateProfile action.

CONTEXT:
- API Base URL: {{baseUrl}}{{apiPath}}
- Endpoint: PUT {{baseUrl}}{{apiPath}}/users/{id}
- Authentication: Required (uses Bearer token)
- Request body: { "firstName": string, "lastName": string, "email": string }
- Response: { "id": number, "firstName": string, "lastName": string, "email": string }

[Include all REQUIREMENTS from template above]
```

---

## 🎨 Token Naming Convention

| Context | Token Name Pattern | Example |
|---------|-------------------|---------|
| Success files | `loginFor[Action]` | `loginForProduct` |
| Failure files | `loginFor[Action]Fail` | `loginForProductFail` |
| Multiple users | `login[UserRole]` | `loginAdmin`, `loginUser` |

---

## ✅ Testing Checklist

When creating tests for a new module/action:

- [ ] Create module folder if doesn't exist
- [ ] Create action subfolder
- [ ] Create `.success.http` file
  - [ ] Add login request with `# @name`
  - [ ] Add 5-8 success scenarios
  - [ ] All scenarios use `Authorization: Bearer {{token}}`
  - [ ] Use `$.` prefix for response fields
- [ ] Create `.fail.http` file
  - [ ] First scenario: 401 Unauthorized (no token)
  - [ ] Add login request after first scenario
  - [ ] Add 5-10 validation failure scenarios
  - [ ] All validation scenarios include authorization
- [ ] Test the files
  - [ ] Run login request
  - [ ] Verify token captured
  - [ ] Run all scenarios
  - [ ] Verify all pass/fail as expected

---

## 🛠️ Quick Start for New Project

### Step 1: Setup Structure
```powershell
mkdir MyApi.Testing
cd MyApi.Testing
mkdir Module1, Module1/Action1, Auth
```

### Step 2: Create Environment File
Create `http-client.env.json`:
```json
{
  "$schema": "http://json.schemastore.org/http-client.env",
  "dev": {
    "baseUrl": "https://localhost:5001",
    "apiPath": "/api"
  }
}
```

### Step 3: Create .gitignore
```
http-client.private.env.json
*.private.http
```

### Step 4: Use Templates
Copy the templates from this README to create your first test files.

### Step 5: Run Tests
1. Open `.http` file in Visual Studio / VS Code
2. Select `dev` environment
3. Run login request
4. Run test scenarios

---

## 📖 Additional Notes

### Why This Structure?

1. **Modular**: Each module isolated in its own folder
2. **Scalable**: Add modules/actions without affecting others
3. **Self-Contained**: Each file includes its own login
4. **Maintainable**: Clear naming, consistent patterns
5. **Reusable**: Templates work for any API project
6. **Version Controlled**: All tests tracked in Git
7. **No External Tools**: Uses built-in HTTP clients

### IDE Support

- ✅ **Visual Studio 2022** - Full support
- ✅ **VS Code** (with REST Client extension) - Full support
- ✅ **JetBrains Rider** - Full support
- ❌ **Postman** - Different syntax (not compatible)

### Common Pitfalls

1. **Forgetting `$.` prefix** - Causes HTTP0021 error
2. **Not running login first** - Token will be undefined
3. **Hardcoding URLs** - Use `{{baseUrl}}{{apiPath}}` instead
4. **Missing `# @name`** - Can't capture response
5. **Wrong token reference** - Must match the `@name` value

---

## 🎓 Summary

This template provides:
- ✅ Modular folder structure
- ✅ Environment variable configuration
- ✅ Authentication token patterns
- ✅ Success/failure test templates
- ✅ AI prompt for generating tests
- ✅ Complete examples
- ✅ Best practices

**Copy this structure to any new project and follow the patterns!**

