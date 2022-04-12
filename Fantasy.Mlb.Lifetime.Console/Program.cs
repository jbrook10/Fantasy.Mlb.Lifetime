using System;
using System.Threading.Tasks;
using Fantasy.Mlb.Lifetime.Business;

namespace Fantasy.Mlb.Lifetime.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {

            // var rosterBuilder = new RosterBuilder();
            // await rosterBuilder.BuildRoster(2022);

            var playerReader = new PlayerReader();
            await playerReader.GetAllStats(2022, Domain.Enums.SeasonType.Regular);


        }
    }
}
