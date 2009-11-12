using System;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The ip helper.
  /// </summary>
  public static class IPHelper
  {
    /// <summary>
    /// Converts an array of strings into a ulong representing a 4 byte IP address
    /// </summary>
    /// <param name="ip">
    /// string array of numbers
    /// </param>
    /// <returns>
    /// ulong represending an encoding IP address
    /// </returns>
    public static ulong Str2IP(string[] ip)
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

    /// <summary>
    /// The ip str to long.
    /// </summary>
    /// <param name="ipAddress">
    /// The ip address.
    /// </param>
    /// <returns>
    /// The ip str to long.
    /// </returns>
    public static ulong IPStrToLong(string ipAddress)
    {
      // not sure why it gives me this for local users on firefox--but it does...
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
    public static bool IsBanned(string ban, string chk)
    {
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
  }
}