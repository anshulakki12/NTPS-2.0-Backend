# IP Address Capture Implementation - Summary

## Overview
IP address capture has been successfully implemented for all Master_Roles logging operations (UPDATE, DELETE, DEACTIVATE, REACTIVATE).

## Changes Made

### 1. Service Interface Updated
**File**: `Services\OfficerService\Services\IMasterRoleService.cs`

Added optional `ipAddress` parameter to all modification methods:
```csharp
Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto, string? ipAddress = null);
Task<bool> DeleteRoleAsync(int roleId, string performedBy = "System", string? ipAddress = null);
Task<RoleResponseDto> DeactivateRoleAsync(int roleId, string deactivatedBy, string? ipAddress = null);
Task<RoleResponseDto> ReactivateRoleAsync(int roleId, string reactivatedBy, string? ipAddress = null);
```

### 2. Service Implementation Updated
**File**: `Services\OfficerService\Services\MasterRoleService.cs`

All logging methods now pass the IP address to the logging service:
- `LogUpdateAsync()` - receives and passes ipAddress
- `LogDeleteAsync()` - receives and passes ipAddress
- `LogDeactivateAsync()` - receives and passes ipAddress
- `LogReactivateAsync()` - receives and passes ipAddress

### 3. Controller Updated
**File**: `Services\OfficerService\Controllers\MasterRolesController.cs`

#### New Helper Method Added:
```csharp
private string? GetClientIpAddress()
{
    // Check for forwarded IP (when behind proxy/load balancer)
    var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
    if (!string.IsNullOrEmpty(forwardedFor))
    {
        return forwardedFor.Split(',').FirstOrDefault()?.Trim();
    }

    // Check X-Real-IP header
    var realIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
    if (!string.IsNullOrEmpty(realIp))
    {
        return realIp;
    }

    // Fall back to RemoteIpAddress
    return HttpContext.Connection.RemoteIpAddress?.ToString();
}
```

#### Updated Controller Actions:
All modification endpoints now capture and pass IP address:
- `UpdateRole()` - captures IP and passes to service
- `DeactivateRole()` - captures IP and passes to service
- `ReactivateRole()` - captures IP and passes to service
- `DeleteRole()` - captures IP and passes to service

## How IP Address is Captured

The `GetClientIpAddress()` helper method follows a hierarchical approach to get the most accurate IP:

1. **X-Forwarded-For Header** (Priority 1)
   - Used when the application is behind a proxy, load balancer, or CDN
   - Takes the first IP from the comma-separated list (original client IP)
   - Example: `"203.0.113.1, 198.51.100.17"`

2. **X-Real-IP Header** (Priority 2)
   - Common alternative header used by reverse proxies (like Nginx)
   - Contains the original client IP
   - Example: `"203.0.113.1"`

3. **RemoteIpAddress** (Priority 3)
   - Falls back to the direct connection IP
   - May be a proxy IP if behind load balancer
   - Example: `"192.168.1.100"`

## IP Address Storage

The IP address is stored in the `Master_Roles_Logs` table in the `IP_Address` column:
- **Column Type**: `nvarchar(50)`
- **Nullable**: Yes
- **Format Examples**:
  - IPv4: `"192.168.1.100"`
  - IPv6: `"2001:0db8:85a3:0000:0000:8a2e:0370:7334"`
  - IPv6 Mapped IPv4: `"::ffff:192.168.1.100"`

## Usage Examples

### When Behind a Proxy/Load Balancer

**Request Headers:**
```
X-Forwarded-For: 203.0.113.1, 198.51.100.17
X-Real-IP: 203.0.113.1
```

**Captured IP:** `"203.0.113.1"` (original client IP)

### Direct Connection

**Request:**
Direct HTTP request from client IP `192.168.1.100`

**Captured IP:** `"192.168.1.100"`

### Example Log Entry with IP

```json
{
  "logId": 25,
  "roleId": 5,
  "roleName": "Manager",
  "operationType": "UPDATE",
  "operationDate": "2025-01-15T10:30:00Z",
  "operationBy": "admin@example.com",
  "oldRoleName": "Manager",
  "newRoleName": "Senior Manager",
  "ipAddress": "203.0.113.1",
  "remarks": null
}
```

## Benefits

1. **Security Auditing**: Track which IP addresses made changes
2. **Fraud Detection**: Identify suspicious activity from unusual IPs
3. **Compliance**: Meet regulatory requirements for audit trails
4. **Troubleshooting**: Correlate issues with specific clients/locations
5. **Geolocation**: Can be used for geographic analysis of operations
6. **Access Patterns**: Understand where users are accessing from

## Configuration for Load Balancers

### Nginx Configuration
```nginx
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
```

### IIS Configuration
Ensure Application Request Routing (ARR) is configured to preserve client IP:
```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="AddForwardedHeader">
        <serverVariables>
          <set name="HTTP_X_FORWARDED_FOR" value="{REMOTE_ADDR}" />
        </serverVariables>
      </rule>
    </rules>
  </rewrite>
</system.webServer>
```

### Azure Application Gateway / Azure Front Door
These services automatically add `X-Forwarded-For` headers.

## Testing

To test IP capture:

1. **Direct Access** (Development):
   ```
   PUT https://localhost:5001/api/MasterRoles/UpdateRole/1
   ```
   - Expected IP: `::1` or `127.0.0.1` (localhost)

2. **Through Proxy** (Production):
   ```
   PUT https://api.example.com/api/MasterRoles/UpdateRole/1
   X-Forwarded-For: 203.0.113.1
   ```
   - Expected IP: `203.0.113.1`

3. **Verify in Database**:
   ```sql
   SELECT TOP 10 
       Log_Id,
       Role_Id,
       Operation_Type,
       Operation_By,
       IP_Address,
       Operation_Date
   FROM Master_Roles_Logs
   ORDER BY Operation_Date DESC
   ```

## Privacy Considerations

⚠️ **Important**: IP addresses are considered Personally Identifiable Information (PII) in some jurisdictions (e.g., GDPR):

1. **Data Retention**: Consider implementing IP address anonymization or deletion after a certain period
2. **Access Control**: Restrict who can view IP addresses in logs
3. **Privacy Policy**: Update your privacy policy to mention IP logging
4. **Anonymization Option**: Consider storing only the first 3 octets (e.g., `192.168.1.xxx`)

## Status
✅ IP Address capture implemented
✅ All modification operations updated
✅ Build successful (code compiles correctly)
⚠️ Application needs restart to apply changes

## Next Steps

1. **Restart the Application**: Stop and restart OfficerService to apply the changes
2. **Test IP Capture**: Perform UPDATE/DELETE/DEACTIVATE/REACTIVATE operations
3. **Verify Logs**: Check Master_Roles_Logs table to confirm IP addresses are being captured
4. **Configure Proxy Headers**: If behind a load balancer, ensure X-Forwarded-For is configured
5. **Review Privacy Policy**: Ensure compliance with data protection regulations
