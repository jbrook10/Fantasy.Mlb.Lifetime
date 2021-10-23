namespace Fantasy.Mlb.Lifetime.Domain
{
    public abstract class Player
    {
        public string FantasyOwner { get; set; }

        public string Name { get; set; }
        public string Link { get; set; }
        

        public Position Position { get; set; }

        public int GamesPlayed { get; set; }

        public int Age { get; set; }

        public abstract int GetFantasyPoints();
    }

    public class Batter : Player
    {
        public Batter(string name)
        {
            Name = name;
            Position = Position.Batter;
        }

        public int AB { get; set; }

        public int H { get; set; }

        public int R { get; set; }

        public int HR { get; set; }

        public int RBI { get; set; }

        public int SB { get; set; }

        public int BB { get; set; }


        public override int GetFantasyPoints()
        {
            return H + R + HR + RBI + SB + BB;
        }
    }

    public class Pitcher : Player
    {
        public Pitcher(string name)
        {
            Name = name;
            Position = Position.Pitcher;
        }

        public bool Probable { get; set; }
        
        public int W { get; set; }

        public int IP { get; set; }

        public int SO { get; set; }

        public int SV { get; set; }

        public override int GetFantasyPoints()
        {
            return (W * 4) + (SV * 5) + IP + SO;
        }
    }


}