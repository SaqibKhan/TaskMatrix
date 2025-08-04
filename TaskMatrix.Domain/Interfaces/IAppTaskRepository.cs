using TaskMatrix.Domain.Entities;

namespace TaskMatrix.Domain.Interfaces
{
    public interface IAppTaskRepository
    {
        Task<AppTask> GetByIdAsync(int id);
        Task<IEnumerable<AppTask>> GetAllAsync();
        Task<AppTask> AddAsync(AppTask product);
        Task UpdateAsync(AppTask product);
        Task DeleteAsync(int id);
    }
}
