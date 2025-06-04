using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class DataInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public DataInitializer(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public void SeedData()
        {
            _dbContext.Database.Migrate();
            SeedRoles();
            SeedUsers();
            SeedPlayers();
            SeedMatches();
           
        }

        private void SeedUsers()
        {
            AddUserIfNotExists("admin@angby.se", "*Admin100", new string[] { "Admin" });
        }

        private void SeedRoles()
        {
            AddRoleIfNotExisting("Admin");
        }

        private void AddRoleIfNotExisting(string roleName)
        {
            var role = _dbContext.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                _dbContext.Roles.Add(new IdentityRole { Name = roleName, NormalizedName = roleName });
                _dbContext.SaveChanges();
            }
        }

        private void AddUserIfNotExists(string userName, string password, string[] roles)
        {
            if (_userManager.FindByEmailAsync(userName).Result != null) return;

            var user = new IdentityUser
            {
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };
            _userManager.CreateAsync(user, password).Wait();
            _userManager.AddToRolesAsync(user, roles).Wait();
        }

        private void SeedPlayers()
        {
            if (!_dbContext.Players.Any())
            {
                var players = new List<Player>
           {
               new Player
               {
                   Name = "Richard Chalk",
                   Gender = Gender.Male,
                   BirthDate = new DateOnly(1980, 1, 1),
               },
               new Player
               {
                   Name = "Truls Möregårdh",
                   Gender = Gender.Male,
                   BirthDate = new DateOnly(2002, 2, 16),
               },
               new Player
               {
                   Name = "Jan-Ove Waldner",
                   Gender = Gender.Male,
                   BirthDate = new DateOnly(1965, 10, 3)
               },
               new Player
               {
                   Name = "Ma Long",
                   Gender = Gender.Male,
                   BirthDate = new DateOnly(1988, 10, 20)
               },
               new Player
               {
                   Name = "Timo Boll",
                   Gender = Gender.Male,
                   BirthDate = new DateOnly(1981, 3, 8)
               },
               new Player
               {
                   Name = "Deng Yaping",
                   Gender = Gender.Female,
                   BirthDate = new DateOnly(1973, 2, 6)
               },
               new Player
               {
                   Name = "Mima Ito",
                   Gender = Gender.Female,
                   BirthDate = new DateOnly(2000, 10, 21)
               },
           };

                _dbContext.Players.AddRange(players);
                _dbContext.SaveChanges();
            }
        }

        private void SeedMatches()
        {
            if (!_dbContext.Matches.Any())
            {
                var players = _dbContext.Players.ToList();

                var chalk = players.First(p => p.Name == "Richard Chalk");
                var truls = players.First(p => p.Name == "Truls Möregårdh");
                var waldner = players.First(p => p.Name == "Jan-Ove Waldner");
                var maLong = players.First(p => p.Name == "Ma Long");
                var boll = players.First(p => p.Name == "Timo Boll");
                var deng = players.First(p => p.Name == "Deng Yaping");
                var mima = players.First(p => p.Name == "Mima Ito");

                var matches = new List<Match>
        {
            new Match
            {
                Player1Id = chalk.Id,
                Player2Id = truls.Id,
                BestOfSets = BestOf.BestOf5,
                MatchDate = new DateTime(2024, 5, 15, 18, 30, 0),
                Player1Score = 2,
                Player2Score = 3,
                IsFinished = true,
                IsActive = true,
                Sets = new List<Set>
                {
                    new Set { SetNumber = 1, Player1Points = 11, Player2Points = 7, SetStartTime = new DateTime(2024, 5, 15, 18, 31, 0), SetDuration = TimeSpan.FromMinutes(5), IsFinished = true },
                    new Set { SetNumber = 2, Player1Points = 9, Player2Points = 11, SetStartTime = new DateTime(2024, 5, 15, 18, 36, 0), SetDuration = TimeSpan.FromMinutes(7), IsFinished = true },
                    new Set { SetNumber = 3, Player1Points = 7, Player2Points = 11, SetStartTime = new DateTime(2024, 5, 15, 18, 43, 0), SetDuration = TimeSpan.FromMinutes(6), IsFinished = true },
                    new Set { SetNumber = 4, Player1Points = 11, Player2Points = 8, SetStartTime = new DateTime(2024, 5, 15, 18, 49, 0), SetDuration = TimeSpan.FromMinutes(8), IsFinished = true },
                    new Set { SetNumber = 5, Player1Points = 5, Player2Points = 11, SetStartTime = new DateTime(2024, 5, 15, 18, 57, 0), SetDuration = TimeSpan.FromMinutes(4), IsFinished = true },
                }
            },
            new Match
            {
                Player1Id = waldner.Id,
                Player2Id = boll.Id,
                BestOfSets = BestOf.BestOf5,
                MatchDate = new DateTime(2024, 5, 10, 17, 15, 0),
                Player1Score = 3,
                Player2Score = 0,
                IsFinished = true,
                IsActive = true,
                Sets = new List<Set>
                {
                    new Set { SetNumber = 1, Player1Points = 11, Player2Points = 4, SetStartTime = new DateTime(2024, 5, 10, 17, 16, 0), SetDuration = TimeSpan.FromMinutes(5), IsFinished = true },
                    new Set { SetNumber = 2, Player1Points = 11, Player2Points = 7, SetStartTime = new DateTime(2024, 5, 10, 17, 21, 0), SetDuration = TimeSpan.FromMinutes(6), IsFinished = true },
                    new Set { SetNumber = 3, Player1Points = 11, Player2Points = 9, SetStartTime = new DateTime(2024, 5, 10, 17, 27, 0), SetDuration = TimeSpan.FromMinutes(7), IsFinished = true },
                }
            },
            new Match
            {
                Player1Id = maLong.Id,
                Player2Id = deng.Id,
                BestOfSets = BestOf.BestOf5,
                MatchDate = new DateTime(2024, 5, 13, 19, 0, 0),
                Player1Score = 3,
                Player2Score = 1,
                IsFinished = true,
                IsActive = true,
                Sets = new List<Set>
                {
                    new Set { SetNumber = 1, Player1Points = 9, Player2Points = 11, SetStartTime = new DateTime(2024, 5, 13, 19, 1, 0), SetDuration = TimeSpan.FromMinutes(6), IsFinished = true },
                    new Set { SetNumber = 2, Player1Points = 11, Player2Points = 8, SetStartTime = new DateTime(2024, 5, 13, 19, 7, 0), SetDuration = TimeSpan.FromMinutes(7), IsFinished = true },
                    new Set { SetNumber = 3, Player1Points = 11, Player2Points = 3, SetStartTime = new DateTime(2024, 5, 13, 19, 14, 0), SetDuration = TimeSpan.FromMinutes(5), IsFinished = true },
                    new Set { SetNumber = 4, Player1Points = 11, Player2Points = 9, SetStartTime = new DateTime(2024, 5, 13, 19, 19, 0), SetDuration = TimeSpan.FromMinutes(8), IsFinished = true },
                }
            },
            new Match
            {
                Player1Id = mima.Id,
                Player2Id = truls.Id,
                BestOfSets = BestOf.BestOf5,
                MatchDate = new DateTime(2024, 5, 17, 16, 45, 0),
                Player1Score = 2,
                Player2Score = 3,
                IsFinished = true,
                IsActive = true,
                Sets = new List<Set>
                {
                    new Set { SetNumber = 1, Player1Points = 10, Player2Points = 12, SetStartTime = new DateTime(2024, 5, 17, 16, 46, 0), SetDuration = TimeSpan.FromMinutes(6), IsFinished = true },
                    new Set { SetNumber = 2, Player1Points = 11, Player2Points = 6, SetStartTime = new DateTime(2024, 5, 17, 16, 52, 0), SetDuration = TimeSpan.FromMinutes(7), IsFinished = true },
                    new Set { SetNumber = 3, Player1Points = 9, Player2Points = 11, SetStartTime = new DateTime(2024, 5, 17, 16, 59, 0), SetDuration = TimeSpan.FromMinutes(8), IsFinished = true },
                    new Set { SetNumber = 4, Player1Points = 11, Player2Points = 7, SetStartTime = new DateTime(2024, 5, 17, 17, 07, 0), SetDuration = TimeSpan.FromMinutes(7), IsFinished = true },
                    new Set { SetNumber = 5, Player1Points = 7, Player2Points = 11, SetStartTime = new DateTime(2024, 5, 17, 17, 14, 0), SetDuration = TimeSpan.FromMinutes(6), IsFinished = true },
                }
            }
        };

                _dbContext.Matches.AddRange(matches);
                _dbContext.SaveChanges();
            }
        }
    }
}


