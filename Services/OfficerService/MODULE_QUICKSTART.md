# Module Management API - Quick Start

## ✅ What Has Been Created

### Backend Components

1. **Model**
   - ✅ `MasterModule.cs` - Module entity with all required fields

2. **DTOs**
   - ✅ `ModuleDto.cs` - Response DTO
   - ✅ `CreateModuleDto.cs` - Create request DTO
   - ✅ `UpdateModuleDto.cs` - Update request DTO

3. **Repository**
   - ✅ `IMasterModuleRepository.cs` - Repository interface
   - ✅ `MasterModuleRepository.cs` - Data access implementation

4. **Service**
   - ✅ `IMasterModuleService.cs` - Service interface
   - ✅ `MasterModuleService.cs` - Business logic implementation

5. **Controller**
   - ✅ `MasterModulesController.cs` - API endpoints (8 endpoints)

6. **Database**
   - ✅ `AppDbContext.cs` - Updated with MasterModule DbSet
   - ✅ `SeedModules.sql` - Initial data (16 modules)

7. **Configuration**
   - ✅ `Program.cs` - Services registered in DI container

8. **Documentation**
   - ✅ `MODULE_API_DOCUMENTATION.md` - Complete API documentation
   - ✅ `MODULE_QUICKSTART.md` - This quick start guide

---

## 🚀 Setup (3 Steps)

### Step 1: Create Database Table

```bash
cd Services\OfficerService
dotnet ef migrations add AddMasterModulesTable
dotnet ef database update
```

### Step 2: Seed Initial Modules

Execute SQL script in SSMS:
```sql
-- File: Services/OfficerService/Database/SeedModules.sql
-- Creates 16 pre-defined modules
```

### Step 3: Test the API

Start your application:
```bash
dotnet run --project Services/OfficerService
```

Test endpoints:
```bash
# Get all modules (Admin only)
GET https://localhost:7001/api/MasterModules/GetAllModules

# Get active modules (All users)
GET https://localhost:7001/api/MasterModules/GetActiveModules
```

---

## 📋 API Endpoints Summary

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/MasterModules/GetAllModules` | Get all modules | Admin |
| GET | `/api/MasterModules/GetActiveModules` | Get active modules only | All |
| GET | `/api/MasterModules/{id}` | Get module by ID | All |
| POST | `/api/MasterModules/CreateModule` | Create new module | Admin |
| PUT | `/api/MasterModules/UpdateModule/{id}` | Update module | Admin |
| PATCH | `/api/MasterModules/ActivateModule/{id}` | Activate module | Admin |
| PATCH | `/api/MasterModules/DeactivateModule/{id}` | Deactivate module | Admin |
| DELETE | `/api/MasterModules/DeleteModule/{id}` | Delete module | Admin |

---

## 💡 Usage Examples

### Create a Module

```bash
POST /api/MasterModules/CreateModule
Authorization: Bearer {token}
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

### Update a Module

```bash
PUT /api/MasterModules/UpdateModule/17
Authorization: Bearer {token}
Content-Type: application/json

{
  "moduleId": 17,
  "moduleName": "Inspection & Verification",
  "moduleCode": "INSPECTION_MANAGEMENT",
  "description": "Updated description",
  "icon": "pi pi-fw pi-search",
  "routePath": "/app/inspections",
  "displayOrder": 260,
  "isActive": true
}
```

### Activate/Deactivate Module

```bash
# Deactivate
PATCH /api/MasterModules/DeactivateModule/17

# Activate
PATCH /api/MasterModules/ActivateModule/17
```

### Delete Module

```bash
DELETE /api/MasterModules/DeleteModule/17
```

---

## 📦 Initial Modules (16 Pre-Seeded)

| Order | Module Name | Code | Icon |
|-------|-------------|------|------|
| 10 | Dashboard | DASHBOARD | pi-chart-line |
| 20 | User Management | USER_MANAGEMENT | pi-users |
| 30 | Role Management | ROLE_MANAGEMENT | pi-user-edit |
| 40 | Designation Management | DESIGNATION_MANAGEMENT | pi-id-card |
| 50 | Permission Management | PERMISSION_MANAGEMENT | pi-shield |
| 60 | Module Management | MODULE_MANAGEMENT | pi-th-large |
| 100 | Forest Produces | FOREST_PRODUCES | pi-sitemap |
| 110 | Species Management | SPECIES_MANAGEMENT | pi-globe |
| 120 | Plant Part Management | PLANT_PART_MANAGEMENT | pi-bookmark |
| 130 | Unit Management | UNIT_MANAGEMENT | pi-calculator |
| 200 | Range Management | RANGE_MANAGEMENT | pi-map-marker |
| 210 | Division Management | DIVISION_MANAGEMENT | pi-building |
| 220 | Circle Management | CIRCLE_MANAGEMENT | pi-circle |
| 300 | Application Management | APPLICATION_MANAGEMENT | pi-file |
| 400 | Reports | REPORTS | pi-chart-bar |
| 500 | Settings | SETTINGS | pi-cog |

---

## 🔧 Frontend Integration

### Angular Service

```typescript
// module.service.ts
getAllModules(): Observable<any> {
  return this.http.get(`${this.baseUrl}/MasterModules/GetAllModules`);
}

createModule(module: any): Observable<any> {
  return this.http.post(`${this.baseUrl}/MasterModules/CreateModule`, module);
}

updateModule(id: number, module: any): Observable<any> {
  return this.http.put(`${this.baseUrl}/MasterModules/UpdateModule/${id}`, module);
}

toggleActive(id: number, activate: boolean): Observable<any> {
  const endpoint = activate ? 'ActivateModule' : 'DeactivateModule';
  return this.http.patch(`${this.baseUrl}/MasterModules/${endpoint}/${id}`, {});
}

deleteModule(id: number): Observable<any> {
  return this.http.delete(`${this.baseUrl}/MasterModules/DeleteModule/${id}`);
}
```

---

## ✅ Verification Checklist

After setup, verify:

- [ ] Migration created and applied successfully
- [ ] Master_Modules table exists in database
- [ ] 16 initial modules inserted
- [ ] All modules have Is_Active = 1
- [ ] API endpoints accessible via Swagger
- [ ] Can create new module via API
- [ ] Can update existing module
- [ ] Can activate/deactivate module
- [ ] Can delete module (without permissions)
- [ ] Cannot delete module with permissions

---

## 🐛 Troubleshooting

### Migration Errors

```bash
# If migration fails, try:
dotnet ef database drop
dotnet ef database update
```

### Module Code Already Exists

**Error:** `Module with code 'USER_MANAGEMENT' already exists.`

**Solution:** Use a different module code or update the existing module.

### Cannot Delete Module

**Error:** `Cannot delete module that has associated permissions.`

**Solution:** First delete or reassign permissions that reference this module:

```sql
-- Find permissions using this module
SELECT * FROM Permissions WHERE Module = 'YOUR_MODULE_CODE';

-- Delete or update those permissions first
DELETE FROM Permissions WHERE Module = 'YOUR_MODULE_CODE';
-- OR
UPDATE Permissions SET Module = 'NEW_MODULE_CODE' WHERE Module = 'YOUR_MODULE_CODE';
```

### Module Not in Menu

**Check:**
1. Is `Is_Active = 1`?
2. Is `Display_Order` set correctly?
3. Is `Route_Path` configured?
4. Does frontend have permission to view?

---

## 📊 Database Queries

### View All Modules

```sql
SELECT Module_Name, Module_Code, Display_Order, Is_Active 
FROM Master_Modules 
ORDER BY Display_Order;
```

### Find Modules by Code Pattern

```sql
SELECT * FROM Master_Modules 
WHERE Module_Code LIKE '%MANAGEMENT%';
```

### Check Module Dependencies

```sql
-- Find permissions using a module
SELECT p.Permission_Code, p.Permission_Name, p.Module
FROM Permissions p
WHERE p.Module = 'USER_MANAGEMENT';
```

### Update Display Order

```sql
UPDATE Master_Modules 
SET Display_Order = 25 
WHERE Module_Code = 'USER_MANAGEMENT';
```

---

## 🎯 Best Practices

### Module Codes
- Use UPPERCASE_UNDERSCORE format
- Keep descriptive but concise
- Avoid abbreviations unless common

### Display Order
- Use increments of 10 (10, 20, 30...)
- Leave gaps for future insertions
- Group related modules (e.g., 200-299 for locations)

### Icons
- Use PrimeNG icons for consistency
- Format: `pi pi-fw pi-{icon-name}`
- Choose icons that match functionality

### Route Paths
- Start with `/app/`
- Use kebab-case for multi-word routes
- Keep simple and logical

---

## 📝 API Response Examples

### Success Response

```json
{
  "message": "Module created successfully",
  "module": {
    "moduleId": 17,
    "moduleName": "Inspection Management",
    "moduleCode": "INSPECTION_MANAGEMENT",
    "isActive": true,
    "createdDate": "2025-01-16T15:30:00Z"
  }
}
```

### Error Response

```json
{
  "message": "Module with code 'USER_MANAGEMENT' already exists."
}
```

---

## 🔗 Related Documentation

- **Complete API Docs**: `MODULE_API_DOCUMENTATION.md`
- **Frontend Component**: See Angular module component documentation
- **RBAC System**: `RBAC_IMPLEMENTATION.md`
- **Permission API**: `PERMISSION_API_DOCUMENTATION.md`

---

## 🎉 You're All Set!

Your Module Management API is ready to use. The system includes:

- ✅ Complete CRUD operations
- ✅ Activate/Deactivate functionality
- ✅ 16 pre-seeded modules
- ✅ Permission integration
- ✅ Full audit trail
- ✅ Error handling
- ✅ Admin-only security

**Next Steps:**
1. Run the migration
2. Seed the initial modules
3. Test the API endpoints
4. Integrate with your Angular frontend

Good luck! 🚀
