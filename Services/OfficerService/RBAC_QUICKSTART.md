# RBAC System Implementation - Quick Start Guide

## ✅ What Has Been Created

### Backend Components (All Created Successfully)

#### 1. **Models** (Entities)
- ✅ `Permission.cs` - Stores all available permissions
- ✅ `RolePermission.cs` - Maps permissions to roles

#### 2. **DTOs** (Data Transfer Objects)
- ✅ `PermissionDto.cs` - Permission data transfer objects
- ✅ `RolePermissionDto.cs` - Role-permission assignment DTOs

#### 3. **Repositories**
- ✅ `IPermissionRepository.cs` - Permission repository interface
- ✅ `PermissionRepository.cs` - Permission data access implementation
- ✅ `IRolePermissionRepository.cs` - Role-permission repository interface
- ✅ `RolePermissionRepository.cs` - Role-permission data access implementation

#### 4. **Services**
- ✅ `IPermissionService.cs` - Permission service interface
- ✅ `PermissionService.cs` - Permission business logic
- ✅ `IRolePermissionService.cs` - Role-permission service interface
- ✅ `RolePermissionService.cs` - Role-permission business logic

#### 5. **Controllers** (API Endpoints)
- ✅ `PermissionsController.cs` - Permission management endpoints
- ✅ `RolePermissionsController.cs` - Role-permission assignment endpoints
- ✅ `UserPermissionsController.cs` - User permission retrieval endpoints

#### 6. **Database**
- ✅ `AppDbContext.cs` - Updated with new DbSets and relationships
- ✅ `SeedPermissions.sql` - Initial permission data (74 permissions)

#### 7. **Documentation**
- ✅ `RBAC_IMPLEMENTATION.md` - Complete implementation guide
- ✅ `CreateRBACMigration.bat` - Easy migration script

#### 8. **Configuration**
- ✅ `Program.cs` - Services registered in DI container

## 🚀 Quick Start (3 Steps)

### Step 1: Create Database Tables

**Option A: Using the batch file (Windows)**
```bash
.\CreateRBACMigration.bat
```

**Option B: Manual commands**
```bash
cd Services\OfficerService
dotnet ef migrations add AddRBACTablesForPermissions
dotnet ef database update
```

### Step 2: Seed Initial Permissions

Execute the SQL script in SQL Server Management Studio:
1. Open `Services/OfficerService/Database/SeedPermissions.sql`
2. Connect to your database
3. Execute the script
4. Verify: Should create 74 permissions and assign all to Admin role (Role_Id = 1)

### Step 3: Test the API

Start your application and test:

```bash
# Get all permissions (Admin only)
GET https://localhost:7001/api/Permissions/GetAllPermissions

# Get your permissions
GET https://localhost:7001/api/UserPermissions/GetMyPermissions

# Assign permissions to a role
POST https://localhost:7001/api/RolePermissions/AssignPermissions
{
  "roleId": 2,
  "permissionIds": [1, 2, 3, 4, 5]
}
```

## 📋 Available API Endpoints

### Permissions Management
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/Permissions/GetAllPermissions` | Get all permissions | Admin |
| GET | `/api/Permissions/GetActivePermissions` | Get active permissions only | All |
| GET | `/api/Permissions/GetPermissionsByModule/{module}` | Get permissions by module | All |
| GET | `/api/Permissions/{id}` | Get permission by ID | All |
| POST | `/api/Permissions/CreatePermission` | Create new permission | Admin |
| PUT | `/api/Permissions/UpdatePermission/{id}` | Update permission | Admin |
| DELETE | `/api/Permissions/DeletePermission/{id}` | Delete permission | Admin |

### Role Permissions Management
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/RolePermissions/GetRolePermissions/{roleId}` | Get role's permissions | Admin |
| POST | `/api/RolePermissions/AssignPermissions` | Assign permissions to role | Admin |
| PUT | `/api/RolePermissions/UpdateRolePermissions` | Update role permissions | Admin |
| DELETE | `/api/RolePermissions/RemovePermission/{roleId}/{permissionId}` | Remove permission from role | Admin |
| GET | `/api/RolePermissions/HasPermission/{roleId}/{permissionCode}` | Check if role has permission | All |

### User Permissions
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/UserPermissions/GetUserPermissions/{loginId}` | Get user's permissions | User/Admin |
| GET | `/api/UserPermissions/GetMyPermissions` | Get current user's permissions | All |

## 📦 Permission Modules (74 Total Permissions)

| Module | Permissions | Operations |
|--------|-------------|------------|
| ROLE | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| DESIGNATION | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| USER | 7 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE, RESET_PASSWORD |
| PERMISSION | 5 | VIEW, CREATE, EDIT, DELETE, ASSIGN |
| FOREST_PRODUCES | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| SPECIES | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| PLANT_PART | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| UNIT | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| RANGE | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| DIVISION | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| CIRCLE | 6 | VIEW, CREATE, EDIT, DELETE, ACTIVATE, DEACTIVATE |
| APPLICATION | 7 | VIEW, CREATE, EDIT, DELETE, APPROVE, REJECT, FORWARD |
| REPORT | 3 | VIEW, EXPORT, GENERATE |
| SETTINGS | 2 | VIEW, EDIT |
| DASHBOARD | 2 | VIEW, ADMIN |

## 🔧 Integration with Frontend

### 1. Call API to Get User Permissions

```typescript
// In permission.service.ts
getUserPermissions(loginId: string): Observable<UserPermissionDto> {
  return this.http.get<any>(
    `${this.apiUrl}/UserPermissions/GetUserPermissions/${loginId}`
  ).pipe(
    map(response => response.userPermissions)
  );
}
```

### 2. Store Permissions Locally

```typescript
// On login or app initialization
this.permissionService.getUserPermissions(loginId).subscribe(
  permissions => {
    localStorage.setItem('userPermissions', JSON.stringify(permissions));
    this.permissionsLoaded.next(true);
  }
);
```

### 3. Check Permissions in Frontend

```typescript
// In permission.service.ts
hasPermission(permissionCode: string): boolean {
  const userPerms = JSON.parse(localStorage.getItem('userPermissions') || '{}');
  return userPerms.permissions?.some(
    p => p.permissionCode === permissionCode && p.isActive
  ) || false;
}
```

### 4. Use in Components/Templates

```html
<!-- In HTML template with directive -->
<button *appHasPermission="'ROLE_CREATE'" pButton label="Create Role"></button>

<!-- Or check in component -->
<button *ngIf="canCreateRole" pButton label="Create Role"></button>
```

```typescript
// In component
canCreateRole = false;

ngOnInit() {
  this.canCreateRole = this.permissionService.hasPermission('ROLE_CREATE');
}
```

## ✅ Verification Checklist

After setup, verify:

- [ ] Tables created: `Permissions`, `Role_Permissions`
- [ ] 74+ permissions inserted
- [ ] Admin role (Role_Id = 1) has all permissions
- [ ] API endpoints accessible via Swagger
- [ ] Can retrieve user permissions via API
- [ ] Frontend can call permission APIs
- [ ] UI elements hide/show based on permissions

## 🔍 Testing Examples

### Test 1: Get All Permissions (Admin)
```http
GET https://localhost:7001/api/Permissions/GetAllPermissions
Authorization: Bearer {admin_jwt_token}
```

Expected: List of 74 permissions grouped by module

### Test 2: Get User Permissions
```http
GET https://localhost:7001/api/UserPermissions/GetMyPermissions
Authorization: Bearer {user_jwt_token}
```

Expected: User's role and list of permissions

### Test 3: Assign Permissions to Role
```http
POST https://localhost:7001/api/RolePermissions/AssignPermissions
Authorization: Bearer {admin_jwt_token}
Content-Type: application/json

{
  "roleId": 2,
  "permissionIds": [1, 2, 3, 7, 8, 13, 14]
}
```

Expected: Confirmation with list of assigned permissions

### Test 4: Check Role Permission
```http
GET https://localhost:7001/api/RolePermissions/HasPermission/2/ROLE_VIEW
Authorization: Bearer {jwt_token}
```

Expected: `{"roleId": 2, "permissionCode": "ROLE_VIEW", "hasPermission": true}`

## 🐛 Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Migration Issues
```bash
# Remove last migration if needed
dotnet ef migrations remove

# Create new migration
dotnet ef migrations add AddRBACTablesForPermissions

# Apply to database
dotnet ef database update
```

### Permission Not Found
- Verify seeding script ran successfully
- Check: `SELECT COUNT(*) FROM Permissions`
- Should return 74+

### User Has No Permissions
- Verify user has a role: `SELECT Role_Id FROM Officer_Registration WHERE Login_Id = 'XXX'`
- Verify role has permissions: `SELECT COUNT(*) FROM Role_Permissions WHERE Role_Id = X`

## 📚 Documentation

- **Full API Documentation**: See `RBAC_IMPLEMENTATION.md`
- **Frontend Integration**: See the RBAC documentation provided
- **Permission Codes**: See `SeedPermissions.sql`

## 🎉 Success!

You now have a complete RBAC system with:
- ✅ 74 pre-defined permissions
- ✅ 15 modules covered
- ✅ Full CRUD APIs for permissions and role assignments
- ✅ User permission retrieval endpoints
- ✅ Ready for frontend integration

**Next Step:** Run the migration and seed script, then test the APIs!
