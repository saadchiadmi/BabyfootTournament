using babyfoot.Models;
using babyfoot.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.RequestManagers
{
    public class PlayerManager
    {
        private BabyfootDbContext context;

        public PlayerManager(BabyfootDbContext context)
        {
            this.context = context;
        }

        public Player Add(String pseudo)
        {
            var player = new Player
            {
                Pseudo = pseudo,
                Score = BabyfootElo.Initial,
                Goals = 0,
                Champions = 0
            };

            context.Add(player);

            context.SaveChanges();

            return player;
        }

        public List<Player> AddMany(List<String> pseudos)
        {
            var players = new List<Player>();

            foreach (String pseudo in pseudos)
            {
                var player = new Player
                {
                    Pseudo = pseudo,
                    Score = BabyfootElo.Initial,
                    Goals = 0,
                    Champions = 0
                };

                context.Add(player);

                players.Add(player);
            }

            context.SaveChanges();

            return players;
        }
    }
}
