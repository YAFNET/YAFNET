/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
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
			// Ederon : 7/14/2007
			get
			{
				string sessionPanelID = "panelstate_" + panelID;

				// try to get panel state from session state first
				if (HttpContext.Current.Session[sessionPanelID] != null)
				{
					return (CollapsiblePanelState)HttpContext.Current.Session[sessionPanelID];
				}
				// if no panel state info is in session state, try cookie
				else if (HttpContext.Current.Request.Cookies[sessionPanelID] != null)
				{
					try
					{
						// we must convert string to int, better get is safe
						return (CollapsiblePanelState)int.Parse(HttpContext.Current.Request.Cookies[sessionPanelID].Value);
					}
					catch
					{
						// in case cookie has wrong value
						HttpContext.Current.Request.Cookies.Remove(sessionPanelID);	// scrap wrong cookie
						return CollapsiblePanelState.None;
					}
				}
				else
				{
					return CollapsiblePanelState.None;
				}
			}
			// Ederon : 7/14/2007
			set
			{
				string sessionPanelID = "panelstate_" + panelID;

				HttpContext.Current.Session [sessionPanelID] = value;

				// create persistent cookie with visibility setting for panel
				HttpCookie c = new HttpCookie(sessionPanelID, ((int)value).ToString());
				c.Expires = DateTime.Now.AddYears(1);
				HttpContext.Current.Response.SetCookie(c);
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
