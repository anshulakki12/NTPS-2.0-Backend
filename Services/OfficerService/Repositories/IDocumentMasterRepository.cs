using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IDocumentMasterRepository
    {
        Task<IEnumerable<DocumentMaster>> GetByStateCode(int stateCode);
        Task<IEnumerable<DocumentMaster>> GetAll();
        Task<DocumentMaster> GetById(int id);
        Task Create(DocumentMaster documentMaster);
        Task Update(DocumentMaster documentMaster);
        Task Delete(int id);
        Task<bool> Exists(int stateCode, string documentTypeId);
        Task CreateOrUpdateBulk(IEnumerable<DocumentMaster> documents);
        Task<IEnumerable<DocumentMaster>> GetDefaultDocuments();
    }
}
