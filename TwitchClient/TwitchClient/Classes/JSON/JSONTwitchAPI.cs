using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchClient.Classes.JSONTwitchAPI
{
	class JSONTwitch
	{
		// USER CLASS
		public class UserLinks
		{
			public string self { get; set; }
		}

		public class UserNotifications
		{
			public bool push { get; set; }
			public bool email { get; set; }
		}

		public class User
		{
			public string error { get; set; }
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
	}
}
