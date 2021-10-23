using System.Collections.Generic;
using System.Linq;

namespace Fantasy.Mlb.Lifetime.Domain
{
    public class Owner
    {
        public string Name { get; set; }
        public List<Batter> Batters { get; set; }
        public List<Pitcher> Pitchers { get; set; }
        public int CountingPlayers { get; set; }
        public int BatterScore => Batters.Where(p => p.Position == Position.Batter).OrderByDescending(p => p.GetFantasyPoints()).Take(CountingPlayers).Sum(p => p.GetFantasyPoints());
        public int PitcherScore => Pitchers.Where(p => p.Position == Position.Pitcher).OrderByDescending(p => p.GetFantasyPoints()).Take(CountingPlayers).Sum(p => p.GetFantasyPoints());    

    }
}