using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection.Metadata.Ecma335;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class UserStoreService : IUserStoreService
    {
        private readonly IMongoCollection<NftUsers> _nftUsersCollections;
        private readonly IMongoCollection<LeaderBoardRecord> _leaderBoardRecordCollections;
        private const double CONSOLATION_PRIZE = 13.8; // 12.500/900


        public UserStoreService(IOptions<UserStoreDatabaseSettings> userStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
            userStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                userStoreDatabaseSettings.Value.DatabaseName);

            _nftUsersCollections = mongoDatabase.GetCollection<NftUsers>(
                userStoreDatabaseSettings.Value.NftUsersCollectionName);

            _leaderBoardRecordCollections = mongoDatabase.GetCollection<LeaderBoardRecord>(
                userStoreDatabaseSettings.Value.LeaderBoardCollectionName);

        }

        public Task<bool> CreateLeaderBoard(int month)
        {
            if (month <= 0 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month range must be between 1-12");
            }
            var MonthCheck = _leaderBoardRecordCollections.AsQueryable().Any(x => x.Month == month);

            if (MonthCheck)
            {
                throw new ArgumentOutOfRangeException(nameof(MonthCheck), "Only one leader board can create at same month.");
            }

            return CreateLeaderBoardImpl(month);
        }

        private Task<bool> CreateLeaderBoardImpl(int month)
        {
            try
            {
                var UserListOrderedByTotalPoint = GetUserListOrderedByTotalPoint();

                AssignRanks(month, UserListOrderedByTotalPoint);

                _leaderBoardRecordCollections.InsertMany(UserListOrderedByTotalPoint.Take(1000));

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void AssignRanks(int month, List<LeaderBoardRecord> UserListOrderedByTotalPoint)
        {
            int index = 1;

            foreach (var leaderBoardRecord in UserListOrderedByTotalPoint)
            {
                leaderBoardRecord.Month = month;
                leaderBoardRecord.Rank = index++;

                if (UserListOrderedByTotalPoint.IndexOf(leaderBoardRecord) == 0)
                {
                    leaderBoardRecord.Prize = "First Prize";
                    continue;
                }
                else if (UserListOrderedByTotalPoint.IndexOf(leaderBoardRecord) == 1)
                {
                    leaderBoardRecord.Prize = "Second Prize";
                    continue;
                }
                else if (UserListOrderedByTotalPoint.IndexOf(leaderBoardRecord) == 2)
                {
                    leaderBoardRecord.Prize = "Third Prize";
                    continue;
                }
                else if (UserListOrderedByTotalPoint.IndexOf(leaderBoardRecord) <= 100)
                {
                    leaderBoardRecord.Prize = "25$";
                    continue;
                }
                leaderBoardRecord.Prize = CONSOLATION_PRIZE.ToString();
            }
        }

        private List<LeaderBoardRecord> GetUserListOrderedByTotalPoint()
        {
            return _nftUsersCollections.AsQueryable().Where(d => d.IsApproved)
                             .GroupBy(d => new { d.UserId })
                             .Select(g => new LeaderBoardRecord()
                             {
                                 UserId = g.Key.UserId,
                                 TotalPoint = g.Sum(c => c.point)
                             }).OrderByDescending(g => g.TotalPoint).ToList();
        }

        public Task<List<LeaderBoardRecord>> GetLeaderBoard(int month, string? userId)
        {
            try
            {
                var leaderBoards = new List<LeaderBoardRecord>();

                if (!(month > 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(month), "Month field  must be fill.");
                }


                if (!(month <= 0) && !string.IsNullOrEmpty(userId))
                {
                    leaderBoards = _leaderBoardRecordCollections.AsQueryable()
                        .Where(x => x.UserId == userId && x.Month == month).ToList();
                }
                else if (!(month <= 0) && string.IsNullOrEmpty(userId))
                {
                    leaderBoards = _leaderBoardRecordCollections.AsQueryable().Where(x => x.Month == month).ToList();
                }

                if (!(leaderBoards?.Count <= 0))
                {
                    return Task.FromResult(leaderBoards);
                }

                return Task.FromResult(new List<LeaderBoardRecord>());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public Task<List<LeaderBoardRecord>> GetUserPrizeBoard(string userId)
        {
            try
            {
                var userPrizeBoard = new List<LeaderBoardRecord>();

                userPrizeBoard = _leaderBoardRecordCollections.AsQueryable().Where(x => x.UserId == userId).ToList();

                if (!(userPrizeBoard?.Count <= 0))
                {
                    return Task.FromResult(userPrizeBoard);
                }

                return Task.FromResult(new List<LeaderBoardRecord>());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
