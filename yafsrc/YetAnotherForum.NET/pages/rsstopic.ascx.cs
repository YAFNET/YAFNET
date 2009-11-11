/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Data;
using System.Text;
using System.Web;
using System.Xml;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Classes.Utils;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for rss.
	/// </summary>
	public partial class rsstopic : ForumPage
	{
		public rsstopic()
			: base("RSSTOPIC")
		{
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Put user code to initialize the page here
			RssFeed rf = new RssFeed();

			XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);

			writer.Formatting = Formatting.Indented;
			rf.WriteRSSPrologue(writer);

			// Usage rf.AddRSSItem(writer, "Item Title", "http://test.com", "This is a test item");

			switch (Request.QueryString["pg"])
			{
				case "latestposts":
					if (!PageContext.ForumReadAccess)
						YafBuildLink.AccessDenied();
					using (DataTable dt = DB.topic_latest(PageContext.PageBoardID, 7, PageContext.PageUserID))
					{
						foreach (DataRow row in dt.Rows)
							rf.AddRSSItem(writer,
							              YafServices.BadWordReplace.Replace(row["Subject"].ToString()),
							              YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", Request.QueryString["t"]),
							              YafServices.BadWordReplace.Replace(row["Message"].ToString()),
							              Convert.ToDateTime(row["Posted"]).ToString("r"));
					}
					break;
				case "latestannouncements":
					if (!PageContext.ForumReadAccess)
						YafBuildLink.AccessDenied();
					using (DataTable dt = DB.topic_announcements(PageContext.PageBoardID, 7, PageContext.PageUserID))
					{
						foreach (DataRow row in dt.Rows)
							rf.AddRSSItem(writer,
							              YafServices.BadWordReplace.Replace(row["Subject"].ToString()),
							              YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", Request.QueryString["t"]),
							              YafServices.BadWordReplace.Replace(row["Message"].ToString()),
							              Convert.ToDateTime(row["Posted"]).ToString("r"));
					}
					break;
				case "posts":
					if (!PageContext.ForumReadAccess)
						YafBuildLink.AccessDenied();

					if (Request.QueryString["t"] != null)
					{
						using (
							DataTable dt =
								DB.post_list( PageContext.PageTopicID, 1, PageContext.BoardSettings.ShowDeletedMessages, YafContext.Current.BoardSettings.UseStyledNicks, YafContext.Current.BoardSettings.ShowThanksDate ) )
						{
							foreach (DataRow row in dt.Rows)
								rf.AddRSSItem(writer,
								              YafServices.BadWordReplace.Replace(row["Subject"].ToString()),
								              YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", Request.QueryString["t"]),
								              FormatMsg.FormatMessage(row["Message"].ToString(), new MessageFlags(row["Flags"])),
								              Convert.ToDateTime(row["Posted"]).ToString("r"));
						}
					}

					break;
				case "forum":
					using (
						DataTable dt = DB.forum_listread(PageContext.PageBoardID, PageContext.PageUserID, null, null))
					{
						foreach (DataRow row in dt.Rows)
						{
							rf.AddRSSItem(writer,
							              YafServices.BadWordReplace.Replace(row["Forum"].ToString()),
							              YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true, "f={0}", row["ForumID"]),
							              YafServices.BadWordReplace.Replace(row["Description"].ToString()));
						}
					}
					break;
				case "topics":
					if (!PageContext.ForumReadAccess)
						YafBuildLink.AccessDenied();

					int forumId;
					if (Request.QueryString["f"] != null &&
					    int.TryParse(Request.QueryString["f"], out forumId))
					{
                        //vzrus changed to separate DLL specific code
                        using (DataTable dt = DB.rsstopic_list(forumId))
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                rf.AddRSSItem(writer,
                                              YafServices.BadWordReplace.Replace(row["Topic"].ToString()),
                                              YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["TopicID"]),
                                              YafServices.BadWordReplace.Replace(row["Topic"].ToString()),
                                              Convert.ToDateTime(row["Posted"]).ToString("r"));
                            }
                        }
				
					}

					break;
				case "active":
using (
	DataTable dt =
		DB.topic_active(PageContext.PageBoardID, PageContext.PageUserID,
		DateTime.Now + TimeSpan.FromHours( -24 ), ( PageContext.Settings.CategoryID == 0 ) ? null : (object)PageContext.Settings.CategoryID ) )
{
	foreach (DataRow row in dt.Rows)
	{
		rf.AddRSSItem(writer,
		              YafServices.BadWordReplace.Replace(row["Subject"].ToString()),
		              YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
		              YafServices.BadWordReplace.Replace(row["Subject"].ToString()));
	}
}
break;
				default:
					break;
			}

			rf.WriteRSSClosing(writer);
			writer.Flush();

			writer.Close();

			Response.ContentEncoding = Encoding.UTF8;
			Response.ContentType = "text/xml";
			Response.Cache.SetCacheability(HttpCacheability.Public);

			Response.End();
		}

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion
	}
}
