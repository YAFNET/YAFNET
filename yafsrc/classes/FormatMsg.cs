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
		static public string ForumCodeToHtml(yaf.pages.ForumPage basePage,string Message) 
		{
			DataTable dtSmileys = GetSmilies(basePage);

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
										if(basePage.BoardSettings.BlankLinks)
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
			{
				string code = row["Code"].ToString();
				tmp = tmp.Replace(code.ToLower(),String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));
				tmp = tmp.Replace(code.ToUpper(),String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));
			}

			return tmp;
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

		static public DataTable GetSmilies(yaf.pages.ForumPage basePage) 
		{
			DataTable dt = (DataTable)System.Web.HttpContext.Current.Cache["Smilies"];
			if(dt==null) 
			{
				dt = DB.smiley_list(basePage.PageBoardID,null);
				System.Web.HttpContext.Current.Cache["Smilies"] = dt;
			}
			return dt;
		}

		static public string FetchURL(yaf.pages.ForumPage basePage,string html) 
		{
			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;
			
			DataTable dtSmileys = GetSmilies(basePage);
			foreach(DataRow row in dtSmileys.Rows) 
			{
				string code = row["Code"].ToString();
				html = html.Replace(code.ToLower(),String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));
				html = html.Replace(code.ToUpper(),String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));
			}

			//Email -- RegEx VS.NET
			html = Regex.Replace(html, @"(?<email>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", "<a href=mailto:${email}>${email}</a>", options);

			//URL (http://) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
 			html = Regex.Replace(html, "(?<!href=\")(?<!src=\")(?<url>http://(?:[\\w-]+\\.)+[\\w-]+(?:/[\\w-./?%&=;,]*)?)", "<a href=${url} target=_blank>${url}</a>", options);

			//URL (www) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
 			html = Regex.Replace(html, @"(?<!http://)(?<url>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=;,]*)?)", "<a href=http://${url} target=_blank>${url}</a>", options);

			options |= RegexOptions.Singleline;
			while(Regex.IsMatch(html,@"\[quote\](.*?)\[/quote\]",options)) 
				html = Regex.Replace(html,@"\[quote\](.*?)\[/quote\]","<div class='quote'><b>QUOTE</b><div class='quoteinner'>$1</div></div>",options);
			while(Regex.IsMatch(html,@"\[quote=(.*?)\](.*?)\[/quote\]",options)) 
				html = Regex.Replace(html,@"\[quote=(.*?)\](.*?)\[/quote\]","<div class='quote'><b>QUOTE</b> ($1)<div class='quoteinner'>$2</div></div>",options);

			// rico : run word replacement from databse table names yaf_replacewords
			using(DataTable dt = DB.replace_words_list())
				foreach(DataRow rwords in dt.Rows)  
				{
					html = Regex.Replace(html,Convert.ToString(rwords["badword"]),Convert.ToString(rwords["goodword"]),options);
				}

			return RepairHtml(basePage,html);
		}

		static private bool IsValidTag(string tag) 
		{
			if(tag.IndexOf("javascript")>=0)
				return false;

			if(tag.IndexOf("vbscript")>=0)
				return false;

			if(tag.IndexOf("onclick")>=0)
				return false;

			char[] endchars = new char[]{' ','>','/','\t'};
			int pos = tag.IndexOfAny(endchars,1);
			if(pos>0) tag = tag.Substring(0,pos);

			if(tag[0]=='/') tag = tag.Substring(1);
			switch(tag) 
			{
				case "br":
				case "hr":
				case "b":
				case "i":
				case "u":
				case "a":
				case "div":
				case "ol":
				case "ul":
				case "li":
				case "blockquote":
				case "img":
				case "span":
				case "p":
				case "em":
				case "strong":
				case "font":
				case "pre":
				case "h1":
				case "h2":
				case "h3":
				case "h4":
				case "h5":
				case "h6":
				case "address":
					return true;
			}
			return false;
		}

		static public string RepairHtml(yaf.pages.ForumPage basePage,string html) 
		{
			RegexOptions options = RegexOptions.IgnoreCase;

			MatchCollection m = Regex.Matches(html,"<.*?>",options);
			for(int i=m.Count-1;i>=0;i--) 
			{
				string tag = html.Substring(m[i].Index+1,m[i].Length-1).Trim().ToLower();
				if(!IsValidTag(tag)) 
				{
					string tmp = System.Web.HttpContext.Current.Server.HtmlEncode(html.Substring(m[i].Index,m[i].Length));
					html = html.Remove(m[i].Index,m[i].Length);
					html = html.Insert(m[i].Index,tmp);
				}
			}
			return html;
		}
	}
}
