using System;
using System.Collections;
using System.Web;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// All references to session should go into this class
	/// </summary>
	public class Mession
	{

		/// <summary>
		/// Gets the last time the forum was read.
		/// </summary>
		/// <param name="forumID">This is the ID of the forum you wish to get the last read date from.</param>
		/// <returns>A DateTime object of when the forum was last read.</returns>
		static public DateTime GetForumRead( int forumID )
		{
			System.Collections.Hashtable t = ForumRead;
			if ( t == null || !t.ContainsKey( forumID ) )
				return ( DateTime ) LastVisit;
			else
				return ( DateTime ) t [forumID];
		}

		/// <summary>
		/// Sets the time that the forum was read.
		/// </summary>
		/// <param name="forumID">The forum ID that was read.</param>
		/// <param name="date">The DateTime you wish to set the read to.</param>
		static public void SetForumRead( int forumID, DateTime date )
		{
			System.Collections.Hashtable t = ForumRead;
			if ( t == null )
			{
				t = new System.Collections.Hashtable();
			}
			t [forumID] = date;
			ForumRead = t;
		}

		/// <summary>
		/// Returns the last time that the topicID was read.
		/// </summary>
		/// <param name="topicID">The topicID you wish to find the DateTime object for.</param>
		/// <returns>The DateTime object from the topicID.</returns>
		static public DateTime GetTopicRead( int topicID )
		{
			System.Collections.Hashtable t = TopicRead;
			if ( t == null || !t.ContainsKey( topicID ) )
				return ( DateTime ) LastVisit;
			else
				return ( DateTime ) t [topicID];
		}

		/// <summary>
		/// Sets the time that the topicID was read.
		/// </summary>
		/// <param name="topicID">The topic ID that was read.</param>
		/// <param name="date">The DateTime you wish to set the read to.</param>
		static public void SetTopicRead( int topicID, DateTime date )
		{
			System.Collections.Hashtable t = TopicRead;
			if ( t == null )
			{
				t = new System.Collections.Hashtable();
			}
			t [topicID] = date;
			TopicRead = t;
		}

		static public PanelSessionState PanelState
		{
			get
			{
				return new PanelSessionState();
			}
		}

		static public DateTime LastVisit
		{
			get
			{
				if ( HttpContext.Current.Session ["lastvisit"] != null )
					return ( DateTime ) HttpContext.Current.Session ["lastvisit"];
				else
					return DateTime.MinValue;
			}
			set
			{
				HttpContext.Current.Session ["lastvisit"] = value;
			}
		}

		static public bool HasLastVisit
		{
			get
			{
				if ( HttpContext.Current.Session ["haslastvisit"] != null )
					return ( bool ) HttpContext.Current.Session ["haslastvisit"];
				else
					return false;
			}
			set
			{
				HttpContext.Current.Session ["haslastvisit"] = value;
			}
		}

		static public DateTime LastPost
		{
			get
			{
				if ( HttpContext.Current.Session ["lastpost"] != null )
					return ( DateTime ) HttpContext.Current.Session ["lastpost"];
				else
					return DateTime.MinValue;
			}
			set
			{
				HttpContext.Current.Session ["lastpost"] = value;
			}
		}

		static public Hashtable TopicRead
		{
			get
			{
				if ( HttpContext.Current.Session ["topicread"] != null )
					return ( Hashtable ) HttpContext.Current.Session ["topicread"];
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session ["topicread"] = value;
			}
		}

		static public Hashtable ForumRead
		{
			get
			{
				if ( HttpContext.Current.Session ["forumread"] != null )
					return ( Hashtable ) HttpContext.Current.Session ["forumread"];
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session ["forumread"] = value;
			}
		}

		static public int ShowList
		{
			get
			{
				if ( HttpContext.Current.Session ["showlist"] != null )
				{
					return ( int ) HttpContext.Current.Session ["showlist"];
				}
				else
				{
					// nothing in session
					return -1;
				}
			}
			set
			{
				HttpContext.Current.Session ["showlist"] = value;
			}
		}

		static public int UnreadTopics
		{
			get
			{
				if ( HttpContext.Current.Session ["unreadtopics"] != null )
					return ( int ) HttpContext.Current.Session ["unreadtopics"];
				else
					return 0;
			}
			set
			{
				HttpContext.Current.Session ["unreadtopics"] = value;
			}
		}

		static public System.Data.DataTable SearchData
		{
			get
			{
				if ( HttpContext.Current.Session ["SearchDataTable"] != null )
				{
					return ( System.Data.DataTable ) HttpContext.Current.Session ["SearchDataTable"];
				}
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session ["SearchDataTable"] = value;
			}
		}
	}

	public class PanelSessionState	
	{
		public enum CollapsiblePanelState
		{
			None = -1,
			Expanded = 0,
			Collapsed = 1
		}

		/// <summary>
		/// Gets panel session state.
		/// </summary>
		/// <param name="panelID">panelID</param>
		/// <returns></returns>
		public CollapsiblePanelState this [string panelID]
		{
			get
			{
				string sessionPanelID = "panelstate_" + panelID;

				if ( HttpContext.Current.Session [sessionPanelID] != null )
					return ( CollapsiblePanelState ) HttpContext.Current.Session [sessionPanelID];
				else
					return CollapsiblePanelState.None;
			}
			set
			{
				string sessionPanelID = "panelstate_" + panelID;

				HttpContext.Current.Session [sessionPanelID] = value;
			}
		}

		public void TogglePanelState( string panelID, CollapsiblePanelState defaultState )
		{
			CollapsiblePanelState currentState = this [panelID];

			if ( currentState == CollapsiblePanelState.None ) currentState = defaultState;

			if ( currentState == CollapsiblePanelState.Collapsed )
			{
				this [panelID] = CollapsiblePanelState.Expanded;
			}
			else if ( currentState == CollapsiblePanelState.Expanded )
			{
				this [panelID] = CollapsiblePanelState.Collapsed;
			}
		}
	}
}
