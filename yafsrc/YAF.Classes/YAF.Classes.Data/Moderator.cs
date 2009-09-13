using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Data
{
	public class Moderator
	{
		public Moderator( long forumID, long moderatorID, string name, bool isGroup )
		{
			ForumID = forumID;
			ModeratorID = moderatorID;
			Name = name;
			IsGroup = isGroup;
		}

		public long ForumID
		{
			get;
			set;
		}

		public long ModeratorID
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool IsGroup
		{
			get;
			set;
		}
	}
}
