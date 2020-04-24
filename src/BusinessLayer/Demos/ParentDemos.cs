using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Showers;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.Interfaces.Constants.StringLengths;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos
{
    public class ParentDemos : IParentDemos
    {
        public const string ErrorMessageILoggerFactoryWrapperIsNull = "ILoggerFactoryWrapper is null";
        public const string ErrorMessageIMyParentManagerIsNull = "IMyParentManager is null";
        private readonly ILogger<ParentDemos> logger;
        private readonly IMyParentManager parentManager;

        public ParentDemos(ILoggerFactory loggerFactory, IMyParentManager parentManager)
        {
            if (null == loggerFactory)
            {
                throw new ArgumentNullException(ErrorMessageILoggerFactoryWrapperIsNull, (Exception)null);
            }

            this.logger = loggerFactory.CreateLogger<ParentDemos>();
            this.parentManager = parentManager ?? throw new ArgumentNullException(ErrorMessageIMyParentManagerIsNull, (Exception)null);
        }

        public async Task PerformDemoIQueryableWithReusedPocoObject()
        {
            IEnumerable<MyParentEntity> magicEntities = await this.parentManager.PerformDemoIQueryableWithReusedPocoObject();
            ParentShower.ShowMyParentEntities("PerformDemoIQueryableWithReusedPocoObject", magicEntities.ToList());
            return;
        }

        public async Task PerformDemoIQueryableWithAnonymousClass()
        {
            IEnumerable<MyParentEntity> magicEntities = await this.parentManager.PerformDemoIQueryableWithAnonymousClass();
            ParentShower.ShowMyParentEntities("PerformDemoIQueryableWithAnonymousClass", magicEntities.ToList());
            return;
        }

        public async Task PerformDemoIQueryableWithPrivateClassHolderObject()
        {
            IEnumerable<MyParentEntity> magicEntities = await this.parentManager.PerformDemoIQueryableWithPrivateClassHolderObject();
            ParentShower.ShowMyParentEntities("PerformDemoIQueryableWithPrivateClassHolderObject", magicEntities.ToList());
            return;
        }

        public async Task PerformBasicCrudDemo()
        {
            IEnumerable<MyParentEntity> entities = await this.parentManager.GetAllAsync();
            ParentShower.ShowMyParentEntities("MyParentsGetAllAsync", entities.ToList());
            Console.WriteLine(string.Empty);

            Guid currentMaxKey = entities.Any() ? entities.Max(x => x.MyParentKey) : Guid.NewGuid();

            if (null != entities)
            {
                MyParentEntity firstMyParentEntity = entities.FirstOrDefault();
                MyParentEntity entity = await this.parentManager.GetSingleAsync(firstMyParentEntity.MyParentKey);
                ParentShower.ShowMyParentEntity("GetSingleAsync", entity);
                Console.WriteLine(string.Empty);

                entity = await this.parentManager.GetSingleAsync(Guid.Empty);
                ParentShower.ShowMyParentEntity("MyParentsGetSingleAsync", entity);
                Console.WriteLine(string.Empty);

                if (null != firstMyParentEntity)
                {
                    firstMyParentEntity.MyParentName = firstMyParentEntity.MyParentName.Length > MyParentValidationStringLengthConstants.MyParentNameMaxLength - 10 ? "MyParentName" + Convert.ToString(firstMyParentEntity.MyParentKey) + "Reset" : firstMyParentEntity.MyParentName + "***Edited";
                    MyParentEntity updateReturnValue = await this.parentManager.UpdateAsync(firstMyParentEntity);
                    Console.WriteLine(string.Format("updateReturnValue.MyParentKey='{0}'", updateReturnValue.MyParentKey));

                    entity = await this.parentManager.GetSingleAsync(firstMyParentEntity.MyParentKey);
                    ParentShower.ShowMyParentEntity("MyParentsGetSingleAsync.After.UpdateAsync", entity);
                    Console.WriteLine(string.Empty);
                }

                MyParentEntity newMyParentEntity = new MyParentEntity();
                newMyParentEntity.MyParentKey = Guid.NewGuid();
                newMyParentEntity.MyParentName = "MyParentName" + Convert.ToString(newMyParentEntity.MyParentKey);
                MyParentEntity addReturnValue = await this.parentManager.AddAsync(newMyParentEntity);
                Console.WriteLine(string.Format("addReturnValue.MyParentKey='{0}'", addReturnValue.MyParentKey));

                entities = await this.parentManager.GetAllAsync();
                ParentShower.ShowMyParentEntities("MyParentsGetAllAsync.After.SingleAddAsync", entities.ToList());
                Console.WriteLine(string.Empty);
            }
        }
    }
}
