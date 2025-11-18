-- ============================================
-- Role-Based Menu System - Verification Script
-- ============================================
-- Run this script to verify the changes

-- 1. Check if Role_Id and Has_Children columns exist
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Master_Menus'
AND COLUMN_NAME IN ('Role_Id', 'Has_Children')
ORDER BY ORDINAL_POSITION;

-- Expected Output:
-- Role_Id       int     NO      NULL
-- Has_Children  bit     NO      ((0))

-- ============================================

-- 2. Check foreign key constraint
SELECT 
    fk.name AS FK_Name,
    OBJECT_NAME(fk.parent_object_id) AS Table_Name,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS Column_Name,
    OBJECT_NAME (fk.referenced_object_id) AS Referenced_Table,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS Referenced_Column
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc 
    ON fk.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Master_Menus'
AND COL_NAME(fc.parent_object_id, fc.parent_column_id) = 'Role_Id';

-- Expected Output:
-- FK_Master_Menus_Master_Roles_Role_Id | Master_Menus | Role_Id | Master_Roles | Role_Id

-- ============================================

-- 3. Check index on Role_Id
SELECT 
    i.name AS Index_Name,
    OBJECT_NAME(i.object_id) AS Table_Name,
    COL_NAME(ic.object_id, ic.column_id) AS Column_Name,
    i.type_desc AS Index_Type
FROM sys.indexes AS i
INNER JOIN sys.index_columns AS ic 
    ON i.object_id = ic.object_id 
    AND i.index_id = ic.index_id
WHERE OBJECT_NAME(i.object_id) = 'Master_Menus'
AND COL_NAME(ic.object_id, ic.column_id) = 'Role_Id';

-- Expected Output:
-- IX_Master_Menus_Role_Id | Master_Menus | Role_Id | NONCLUSTERED

-- ============================================

-- 4. Verify all existing menus have Role_Id assigned
SELECT 
    COUNT(*) AS Total_Menus,
    COUNT(CASE WHEN Role_Id IS NULL THEN 1 END) AS Null_Role_Id,
    COUNT(CASE WHEN Role_Id IS NOT NULL THEN 1 END) AS Valid_Role_Id
FROM Master_Menus;

-- Expected Output:
-- Total_Menus should equal Valid_Role_Id, Null_Role_Id should be 0

-- ============================================

-- 5. View menus with role information
SELECT 
    m.Menu_Id,
    m.Menu_Name,
    m.Menu_Code,
    m.Role_Id,
    r.Role_Name,
    m.Label,
    m.Icon,
    m.Router_Link,
    m.Is_Parent,
    m.Has_Children,
    m.Is_Active
FROM Master_Menus m
INNER JOIN Master_Roles r ON m.Role_Id = r.Role_Id
ORDER BY r.Role_Name, m.Label;

-- ============================================

-- 6. Count menus by role
SELECT 
    r.Role_Id,
    r.Role_Name,
    COUNT(m.Menu_Id) AS Total_Menus,
    COUNT(CASE WHEN m.Is_Active = 1 THEN 1 END) AS Active_Menus,
    COUNT(CASE WHEN m.Is_Parent = 1 THEN 1 END) AS Parent_Menus,
    COUNT(CASE WHEN m.Parent_Menu_Id IS NULL THEN 1 END) AS Root_Menus
FROM Master_Roles r
LEFT JOIN Master_Menus m ON r.Role_Id = m.Role_Id
GROUP BY r.Role_Id, r.Role_Name
ORDER BY r.Role_Name;

-- ============================================

-- 7. Check for orphaned menus (menus with non-existent Role_Id)
SELECT 
    m.Menu_Id,
    m.Menu_Name,
    m.Role_Id
FROM Master_Menus m
WHERE NOT EXISTS (
    SELECT 1 
    FROM Master_Roles r 
    WHERE r.Role_Id = m.Role_Id
);

-- Expected Output: No rows (empty result set)

-- ============================================

-- 8. Verify parent-child relationships
SELECT 
    p.Menu_Id AS Parent_Id,
    p.Menu_Name AS Parent_Menu,
    p.Role_Id AS Parent_Role_Id,
    COUNT(c.Menu_Id) AS Child_Count
FROM Master_Menus p
LEFT JOIN Master_Menus c ON p.Menu_Id = c.Parent_Menu_Id
WHERE p.Is_Parent = 1
GROUP BY p.Menu_Id, p.Menu_Name, p.Role_Id
ORDER BY Parent_Menu;

-- ============================================

-- 9. Check migration history
SELECT 
    MigrationId,
    ProductVersion
FROM __EFMigrationsHistory
WHERE MigrationId LIKE '%AddRoleIdToMenusAndHasChildren%'
OR MigrationId LIKE '%UpdateMenuAndRBACTables%';

-- Expected Output:
-- 20251018154329_UpdateMenuAndRBACTables
-- 20251018164242_AddRoleIdToMenusAndHasChildren

-- ============================================

-- 10. Test query: Get menus for a specific role (e.g., Admin - Role_Id = 1)
SELECT 
    m.Menu_Id,
    m.Menu_Name,
    m.Label,
    m.Icon,
    m.Router_Link,
    m.Parent_Menu_Id,
    pm.Menu_Name AS Parent_Menu_Name,
    m.Is_Active
FROM Master_Menus m
LEFT JOIN Master_Menus pm ON m.Parent_Menu_Id = pm.Menu_Id
WHERE m.Role_Id = 1  -- Change this to test different roles
  AND m.Is_Active = 1
ORDER BY 
    CASE WHEN m.Parent_Menu_Id IS NULL THEN 0 ELSE 1 END,
    m.Label;

-- ============================================
-- ✅ All checks complete!
-- ============================================
