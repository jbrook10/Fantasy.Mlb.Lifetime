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
            // await rosterBuilder.BuildRoster(2021);

            var playerReader = new PlayerReader();
            await playerReader.GetAllStats(2021, Domain.Enums.SeasonType.Post);


        }
    }
}
