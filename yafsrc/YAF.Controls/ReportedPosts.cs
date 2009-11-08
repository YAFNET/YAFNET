/* Yet Another Forum.NET
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
 * Written by vzrus (c) 2009 for Yet Another Forum.NET  */
using System;
using System.Web.UI;
using YAF.Classes.Data;
using YAF.Classes.UI;
using System.Data;

namespace YAF.Controls
{
	/// <summary>
	/// Shows a Reporters for reported posts
	/// </summary>
	public class ReportedPosts : BaseControl
	{
		public ReportedPosts()
			: base()
		{

		}
		public int MessageID
		{
			get
			{
				if ( ViewState["MessageID"] != null )
				{
					return Convert.ToInt32( ViewState["MessageID"] );
				}

				return 0;
			}
			set
			{
				ViewState["MessageID"] = value;
			}
		}

		protected override void Render( HtmlTextWriter writer )
		{
			// TODO: Needs better commentting.

			writer.WriteLine( String.Format( @"<div id=""{0}"" class=""yafReportedPosts"">", this.ClientID ) );

			DataTable reportersList = YAF.Classes.Data.DB.message_listreporters( MessageID );
			if ( reportersList.Rows.Count > 0 )
			{
				int i = 0;
				writer.BeginRender();

				foreach ( DataRow reporter in reportersList.Rows )
				{
					string howMany = null;
					if ( Convert.ToInt32( reporter["ReportedNumber"] ) > 1 )
						howMany = "(" + reporter["ReportedNumber"].ToString() + ")";

					writer.Write( @"<a class=""YafReported_Link"" href=""{3}"">{5}{2}{4}</a>", i, Convert.ToInt32( reporter["UserID"] ), reporter["UserName"].ToString(), YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.ForumPages.profile, "u={0}", Convert.ToInt32( reporter["UserID"] ) ), howMany, PageContext.Localization.GetText( "REPORTEDBY" ) );
					
					string[] reportString = reporter["ReportText"].ToString().Trim().Split( '|' );

					for ( int istr = 0; istr < reportString.Length; istr++ )
					{
						string[] textString = reportString[istr].Split( "??".ToCharArray() );

						if ( textString.Length > 2 )
						{
							// TODO: Remove hard-coded formatting.
							// TODO: Move DateTime Conversion to DateTime Services.
							writer.Write( @"<br></br><strong>{0}:</strong><br></br>", Convert.ToDateTime( textString[0].TrimEnd( ':' ) ).AddMinutes( (double)PageContext.CurrentUserData.TimeZone ) );
							writer.Write( @"<p>{0}</p>", textString[2] );
						}
						else
						{
							writer.Write( @"<p>{0}</p>", reportString[istr] );
						}

						writer.Write( @"<a class=""YafReported_Link"" href=""{3}"">{4} {2}</a>", i, Convert.ToInt32( reporter["UserID"] ), reporter["UserName"].ToString(), YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.ForumPages.cp_pm ), PageContext.Localization.GetText( "REPLYTO" ) );
					}

					// TODO: Remove hard-coded formatting.
					if ( i < reportersList.Rows.Count - 1 ) writer.Write( "<br></br>" );
					else writer.Write( @"<br></br><p>@@@</p>" );
					i++;
				}

				// render controls...
				base.Render( writer );

				writer.WriteLine( "</div>" );
				writer.EndRender();
			}
		}
	}
}