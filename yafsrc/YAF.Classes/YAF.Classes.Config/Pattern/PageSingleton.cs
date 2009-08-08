using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace YAF.Classes.Pattern
{
	// Singleton factory implementation
	public static class PageSingleton<T> where T : class
	{
		// static constructor, 
		//runtime ensures thread safety
		static PageSingleton()
		{
			// create the single instance 
			//_instance = GetInstance();
		}

		static private T GetInstance()
		{
			/*
			Page currentPage = HttpContext.Current.Handler as Page;

			if ( currentPage == null )
			{
				if ( _instance == null )
				{
					_instance = (T)Activator.CreateInstance( typeof( T ), true );
				}
				return _instance;
			}			

			return (T)( currentPage.Items[typeStr] ?? ( currentPage.Items[typeStr] = (T)Activator.CreateInstance( typeof( T ), true ) ) );
			*/

			string typeStr = typeof( T ).ToString();

			return (T)( HttpContext.Current.Items[typeStr] ?? ( HttpContext.Current.Items[typeStr] = (T)Activator.CreateInstance( typeof( T ), true ) ) );
		}

		private static T _instance = null;
		public static T Instance
		{
			private set { _instance = value; }
			get
			{
				return GetInstance();
			}
		}
	}
}