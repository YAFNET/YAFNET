/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Data.SqlClient;

namespace yaf
{
	/// <summary>
	/// Summary description for FormatMsg.
	/// </summary>
	public class FormatMsg
	{
		private DataTable dtSmileys;
		private BasePage basePage;

		public FormatMsg(BasePage bp) {
			basePage = bp;
		}

		public string FormatMessage(System.Web.UI.Page page,string Message) {
			if(dtSmileys == null)
				dtSmileys = DataManager.GetData("yaf_smiley_list",CommandType.StoredProcedure);

			string tmp = "";
			for(int i=0;i<Message.Length;i++) {
				if(Message[i]=='[') {
					int e1 = Message.IndexOf(']',i);
					int e2 = Message.IndexOf('=',i);
					if(e1>0) {
						bool bNone = false;
						string cmd, arg = null;
						if(e2<0 || e2>e1) {
							cmd = Message.Substring(i+1,e1-i-1);
							arg = null;
						} else {
							cmd = Message.Substring(i+1,e2-i-1);
							arg = Message.Substring(e2+1,e1-e2-1);

							arg = arg.Trim();
							arg = page.Server.HtmlDecode(arg);
							if(arg.Length>2 && arg[0]=='"' && arg[arg.Length-1]=='"')
								arg = arg.Substring(1,arg.Length-2);
						}

						cmd = cmd.ToLower();
						switch(cmd) {
							case "b":
								tmp += "<b>";
								break;
							case "/b":
								tmp += "</b>";
								break;
							case "i":
								tmp += "<em>";
								break;
							case "/i":
								tmp += "</em>";
								break;
							case "u":
								tmp += "<u>";
								break;
							case "/u":
								tmp += "</u>";
								break;
							case "url":
								if(arg!=null)
									tmp += String.Format("<a href=\"{0}\">",arg);
								else
									tmp += "<a>";
								break;
							case "/url":
								tmp += "</a>";
								break;
							case "quote":
								if(arg!=null)
									tmp += String.Format("<div class=quote>{0} wrote:<div class=\"quoteinner\">",arg);
								else
									tmp += "<div class=quote><div class=\"quoteinner\">";
								break;
							case "/quote":
								tmp += "</div></div>";
								break;
							case "color":
								if(arg!=null)
									tmp += String.Format("<span style=\"color:{0}\">",arg);
								else
									tmp += "<span>";
								break;
							case "/color":
								tmp += "</span>";
								break;
							default:
								bNone = true;
								break;
						}
						if(!bNone) {
							i = e1;
							continue;
						}
					}
				}
				tmp += Message[i];
			}

			tmp = tmp.Replace("\r\n","<br />");

			for(int i=0;i<dtSmileys.Rows.Count;i++) {
				DataRow row = dtSmileys.Rows[i];
				tmp = tmp.Replace((string)row["Code"],String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));
			}

			return tmp;
		}
	}
}
