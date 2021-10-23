using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Fantasy.Mlb.Lifetime.Domain;
using HtmlAgilityPack;
using Amazon.S3;
using Amazon;

namespace Fantasy.Mlb.Lifetime.Business
{
    public class RosterBuilder
    {
        static readonly HttpClient client = new HttpClient();
        private AmazonS3Client  _s3Client;
        

        public async Task BuildRoster(int year)
        {
            _s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var objectResponse = await _s3Client.GetObjectAsync("mlb-lifetime","data/RosterRaw.txt" );
            StreamReader reader = new StreamReader(objectResponse.ResponseStream);
            string rosterContent = reader.ReadToEnd();
            var rosterRaw = rosterContent.Split("\r\n");

            var battingRosterUri = $@"https://www.baseball-reference.com/leagues/MLB/{year}-standard-batting.shtml";
            var pitchingRosterUri = $@"https://www.baseball-reference.com/leagues/MLB/{year}-standard-pitching.shtml";

            var battingRosterContent = await client.GetStringAsync(battingRosterUri);
            var pitchingRosterContent = await client.GetStringAsync(pitchingRosterUri);

            var battingTableSearchString = "<table class=\"sortable stats_table\" id=\"players_standard_batting\"";
            var battingTableStart = battingRosterContent.IndexOf(battingTableSearchString.Replace("\\", ""));
            var battingTableEnd = battingRosterContent.IndexOf("</table>", battingTableStart);

            var shortBattingContent = $"<html> {battingRosterContent.Substring(battingTableStart, battingTableEnd - battingTableStart + 8)} </html>";

            HtmlDocument battingDoc = new HtmlDocument();
            HtmlDocument pitchingDoc = new HtmlDocument(); 
            if (shortBattingContent != null)
            {
                battingDoc.LoadHtml(shortBattingContent);
            }
            else
            {
                battingDoc.LoadHtml("<html></html>");
            }

            var battingTable = battingDoc.GetElementbyId("players_standard_batting");

            var pitchingTableSearchString = "<table class=\"sortable stats_table\" id=\"players_standard_pitching\"";
            var pitchingTableStart = pitchingRosterContent.IndexOf(pitchingTableSearchString.Replace("\\", ""));
            var pitchingTableEnd = pitchingRosterContent.IndexOf("</table>", pitchingTableStart);

            var shortPitchingContent = $"<html> {pitchingRosterContent.Substring(pitchingTableStart, pitchingTableEnd - pitchingTableStart + 8)} </html>";

            if (pitchingRosterContent != null)
            {
                pitchingDoc.LoadHtml(shortPitchingContent);
            }
            else
            {
                pitchingDoc.LoadHtml("<html></html>");
            }

            var pitchingTable = pitchingDoc.GetElementbyId("players_standard_pitching");

            // var rosterRaw = File.ReadAllLines(@"D:\jerem\Documents\code\fantasy\Fantasy.Mlb.Lifetime\Fantasy.Mlb.Lifetime.Data\2021\RosterRaw.txt");

            await MergeRoster(battingTable, pitchingTable, rosterRaw, year);
        }

        private async Task MergeRoster(HtmlNode battingTable, HtmlNode pitchingTable, string[] rawRoster, int year)
        {
            var roster = new List<Roster>();

            foreach (var rawEntry in rawRoster)
            {
                if (string.IsNullOrEmpty(rawEntry)) {
                    continue;
                }

                var entry = new Roster(rawEntry);

                string link = FindLink(entry, entry.Position == Position.Batter ? battingTable : pitchingTable);

                entry.Link = link;
                roster.Add(entry);
            }

            await WriteRoster(roster, year); 

        }

        private async Task WriteRoster(List<Roster> roster, int year)
        {
            var s = new StringBuilder();
            foreach (var entry in roster)
            {
                s.AppendLine(entry.RosterText());
            }
            // var path = $@"D:\jerem\Documents\code\fantasy\Fantasy.Mlb.Lifetime\Fantasy.Mlb.Lifetime.Data\{year}\Roster.Final.txt";
            // File.WriteAllText(path, s.ToString());


            var putObjectRequest = new Amazon.S3.Model.PutObjectRequest() {
                BucketName = "mlb-lifetime",
                Key = "data/Roster.Final.txt",
                ContentBody = s.ToString(),
                ContentType = "text/plain"
            };
            await _s3Client.PutObjectAsync(putObjectRequest);
            
        }

        private string FindLink(Roster entry, HtmlNode node)
        {
            if (!string.IsNullOrEmpty(entry.Link.Trim())) {
                return entry.Link;
            }

            try
            {
                var searchableName = entry.Name.Replace(" ", "&nbsp;");
                var searchPath = $"//td[a='{searchableName}']/a";
                var playerNode = node.SelectSingleNode(searchPath);

                if (playerNode != null)
                {
                    var link = playerNode.GetAttributeValue("href", string.Empty);
                    var parts =  link.Replace(".shtml","").Split('/').ToList();
                    return parts.LastOrDefault();
                }
                return string.Empty;
            }
            catch (System.Exception ex)
            {
                // TODO
                return string.Empty;
            }

        }
    }
}