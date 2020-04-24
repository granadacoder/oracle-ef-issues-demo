using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities;

namespace MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces
{
    public interface IBoardGameManager
    {
        Task<ICollection<BoardGameEntity>> BoardGamesGetAllAsync();

        Task<BoardGameEntity> BoardGamesGetSingleAsync(int id);

        Task<int> AddAsync(BoardGameEntity bge, CancellationToken token);

        Task<int> UpdateAsync(BoardGameEntity bge, CancellationToken token);

        Task<int> AddAsync(BoardGameEntity bge);

        Task<int> UpdateAsync(BoardGameEntity bge);
    }
}
