/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Controls.Statistics
{
	#region Using

	using System;
	using System.Data;
	using System.Text;
	using System.Web.UI;

	using YAF.Core;
	using YAF.Types.Interfaces;
	using YAF.Types.Constants;
	using YAF.Utils;
	using YAF.Types;

	#endregion

	/// <summary>
	/// The most active users.
	/// </summary>
	[ToolboxData("<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>")]
	public class MostActiveUsers : BaseControl
	{
		#region Constants and Fields

		/// <summary>
		///   The _display number.
		/// </summary>
		private int _displayNumber = 10;

		/// <summary>
		///   The _last num of days.
		/// </summary>
		private int _lastNumOfDays = 7;

		#endregion

		#region Properties

		/// <summary>
		///   Gets or sets DisplayNumber.
		/// </summary>
		public int DisplayNumber
		{
			get
			{
				return this._displayNumber;
			}

			set
			{
				this._displayNumber = value;
			}
		}

		/// <summary>
		///   Gets or sets LastNumOfDays.
		/// </summary>
		public int LastNumOfDays
		{
			get
			{
				return this._lastNumOfDays;
			}

			set
			{
				this._lastNumOfDays = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Renders the MostActiveUsers class.
		/// </summary>
		/// <param name="writer">
		/// </param>
		protected override void Render([NotNull] HtmlTextWriter writer)
		{
			int currentRank = 1;
			string actRank = string.Empty;

			DataTable rankDt = this.Get<IDataCache>().GetOrSet<DataTable>(
				Constants.Cache.MostActiveUsers,
				() =>
				this.Get<IDbFunction>().GetData.user_activity_rank(
					this.PageContext.PageBoardID, DateTime.UtcNow.AddDays(-this.LastNumOfDays), this.DisplayNumber),
				TimeSpan.FromMinutes(5));

			//// create XML data document...
			// XmlDocument xml = new XmlDocument();

			// rankDt.TableName = "UserActivityRank";
			// xml.LoadXml( rankDt.DataSet.GetXml() );

			//// transform using the MostActiveUser xslt...
			// const string xsltFile = "YAF.Controls.Statistics.MostActiveUser.xslt";

			// using ( Stream resourceStream = Assembly.GetAssembly( this.GetType() ).GetManifestResourceStream( xsltFile ) )
			// {
			// if ( resourceStream != null )
			// {
			// XslCompiledTransform myXslTrans = new XslCompiledTransform();

			// //load the Xsl 
			// myXslTrans.Load( XmlReader.Create( resourceStream ) );
			// myXslTrans.Transform( xml.CreateNavigator(), xslArgs, writer );
			// }
			// }
			writer.BeginRender();

			var html = new StringBuilder();

			html.AppendFormat(@"<div id=""{0}"" class=""yaf_activeuser"">", this.ClientID);
			html.AppendFormat(@"<h2 class=""yaf_header"">{0}</h2>", "Most Active Users");
			html.AppendFormat(@"<h4 class=""yaf_subheader"">Last {0} Days</h4>", this.LastNumOfDays);

			html.AppendLine("<ol>");

			// flush...
			writer.Write(html.ToString());

			foreach (DataRow row in rankDt.Rows)
			{
				writer.WriteLine("<li>");

				// render UserLink...
				var userLink = new UserLink { UserID = row.Field<int>("ID"), };
				userLink.RenderControl(writer);

				// render online image...
				var onlineStatusImage = new OnlineStatusImage { UserID = row.Field<int>("ID") };
				onlineStatusImage.RenderControl(writer);

				writer.WriteLine(" ");
				writer.WriteLine(@"<span class=""NumberOfPosts"">({0})</span>".FormatWith(row.Field<int>("NumOfPosts")));
				writer.WriteLine("</li>");
			}

			writer.WriteLine("</ol>");
			writer.EndRender();
		}

		#endregion
	}
}