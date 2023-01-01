/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Helpers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

/// <summary>
///     The IP Helper Class.
/// </summary>
public static class IPHelper
{
    /// <summary>
    /// The non routable IP v4 networks.
    /// </summary>
    private static readonly List<string> NonRoutableIPv4Networks = new()
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
    private static readonly List<string> NonRoutableIPv6Networks = new()
                                                                       {
                                                                           "::/128",
                                                                           "::1/128",
                                                                           "2001:db8::/32",
                                                                           "fc00::/7",
                                                                           "::ffff:0:0/96"
                                                                       };

    /// <summary>
    /// Attempts to get an IPv4 from IPv6 address - falls back to IPv6, then localhost.
    /// </summary>
    /// <param name="inputIpAddress">The input IP Address.</param>
    /// <returns>
    /// IPv4 address if found, else IPv6 address, else localhost.
    /// </returns>
    public static string GetIpAddressAsString([CanBeNull]string inputIpAddress)
    {
        var ipAddressAsString = string.Empty;

        // don't resolve nntp
        if (inputIpAddress.IsSet() && inputIpAddress.ToLower().Contains("nntp"))
        {
            return ipAddressAsString;
        }

        // don't resolve ip regex
        if (inputIpAddress.IsSet() && inputIpAddress.ToLower().Contains("*"))
        {
            return ipAddressAsString;
        }

        try
        {
            // attempt to find IPv4 address from input IP - may fail because IPv6 does not map to IPv4 under most circumstances
            var address = Dns.GetHostAddresses(inputIpAddress)
                .FirstOrDefault(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork);

            if (address != null)
            {
                return address.ToString();
            }

            // return IPv6 if IPv4 not found
            address = Dns.GetHostAddresses(inputIpAddress)
                .FirstOrDefault(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetworkV6);

            if (address != null)
            {
                return address.ToString();
            }

            // to find by host name - is not in use so far (does not work properly via rDNSing a host from it's IPv6 to IPv4).
            // address = Dns.GetHostAddresses(Dns.GetHostName())
            //     .FirstOrDefault(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork);


            // return localhost if no IP address found or detected (prevents server IP from being listed as user IP
            // we should never get here -- connections to server ALWAYS return some form of remote IP address
            return "127.0.0.1";


        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILoggerService>().Log("Exception in GetIpAddressAsString", exception: ex);
        }


        return ipAddressAsString;
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
        CodeContracts.VerifyNotNull(httpRequest);

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
        CodeContracts.VerifyNotNull(httpRequest);

        IPAddress ipAddress;
        var ipString = httpRequest.Headers["X-Forwarded-For"];

        if (ipString.IsSet())
        {
            var ipAddresses = ipString.Split(',');
            var firstNonLocalAddress =
                ipAddresses.FirstOrDefault(
                    ip => IPAddress.TryParse(ipString.Split(',')[0].Trim(), out ipAddress) && ipAddress.IsRoutable());

            if (firstNonLocalAddress.IsSet())
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
    /// Verifies that an IP and mask aren't banned
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
        CodeContracts.VerifyNotNull(ban);
        CodeContracts.VerifyNotNull(chk);

        var bannedIP = ban.Trim();

        if (chk == "::1")
        {
            chk = "127.0.0.1";
        }

        if (bannedIP == "::1")
        {
            bannedIP = "127.0.0.1";
        }

        if (bannedIP.Contains("*"))
        {
            bannedIP = bannedIP.Replace("*", "0");
        }


        var ipCheck = StringToIP(chk);

        var banCheck = StringToIP(bannedIP);

        return banCheck.Equals(ipCheck);
    }

    /// <summary>
    /// Converts an array of strings into a ulong representing a 4 byte IP address
    /// </summary>
    /// <param name="ip">
    /// string array of numbers
    /// </param>
    /// <returns>
    /// ulong representing an encoding IP address
    /// </returns>
    public static IPAddress StringToIP([NotNull] string ip)
    {
        CodeContracts.VerifyNotNull(ip);

        return IPAddress.Parse(ip);
    }

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
    private static bool IsIpAddressInRange(IReadOnlyList<byte> ipAddressBytes, string reservedIpAddress)
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

        if (!IPAddress.TryParse(ipAddressRange, out var ipAddress))
        {
            return false;
        }

        var ipBytes = ipAddress.GetAddressBytes();
        if (!int.TryParse(ipAddressSplit[1], out var bits))
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
        CodeContracts.VerifyNotNull(ipAddress);

        // Reference: http://en.wikipedia.org/wiki/Reserved_IP_addresses
        var ipAddressBytes = ipAddress.GetAddressBytes();

        return ipAddress.AddressFamily switch
            {
                AddressFamily.InterNetwork => !NonRoutableIPv4Networks.Any(
                                                  network => IsIpAddressInRange(ipAddressBytes, network)),
                AddressFamily.InterNetworkV6 => !NonRoutableIPv6Networks.Any(
                                                    network => IsIpAddressInRange(ipAddressBytes, network)),
                _ => false
            };
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
}