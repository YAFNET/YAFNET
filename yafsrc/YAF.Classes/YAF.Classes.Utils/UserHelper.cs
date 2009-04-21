using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace YAF.Classes.Utils
{
	static public class UserHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>language file name. If null -- use default language</returns>
		static public string GetUserLanguageFile( long userId )
		{
			// get the user information...
			DataRow row = UserMembershipHelper.GetUserRowForID( userId );

			if ( row != null && row["LanguageFile"] != DBNull.Value && YafContext.Current.BoardSettings.AllowUserLanguage )
			{
				return row["LanguageFile"].ToString();
			}

			return null;
		}
	}
}
