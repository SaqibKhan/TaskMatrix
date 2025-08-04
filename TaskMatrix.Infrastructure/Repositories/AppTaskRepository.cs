using Microsoft.EntityFrameworkCore;
using TaskMatrix.Domain.Entities;
using TaskMatrix.Domain.Interfaces;
using TaskMatrix.Infrastructure.Data;

namespace TaskMatrix.Infrastructure.Repositories
{
    public class AppTaskRepository : IAppTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public AppTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppTask?> GetByIdAsync(int id)
        {
            return await _context.AppTasks.FindAsync(id);
        }

        public async Task<IEnumerable<AppTask>> GetAllAsync()
        {
            return await _context.AppTasks.ToListAsync();
        }

        public async Task<AppTask> AddAsync(AppTask product)
        {
            await _context.AppTasks.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(AppTask product)
        {
            _context.AppTasks.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            _context.AppTasks.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}

