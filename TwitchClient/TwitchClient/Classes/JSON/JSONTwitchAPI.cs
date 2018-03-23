using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchClient.Classes.JSONTwitchAPI
{
	/// <summary>
	/// Class containing classes for deserializing requests from the Twitch API
	/// </summary>
	class JSONTwitch
	{
		#region ERROR
		public class ErrorMessage
		{
			public string error { get; set; }
			public int status { get; set; }
			public string message { get; set; }
		}
		#endregion

		#region USER
		// https://api.twitch.tv/kraken/user/

		public class UserLinks
		{
			public string self { get; set; }
		}

		public class UserNotifications
		{
			public bool push { get; set; }
			public bool email { get; set; }
		}

		public class User : ErrorMessage
		{
			public string display_name { get; set; }
			public int _id { get; set; }
			public string name { get; set; }
			public string type { get; set; }
			public string bio { get; set; }
			public DateTime created_at { get; set; }
			public DateTime updated_at { get; set; }
			public string logo { get; set; }
			public UserLinks _links { get; set; }
			public string email { get; set; }
			public bool partnered { get; set; }
			public UserNotifications notifications { get; set; }
		}
		#endregion

		#region STREAMS
		// https://api.twitch.tv/kraken/streams/

		public class StreamsPreview
		{
			public string small { get; set; }
			public string medium { get; set; }
			public string large { get; set; }
			public string template { get; set; }
		}

		public class StreamsLinks
		{
			public string self { get; set; }
			public string follows { get; set; }
			public string commercial { get; set; }
			public string stream_key { get; set; }
			public string chat { get; set; }
			public string features { get; set; }
			public string subscriptions { get; set; }
			public string editors { get; set; }
			public string teams { get; set; }
			public string videos { get; set; }
		}

		public class StreamsChannel
		{
			public bool mature { get; set; }
			public bool partner { get; set; }
			public string status { get; set; }
			public string broadcaster_language { get; set; }
			public string display_name { get; set; }
			public string game { get; set; }
			public string language { get; set; }
			public int _id { get; set; }
			public string name { get; set; }
			public DateTime created_at { get; set; }
			public DateTime updated_at { get; set; }
			public object delay { get; set; }
			public string logo { get; set; }
			public object banner { get; set; }
			public string video_banner { get; set; }
			public object background { get; set; }
			public string profile_banner { get; set; }
			public string profile_banner_background_color { get; set; }
			public string url { get; set; }
			public int views { get; set; }
			public int followers { get; set; }
			public StreamsLinks _links { get; set; }
		}

		public class StreamsLinks2
		{
			public string self { get; set; }
		}

		public class StreamsStream
		{
			public object _id { get; set; }
			public string game { get; set; }
			public int viewers { get; set; }
			public int video_height { get; set; }
			public double average_fps { get; set; }
			public int delay { get; set; }
			public DateTime created_at { get; set; }
			public bool is_playlist { get; set; }
			public string stream_type { get; set; }
			public StreamsPreview preview { get; set; }
			public StreamsChannel channel { get; set; }
			public StreamsLinks2 _links { get; set; }
		}

		public class StreamsLinks3
		{
			public string self { get; set; }
			public string next { get; set; }
			public string featured { get; set; }
			public string summary { get; set; }
			public string followed { get; set; }
		}

		public class Streams : ErrorMessage
		{
			public int _total { get; set; }
			public List<StreamsStream> streams { get; set; }
			public StreamsLinks3 _links { get; set; }
		}
		#endregion
	}
}
