using System.Threading.Tasks;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Interfaces
{
    public interface IParentDemos
    {
        Task PerformBasicCrudDemo();

        Task PerformDemoIQueryableWithReusedPocoObject();

        Task PerformDemoIQueryableWithAnonymousClass();

        Task PerformDemoIQueryableWithPrivateClassHolderObject();
    }
}
