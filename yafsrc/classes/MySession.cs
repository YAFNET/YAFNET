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
