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

        public async Task<AppTask> AddAsync(AppTask task)
        {
            await _context.AppTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(AppTask task)
        {
            _context.AppTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await GetByIdAsync(id);
            _context.AppTasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppTask>> GetPagedAsync(int skip, int take)
        {
            return await _context.AppTasks
                .OrderBy(t => t.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}

