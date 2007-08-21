using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace YAF.Classes.Utils
{
	public class yaf_Cache
	{
		private static yaf_Cache currentInstance = new yaf_Cache();

		public static yaf_Cache Current
		{
			get
			{
				return currentInstance;
			}
		}

		public object this [string index]
		{
			get
			{
				return HttpContext.Current.Cache [index];
			}
		}
	}

	public class yaf_CacheEntryInfo
	{
		private DateTime itemExpire;
	}
}
