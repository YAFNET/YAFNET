using System;
using System.Collections;
using System.Web;

namespace yaf
{
	/// <summary>
	/// All references to session should go into this class
	/// </summary>
	public class Mession
	{
		static public DateTime LastVisit
		{
			get
			{
				if(HttpContext.Current.Session["lastvisit"]!=null)
					return (DateTime)HttpContext.Current.Session["lastvisit"];
				else
					return DateTime.MinValue;
			}
			set
			{
				HttpContext.Current.Session["lastvisit"] = value;
			}
		}

		static public DateTime LastPost
		{
			get
			{
				if(HttpContext.Current.Session["lastpost"]!=null)
					return (DateTime)HttpContext.Current.Session["lastpost"];
				else
					return DateTime.MinValue;
			}
			set
			{
				HttpContext.Current.Session["lastpost"] = value;
			}
		}

		static public Hashtable TopicRead
		{
			get
			{
				if(HttpContext.Current.Session["topicread"]!=null)
					return (Hashtable)HttpContext.Current.Session["topicread"];
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session["topicread"] = value;
			}
		}

		static public Hashtable ForumRead
		{
			get
			{
				if(HttpContext.Current.Session["forumread"]!=null)
					return (Hashtable)HttpContext.Current.Session["forumread"];
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session["forumread"] = value;
			}
		}

		static public int ShowList
		{
			get
			{
				if(HttpContext.Current.Session["showlist"]!=null)
					return (int)HttpContext.Current.Session["showlist"];
				else
					return 0;
			}
			set
			{
				HttpContext.Current.Session["showlist"] = value;
			}
		}

		static public int UnreadTopics
		{
			get
			{
				if(HttpContext.Current.Session["unreadtopics"]!=null)
					return (int)HttpContext.Current.Session["unreadtopics"];
				else
					return 0;
			}
			set
			{
				HttpContext.Current.Session["unreadtopics"] = value;
			}
		}

		static public IEnumerable SearchData
		{
			get
			{
				if(HttpContext.Current.Session["searchds"]!=null)
					return (IEnumerable)HttpContext.Current.Session["searchds"];
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session["searchds"] = value;
			}
		}
	}
}
