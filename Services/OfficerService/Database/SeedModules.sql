-- ====================================================
-- Module Management - Initial Modules Seeding Script
-- ====================================================

-- This script creates initial modules for the RBAC system
-- Execute this after creating the Master_Modules table

-- ====================================================
-- INITIAL MODULES
-- ====================================================

INSERT INTO Master_Modules (Module_Name, Module_Code, Description, Icon, Route_Path, Display_Order, Is_Active, Created_Date, Created_By)
VALUES 
-- Dashboard
('Dashboard', 'DASHBOARD', 'Main dashboard with statistics and overview', 'pi pi-fw pi-chart-line', '/app/dashboard', 10, 1, GETUTCDATE(), 'System'),

-- User Management
('User Management', 'USER_MANAGEMENT', 'Manage system users and their accounts', 'pi pi-fw pi-users', '/app/users', 20, 1, GETUTCDATE(), 'System'),

-- Role Management
('Role Management', 'ROLE_MANAGEMENT', 'Manage user roles and access levels', 'pi pi-fw pi-user-edit', '/app/roles', 30, 1, GETUTCDATE(), 'System'),

-- Designation Management
('Designation Management', 'DESIGNATION_MANAGEMENT', 'Manage officer designations and ranks', 'pi pi-fw pi-id-card', '/app/designation', 40, 1, GETUTCDATE(), 'System'),

-- Permission Management
('Permission Management', 'PERMISSION_MANAGEMENT', 'Manage system permissions and access control', 'pi pi-fw pi-shield', '/app/permissions', 50, 1, GETUTCDATE(), 'System'),

-- Module Management
('Module Management', 'MODULE_MANAGEMENT', 'Manage application modules and features', 'pi pi-fw pi-th-large', '/app/module', 60, 1, GETUTCDATE(), 'System'),

-- Forest Produces
('Forest Produces', 'FOREST_PRODUCES', 'Manage forest produce types and categories', 'pi pi-fw pi-sitemap', '/app/forest-produces', 100, 1, GETUTCDATE(), 'System'),

-- Species Management
('Species Management', 'SPECIES_MANAGEMENT', 'Manage plant and tree species information', 'pi pi-fw pi-globe', '/app/species', 110, 1, GETUTCDATE(), 'System'),

-- Plant Part Management
('Plant Part Management', 'PLANT_PART_MANAGEMENT', 'Manage plant parts and their characteristics', 'pi pi-fw pi-bookmark', '/app/plant-parts', 120, 1, GETUTCDATE(), 'System'),

-- Unit Management
('Unit Management', 'UNIT_MANAGEMENT', 'Manage measurement units', 'pi pi-fw pi-calculator', '/app/units', 130, 1, GETUTCDATE(), 'System'),

-- Range Management
('Range Management', 'RANGE_MANAGEMENT', 'Manage forest ranges and boundaries', 'pi pi-fw pi-map-marker', '/app/ranges', 200, 1, GETUTCDATE(), 'System'),

-- Division Management
('Division Management', 'DIVISION_MANAGEMENT', 'Manage forest divisions', 'pi pi-fw pi-building', '/app/divisions', 210, 1, GETUTCDATE(), 'System'),

-- Circle Management
('Circle Management', 'CIRCLE_MANAGEMENT', 'Manage forest circles', 'pi pi-fw pi-circle', '/app/circles', 220, 1, GETUTCDATE(), 'System'),

-- Application Management
('Application Management', 'APPLICATION_MANAGEMENT', 'Manage transit pass applications', 'pi pi-fw pi-file', '/app/applications', 300, 1, GETUTCDATE(), 'System'),

-- Reports
('Reports', 'REPORTS', 'View and generate system reports', 'pi pi-fw pi-chart-bar', '/app/reports', 400, 1, GETUTCDATE(), 'System'),

-- Settings
('Settings', 'SETTINGS', 'System configuration and settings', 'pi pi-fw pi-cog', '/app/settings', 500, 1, GETUTCDATE(), 'System');

PRINT 'Module Management - Initial modules seeded successfully!';
PRINT 'Total modules created: ' + CAST((SELECT COUNT(*) FROM Master_Modules) AS VARCHAR);
