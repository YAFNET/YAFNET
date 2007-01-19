using System;
using System.Web;
using System.Text.RegularExpressions;
using YAF.Classes.Utils;

namespace YAF
{
	/// <summary>
	/// Summary description for BBCode.
	/// </summary>
	public class BBCode
	{
		private BBCode()
		{
		}

		static private int GetNumber( string input )
		{
			try
			{
				return int.Parse( input );
			}
			catch ( FormatException )
			{
				return -1;
			}
		}

		static private string GetFontSize( int input )
		{
			switch ( input )
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

		static private RegexOptions m_options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
		static private Regex r_code2 = new Regex( @"\[code=(?<language>[^\]]*)\](?<inner>(.*?))\[/code\]", m_options );
		static private Regex r_code1 = new Regex( @"\[code\](?<inner>(.*?))\[/code\]", m_options );
		static private Regex r_size = new Regex( @"\[size=(?<size>([1-9]))\](?<inner>(.*?))\[/size\]", m_options );
		static private Regex r_bold = new Regex( @"\[B\](?<inner>(.*?))\[/B\]", m_options );
		static private Regex r_strike = new Regex( @"\[S\](?<inner>(.*?))\[/S\]", m_options );
		static private Regex r_italic = new Regex( @"\[I\](?<inner>(.*?))\[/I\]", m_options );
		static private Regex r_underline = new Regex( @"\[U\](?<inner>(.*?))\[/U\]", m_options );
		static private Regex r_email2 = new Regex( @"\[email=(?<email>[^\]]*)\](?<inner>(.*?))\[/email\]", m_options );
		static private Regex r_email1 = new Regex( @"\[email[^\]]*\](?<inner>(.*?))\[/email\]", m_options );
		static private Regex r_url1 = new Regex( @"\[url\]( ?<http>( skype:)|( http://)|( https://)| (ftp://)|( ftps://))? (?<inner>( .*?))\[/ur l\]", m_options );
		static private Regex r_url2 = new Regex( @"\[url\]( ?<http>( skype:)|( http://)|( https://)| (ftp://)|( ftps://))? (?<inner>( .*?))\[/ur l\]", m_options );
		static private Regex r_font = new Regex( @"\[font=(?<font>([-a-z0-9, ]*))\](?<inner>(.*?))\[/font\]", m_options );
		static private Regex r_color = new Regex( @"\[color=(?<color>(\#?[-a-z0-9]*))\](?<inner>(.*?))\[/color\]", m_options );
		static private Regex r_bullet = new Regex( @"\[\*\]", m_options );
		static private Regex r_list4 = new Regex( @"\[list=i\](?<inner>(.*?))\[/list\]", m_options );
		static private Regex r_list3 = new Regex( @"\[list=a\](?<inner>(.*?))\[/list\]", m_options );
		static private Regex r_list2 = new Regex( @"\[list=1\](?<inner>(.*?))\[/list\]", m_options );
		static private Regex r_list1 = new Regex( @"\[list\](?<inner>(.*?))\[/list\]", m_options );
		static private Regex r_center = new Regex( @"\[center\](?<inner>(.*?))\[/center\]", m_options );
		static private Regex r_left = new Regex( @"\[left\](?<inner>(.*?))\[/left\]", m_options );
		static private Regex r_right = new Regex( @"\[right\](?<inner>(.*?))\[/right\]", m_options );
		static private Regex r_quote2 = new Regex( @"\[quote=(?<quote>[^\]]*)\](?<inner>(.*?))\[/quote\]", m_options );
		static private Regex r_quote1 = new Regex( @"\[quote\](?<inner>(.*?))\[/quote\]", m_options );
		static private Regex r_hr = new Regex( "^[-][-][-][-][-]*[\r]?[\n]", m_options );
		static private Regex r_br = new Regex( "[\r]?\n", m_options );
		static private Regex r_post = new Regex( @"\[post=(?<post>[^\]]*)\](?<inner>(.*?))\[/post\]", m_options );
		static private Regex r_topic = new Regex( @"\[topic=(?<topic>[^\]]*)\](?<inner>(.*?))\[/topic\]", m_options );
		static private Regex r_img = new Regex( @"\[img\](?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>(.*?))\[/img\]", m_options );

		static public string MakeHtml( string bbcode, bool DoFormatting )
		{
			System.Collections.ArrayList codes = new System.Collections.ArrayList();
			const string codeFormat = ".code@{0}.";

			string localQuoteStr = yaf_Context.Current.Localization.GetText( "COMMON", "BBCODE_QUOTE" );
			string localCodeStr = yaf_Context.Current.Localization.GetText( "COMMON", "BBCODE_CODE" );
			string localQuoteWroteStr = yaf_Context.Current.Localization.GetText( "COMMON", "BBCODE_QUOTEWROTE" );

			Match m = r_code2.Match( bbcode );
			int nCodes = 0;

			while ( m.Success )
			{
				string before_replace = m.Groups [0].Value;
				string after_replace = m.Groups ["inner"].Value;

				try
				{
					HighLighter hl = new HighLighter();
					hl.ReplaceEnter = true;
					after_replace = hl.colorText( after_replace, HttpContext.Current.Server.MapPath( yaf_ForumInfo.ForumRoot + "defs/" ), m.Groups ["language"].Value );
				}
				catch ( Exception x )
				{
					if ( yaf_Context.Current.IsAdmin )
						yaf_Context.Current.AddLoadMessage( x.Message );
					after_replace = FixCode( after_replace );
				}

				bbcode = bbcode.Replace( before_replace, string.Format( codeFormat, nCodes++ ) );
				codes.Add( string.Format( @"<div class=""code""><b>{1}</b><div class=""innercode"">{0}</div></div>", after_replace, localCodeStr ) );
				m = r_code2.Match( bbcode );

			}

			m = r_code1.Match( bbcode );
			while ( m.Success )
			{
				string before_replace = m.Groups [0].Value;
				string after_replace = FixCode( m.Groups ["inner"].Value );
				bbcode = bbcode.Replace( before_replace, string.Format( codeFormat, nCodes++ ) );
				codes.Add( string.Format( @"<div class=""code""><b>{1}</b><div class=""innercode"">{0}</div></div>", after_replace, localCodeStr ) );
				m = r_code1.Match( bbcode );
			}

			m = r_size.Match( bbcode );

			while ( m.Success )
			{
				///Console.WriteLine("{0}",m.Groups["size"]);
				int i = GetNumber( m.Groups ["size"].Value );
				string tmp;
				if ( i < 1 )
					tmp = m.Groups ["inner"].Value;
				else if ( i > 9 )
					tmp = string.Format( "<span style=\"font-size:{1}\">{0}</span>", m.Groups ["inner"].Value, GetFontSize( 9 ) );
				else
					tmp = string.Format( "<span style=\"font-size:{1}\">{0}</span>", m.Groups ["inner"].Value, GetFontSize( i ) );
				bbcode = bbcode.Substring( 0, m.Groups [0].Index ) + tmp + bbcode.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = r_size.Match( bbcode );
			}

			if ( DoFormatting )
			{
				NestedReplace( ref bbcode, r_bold, "<b>${inner}</b>" );
				NestedReplace( ref bbcode, r_strike, "<s>${inner}</s>" );
				NestedReplace( ref bbcode, r_italic, "<i>${inner}</i>" );
				NestedReplace( ref bbcode, r_underline, "<u>${inner}</u>" );
				// e-mails
				NestedReplace( ref bbcode, r_email2, "<a href=\"mailto:${email}\">${inner}</a>", new string [] { "email" } );
				NestedReplace( ref bbcode, r_email1, "<a href=\"mailto:${inner}\">${inner}</a>" );
				// urls
				if ( yaf_Context.Current.BoardSettings.BlankLinks )
				{
					NestedReplace( ref bbcode, r_url2, "<a target=\"_blank\" rel=\"nofollow\" href=\"${http}${url}\">${inner}</a>", new string [] { "url", "http" }, new string [] { "", "http://" } );
					NestedReplace( ref bbcode, r_url1, "<a target=\"_blank\" rel=\"nofollow\" href=\"${http}${inner}\">${http}${inner}</a>", new string [] { "http" }, new string [] { "http://" } );
				}
				else
				{
					NestedReplace( ref bbcode, r_url2, "<a rel=\"nofollow\" href=\"${http}${url}\">${inner}</a>", new string [] { "url", "http" }, new string [] { "", "http://" } );
					NestedReplace( ref bbcode, r_url1, "<a rel=\"nofollow\" href=\"${http}${inner}\">${http}${inner}</a>", new string [] { "http" }, new string [] { "http://" } );
				}
				// font
				NestedReplace( ref bbcode, r_font, "<span style=\"font-family:${font}\">${inner}</span>", new string [] { "font" } );
				NestedReplace( ref bbcode, r_color, "<span style=\"color:${color}\">${inner}</span>", new string [] { "color" } );
				// bullets
				bbcode = r_bullet.Replace( bbcode, "<li>" );
				NestedReplace( ref bbcode, r_list4, "<ol type=\"i\">${inner}</ol>" );
				NestedReplace( ref bbcode, r_list3, "<ol type=\"a\">${inner}</ol>" );
				NestedReplace( ref bbcode, r_list2, "<ol>${inner}</ol>" );
				NestedReplace( ref bbcode, r_list2, "<ul>${inner}</ul>" );
				// alignment
				NestedReplace( ref bbcode, r_center, "<div align=\"center\">${inner}</div>" );
				NestedReplace( ref bbcode, r_left, "<div align=\"left\">${inner}</div>" );
				NestedReplace( ref bbcode, r_right, "<div align=\"right\">${inner}</div>" );
				// image
				NestedReplace( ref bbcode, r_img, "<img src=\"${http}${inner}\"/>", new string [] { "http" }, new string [] { "http://" } );

				bbcode = r_hr.Replace( bbcode, "<hr noshade/>" );
				bbcode = r_br.Replace( bbcode, "<br/>" );
			}

			bbcode = FormatMsg.iAddSmiles( bbcode );

			string tmpReplaceStr;

			tmpReplaceStr = string.Format( @"<div class=""quote""><b>{0}</b><div class=""innerquote"">{1}</div></div>", localQuoteWroteStr.Replace( "{0}", "${quote}" ), "${inner}" );

			while ( r_quote2.IsMatch( bbcode ) )
				bbcode = r_quote2.Replace( bbcode, tmpReplaceStr );

			tmpReplaceStr = string.Format( @"<div class=""quote""><b>{0}</b><div class=""innerquote"">{1}</div></div>", localQuoteStr, "${inner}" );
						
			while ( r_quote1.IsMatch( bbcode ) )
				bbcode = r_quote1.Replace( bbcode, tmpReplaceStr );

			m = r_post.Match( bbcode );
			while ( m.Success )
			{
				string link = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", m.Groups ["post"] );
				if ( yaf_Context.Current.BoardSettings.BlankLinks )
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a target=\"_blank\" href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				else
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				m = r_post.Match( bbcode );
			}

			m = r_topic.Match( bbcode );
			while ( m.Success )
			{
				string link = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", m.Groups ["topic"] );
				if ( yaf_Context.Current.BoardSettings.BlankLinks )
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a target=\"_blank\" href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				else
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				m = r_topic.Match( bbcode );
			}

			while ( nCodes > 0 )
			{
				bbcode = bbcode.Replace( string.Format( codeFormat, --nCodes ), codes [nCodes].ToString() );
			}

			return bbcode;
		}

		static protected void NestedReplace( ref string refText, Regex regexMatch, string strReplace, string [] Variables, string [] VarDefaults )
		{
			Match m = regexMatch.Match( refText );
			while ( m.Success )
			{
				string tStr = strReplace;
				int i = 0;

				foreach ( string tVar in Variables )
				{
					string tValue = m.Groups [tVar].Value;
					if ( tValue.Length == 0 )
					{
						// use default instead
						tValue = VarDefaults [i];
					}
					tStr = tStr.Replace( "${" + tVar + "}", tValue );
					i++;
				}

				tStr = tStr.Replace( "${inner}", m.Groups ["inner"].Value );

				refText = refText.Substring( 0, m.Groups [0].Index ) + tStr + refText.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = regexMatch.Match( refText );
			}
		}

		static protected void NestedReplace( ref string refText, Regex regexMatch, string strReplace, string [] Variables )
		{
			Match m = regexMatch.Match( refText );
			while ( m.Success )
			{
				string tStr = strReplace;

				foreach ( string tVar in Variables )
				{
					tStr = tStr.Replace( "${" + tVar + "}", m.Groups [tVar].Value );
				}

				tStr = tStr.Replace( "${inner}", m.Groups ["inner"].Value );

				refText = refText.Substring( 0, m.Groups [0].Index ) + tStr + refText.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = regexMatch.Match( refText );
			}
		}

		static protected void NestedReplace( ref string refText, Regex regexMatch, string strReplace )
		{
			Match m = regexMatch.Match( refText );
			while ( m.Success )
			{
				string tStr = strReplace.Replace( "${inner}", m.Groups ["inner"].Value );
				refText = refText.Substring( 0, m.Groups [0].Index ) + tStr + refText.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = regexMatch.Match( refText );
			}
		}

		static public string EncodeHTML( string html )
		{
			return System.Web.HttpContext.Current.Server.HtmlEncode( html );
		}

		static public string DecodeHTML( string text )
		{
			return System.Web.HttpContext.Current.Server.HtmlDecode( text );
		}

		static private string FixCode( string html )
		{
			html = html.Replace( "  ", "&nbsp; " );
			html = html.Replace( "  ", " &nbsp;" );
			html = html.Replace( "\t", "&nbsp; &nbsp;&nbsp;" );
			html = html.Replace( "[", "&#91;" );
			html = html.Replace( "]", "&#93;" );
			html = html.Replace( "<", "&lt;" );
			html = html.Replace( ">", "&gt;" );
			html = html.Replace( "\r\n", "<br/>" );
			return html;
		}
	}
}
