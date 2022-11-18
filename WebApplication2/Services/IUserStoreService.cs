using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IUserStoreService
    {
        public Task<bool> CreateLeaderBoard(int month);
        public Task<List<LeaderBoardRecord>> GetLeaderBoard(int month, string? userId);
        public Task<List<LeaderBoardRecord>> GetUserPrizeBoard(string userId);
    }
}