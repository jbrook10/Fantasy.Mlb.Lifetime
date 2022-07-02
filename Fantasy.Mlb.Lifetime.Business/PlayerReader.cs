using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Fantasy.Mlb.Lifetime.Domain;
using HtmlAgilityPack;
using System.Xml;
using System.Text.Json.Serialization;
using static Fantasy.Mlb.Lifetime.Domain.Enums;
using Amazon.S3;
using Amazon;

namespace Fantasy.Mlb.Lifetime.Business
{
    public class PlayerReader
    {
        private const string PitchingUri = "https://widgets.sports-reference.com/wg.fcgi?site=br&url=%2Fplayers%2F{0}%2F{1}.shtml&div=div_pitching_{2}&cx={3}";
        private const string BattingUri = "https://widgets.sports-reference.com/wg.fcgi?site=br&url=%2Fplayers%2F{0}%2F{1}.shtml&div=div_batting_{2}&cx={3}";
        static readonly HttpClient client = new HttpClient();
        private AmazonS3Client _s3Client;

        public async Task GetAllStats(int year, SeasonType seasonType)
        {
            _s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var objectResponse = await _s3Client.GetObjectAsync("mlb-lifetime", $"data/{year}.Roster.Final.txt");
            StreamReader reader = new StreamReader(objectResponse.ResponseStream);
            string rosterContent = reader.ReadToEnd();

            var splitChar = rosterContent.Contains("\r\n") ? "\r\n" : "\n";

            var roster = rosterContent.Split(splitChar);


            // string rosterPath = $@"D:\jerem\Documents\code\fantasy\Fantasy.Mlb.Lifetime\Fantasy.Mlb.Lifetime.Data\{year}\Roster.Final.txt";
            // var roster = File.ReadAllLines(rosterPath);

            //    var request = new GetObjectRequest();


            var probablesContent = await client.GetStringAsync("https://www.baseball-reference.com/previews/index.shtml");
            HtmlDocument probablesDoc = new HtmlDocument();
            probablesDoc.LoadHtml(probablesContent);

            probablesContent = probablesDoc.DocumentNode.SelectSingleNode("//div[@class='game_summaries']")?.InnerHtml ?? "";
            var probablesDate = probablesDoc.GetElementbyId("content").SelectSingleNode("h1").InnerText;

            var players = new ConcurrentBag<Player>();
            foreach (var player in roster)
            {
                if (string.IsNullOrEmpty(player)) continue;

                var playerObj = await ReadPlayerAsync(player, probablesContent, year, seasonType);
                players.Add(playerObj);
                Console.WriteLine(player + "     " + playerObj.GetFantasyPoints());
            }

            var owners = players.GroupBy(p => p.FantasyOwner);
            var ownerObjects = new List<Owner>();
            foreach (var owner in owners)
            {
                var ownerObject = new Owner()
                {
                    Name = owner.Key,
                    Batters = new List<Batter>(),
                    Pitchers = new List<Pitcher>(),
                    CountingPlayers = seasonType == SeasonType.Regular ? 10 : 5
                };

                var batters = owner.Where(p => p.Position == Position.Batter).Select(p => (Batter)p);
                var pitchers = owner.Where(p => p.Position == Position.Pitcher).Select(p => (Pitcher)p);

                ownerObject.Batters.AddRange(batters);
                ownerObject.Pitchers.AddRange(pitchers);

                ownerObjects.Add(ownerObject);
            }

            var leagueData = new LeagueData()
            {
                Year = year,
                SeasonType = seasonType,
                LastUpdated = DateTime.UtcNow,
                ProbablesDate = probablesDate,
                Owners = ownerObjects
            };

            var getObjectRequest = new Amazon.S3.Model.GetObjectRequest()
            {
                BucketName = "mlb-lifetime",
                Key = $"assets/{year}.{seasonType.ToString()}.Data.json",
            };

            var previousFileResponse = await _s3Client.GetObjectAsync(getObjectRequest);
            reader = new StreamReader(previousFileResponse.ResponseStream);
            string previousFileContent = reader.ReadToEnd();
            var previousFile = JsonSerializer.Deserialize<LeagueData>(previousFileContent);

            leagueData = AddChanges(previousFile, leagueData);

            string jsonString = JsonSerializer.Serialize(leagueData);
            // File.WriteAllText($@"D:\jerem\Documents\code\fantasy\Fantasy.Mlb.Lifetime\Fantasy.Mlb.Lifetime.Data\{year}\{year}.Regular.Data.json", jsonString);

            var putObjectRequest = new Amazon.S3.Model.PutObjectRequest()
            {
                BucketName = "mlb-lifetime",
                Key = $"assets/{year}.{seasonType.ToString()}.Data.json",
                ContentBody = jsonString,
                ContentType = "application/json"
            };
            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        private LeagueData AddChanges(LeagueData previous, LeagueData current) 
        {
            var cdate = DateTime.Now.DayOfYear - 1;
            var compDate = DateTime.Now.DayOfYear;

            foreach (var currentOwner in current.Owners)
            {
                var matchingOwner = previous.Owners.FirstOrDefault(o => o.Name == currentOwner.Name);
                if (matchingOwner == null) continue;

                foreach (var currentBatter in currentOwner.Batters) 
                {
                    var matchingBatter = matchingOwner.Batters?.FirstOrDefault(b => b.Name == currentBatter.Name);
                    var currentPoints = currentBatter.GetFantasyPoints();
                    var previousPoints =  matchingBatter.GetFantasyPoints();

                    if (currentPoints != previousPoints) {
                        currentBatter.Change = currentPoints - previousPoints;
                        currentBatter.CDate = cdate;
                    } 
                    else {
                        if (compDate - matchingBatter.CDate <= 1) 
                        {
                            currentBatter.Change = matchingBatter.Change;
                        }
                        else 
                        {
                            currentBatter.Change = 0;
                        }
                    }
                }

                foreach (var currentPitcher in currentOwner.Pitchers)
                {
                    var matchingPitcher = matchingOwner.Pitchers?.FirstOrDefault(b => b.Name == currentPitcher.Name);
                    var currentPoints = currentPitcher.GetFantasyPoints();
                    var previousPoints =  matchingPitcher.GetFantasyPoints();

                    if (currentPoints != previousPoints) {
                        currentPitcher.Change = currentPoints - previousPoints;
                        currentPitcher.CDate = cdate;
                    } 
                    else {
                        if (compDate - matchingPitcher.CDate <= 1) 
                        {
                            currentPitcher.Change = matchingPitcher.Change;
                        }
                        else 
                        {
                            currentPitcher.Change = 0;
                        }
                    }
                }
            }

            return current;
        }

        private async Task<Player> ReadPlayerAsync(string entry, string probables, int year, SeasonType seasonType)
        {
            var parts = entry.Split('|', 4, StringSplitOptions.RemoveEmptyEntries);
            var owner = parts[0].Trim();
            var name = parts[1].Trim();
            var position = parts[2].Trim();
            var link = parts.Length >= 4 ? parts[3].Trim() : string.Empty;

            if (position.ToLower() == "bat")
            {
                var batter = await GetBatter(name, link, year, seasonType).ConfigureAwait(false);
                batter.FantasyOwner = owner;
                return batter;
            }
            var pitcher = await GetPitcher(name, link, probables, year, seasonType);
            pitcher.FantasyOwner = owner;
            return pitcher;
        }

        private async Task<Batter> GetBatter(string name, string link, int year, SeasonType seasonType)
        {
            var batter = new Batter(name);
            batter.Link = link;

            if (string.IsNullOrEmpty(link))
            {
                return batter;
            }

            var allBatting = await client.GetStringAsync(string.Format(BattingUri, link.Substring(0, 1), link, GetSeasonUrlString(seasonType), DateTime.UtcNow.ToString("o")));
            var battingStart = allBatting.IndexOf("<table");

            if (battingStart >= 0)
            {
                var battingEnd = allBatting.IndexOf("</table>") + 8;

                allBatting = allBatting.Substring(battingStart, battingEnd - battingStart);
                HtmlDocument battingDoc = new HtmlDocument();
                battingDoc.LoadHtml(allBatting);

                HtmlNodeCollection battingNodes = null;

                if (seasonType == SeasonType.Regular)
                {
                    battingNodes = battingDoc.DocumentNode.SelectNodes($"//tr[@id='batting_{GetSeasonUrlString(seasonType)}.{year}']");
                } 
                else {
                    battingNodes = battingDoc.DocumentNode.SelectNodes($"//tr/th[starts-with(@csk,'{year}')]/..");
                }

                if (battingNodes == null || !battingNodes.Any()) return batter;

                batter = new Batter(name)
                {
                    Position = Position.Batter,
                    Link = $"https://www.baseball-reference.com/players/{link.Substring(0, 1)}/{link}.shtml",
                };

                foreach (var batting in battingNodes)
                {
                    batter.Age = GetByDataStat(batting, "age");
                    batter.GamesPlayed += GetByDataStat(batting, "G");
                    batter.AB += GetByDataStat(batting, "AB");
                    batter.BB += GetByDataStat(batting, "BB");
                    batter.H += GetByDataStat(batting, "H");
                    batter.HR += GetByDataStat(batting, "HR");
                    batter.R += GetByDataStat(batting, "R");
                    batter.RBI += GetByDataStat(batting, "RBI");
                    batter.SB += GetByDataStat(batting, "SB");
                }
            }

            return batter;
        }

        private async Task<Pitcher> GetPitcher(string name, string link, string probables, int year, SeasonType seasonType)
        {
            var pitcher = new Pitcher(name);
            pitcher.Link = link;

            if (string.IsNullOrEmpty(link))
            {
                return pitcher;
            }

            var allPitching = await client.GetStringAsync(string.Format(PitchingUri, link.Substring(0, 1), link, GetSeasonUrlString(seasonType), DateTime.UtcNow.ToString("o")));
            var pitchingStart = allPitching.IndexOf("<table");

            if (pitchingStart >= 0)
            {

                var pitchingEnd = allPitching.IndexOf("</table>") + 8;
                allPitching = allPitching.Substring(pitchingStart, pitchingEnd - pitchingStart);
                HtmlDocument pitchingDoc = new HtmlDocument();
                pitchingDoc.LoadHtml(allPitching);

                HtmlNodeCollection pitchingNodes;

                if (seasonType == SeasonType.Regular)
                {
                    pitchingNodes = pitchingDoc.DocumentNode.SelectNodes($"//tr[@id='pitching_{GetSeasonUrlString(seasonType)}.{year}']");
                } 
                else {
                    pitchingNodes = pitchingDoc.DocumentNode.SelectNodes($"//tr/th[starts-with(@csk,'{year}')]/..");
                }

                if (pitchingNodes == null || !pitchingNodes.Any()) return pitcher;

                pitcher = new Pitcher(name)
                {
                    Probable = probables.Contains(link),
                    Position = Position.Pitcher,
                    Link = $"https://www.baseball-reference.com/players/{link.Substring(0, 1)}/{link}.shtml",
                };


                foreach (var pitching in pitchingNodes)
                {
                    pitcher.GamesPlayed += GetByDataStat(pitching, "G");
                    pitcher.Age = GetByDataStat(pitching, "age");
                    pitcher.IP += GetByDataStat(pitching, "IP");
                    pitcher.SO += GetByDataStat(pitching, "SO");
                    pitcher.SV += GetByDataStat(pitching, "SV");
                    pitcher.W += GetByDataStat(pitching, "W");
                }
            }

            return pitcher;
        }

        private string GetSeasonUrlString(SeasonType type)
        {
            switch (type)
            {
                case SeasonType.Regular:
                    return "standard";
                case SeasonType.Post:
                    return "postseason";
                default:
                    throw new Exception("invalid season type");
            }
        }

        private int GetByDataStat(HtmlNode node, string stat)
        {
            var statText = node.SelectSingleNode($"td[@data-stat='{stat}']")?.InnerHtml ?? "";

            statText = statText.Replace("<strong>", "")
                               .Replace("</strong>", "")
                               .Replace("<em>", "")
                               .Replace("</em>", "");

            var preDecimal = statText.Split('.').FirstOrDefault();
            var parsed = int.TryParse(preDecimal, out var parsedValue);
            return parsedValue;
        }
    }
}
