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
using System.Net;
using System.Net.Sockets;

/// <summary>
///     The IP Helper Class.
/// </summary>
public static class IPHelper
{
    /// <summary>
    /// The networks of the reverse proxies that are always trusted (loopback and private networks).
    /// </summary>
    private readonly static List<string> TrustedProxyNetworks = [
        "127.0.0.0/8",
        "10.0.0.0/8",
        "172.16.0.0/12",
        "192.168.0.0/16",
        "::1/128",
        "fc00::/7"
    ];

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
        if (inputIpAddress.IsSet() && inputIpAddress.ToLower().Contains("*"))
        {
            return inputIpAddress;
        }

        try
        {
            var addresses = Dns.GetHostAddresses(inputIpAddress);
            // attempt to find IPv4 address from input IP - may fail because IPv6 does not map to IPv4 under most circumstances
            var address = addresses
                .Find(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork);

            if (address != null)
            {
                return address.ToString();
            }

            // return IPv6 if IPv4 not found
            address = addresses
                .Find(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetworkV6);

            if (address != null)
            {
                return address.ToString();
            }

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
    public static string GetUserRealIPAddress(this HttpRequest httpRequest)
    {
        return new HttpRequestWrapper(httpRequest).GetUserRealIPAddress();
    }

    /// <summary>
    /// Gets the client address. The X-Forwarded-For and X-Real-IP headers are only used for requests of
    /// trusted proxies (loopback and private networks, and the proxies of the YAF.TrustedProxies setting),
    /// the headers of other clients are ignored: they could send any address.
    /// </summary>
    /// <param name="httpRequest">The HTTP request.</param>
    /// <returns>
    /// Client IP
    /// </returns>
    /// <see cref="http://wiki.nginx.org/HttpRealipModule" />
    /// <see cref="http://en.wikipedia.org/wiki/X-Forwarded-For" />
    public static string GetUserRealIPAddress(this HttpRequestBase httpRequest)
    {
        CodeContracts.ThrowIfNull(httpRequest);

        if (!IPAddress.TryParse(httpRequest.UserHostAddress ?? string.Empty, out var remoteAddress))
        {
            return httpRequest.UserHostAddress ?? string.Empty;
        }

        remoteAddress = Normalize(remoteAddress);

        if (!IsTrustedProxy(remoteAddress))
        {
            return remoteAddress.ToString();
        }

        var forwardedFor = httpRequest.Headers["X-Forwarded-For"];

        if (forwardedFor.IsSet())
        {
            // the proxies add the address of their client at the end, all trusted proxies of the
            // chain are skipped, up to the first untrusted address
            var clientAddress = remoteAddress;

            foreach (var entry in forwardedFor.Split(',').Select(e => e.Trim()).Reverse())
            {
                if (!IPAddress.TryParse(entry, out var address))
                {
                    break;
                }

                clientAddress = Normalize(address);

                if (!IsTrustedProxy(clientAddress))
                {
                    break;
                }
            }

            return clientAddress.ToString();
        }

        // proxies that only send X-Real-IP (e.g. nginx)
        var realIp = httpRequest.Headers["X-Real-IP"];

        return realIp.IsSet() && IPAddress.TryParse(
                   (realIp.Split(',').LastOrDefault() ?? string.Empty).Trim(),
                   out var realIpAddress)
                   ? Normalize(realIpAddress).ToString()
                   : remoteAddress.ToString();
    }

    /// <summary>
    /// Checks if the address is a trusted reverse proxy: an address of the loopback or a private network,
    /// or an address or network of the YAF.TrustedProxies setting.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Returns if the proxy is trusted.</returns>
    public static bool IsTrustedProxy(IPAddress address)
    {
        if (address is null)
        {
            return false;
        }

        address = Normalize(address);

        var addressBytes = address.GetAddressBytes();

        return TrustedProxyNetworks.Concat(Config.TrustedProxies.Split([','], StringSplitOptions.RemoveEmptyEntries))
            .Select(entry => entry.Trim())
            .Select(entry => entry.Contains("/") ? entry : $"{entry}/{(entry.Contains(":") ? 128 : 32)}")
            .Any(
                network => IPAddress.TryParse(network.Split('/')[0], out var networkAddress)
                           && networkAddress.AddressFamily == address.AddressFamily
                           && IsIpAddressInRange(addressBytes, network));
    }

    /// <summary>
    /// Verifies that an IP and mask aren't banned
    /// </summary>
    /// <param name="ban">
    /// Banned IP (an address, or an ipv4 mask with * for a part of the address)
    /// </param>
    /// <param name="chk">
    /// IP to Check
    /// </param>
    /// <returns>
    /// true if it's banned
    /// </returns>
    public static bool IsBanned(string ban, string chk)
    {
        CodeContracts.ThrowIfNull(ban);
        CodeContracts.ThrowIfNull(chk);

        var bannedIp = ban.Trim();
        var ip = chk.Trim();

        if (ip == "::1")
        {
            ip = "127.0.0.1";
        }

        if (bannedIp == "::1")
        {
            bannedIp = "127.0.0.1";
        }

        // masks are compared exactly (a wildcard * matches one part of an ipv4 address), invalid masks never match
        if (bannedIp.Contains("*"))
        {
            return TryGetIpV4Parts(ip, out var ipParts) && IsValidIpV4Mask(bannedIp) && bannedIp.Split('.')
                       .Select((part, index) => part == "*" || part == ipParts[index]).All(match => match);
        }

        return TryParseExact(bannedIp, out var banned) && TryParseExact(ip, out var address)
                                                       && Normalize(banned).Equals(Normalize(address));
    }

    /// <summary>
    /// Checks if the mask is a valid ban mask: an ip address, or an ipv4 mask with * for a part of the address.
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <returns>Returns if the mask is valid.</returns>
    public static bool IsValidBanMask(string mask)
    {
        if (mask.IsNotSet())
        {
            return false;
        }

        mask = mask.Trim();

        return mask.Contains("*") ? IsValidIpV4Mask(mask) : TryParseExact(mask, out _);
    }

    private static bool IsValidIpV4Mask(string mask)
    {
        var parts = mask.Split('.');

        return parts.Length == 4 && parts.All(part => part == "*" || byte.TryParse(part, out _));
    }

    private static bool TryGetIpV4Parts(string ip, out string[] parts)
    {
        parts = null;

        if (!TryParseExact(ip, out var address))
        {
            return false;
        }

        address = Normalize(address);

        if (address.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        parts = address.ToString().Split('.');
        return true;
    }

    private static bool TryParseExact(string value, out IPAddress address)
    {
        address = null;

        if (value.IsNotSet())
        {
            return false;
        }

        // IPAddress.TryParse accepts incomplete addresses like "1.2.3"
        if (!value.Contains(":") && value.Split('.').Length != 4)
        {
            return false;
        }

        return IPAddress.TryParse(value, out address);
    }

    private static IPAddress Normalize(IPAddress address)
    {
        return address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
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

        var ipAddressSplit = reservedIpAddress.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

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
                    maskBytes = BitConverter.GetBytes(mask).ToArray();

                    Array.Reverse(maskBytes);
            }

                break;
            case AddressFamily.InterNetworkV6:
                {
                    var bitArray = new BitArray(128, false);
                    ShiftRight(bitArray, bits, true);
                    maskBytes = ConvertToByteArray(bitArray).ToArray();

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
        CodeContracts.ThrowIfNull(ipAddress);

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