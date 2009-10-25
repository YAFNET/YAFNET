using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAF.Classes.Pattern;

namespace YAF.Classes
{
	public class YafBoardFolders
	{

		public static YafBoardFolders Current
		{
			get
			{
				return PageSingleton<YafBoardFolders>.Instance;
			}
		}

		public string BoardFolder
		{
			get
			{
				if ( Config.MultiBoardFolders )
					return String.Format( Config.BoardRoot + "{0}/", YafControlSettings.Current.BoardID );
				else
					return Config.BoardRoot;
			}
		}

		public string Uploads
		{
			get
			{
				return String.Concat( BoardFolder, "Uploads" );
			}
		}

		public string Themes
		{
			get
			{
				return String.Concat( BoardFolder, "Themes" );
			}
		}

		public string Images
		{
			get
			{
				return String.Concat( BoardFolder, "Images" );
			}
		}

		public string Avatars
		{
			get
			{
				return String.Concat( BoardFolder, "Images/Avatars" );
			}
		}

		public string Categories
		{
			get
			{
				return String.Concat( BoardFolder, "Images/Categories" );
			}
		}

		public string Emoticons
		{
			get
			{
				return String.Concat( BoardFolder, "Images/Emoticons" );
			}
		}

		public string Medals
		{
			get
			{
				return String.Concat( BoardFolder, "Images/Medals" );
			}
		}

		public string Ranks
		{
			get
			{
				return String.Concat( BoardFolder, "Images/Ranks" );
			}
		}
	}
}
