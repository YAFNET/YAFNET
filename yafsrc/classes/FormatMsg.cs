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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace yaf
{
	/// <summary>
	/// Summary description for FormatMsg.
	/// </summary>
	public class FormatMsg
	{
		/// <summary>
		/// Formats a message to HTML by:
		/// 1. Converting "Forum Code" to HTML
		/// 2. Converting carriage returns to &lt;br/&gt;
		/// 3. Converting smiles code to img tags 
		/// </summary>
		/// <param name="basePage">Forum Page</param>
		/// <param name="Message">Message to Format</param>
		/// <returns>Formatted Message</returns>
		static public string ForumCodeToHtml(yaf.pages.ForumPage basePage,string Message) 
		{
#if true
			return Message;
#else
			string strReturn = iConvertForumCode(basePage,Message);

			strReturn = strReturn.Replace("\n","<br />");
			strReturn = strReturn.Replace("\r","");

			strReturn = iAddSmiles(basePage,strReturn);

			return strReturn;
#endif
		}

		/// <summary>
		/// Formats message by converting "Forum Code" to HTML.
		/// </summary>
		/// <param name="basePage">Forum Page</param>
		/// <param name="Message">Message to Convert</param>
		/// <returns>Converted Message Text</returns>
		static protected string iConvertForumCode(yaf.pages.ForumPage basePage,string Message)
		{
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

			return tmp;
		}

		/// <summary>
		/// Adds smiles into the code.
		/// </summary>
		/// <param name="basePagee">Forum base page</param>
		/// <param name="Message">Text to add smiles to.</param>
		/// <returns>Processed text with smiles added.</returns>
		static public string iAddSmiles(yaf.pages.ForumPage basePage,string Message)
		{
			DataTable dtSmileys = GetSmilies(basePage);
			string strTemp = Message;

			foreach(DataRow row in dtSmileys.Rows) 
			{
				string code = row["Code"].ToString();

				strTemp = strTemp.Replace(code.ToLower(),String.Format("<img src=\"{0}\" alt=\"{1}\">",basePage.Smiley(Convert.ToString(row["Icon"])),basePage.Server.HtmlEncode(row["Emoticon"].ToString())));
				strTemp = strTemp.Replace(code.ToUpper(),String.Format("<img src=\"{0}\" alt=\"{1}\">",basePage.Smiley(Convert.ToString(row["Icon"])),basePage.Server.HtmlEncode(row["Emoticon"].ToString())));
			}

			return strTemp;
		}
	
		static public string HtmlToForumCode(string html) 
		{
#if true
			return html;
#else
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
#endif
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

		static public string FormatMessage(yaf.pages.ForumPage basePage,string Message,MessageFlags mFlags) 
		{
			// do html damage control
			Message = RepairHtml(basePage,Message,mFlags.IsHTML);

			// do BBCode and Smilies...
			Message = BBCode.MakeHtml(basePage,Message,mFlags.IsBBCode);

			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

			//Email -- RegEx VS.NET
			Message = Regex.Replace(Message, @"(^|[\n ])(?<email>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", "[email]${email}[/email]", options);

			//URL (http://) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
			Message = Regex.Replace(Message, "(^|[\n ])(?<!href=\")(?<!src=\")(?<url>http://(?:[\\w-]+\\.)+[\\w-]+(?:/[\\w-./?%&=;,]*)?)", "[url]${url}[/url]", options);

			//URL (www) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
			Message = Regex.Replace(Message, @"(^|[\n ])(?<!http://)(?<url>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=;,]*)?)", "[url=http://${url}]${url}[/url]", options);

			// jaben : moved word replace to reusable function in class utils
			Message = Utils.BadWordReplace(Message);

			return Message;
		}

		static private bool IsValidTag(string tag,string[] AllowedTags) 
		{
			if (tag.IndexOf("javascript") >= 0) return false;
			if (tag.IndexOf("vbscript") >= 0) return false;
			if (tag.IndexOf("onclick") >= 0)	return false;

			char[] endchars = new char[]{' ','>','/','\t'};
			
			int pos = tag.IndexOfAny(endchars,1);
			if (pos > 0) tag = tag.Substring(0,pos);
			if (tag[0] == '/') tag = tag.Substring(1);

			// check if it's a valid tag
			foreach (string aTag in AllowedTags)
			{
				if (tag == aTag) return true;
			}

			return false;
		}

		static public string RepairHtml(yaf.pages.ForumPage basePage,string html,bool bAllowHtml) 
		{
			if(!bAllowHtml) 
			{
				html = BBCode.SafeHtml(html);
			} 
			else 
			{
				// get allowable html tags
				string tStr = basePage.BoardSettings.AcceptedHTML;
				string[] AllowedTags = tStr.Split(',');

				RegexOptions options = RegexOptions.IgnoreCase;

				MatchCollection m = Regex.Matches(html,"<.*?>",options);

				for(int i=m.Count-1;i>=0;i--) 
				{
					string tag = html.Substring(m[i].Index+1,m[i].Length-1).Trim().ToLower();

					if (!IsValidTag(tag,AllowedTags)) 
					{
						html = html.Remove(m[i].Index,m[i].Length);
						// just don't show this tag for now

						//string tmp = System.Web.HttpContext.Current.Server.HtmlEncode(html.Substring(m[i].Index,m[i].Length));
						//html = html.Insert(m[i].Index,tmp);
					}
				}
			}
			return html;
		}
	}

	public class MessageFlags
	{
		int FBitValue;

		public MessageFlags() : this(0x7FFFFFFF)
		{

		}

		public MessageFlags(int bitValue)
		{
			FBitValue = bitValue;
		}

		static public bool GetBitAsBool(int bitValue,int bitShift)
		{
			if (bitShift > 31) bitShift %= 31;
			if (((bitValue >> bitShift) & 0x00000001) == 1) return true;
			return false;
		}

		static public int SetBitFromBool(int bitValue,int bitShift,bool bValue)
		{
			if (bitShift > 31) bitShift %= 31;

			if (GetBitAsBool(bitValue,bitShift) != bValue)
			{
				// toggle that value using XOR
				int tV = 0x00000001 << bitShift;
				bitValue ^= tV;
			}
			return bitValue;
		}

		public static implicit operator	MessageFlags(int newBitValue)
		{
			MessageFlags mf = new MessageFlags(newBitValue);
			return mf;
		}

		public int BitValue
		{
			get { return FBitValue; }
			set { FBitValue = value; }
		}

		public bool this[int index]
		{
			get { return GetBitAsBool(FBitValue,index); }
			set { FBitValue = SetBitFromBool(FBitValue,index,value); }
		}

		// actual flags here -- can be a total of 31
		public bool IsHTML
		{
			get { return GetBitAsBool(FBitValue,0); }
			set { FBitValue = SetBitFromBool(FBitValue,0,value); }
		}

		public bool IsBBCode
		{
			get { return GetBitAsBool(FBitValue,1); }
			set { FBitValue = SetBitFromBool(FBitValue,1,value); }
		}

		public bool IsSmilies
		{
			get { return GetBitAsBool(FBitValue,2); }
			set { FBitValue = SetBitFromBool(FBitValue,2,value); }
		}
	}
}
