/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj?rnar Henden
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

namespace YAF.Controls
{
	using System;
	using System.Data;
	using System.Web.UI.WebControls;
	using YAF.Classes;
	using YAF.Classes.Core;
	using YAF.Classes.Data;
	using YAF.Classes.Utils;

	/// <summary>
	/// Summary description for buddies.
	/// </summary>
	public partial class MyTopicsList : BaseUserControl
	{
		/* Data Fields */
		#region Control Data

		/// <summary>
		/// The _last forum name.
		/// </summary>
		private string _lastForumName = string.Empty;

		/// <summary>
		/// the control mode.
		/// </summary>
		private int _controlMode;
		
		#endregion


		/* Properties */
		#region Control Mode

		/// <summary>
		/// Determines what is th current mode of the control.
		/// </summary>
		public int Mode
		{
			get
			{
				return this._controlMode;
			}
			set
			{
				this._controlMode = value;
			}

		}
		
		#endregion		

		
		/* Event Handlers */
		#region Page Events
		
		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			// RSS link setup
			if (PageContext.BoardSettings.ShowRSSLink)
			{
				if (this.Mode == 1)
				{
					// for active topics list
					this.RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=active");
                    this.RssIcon.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=active");
                    this.RssIcon.ToolTip = PageContext.Localization.GetText("RSSICONTOOLTIPACTIVE");
                }
				else
				{
					// for favorites list
					this.RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=favorite");
                    this.RssIcon.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=favorite");
                    this.RssIcon.ToolTip = PageContext.Localization.GetText("RSSICONTOOLTIPFAVORITE");

                }
				this.RssFeed.Text = PageContext.Localization.GetText("RSSFEED");
				this.RssFeed.Visible = true;
                this.RssIcon.Visible = true;
			}
			else
			{
				// hide RSS link if board has them turned off
				this.RssFeed.Visible = false;
                this.RssIcon.Visible = false;
			}

			if (!IsPostBack)
			{
				InitSinceDropdown();

				int? previousSince = null;

				if (this.Mode == 1)
				{
					// active topics mode
					previousSince = Mession.ActiveTopicSince;

					// by default select "Last visited..." for active discussions
					this.Since.SelectedIndex = 0;
				}
				else
				{
					// favorites mode
					previousSince = Mession.FavoriteTopicSince;

					// add "Show All" option
					this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("show_all"), "9999"));

					// by default select "Show All" for favorites topics
					this.Since.SelectedIndex = this.Since.Items.Count - 1;
				}

				// has user selected any "since" value before we can remember now?
				if (previousSince.HasValue)
				{
					// look for value previously selected
					ListItem sinceItem = this.Since.Items.FindByValue(previousSince.Value.ToString());
					// and select it if found
					if (sinceItem != null) this.Since.SelectedIndex = this.Since.Items.IndexOf(sinceItem);
				}
			}

			BindData();
		}

		#endregion

		#region Control Events

		/// <summary>
		/// The since_ selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Since_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Set the controls' pager index to 0.
			PagerTop.CurrentPageIndex = 0;

			// save since option to rememver it next time
			if (Mode == 1)
			{
				// for active topics
				Mession.ActiveTopicSince = Convert.ToInt32(this.Since.SelectedValue);
			}
			else
			{
				// for favorites
				Mession.FavoriteTopicSince = Convert.ToInt32(this.Since.SelectedValue);
			}

			// re-bind data
			BindData();
		}

		/// <summary>
		/// The pager_ page change.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Pager_PageChange(object sender, EventArgs e)
		{
			BindData();
		}

		#endregion


		/* Methods */
		#region Data Binding

		/// <summary>
		/// The bind data.
		/// </summary>
		public void BindData()
		{
			// default since date is now
			DateTime sinceDate = DateTime.Now;
			// default since option is "since last visit"
			int sinceValue = 0;

			// is any "since"option selected
			if (this.Since.SelectedItem != null)
			{
				// get selected value
				sinceValue = int.Parse(this.Since.SelectedItem.Value);
				//sinceDate = DateTime.Now;	// no need to do it again (look above)

				// decrypt selected option
				if (sinceValue == 9999)		// all
				{
					// get all, from the beginning
					sinceDate = (DateTime)(System.Data.SqlTypes.SqlDateTime.MinValue);
				}
				else if (sinceValue > 0)	// days
				{
					// get posts newer then defined number of days
					sinceDate = DateTime.Now - TimeSpan.FromDays(sinceValue);
				}
				else if (sinceValue < 0)	// hours
				{
					// get posts newer then defined number of hours
					sinceDate = DateTime.Now + TimeSpan.FromHours(sinceValue);
				}
			}

			// we want to filter topics since last visit
			if (sinceValue == 0) sinceDate = Mession.LastVisit;

			// we want to page results
			var pds = new PagedDataSource();
			pds.AllowPaging = true;

			// filter by category
			object categoryIDObject = null;
			// is category set?
			if (PageContext.Settings.CategoryID != 0)
			{
				categoryIDObject = PageContext.Settings.CategoryID;
			}

			// we'll hold topics in this table
			DataTable topicList;
			// now depending on mode fill the table
			if (this.Mode == 1)
			{
				// we are getting active topics
				topicList = DB.topic_active(
					PageContext.PageBoardID,
					PageContext.PageUserID,
					sinceDate,
					categoryIDObject,
					PageContext.BoardSettings.UseStyledNicks
					);
			}
			else
			{
				// we are getting favotes
				topicList = YafServices.FavoriteTopic.FavoriteTopicDetails(sinceDate);
			}

			// styled nicks
			if (PageContext.BoardSettings.UseStyledNicks)
			{
				new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref topicList, true, "LastUserStyle", "StarterStyle");
			}

			// let's page the results
			DataView dv = topicList.DefaultView;
			pds.DataSource = dv;
			this.PagerTop.Count = dv.Count;
			/// TODO : page size definable?
			this.PagerTop.PageSize = 15;
			pds.PageSize = this.PagerTop.PageSize;
			pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;

			// set datasource of repeater
			this.TopicList.DataSource = pds;

			// data bind controls
			DataBind();
		}

		/// <summary>
		/// Initializes dropdown with options to filter results by date.
		/// </summary>
		protected void InitSinceDropdown()
		{
			// value 0, for since last visted
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetTextFormatted("last_visit", YafServices.DateTime.FormatDateTime(Mession.LastVisit)), "0"));
			// negative values for hours backward
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_hour"), "-1"));
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_two_hours"), "-2"));
			// positive values for days backward
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_day"), "1"));
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_two_days"), "2"));
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_week"), "7"));
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_two_weeks"), "14"));
			this.Since.Items.Add(new ListItem(PageContext.Localization.GetText("last_month"), "31"));
		}

		#endregion

		#region Formatting

		/// <summary>
		/// The print forum name.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <returns>
		/// The print forum name.
		/// </returns>
		protected string PrintForumName(DataRowView row)
		{
			var forumName = (string)row["ForumName"];
			string html = string.Empty;
			if (forumName != this._lastForumName)
			{
				html = String.Format(
				  @"<tr><td class=""header2"" colspan=""6""><a href=""{1}"">{0}</a></td></tr>",
				  forumName,
				  YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]));
				this._lastForumName = forumName;
			}

			return html;
		}

		#endregion

	}
}
