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
 */
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls.Statistics
{
	[ToolboxData( "<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>" )]
	public class MostActiveUsers : BaseControl
	{
		private int _displayNumber = 10;
		private int _lastNumOfDays = 7;

		/// <summary>
		/// The default constructor for MostActiveUsers.
		/// </summary>
		public MostActiveUsers()
		{
		}

		/// <summary>
		/// Renders the MostActiveUsers class.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			int currentRank = 1;
			string actRank = "";
			string cacheKey = YafCache.GetBoardCacheKey( Constants.Cache.MostActiveUsers );

			DataTable rankDt = PageContext.Cache.GetItem( cacheKey, 5,
			                                              () =>
			                                              DB.user_activity_rank( PageContext.PageBoardID,
			                                                                     DateTime.Now.AddDays( -LastNumOfDays ),
			                                                                     DisplayNumber ) );

			//// create XML data document...
			//XmlDocument xml = new XmlDocument();

			//rankDt.TableName = "UserActivityRank";
			//xml.LoadXml( rankDt.DataSet.GetXml() );

			//// transform using the MostActiveUser xslt...
			//const string xsltFile = "YAF.Controls.Statistics.MostActiveUser.xslt";

			//using ( Stream resourceStream = Assembly.GetAssembly( this.GetType() ).GetManifestResourceStream( xsltFile ) )
			//{
			//  if ( resourceStream != null )
			//  {
			//    XslCompiledTransform myXslTrans = new XslCompiledTransform();

			//    //load the Xsl 
			//    myXslTrans.Load( XmlReader.Create( resourceStream ) );
			//    myXslTrans.Transform( xml.CreateNavigator(), xslArgs, writer );
			//  }
			//}

			writer.BeginRender();

			StringBuilder html = new StringBuilder();

			html.AppendFormat( @"<div id=""{0}"" class=""yaf_activeuser"">", this.ClientID );
			html.AppendFormat( @"<h2 class=""yaf_header"">{0}</h2>", "Most Active Users" );
			html.AppendFormat( @"<h4 class=""yaf_subheader"">Last {0} Days</h4>", LastNumOfDays );

			html.AppendLine( "<ol>" );

			// flush...
			writer.Write( html.ToString() );

			foreach ( DataRow row in rankDt.Rows )
			{
				writer.WriteLine( "<li>" );

				// render UserLink...
				var userLink = new UserLink() { UserID = row.Field<int>( "ID" ), UserName = row.Field<string>( "Name" ) };
				userLink.RenderControl( writer );

				// render online image...
				var onlineStatusImage = new OnlineStatusImage() { UserID = row.Field<int>( "ID" ) };
				onlineStatusImage.RenderControl( writer );

				writer.WriteLine( " " );
				writer.WriteLine( String.Format( @"<span class=""NumberOfPosts"">({0})</span>", row.Field<int>( "NumOfPosts" ) ) );
				writer.WriteLine( "</li>" );
			}

			writer.WriteLine( "</ol>" );
			writer.EndRender();
		}

		public int DisplayNumber
		{
			get { return _displayNumber; }
			set { _displayNumber = value; }
		}

		public int LastNumOfDays
		{
			get { return _lastNumOfDays; }
			set { _lastNumOfDays = value; }
		}
	}
}
