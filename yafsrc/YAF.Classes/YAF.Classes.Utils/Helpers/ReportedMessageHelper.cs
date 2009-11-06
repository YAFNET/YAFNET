using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace YAF.Classes.Utils
{
	public static class ReportedMessageHelper
	{
		/// <summary>
		/// TODO: Include Discription or more comments
		/// </summary>
		/// <param name="flags"></param>
		/// <param name="forumid"></param>
		/// <returns></returns>
		public static DataTable GetReportersList( int flags, int forumid )
		{
			System.Data.DataTable dt = YAF.Classes.Data.DB.message_listreported( flags, forumid );
			dt.Columns.Add( "Reporters", typeof( string ) );
			dt.AcceptChanges();
			int i = 0;
			foreach ( DataRow dr in dt.Rows )
			{
				DataTable _reportersList = YAF.Classes.Data.DB.message_listreporters( Convert.ToInt32( dr["MessageID"] ) );
				if ( _reportersList.Rows.Count > 0 )
				{
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					foreach ( DataRow reporter in _reportersList.Rows )
					{
						string howMany = null;
						if ( Convert.ToInt32( reporter["ReportedNumber"] ) > 1 )
							howMany = "(" + reporter["ReportedNumber"].ToString() + ")";

						// ?? Bad place for UI. Please put UI in controls or pages. Not in here.
						sb.AppendFormat( @"<a id=""Link{1}{0}"" href=""{3}"" runat=""server"">{2}{4}</a>, ", i, Convert.ToInt32( reporter["UserID"] ), reporter["UserName"].ToString(), YafBuildLink.GetLink( ForumPages.profile, "u={0}", Convert.ToInt32( reporter["UserID"] ) ), howMany );

						i++;
					}
					dr["Reporters"] = sb.ToString().TrimEnd().TrimEnd( ',' );
				}

			}
			return dt;
		}
	}
}
