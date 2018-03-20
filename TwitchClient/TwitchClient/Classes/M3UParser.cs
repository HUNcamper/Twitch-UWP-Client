using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TwitchClient
{
    /// <summary>
    /// Struct for storing parsed M3U stream data.
    /// </summary>
    public struct M3U
    {
        public string name;
        public int bandwidth;
        public string url;
    }

    /// <summary>
    /// Class for parsing M3U data from Twitch.
    /// </summary>
    class M3UParser
    {
        // Regexes for gathering stream info
        private static Regex regexMediaName =              new Regex(",NAME=\"(.+)\",");
        private static Regex regexStreamBandwidth =        new Regex(",BANDWIDTH=([0-9]+),");

        /// <summary>
        /// Parse M3U data.
        /// </summary>
        /// <param name="m3uRaw">multiline M3U raw data</param>
        /// <returns>List of M3U objects with stream info</returns>
        public static List<M3U> Parse(string m3uRaw)
        {
            string[] lines = m3uRaw.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<M3U> m3uList = new List<M3U>();

            // Invalid argument format exceptions
            if (lines.Length == 0)                  throw new ArgumentException("Data must contain at least 1 line of text", "m3uRaw");
            if (lines[0] != "#EXTM3U")              throw new ArgumentException("Invalid M3U format (must start with '#EXTM3U')", "m3uRaw");

            M3U cObj = new M3U();
            foreach (var line in lines)
            {
                if (line.StartsWith("#EXT-X-TWITCH-INFO")) continue;
                
                // New Stream header
                if (line.StartsWith("#EXT-X-MEDIA"))
                {
                    cObj = new M3U();
                    cObj.bandwidth = -1;
                    cObj.name = "NULL";
                    cObj.url = "NULL";
                    Match streamName = regexMediaName.Match(line);

                    if (streamName.Success) cObj.name = streamName.Groups[1].Value;
                }
                // Stream info
                else if (line.StartsWith("#EXT-X-STREAM-INF"))
                {
                    Match streamBandwidth = regexStreamBandwidth.Match(line);

                    if (streamBandwidth.Success)
                    {
                        // If not int, set bandwidth to 0.
                        int bandwidth;
                        if (!int.TryParse(streamBandwidth.Groups[1].Value, out bandwidth))
                        {
                            bandwidth = 0;
                        }

                        cObj.bandwidth = bandwidth;
                    }
                }
                // Stream url
                else if (line.StartsWith("http://"))
                {
                    cObj.url = line;
                }

                // If all values set, add to list
                if (cObj.name != "NULL" && cObj.bandwidth != -1 && cObj.url != "NULL")
                {
                    m3uList.Add(cObj);
                }
            }

            if (m3uList.Count == 0)                 throw new ArgumentException("Invalid data, parsing failed.", "m3uRaw");

            return m3uList;
        }
    }
}
