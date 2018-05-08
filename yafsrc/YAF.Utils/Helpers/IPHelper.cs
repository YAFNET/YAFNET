/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
    ///     The IP Helper Class.
    /// </summary>
    public static class IPHelper
    {
        #region Static Fields

        /// <summary>
        /// The non routable IP v4 networks.
        /// </summary>
        private static readonly List<string> NonRoutableIPv4Networks = new List<string>
                                                                           {
                                                                               "10.0.0.0/8",
                                                                               "172.16.0.0/12",
                                                                               "192.168.0.0/16",
                                                                               "169.254.0.0/16",
                                                                               "127.0.0.0/8",
                                                                               "0.0.0.0/8"
                                                                           };

        /// <summary>
        /// The non routable IP v6 networks.
        /// </summary>
        private static readonly List<string> NonRoutableIPv6Networks = new List<string>
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
        /// Converts IP v6 or hostname to IP v4.
        /// </summary>
        /// <param name="addressIpv6">The address IP v6.</param>
        /// <returns>
        /// IP v4 address string.
        /// </returns>
        public static string GetIp4Address(string addressIpv6)
        {
            var ip4Address = string.Empty;

            // don't resolve nntp
            if (addressIpv6.IsSet() && addressIpv6.ToLower().Contains("nntp"))
            {
                return addressIpv6;
            }

            try
            {
                // Loop through all address InterNetwork - Address for IP version 4))
                foreach (
                    var ipAddress in
                        Dns.GetHostAddresses(addressIpv6)
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
                foreach (
                    var ipAddress in
                        Dns.GetHostAddresses(Dns.GetHostName())
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
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// Client IP
        /// </returns>
        /// <see cref="http://wiki.nginx.org/HttpRealipModule" />
        /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For" />
        /// <see cref="http://dev.opera.com/articles/view/opera-mini-request-headers/#x-forwarded-for" />
        public static string GetUserRealIPAddress([NotNull] this HttpRequest httpRequest)
        {
            CodeContracts.VerifyNotNull(httpRequest, "httpRequest");

            return new HttpRequestWrapper(httpRequest).GetUserRealIPAddress();
        }

        /// <summary>
        /// Gets User IP considering X-Forwarded-For and X-Real-IP HTTP headers
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// Client IP
        /// </returns>
        /// <see cref="http://wiki.nginx.org/HttpRealipModule" />
        /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For" />
        /// <see cref="http://dev.opera.com/articles/view/opera-mini-request-headers/#x-forwarded-for" />
        public static string GetUserRealIPAddress([NotNull] this HttpRequestBase httpRequest)
        {
            CodeContracts.VerifyNotNull(httpRequest, "httpRequest");

            IPAddress ipAddress;
            var ipString = httpRequest.Headers["X-Forwarded-For"];

            if (ipString.IsSet())
            {
                var ipAddresses = ipString.Split(',');
                var firstNonLocalAddress =
                    ipAddresses.FirstOrDefault(
                        ip => IPAddress.TryParse(ipString.Split(',')[0].Trim(), out ipAddress) && ipAddress.IsRoutable());

                if (!string.IsNullOrEmpty(firstNonLocalAddress))
                {
                    return firstNonLocalAddress;
                }
            }

            ipString = httpRequest.Headers["X-Real-IP"];

            if (!ipString.IsSet())
            {
                return httpRequest.UserHostAddress;
            }

            return IPAddress.TryParse((ipString.Split(',').LastOrDefault() ?? string.Empty).Trim(), out ipAddress)
                       ? ipString
                       : httpRequest.UserHostAddress;
        }

        /// <summary>
        /// The IP String to long.
        /// </summary>
        /// <param name="ipAddress">
        /// The IP address.
        /// </param>
        /// <returns>
        /// The IP as long.
        /// </returns>
        public static ulong IPStringToLong([NotNull] string ipAddress)
        {
            // not sure why it gives me this for local users on firefox--but it does...
            CodeContracts.VerifyNotNull(ipAddress, "ipAddress");

            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            var ip = ipAddress.Split('.');
            return StringToIP(ip);
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

            var bannedIP = ban.Trim();

            if (chk == "::1")
            {
                chk = "127.0.0.1";
            }

            if (bannedIP == "::1")
            {
                bannedIP = "127.0.0.1";
            }

            // handle IP v6 Addresses
            var splitCharBannedIp = bannedIP.Contains(".") ? '.' : ':';
            var splitCharChk = chk.Contains(".") ? '.' : ':';

            var ipmask = bannedIP.Split(splitCharBannedIp);
            var ip = bannedIP.Split(splitCharBannedIp);

            for (var i = 0; i < ipmask.Length; i++)
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

            var banmask = StringToIP(ip);
            var banchk = StringToIP(ipmask);
            var ipchk = StringToIP(chk.Split(splitCharChk));

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
        public static ulong StringToIP([NotNull] string[] ip)
        {
            CodeContracts.VerifyNotNull(ip, "ip");

            if (ip.Length != 4)
            {
                if (ip.Length != 8)
                {
                    throw new ArgumentOutOfRangeException("ip", "Invalid ip address.");
                }
            }

            ulong num = 0;

            foreach (var section in ip)
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
        /// Converts to byte array.
        /// </summary>
        /// <param name="bitArray">The bit array.</param>
        /// <returns>Returns the byte array</returns>
        private static IEnumerable<byte> ConvertToByteArray(BitArray bitArray)
        {
            // pack (in this case, using the first bool as the lsb - if you want
            // the first bool as the msb, reverse things ;-p)
            var bytes = (bitArray.Length + 7) / 8;
            var arr2 = new byte[bytes];
            var bitIndex = 0;
            var byteIndex = 0;

            for (var i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    arr2[byteIndex] |= (byte)(1 << bitIndex);
                }

                bitIndex++;

                if (bitIndex != 8)
                {
                    continue;
                }

                bitIndex = 0;
                byteIndex++;
            }

            return arr2;
        }

        /// <summary>
        /// Determines whether [is ip address in range] [the specified ip address bytes].
        /// </summary>
        /// <param name="ipAddressBytes">The ip address bytes.</param>
        /// <param name="reservedIpAddress">The reserved ip address.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        /// <see cref="http://stackoverflow.com/questions/8230728/is-there-a-function-that-can-take-an-ipaddress-as-string-and-tell-me-if-its-a-no" />
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

            var ipAddressSplit = reservedIpAddress.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (ipAddressSplit.Length != 2)
            {
                return false;
            }

            var ipAddressRange = ipAddressSplit[0];
            IPAddress ipAddress;

            if (!IPAddress.TryParse(ipAddressRange, out ipAddress))
            {
                return false;
            }

            var ipBytes = ipAddress.GetAddressBytes();
            int bits;
            if (!int.TryParse(ipAddressSplit[1], out bits))
            {
                bits = 0;
            }

            byte[] maskBytes = null;

            switch (ipAddress.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    {
                        var mask = ~(uint.MaxValue >> bits);
                        maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray();
                    }
                    break;
                case AddressFamily.InterNetworkV6:
                    {
                        var bitArray = new BitArray(128, false);
                        ShiftRight(bitArray, bits, true);
                        maskBytes = ConvertToByteArray(bitArray).Reverse().ToArray();
                    }
                    break;
            }

            var result = true;
            for (var i = 0; i < ipBytes.Length; i++)
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
            var ipAddressBytes = ipAddress.GetAddressBytes();

            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                return !NonRoutableIPv4Networks.Any(network => IsIpAddressInRange(ipAddressBytes, network));
            }

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return !NonRoutableIPv6Networks.Any(network => IsIpAddressInRange(ipAddressBytes, network));
            }

            return false;
        }

        /// <summary>
        /// Shifts the right.
        /// </summary>
        /// <param name="bitArray">The bit array.</param>
        /// <param name="shiftN">The shift n.</param>
        /// <param name="fillValue">if set to <c>true</c> [fill value].</param>
        private static void ShiftRight(BitArray bitArray, int shiftN, bool fillValue)
        {
            for (var i = shiftN; i < bitArray.Count; i++)
            {
                bitArray[i - shiftN] = bitArray[i];
            }

            for (var index = bitArray.Count - shiftN; index < bitArray.Count; index++)
            {
                bitArray[index] = fillValue;
            }
        }

        #endregion
    }
}