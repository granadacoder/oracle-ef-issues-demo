using System;
using System.Collections.Generic;

using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Showers
{
    public static class ParentShower
    {
        public static void ShowMyParentEntities(string label, ICollection<MyParentEntity> items)
        {
            if (null != items)
            {
                foreach (MyParentEntity item in items)
                {
                    ShowMyParentEntity(label, item);
                }
            }
        }

        public static void ShowMyParentEntity(string label, MyParentEntity item)
        {
            if (null != item)
            {
                Console.WriteLine("({0}) MyParentKey='{1}', MyParentName='{2}'", label, item.MyParentKey, item.MyParentName);
            }
        }
    }
}
