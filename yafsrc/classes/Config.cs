using System;
using System.Web;
using yaf.pages;

namespace yaf
{
	public class Config
	{
		private	System.Xml.XmlNode m_section;

		public Config(System.Xml.XmlNode node)
		{
			m_section = node;
		}

		public string this[string key]
		{
			get
			{
				System.Xml.XmlNode node = m_section.SelectSingleNode(key);
				if(node!=null)
					return node.InnerText;
				else
					return null;
			}
		}

		static public Config ConfigSection
		{
			get
			{
				Config config = (Config)System.Configuration.ConfigurationSettings.GetConfig("yafnet");
				if(config==null)
					throw new ApplicationException("Failed to get configuration from Web.config");
				return config;
			}
		}

		static public bool IsDotNetNuke
		{
			get
			{
				object obj = HttpContext.Current.Items["PortalSettings"];
				return obj!=null && obj.GetType().ToString().ToLower().IndexOf("dotnetnuke")>=0;
			}
		}

		static public bool IsRainbow
		{
			get
			{
				object obj = HttpContext.Current.Items["PortalSettings"];
				return obj!=null && obj.GetType().ToString().ToLower().IndexOf("rainbow")>=0;
			}
		}

		static public BoardSettings BoardSettings
		{
			get
			{
				string key = string.Format("yaf_BoardSettings.{0}",ForumPage.PageBoardID);
				if(HttpContext.Current.Application[key]==null)
					HttpContext.Current.Application[key] = new BoardSettings(ForumPage.PageBoardID);

				return (BoardSettings)HttpContext.Current.Application[key];
			}
			set
			{
				string key = string.Format("yaf_BoardSettings.{0}",ForumPage.PageBoardID);
				HttpContext.Current.Application.Remove(key);
			}
		}

		static public IUrlBuilder UrlBuilder
		{
			get
			{
				if(HttpContext.Current.Application["yaf_UrlBuilder"]==null)
				{
					if(IsRainbow)
						HttpContext.Current.Application["yaf_UrlBuilder"] = new UrlBuilderRainbow();
					else if(IsDotNetNuke)
						HttpContext.Current.Application["yaf_UrlBuilder"] = new UrlBuilderDotNetNuke();
					else
						HttpContext.Current.Application["yaf_UrlBuilder"] = new UrlBuilder();
				}

				return (IUrlBuilder)HttpContext.Current.Application["yaf_UrlBuilder"];
			}
		}
	}
}
