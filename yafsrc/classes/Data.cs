/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Data;
using System.Web;

namespace yaf
{
	public enum AuthType 
	{
		Guest,
		Forms,
		Windows,
		Rainbow,
		DotNetNuke
	};

	public enum SEARCH_FIELD
	{
		sfMESSAGE = 0,
		sfUSER_NAME = 1
	}

	public enum SEARCH_WHAT
	{
		sfALL_WORDS = 0,
		sfANY_WORDS = 1,
		sfEXACT = 2
	}

	/// <summary>
	/// Summary description for Data.
	/// </summary>
	public class Data 
	{
		public static string ForumRoot
		{
			get 
			{
				try
				{
					return Config.ConfigSection["root"];
				}
				catch(Exception)
				{
					return "/";
				}
			}
		}

		public static AuthType GetAuthType 
		{
			get 
			{
				if(Config.IsDotNetNuke)
					return AuthType.DotNetNuke;
				else if(Config.IsRainbow)
					return AuthType.Rainbow;

				Type secType = System.Web.HttpContext.Current.User.Identity.GetType();

				if(secType==typeof(System.Web.Security.FormsIdentity)) 
					return AuthType.Forms;
				else if(secType==typeof(System.Security.Principal.WindowsIdentity)) 
					return AuthType.Windows;
				else if(secType==typeof(System.Security.Principal.GenericIdentity)) 
					return AuthType.Guest;
				else
#if DEBUG
					throw new Exception(string.Format("Unknown security: {0}",secType));
#else
					return AuthType.Guest;
#endif
			}
		}

		public static void AccessDenied() 
		{
			Forum.Redirect(Pages.info,"i=4");
		}

		public static DataTable TimeZones() {
			using(DataTable dt = new DataTable("TimeZone")) 
			{
				dt.Columns.Add("Value",Type.GetType("System.Int32"));
				dt.Columns.Add("Name",Type.GetType("System.String"));

				dt.Rows.Add(new object[]{-720,"(GMT - 12:00) Enitwetok, Kwajalien"});
				dt.Rows.Add(new object[]{-660,"(GMT - 11:00) Midway Island, Samoa"});
				dt.Rows.Add(new object[]{-600,"(GMT - 10:00) Hawaii"});
				dt.Rows.Add(new object[]{-540,"(GMT - 9:00) Alaska"});
				dt.Rows.Add(new object[]{-480,"(GMT - 8:00) Pacific Time (US & Canada)"});
				dt.Rows.Add(new object[]{-420,"(GMT - 7:00) Mountain Time (US & Canada)"});
				dt.Rows.Add(new object[]{-360,"(GMT - 6:00) Central Time (US & Canada), Mexico City"});
				dt.Rows.Add(new object[]{-300,"(GMT - 5:00) Eastern Time (US & Canada), Bogota, Lima, Quito"});
				dt.Rows.Add(new object[]{-240,"(GMT - 4:00) Atlantic Time (Canada), Caracas, La Paz"});
				dt.Rows.Add(new object[]{-210,"(GMT - 3:30) Newfoundland"});
				dt.Rows.Add(new object[]{-180,"(GMT - 3:00) Brazil, Buenos Aires, Georgetown, Falkland Is."});
				dt.Rows.Add(new object[]{-120,"(GMT - 2:00) Mid-Atlantic, Ascention Is., St Helena"});
				dt.Rows.Add(new object[]{-60,"(GMT - 1:00) Azores, Cape Verde Islands"});
				dt.Rows.Add(new object[]{0,"(GMT) Casablanca, Dublin, Edinburgh, London, Lisbon, Monrovia"});
				dt.Rows.Add(new object[]{60,"(GMT + 1:00) Berlin, Brussels, Kristiansund, Madrid, Paris, Rome"});
				dt.Rows.Add(new object[]{120,"(GMT + 2:00) Kaliningrad, South Africa, Warsaw"});
				dt.Rows.Add(new object[]{180,"(GMT + 3:00) Baghdad, Riyadh, Moscow, Nairobi"});
				dt.Rows.Add(new object[]{210,"(GMT + 3:30) Tehran"});
				dt.Rows.Add(new object[]{240,"(GMT + 4:00) Adu Dhabi, Baku, Muscat, Tbilisi"});
				dt.Rows.Add(new object[]{270,"(GMT + 4:30) Kabul"});
				dt.Rows.Add(new object[]{300,"(GMT + 5:00) Ekaterinburg, Islamabad, Karachi, Tashkent"});
				dt.Rows.Add(new object[]{330,"(GMT + 5:30) Bombay, Calcutta, Madras, New Delhi"});
				dt.Rows.Add(new object[]{360,"(GMT + 6:00) Almaty, Colomba, Dhakra"});
				dt.Rows.Add(new object[]{420,"(GMT + 7:00) Bangkok, Hanoi, Jakarta"});
				dt.Rows.Add(new object[]{480,"(GMT + 8:00) Beijing, Hong Kong, Perth, Singapore, Taipei"});
				dt.Rows.Add(new object[]{540,"(GMT + 9:00) Osaka, Sapporo, Seoul, Tokyo, Yakutsk"});
				dt.Rows.Add(new object[]{570,"(GMT + 9:30) Adelaide, Darwin"});
				dt.Rows.Add(new object[]{600,"(GMT + 10:00) Melbourne, Papua New Guinea, Sydney, Vladivostok"});
				dt.Rows.Add(new object[]{660,"(GMT + 11:00) Magadan, New Caledonia, Solomon Islands"});
				dt.Rows.Add(new object[]{720,"(GMT + 12:00) Auckland, Wellington, Fiji, Marshall Island"});
				return dt;
			}
		}

		public static DataTable Themes() 
		{
			using(DataTable dt = new DataTable("Themes")) 
			{
				dt.Columns.Add("Theme",typeof(string));
				dt.Columns.Add("FileName",typeof(string));
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Web.HttpContext.Current.Request.MapPath(String.Format("{0}themes",Data.ForumRoot)));
				System.IO.FileInfo[] files = dir.GetFiles("*.xml");
				foreach(System.IO.FileInfo file in files) 
				{
					try 
					{
						System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
						doc.Load(file.FullName);

						DataRow dr = dt.NewRow();
						dr["Theme"] = doc.DocumentElement.Attributes["theme"].Value;
						dr["FileName"] = file.Name;
						dt.Rows.Add(dr);
					}
					catch(Exception) 
					{
					}
				}
				return dt;
			}
		}

		public static DataTable Languages() 
		{
			using(DataTable dt = new DataTable("Languages")) 
			{
				dt.Columns.Add("Language",typeof(string));
				dt.Columns.Add("FileName",typeof(string));
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Web.HttpContext.Current.Request.MapPath(String.Format("{0}languages",ForumRoot)));
				System.IO.FileInfo[] files = dir.GetFiles("*.xml");
				foreach(System.IO.FileInfo file in files) 
				{
					try 
					{
						System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
						doc.Load(file.FullName);
						DataRow dr = dt.NewRow();
						dr["Language"] = doc.DocumentElement.Attributes["language"].Value;
						dr["FileName"] = file.Name;
						dt.Rows.Add(dr);
					}
					catch(Exception) 
					{
					}
				}
				return dt;
			}
		}
		static public bool IsLocal 
		{
			get 
			{
				string s = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
				return s!=null && s.ToLower()=="localhost";
			}
		}

		#region Version Information
		static public string AppVersionNameFromCode(long code) 
		{
			if((code & 0xFF)>0)
				return String.Format("{0}.{1}.{2}.{3}",(code>>24) & 0xFF,(code>>16) & 0xFF,(code>>8) & 0xFF,code & 0xFF);
			else
				return String.Format("{0}.{1}.{2}",(code>>24) & 0xFF,(code>>16) & 0xFF,(code>>8) & 0xFF);
		}
		static public string AppVersionName 
		{
			get 
			{
				return AppVersionNameFromCode(AppVersionCode);
			}
		}
		static public int AppVersion 
		{
			get 
			{
				return 15;
			}
		}
		static public long AppVersionCode 
		{
			get 
			{
				return 0x00090900;
			}
		}
		static public DateTime AppVersionDate 
		{
			get 
			{
				return new DateTime(2004,11,10);
			}
		}
		#endregion
	}
}
