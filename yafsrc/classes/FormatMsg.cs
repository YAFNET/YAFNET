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

		public string FormatMessage(string Message) {
			if(dtSmileys == null)
				dtSmileys = DB.smiley_list(null);

			string tmp = Message;

			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

			// [b]...[/b]
			tmp = Regex.Replace(tmp,@"\[b\](.*?)\[/b\]","<b>$1</b>",options);

			// [i]...[/i]
			tmp = Regex.Replace(tmp,@"\[i\](.*?)\[/i\]","<em>$1</em>",options);

			// [u]...[/u]
			tmp = Regex.Replace(tmp,@"\[u\](.*?)\[/u\]","<u>$1</u>",options);

			// [img]...[/img]
			tmp = Regex.Replace(tmp,@"\[img\](.*?)\[/img\]","<image src=\"$1\"/>",options);

			// [url=...]...[/url]
			tmp = Regex.Replace(tmp,@"\[url=(.*?)\](.*?)\[/url\]","<a href=\"$1\">$2</a>",options);

			// [url]...[/url]
			tmp = Regex.Replace(tmp,@"\[url\](.*?)\[/url\]","<a href=\"$1\">$1</a>",options);

			// [color=...]...[/color]
			tmp = Regex.Replace(tmp,@"\[color=(.*?)\](.*?)\[/color\]","<span style=\"color:$1\">$2</span>",options);

			// [quote=...]...[/quote]
			// [quote]...[/quote]
			tmp = Regex.Replace(tmp,@"\[quote=(.*?)\]","<div class=quote>$1 wrote:<div class=\"quoteinner\">",options);
			tmp = Regex.Replace(tmp,@"\[quote\]","<div class=quote><div class=\"quoteinner\">",options);
			tmp = Regex.Replace(tmp,@"\[/quote\]","</div></div>",options);
		
			// [code]...[/code]
			tmp = Regex.Replace(tmp,@"\[code\]","<pre class=\"code\">",options);
			tmp = Regex.Replace(tmp,@"\[/code\]","</pre>",options);

			tmp = tmp.Replace("\r\n","<br/>");

			foreach(DataRow row in dtSmileys.Rows)
				tmp = tmp.Replace((string)row["Code"],String.Format("<img src=\"{0}\"/>",basePage.Smiley((string)row["Icon"])));

			return tmp;
		}
	}
}
