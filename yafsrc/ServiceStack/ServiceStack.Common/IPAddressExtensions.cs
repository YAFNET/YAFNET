// ***********************************************************************
// <copyright file="IPAddressExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !NETFX_CORE
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServiceStack
{
    /// <summary>
    /// Useful IPAddressExtensions from:
    /// http://blogs.msdn.com/knom/archive/2008/12/31/ip-address-calculations-with-c-subnetmasks-networks.aspx
    /// </summary>
    public static class IPAddressExtensions
    {
        /// <summary>
        /// Gets the broadcast address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns>IPAddress.</returns>
        /// <exception cref="System.ArgumentException">Lengths of IP address and subnet mask do not match.</exception>
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// Gets the network address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns>IPAddress.</returns>
        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            return new IPAddress(GetNetworkAddressBytes(ipAdressBytes, subnetMaskBytes));
        }

        /// <summary>
        /// Gets the network address bytes.
        /// </summary>
        /// <param name="ipAdressBytes">The ip adress bytes.</param>
        /// <param name="subnetMaskBytes">The subnet mask bytes.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="System.ArgumentException">Lengths of IP address and subnet mask do not match.</exception>
        public static byte[] GetNetworkAddressBytes(byte[] ipAdressBytes, byte[] subnetMaskBytes)
        {
            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & subnetMaskBytes[i]);
            }
            return broadcastAddress;
        }

        /// <summary>
        /// Determines whether [is in same ipv6 subnet] [the specified address].
        /// </summary>
        /// <param name="address2">The address2.</param>
        /// <param name="address">The address.</param>
        /// <returns><c>true</c> if [is in same ipv6 subnet] [the specified address]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentException">Both IPAddress must be IPV6 addresses</exception>
        public static bool IsInSameIpv6Subnet(this IPAddress address2, IPAddress address)
        {
            if (address2.AddressFamily != AddressFamily.InterNetworkV6 || address.AddressFamily != AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException("Both IPAddress must be IPV6 addresses");
            }
            var address1Bytes = address.GetAddressBytes();
            var address2Bytes = address2.GetAddressBytes();

            return IsInSameIpv6Subnet(address1Bytes, address2Bytes);
        }

        /// <summary>
        /// Determines whether [is in same ipv6 subnet] [the specified address2 bytes].
        /// </summary>
        /// <param name="address1Bytes">The address1 bytes.</param>
        /// <param name="address2Bytes">The address2 bytes.</param>
        /// <returns><c>true</c> if [is in same ipv6 subnet] [the specified address2 bytes]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentException">Lengths of IP addresses do not match.</exception>
        public static bool IsInSameIpv6Subnet(this byte[] address1Bytes, byte[] address2Bytes)
        {
            if (address1Bytes.Length != address2Bytes.Length)
                throw new ArgumentException("Lengths of IP addresses do not match.");

            for (var i = 0; i < 8; i++)
            {
                if (address1Bytes[i] != address2Bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is in same ipv4 subnet] [the specified address].
        /// </summary>
        /// <param name="address2">The address2.</param>
        /// <param name="address">The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns><c>true</c> if [is in same ipv4 subnet] [the specified address]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentException">Both IPAddress must be IPV4 addresses</exception>
        public static bool IsInSameIpv4Subnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            if (address2.AddressFamily != AddressFamily.InterNetwork || address.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException("Both IPAddress must be IPV4 addresses");
            }
            var network1 = address.GetNetworkAddress(subnetMask);
            var network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }

        /// <summary>
        /// Determines whether [is in same ipv4 subnet] [the specified address2 bytes].
        /// </summary>
        /// <param name="address1Bytes">The address1 bytes.</param>
        /// <param name="address2Bytes">The address2 bytes.</param>
        /// <param name="subnetMaskBytes">The subnet mask bytes.</param>
        /// <returns><c>true</c> if [is in same ipv4 subnet] [the specified address2 bytes]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentException">Lengths of IP addresses do not match.</exception>
        public static bool IsInSameIpv4Subnet(this byte[] address1Bytes, byte[] address2Bytes, byte[] subnetMaskBytes)
        {
            if (address1Bytes.Length != address2Bytes.Length)
                throw new ArgumentException("Lengths of IP addresses do not match.");

            var network1Bytes = GetNetworkAddressBytes(address1Bytes, subnetMaskBytes);
            var network2Bytes = GetNetworkAddressBytes(address2Bytes, subnetMaskBytes);

            return network1Bytes.AreEqual(network2Bytes);
        }

        /// <summary>
        /// Gets the ipv4 addresses from all Network Interfaces that have Subnet masks.
        /// </summary>
        /// <returns>Dictionary&lt;IPAddress, IPAddress&gt;.</returns>
        public static Dictionary<IPAddress, IPAddress> GetAllNetworkInterfaceIpv4Addresses()
        {
            var map = new Dictionary<IPAddress, IPAddress>();

            try
            {
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                        if (uipi.IPv4Mask == null) continue; //ignore 127.0.0.1
                        map[uipi.Address] = uipi.IPv4Mask;
                    }
                }
            }
            catch /*(NotImplementedException ex)*/
            {
                //log.Warn("MONO does not support NetworkInterface.GetAllNetworkInterfaces(). Could not detect local ip subnets.", ex);
            }
            return map;
        }

        /// <summary>
        /// Gets the ipv6 addresses from all Network Interfaces.
        /// </summary>
        /// <returns>List&lt;IPAddress&gt;.</returns>
        public static List<IPAddress> GetAllNetworkInterfaceIpv6Addresses()
        {
            var list = new List<IPAddress>();

            try
            {
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetworkV6) continue;
                        list.Add(uipi.Address);
                    }
                }
            }
            catch /*(NotImplementedException ex)*/
            {
                //log.Warn("MONO does not support NetworkInterface.GetAllNetworkInterfaces(). Could not detect local ip subnets.", ex);
            }

            return list;
        }

    }
}
#endif