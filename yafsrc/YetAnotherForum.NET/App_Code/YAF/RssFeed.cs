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
using System.Xml;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF
{
	/// <summary>
	/// Summary description for RssFeed.
	/// </summary>
	public class RssFeed
	{
		public string en = Environment.NewLine;

		public RssFeed()
		{

		}

		public XmlTextWriter WriteRSSPrologue( XmlTextWriter writer )
		{
			/*
				writer.WriteStartDocument();
				writer.WriteStartElement("rss");
				writer.WriteAttributeString("version", "2.0");
				writer.WriteStartElement("channel");
				writer.WriteElementString("title", "RSS File for " + page.ForumURL);
				writer.WriteElementString("link", page.ForumURL);
				writer.WriteElementString("description", "Yet Another Forum Web Application");
				writer.WriteElementString("copyright", "Copyright 2002-2004 Bjørnar Henden");
			*/

			writer.WriteRaw( "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + en );
			writer.WriteRaw( "<?xml-stylesheet type=\"text/xsl\" href=\"" + YafForumInfo.ForumRoot + "rss.xsl\" media=\"screen\"?>" );
			writer.WriteRaw( "<rss version=\"2.0\">" + en );
			writer.WriteRaw( "\t<channel>" + en );
			writer.WriteRaw( "\t\t<title>RSS Feed for " + YafContext.Current.BoardSettings.Name + "</title>" + en );
			writer.WriteRaw( "\t\t<link>" + Encode( YafForumInfo.ForumURL ) + "</link>" + en );
			writer.WriteRaw( "\t\t<description>Yet Another Forum.NET Forum RSS Feed</description>" + en );
			writer.WriteRaw( "\t\t<copyright>Copyright 2006 - 2009 Jaben Cargman</copyright>" + en );

			return writer;
		}

		public XmlTextWriter AddRSSItem( XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription )
		{
			return this.AddRSSItem( writer, sItemTitle, sItemLink, sItemDescription, DateTime.Now.ToString( "r" ) );
		}

		public XmlTextWriter AddRSSItem( XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription, string sPubDate )
		{
			/*
				writer.WriteStartElement("item");
				writer.WriteElementString("title", sItemTitle);
				writer.WriteElementString("link", sItemLink);
				writer.WriteElementString("description", sItemDescription);
				writer.WriteElementString("pubDate", DateTime.Now.ToString("r"));
				writer.WriteEndElement();
			*/

			writer.WriteRaw( "\t\t<item>" + en );
			writer.WriteRaw( "\t\t\t<title>" + Encode( sItemTitle ) + "</title>" + en );
			writer.WriteRaw( "\t\t\t<link>" + Encode( sItemLink ) + "</link>" + en );
			writer.WriteRaw( "\t\t\t<description><![CDATA[" + sItemDescription + "]]></description>" + en );
			writer.WriteRaw( "\t\t\t<pubDate>" + sPubDate + "</pubDate>" + en );
			writer.WriteRaw( "\t\t</item>" + en );

			return writer;
		}

		public XmlTextWriter WriteRSSClosing( XmlTextWriter writer )
		{
			/*
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndDocument();
			*/

			writer.WriteRaw( "\t</channel>" + en );
			writer.WriteRaw( "</rss>" );

			return writer;
		}

		private string Encode( string input )
		{
			string output = input;
			output = output.Replace( "&", "&amp;" );
			return output;
		}
	}
}