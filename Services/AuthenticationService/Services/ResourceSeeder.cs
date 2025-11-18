using ApplicantAuthenticationService.Models;
using AuthenticationService.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Globalization;

namespace ApplicantAuthenticationService.Services
{
    public class ResourceSeeder
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ResourceSeeder> _logger;
        private readonly string _serviceName;

        public ResourceSeeder(AppDbContext db, ILogger<ResourceSeeder> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _serviceName = config.GetValue<string>("ServiceName") ?? "ApplicantAuthenticationService";
        }

        public async Task SeedFromResxAsync(string culture = "en-US", CancellationToken ct = default)
        {
            var rm = ApplicantAuthenticationService.Resources.ApplicantResources.ResourceManager;
            var ci = CultureInfo.GetCultureInfo(culture);
            var rs = rm.GetResourceSet(ci, true, true);

            if (rs == null)
            {
                _logger.LogWarning("⚠️ No resources found for culture {culture}", culture);
                return;
            }

            int inserted = 0, updated = 0;

            foreach (DictionaryEntry de in rs)
            {
                var key = de.Key?.ToString();
                var value = de.Value?.ToString();
                if (string.IsNullOrWhiteSpace(key)) continue;

                var existing = await _db.ResourceCollection
                    .FirstOrDefaultAsync(r => r.ServiceName == _serviceName && r.ResourceKey == key && r.Culture == culture, ct);

                if (existing == null)
                {
                    _db.ResourceCollection.Add(new ApplicantResourceCollection
                    {
                        ServiceName = _serviceName,
                        ResourceKey = key!,
                        ResourceValue = value,
                        Culture = culture,
                        PageOrModule = ExtractModuleFromKey(key!),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    inserted++;
                }
                else if (existing.ResourceValue != value)
                {
                    existing.ResourceValue = value;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _db.ResourceCollection.Update(existing);
                    updated++;
                }
            }

            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("✅ Resource seeding complete: {inserted} inserted, {updated} updated for {service}", inserted, updated, _serviceName);
        }

        private string? ExtractModuleFromKey(string key)
        {
            var parts = key.Split('.');
            return parts.Length > 0 ? parts[0] : null;
        }
    }
}
