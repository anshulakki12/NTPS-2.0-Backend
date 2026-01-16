using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class DocumentMasterRepository : IDocumentMasterRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DocumentMasterRepository> _logger;

        public DocumentMasterRepository(AppDbContext context, ILogger<DocumentMasterRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DocumentMaster>> GetByStateCode(int stateCode)
        {
            return await _context.DocumentMasters
                .Where(d => d.StateCode == stateCode)
                .OrderBy(d => d.DocumentTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentMaster>> GetAll()
        {
            return await _context.DocumentMasters
                .OrderBy(d => d.StateCode)
                .ThenBy(d => d.DocumentTypeId)
                .ToListAsync();
        }

        public async Task<DocumentMaster> GetById(int id)
        {
            return await _context.DocumentMasters.FindAsync(id);
        }

        public async Task Create(DocumentMaster documentMaster)
        {
            documentMaster.CreatedDate = DateTime.UtcNow;
            _context.DocumentMasters.Add(documentMaster);
            await _context.SaveChangesAsync();
        }

        public async Task Update(DocumentMaster documentMaster)
        {
            documentMaster.ModifiedDate = DateTime.UtcNow;
            _context.DocumentMasters.Update(documentMaster);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var document = await _context.DocumentMasters.FindAsync(id);
            if (document != null)
            {
                _context.DocumentMasters.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int stateCode, string documentTypeId)
        {
            return await _context.DocumentMasters
                .AnyAsync(d => d.StateCode == stateCode && d.DocumentTypeId == documentTypeId);
        }

        public async Task CreateOrUpdateBulk(IEnumerable<DocumentMaster> documents)
        {
            foreach (var doc in documents)
            {
                var existing = await _context.DocumentMasters
                    .FirstOrDefaultAsync(d =>
                        d.StateCode == doc.StateCode &&
                        d.DocumentTypeId == doc.DocumentTypeId);

                if (existing != null)
                {
                    existing.IsActive = doc.IsActive;
                    existing.IsMandatory = doc.IsMandatory;
                    existing.ModifiedDate = DateTime.UtcNow;
                    existing.ModifiedBy = doc.ModifiedBy;
                    _context.DocumentMasters.Update(existing);
                }
                else
                {
                    doc.CreatedDate = DateTime.UtcNow;
                    _context.DocumentMasters.Add(doc);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DocumentMaster>> GetDefaultDocuments()
        {
            return new List<DocumentMaster>
        {
            new DocumentMaster
            {
                DocumentTypeId = "DOC001",
                DocumentTypeName = "Photo of the Forest Produced",
                IsActive = true,
                IsMandatory = true
            },
            new DocumentMaster
            {
                DocumentTypeId = "DOC002",
                DocumentTypeName = "Proof of ownership(Land Revenue Records)",
                IsActive = true,
                IsMandatory = true
            },
            new DocumentMaster
            {
                DocumentTypeId = "DOC003",
                DocumentTypeName = "Document of felling order",
                IsActive = false,
                IsMandatory = false
            },
            new DocumentMaster
            {
                DocumentTypeId = "DOC004",
                DocumentTypeName = "Any Other document",
                IsActive = true,
                IsMandatory = false
            }
        };
        }
    }
}
