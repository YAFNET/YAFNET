using System;
using System.Data;
using System.Web;

namespace yaf
{
	public class SectionHandler : System.Configuration.IConfigurationSectionHandler 
	{
		public object Create(object parent,object configContext,System.Xml.XmlNode section) 
		{
			return new Config(section);
		}
	}
}
