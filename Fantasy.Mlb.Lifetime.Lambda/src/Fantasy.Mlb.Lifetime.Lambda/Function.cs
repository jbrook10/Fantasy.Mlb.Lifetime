using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fantasy.Mlb.Lifetime.Domain;
using Fantasy.Mlb.Lifetime.Business;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Fantasy.Mlb.Lifetime.Lambda
{
    public class LeagueRequest 
    {
        public int Year { get; set; }
        public Enums.SeasonType SeasonType { get; set; }
    }

    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(LeagueRequest input, ILambdaContext context)
        {
            var year = DateTime.UtcNow.Year;

            var rosterBuilder = new RosterBuilder();
            Task.Run(() => rosterBuilder.BuildRoster(input.Year)).GetAwaiter().GetResult();

            var playerReader = new PlayerReader();
            Task.Run(() => playerReader.GetAllStats(input.Year, input.SeasonType)).GetAwaiter().GetResult();

            return string.Empty;

        }
    }
}
