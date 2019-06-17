using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchClient.Classes.JSONTwitchAPI;

namespace TwitchClient.Classes
{
    public class StreamViewModel
    {
        private ObservableCollection<Stream> streams = new ObservableCollection<Stream>();
        public ObservableCollection<Stream> Streams { get { return this.streams; } }

        public void FillStreams(JSONTwitch.Streams streamList)
        {
            foreach (var stream in streamList.streams)
            {
                this.streams.Add(new Stream()
                {
                    streamerName = stream.channel.name,
                    gameName = stream.game,
                    views = stream.viewers,
                    preview = stream.preview.medium
                });
            }
        }


        /*public StreamViewModel()
        {
            this.streams.Add(new Stream()
            {
                streamerName = "Ninja",
                gameName = "Fortnite",
                views = 1000
            });
            this.streams.Add(new Stream()
            {
                streamerName = "Ludwig van Beethoven",
                gameName = "Third Symphony",
                views = 654
            });
            this.streams.Add(new Stream()
            {
                streamerName = "George Frideric Handel",
                gameName = "Serse",
                views = 50
            });
        }*/
    }
}
