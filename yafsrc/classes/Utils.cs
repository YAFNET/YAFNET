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
using System.Data.SqlClient;

namespace yaf
{
	public class User 
	{
		static public long ValidateUser(string username,string email) 
		{
			try 
			{
				using(SqlCommand cmd = new SqlCommand("yaf_user_extvalidate")) 
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@Name",username);
					cmd.Parameters.Add("@Email",email);
					return (long)DataManager.ExecuteScalar(cmd);
				}
			}
			catch(Exception) 
			{
				return 0;
			}
		}
	}

	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils {
		static public ulong Str2IP(String[] ip) {
			if(ip.Length!=4)
				throw new Exception("Invalid ip address.");

			ulong num = 0;
			for(int i=0;i<ip.Length;i++) {
				num <<= 8;
				num |= ulong.Parse(ip[i]);
			}
			return num;
		}

		static public bool IsBanned(string ban,string chk) {
			String[] ipmask = ban.Split('.');
			String[] ip = ban.Split('.');
			for(int i=0;i<ipmask.Length;i++) {
				if(ipmask[i]=="*") {
					ipmask[i] = "0";
					ip[i] = "0";
				}
				else
					ipmask[i] = "255";
			}

			ulong banmask = Str2IP(ip);
			ulong banchk = Str2IP(ipmask);
			ulong ipchk = Str2IP(chk.Split('.'));
			
			return (ipchk & banchk) == banmask;
		}
	}
}
