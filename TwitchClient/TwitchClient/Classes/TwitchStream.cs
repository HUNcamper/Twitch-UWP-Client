using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchClient.Classes
{
    public class Stream
    {
        public string streamerName { get; set; }
        public string gameName { get; set; }
        public int views { get; set; }
        public string preview { get; set; }
        public Stream()
        {
            this.streamerName = "Ninja";
            this.gameName = "Fortnite";
            this.views = 1152;
        }
        /*public string OneLineSummary
        {
            get
            {
                return $"{this.CompositionName} by {this.ArtistName}, released: "
                    + this.ReleaseDateTime.ToString("d");
            }
        }*/
    }
    /*public class StreamViewModel
    {
        private Stream defaultStream = new Stream();
        public Stream DefaultStream { get { return this.defaultStream; } }
    }*/
}
