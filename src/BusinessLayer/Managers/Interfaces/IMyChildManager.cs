using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces
{
    public interface IMyChildManager
    {
        Task<IEnumerable<MyChildEntity>> GetAllAsync();

        Task<IEnumerable<MyChildEntity>> GetAllAsync(CancellationToken token);

        Task<MyChildEntity> GetSingleAsync(Guid keyValue);

        Task<MyChildEntity> GetSingleAsync(Guid keyValue, CancellationToken token);

        Task<MyChildEntity> AddAsync(MyChildEntity entity);

        Task<MyChildEntity> AddAsync(MyChildEntity entity, CancellationToken token);

        Task<MyChildEntity> UpdateAsync(MyChildEntity entity);

        Task<MyChildEntity> UpdateAsync(MyChildEntity entity, CancellationToken token);

        Task<int> DeleteAsync(Guid keyValue);

        Task<int> DeleteAsync(Guid keyValue, CancellationToken token);
    }
}
