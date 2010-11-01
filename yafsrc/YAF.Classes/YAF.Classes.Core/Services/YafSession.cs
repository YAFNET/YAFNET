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
  #region Using

  using System;
  using System.Collections;
  using System.Data;
  using System.Web;

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// All references to session should go into this class
  /// </summary>
  public class YafSession
  {
    #region Properties

    /// <summary>
    ///   Gets or sets ActiveTopicSince.
    /// </summary>
    public int? ActiveTopicSince
    {
      get
      {
        if (HttpContext.Current.Session["ActiveTopicSince"] != null)
        {
          return (int)HttpContext.Current.Session["ActiveTopicSince"];
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session["ActiveTopicSince"] = value;
      }
    }

    /// <summary>
    /// Gets or sets if the user wants to use the mobile theme.
    /// </summary>
    public bool? UseMobileTheme
    {
      get
      {
        if (HttpContext.Current.Session["UseMobileTheme"] == null)
        {
          return null;
        }

        return (bool)HttpContext.Current.Session["UseMobileTheme"];
      }

      set
      {
        HttpContext.Current.Session["UseMobileTheme"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets FavoriteTopicSince.
    /// </summary>
    public int? FavoriteTopicSince
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
    ///   Gets or sets ForumRead.
    /// </summary>
    public Hashtable ForumRead
    {
      get {
          return HttpContext.Current.Session["forumread"] != null
                     ? (Hashtable) HttpContext.Current.Session["forumread"]
                     : null;
      }

        set
      {
        HttpContext.Current.Session["forumread"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether HasLastVisit.
    /// </summary>
    public bool HasLastVisit
    {
      get
      {
          if (HttpContext.Current.Session["haslastvisit"] != null)
        {
          return (bool)HttpContext.Current.Session["haslastvisit"];
        }
          return false;
      }

        set
      {
        HttpContext.Current.Session["haslastvisit"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets LastPm.
    /// </summary>
    public DateTime LastPendingBuddies
    {
      get
      {
          if (HttpContext.Current.Session["lastpendingbuddies"] != null)
        {
          return (DateTime)HttpContext.Current.Session["lastpendingbuddies"];
        }
          return DateTime.MinValue;
      }

        set
      {
        HttpContext.Current.Session["lastpendingbuddies"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets LastPm.
    /// </summary>
    public DateTime LastPm
    {
      get
      {
          if (HttpContext.Current.Session["lastpm"] != null)
        {
          return (DateTime)HttpContext.Current.Session["lastpm"];
        }
          return DateTime.MinValue;
      }

        set
      {
        HttpContext.Current.Session["lastpm"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets LastPost.
    /// </summary>
    public DateTime LastPost
    {
      get
      {
          if (HttpContext.Current.Session["lastpost"] != null)
        {
          return (DateTime)HttpContext.Current.Session["lastpost"];
        }
          return DateTime.MinValue;
      }

        set
      {
        HttpContext.Current.Session["lastpost"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets LastVisit.
    /// </summary>
    public DateTime LastVisit
    {
      get
      {
          if (HttpContext.Current.Session["lastvisit"] != null)
        {
          return (DateTime)HttpContext.Current.Session["lastvisit"];
        }
          return DateTime.MinValue;
      }

        set
      {
        HttpContext.Current.Session["lastvisit"] = value;
      }
    }

    /// <summary>
    ///   Gets PanelState.
    /// </summary>
    [NotNull]
    public PanelSessionState PanelState
    {
      get
      {
        return new PanelSessionState();
      }
    }

    /// <summary>
    ///   Gets or sets SearchData.
    /// </summary>
    [CanBeNull]
    public DataTable SearchData
    {
      get
      {
          if (HttpContext.Current.Session["SearchDataTable"] != null)
        {
          return HttpContext.Current.Session["SearchDataTable"] as DataTable;
        }
          return null;
      }

        set
      {
        HttpContext.Current.Session["SearchDataTable"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets ShowList.
    /// </summary>
    public int ShowList
    {
      get
      {
        if (HttpContext.Current.Session["showlist"] != null)
        {
          return (int)HttpContext.Current.Session["showlist"];
        }
          // nothing in session
          return -1;
      }

      set
      {
        HttpContext.Current.Session["showlist"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets TopicRead.
    /// </summary>
    public Hashtable TopicRead
    {
      get
      {
          if (HttpContext.Current.Session["topicread"] != null)
        {
          return (Hashtable)HttpContext.Current.Session["topicread"];
        }
          return null;
      }

        set
      {
        HttpContext.Current.Session["topicread"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets UnreadTopics.
    /// </summary>
    /// </summary>
    public int UnreadTopics
    {
      get
      {
          if (HttpContext.Current.Session["unreadtopics"] != null)
        {
          return (int)HttpContext.Current.Session["unreadtopics"];
        }
          return 0;
      }

        set
      {
        HttpContext.Current.Session["unreadtopics"] = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the last time the forum was read.
    /// </summary>
    /// <param name="forumID">
    /// This is the ID of the forum you wish to get the last read date from.
    /// </param>
    /// <returns>
    /// A DateTime object of when the forum was last read.
    /// </returns>
    public DateTime GetForumRead(int forumID)
    {
      Hashtable t = this.ForumRead;
      if (t == null || !t.ContainsKey(forumID))
      {
        return this.LastVisit;
      }
        return (DateTime)t[forumID];
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
    public DateTime GetTopicRead(int topicID)
    {
      Hashtable t = this.TopicRead;
      if (t == null || !t.ContainsKey(topicID))
      {
        return this.LastVisit;
      }
        return (DateTime)t[topicID];
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
    public void SetForumRead(int forumID, DateTime date)
    {
      Hashtable t = this.ForumRead ?? new Hashtable();

      t[forumID] = date;
      this.ForumRead = t;
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
    public void SetTopicRead(int topicID, DateTime date)
    {
      Hashtable t = this.TopicRead ?? new Hashtable();

        t[topicID] = date;
      this.TopicRead = t;
    }

    #endregion
  }

  /// <summary>
  /// The panel session state.
  /// </summary>
  public class PanelSessionState
  {
    #region Enums

    /// <summary>
    /// The collapsible panel state.
    /// </summary>
    public enum CollapsiblePanelState
    {
      /// <summary>
      ///   The none.
      /// </summary>
      None = -1, 

      /// <summary>
      ///   The expanded.
      /// </summary>
      Expanded = 0, 

      /// <summary>
      ///   The collapsed.
      /// </summary>
      Collapsed = 1
    }

    #endregion

    #region Indexers

    /// <summary>
    ///   Gets panel session state.
    /// </summary>
    /// <param name = "panelID">panelID</param>
    /// <returns></returns>
    public CollapsiblePanelState this[[NotNull] string panelID]
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
          if (HttpContext.Current.Request.Cookies[sessionPanelID] != null)
          {
              try
              {
                  // we must convert string to int, better get is safe
                  if (HttpContext.Current.Request != null)
                  {
                      return (CollapsiblePanelState)int.Parse(HttpContext.Current.Request.Cookies[sessionPanelID].Value);
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
        var c = new HttpCookie(sessionPanelID, ((int)value).ToString()) {Expires = DateTime.UtcNow.AddYears(1)};
          HttpContext.Current.Response.SetCookie(c);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The toggle panel state.
    /// </summary>
    /// <param name="panelID">
    /// The panel id.
    /// </param>
    /// <param name="defaultState">
    /// The default state.
    /// </param>
    public void TogglePanelState([NotNull] string panelID, CollapsiblePanelState defaultState)
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

    #endregion
  }
}