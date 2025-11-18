# RBAC (Role-Based Access Control) System - Backend Implementation

## Overview
This backend implementation provides a complete RBAC system for managing permissions at a granular level. It allows administrators to assign specific permissions to roles, and users inherit permissions based on their assigned roles.

## Database Schema

### Tables Created

#### 1. **Permissions Table**
Stores all available permissions in the system.

```sql
CREATE TABLE Permissions (
    Permission_Id INT PRIMARY KEY IDENTITY(1,1),
    Permission_Code NVARCHAR(100) NOT NULL UNIQUE,
    Permission_Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    Module NVARCHAR(100) NOT NULL,
    Is_Active BIT NOT NULL DEFAULT 1,
    Created_Date DATETIME NOT NULL DEFAULT GETUTCDATE(),
    Created_By NVARCHAR(50),
    Updated_Date DATETIME,
    Updated_By NVARCHAR(50)
);
```

#### 2. **Role_Permissions Table**
Maps permissions to roles (many-to-many relationship).

```sql
CREATE TABLE Role_Permissions (
    Role_Permission_Id INT PRIMARY KEY IDENTITY(1,1),
    Role_Id INT NOT NULL,
    Permission_Id INT NOT NULL,
    Assigned_Date DATETIME NOT NULL DEFAULT GETUTCDATE(),
    Assigned_By NVARCHAR(50),
    FOREIGN KEY (Role_Id) REFERENCES Master_Roles(Role_Id) ON DELETE CASCADE,
    FOREIGN KEY (Permission_Id) REFERENCES Permissions(Permission_Id) ON DELETE CASCADE,
    UNIQUE (Role_Id, Permission_Id)
);
```

## API Endpoints

### Permissions Management

#### 1. Get All Permissions
```http
GET /api/Permissions/GetAllPermissions
Authorization: Bearer {token}
Roles: Admin (1)
```

**Response:**
```json
{
  "message": "Permissions retrieved successfully",
  "permissions": [
    {
      "permissionId": 1,
      "permissionCode": "ROLE_VIEW",
      "permissionName": "View Roles",
      "description": "View roles and their details",
      "module": "ROLE",
      "isActive": true,
      "createdDate": "2025-01-15T10:00:00Z",
      "createdBy": "System"
    }
  ],
  "count": 85
}
```

#### 2. Get Active Permissions
```http
GET /api/Permissions/GetActivePermissions
Authorization: Bearer {token}
```

#### 3. Get Permissions by Module
```http
GET /api/Permissions/GetPermissionsByModule/{module}
Authorization: Bearer {token}
```

**Example:**
```http
GET /api/Permissions/GetPermissionsByModule/ROLE
```

#### 4. Get Permission by ID
```http
GET /api/Permissions/{id}
Authorization: Bearer {token}
```

#### 5. Create Permission
```http
POST /api/Permissions/CreatePermission
Authorization: Bearer {token}
Roles: Admin (1)
Content-Type: application/json
```

**Request Body:**
```json
{
  "permissionCode": "CUSTOM_ACTION",
  "permissionName": "Custom Action",
  "description": "Description of custom action",
  "module": "CUSTOM_MODULE"
}
```

#### 6. Update Permission
```http
PUT /api/Permissions/UpdatePermission/{id}
Authorization: Bearer {token}
Roles: Admin (1)
Content-Type: application/json
```

**Request Body:**
```json
{
  "permissionName": "Updated Permission Name",
  "description": "Updated description",
  "isActive": true
}
```

#### 7. Delete Permission
```http
DELETE /api/Permissions/DeletePermission/{id}
Authorization: Bearer {token}
Roles: Admin (1)
```

### Role Permissions Management

#### 1. Get Role Permissions
```http
GET /api/RolePermissions/GetRolePermissions/{roleId}
Authorization: Bearer {token}
Roles: Admin (1)
```

**Response:**
```json
{
  "message": "Role permissions retrieved successfully",
  "roleId": 2,
  "permissions": [
    {
      "rolePermissionId": 1,
      "roleId": 2,
      "permissionId": 5,
      "roleName": "Officer",
      "permissionCode": "APPLICATION_VIEW",
      "permissionName": "View Applications",
      "module": "APPLICATION",
      "assignedDate": "2025-01-15T10:00:00Z",
      "assignedBy": "admin@example.com"
    }
  ],
  "count": 15
}
```

#### 2. Assign Permissions to Role
```http
POST /api/RolePermissions/AssignPermissions
Authorization: Bearer {token}
Roles: Admin (1)
Content-Type: application/json
```

**Request Body:**
```json
{
  "roleId": 2,
  "permissionIds": [1, 2, 3, 4, 5, 6]
}
```

**Response:**
```json
{
  "message": "Permissions assigned successfully",
  "roleId": 2,
  "permissions": [...],
  "count": 6
}
```

#### 3. Update Role Permissions (Replace All)
```http
PUT /api/RolePermissions/UpdateRolePermissions
Authorization: Bearer {token}
Roles: Admin (1)
Content-Type: application/json
```

**Request Body:**
```json
{
  "roleId": 2,
  "permissionIds": [1, 2, 5, 7, 10]
}
```

**Note:** This will remove all existing permissions for the role and assign the new ones.

#### 4. Remove Permission from Role
```http
DELETE /api/RolePermissions/RemovePermission/{roleId}/{permissionId}
Authorization: Bearer {token}
Roles: Admin (1)
```

**Example:**
```http
DELETE /api/RolePermissions/RemovePermission/2/5
```

#### 5. Check if Role Has Permission
```http
GET /api/RolePermissions/HasPermission/{roleId}/{permissionCode}
Authorization: Bearer {token}
```

**Example:**
```http
GET /api/RolePermissions/HasPermission/2/ROLE_VIEW
```

**Response:**
```json
{
  "roleId": 2,
  "permissionCode": "ROLE_VIEW",
  "hasPermission": true
}
```

### User Permissions

#### 1. Get User Permissions
```http
GET /api/UserPermissions/GetUserPermissions/{loginId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "User permissions retrieved successfully",
  "userPermissions": {
    "loginId": "OFF12345",
    "roleId": 2,
    "roleName": "Officer",
    "permissions": [
      {
        "permissionId": 1,
        "permissionCode": "APPLICATION_VIEW",
        "permissionName": "View Applications",
        "description": "View applications and their details",
        "module": "APPLICATION",
        "isActive": true
      }
    ]
  }
}
```

#### 2. Get My Permissions (Current User)
```http
GET /api/UserPermissions/GetMyPermissions
Authorization: Bearer {token}
```

**Response:** Same as above, but for the currently authenticated user.

## Setup Instructions

### 1. Run Database Migration

First, create a migration for the new tables:

```bash
cd Services/OfficerService
dotnet ef migrations add AddRBACTables
dotnet ef database update
```

### 2. Seed Initial Permissions

After creating the tables, run the seeding script to populate initial permissions:

```bash
# Execute the SQL script in SSMS or using sqlcmd
sqlcmd -S your_server -d your_database -i Database/SeedPermissions.sql
```

Or execute the script directly from SSMS by opening `Services/OfficerService/Database/SeedPermissions.sql`.

### 3. Verify Permissions

Check that permissions were created:

```sql
SELECT COUNT(*) FROM Permissions;
-- Should return 85+ permissions

SELECT COUNT(*) FROM Role_Permissions WHERE Role_Id = 1;
-- Should return 85+ (all permissions assigned to Admin role)
```

## Permission Modules

The system includes permissions for the following modules:

1. **ROLE** - Role management (6 permissions)
2. **DESIGNATION** - Designation management (6 permissions)
3. **USER** - User management (7 permissions)
4. **PERMISSION** - Permission management (5 permissions)
5. **FOREST_PRODUCES** - Forest produces management (6 permissions)
6. **SPECIES** - Species management (6 permissions)
7. **PLANT_PART** - Plant part management (6 permissions)
8. **UNIT** - Unit management (6 permissions)
9. **RANGE** - Range management (6 permissions)
10. **DIVISION** - Division management (6 permissions)
11. **CIRCLE** - Circle management (6 permissions)
12. **APPLICATION** - Application management (7 permissions)
13. **REPORT** - Reporting (3 permissions)
14. **SETTINGS** - System settings (2 permissions)
15. **DASHBOARD** - Dashboard access (2 permissions)

**Total: 74 permissions**

## Usage Examples

### Example 1: Assign Permissions to a New Role

```http
POST /api/RolePermissions/AssignPermissions
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "roleId": 3,
  "permissionIds": [
    1, 2, 7, 8, 9, 10, 13, 14, 15, 16, 17, 18
  ]
}
```

### Example 2: Get User's Permissions in Frontend

```typescript
// In Angular service
getUserPermissions(loginId: string): Observable<UserPermissionDto> {
  return this.http.get<any>(
    `${this.baseUrl}/UserPermissions/GetUserPermissions/${loginId}`
  ).pipe(
    map(response => response.userPermissions)
  );
}

// In component
ngOnInit() {
  const loginId = this.authService.getLoginId();
  this.permissionService.getUserPermissions(loginId).subscribe(
    permissions => {
      this.userPermissions = permissions;
      // Store in local storage for directive to use
      localStorage.setItem('userPermissions', JSON.stringify(permissions));
    }
  );
}
```

### Example 3: Check Permission Before Action

```typescript
// In component
canEditRole(): boolean {
  const permissions = JSON.parse(localStorage.getItem('userPermissions') || '{}');
  return permissions.permissions?.some(
    p => p.permissionCode === 'ROLE_EDIT' && p.isActive
  );
}

onEditRole(role: any) {
  if (!this.canEditRole()) {
    this.showError('You do not have permission to edit roles');
    return;
  }
  
  // Proceed with edit
  this.editRole(role);
}
```

## Security Considerations

1. **JWT Authentication Required**: All endpoints require valid JWT token
2. **Role-Based Authorization**: Most admin operations require Admin role (Role_Id = 1)
3. **User Isolation**: Users can only view their own permissions unless they are admins
4. **Cascade Delete**: Deleting a role or permission will remove associated Role_Permissions entries
5. **Unique Constraints**: Prevents duplicate permission codes and role-permission assignments

## Testing the System

### 1. Test Permission Creation

```bash
curl -X POST "https://localhost:7001/api/Permissions/CreatePermission" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "permissionCode": "TEST_PERMISSION",
    "permissionName": "Test Permission",
    "description": "This is a test permission",
    "module": "TEST"
  }'
```

### 2. Test Permission Assignment

```bash
curl -X POST "https://localhost:7001/api/RolePermissions/AssignPermissions" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roleId": 2,
    "permissionIds": [1, 2, 3]
  }'
```

### 3. Test User Permissions Retrieval

```bash
curl -X GET "https://localhost:7001/api/UserPermissions/GetMyPermissions" \
  -H "Authorization: Bearer YOUR_USER_TOKEN"
```

## Troubleshooting

### Issue: Permissions table not found
**Solution:** Run the migration: `dotnet ef database update`

### Issue: No permissions in database
**Solution:** Run the seeding script: `Database/SeedPermissions.sql`

### Issue: Admin role has no permissions
**Solution:** Re-run the seeding script or manually assign:
```sql
INSERT INTO Role_Permissions (Role_Id, Permission_Id, Assigned_Date, Assigned_By)
SELECT 1, Permission_Id, GETUTCDATE(), 'System'
FROM Permissions WHERE Is_Active = 1;
```

### Issue: User has no permissions
**Solution:** Ensure user has a role assigned in Officer_Registration table and that role has permissions

## Next Steps

1. ✅ Create database tables via migration
2. ✅ Seed initial permissions
3. ✅ Test API endpoints using Postman/Swagger
4. ✅ Integrate with frontend Angular application
5. ✅ Implement permission-based UI hiding/showing
6. ✅ Add permission checks in frontend before API calls
7. ✅ Monitor and audit permission usage

## Support

For issues or questions about the RBAC system:
- Check the API documentation at `/swagger`
- Review the permission codes in `SeedPermissions.sql`
- Verify role assignments in `Master_Roles` and `Officer_Registration` tables
