using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.Interfaces;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers
{
    public class BoardGameManager : IBoardGameManager
    {
        public const string ErrorMessageILoggerFactoryWrapperIsNull = "ILoggerFactoryWrapper is null";
        public const string ErrorMessageIMyChildDomainDataIsNull = "IMyChildDomainData is null";
        protected readonly IBoardGameDataLayer BoardGameDataLayer;
        private readonly ILogger<BoardGameManager> logger;

        public BoardGameManager(ILoggerFactory loggerFactory, IBoardGameDataLayer boardGameDataLayer)
        {
            if (null == loggerFactory)
            {
                throw new ArgumentNullException(ErrorMessageILoggerFactoryWrapperIsNull, (Exception)null);
            }

            this.logger = loggerFactory.CreateLogger<BoardGameManager>();
            this.BoardGameDataLayer = boardGameDataLayer ?? throw new ArgumentNullException("IBoardGameDataLayer is null");
        }

        public async Task<int> AddAsync(BoardGameEntity bge, CancellationToken token)
        {
            int returnValue = await this.BoardGameDataLayer.AddAsync(bge, token);
            return returnValue;
        }

        public async Task<int> AddAsync(BoardGameEntity bge)
        {
            int returnValue = await this.AddAsync(bge, CancellationToken.None);
            return returnValue;
        }

        public async Task<ICollection<BoardGameEntity>> BoardGamesGetAllAsync()
        {
            ICollection<BoardGameEntity> returnItems = await this.BoardGameDataLayer.BoardGamesGetAllAsync();
            return returnItems;
        }

        public async Task<BoardGameEntity> BoardGamesGetSingleAsync(int id)
        {
            BoardGameEntity returnItem = await this.BoardGameDataLayer.BoardGamesGetSingleAsync(id);
            return returnItem;
        }

        public async Task<int> UpdateAsync(BoardGameEntity bge, CancellationToken token)
        {
            int returnValue = await this.BoardGameDataLayer.UpdateAsync(bge, token);
            return returnValue;
        }

        public async Task<int> UpdateAsync(BoardGameEntity bge)
        {
            int returnValue = await this.UpdateAsync(bge, CancellationToken.None);
            return returnValue;
        }
    }
}
