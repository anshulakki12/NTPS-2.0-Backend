-- ====================================================
-- Dynamic Menu System - Initial Menus Seeding Script
-- ====================================================

-- This script creates initial menus for the Dynamic Menu System
-- Execute this after creating the Master_Menus and Role_Menus tables

-- IMPORTANT: Menus are assigned to ROLES, not modules!
-- Module_Id is OPTIONAL and used only for organizational purposes
-- You can set Module_Id to NULL if you don't need module categorization

-- ====================================================
-- PARENT MENUS
-- ====================================================

-- Dashboard (no module assignment needed)
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Dashboard', 'DASHBOARD', NULL, NULL, 'Dashboard', 'pi pi-fw pi-chart-line', '/app/dashboard', 0, 'Main dashboard with overview and statistics', 1, GETUTCDATE(), 'System');

-- Master Creation Parent Menu (optional module categorization)
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Master Creation', 'MASTER_CREATION', NULL, NULL, 'Master Creation', 'pi pi-fw pi-cog', NULL, 1, 'Master data creation and management', 1, GETUTCDATE(), 'System');

-- Get the ID of Master Creation for child menus
DECLARE @MasterCreationId INT = SCOPE_IDENTITY();

-- Child Menus under Master Creation (Module_Id is optional, set to NULL)
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Roles', 'ROLES_MENU', NULL, @MasterCreationId, 'Roles', 'pi pi-fw pi-users', '/app/roles', 0, 'Manage user roles', 1, GETUTCDATE(), 'System'),
('Designations', 'DESIGNATIONS_MENU', NULL, @MasterCreationId, 'Designation', 'pi pi-fw pi-id-card', '/app/designation', 0, 'Manage officer designations', 1, GETUTCDATE(), 'System'),
('Permissions', 'PERMISSIONS_MENU', NULL, @MasterCreationId, 'Permissions', 'pi pi-fw pi-shield', '/app/permissions', 0, 'Manage system permissions', 1, GETUTCDATE(), 'System'),
('Modules', 'MODULES_MENU', NULL, @MasterCreationId, 'Modules', 'pi pi-fw pi-th-large', '/app/module', 0, 'Manage application modules', 1, GETUTCDATE(), 'System'),
('Menus', 'MENUS_MENU', NULL, @MasterCreationId, 'Menus', 'pi pi-fw pi-bars', '/app/menu', 0, 'Manage dynamic menus', 1, GETUTCDATE(), 'System'),
('Role Permissions', 'ROLE_PERMISSIONS_MENU', NULL, @MasterCreationId, 'Role Permissions', 'pi pi-fw pi-key', '/app/role-permissions', 0, 'Assign permissions to roles', 1, GETUTCDATE(), 'System'),
('Role Menus', 'ROLE_MENUS_MENU', NULL, @MasterCreationId, 'Role Menus', 'pi pi-fw pi-sitemap', '/app/role-menus', 0, 'Assign menus to roles', 1, GETUTCDATE(), 'System');

-- Forest Data Parent Menu
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Forest Data', 'FOREST_DATA', NULL, NULL, 'Forest Data', 'pi pi-fw pi-globe', NULL, 1, 'Forest-related data management', 1, GETUTCDATE(), 'System');

DECLARE @ForestDataId INT = SCOPE_IDENTITY();

INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Forest Produces', 'FOREST_PRODUCES_MENU', NULL, @ForestDataId, 'Forest Produces', 'pi pi-fw pi-sitemap', '/app/forest-produces', 0, 'Manage forest produce types', 1, GETUTCDATE(), 'System'),
('Species', 'SPECIES_MENU', NULL, @ForestDataId, 'Species', 'pi pi-fw pi-globe', '/app/species', 0, 'Manage plant species', 1, GETUTCDATE(), 'System'),
('Plant Parts', 'PLANT_PARTS_MENU', NULL, @ForestDataId, 'Plant Parts', 'pi pi-fw pi-bookmark', '/app/plant-parts', 0, 'Manage plant parts', 1, GETUTCDATE(), 'System'),
('Units', 'UNITS_MENU', NULL, @ForestDataId, 'Units', 'pi pi-fw pi-calculator', '/app/units', 0, 'Manage measurement units', 1, GETUTCDATE(), 'System');

-- Location Management Parent Menu
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Location Management', 'LOCATION_MANAGEMENT', NULL, NULL, 'Location Management', 'pi pi-fw pi-map', NULL, 1, 'Geographical location management', 1, GETUTCDATE(), 'System');

DECLARE @LocationMgmtId INT = SCOPE_IDENTITY();

INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Ranges', 'RANGES_MENU', NULL, @LocationMgmtId, 'Ranges', 'pi pi-fw pi-map-marker', '/app/ranges', 0, 'Manage forest ranges', 1, GETUTCDATE(), 'System'),
('Divisions', 'DIVISIONS_MENU', NULL, @LocationMgmtId, 'Divisions', 'pi pi-fw pi-building', '/app/divisions', 0, 'Manage forest divisions', 1, GETUTCDATE(), 'System'),
('Circles', 'CIRCLES_MENU', NULL, @LocationMgmtId, 'Circles', 'pi pi-fw pi-circle', '/app/circles', 0, 'Manage forest circles', 1, GETUTCDATE(), 'System');

-- Applications
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Applications', 'APPLICATIONS_MENU', NULL, NULL, 'Applications', 'pi pi-fw pi-file', '/app/applications', 0, 'Transit pass applications', 1, GETUTCDATE(), 'System');

-- Reports
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Reports', 'REPORTS_MENU', NULL, NULL, 'Reports', 'pi pi-fw pi-chart-bar', '/app/reports', 0, 'View and generate reports', 1, GETUTCDATE(), 'System');

-- Settings
INSERT INTO Master_Menus (Menu_Name, Menu_Code, Module_Id, Parent_Menu_Id, Label, Icon, Router_Link, Is_Parent, Description, Is_Active, Created_Date, Created_By)
VALUES 
('Settings', 'SETTINGS_MENU', NULL, NULL, 'Settings', 'pi pi-fw pi-cog', '/app/settings', 0, 'System settings and configuration', 1, GETUTCDATE(), 'System');

PRINT 'Dynamic Menu System - Initial menus seeded successfully!';
PRINT 'Total menus created: ' + CAST((SELECT COUNT(*) FROM Master_Menus) AS VARCHAR);
PRINT '';
PRINT '⚠️  IMPORTANT: Menus are assigned to ROLES, not modules!';
PRINT 'Module_Id is optional and used only for organizational purposes.';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Verify menus in Master_Menus table';
PRINT '2. Assign menus to ROLES using Role_Menus table or API';
PRINT '3. Example: Assign all menus to Admin role (Role_Id = 1):';
PRINT '   INSERT INTO Role_Menus (Role_Id, Menu_Id, Display_Order, Is_Visible, Is_Active, Created_Date, Created_By)';
PRINT '   SELECT 1, Menu_Id, ROW_NUMBER() OVER (ORDER BY Menu_Id), 1, 1, GETUTCDATE(), ''System''';
PRINT '   FROM Master_Menus WHERE Is_Active = 1;';
PRINT '4. Test menu retrieval via API: GET /api/UserMenus/GetMyMenuTree';
