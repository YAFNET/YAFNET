using System;
using System.Xml;

namespace yaf
{
	/// <summary>
	/// Summary description for RssFeed.
	/// </summary>
	public class RssFeed
	{
		// Credit goes to http://www.codeproject.com/aspnet/RSSviaXmlTextWriter.asp
		public RssFeed()
		{
			//
			// TODO: Add constructor logic here
			//

		}

		public XmlTextWriter WriteRSSPrologue(XmlTextWriter writer,pages.ForumPage page)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("rss");
			writer.WriteAttributeString("version", "2.0");
			writer.WriteStartElement("channel");
			writer.WriteElementString("title", "RSS File for " + page.ForumURL);
			writer.WriteElementString("link", page.ForumURL);
			writer.WriteElementString("description", "Yet Another Forum Web Application");
			writer.WriteElementString("copyright", "Copyright 2002-2004 Bjørnar Henden");

			return writer;
		}

		public XmlTextWriter AddRSSItem(XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription)
		{
			writer.WriteStartElement("item");
			writer.WriteElementString("title", sItemTitle);
			writer.WriteElementString("link", sItemLink);
			writer.WriteElementString("description", sItemDescription);
			writer.WriteElementString("pubDate", DateTime.Now.ToString("r"));
			writer.WriteEndElement();

			return writer;
		}

		public XmlTextWriter WriteRSSClosing(XmlTextWriter writer)
		{
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndDocument();

			return writer;
		}
	}
}