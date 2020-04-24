using System;
using System.Collections.Generic;

using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Showers
{
    public static class ChildShower
    {
        public static void ShowMyChildEntities(string label, ICollection<MyChildEntity> items)
        {
            if (null != items)
            {
                foreach (MyChildEntity item in items)
                {
                    ShowMyChildEntity(label, item);
                }
            }
        }

        public static void ShowMyChildEntity(string label, MyChildEntity item)
        {
            if (null != item)
            {
                Console.WriteLine("({0}) MyChildKey='{1}', MyChildName='{2}', MyParentUuidFk='{3}', MyChildMagicStatus='{4}', FavoriteColor='{5}', UpdateDateStamp='{6}'", label, item.MyChildKey, item.MyChildName, item.MyParentUuidFk, item.MyChildMagicStatus, item.FavoriteColor, item.UpdateDateStamp);
            }
        }
    }
}
