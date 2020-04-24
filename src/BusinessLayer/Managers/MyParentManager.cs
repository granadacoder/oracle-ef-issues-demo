using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Constants;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.Interfaces;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers
{
    public class MyParentManager : IMyParentManager
    {
        public const string ErrorMessageILoggerFactoryWrapperIsNull = "ILoggerFactoryWrapper is null";
        public const string ErrorMessageIMyParentDomainDataIsNull = "IMyParentDomainData is null";
        private readonly ILogger<MyParentManager> logger;
        private readonly IMyParentDomainData myParentDomainData;

        public MyParentManager(ILoggerFactory loggerFactory, IMyParentDomainData myParentDomainData)
        {
            if (null == loggerFactory)
            {
                throw new ArgumentNullException(ErrorMessageILoggerFactoryWrapperIsNull, (Exception)null);
            }

            this.logger = loggerFactory.CreateLogger<MyParentManager>();
            this.myParentDomainData = myParentDomainData ?? throw new ArgumentNullException(ErrorMessageIMyParentDomainDataIsNull, (Exception)null);
        }

        public async Task<MyParentEntity> AddAsync(MyParentEntity entity, CancellationToken token)
        {
            this.ValidateMyParentEntity(entity);
            MyParentEntity returnItem = await this.myParentDomainData.AddAsync(entity, token);

            return returnItem;
        }

        public async Task<MyParentEntity> AddAsync(MyParentEntity entity)
        {
            MyParentEntity returnItem = await this.AddAsync(entity, CancellationToken.None);

            return returnItem;
        }

        public async Task<IEnumerable<MyParentEntity>> GetAllAsync()
        {
            IEnumerable<MyParentEntity> returnItems = await this.GetAllAsync(CancellationToken.None);

            return returnItems;
        }

        public async Task<IEnumerable<MyParentEntity>> GetAllAsync(CancellationToken token)
        {
            IEnumerable<MyParentEntity> returnItems = await this.myParentDomainData.GetAllAsync(token);
            returnItems = returnItems.OrderBy(x => x.MyParentKey);

            return returnItems;
        }

        public async Task<MyParentEntity> GetSingleAsync(Guid keyValue)
        {
            MyParentEntity returnItem = await this.GetSingleAsync(keyValue, CancellationToken.None);

            return returnItem;
        }

        public async Task<MyParentEntity> GetSingleAsync(Guid keyValue, CancellationToken token)
        {
            MyParentEntity returnItem = await this.myParentDomainData.GetSingleAsync(keyValue, token);

            return returnItem;
        }

        public async Task<MyParentEntity> UpdateAsync(MyParentEntity entity, CancellationToken token)
        {
            this.ValidateMyParentEntity(entity);
            MyParentEntity returnItem = await this.myParentDomainData.UpdateAsync(entity, token);

            return returnItem;
        }

        public async Task<MyParentEntity> UpdateAsync(MyParentEntity entity)
        {
            MyParentEntity returnItem = await this.UpdateAsync(entity, CancellationToken.None);

            return returnItem;
        }

        public async Task<int> DeleteAsync(Guid keyValue, CancellationToken token)
        {
            int returnValue = await this.myParentDomainData.DeleteAsync(keyValue, token);

            return returnValue;
        }

        public async Task<int> DeleteAsync(Guid keyValue)
        {
            int returnValue = await this.DeleteAsync(keyValue, CancellationToken.None);

            return returnValue;
        }

        public async Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithReusedPocoObject()
        {
            IEnumerable<MyParentEntity> returnItems = await this.PerformDemoIQueryableWithReusedPocoObject(CancellationToken.None);
            return returnItems;
        }

        public async Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithReusedPocoObject(CancellationToken token)
        {
            IEnumerable<MyParentEntity> returnItems = await this.myParentDomainData.PerformDemoIQueryableWithReusedPocoObject(this.GetDefaultCutOffTimeSpan(), this.GetDefaultMagicStatusValues(), token);
            returnItems = returnItems.OrderBy(x => x.MyParentKey);
            return returnItems;
        }

        public async Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithAnonymousClass()
        {
            IEnumerable<MyParentEntity> returnItems = await this.PerformDemoIQueryableWithAnonymousClass(CancellationToken.None);
            return returnItems;
        }

        public async Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithAnonymousClass(CancellationToken token)
        {
            IEnumerable<MyParentEntity> returnItems = await this.myParentDomainData.PerformDemoIQueryableWithAnonymousClass(this.GetDefaultCutOffTimeSpan(), this.GetDefaultMagicStatusValues(), token);
            returnItems = returnItems.OrderBy(x => x.MyParentKey);
            return returnItems;
        }

        public async Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithPrivateClassHolderObject()
        {
            IEnumerable<MyParentEntity> returnItems = await this.PerformDemoIQueryableWithPrivateClassHolderObject(CancellationToken.None);
            return returnItems;
        }

        public async Task<IEnumerable<MyParentEntity>> PerformDemoIQueryableWithPrivateClassHolderObject(CancellationToken token)
        {
            IEnumerable<MyParentEntity> returnItems = await this.myParentDomainData.PerformDemoIQueryableWithPrivateClassHolderObject(this.GetDefaultCutOffTimeSpan(), this.GetDefaultMagicStatusValues(), token);
            returnItems = returnItems.OrderBy(x => x.MyParentKey);
            return returnItems;
        }

        private void ValidateMyParentEntity(MyParentEntity entity)
        {
            ////new MyParentValidator().ValidateSingle(entity);
        }

        private ICollection<int> GetDefaultMagicStatusValues()
        {
            List<int> returnValues = new List<int> { MagicStatusValues.NinetyEight, MagicStatusValues.NinetyNine };
            return returnValues;
        }

        private TimeSpan GetDefaultCutOffTimeSpan()
        {
            TimeSpan returnValue = TimeSpan.FromDays(3).Negate();
            return returnValue;
        }
    }
}
