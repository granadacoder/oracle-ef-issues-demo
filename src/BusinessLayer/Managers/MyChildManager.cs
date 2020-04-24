using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.Interfaces;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers
{
    public class MyChildManager : IMyChildManager
    {
        public const string ErrorMessageILoggerFactoryWrapperIsNull = "ILoggerFactoryWrapper is null";
        public const string ErrorMessageIMyChildDomainDataIsNull = "IMyChildDomainData is null";
        private readonly ILogger<MyChildManager> logger;
        private readonly IMyChildDomainData myChildDomainData;

        public MyChildManager(ILoggerFactory loggerFactory, IMyChildDomainData myChildDomainData)
        {
            if (null == loggerFactory)
            {
                throw new ArgumentNullException(ErrorMessageILoggerFactoryWrapperIsNull, (Exception)null);
            }

            this.logger = loggerFactory.CreateLogger<MyChildManager>();
            this.myChildDomainData = myChildDomainData ?? throw new ArgumentNullException(ErrorMessageIMyChildDomainDataIsNull, (Exception)null);
        }

        public async Task<MyChildEntity> AddAsync(MyChildEntity entity, CancellationToken token)
        {
            this.ValidateMyChildEntity(entity);
            MyChildEntity returnItem = await this.myChildDomainData.AddAsync(entity, token);
            return returnItem;
        }

        public async Task<MyChildEntity> AddAsync(MyChildEntity entity)
        {
            MyChildEntity returnItem = await this.AddAsync(entity, CancellationToken.None);
            return returnItem;
        }

        public async Task<IEnumerable<MyChildEntity>> GetAllAsync()
        {
            IEnumerable<MyChildEntity> returnItems = await this.GetAllAsync(CancellationToken.None);
            return returnItems;
        }

        public async Task<IEnumerable<MyChildEntity>> GetAllAsync(CancellationToken token)
        {
            IEnumerable<MyChildEntity> returnItems = await this.myChildDomainData.GetAllAsync(token);
            returnItems = returnItems.OrderBy(x => x.MyChildKey);
            return returnItems;
        }

        public async Task<MyChildEntity> GetSingleAsync(Guid keyValue)
        {
            MyChildEntity returnItem = await this.GetSingleAsync(keyValue, CancellationToken.None);
            return returnItem;
        }

        public async Task<MyChildEntity> GetSingleAsync(Guid keyValue, CancellationToken token)
        {
            MyChildEntity returnItem = await this.myChildDomainData.GetSingleAsync(keyValue, token);
            return returnItem;
        }

        public async Task<MyChildEntity> UpdateAsync(MyChildEntity entity, CancellationToken token)
        {
            this.ValidateMyChildEntity(entity);
            MyChildEntity returnItem = await this.myChildDomainData.UpdateAsync(entity, token);
            return returnItem;
        }

        public async Task<MyChildEntity> UpdateAsync(MyChildEntity entity)
        {
            MyChildEntity returnItem = await this.UpdateAsync(entity, CancellationToken.None);
            return returnItem;
        }

        public async Task<int> DeleteAsync(Guid keyValue, CancellationToken token)
        {
            int returnValue = await this.myChildDomainData.DeleteAsync(keyValue, token);
            return returnValue;
        }

        public async Task<int> DeleteAsync(Guid keyValue)
        {
            int returnValue = await this.DeleteAsync(keyValue, CancellationToken.None);
            return returnValue;
        }

        private void ValidateMyChildEntity(MyChildEntity entity)
        {
            ////new MyChildValidator().ValidateSingle(entity);
        }
    }
}
