using System;
using System.Text.RegularExpressions;

namespace yaf
{
	/// <summary>
	/// Summary description for BBCode.
	/// </summary>
	public class BBCode
	{
		private BBCode()
		{
		}

		static private int GetNumber(string input) 
		{
			try
			{
				return int.Parse(input);
			}
			catch(Exception) 
			{
				return -1;
			}
		}

		static private string GetFontSize(int input) 
		{
			switch(input) 
			{
				case 1:
					return "50%";
				case 2:
					return "70%";
				case 3:
					return "80%";
				case 4:
					return "90%";
				case 5:
				default:
					return "100%";
				case 6:
					return "120%";
				case 7:
					return "140%";
				case 8:
					return "160%";
				case 9:
					return "180%";
			}
			///return string.Format("{0}pt",input*2);
		}

		static private RegexOptions	m_options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
		static private Regex		r_code = new Regex(@"\[code\](?<inner>(.*?))\[/code\]",m_options);
		static private Regex		r_size = new Regex(@"\[size=(?<size>[^\]]*)\](?<inner>(.*?))\[/size\]",m_options);
		static private Regex		r_bold = new Regex(@"\[B[^\]]*\](?<inner>(.*?))\[/B\]",m_options);
		static private Regex		r_strike = new Regex(@"\[S[^\]]*\](?<inner>(.*?))\[/S\]",m_options);
		static private Regex		r_italic = new Regex(@"\[I[^\]]*\](?<inner>(.*?))\[/I\]",m_options);
		static private Regex		r_underline = new Regex(@"\[U[^\]]*\](?<inner>(.*?))\[/U\]",m_options);
		static private Regex		r_email2 = new Regex(@"\[email=(?<email>[^\]]*)\](?<inner>(.*?))\[/email\]",m_options);
		static private Regex		r_email1 = new Regex(@"\[email[^\]]*\](?<inner>(.*?))\[/email\]",m_options);
		static private Regex		r_url2 = new Regex(@"\[url=(?<url>[^\]]*)\](?<inner>(.*?))\[/url\]",m_options);
		static private Regex		r_url1 = new Regex(@"\[url[^\]]*\](?<inner>(.*?))\[/url\]",m_options);
		static private Regex		r_font = new Regex(@"\[font=(?<font>[^\]]*)\](?<inner>(.*?))\[/font\]",m_options);
		static private Regex		r_color = new Regex(@"\[color=(?<color>[^\]]*)\](?<inner>(.*?))\[/color\]",m_options);
		static private Regex		r_bullet = new Regex(@"\[\*\]",m_options);
		static private Regex		r_list4 = new Regex(@"\[list=i\](?<inner>(.*?))\[/list\]",m_options);
		static private Regex		r_list3 = new Regex(@"\[list=a\](?<inner>(.*?))\[/list\]",m_options);
		static private Regex		r_list2 = new Regex(@"\[list=1\](?<inner>(.*?))\[/list\]",m_options);
		static private Regex		r_list1 = new Regex(@"\[list\](?<inner>(.*?))\[/list\]",m_options);
		static private Regex		r_center = new Regex(@"\[center\](?<inner>(.*?))\[/center\]",m_options);
		static private Regex		r_left = new Regex(@"\[left\](?<inner>(.*?))\[/left\]",m_options);
		static private Regex		r_right = new Regex(@"\[right\](?<inner>(.*?))\[/right\]",m_options);
		static private Regex		r_quote2 = new Regex(@"\[quote=(?<quote>[^\]]*)\](?<inner>(.*?))\[/quote\]",m_options);
		static private Regex		r_quote1 = new Regex(@"\[quote\](?<inner>(.*?))\[/quote\]",m_options);
		static private Regex		r_hr = new Regex("^[-][-][-][-][-]*[\r]?[\n]",m_options);
		static private Regex		r_br = new Regex("[\r]?\n",m_options);

		static public string MakeHtml(string bbcode)
		{
			Match m = r_code.Match(bbcode);
			while(m.Success) 
			{
				string before_replace = m.Groups[0].Value;
				string after_replace = m.Groups["inner"].Value;

				after_replace = after_replace.Replace("  ","&nbsp; ");
				after_replace = after_replace.Replace("  "," &nbsp;");
				after_replace = after_replace.Replace("\t","&nbsp; &nbsp;&nbsp;");
				after_replace = after_replace.Replace("[","&#91;");
				after_replace = after_replace.Replace("]","&#93;");
				after_replace = after_replace.Replace("<br/>","\n");
				after_replace = System.Web.HttpContext.Current.Server.HtmlEncode(after_replace);
				bbcode = bbcode.Replace(before_replace,string.Format("<pre>{0}</pre>",after_replace));
				break;
			}

			m = r_size.Match(bbcode);
			while(m.Success) 
			{
				Console.WriteLine("{0}",m.Groups["size"]);
				int i = GetNumber(m.Groups["size"].Value);
				string tmp;
				if(i<1)
					tmp = m.Groups["inner"].Value;
				else if(i>9)
					tmp = string.Format("<span style=\"font-size:{1}\">{0}</span>",m.Groups["inner"].Value,GetFontSize(9));
				else
					tmp = string.Format("<span style=\"font-size:{1}\">{0}</span>",m.Groups["inner"].Value,GetFontSize(i));
				bbcode = bbcode.Substring(0,m.Groups[0].Index) + tmp + bbcode.Substring(m.Groups[0].Index + m.Groups[0].Length);
				m = r_size.Match(bbcode);
			}

			bbcode = r_bold.Replace(bbcode,"<b>${inner}</b>");
			bbcode = r_strike.Replace(bbcode,"<s>${inner}</s>");
			bbcode = r_italic.Replace(bbcode,"<em>${inner}</em>");
			bbcode = r_underline.Replace(bbcode,"<u>${inner}</u>");
			bbcode = r_email2.Replace(bbcode,"<a href=\"mailto:${email}\">${inner}</a>");
			bbcode = r_email1.Replace(bbcode,"<a href=\"mailto:${inner}\">${inner}</a>");
			bbcode = r_url2.Replace(bbcode,"<a href=\"${url}\">${inner}</a>");
			bbcode = r_url1.Replace(bbcode,"<a href=\"${inner}\">${inner}</a>");
			bbcode = r_font.Replace(bbcode,"<span style=\"font-family:${font}\">${inner}</span>");
			bbcode = r_color.Replace(bbcode,"<span style=\"color:${color}\">${inner}</span>");
			bbcode = r_bullet.Replace(bbcode,"<li>");
			bbcode = r_list4.Replace(bbcode,"<ol type=\"i\">${inner}</ol>");
			bbcode = r_list3.Replace(bbcode,"<ol type=\"a\">${inner}</ol>");
			bbcode = r_list2.Replace(bbcode,"<ol>${inner}</ol>");
			bbcode = r_list1.Replace(bbcode,"<ul>${inner}</ul>");
			bbcode = r_center.Replace(bbcode,"<div align=\"center\">${inner}</div>");
			bbcode = r_left.Replace(bbcode,"<div align=\"left\">${inner}</div>");
			bbcode = r_right.Replace(bbcode,"<div align=\"right\">${inner}</div>");
			bbcode = r_quote2.Replace(bbcode,"<div><b>QUOTE (${quote})</b><div style=\"background-color:#F0F0F0\">${inner}</div></div>");
			bbcode = r_quote1.Replace(bbcode,"<div><b>QUOTE</b><div style=\"background-color:#F0F0F0\">${inner}</div></div>");

			bbcode = r_hr.Replace(bbcode,"<hr noshade/>");
			bbcode = r_br.Replace(bbcode,"<br/>");

			return bbcode;
		}
	}
}
