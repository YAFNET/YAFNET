using System;
using System.Web;

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

		static public ForumSettings ForumSettings
		{
			get
			{
				if(HttpContext.Current.Application["yaf_ForumSettings"]==null)
					HttpContext.Current.Application["yaf_ForumSettings"] = new ForumSettings();

				return (ForumSettings)HttpContext.Current.Application["yaf_ForumSettings"];
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
