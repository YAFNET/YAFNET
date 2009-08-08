using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Utils
{
	public static class IPHelper
	{
		/// <summary>
		/// Converts an array of strings into a ulong representing a 4 byte IP address
		/// </summary>
		/// <param name="ip">string array of numbers</param>
		/// <returns>ulong represending an encoding IP address</returns>
		static public ulong Str2IP(String[] ip)
		{
			if (ip.Length != 4)
			{
				throw new Exception("Invalid ip address.");
			}

			ulong num = 0, tNum;
			for (int i = 0; i < ip.Length; i++)
			{
				num <<= 8;
				if (ulong.TryParse(ip[i], out tNum))
				{
					num |= tNum;
				}
			}

			return num;
		}

		static public ulong IPStrToLong(string ipAddress)
		{
			// not sure why it gives me this for local users on firefox--but it does...
			if (ipAddress == "::1") ipAddress = "127.0.0.1";

			string[] ip = ipAddress.Split('.');
			return Str2IP(ip);
		}

		/// <summary>
		/// Verifies that an ip and mask aren't banned
		/// </summary>
		/// <param name="ban">Banned IP</param>
		/// <param name="chk">IP to Check</param>
		/// <returns>true if it's banned</returns>
		static public bool IsBanned(string ban, string chk)
		{
			string bannedIP = ban.Trim();
			if (chk == "::1") chk = "127.0.0.1";

			String[] ipmask = bannedIP.Split('.');
			String[] ip = bannedIP.Split('.');

			for (int i = 0; i < ipmask.Length; i++)
			{
				if (ipmask[i] == "*")
				{
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
