using System;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.Interfaces
{
    public interface IMyChildDomainData : IDataRepository<Guid, MyChildEntity>
    {
    }
}
