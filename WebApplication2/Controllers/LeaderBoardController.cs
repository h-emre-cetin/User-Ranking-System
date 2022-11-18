using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        
        private readonly IUserStoreService _userStoreService;

        public LeaderBoardController(IUserStoreService userStoreService)
        {
            _userStoreService = userStoreService ?? throw new ArgumentNullException(nameof(userStoreService));
        }


        [HttpPost]
        public Task<bool> Create([FromBody] CreateLeaderBoardRequest request)
        {


            return Task.Run(() =>
            _userStoreService.CreateLeaderBoard(request.Month) 
            );

        }


        [HttpGet]
        public Task<List<LeaderBoardDefinition>> GetLeaderBoard(int? month, string? userId)
        {
            return Task.Run(async () =>
            {
                var leaderBoardDefinitions = new List<LeaderBoardDefinition>();
                if (month != null)
                {
                    var leaderBoards = await _userStoreService.GetLeaderBoard(month ?? 0, userId);
                    
                    foreach (var leaderBoard in leaderBoards)
                    {
                        var leaderBoardDefinition = new LeaderBoardDefinition
                        {
                            Month = leaderBoard.Month,
                            UserId = leaderBoard.UserId,
                            Rank = leaderBoard.Rank,
                            TotalPoint = leaderBoard.TotalPoint
                        };
                        leaderBoardDefinitions.Add(leaderBoardDefinition);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(month), "Month field must be fill.");
                }

                return leaderBoardDefinitions;
            });
        }

        [HttpGet]
        public Task<List<PrizeBoardDefinition>> GetUserPrizeBoardById(string? userId)
        {
            return Task.Run(async () =>
            {
                var prizeBoards = new List<LeaderBoardRecord>();

                if (!string.IsNullOrEmpty(userId))
                {
                    prizeBoards = await _userStoreService.GetUserPrizeBoard(userId);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(userId), "UserId field must be fill.");
                }

                var prizeBoardDefinitions = new List<PrizeBoardDefinition>();

                foreach (var prizeBoardRecord in prizeBoards)
                {
                    var prizeBoardDefinition = new PrizeBoardDefinition
                    {
                        UserId = prizeBoardRecord.UserId,
                        Prize = prizeBoardRecord.Prize
                    };
                    prizeBoardDefinitions.Add(prizeBoardDefinition);
                }

                return prizeBoardDefinitions;
            });
        }


    }
}
