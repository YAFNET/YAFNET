using System;

namespace yaf
{
	public class SectionHandler : System.Configuration.IConfigurationSectionHandler 
	{
		public object Create(object parent,object configContext,System.Xml.XmlNode section) 
		{
			return new Config(section);
		}
	}

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
				return (Config)System.Configuration.ConfigurationSettings.GetConfig("yafnet");
			}
		}
	}
}
