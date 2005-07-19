using System;
using System.Xml;

namespace yaf
{
	/// <summary>
	/// Summary description for RssFeed.
	/// </summary>
	public class RssFeed
	{
		public string en = Environment.NewLine;
		
		public RssFeed()
		{
			//
			// TODO: Add constructor logic here
			//

		}

		public XmlTextWriter WriteRSSPrologue(XmlTextWriter writer, pages.ForumPage page)
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

			writer.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + en);
			writer.WriteRaw("<rss version=\"2.0\">" + en);
			writer.WriteRaw("\t<channel>" + en);
			writer.WriteRaw("\t\t<title>RSS Feed for " + page.ServerURL + "</title>" + en);
			writer.WriteRaw("\t\t<link>" + Encode(page.ForumURL) + "</link>" + en);
			writer.WriteRaw("\t\t<description>Yet Another Forum Web Application RSS Feed</description>" + en);
			writer.WriteRaw("\t\t<copyright>Copyright 2002 - 2004 Bjørnar Henden</copyright>" + en);

			return writer;
		}

		public XmlTextWriter AddRSSItem(XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription)
		{
			return this.AddRSSItem(writer,sItemTitle,sItemLink,sItemDescription,DateTime.Now.ToString("r"));
		}

		public XmlTextWriter AddRSSItem(XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription, string sPubDate)
		{
			/*
				writer.WriteStartElement("item");
				writer.WriteElementString("title", sItemTitle);
				writer.WriteElementString("link", sItemLink);
				writer.WriteElementString("description", sItemDescription);
				writer.WriteElementString("pubDate", DateTime.Now.ToString("r"));
				writer.WriteEndElement();
			*/

			writer.WriteRaw("\t\t<item>" + en);
			writer.WriteRaw("\t\t\t<title>" + Encode(sItemTitle) + "</title>" + en);
			writer.WriteRaw("\t\t\t<link>" + Encode(sItemLink) + "</link>" + en);
			writer.WriteRaw("\t\t\t<description><![CDATA[" + sItemDescription + "]]></description>" + en);
			writer.WriteRaw("\t\t\t<pubDate>" + sPubDate + "</pubDate>" + en);
			writer.WriteRaw("\t\t</item>" + en);

			return writer;
		}

		public XmlTextWriter WriteRSSClosing(XmlTextWriter writer)
		{
			/*
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndDocument();
			*/

			writer.WriteRaw("\t</channel>" + en);
			writer.WriteRaw("</rss>");

			return writer;
		}

		private string Encode(string input)
		{
			string output = input;
			output = output.Replace("&", "&amp;");
			return output;
		}
	}
}