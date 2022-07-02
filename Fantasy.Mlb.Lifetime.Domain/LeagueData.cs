using System;
using System.Collections.Generic;
using static Fantasy.Mlb.Lifetime.Domain.Enums;

namespace Fantasy.Mlb.Lifetime.Domain
{
    public class LeagueData
    {
        public LeagueData()
        {
            
        }
        
        public int Year { get; set; }
        public  SeasonType  SeasonType { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ProbablesDate { get; set; }    
        public List<Owner> Owners { get; set; }          
    }
}