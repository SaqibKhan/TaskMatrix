using TaskMatrix.Application.DTOs;
using TaskMatrix.Application.Interfaces;
using TaskMatrix.Domain.Entities;
using TaskMatrix.Domain.Interfaces;

namespace TaskMatrix.Application.Services
{
    public class AppTaskService : IAppTaskService
    {
        private readonly IAppTaskRepository _appTaskRepository;
        public AppTaskService(IAppTaskRepository appTaskRepository)
        {
            _appTaskRepository = appTaskRepository;
        }
        public async Task<AppTaskDto> GetByIdAsync(int id)
        {
            var appTask = await _appTaskRepository.GetByIdAsync(id);
            return new AppTaskDto(appTask.Id, appTask.Title, appTask.Description, appTask.Priority, appTask.DueDate, appTask.Status);
        }
        public async Task<IEnumerable<AppTaskDto>> GetAllAsync()
        {
            var appTasks = await _appTaskRepository.GetAllAsync();
            return appTasks.Select(p => new AppTaskDto(p.Id, p.Title, p.Description, p.Priority, p.DueDate, p.Status));
        }
        public async Task<AppTaskDto> CreateAsync(CreateAppTaskDto dto)
        {
            var appTask = new AppTask(0, dto.Title, dto.Description, dto.Priority, dto.DueDate, dto.Status);
            var created = await _appTaskRepository.AddAsync(appTask);
            return new AppTaskDto(created.Id, created.Title, created.Description, created.Priority, created.DueDate, created.Status);
        }
        public async Task UpdateAsync(UpdateAppTaskDto dto)
        {
            var appTask = await _appTaskRepository.GetByIdAsync(dto.Id);
            appTask.UpdateAppTask(dto.Title, dto.Description, (int)dto.Priority, dto.Status);
            await _appTaskRepository.UpdateAsync(appTask);
        }
        public async Task DeleteAsync(int id)
        {
            await _appTaskRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<AppTaskDto>> GetPagedAsync(int skip, int take)
        {
            var appTasks = await _appTaskRepository.GetPagedAsync(skip, take);
            return appTasks.Select(p => new AppTaskDto(p.Id, p.Title, p.Description, p.Priority, p.DueDate, p.Status));
        }
    }
}

