/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Net.Sockets;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

/// <summary>
///     The IP Helper Class.
/// </summary>
public static class IPHelper
{
    /// <summary>
    /// The non routable IP v4 networks.
    /// </summary>
    private readonly static List<string> NonRoutableIPv4Networks = [
        "10.0.0.0/8",
        "172.16.0.0/12",
        "192.168.0.0/16",
        "169.254.0.0/16",
        "127.0.0.0/8",
        "0.0.0.0/8"
    ];

    /// <summary>
    /// The non routable IP v6 networks.
    /// </summary>
    private readonly static List<string> NonRoutableIPv6Networks = [
        "::/128",
        "::1/128",
        "2001:db8::/32",
        "fc00::/7",
        "::ffff:0:0/96"
    ];

    private readonly static char[] separator = ['/'];

    /// <summary>
    /// Attempts to get an IPv4 from IPv6 address - falls back to IPv6, then localhost.
    /// </summary>
    /// <param name="inputIpAddress">The input IP Address.</param>
    /// <returns>
    /// IPv4 address if found, else IPv6 address, else localhost.
    /// </returns>
    public static string GetIpAddressAsString(string inputIpAddress)
    {
        var ipAddressAsString = string.Empty;

        // don't resolve ip regex
        if (inputIpAddress.IsSet() && inputIpAddress.ToLower().Contains('*'))
        {
            return inputIpAddress;
        }

        try
        {
            var hostAddresses = Dns.GetHostAddresses(inputIpAddress);
            // attempt to find IPv4 address from input IP - may fail because IPv6 does not map to IPv4 under most circumstances
            var address =
                Array.Find(hostAddresses,
                    ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork);

            if (address != null)
            {
                return address.ToString();
            }

            // return IPv6 if IPv4 not found
            address = Array.Find(hostAddresses, ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetworkV6);

            return address != null ? address.ToString() : "127.0.0.1";
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILogger<IPAddress>>().Error(ex, "Exception in GetIpAddressAsString");
        }

        return ipAddressAsString;
    }

    /// <summary>
    /// Gets PageUser IP considering X-Forwarded-For and X-Real-IP HTTP headers
    /// </summary>
    /// <param name="httpRequest">The HTTP request.</param>
    /// <returns>
    /// Client IP
    /// </returns>
    /// <see cref="http://wiki.nginx.org/HttpRealipModule" />
    /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For" />
    /// <see cref="http://dev.opera.com/articles/view/opera-mini-request-headers/#x-forwarded-for" />
    public static string GetUserRealIPAddress(this HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);

        return httpRequest.HttpContext.GetUserRealIPAddress();
    }

    /// <summary>
    /// Gets PageUser IP considering X-Forwarded-For and X-Real-IP HTTP headers
    /// </summary>
    /// <returns>
    /// Client IP
    /// </returns>
    /// <see cref="http://wiki.nginx.org/HttpRealipModule" />
    /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For" />
    /// <see cref="http://dev.opera.com/articles/view/opera-mini-request-headers/#x-forwarded-for" />
    public static string GetUserRealIPAddress(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        IPAddress ipAddress;
        var ipString = httpContext.Request.Headers["X-Forwarded-For"].ToString();

        if (ipString.IsSet())
        {
            var ipAddresses = ipString.Split(',');
            var firstNonLocalAddress =
                Array.Find(ipAddresses,
                    ip => IPAddress.TryParse(ipString.Split(',', StringSplitOptions.TrimEntries)[0], out ipAddress) && ipAddress.IsRoutable());

            if (firstNonLocalAddress.IsSet())
            {
                return firstNonLocalAddress;
            }
        }

        ipString = httpContext.Request.Headers["X-Real-IP"].ToString();

        if (ipString.IsNotSet())
        {
            return httpContext.Connection.RemoteIpAddress.ToString();
        }

        return IPAddress.TryParse((ipString.Split(',', StringSplitOptions.TrimEntries).LastOrDefault() ?? string.Empty), out ipAddress)
                   ? ipString
                   : httpContext.Connection.RemoteIpAddress.ToString();
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
    public static bool IsBanned(string ban, string chk)
    {
        ArgumentNullException.ThrowIfNull(ban);
        ArgumentNullException.ThrowIfNull(chk);

        var bannedIp = ban.Trim();

        if (chk == "::1")
        {
            chk = "127.0.0.1";
        }

        if (bannedIp == "::1")
        {
            bannedIp = "127.0.0.1";
        }

        var check = Regex.Match(chk, bannedIp, RegexOptions.None, TimeSpan.FromMilliseconds(100));

        return check.Success;
    }

    /// <summary>
    /// Converts to byte array.
    /// </summary>
    /// <param name="bitArray">The bit array.</param>
    /// <returns>Returns the byte array</returns>
    private static byte[] ConvertToByteArray(BitArray bitArray)
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

#pragma warning disable S2583 // Conditionally executed code should be reachable
            if (bitIndex != 8)
            {
                continue;
            }
#pragma warning restore S2583 // Conditionally executed code should be reachable

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

        var ipAddressSplit = reservedIpAddress.Split(separator, StringSplitOptions.RemoveEmptyEntries);

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
                maskBytes = [.. BitConverter.GetBytes(mask)];

                Array.Reverse(maskBytes);
            }

                break;
            case AddressFamily.InterNetworkV6:
            {
                var bitArray = new BitArray(128, false);
                ShiftRight(bitArray, bits, true);
                maskBytes = [.. ConvertToByteArray(bitArray)];

                Array.Reverse(maskBytes);
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
    public static bool IsRoutable(this IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);

        // Reference: http://en.wikipedia.org/wiki/Reserved_IP_addresses
        var ipAddressBytes = ipAddress.GetAddressBytes();

        return ipAddress.AddressFamily switch
            {
                AddressFamily.InterNetwork => !NonRoutableIPv4Networks.Exists(
                                                  network => IsIpAddressInRange(ipAddressBytes, network)),
                AddressFamily.InterNetworkV6 => !NonRoutableIPv6Networks.Exists(
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