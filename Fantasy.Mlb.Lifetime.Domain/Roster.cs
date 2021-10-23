namespace Fantasy.Mlb.Lifetime.Domain
{
    public class Roster
    {
        public Roster(string rawEntry)
        {
            var parts = rawEntry.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            Owner = parts[0].Trim();
            Name = parts[1].Trim();
            Position = parts[2].ToLower().Trim() == "bat" ? Position.Batter : Position.Pitcher;
            Link = parts.Length >= 4 ? parts[3].Trim() : string.Empty;
        }

        public string Owner { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
        public string Link { get; set; }

        public string RosterText() {
            return $"{Owner} | {Name} | {PostitionText()} | {Link}";
        }

        public string PostitionText() {
            return Position == Position.Batter ? "Bat" : "P";
        }
    }

    public enum Position
    {
        None = 0,
        Batter = 1,
        Pitcher = 2
    }
}