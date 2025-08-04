using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMatrix.Application.DTOs;

namespace TaskMatrix.Application.Interfaces
{
    public interface IAppTaskService
    {
        Task<AppTaskDto> GetByIdAsync(int id);
        Task<IEnumerable<AppTaskDto>> GetAllAsync();
        Task<IEnumerable<AppTaskDto>> GetPagedAsync(int skip, int take);
        Task<AppTaskDto> CreateAsync(CreateAppTaskDto dto);
        Task UpdateAsync(UpdateAppTaskDto dto);
        Task DeleteAsync(int id);
    }
}
