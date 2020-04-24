namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos
{
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

    public class ChildDemos : IChildDemos
    {
        public const string ErrorMessageILoggerFactoryWrapperIsNull = "ILoggerFactoryWrapper is null";
        public const string ErrorMessageIMyChildManagerIsNull = "IMyChildManager is null";
        public const string ErrorMessageIMyParentManagerIMyChildManagerIsNull = "IMyParentManager is null";

        private readonly ILogger<ChildDemos> logger;
        private readonly IMyChildManager childManager;
        private readonly IMyParentManager parentManager;

        public ChildDemos(ILoggerFactory loggerFactory, IMyChildManager childManager, IMyParentManager parentManager)
        {
            if (null == loggerFactory)
            {
                throw new ArgumentNullException(ErrorMessageILoggerFactoryWrapperIsNull, (Exception)null);
            }

            this.logger = loggerFactory.CreateLogger<ChildDemos>();
            this.childManager = childManager ?? throw new ArgumentNullException(ErrorMessageIMyChildManagerIsNull, (Exception)null);
            this.parentManager = parentManager ?? throw new ArgumentNullException(ErrorMessageIMyParentManagerIMyChildManagerIsNull, (Exception)null);
        }

        public async Task PerformBasicCrudDemo()
        {
            IEnumerable<MyChildEntity> entities = await this.childManager.GetAllAsync();
            ChildShower.ShowMyChildEntities("MyChildsGetAllAsync", entities.ToList());
            Console.WriteLine(string.Empty);

            IEnumerable<MyParentEntity> parentEntities = await this.parentManager.GetAllAsync();
            Console.WriteLine(string.Empty);
            Guid oneOfTheParentUuids = parentEntities.Any() ? parentEntities.OrderByDescending(x => x.UpdateDateStamp).First().MyParentKey : Guid.NewGuid();

            if (null != entities)
            {
                MyChildEntity firstMyChildEntity = entities.FirstOrDefault();
                MyChildEntity entity = await this.childManager.GetSingleAsync(firstMyChildEntity.MyChildKey);
                ChildShower.ShowMyChildEntity("GetSingleAsync", entity);
                Console.WriteLine(string.Empty);

                entity = await this.childManager.GetSingleAsync(Guid.Empty);
                ChildShower.ShowMyChildEntity("MyChildsGetSingleAsync", entity);
                Console.WriteLine(string.Empty);

                if (null != firstMyChildEntity)
                {
                    firstMyChildEntity.MyChildName = firstMyChildEntity.MyChildName.Length > MyParentValidationStringLengthConstants.MyParentNameMaxLength - 10 ? "MyChildName" + Convert.ToString(firstMyChildEntity.MyChildKey) + "Reset" : firstMyChildEntity.MyChildName + "***Edited";
                    MyChildEntity updateReturnValue = await this.childManager.UpdateAsync(firstMyChildEntity);
                    Console.WriteLine(string.Format("updateReturnValue.MyChildKey='{0}'", updateReturnValue.MyChildKey));

                    entity = await this.childManager.GetSingleAsync(firstMyChildEntity.MyChildKey);
                    ChildShower.ShowMyChildEntity("MyChildsGetSingleAsync.After.UpdateAsync", entity);
                    Console.WriteLine(string.Empty);
                }

                MyChildEntity newMyChildEntity = new MyChildEntity();
                newMyChildEntity.MyChildKey = Guid.NewGuid();
                newMyChildEntity.MyChildName = "MyChildName" + Convert.ToString(newMyChildEntity.MyChildKey);
                newMyChildEntity.MyParentUuidFk = oneOfTheParentUuids;
                MyChildEntity addReturnValue = await this.childManager.AddAsync(newMyChildEntity);
                Console.WriteLine(string.Format("addReturnValue.MyChildKey='{0}'", addReturnValue.MyChildKey));

                entities = await this.childManager.GetAllAsync();
                ChildShower.ShowMyChildEntities("MyChildsGetAllAsync.After.SingleAddAsync", entities.ToList());
                Console.WriteLine(string.Empty);
            }
        }
    }
}
