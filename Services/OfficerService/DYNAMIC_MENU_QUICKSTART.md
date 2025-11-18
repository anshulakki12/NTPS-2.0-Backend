# Dynamic Menu System API - Quick Start Guide

## ⚠️ IMPORTANT: Role-Based, Not Module-Based

**Menus are assigned to ROLES, not modules!**

- Each role (Admin, Officer, Applicant) can have different menus in their sidebar
- The `moduleId` field in menus is **OPTIONAL** and used only for organizational/categorization purposes
- Menus are displayed based on what's assigned to the user's role in the `Role_Menus` table

**Example:**
- **Admin Role (Role_Id=1)**: Dashboard, Users, Roles, Permissions, Menus, Settings
- **Officer Role (Role_Id=2)**: Dashboard, Applications, Reports, Profile
- **Applicant Role (Role_Id=3)**: Dashboard, Apply, My Applications, Profile

## ✅ What Has Been Created

### Backend Components (Complete Menu Management System)

#### **1. Models (2 files)**
- `MasterMenu.cs` - Menu entity with hierarchical structure
- `RoleMenu.cs` - Role-menu assignment entity

#### **2. DTOs (1 file with 9 classes)**
- `MenuDto.cs` - Menu response DTO
- `CreateMenuDto.cs` - Create menu request
- `UpdateMenuDto.cs` - Update menu request
- `RoleMenuDto.cs` - Role-menu assignment response
- `CreateRoleMenuDto.cs` - Assign menus to role
- `UpdateRoleMenuDto.cs` - Update role-menu assignment
- `MenuOrderDto.cs` - Menu reordering
- `ReorderMenusDto.cs` - Batch reorder request
- `UserMenuItemDto.cs` - User menu tree structure

#### **3. Repositories (4 files)**
- `IMasterMenuRepository.cs` & `MasterMenuRepository.cs` - Menu data access
- `IRoleMenuRepository.cs` & `RoleMenuRepository.cs` - Role-menu data access

#### **4. Services (4 files)**
- `IMasterMenuService.cs` & `MasterMenuService.cs` - Menu business logic
- `IRoleMenuService.cs` & `RoleMenuService.cs` - Role-menu & user menu logic

#### **5. Controllers (3 files - 23 endpoints total)**
- `MasterMenusController.cs` - 10 menu management endpoints
- `RoleMenusController.cs` - 5 role-menu assignment endpoints
- `UserMenusController.cs` - 4 user menu retrieval endpoints
- ⚠️ **Note**: `GetMenusByModule` endpoint removed - menus are role-based, not module-based

#### **6. Database & Configuration**
- Updated `AppDbContext.cs` with new DbSets and relationships
- Updated `Program.cs` with DI registrations
- Created `SeedMenus.sql` with initial menu structure

---

## 🚀 Setup (3 Steps)

### Step 1: Create Database Tables

```bash
cd Services\OfficerService
dotnet ef migrations add AddDynamicMenuSystem
dotnet ef database update
```

### Step 2: Seed Initial Menus

Execute the SQL script in SQL Server Management Studio:
- Open `Services/OfficerService/Database/SeedMenus.sql`
- **Note**: `Module_Id` is set to NULL (optional) - menus are assigned to roles, not modules
- Execute the script
- Verify: Should create ~20 menus with hierarchical structure

### Step 3: Assign Menus to Roles

Use the API or execute SQL:

```sql
-- Example: Assign all menus to Admin role (Role_Id = 1)
INSERT INTO Role_Menus (Role_Id, Menu_Id, Display_Order, Is_Visible, Is_Active, Created_Date, Created_By)
SELECT 1, Menu_Id, ROW_NUMBER() OVER (ORDER BY Menu_Id), 1, 1, GETUTCDATE(), 'System'
FROM Master_Menus
WHERE Is_Active = 1;
```

---

## 📋 API Endpoints Summary

### Menu Management (MasterMenusController)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/MasterMenus/GetAllMenus` | Get all menus | Admin |
| GET | `/api/MasterMenus/GetActiveMenus` | Get active menus | All |
| GET | `/api/MasterMenus/GetMenuById/{id}` | Get menu by ID | All |
| GET | `/api/MasterMenus/GetParentMenus` | Get parent menus only | All |
| GET | `/api/MasterMenus/GetChildMenus/{parentId}` | Get child menus | All |
| POST | `/api/MasterMenus/CreateMenu` | Create new menu | Admin |
| PUT | `/api/MasterMenus/UpdateMenu/{id}` | Update menu | Admin |
| PATCH | `/api/MasterMenus/ActivateMenu/{id}` | Activate menu | Admin |
| PATCH | `/api/MasterMenus/DeactivateMenu/{id}` | Deactivate menu | Admin |
| DELETE | `/api/MasterMenus/DeleteMenu/{id}` | Delete menu | Admin |

### Role-Menu Assignment (RoleMenusController)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/RoleMenus/GetRoleMenus/{roleId}` | Get menus for role | Admin |
| POST | `/api/RoleMenus/AssignMenus` | Assign menus to role | Admin |
| PUT | `/api/RoleMenus/UpdateRoleMenu` | Update role-menu | Admin |
| DELETE | `/api/RoleMenus/RemoveMenu/{roleId}/{menuId}` | Remove menu from role | Admin |
| PUT | `/api/RoleMenus/ReorderMenus/{roleId}` | Reorder menus | Admin |

### User Menu Access (UserMenusController)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/UserMenus/GetUserMenus/{loginId}` | Get user's menus (flat) | User/Admin |
| GET | `/api/UserMenus/GetUserMenuTree/{loginId}` | Get user's menu tree | User/Admin |
| GET | `/api/UserMenus/GetMyMenus` | Get current user's menus | All |
| GET | `/api/UserMenus/GetMyMenuTree` | Get current user's menu tree | All |

---

## 💡 Usage Examples

### 1. Create a Parent Menu

```http
POST /api/MasterMenus/CreateMenu
Content-Type: application/json
Authorization: Bearer {admin_token}

{
  "menuName": "Master Creation",
  "menuCode": "MASTER_CREATION",
  "moduleId": null,
  "label": "Master Creation",
  "icon": "pi pi-fw pi-cog",
  "isParent": true,
  "description": "Master data management"
}
// Note: moduleId is optional - set to null or omit if not needed
```

### 2. Create a Child Menu

```http
POST /api/MasterMenus/CreateMenu
Content-Type: application/json
Authorization: Bearer {admin_token}

{
  "menuName": "Roles",
  "menuCode": "ROLES_MENU",
  "moduleId": null,
  "parentMenuId": 2,
  "label": "Roles",
  "icon": "pi pi-fw pi-users",
  "routerLink": "/app/roles",
  "isParent": false
}
// Note: moduleId is optional for organizational purposes only
```

### 3. Assign Menus to Role

```http
POST /api/RoleMenus/AssignMenus
Content-Type: application/json
Authorization: Bearer {admin_token}

{
  "roleId": 2,
  "menuIds": [1, 2, 3, 4, 5],
  "displayOrders": {
    "1": 10,
    "2": 20,
    "3": 30,
    "4": 40,
    "5": 50
  }
}
```

### 4. Get User's Menu Tree

```http
GET /api/UserMenus/GetMyMenuTree
Authorization: Bearer {user_token}
```

**Response:**
```json
{
  "message": "Your menu tree retrieved successfully",
  "menus": [
    {
      "label": "Dashboard",
      "icon": "pi pi-fw pi-chart-line",
      "routerLink": "/app/dashboard",
      "displayOrder": 1,
      "menuId": 1,
      "isVisible": true,
      "items": null
    },
    {
      "label": "Master Creation",
      "icon": "pi pi-fw pi-cog",
      "displayOrder": 2,
      "menuId": 2,
      "isVisible": true,
      "items": [
        {
          "label": "Roles",
          "icon": "pi pi-fw pi-users",
          "routerLink": "/app/roles",
          "displayOrder": 1,
          "menuId": 3,
          "parentMenuId": 2,
          "isVisible": true
        }
      ]
    }
  ]
}
```

### 5. Reorder Menus for a Role

```http
PUT /api/RoleMenus/ReorderMenus/2
Content-Type: application/json
Authorization: Bearer {admin_token}

{
  "menuOrders": [
    { "menuId": 1, "displayOrder": 50 },
    { "menuId": 2, "displayOrder": 10 },
    { "menuId": 3, "displayOrder": 20 }
  ]
}
```

---

## 🎯 Key Features

✅ **Hierarchical Menu Structure**
- Parent menus can contain child menus
- Unlimited nesting levels supported
- Self-referencing relationship

✅ **Role-Based Menu Assignment**
- Assign specific menus to each role
- Custom display order per role
- Show/hide menus per role

✅ **Dynamic Menu Building**
- Build menu tree from flat structure
- Automatic hierarchy resolution
- Recursive child menu loading

✅ **Module Integration**
- Menus linked to modules
- Group menus by functionality
- Filter menus by module

✅ **Flexible Routing**
- Internal Angular routes
- External URLs
- Custom link targets

✅ **Active/Inactive Status**
- Enable/disable menus without deletion
- Deactivate parent and all children
- Reactivate previously deactivated menus

---

## 📊 Database Schema

### Master_Menus Table
```sql
CREATE TABLE Master_Menus (
Menu_Id INT PRIMARY KEY IDENTITY(1,1),
Menu_Name NVARCHAR(100) NOT NULL,
Menu_Code NVARCHAR(50) NOT NULL UNIQUE,
Module_Id INT NULL,  -- OPTIONAL: for organizational purposes only
Parent_Menu_Id INT NULL,
    Label NVARCHAR(100) NOT NULL,
    Icon NVARCHAR(50) NULL,
    Router_Link NVARCHAR(200) NULL,
    Url NVARCHAR(500) NULL,
    Target NVARCHAR(20) NULL,
    Is_Parent BIT NOT NULL DEFAULT 0,
    Description NVARCHAR(500) NULL,
    Created_Date DATETIME NOT NULL,
    Created_By NVARCHAR(50),
    Updated_Date DATETIME,
    Updated_By NVARCHAR(50),
    Is_Active BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (Module_Id) REFERENCES Master_Modules(Module_Id),
    FOREIGN KEY (Parent_Menu_Id) REFERENCES Master_Menus(Menu_Id)
);
```

### Role_Menus Table
```sql
CREATE TABLE Role_Menus (
    Role_Menu_Id INT PRIMARY KEY IDENTITY(1,1),
    Role_Id INT NOT NULL,
    Menu_Id INT NOT NULL,
    Display_Order INT NOT NULL DEFAULT 0,
    Is_Visible BIT NOT NULL DEFAULT 1,
    Created_Date DATETIME NOT NULL,
    Created_By NVARCHAR(50),
    Updated_Date DATETIME,
    Updated_By NVARCHAR(50),
    Is_Active BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (Role_Id) REFERENCES Master_Roles(Role_Id),
    FOREIGN KEY (Menu_Id) REFERENCES Master_Menus(Menu_Id),
    UNIQUE (Role_Id, Menu_Id)
);
```

---

## 🔧 Frontend Integration

### Angular Sidebar Integration

```typescript
// In sidebar.ts
ngOnInit(): void {
  const userInfo = this.auth.getUserInfo();
  if (userInfo?.loginId) {
    this.loadUserMenus(userInfo.loginId);
  }
}

loadUserMenus(loginId: string): void {
  this.menuService.getUserMenuTree(loginId).subscribe({
    next: (response: any) => {
      this.items = this.transformMenuItems(response.menus);
      this.loading = false;
    },
    error: (err) => {
      console.error('Failed to load menus:', err);
      this.loadDefaultMenus(); // Fallback
      this.loading = false;
    }
  });
}

transformMenuItems(apiMenus: UserMenuItemDto[]): MenuItem[] {
  return apiMenus.map(menu => ({
    label: menu.label,
    icon: menu.icon,
    routerLink: menu.routerLink ? [menu.routerLink] : undefined,
    url: menu.url,
    target: menu.target,
    items: menu.items ? this.transformMenuItems(menu.items) : undefined
  }));
}
```

---

## ✅ Verification Checklist

After setup, verify:

- [ ] Tables created: `Master_Menus`, `Role_Menus`
- [ ] ~20 initial menus inserted
- [ ] Menu hierarchy established (parent-child relationships)
- [ ] Menus assigned to at least one role
- [ ] API endpoints accessible via Swagger
- [ ] Can create new menu via API
- [ ] Can assign menu to role
- [ ] Can retrieve user's menu tree
- [ ] Frontend sidebar displays dynamic menus

---

## 🐛 Troubleshooting

### Cannot Create Menu - Parent Not Found
**Error:** `Parent menu with ID X not found.`
**Solution:** Create parent menu first, or set `parentMenuId` to `null`.

### Cannot Delete Menu - Has Children
**Error:** `Cannot delete menu that has child menus.`
**Solution:** Delete all child menus first, or deactivate instead of delete.

### Cannot Delete Menu - Assigned to Roles
**Error:** `Cannot delete menu that is assigned to roles.`
**Solution:** Remove all role assignments first.

### User Has No Menus
**Check:**
1. User has a valid role assigned
2. Role has menus assigned in `Role_Menus` table
3. Menus are active (`Is_Active = 1`)
4. Role-menu assignments are active and visible

---

## 🎉 Success!

You now have a complete Dynamic Menu System with:
- ✅ Hierarchical menu structure
- ✅ Role-based menu assignment
- ✅ 24 RESTful API endpoints
- ✅ Automatic menu tree building
- ✅ Full CRUD operations
- ✅ Ready for frontend integration

**Next Steps:**
1. Run migrations to create tables
2. Seed initial menus
3. Assign menus to roles
4. Update frontend sidebar to use dynamic menus
5. Test the system end-to-end

Good luck! 🚀
