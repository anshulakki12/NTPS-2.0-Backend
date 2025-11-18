-- ====================================================
-- RBAC System - Initial Permissions Seeding Script
-- ====================================================

-- This script creates all the necessary permissions for the RBAC system
-- Execute this after creating the Permissions and Role_Permissions tables

-- ====================================================
-- 1. ROLE MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('ROLE_VIEW', 'View Roles', 'View roles and their details', 'ROLE', 1, GETUTCDATE(), 'System'),
('ROLE_CREATE', 'Create Role', 'Create new roles', 'ROLE', 1, GETUTCDATE(), 'System'),
('ROLE_EDIT', 'Edit Role', 'Edit existing roles', 'ROLE', 1, GETUTCDATE(), 'System'),
('ROLE_DELETE', 'Delete Role', 'Delete roles', 'ROLE', 1, GETUTCDATE(), 'System'),
('ROLE_ACTIVATE', 'Activate Role', 'Activate deactivated roles', 'ROLE', 1, GETUTCDATE(), 'System'),
('ROLE_DEACTIVATE', 'Deactivate Role', 'Deactivate active roles', 'ROLE', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 2. DESIGNATION MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('DESIGNATION_VIEW', 'View Designations', 'View designations and their details', 'DESIGNATION', 1, GETUTCDATE(), 'System'),
('DESIGNATION_CREATE', 'Create Designation', 'Create new designations', 'DESIGNATION', 1, GETUTCDATE(), 'System'),
('DESIGNATION_EDIT', 'Edit Designation', 'Edit existing designations', 'DESIGNATION', 1, GETUTCDATE(), 'System'),
('DESIGNATION_DELETE', 'Delete Designation', 'Delete designations', 'DESIGNATION', 1, GETUTCDATE(), 'System'),
('DESIGNATION_ACTIVATE', 'Activate Designation', 'Activate deactivated designations', 'DESIGNATION', 1, GETUTCDATE(), 'System'),
('DESIGNATION_DEACTIVATE', 'Deactivate Designation', 'Deactivate active designations', 'DESIGNATION', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 3. USER MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('USER_VIEW', 'View Users', 'View users and their details', 'USER', 1, GETUTCDATE(), 'System'),
('USER_CREATE', 'Create User', 'Create new users', 'USER', 1, GETUTCDATE(), 'System'),
('USER_EDIT', 'Edit User', 'Edit existing users', 'USER', 1, GETUTCDATE(), 'System'),
('USER_DELETE', 'Delete User', 'Delete users', 'USER', 1, GETUTCDATE(), 'System'),
('USER_ACTIVATE', 'Activate User', 'Activate deactivated users', 'USER', 1, GETUTCDATE(), 'System'),
('USER_DEACTIVATE', 'Deactivate User', 'Deactivate active users', 'USER', 1, GETUTCDATE(), 'System'),
('USER_RESET_PASSWORD', 'Reset User Password', 'Reset user passwords', 'USER', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 4. PERMISSION MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('PERMISSION_VIEW', 'View Permissions', 'View permissions and their details', 'PERMISSION', 1, GETUTCDATE(), 'System'),
('PERMISSION_CREATE', 'Create Permission', 'Create new permissions', 'PERMISSION', 1, GETUTCDATE(), 'System'),
('PERMISSION_EDIT', 'Edit Permission', 'Edit existing permissions', 'PERMISSION', 1, GETUTCDATE(), 'System'),
('PERMISSION_DELETE', 'Delete Permission', 'Delete permissions', 'PERMISSION', 1, GETUTCDATE(), 'System'),
('PERMISSION_ASSIGN', 'Assign Permissions', 'Assign permissions to roles', 'PERMISSION', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 5. FOREST PRODUCES MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('FOREST_PRODUCES_VIEW', 'View Forest Produces', 'View forest produces and their details', 'FOREST_PRODUCES', 1, GETUTCDATE(), 'System'),
('FOREST_PRODUCES_CREATE', 'Create Forest Produce', 'Create new forest produces', 'FOREST_PRODUCES', 1, GETUTCDATE(), 'System'),
('FOREST_PRODUCES_EDIT', 'Edit Forest Produce', 'Edit existing forest produces', 'FOREST_PRODUCES', 1, GETUTCDATE(), 'System'),
('FOREST_PRODUCES_DELETE', 'Delete Forest Produce', 'Delete forest produces', 'FOREST_PRODUCES', 1, GETUTCDATE(), 'System'),
('FOREST_PRODUCES_ACTIVATE', 'Activate Forest Produce', 'Activate deactivated forest produces', 'FOREST_PRODUCES', 1, GETUTCDATE(), 'System'),
('FOREST_PRODUCES_DEACTIVATE', 'Deactivate Forest Produce', 'Deactivate active forest produces', 'FOREST_PRODUCES', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 6. SPECIES MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('SPECIES_VIEW', 'View Species', 'View species and their details', 'SPECIES', 1, GETUTCDATE(), 'System'),
('SPECIES_CREATE', 'Create Species', 'Create new species', 'SPECIES', 1, GETUTCDATE(), 'System'),
('SPECIES_EDIT', 'Edit Species', 'Edit existing species', 'SPECIES', 1, GETUTCDATE(), 'System'),
('SPECIES_DELETE', 'Delete Species', 'Delete species', 'SPECIES', 1, GETUTCDATE(), 'System'),
('SPECIES_ACTIVATE', 'Activate Species', 'Activate deactivated species', 'SPECIES', 1, GETUTCDATE(), 'System'),
('SPECIES_DEACTIVATE', 'Deactivate Species', 'Deactivate active species', 'SPECIES', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 7. PLANT PART MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('PLANT_PART_VIEW', 'View Plant Parts', 'View plant parts and their details', 'PLANT_PART', 1, GETUTCDATE(), 'System'),
('PLANT_PART_CREATE', 'Create Plant Part', 'Create new plant parts', 'PLANT_PART', 1, GETUTCDATE(), 'System'),
('PLANT_PART_EDIT', 'Edit Plant Part', 'Edit existing plant parts', 'PLANT_PART', 1, GETUTCDATE(), 'System'),
('PLANT_PART_DELETE', 'Delete Plant Part', 'Delete plant parts', 'PLANT_PART', 1, GETUTCDATE(), 'System'),
('PLANT_PART_ACTIVATE', 'Activate Plant Part', 'Activate deactivated plant parts', 'PLANT_PART', 1, GETUTCDATE(), 'System'),
('PLANT_PART_DEACTIVATE', 'Deactivate Plant Part', 'Deactivate active plant parts', 'PLANT_PART', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 8. UNIT MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('UNIT_VIEW', 'View Units', 'View units and their details', 'UNIT', 1, GETUTCDATE(), 'System'),
('UNIT_CREATE', 'Create Unit', 'Create new units', 'UNIT', 1, GETUTCDATE(), 'System'),
('UNIT_EDIT', 'Edit Unit', 'Edit existing units', 'UNIT', 1, GETUTCDATE(), 'System'),
('UNIT_DELETE', 'Delete Unit', 'Delete units', 'UNIT', 1, GETUTCDATE(), 'System'),
('UNIT_ACTIVATE', 'Activate Unit', 'Activate deactivated units', 'UNIT', 1, GETUTCDATE(), 'System'),
('UNIT_DEACTIVATE', 'Deactivate Unit', 'Deactivate active units', 'UNIT', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 9. RANGE MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('RANGE_VIEW', 'View Ranges', 'View ranges and their details', 'RANGE', 1, GETUTCDATE(), 'System'),
('RANGE_CREATE', 'Create Range', 'Create new ranges', 'RANGE', 1, GETUTCDATE(), 'System'),
('RANGE_EDIT', 'Edit Range', 'Edit existing ranges', 'RANGE', 1, GETUTCDATE(), 'System'),
('RANGE_DELETE', 'Delete Range', 'Delete ranges', 'RANGE', 1, GETUTCDATE(), 'System'),
('RANGE_ACTIVATE', 'Activate Range', 'Activate deactivated ranges', 'RANGE', 1, GETUTCDATE(), 'System'),
('RANGE_DEACTIVATE', 'Deactivate Range', 'Deactivate active ranges', 'RANGE', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 10. DIVISION MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('DIVISION_VIEW', 'View Divisions', 'View divisions and their details', 'DIVISION', 1, GETUTCDATE(), 'System'),
('DIVISION_CREATE', 'Create Division', 'Create new divisions', 'DIVISION', 1, GETUTCDATE(), 'System'),
('DIVISION_EDIT', 'Edit Division', 'Edit existing divisions', 'DIVISION', 1, GETUTCDATE(), 'System'),
('DIVISION_DELETE', 'Delete Division', 'Delete divisions', 'DIVISION', 1, GETUTCDATE(), 'System'),
('DIVISION_ACTIVATE', 'Activate Division', 'Activate deactivated divisions', 'DIVISION', 1, GETUTCDATE(), 'System'),
('DIVISION_DEACTIVATE', 'Deactivate Division', 'Deactivate active divisions', 'DIVISION', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 11. CIRCLE MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('CIRCLE_VIEW', 'View Circles', 'View circles and their details', 'CIRCLE', 1, GETUTCDATE(), 'System'),
('CIRCLE_CREATE', 'Create Circle', 'Create new circles', 'CIRCLE', 1, GETUTCDATE(), 'System'),
('CIRCLE_EDIT', 'Edit Circle', 'Edit existing circles', 'CIRCLE', 1, GETUTCDATE(), 'System'),
('CIRCLE_DELETE', 'Delete Circle', 'Delete circles', 'CIRCLE', 1, GETUTCDATE(), 'System'),
('CIRCLE_ACTIVATE', 'Activate Circle', 'Activate deactivated circles', 'CIRCLE', 1, GETUTCDATE(), 'System'),
('CIRCLE_DEACTIVATE', 'Deactivate Circle', 'Deactivate active circles', 'CIRCLE', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 12. APPLICATION MANAGEMENT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('APPLICATION_VIEW', 'View Applications', 'View applications and their details', 'APPLICATION', 1, GETUTCDATE(), 'System'),
('APPLICATION_CREATE', 'Create Application', 'Create new applications', 'APPLICATION', 1, GETUTCDATE(), 'System'),
('APPLICATION_EDIT', 'Edit Application', 'Edit existing applications', 'APPLICATION', 1, GETUTCDATE(), 'System'),
('APPLICATION_DELETE', 'Delete Application', 'Delete applications', 'APPLICATION', 1, GETUTCDATE(), 'System'),
('APPLICATION_APPROVE', 'Approve Application', 'Approve applications', 'APPLICATION', 1, GETUTCDATE(), 'System'),
('APPLICATION_REJECT', 'Reject Application', 'Reject applications', 'APPLICATION', 1, GETUTCDATE(), 'System'),
('APPLICATION_FORWARD', 'Forward Application', 'Forward applications to other officers', 'APPLICATION', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 13. REPORT PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('REPORT_VIEW', 'View Reports', 'View reports and analytics', 'REPORT', 1, GETUTCDATE(), 'System'),
('REPORT_EXPORT', 'Export Reports', 'Export reports to various formats', 'REPORT', 1, GETUTCDATE(), 'System'),
('REPORT_GENERATE', 'Generate Reports', 'Generate custom reports', 'REPORT', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 14. SETTINGS PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('SETTINGS_VIEW', 'View Settings', 'View system settings', 'SETTINGS', 1, GETUTCDATE(), 'System'),
('SETTINGS_EDIT', 'Edit Settings', 'Edit system settings', 'SETTINGS', 1, GETUTCDATE(), 'System');

-- ====================================================
-- 15. DASHBOARD PERMISSIONS
-- ====================================================
INSERT INTO Permissions (Permission_Code, Permission_Name, Description, Module, Is_Active, Created_Date, Created_By)
VALUES 
('DASHBOARD_VIEW', 'View Dashboard', 'View dashboard and statistics', 'DASHBOARD', 1, GETUTCDATE(), 'System'),
('DASHBOARD_ADMIN', 'Admin Dashboard', 'View administrative dashboard', 'DASHBOARD', 1, GETUTCDATE(), 'System');

-- ====================================================
-- ASSIGN ALL PERMISSIONS TO ADMIN ROLE (Role_Id = 1)
-- ====================================================
-- This assumes Role_Id = 1 is the Admin role
-- Adjust the Role_Id if your admin role has a different ID

INSERT INTO Role_Permissions (Role_Id, Permission_Id, Assigned_Date, Assigned_By)
SELECT 1, Permission_Id, GETUTCDATE(), 'System'
FROM Permissions
WHERE Is_Active = 1;

PRINT 'RBAC System permissions seeded successfully!';
PRINT 'Total permissions created: ' + CAST((SELECT COUNT(*) FROM Permissions) AS VARCHAR);
PRINT 'Permissions assigned to Admin role (Role_Id = 1): ' + CAST((SELECT COUNT(*) FROM Role_Permissions WHERE Role_Id = 1) AS VARCHAR);
