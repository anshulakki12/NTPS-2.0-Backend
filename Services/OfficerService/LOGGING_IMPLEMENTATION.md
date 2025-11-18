# Master Roles Logging System - Implementation Summary

## Overview
A comprehensive audit logging system has been implemented for the Master_Roles table to track all UPDATE, DELETE, DEACTIVATE, and REACTIVATE operations.

## Changes Made

### 1. Database Model
- **File**: `Services\OfficerService\Models\MasterRolesLogs.cs`
- **Table**: Master_Roles_Logs
- **Purpose**: Store audit logs for all role operations

#### Key Fields:
- **Log_Id**: Primary key (auto-increment)
- **Role_Id**: Reference to the role
- **Operation_Type**: UPDATE, DELETE, DEACTIVATE, REACTIVATE
- **Operation_Date**: Timestamp of the operation
- **Operation_By**: User who performed the operation
- **Old_Role_Name** / **New_Role_Name**: Track name changes
- **Old_Is_Active** / **New_Is_Active**: Track status changes
- **Remarks**: Optional notes
- **IP_Address**: Optional IP tracking

### 2. Repository Layer
- **Interface**: `Services\OfficerService\Repositories\IMasterRolesLogsRepository.cs`
- **Implementation**: `Services\OfficerService\Repositories\MasterRolesLogsRepository.cs`

#### Available Methods:
- `CreateLogAsync(MasterRolesLogs log)`: Create a new log entry
- `GetLogsByRoleIdAsync(int roleId)`: Get all logs for a specific role
- `GetAllLogsAsync()`: Get all logs
- `GetLogsByOperationTypeAsync(string operationType)`: Filter by operation type
- `GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)`: Filter by date range

### 3. Service Layer
- **Interface**: `Services\OfficerService\Services\IMasterRolesLogsService.cs`
- **Implementation**: `Services\OfficerService\Services\MasterRolesLogsService.cs`

#### Key Methods:
- `LogUpdateAsync()`: Log UPDATE operations
- `LogDeleteAsync()`: Log DELETE operations
- `LogDeactivateAsync()`: Log DEACTIVATE operations
- `LogReactivateAsync()`: Log REACTIVATE operations
- `GetLogsByRoleIdAsync()`: Retrieve logs by role
- `GetAllLogsAsync()`: Retrieve all logs

### 4. Updated MasterRoleService
- **File**: `Services\OfficerService\Services\MasterRoleService.cs`
- **Changes**: Integrated logging calls in all modification operations

#### Operations Now Logged:
1. **UPDATE** - When role name or status is changed
2. **DELETE** - When a role is permanently deleted
3. **DEACTIVATE** - When a role is deactivated
4. **REACTIVATE** - When a role is reactivated

### 5. Log Viewing Controller
- **File**: `Services\OfficerService\Controllers\MasterRolesLogsController.cs`
- **Endpoints**:
  - `GET api/MasterRolesLogs/GetAllLogs` - Get all logs (Admin only)
  - `GET api/MasterRolesLogs/GetLogsByRoleId/{roleId}` - Get logs for specific role (Admin only)

### 6. Dependency Injection
- **File**: `Services\OfficerService\Program.cs`
- **Added**:
  ```csharp
  builder.Services.AddScoped<IMasterRolesLogsRepository, MasterRolesLogsRepository>();
  builder.Services.AddScoped<IMasterRolesLogsService, MasterRolesLogsService>();
  ```

### 7. Database Migration
- **Migration**: `20251015090202_AddMasterRolesLogsTable`
- **Status**: ✅ Applied to database
- **Table Created**: Master_Roles_Logs

## How It Works

### 1. UPDATE Operation
When a role is updated via `PUT api/MasterRoles/UpdateRole/{id}`:
- The old role state is captured
- Role is updated in the database
- A log entry is created with:
  - Operation_Type: "UPDATE"
  - Old and new values for changed fields
  - User who performed the update
  - Timestamp

### 2. DELETE Operation
When a role is deleted via `DELETE api/MasterRoles/DeleteRole/{id}`:
- Role details are captured before deletion
- Role is removed from the database
- A log entry is created with:
  - Operation_Type: "DELETE"
  - All role details preserved
  - User who performed the deletion
  - Remarks: "Role permanently deleted"

### 3. DEACTIVATE Operation
When a role is deactivated via `PATCH api/MasterRoles/DeactivateRole/{id}`:
- Role status is changed to inactive
- Deactivated_On timestamp is set
- A log entry is created with:
  - Operation_Type: "DEACTIVATE"
  - Status change (true → false)
  - User who deactivated
  - Remarks: "Role deactivated"

### 4. REACTIVATE Operation
When a role is reactivated via `PATCH api/MasterRoles/ActivateRole/{id}`:
- Role status is changed to active
- Reactivated_On timestamp is set
- A log entry is created with:
  - Operation_Type: "REACTIVATE"
  - Status change (false → true)
  - User who reactivated
  - Remarks: "Role reactivated"

## Benefits

1. **Complete Audit Trail**: Every modification is logged with who, what, when, and why
2. **Historical Data**: All previous states are preserved
3. **Compliance**: Meets audit requirements for tracking changes
4. **Investigation**: Easy to trace back changes and identify issues
5. **Accountability**: Clear record of who made each change
6. **Reversibility**: Historical data can help understand and potentially reverse changes

## API Endpoints Summary

### Existing Endpoints (Enhanced with Logging):
- `PUT api/MasterRoles/UpdateRole/{id}` - Updates and logs
- `DELETE api/MasterRoles/DeleteRole/{id}` - Deletes and logs
- `PATCH api/MasterRoles/DeactivateRole/{id}` - Deactivates and logs
- `PATCH api/MasterRoles/ActivateRole/{id}` - Reactivates and logs

### New Log Viewing Endpoints:
- `GET api/MasterRolesLogs/GetAllLogs` - View all logs (Admin only)
- `GET api/MasterRolesLogs/GetLogsByRoleId/{roleId}` - View logs for specific role (Admin only)

## Example Log Entry

```json
{
  "logId": 1,
  "roleId": 5,
  "roleName": "Manager",
  "isActive": false,
  "operationType": "DEACTIVATE",
  "operationDate": "2025-01-15T10:30:00Z",
  "operationBy": "admin@example.com",
  "oldRoleName": "Manager",
  "newRoleName": "Manager",
  "oldIsActive": true,
  "newIsActive": false,
  "createdDate": "2025-01-01T08:00:00Z",
  "createdBy": "system",
  "updatedDate": "2025-01-15T10:30:00Z",
  "updatedBy": "admin@example.com",
  "deactivatedOn": "2025-01-15T10:30:00Z",
  "reactivatedOn": null,
  "remarks": "Role deactivated",
  "ipAddress": null
}
```

## Future Enhancements (Optional)

1. **IP Address Tracking**: Capture user IP addresses from HttpContext
2. **Remarks Parameter**: Allow users to add custom remarks when modifying roles
3. **Log Retention Policy**: Implement archival/cleanup of old logs
4. **Advanced Filtering**: Add more filtering options (by date, user, operation type)
5. **Export Functionality**: Export logs to CSV/Excel
6. **Real-time Notifications**: Alert admins of critical operations
7. **Log Analytics**: Dashboard showing statistics and trends

## Testing

To test the logging system:

1. **Update a role**: Change the role name or status
2. **Deactivate a role**: Mark a role as inactive
3. **Reactivate a role**: Mark an inactive role as active
4. **Delete a role**: Permanently remove a role
5. **View logs**: Use the log endpoints to verify all operations were logged

All operations should automatically create corresponding log entries in the Master_Roles_Logs table.

## Status
✅ All files created
✅ Migration applied
✅ Services registered
✅ Build successful
✅ Ready for testing
