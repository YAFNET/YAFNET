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
using System.Text.RegularExpressions;

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

		public string FormatMessage(string Message) 
		{
			if(dtSmileys == null)
				dtSmileys = DB.smiley_list(null);

			string tmp = "";
			bool bInCode = false;
			for(int i=0;i<Message.Length;i++) 
			{
				if(Message[i]=='[') 
				{
					int e1 = Message.IndexOf(']',i);
					int e2 = Message.IndexOf('=',i);
					if(e1>0) 
					{
						bool bNone = false;
						string cmd, arg = null;
						if(e2<0 || e2>e1) 
						{
							cmd = Message.Substring(i+1,e1-i-1);
							arg = null;
						} 
						else 
						{
							cmd = Message.Substring(i+1,e2-i-1);
							arg = Message.Substring(e2+1,e1-e2-1);

							arg = arg.Trim();
							arg = basePage.Server.HtmlDecode(arg);
							if(arg.Length>2 && arg[0]=='"' && arg[arg.Length-1]=='"')
								arg = arg.Substring(1,arg.Length-2);
						}

						cmd = cmd.ToLower();
						if(!bInCode || cmd=="/code") 
						{
							switch(cmd) 
							{
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
									{
										if(basePage.UseBlankLinks)
											tmp += String.Format("<a target=\"_blank\" href=\"{0}\">",arg);
										else
											tmp += String.Format("<a target=\"_top\" href=\"{0}\">",arg);
									}
									else
										tmp += "<a>";
									break;
								case "/url":
									tmp += "</a>";
									break;
								case "img":
									tmp += "<img src=\"";
									break;
								case "/img":
									tmp += "\"/>";
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
								case "code":
									tmp += "<pre>";
									bInCode = true;
									break;
								case "/code":
									tmp += "</pre>";
									bInCode = false;
									break;
								default:
									bNone = true;
									break;
							}
						}
						else 
						{
							bNone = true;
						}
						if(!bNone) 
						{
							i = e1;
							continue;
						}
					}
				}
				tmp += Message[i];
			}

			tmp = tmp.Replace("\n","<br />");
			tmp = tmp.Replace("\r","");

			foreach(DataRow row in dtSmileys.Rows)
				tmp = tmp.Replace((string)row["Code"],String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));

			return tmp;
		}
	
		static public string ForumCodeToHtml(BasePage basePage,string message) 
		{
			FormatMsg fmt = new FormatMsg(basePage);
			return fmt.FormatMessage(message);
		}

		static public string HtmlToForumCode(string html) 
		{
			html = html.Replace("<ul>","[list]");	// TODO
			html = html.Replace("</ul>","[/list]");	// TODO
			html = html.Replace("<ol>","[list]");	// TODO
			html = html.Replace("</ol>","[/list]");	// TODO
			html = html.Replace("<li>","[*]");		// TODO
			html = html.Replace("</li>","");		// TODO
			
			RegexOptions options = RegexOptions.IgnoreCase;
			html = Regex.Replace(html,"<a href=\"(.*)\">(.*)</a>","[url=\"$1\"]$2[/url]",options);
			html = Regex.Replace(html,"<img src=\"(.*)\">","[img]$1[/img]",options);
			html = Regex.Replace(html,"<p(.*?)>","",options);
			html = Regex.Replace(html,"</p>","<br><br>",options);
			html = Regex.Replace(html,"<br>","\n",options);
			html = Regex.Replace(html,"<b>","[b]",options);
			html = Regex.Replace(html,"</b>","[/b]",options);
			html = Regex.Replace(html,"<strong>","[b]",options);
			html = Regex.Replace(html,"</strong>","[/b]",options);
			html = Regex.Replace(html,"<i>","[i]",options);
			html = Regex.Replace(html,"</i>","[/i]",options);
			html = Regex.Replace(html,"<em>","[i]",options);
			html = Regex.Replace(html,"</em>","[/i]",options);
			html = Regex.Replace(html,"<u>","[u]",options);
			html = Regex.Replace(html,"</u>","[/u]",options);
			html = Regex.Replace(html,"<blockquote(.*)>","[block]",options);			
			html = Regex.Replace(html,"</blockquote>","[/block]",options);			

			html = Regex.Replace(html,"<","&lt;",options);
			html = Regex.Replace(html,">","&gt;",options);
//			if(html.IndexOf('<')>=0 || html.IndexOf('>')>=0)
//				html += "\n\nINVALID";

			return html;
		}

		static public string FetchURL(string html) 
		{
			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;
			
			//Email -- RegEx VS.NET
			html = Regex.Replace(html, @"(?<email>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", "<a href=mailto:${email}>${email}</a>", options);

			//URL (http://) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
//			html = Regex.Replace(html, @"(?<url>http://(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=]*)?)", "<a href=${url} target=_blank>${url}</a>", options);
			html = Regex.Replace(html, "(?<!href=\")(?<url>http://(?:[\\w-]+\\.)+[\\w-]+(?:/[\\w-./?%&=]*)?)", "<a href=${url} target=_blank>${url}</a>", options);

			//URL (www) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
			html = Regex.Replace(html, @"(?<!http://)(?<url>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=]*)?)", "<a href=http://${url} target=_blank>${url}</a>", options);

			return RepairHtml(html);
		}

		static public string RepairHtml(string html) 
		{
			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

			html = Regex.Replace(html,"<table(.*?)>","&lt;table$1&gt;",options);
			html = Regex.Replace(html,"</table>","&lt;/table&gt;",options);
			html = Regex.Replace(html,"<tr(.*?)>","&lt;tr$1&gt;",options);
			html = Regex.Replace(html,"</tr>","&lt;/tr&gt;",options);
			html = Regex.Replace(html,"<td(.*?)>","&lt;td$1&gt;",options);
			html = Regex.Replace(html,"</td>","&lt;/td&gt;",options);
			html = Regex.Replace(html,"<script(.*?)>","&lt;td$1&gt;",options);
			html = Regex.Replace(html,"</script>","&lt;/td&gt;",options);
			html = Regex.Replace(html,"<%","&lt;%",options);
			html = Regex.Replace(html,"%>","%&gt;",options);

			MatchCollection m = Regex.Matches(html,"<.*?>",options);
			for(int i=0;i<m.Count;i++) 
			{
				if(!IsValidTag(m[i].Value))
					throw new Exception(String.Format("You have entered some illegal html: {0}",m[i].ToString()));
			}

			return html;
		}

		static public bool IsValidHtml(string html) 
		{
			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

			MatchCollection m = Regex.Matches(html,"<.*?>",options);
			for(int i=0;i<m.Count;i++) 
			{
				if(!IsValidTag(m[i].Value))
					throw new Exception(String.Format("You have entered some illegal html: {0}",m[i].ToString()));
			}
			return true;
		}

		static private bool IsValidTag(string tag) 
		{
			tag = tag.ToLower();
			switch(tag) 
			{
				case "<br>":
				case "<br/>":
				case "</a>":
				case "<em>":
				case "</em>":
				case "<b>":
				case "</b>":
				case "<strong>":
				case "</strong>":
				case "<u>":
				case "</u>":
				case "<i>":
				case "</i>":
				case "<blockquote>":
				case "</blockquote>":
				case "</div>":
				case "<p>":
				case "</p>":
				case "</font>":
				case "<ul>":
				case "</ul>":
				case "<ol>":
				case "</ol>":
				case "<li>":
				case "</li>":
				case "<tbody>":
				case "</tbody>":
				case "<pre>":
				case "</pre>":
					return true;
			}
			if(tag.StartsWith("<a "))
				return true;
			if(tag.StartsWith("<br "))
				return true;
			if(tag.StartsWith("<hr "))
				return true;
			if(tag.StartsWith("<img "))
				return true;
			if(tag.StartsWith("<div "))
				return true;
			if(tag.StartsWith("<font "))
				return true;

			return false;
		}
	}
}
