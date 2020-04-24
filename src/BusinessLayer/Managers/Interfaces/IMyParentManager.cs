using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces
{
    public interface IMyParentManager
    {
        Task<IEnumerable<MyParentEntity>> GetAllAsync();

        Task<IEnumerable<MyParentEntity>> GetAllAsync(CancellationToken token);

        Task<MyParentEntity> GetSingleAsync(Guid keyValue);

        Task<MyParentEntity> GetSingleAsync(Guid keyValue, CancellationToken token);

        Task<MyParentEntity> AddAsync(MyParentEntity entity);

        Task<MyParentEntity> AddAsync(MyParentEntity entity, CancellationToken token);

        Task<MyParentEntity> UpdateAsync(MyParentEntity entity);

        Task<MyParentEntity> UpdateAsync(MyParentEntity entity, CancellationToken token);

        Task<int> DeleteAsync(Guid keyValue);

        Task<int> DeleteAsync(Guid keyValue, CancellationToken token);

        Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithReusedPocoObject();

        Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithReusedPocoObject(CancellationToken token);

        Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithAnonymousClass();

        Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithAnonymousClass(CancellationToken token);

        Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithPrivateClassHolderObject();

        Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithPrivateClassHolderObject(CancellationToken token);
    }
}
