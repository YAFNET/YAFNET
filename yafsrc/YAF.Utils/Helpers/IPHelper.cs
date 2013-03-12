/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace YAF.Utils.Helpers
{
  #region Using

  using System;

  using YAF.Types;
  using YAF.Types.Extensions;

    #endregion

  /// <summary>
  /// The ip helper.
  /// </summary>
  public static class IPHelper
  {
    #region Public Methods

    /// <summary>
    /// The ip str to long.
    /// </summary>
    /// <param name="ipAddress">
    /// The ip address.
    /// </param>
    /// <returns>
    /// The ip str to long.
    /// </returns>
    public static ulong IPStrToLong([NotNull] string ipAddress)
    {
      // not sure why it gives me this for local users on firefox--but it does...
      CodeContracts.ArgumentNotNull(ipAddress, "ipAddress");

      if (ipAddress == "::1")
      {
        ipAddress = "127.0.0.1";
      }

      string[] ip = ipAddress.Split('.');
      return Str2IP(ip);
    }

    /// <summary>
    /// Verifies that an ip and mask aren't banned
    /// </summary>
    /// <param name="ban">
    /// Banned IP
    /// </param>
    /// <param name="chk">
    /// IP to Check
    /// </param>
    /// <returns>
    /// true if it's banned
    /// </returns>
    public static bool IsBanned([NotNull] string ban, [NotNull] string chk)
    {
      CodeContracts.ArgumentNotNull(ban, "ban");
      CodeContracts.ArgumentNotNull(chk, "chk");

      string bannedIP = ban.Trim();
      if (chk == "::1")
      {
        chk = "127.0.0.1";
      }

      string[] ipmask = bannedIP.Split('.');
      string[] ip = bannedIP.Split('.');

      for (int i = 0; i < ipmask.Length; i++)
      {
        if (ipmask[i] == "*")
        {
          ipmask[i] = "0";
          ip[i] = "0";
        }
        else
        {
          ipmask[i] = "255";
        }
      }

      ulong banmask = Str2IP(ip);
      ulong banchk = Str2IP(ipmask);
      ulong ipchk = Str2IP(chk.Split('.'));

      return (ipchk & banchk) == banmask;
    }

    /// <summary>
    /// Converts an array of strings into a ulong representing a 4 byte IP address
    /// </summary>
    /// <param name="ip">
    /// string array of numbers
    /// </param>
    /// <returns>
    /// ulong represending an encoding IP address
    /// </returns>
    public static ulong Str2IP([NotNull] string[] ip)
    {
      CodeContracts.ArgumentNotNull(ip, "ip");

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

  /// <summary>
  /// Converts Ipv6 or hostname to IpV4.
  /// </summary>
  /// <param name="addressIpv6"></param>
  /// <returns>IPv4 address string.</returns>
  public static string GetIp4Address(string addressIpv6)
  {
    string ip4Address = String.Empty;

    // don't resolve nntp
    if (addressIpv6.IsSet() && addressIpv6.ToLower().Contains("nntp")) return addressIpv6;

    try
    {
        // Loop through all address InterNetwork - Address for IP version 4))
        foreach (var ipAddress in Dns.GetHostAddresses(addressIpv6).Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork))
        {
            ip4Address = ipAddress.ToString();
            break;
        }

        if (ip4Address.IsSet())
        {
            return ip4Address;
        }

        // to find by host name - is not in use so far. 
        foreach (var ipAddress in Dns.GetHostAddresses(Dns.GetHostName()).Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork))
        {
            ip4Address = ipAddress.ToString();
            break;
        }
    }
    catch (Exception)
    {
        // TODO: log.
        
    }

    return ip4Address;
  }


    #endregion
  }
}