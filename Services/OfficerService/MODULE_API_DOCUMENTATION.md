# Module Management API - Backend Implementation

## Overview
The Module Management API provides complete CRUD operations for managing application modules. Modules represent logical groupings of functionality used for permission management and menu organization.

---

## Database Schema

### Master_Modules Table

```sql
CREATE TABLE Master_Modules (
    Module_Id INT PRIMARY KEY IDENTITY(1,1),
    Module_Name NVARCHAR(100) NOT NULL,
    Module_Code NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    Icon NVARCHAR(50),
    Route_Path NVARCHAR(200),
    Display_Order INT NOT NULL,
    Is_Active BIT NOT NULL DEFAULT 1,
    Created_Date DATETIME NOT NULL DEFAULT GETUTCDATE(),
    Created_By NVARCHAR(50),
    Modified_Date DATETIME,
    Modified_By NVARCHAR(50),
    Deactivated_On DATETIME,
    Reactivated_On DATETIME
);

-- Unique constraint on Module_Code
CREATE UNIQUE INDEX IX_Master_Modules_ModuleCode ON Master_Modules(Module_Code);
```

---

## API Endpoints

### Base URL
The API base URL is determined dynamically based on the user's login ID prefix.

### 1. Get All Modules

**Endpoint:** `GET /api/MasterModules/GetAllModules`

**Authorization:** Admin (Role: 1)

**Description:** Retrieves all modules (active and inactive), ordered by display order.

**Response:**
```json
{
  "message": "Modules retrieved successfully",
  "modules": [
    {
      "moduleId": 1,
      "moduleName": "Dashboard",
      "moduleCode": "DASHBOARD",
      "description": "Main dashboard with statistics",
      "icon": "pi pi-fw pi-chart-line",
      "routePath": "/app/dashboard",
      "displayOrder": 10,
      "isActive": true,
      "createdDate": "2025-01-16T10:00:00Z",
      "createdBy": "System",
      "modifiedDate": null,
      "modifiedBy": null,
      "deactivatedOn": null,
      "reactivatedOn": null
    }
  ],
  "count": 16
}
```

**Status Codes:**
- `200 OK` - Success
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized (not admin)
- `500 Internal Server Error` - Server error

---

### 2. Get Active Modules

**Endpoint:** `GET /api/MasterModules/GetActiveModules`

**Authorization:** All authenticated users

**Description:** Retrieves only active modules, ordered by display order.

**Response:**
```json
{
  "message": "Active modules retrieved successfully",
  "modules": [
    {
      "moduleId": 1,
      "moduleName": "Dashboard",
      "moduleCode": "DASHBOARD",
      "description": "Main dashboard with statistics",
      "icon": "pi pi-fw pi-chart-line",
      "routePath": "/app/dashboard",
      "displayOrder": 10,
      "isActive": true,
      "createdDate": "2025-01-16T10:00:00Z",
      "createdBy": "System"
    }
  ],
  "count": 14
}
```

**Status Codes:**
- `200 OK` - Success
- `401 Unauthorized` - Not authenticated
- `500 Internal Server Error` - Server error

---

### 3. Get Module by ID

**Endpoint:** `GET /api/MasterModules/{id}`

**Authorization:** All authenticated users

**Parameters:**
- `id` (path, required) - Module ID

**Example:** `GET /api/MasterModules/5`

**Response:**
```json
{
  "message": "Module retrieved successfully",
  "module": {
    "moduleId": 5,
    "moduleName": "Permission Management",
    "moduleCode": "PERMISSION_MANAGEMENT",
    "description": "Manage system permissions and access control",
    "icon": "pi pi-fw pi-shield",
    "routePath": "/app/permissions",
    "displayOrder": 50,
    "isActive": true,
    "createdDate": "2025-01-16T10:00:00Z",
    "createdBy": "System",
    "modifiedDate": null,
    "modifiedBy": null,
    "deactivatedOn": null,
    "reactivatedOn": null
  }
}
```

**Status Codes:**
- `200 OK` - Success
- `404 Not Found` - Module not found
- `401 Unauthorized` - Not authenticated
- `500 Internal Server Error` - Server error

---

### 4. Create Module

**Endpoint:** `POST /api/MasterModules/CreateModule`

**Authorization:** Admin (Role: 1)

**Request Body:**
```json
{
  "moduleName": "Inspection Management",
  "moduleCode": "INSPECTION_MANAGEMENT",
  "description": "Manage field inspections and verifications",
  "icon": "pi pi-fw pi-eye",
  "routePath": "/app/inspections",
  "displayOrder": 250
}
```

**Field Validations:**
- `moduleName` (required): 3-100 characters
- `moduleCode` (required): 2-50 characters, pattern: `^[A-Z_]+$`
- `description` (optional): Max 500 characters
- `icon` (optional): Max 50 characters
- `routePath` (optional): Max 200 characters
- `displayOrder` (required): 1-9999

**Response:**
```json
{
  "message": "Module created successfully",
  "module": {
    "moduleId": 17,
    "moduleName": "Inspection Management",
    "moduleCode": "INSPECTION_MANAGEMENT",
    "description": "Manage field inspections and verifications",
    "icon": "pi pi-fw pi-eye",
    "routePath": "/app/inspections",
    "displayOrder": 250,
    "isActive": true,
    "createdDate": "2025-01-16T15:30:00Z",
    "createdBy": "OFF12345"
  }
}
```

**Status Codes:**
- `201 Created` - Module created successfully
- `400 Bad Request` - Invalid input data
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized (not admin)
- `409 Conflict` - Module code already exists
- `500 Internal Server Error` - Server error

---

### 5. Update Module

**Endpoint:** `PUT /api/MasterModules/UpdateModule/{id}`

**Authorization:** Admin (Role: 1)

**Parameters:**
- `id` (path, required) - Module ID

**Request Body:**
```json
{
  "moduleId": 17,
  "moduleName": "Inspection & Verification",
  "moduleCode": "INSPECTION_MANAGEMENT",
  "description": "Updated description for inspection management",
  "icon": "pi pi-fw pi-search",
  "routePath": "/app/inspections",
  "displayOrder": 260,
  "isActive": true
}
```

**Important:** 
- `moduleId` in body must match `id` in URL
- `isActive` should always be `true` when updating via this endpoint
- Use Activate/Deactivate endpoints for status changes

**Response:**
```json
{
  "message": "Module updated successfully",
  "module": {
    "moduleId": 17,
    "moduleName": "Inspection & Verification",
    "moduleCode": "INSPECTION_MANAGEMENT",
    "description": "Updated description",
    "icon": "pi pi-fw pi-search",
    "routePath": "/app/inspections",
    "displayOrder": 260,
    "isActive": true,
    "createdDate": "2025-01-16T15:30:00Z",
    "createdBy": "OFF12345",
    "modifiedDate": "2025-01-16T16:00:00Z",
    "modifiedBy": "OFF12345"
  }
}
```

**Status Codes:**
- `200 OK` - Module updated successfully
- `400 Bad Request` - Invalid input or ID mismatch
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized (not admin)
- `404 Not Found` - Module not found
- `409 Conflict` - Module code already exists (when changing code)
- `500 Internal Server Error` - Server error

---

### 6. Activate Module

**Endpoint:** `PATCH /api/MasterModules/ActivateModule/{id}`

**Authorization:** Admin (Role: 1)

**Parameters:**
- `id` (path, required) - Module ID

**Example:** `PATCH /api/MasterModules/ActivateModule/17`

**Description:** Activates a previously deactivated module.

**Response:**
```json
{
  "message": "Module activated successfully",
  "module": {
    "moduleId": 17,
    "moduleName": "Inspection Management",
    "moduleCode": "INSPECTION_MANAGEMENT",
    "isActive": true,
    "modifiedDate": "2025-01-16T16:30:00Z",
    "modifiedBy": "OFF12345",
    "reactivatedOn": "2025-01-16T16:30:00Z"
  }
}
```

**Status Codes:**
- `200 OK` - Module activated successfully
- `400 Bad Request` - Module is already active
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized (not admin)
- `404 Not Found` - Module not found
- `500 Internal Server Error` - Server error

---

### 7. Deactivate Module

**Endpoint:** `PATCH /api/MasterModules/DeactivateModule/{id}`

**Authorization:** Admin (Role: 1)

**Parameters:**
- `id` (path, required) - Module ID

**Example:** `PATCH /api/MasterModules/DeactivateModule/17`

**Description:** Deactivates an active module. Deactivated modules are hidden from menus but data is preserved.

**Response:**
```json
{
  "message": "Module deactivated successfully",
  "module": {
    "moduleId": 17,
    "moduleName": "Inspection Management",
    "moduleCode": "INSPECTION_MANAGEMENT",
    "isActive": false,
    "modifiedDate": "2025-01-16T17:00:00Z",
    "modifiedBy": "OFF12345",
    "deactivatedOn": "2025-01-16T17:00:00Z"
  }
}
```

**Status Codes:**
- `200 OK` - Module deactivated successfully
- `400 Bad Request` - Module is already deactivated
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized (not admin)
- `404 Not Found` - Module not found
- `500 Internal Server Error` - Server error

---

### 8. Delete Module

**Endpoint:** `DELETE /api/MasterModules/DeleteModule/{id}`

**Authorization:** Admin (Role: 1)

**Parameters:**
- `id` (path, required) - Module ID

**Example:** `DELETE /api/MasterModules/DeleteModule/17`

**Description:** Permanently deletes a module. This operation cannot be undone.

**Validation:**
- Module cannot have associated permissions
- If permissions exist, they must be deleted or reassigned first

**Response:**
```json
{
  "message": "Module deleted successfully",
  "deletedModule": "Inspection Management"
}
```

**Status Codes:**
- `200 OK` - Module deleted successfully
- `400 Bad Request` - Module has associated permissions
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Not authorized (not admin)
- `404 Not Found` - Module not found
- `500 Internal Server Error` - Server error

**Error Response (when module has permissions):**
```json
{
  "message": "Cannot delete module that has associated permissions. Please delete or reassign permissions first."
}
```

---

## Setup Instructions

### 1. Run Database Migration

```bash
cd Services/OfficerService
dotnet ef migrations add AddMasterModulesTable
dotnet ef database update
```

### 2. Seed Initial Modules

Execute the SQL script in SQL Server Management Studio:

```bash
# Open and execute
Services/OfficerService/Database/SeedModules.sql
```

This will create 16 initial modules:
1. Dashboard
2. User Management
3. Role Management
4. Designation Management
5. Permission Management
6. Module Management
7. Forest Produces
8. Species Management
9. Plant Part Management
10. Unit Management
11. Range Management
12. Division Management
13. Circle Management
14. Application Management
15. Reports
16. Settings

### 3. Verify Installation

```sql
-- Check modules were created
SELECT COUNT(*) FROM Master_Modules;
-- Should return 16

-- View all modules
SELECT Module_Name, Module_Code, Display_Order, Is_Active 
FROM Master_Modules 
ORDER BY Display_Order;
```

---

## Business Rules

### Module Code
- Must be unique across all modules
- Only uppercase letters and underscores allowed
- Pattern: `^[A-Z_]+$`
- Examples: `USER_MANAGEMENT`, `FOREST_PRODUCES`

### Display Order
- Determines menu/list order
- Use increments of 10 (10, 20, 30) for flexibility
- Leave gaps for future insertions
- Range: 1-9999

### Module Status
- Active modules appear in menus and can be used
- Inactive modules are hidden but data preserved
- Edit/Delete only available for active modules
- Can reactivate deactivated modules

### Deletion Rules
- Cannot delete modules with associated permissions
- Must remove all permission references first
- Deletion is permanent (no soft delete)
- Consider deactivation instead of deletion

---

## Integration with Frontend

### Angular Service Example

```typescript
// In module.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ModuleService {
  private baseUrl = this.apiService.getBaseUrl();

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) {}

  getAllModules(): Observable<any> {
    return this.http.get(`${this.baseUrl}/MasterModules/GetAllModules`);
  }

  getActiveModules(): Observable<any> {
    return this.http.get(`${this.baseUrl}/MasterModules/GetActiveModules`);
  }

  createModule(module: CreateModuleDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/MasterModules/CreateModule`, module);
  }

  updateModule(id: number, module: UpdateModuleDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/MasterModules/UpdateModule/${id}`, module);
  }

  activateModule(id: number): Observable<any> {
    return this.http.patch(`${this.baseUrl}/MasterModules/ActivateModule/${id}`, {});
  }

  deactivateModule(id: number): Observable<any> {
    return this.http.patch(`${this.baseUrl}/MasterModules/DeactivateModule/${id}`, {});
  }

  deleteModule(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/MasterModules/DeleteModule/${id}`);
  }
}
```

---

## Error Handling

### Common Error Responses

#### 400 Bad Request
```json
{
  "message": "Invalid module data. Please check your input.",
  "errors": {
    "ModuleName": ["Module name is required."],
    "ModuleCode": ["Module code must be uppercase letters and underscores only."]
  }
}
```

#### 409 Conflict
```json
{
  "message": "Module with code 'USER_MANAGEMENT' already exists."
}
```

#### 404 Not Found
```json
{
  "message": "Module with ID 17 not found."
}
```

#### 500 Internal Server Error
```json
{
  "message": "An error occurred while creating the module.",
  "error": "Detailed error message"
}
```

---

## Testing with Postman/Swagger

### Example: Create Module

```http
POST https://localhost:7001/api/MasterModules/CreateModule
Authorization: Bearer {your_jwt_token}
Content-Type: application/json

{
  "moduleName": "Inspection Management",
  "moduleCode": "INSPECTION_MANAGEMENT",
  "description": "Manage field inspections",
  "icon": "pi pi-fw pi-eye",
  "routePath": "/app/inspections",
  "displayOrder": 250
}
```

### Example: Toggle Module Status

```http
# Deactivate
PATCH https://localhost:7001/api/MasterModules/DeactivateModule/17
Authorization: Bearer {your_jwt_token}

# Activate
PATCH https://localhost:7001/api/MasterModules/ActivateModule/17
Authorization: Bearer {your_jwt_token}
```

---

## Security Considerations

1. **Authentication Required**: All endpoints require valid JWT token
2. **Admin-Only Operations**: Create, Update, Activate, Deactivate, Delete require Admin role
3. **Read Operations**: GetAll requires Admin, GetActive and GetById available to all authenticated users
4. **Audit Trail**: Created_By, Modified_By, Deactivated_On, Reactivated_On tracked automatically
5. **Unique Constraints**: Module_Code uniqueness enforced at database level

---

## Performance Considerations

1. **Indexing**: Unique index on Module_Code for fast lookups
2. **Ordering**: Results ordered by Display_Order for menu rendering
3. **Caching**: Consider caching active modules in frontend
4. **Pagination**: Not required due to small dataset (typically < 50 modules)

---

## Troubleshooting

### Issue: Cannot create module with code "USER_MANAGEMENT"
**Solution:** Module code already exists. Use a different code.

### Issue: Cannot delete module
**Solution:** Check if module has associated permissions in Permissions table.

```sql
SELECT * FROM Permissions WHERE Module = 'YOUR_MODULE_CODE';
```

### Issue: Module not appearing in menu
**Solution:** Verify `Is_Active = 1` and `Display_Order` is set correctly.

---

## Version History

### Version 1.0.0 (January 16, 2025)
- ✅ Initial release
- ✅ Complete CRUD operations
- ✅ Activate/Deactivate functionality
- ✅ 16 pre-seeded modules
- ✅ Permission integration

---

## Related Documentation

- [RBAC Implementation Guide](./RBAC_IMPLEMENTATION.md)
- [Permission Management API](./PERMISSION_API.md)
- [Frontend Module Component](./MODULE_COMPONENT.md)

---

**Last Updated**: January 16, 2025  
**API Version**: 1.0.0  
**Status**: ✅ Production Ready
