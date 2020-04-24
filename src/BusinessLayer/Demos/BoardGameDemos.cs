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

    public class BoardGameDemos : IBoardGameDemos
    {
        public const string ErrorMessageILoggerFactoryWrapperIsNull = "ILoggerFactoryWrapper is null";
        public const string ErrorMessageIBoardGameManagerIsNull = "IBoardGameManager is null";
        private readonly ILogger<BoardGameDemos> logger;
        private readonly IBoardGameManager boardGameManager;

        public BoardGameDemos(ILoggerFactory loggerFactory, IBoardGameManager boardGameManager)
        {
            if (null == loggerFactory)
            {
                throw new ArgumentNullException(ErrorMessageILoggerFactoryWrapperIsNull, (Exception)null);
            }

            this.logger = loggerFactory.CreateLogger<BoardGameDemos>();
            this.boardGameManager = boardGameManager ?? throw new ArgumentNullException(ErrorMessageIBoardGameManagerIsNull, (Exception)null);
        }

        public async Task PerformBasicCrudDemo()
        {
            ICollection<BoardGameEntity> boardGames = await this.boardGameManager.BoardGamesGetAllAsync();
            BoardGameShower.ShowBoardGameEntities("BoardGamesGetAllAsync", boardGames);
            Console.WriteLine(string.Empty);

            int currentMaxKey = boardGames.Any() ? boardGames.Max(x => x.BoardGameKey) : 1;

            if (null != boardGames)
            {
                BoardGameEntity firstBoardGameEntity = boardGames.FirstOrDefault();
                BoardGameEntity boardGame = await this.boardGameManager.BoardGamesGetSingleAsync(firstBoardGameEntity.BoardGameKey);
                BoardGameShower.ShowBoardGameEntity("BoardGamesGetSingleAsync", boardGame);
                Console.WriteLine(string.Empty);

                boardGame = await this.boardGameManager.BoardGamesGetSingleAsync(-999);
                BoardGameShower.ShowBoardGameEntity("BoardGamesGetSingleAsync", boardGame);
                Console.WriteLine(string.Empty);

                if (null != firstBoardGameEntity)
                {
                    firstBoardGameEntity.MinPlayers = firstBoardGameEntity.MinPlayers + 100;
                    firstBoardGameEntity.MaxPlayers = firstBoardGameEntity.MaxPlayers + 100;
                    firstBoardGameEntity.Title = firstBoardGameEntity.Title + "***Edited";
                    firstBoardGameEntity.PublishingCompany = firstBoardGameEntity.PublishingCompany + "***Edited";
                    int updateReturnValue = await this.boardGameManager.UpdateAsync(firstBoardGameEntity);
                    Console.WriteLine(string.Format("updateReturnValue='{0}'", updateReturnValue));

                    boardGame = await this.boardGameManager.BoardGamesGetSingleAsync(firstBoardGameEntity.BoardGameKey);
                    BoardGameShower.ShowBoardGameEntity("BoardGamesGetSingleAsync.After.UpdateAsync", boardGame);
                    Console.WriteLine(string.Empty);
                }

                BoardGameEntity newBoardGameEntity = new BoardGameEntity();
                newBoardGameEntity.BoardGameKey = currentMaxKey + 1;
                newBoardGameEntity.MinPlayers = 333;
                newBoardGameEntity.MaxPlayers = 444;
                newBoardGameEntity.Title = "MyNewTitle" + Convert.ToString(newBoardGameEntity.BoardGameKey);
                newBoardGameEntity.PublishingCompany = "MyNewPublishingCompanyOne";
                int addReturnValue = await this.boardGameManager.AddAsync(newBoardGameEntity);
                Console.WriteLine(string.Format("addReturnValue='{0}'", addReturnValue));

                boardGames = await this.boardGameManager.BoardGamesGetAllAsync();
                BoardGameShower.ShowBoardGameEntities("BoardGamesGetAllAsync.After.SingleAddAsync", boardGames);
                Console.WriteLine(string.Empty);
            }
        }
    }
}
