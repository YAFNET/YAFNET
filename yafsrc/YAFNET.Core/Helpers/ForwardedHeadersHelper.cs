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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

/// <summary>
/// The client IP address behind reverse proxies: the forwarded headers (X-Forwarded-For, X-Real-IP) are
/// only used when the request comes from a trusted proxy, otherwise every client could send any address
/// (and bypass IP bans, the IP checks of guest posts, ...).
/// </summary>
public static class ForwardedHeadersHelper
{
    /// <summary>
    /// Loopback and private networks are always trusted (a proxy on the same host, in the LAN or in docker).
    /// </summary>
    private readonly static string[] DefaultTrustedNetworks =
    [
        "127.0.0.0/8",
        "10.0.0.0/8",
        "172.16.0.0/12",
        "192.168.0.0/16",
        "::1/128",
        "fc00::/7"
    ];

    /// <summary>
    /// The trusted networks, set by <see cref="CreateOptions"/>.
    /// </summary>
    private static IReadOnlyList<System.Net.IPNetwork> trustedNetworks = ParseNetworks([]);

    /// <summary>
    /// Creates the options of the forwarded headers middleware.
    /// </summary>
    /// <param name="trustedProxies">
    /// The additional trusted proxies (addresses or networks in CIDR notation) from the board configuration,
    /// e.g. the address ranges of Cloudflare.
    /// </param>
    /// <returns>The options.</returns>
    public static ForwardedHeadersOptions CreateOptions(IEnumerable<string> trustedProxies)
    {
        trustedNetworks = ParseNetworks(trustedProxies ?? []);

        var options = new ForwardedHeadersOptions
                      {
                          ForwardedHeaders = ForwardedHeaders.XForwardedFor,

                          // all trusted proxies of the chain are skipped, up to the first untrusted address
                          ForwardLimit = null
                      };

        options.KnownProxies.Clear();
        options.KnownIPNetworks.Clear();

        foreach (var network in trustedNetworks)
        {
            options.KnownIPNetworks.Add(network);
        }

        return options;
    }

    /// <summary>
    /// Checks if the address is a trusted proxy.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Returns if the forwarded headers of this address can be used.</returns>
    public static bool IsTrustedProxy(IPAddress address)
    {
        if (address is null)
        {
            return false;
        }

        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        return trustedNetworks.Any(network => network.Contains(address));
    }

    /// <summary>
    /// Parses the default and the configured networks, invalid entries are skipped.
    /// </summary>
    /// <param name="trustedProxies">The configured proxies.</param>
    /// <returns>The networks.</returns>
    private static List<System.Net.IPNetwork> ParseNetworks(IEnumerable<string> trustedProxies)
    {
        var networks = new List<System.Net.IPNetwork>();

        foreach (var entry in DefaultTrustedNetworks.Concat(trustedProxies).Select(e => e?.Trim()).Where(entry => !entry.IsNotSet()))
        {
            if (System.Net.IPNetwork.TryParse(entry, out var network))
            {
                networks.Add(network);
            }
            else if (IPAddress.TryParse(entry, out var address))
            {
                networks.Add(new System.Net.IPNetwork(address, address.GetAddressBytes().Length * 8));
            }
        }

        return networks;
    }
}
