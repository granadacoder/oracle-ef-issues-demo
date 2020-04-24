using System;
using System.Collections.Generic;

using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Showers
{
    public static class BoardGameShower
    {
        public static void ShowBoardGameEntities(string label, ICollection<BoardGameEntity> items)
        {
            if (null != items)
            {
                foreach (BoardGameEntity item in items)
                {
                    ShowBoardGameEntity(label, item);
                }
            }
        }

        public static void ShowBoardGameEntity(string label, BoardGameEntity item)
        {
            if (null != item)
            {
                Console.WriteLine("({0}) BoardGameKey='{1}', Title='{2}', MinPlayers='{3}', MaxPlayers='{4}', PublishingCompany='{5}'", label, item.BoardGameKey, item.Title, item.MinPlayers, item.MaxPlayers, item.PublishingCompany);
            }
        }
    }
}
