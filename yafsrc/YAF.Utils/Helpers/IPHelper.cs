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
namespace YAF.Utils.Helpers
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    ///     The ip helper.
    /// </summary>
    public static class IPHelper
    {
        #region Static Fields

        /// <summary>
        /// The non routable i pv 4 networks.
        /// </summary>
        private static readonly List<string> nonRoutableIPv4Networks = new List<string>
                                                                           {
                                                                               "10.0.0.0/8", 
                                                                               "172.16.0.0/12", 
                                                                               "192.168.0.0/16", 
                                                                               "169.254.0.0/16", 
                                                                               "127.0.0.0/8", 
                                                                               "0.0.0.0/8"
                                                                           };

        /// <summary>
        /// The non routable i pv 6 networks.
        /// </summary>
        private static readonly List<string> nonRoutableIPv6Networks = new List<string>
                                                                           {
                                                                               "::/128", 
                                                                               "::1/128", 
                                                                               "2001:db8::/32", 
                                                                               "fc00::/7", 
                                                                               "::ffff:0:0/96"
                                                                           };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Converts Ipv6 or hostname to IpV4.
        /// </summary>
        /// <param name="addressIpv6">
        /// </param>
        /// <returns>
        /// IPv4 address string.
        /// </returns>
        public static string GetIp4Address(string addressIpv6)
        {
            string ip4Address = string.Empty;

            // don't resolve nntp
            if (addressIpv6.IsSet() && addressIpv6.ToLower().Contains("nntp"))
            {
                return addressIpv6;
            }

            try
            {
                // Loop through all address InterNetwork - Address for IP version 4))
                foreach (var ipAddress in Dns.GetHostAddresses(addressIpv6)
                    .Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork))
                {
                    ip4Address = ipAddress.ToString();
                    break;
                }

                if (ip4Address.IsSet())
                {
                    return ip4Address;
                }

                // to find by host name - is not in use so far. 
                foreach (var ipAddress in Dns.GetHostAddresses(Dns.GetHostName())
                    .Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork))
                {
                    ip4Address = ipAddress.ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetIp4Address: {0}", ex);
            }

            return ip4Address;
        }

        /// <summary>
        /// Gets User IP considering X-Forwarded-For and X-Real-IP HTTP headers
        /// </summary>
        /// <see cref="http://wiki.nginx.org/HttpRealipModule"/>
        /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For"/>
        /// <see cref="http://dev.opera.com/articles/view/opera-mini-request-headers/#x-forwarded-for"/>
        /// <param name="httpRequest">
        /// </param>
        /// <returns>
        /// Client ip
        /// </returns>
        public static string GetUserRealIPAddress([NotNull] this HttpRequest httpRequest)
        {
            CodeContracts.VerifyNotNull(httpRequest, "httpRequest");

            return new HttpRequestWrapper(httpRequest).GetUserRealIPAddress();
        }

        /// <summary>
        /// Gets User IP considering X-Forwarded-For and X-Real-IP HTTP headers
        /// </summary>
        /// <see cref="http://wiki.nginx.org/HttpRealipModule"/>
        /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For"/>
        /// <see cref="http://dev.opera.com/articles/view/opera-mini-request-headers/#x-forwarded-for"/>
        /// <param name="httpRequest">
        /// </param>
        /// <returns>
        /// Client ip
        /// </returns>
        public static string GetUserRealIPAddress([NotNull] this HttpRequestBase httpRequest)
        {
            CodeContracts.VerifyNotNull(httpRequest, "httpRequest");

            IPAddress ipAddress;
            string ipString = httpRequest.Headers["X-Forwarded-For"];

            if (ipString.IsSet())
            {
                string[] ipAddresses = ipString.Split(',');
                string firstNonLocalAddress =
                    ipAddresses.FirstOrDefault(ip => 
                        IPAddress.TryParse(ipString.Split(',')[0].Trim(), out ipAddress) && ipAddress.IsRoutable());

                if (!string.IsNullOrEmpty(firstNonLocalAddress))
                {
                    return firstNonLocalAddress;
                }
            }

            ipString = httpRequest.Headers["X-Real-IP"];
            if (ipString.IsSet())
            {
                if (IPAddress.TryParse((ipString.Split(',').LastOrDefault() ?? string.Empty).Trim(), out ipAddress))
                {
                    return ipString;
                }
            }

            return httpRequest.UserHostAddress;
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
        public static ulong IPStrToLong([NotNull] string ipAddress)
        {
            // not sure why it gives me this for local users on firefox--but it does...
            CodeContracts.VerifyNotNull(ipAddress, "ipAddress");

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
            CodeContracts.VerifyNotNull(ban, "ban");
            CodeContracts.VerifyNotNull(chk, "chk");

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
            CodeContracts.VerifyNotNull(ip, "ip");

            if (ip.Length != 4)
            {
                throw new ArgumentOutOfRangeException("ip", "Invalid ip address.");
            }

            ulong num = 0;

            foreach (string section in ip)
            {
                num <<= 8;
                ulong result;
                if (ulong.TryParse(section, out result))
                {
                    num |= result;
                }
            }

            return num;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="bitArray">
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private static byte[] ConvertToByteArray(BitArray bitArray)
        {
            // pack (in this case, using the first bool as the lsb - if you want
            // the first bool as the msb, reverse things ;-p)
            int bytes = (bitArray.Length + 7) / 8;
            var arr2 = new byte[bytes];
            int bitIndex = 0;
            int byteIndex = 0;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    arr2[byteIndex] |= (byte)(1 << bitIndex);
                }

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return arr2;
        }

        /// <summary>
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/8230728/is-there-a-function-that-can-take-an-ipaddress-as-string-and-tell-me-if-its-a-no"/>
        /// <param name="ipAddressBytes">
        /// </param>
        /// <param name="reservedIpAddress">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsIpAddressInRange(byte[] ipAddressBytes, string reservedIpAddress)
        {
            if (reservedIpAddress.IsNotSet())
            {
                return false;
            }

            if (ipAddressBytes == null)
            {
                return false;
            }

            string[] ipAddressSplit = reservedIpAddress.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (ipAddressSplit.Length != 2)
            {
                return false;
            }

            string ipAddressRange = ipAddressSplit[0];
            IPAddress ipAddress = null;

            if (!IPAddress.TryParse(ipAddressRange, out ipAddress))
            {
                return false;
            }

            byte[] ipBytes = ipAddress.GetAddressBytes();
            int bits = 0;
            if (!int.TryParse(ipAddressSplit[1], out bits))
            {
                bits = 0;
            }

            byte[] maskBytes = null;
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                uint mask = ~(uint.MaxValue >> bits);
                maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray();
            }
            else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                var bitArray = new BitArray(128, false);
                ShiftRight(bitArray, bits, true);
                maskBytes = ConvertToByteArray(bitArray).Reverse().ToArray();
            }

            bool result = true;
            for (int i = 0; i < ipBytes.Length; i++)
            {
                result &= (byte)(ipAddressBytes[i] & maskBytes[i]) == ipBytes[i];
            }

            return result;
        }

        /// <summary>
        /// Is this ip address non-routable?
        /// </summary>
        /// <param name="ipAddress">
        /// The ip address.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsRoutable([NotNull] this IPAddress ipAddress)
        {
            CodeContracts.VerifyNotNull(ipAddress, "ipAddress");

            // Reference: http://en.wikipedia.org/wiki/Reserved_IP_addresses
            byte[] ipAddressBytes = ipAddress.GetAddressBytes();

            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                return !nonRoutableIPv4Networks.Any(network => IsIpAddressInRange(ipAddressBytes, network));
            }

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return !nonRoutableIPv6Networks.Any(network => IsIpAddressInRange(ipAddressBytes, network));
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="bitArray">
        /// </param>
        /// <param name="shiftN">
        /// </param>
        /// <param name="fillValue">
        /// </param>
        private static void ShiftRight(BitArray bitArray, int shiftN, bool fillValue)
        {
            for (int i = shiftN; i < bitArray.Count; i++)
            {
                bitArray[i - shiftN] = bitArray[i];
            }

            for (int index = bitArray.Count - shiftN; index < bitArray.Count; index++)
            {
                bitArray[index] = fillValue;
            }
        }

        #endregion
    }
}