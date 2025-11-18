# Fixing IP Address Capture - Getting Real IP Instead of ::1

## Problem
You're seeing `::1` (IPv6 loopback) or `127.0.0.1` (IPv4 loopback) instead of the real client IP address.

## Root Cause
When testing locally (localhost), the application sees the connection coming from the local machine, hence the loopback address.

## Solutions Implemented

### 1. Added ForwardedHeaders Middleware
**File**: `Services\OfficerService\Program.cs`

```csharp
using Microsoft.AspNetCore.HttpOverrides;

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.ForwardLimit = 2;
});

// Apply middleware BEFORE other middleware
app.UseForwardedHeaders();
```

### 2. Enhanced IP Detection Logic
**File**: `Services\OfficerService\Controllers\MasterRolesController.cs`

The `GetClientIpAddress()` method now:
- ✅ Checks `X-Forwarded-For` header
- ✅ Checks `X-Real-IP` header
- ✅ Converts IPv4-mapped IPv6 to IPv4
- ✅ Attempts to get local network IP for development
- ✅ Filters out loopback addresses

## How to Get Real IP Addresses

### Scenario 1: Local Development (Testing on Same Machine)

**Problem**: You'll always see `::1` or `127.0.0.1` when testing from the same machine.

**Solution A - Get Your Local Network IP**:
The updated code now attempts to get your actual local network IP (e.g., `192.168.1.100`) even when testing locally.

**Solution B - Test from Another Device**:
```
1. Find your development machine's IP:
   - Windows: Run `ipconfig` in CMD
   - Look for "IPv4 Address" (e.g., 192.168.1.100)

2. Update launchSettings.json to listen on all interfaces:
   "applicationUrl": "http://0.0.0.0:5000;https://0.0.0.0:5001"

3. Test from another device on same network:
   http://192.168.1.100:5000/api/MasterRoles/...
```

### Scenario 2: Behind Nginx Reverse Proxy

**Nginx Configuration**:
```nginx
server {
    listen 80;
    server_name api.example.com;

    location / {
        proxy_pass http://localhost:5000;
        
        # Forward client IP
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Host $host;
    }
}
```

**Expected Result**: Real client IP will be captured from `X-Real-IP` or `X-Forwarded-For`.

### Scenario 3: Behind IIS as Reverse Proxy

**IIS Application Request Routing (ARR) Configuration**:

1. Install ARR module
2. Configure Server Farms
3. Enable "Preserve client IP in the following header": `X-Forwarded-For`

**web.config**:
```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="ReverseProxyInboundRule">
        <match url="(.*)" />
        <action type="Rewrite" url="http://localhost:5000/{R:1}" />
        <serverVariables>
          <set name="HTTP_X_FORWARDED_FOR" value="{REMOTE_ADDR}" />
          <set name="HTTP_X_REAL_IP" value="{REMOTE_ADDR}" />
        </serverVariables>
      </rule>
    </rules>
  </rewrite>
</system.webServer>
```

### Scenario 4: Azure App Service / Azure Application Gateway

Azure automatically adds forwarded headers. The middleware will handle them automatically.

**No additional configuration needed!**

### Scenario 5: Docker Container

**docker-compose.yml**:
```yaml
services:
  officerservice:
    image: officerservice:latest
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
```

**In code** (already added):
The `UseForwardedHeaders()` middleware handles this.

## Testing IP Capture

### Test 1: Local Development
```bash
# Your code will now attempt to get local network IP
# Expected: 192.168.x.x or 10.0.x.x (your local IP)
# Instead of: ::1 or 127.0.0.1
```

### Test 2: With Custom Header (Simulate Proxy)
```bash
curl -X PUT https://localhost:5001/api/MasterRoles/UpdateRole/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "X-Forwarded-For: 203.0.113.1" \
  -d '{"roleName":"TestRole","isActive":true}'
```
**Expected IP in logs**: `203.0.113.1`

### Test 3: From Another Device
```bash
# From another computer/phone on same network
curl -X PUT http://192.168.1.100:5000/api/MasterRoles/UpdateRole/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"roleName":"TestRole","isActive":true}'
```
**Expected IP in logs**: The other device's IP (e.g., `192.168.1.50`)

### Test 4: Check Database
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

## Troubleshooting

### Issue 1: Still Getting ::1 or 127.0.0.1

**Causes**:
1. Testing from the same machine (localhost)
2. ForwardedHeaders middleware not applied
3. Proxy not sending headers

**Solutions**:
```csharp
// Check if middleware is applied in correct order
app.UseForwardedHeaders();  // ← MUST be first
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
```

### Issue 2: Getting Proxy IP Instead of Client IP

**Cause**: Proxy not configured to forward client IP

**Solution**: Configure proxy to send `X-Forwarded-For` header (see Nginx/IIS examples above)

### Issue 3: IPv6 Format Issues

**Cause**: IPv4-mapped IPv6 addresses like `::ffff:192.168.1.1`

**Solution**: Already handled in updated code:
```csharp
if (remoteIp.IsIPv4MappedToIPv6)
{
    remoteIp = remoteIp.MapToIPv4();
}
```

### Issue 4: Security - Accepting Headers from Unknown Sources

**Current Setting** (Development):
```csharp
options.KnownNetworks.Clear();  // Accepts from any source
options.KnownProxies.Clear();
```

**Production Recommendation**:
```csharp
// Only accept forwarded headers from known proxies
options.KnownProxies.Add(IPAddress.Parse("10.0.0.1"));  // Your load balancer IP
options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));  // Your network
```

## Update launchSettings.json for Network Testing

**File**: `Services\OfficerService\Properties\launchSettings.json`

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://0.0.0.0:5000",  // ← Listen on all interfaces
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://0.0.0.0:5001;http://0.0.0.0:5000",  // ← Listen on all interfaces
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

**Note**: Change `0.0.0.0` back to `localhost` after testing for security.

## IP Address Format Examples

| Scenario | IP Format | Example |
|----------|-----------|---------|
| Local Development (Original) | IPv6 Loopback | `::1` |
| Local Development (Updated) | Private IPv4 | `192.168.1.100` |
| Through Proxy | Public IPv4 | `203.0.113.1` |
| Direct IPv6 | Full IPv6 | `2001:0db8:85a3::8a2e:0370:7334` |
| IPv4-Mapped IPv6 (Converted) | IPv4 | `192.168.1.100` |

## Verification Checklist

✅ **Restart the application** after code changes
✅ **Check middleware order** - `UseForwardedHeaders()` must be first
✅ **Test from different device** to see real IP capture
✅ **Check database** to verify IPs are stored correctly
✅ **Configure proxy** to send forwarded headers (if applicable)
✅ **Update security settings** for production (known proxies only)

## Quick Test Commands

```bash
# 1. Get your local IP
ipconfig  # Windows
ifconfig  # Linux/Mac

# 2. Start application
cd Services\OfficerService
dotnet run

# 3. Test from another device (replace IP and token)
curl -X PUT http://YOUR_DEV_MACHINE_IP:5000/api/MasterRoles/UpdateRole/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "X-Forwarded-For: 203.0.113.1" \
  -d '{"roleName":"TestRole","isActive":true}'

# 4. Check logs in database
# Should see 203.0.113.1 or your device's IP
```

## Expected Behavior After Fix

| Test Scenario | Before | After |
|---------------|--------|-------|
| Local browser (localhost) | `::1` | Your local network IP (e.g., `192.168.1.100`) |
| From another device | `::1` | Other device's IP (e.g., `192.168.1.50`) |
| Through proxy with X-Forwarded-For | `::1` or Proxy IP | Original client IP from header |
| Production (Azure/AWS) | Load balancer IP | Real client IP from forwarded headers |

## Security Best Practices

1. **In Production**: Configure `KnownProxies` and `KnownNetworks`
2. **Limit ForwardLimit**: Already set to 2 to prevent header injection
3. **Validate IP Format**: Consider adding IP validation before storing
4. **Privacy Compliance**: Implement IP anonymization if required by GDPR/regulations
5. **Log Retention**: Set appropriate retention policy for IP addresses

## Status
✅ ForwardedHeaders middleware configured
✅ Enhanced IP detection logic implemented
✅ IPv4/IPv6 handling improved
✅ Local network IP detection added
✅ Code compiles successfully
⚠️ **Restart application to apply changes**

## Next Steps

1. **Stop and restart** the OfficerService application
2. **Test with X-Forwarded-For header** using curl or Postman
3. **Test from another device** on your network
4. **Verify in database** that real IPs are captured
5. **Configure production proxies** to send forwarded headers
