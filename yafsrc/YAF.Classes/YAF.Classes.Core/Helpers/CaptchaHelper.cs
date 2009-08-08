using System;
using System.Collections.Generic;
using System.Text;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public static class CaptchaHelper
	{
		/// <summary>
		/// Gets the CaptchaString using the BoardSettings
		/// </summary>
		/// <returns></returns>
		public static string GetCaptchaString()
		{
			return StringHelper.GenerateRandomString(YafContext.Current.BoardSettings.CaptchaSize, "abcdefghijkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ123456789");
		}
	}
}
