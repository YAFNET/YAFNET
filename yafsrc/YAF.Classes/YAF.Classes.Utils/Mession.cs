/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Utils
{
  using System;
  using System.Collections;
  using System.Data;
  using System.Web;
  using System.Collections.Generic;

  /// <summary>
  /// All references to session should go into this class
  /// </summary>
  public class Mession
  {
    /// <summary>
    /// Gets PanelState.
    /// </summary>
    public static PanelSessionState PanelState
    {
      get
      {
        return new PanelSessionState();
      }
    }

    /// <summary>
    /// Gets or sets LastVisit.
    /// </summary>
    public static DateTime LastVisit
    {
      get
      {
        if (HttpContext.Current.Session["lastvisit"] != null)
        {
          return (DateTime) HttpContext.Current.Session["lastvisit"];
        }
        else
        {
          return DateTime.MinValue;
        }
      }

      set
      {
        HttpContext.Current.Session["lastvisit"] = value;
      }
    }

    /// <summary>
    /// Gets or sets LastPm.
    /// </summary>
    public static DateTime LastPm
    {
      get
      {
        if (HttpContext.Current.Session["lastpm"] != null)
        {
          return (DateTime) HttpContext.Current.Session["lastpm"];
        }
        else
        {
          return DateTime.MinValue;
        }
      }

      set
      {
        HttpContext.Current.Session["lastpm"] = value;
      }
    }

    /// <summary>
    /// Gets or sets LastPm.
    /// </summary>
    public static DateTime LastPendingBuddies
    {
        get
        {
            if (HttpContext.Current.Session["lastpendingbuddies"] != null)
            {
                return (DateTime)HttpContext.Current.Session["lastpendingbuddies"];
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        set
        {
            HttpContext.Current.Session["lastpendingbuddies"] = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether HasLastVisit.
    /// </summary>
    public static bool HasLastVisit
    {
      get
      {
        if (HttpContext.Current.Session["haslastvisit"] != null)
        {
          return (bool) HttpContext.Current.Session["haslastvisit"];
        }
        else
        {
          return false;
        }
      }

      set
      {
        HttpContext.Current.Session["haslastvisit"] = value;
      }
    }

    /// <summary>
    /// Gets or sets LastPost.
    /// </summary>
    public static DateTime LastPost
    {
      get
      {
        if (HttpContext.Current.Session["lastpost"] != null)
        {
          return (DateTime) HttpContext.Current.Session["lastpost"];
        }
        else
        {
          return DateTime.MinValue;
        }
      }

      set
      {
        HttpContext.Current.Session["lastpost"] = value;
      }
    }

    /// <summary>
    /// Gets or sets TopicRead.
    /// </summary>
    public static Hashtable TopicRead
    {
      get
      {
        if (HttpContext.Current.Session["topicread"] != null)
        {
          return (Hashtable) HttpContext.Current.Session["topicread"];
        }
        else
        {
          return null;
        }
      }

      set
      {
        HttpContext.Current.Session["topicread"] = value;
      }
    }

    /// <summary>
    /// Gets or sets ForumRead.
    /// </summary>
    public static Hashtable ForumRead
    {
      get
      {
        if (HttpContext.Current.Session["forumread"] != null)
        {
          return (Hashtable) HttpContext.Current.Session["forumread"];
        }
        else
        {
          return null;
        }
      }

      set
      {
        HttpContext.Current.Session["forumread"] = value;
      }
    }

    /// <summary>
    /// Gets or sets ShowList.
    /// </summary>
    public static int ShowList
    {
      get
      {
        if (HttpContext.Current.Session["showlist"] != null)
        {
          return (int) HttpContext.Current.Session["showlist"];
        }
        else
        {
          // nothing in session
          return -1;
        }
      }

      set
      {
        HttpContext.Current.Session["showlist"] = value;
      }
    }

    /// <summary>
    /// Gets or sets UnreadTopics.
    /// </summary>
    public static int UnreadTopics
    {
      get
      {
        if (HttpContext.Current.Session["unreadtopics"] != null)
        {
          return (int) HttpContext.Current.Session["unreadtopics"];
        }
        else
        {
          return 0;
        }
      }

      set
      {
        HttpContext.Current.Session["unreadtopics"] = value;
      }
    }

    /// <summary>
    /// Gets or sets SearchData.
    /// </summary>
    public static DataTable SearchData
    {
      get
      {
        if (HttpContext.Current.Session["SearchDataTable"] != null)
        {
          return HttpContext.Current.Session["SearchDataTable"] as DataTable;
        }
        else
        {
          return null;
        }
      }

      set
      {
        HttpContext.Current.Session["SearchDataTable"] = value;
      }
    }

    /// <summary>
    /// Gets or sets ActiveTopicSince.
    /// </summary>
    public static int? ActiveTopicSince
    {
      get
      {
        if (HttpContext.Current.Session["ActiveTopicSince"] != null)
        {
          return (int) HttpContext.Current.Session["ActiveTopicSince"];
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session["ActiveTopicSince"] = value;
      }
    }

    /// <summary>
    /// Gets or sets FavoriteTopicSince.
    /// </summary>
    public static int? FavoriteTopicSince
    {
        get
        {
            if (HttpContext.Current.Session["FavoriteTopicSince"] != null)
            {
                return (int)HttpContext.Current.Session["FavoriteTopicSince"];
            }

            return null;
        }

        set
        {
            HttpContext.Current.Session["FavoriteTopicSince"] = value;
        }
    }

    /// <summary>
    /// Gets the last time the forum was read.
    /// </summary>
    /// <param name="forumID">
    /// This is the ID of the forum you wish to get the last read date from.
    /// </param>
    /// <returns>
    /// A DateTime object of when the forum was last read.
    /// </returns>
    public static DateTime GetForumRead(int forumID)
    {
      Hashtable t = ForumRead;
      if (t == null || !t.ContainsKey(forumID))
      {
        return LastVisit;
      }
      else
      {
        return (DateTime) t[forumID];
      }
    }

    /// <summary>
    /// Sets the time that the forum was read.
    /// </summary>
    /// <param name="forumID">
    /// The forum ID that was read.
    /// </param>
    /// <param name="date">
    /// The DateTime you wish to set the read to.
    /// </param>
    public static void SetForumRead(int forumID, DateTime date)
    {
      Hashtable t = ForumRead ?? new Hashtable();

      t[forumID] = date;
      ForumRead = t;
    }

    /// <summary>
    /// Returns the last time that the topicID was read.
    /// </summary>
    /// <param name="topicID">
    /// The topicID you wish to find the DateTime object for.
    /// </param>
    /// <returns>
    /// The DateTime object from the topicID.
    /// </returns>
    public static DateTime GetTopicRead(int topicID)
    {
      Hashtable t = TopicRead;
      if (t == null || !t.ContainsKey(topicID))
      {
        return LastVisit;
      }
      else
      {
        return (DateTime) t[topicID];
      }
    }

    /// <summary>
    /// Sets the time that the <paramref name="topicID"/> was read.
    /// </summary>
    /// <param name="topicID">
    /// The topic ID that was read.
    /// </param>
    /// <param name="date">
    /// The DateTime you wish to set the read to.
    /// </param>
    public static void SetTopicRead(int topicID, DateTime date)
    {
      Hashtable t = TopicRead;
      if (t == null)
      {
        t = new Hashtable();
      }

      t[topicID] = date;
      TopicRead = t;
    }
  }

  /// <summary>
  /// The panel session state.
  /// </summary>
  public class PanelSessionState
  {
    #region CollapsiblePanelState enum

    /// <summary>
    /// The collapsible panel state.
    /// </summary>
    public enum CollapsiblePanelState
    {
      /// <summary>
      /// The none.
      /// </summary>
      None = -1, 

      /// <summary>
      /// The expanded.
      /// </summary>
      Expanded = 0, 

      /// <summary>
      /// The collapsed.
      /// </summary>
      Collapsed = 1
    }

    #endregion

    /// <summary>
    /// Gets panel session state.
    /// </summary>
    /// <param name="panelID">panelID</param>
    /// <returns></returns>
    public CollapsiblePanelState this[string panelID]
    {
      // Ederon : 7/14/2007
      get
      {
        string sessionPanelID = "panelstate_" + panelID;

        // try to get panel state from session state first
        if (HttpContext.Current.Session[sessionPanelID] != null)
        {
          return (CollapsiblePanelState) HttpContext.Current.Session[sessionPanelID];
        }
          
          // if no panel state info is in session state, try cookie
        else if (HttpContext.Current.Request.Cookies[sessionPanelID] != null)
        {
          try
          {
            // we must convert string to int, better get is safe
            if (HttpContext.Current.Request != null)
            {
              return (CollapsiblePanelState) int.Parse(HttpContext.Current.Request.Cookies[sessionPanelID].Value);
            }
          }
          catch
          {
            // in case cookie has wrong value
            if (HttpContext.Current.Request != null)
            {
              HttpContext.Current.Request.Cookies.Remove(sessionPanelID); // scrap wrong cookie
            }

            return CollapsiblePanelState.None;
          }
        }

        return CollapsiblePanelState.None;
      }
 // Ederon : 7/14/2007
      set
      {
        string sessionPanelID = "panelstate_" + panelID;

        HttpContext.Current.Session[sessionPanelID] = value;

        // create persistent cookie with visibility setting for panel
        var c = new HttpCookie(sessionPanelID, ((int) value).ToString());
        c.Expires = DateTime.UtcNow.AddYears(1);
        HttpContext.Current.Response.SetCookie(c);
      }
    }

    /// <summary>
    /// The toggle panel state.
    /// </summary>
    /// <param name="panelID">
    /// The panel id.
    /// </param>
    /// <param name="defaultState">
    /// The default state.
    /// </param>
    public void TogglePanelState(string panelID, CollapsiblePanelState defaultState)
    {
      CollapsiblePanelState currentState = this[panelID];

      if (currentState == CollapsiblePanelState.None)
      {
        currentState = defaultState;
      }

      if (currentState == CollapsiblePanelState.Collapsed)
      {
        this[panelID] = CollapsiblePanelState.Expanded;
      }
      else if (currentState == CollapsiblePanelState.Expanded)
      {
        this[panelID] = CollapsiblePanelState.Collapsed;
      }
    }
  }
}