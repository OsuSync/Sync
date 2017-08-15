using osu_database_reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NowPlaying
{
    public static class OsuFileParser
    {
        public static BeatmapEntry ParseText(string content)
        {
            BeatmapEntry entry = null;

            var parser_data = PickValues(ref content);

            try
            {
                entry = new BeatmapEntry();

                entry.Artist = parser_data["Artist"];
                entry.ArtistUnicode = parser_data["ArtistUnicode"];
                entry.Title = parser_data["Title"];
                entry.TitleUnicode = parser_data["TitleUnicode"];
                entry.Creator = parser_data["Creator"];
                entry.SongSource = parser_data["Source"];
                entry.SongTags = parser_data["Tags"];
                entry.BeatmapId = int.Parse(parser_data["BeatmapID"]);
                entry.BeatmapSetId = int.Parse(parser_data["BeatmapSetID"]);
                entry.Difficulty = parser_data["Version"];
                entry.DiffAR = float.Parse(parser_data["ApproachRate"]);
                entry.DiffOD = float.Parse(parser_data["OverallDifficulty"]);
                entry.DiffCS = float.Parse(parser_data["CircleSize"]);
                entry.DiffHP = float.Parse(parser_data["HPDrainRate"]);
            }
            catch
            {
                return null;
            }

            return entry;
        }

        static Dictionary<string, string> PickValues(ref string content)
        {
            MatchCollection result = Regex.Matches(content, $@"^(\w+):(.*)$", RegexOptions.Multiline);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (Match match in result)
            {
                dic.Add(match.Groups[1].Value, match.Groups[2].Value.Replace("\r", string.Empty));
            }

            return dic;
        }
    }
}
