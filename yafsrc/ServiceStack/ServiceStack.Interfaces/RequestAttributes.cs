// ***********************************************************************
// <copyright file="RequestAttributes.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack
{
    /// <summary>
    /// Enum RequestAttributes
    /// </summary>
    [Flags]
    public enum RequestAttributes : long
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// Any
        /// </summary>
        Any = AnyNetworkAccessType | AnySecurityMode | AnyHttpMethod | AnyCallStyle | AnyFormat | AnyEndpoint,
        /// <summary>
        /// Any network access type
        /// </summary>
        AnyNetworkAccessType = External | LocalSubnet | Localhost | InProcess,
        /// <summary>
        /// Any security mode
        /// </summary>
        AnySecurityMode = Secure | InSecure,
        /// <summary>
        /// Any HTTP method
        /// </summary>
        AnyHttpMethod = HttpHead | HttpGet | HttpPost | HttpPut | HttpDelete | HttpPatch | HttpOptions | HttpOther,
        /// <summary>
        /// Any call style
        /// </summary>
        AnyCallStyle = OneWay | Reply,
        /// <summary>
        /// Any format
        /// </summary>
        AnyFormat = Soap11 | Soap12 | Xml | Json | Jsv | Html | ProtoBuf | Csv | MsgPack | Wire | FormatOther,
        /// <summary>
        /// Any endpoint
        /// </summary>
        AnyEndpoint = Http | MessageQueue | Tcp | Grpc | EndpointOther,
        /// <summary>
        /// The internal network access
        /// </summary>
        InternalNetworkAccess = InProcess | Localhost | LocalSubnet,

        //Whether it came from an Internal or External address
        /// <summary>
        /// The localhost
        /// </summary>
        Localhost = 1 << 0,
        /// <summary>
        /// The local subnet
        /// </summary>
        LocalSubnet = 1 << 1,
        /// <summary>
        /// The external
        /// </summary>
        External = 1 << 2,

        //Called over a secure or insecure channel
        /// <summary>
        /// The secure
        /// </summary>
        Secure = 1 << 3,
        /// <summary>
        /// The in secure
        /// </summary>
        InSecure = 1 << 4,

        //HTTP request type
        /// <summary>
        /// The HTTP head
        /// </summary>
        HttpHead = 1 << 5,
        /// <summary>
        /// The HTTP get
        /// </summary>
        HttpGet = 1 << 6,
        /// <summary>
        /// The HTTP post
        /// </summary>
        HttpPost = 1 << 7,
        /// <summary>
        /// The HTTP put
        /// </summary>
        HttpPut = 1 << 8,
        /// <summary>
        /// The HTTP delete
        /// </summary>
        HttpDelete = 1 << 9,
        /// <summary>
        /// The HTTP patch
        /// </summary>
        HttpPatch = 1 << 10,
        /// <summary>
        /// The HTTP options
        /// </summary>
        HttpOptions = 1 << 11,
        /// <summary>
        /// The HTTP other
        /// </summary>
        HttpOther = 1 << 12,

        //Call Styles
        /// <summary>
        /// The one way
        /// </summary>
        OneWay = 1 << 13,
        /// <summary>
        /// The reply
        /// </summary>
        Reply = 1 << 14,

        //Different formats
        /// <summary>
        /// The soap11
        /// </summary>
        Soap11 = 1 << 15,
        /// <summary>
        /// The soap12
        /// </summary>
        Soap12 = 1 << 16,
        //POX
        /// <summary>
        /// The XML
        /// </summary>
        Xml = 1 << 17,
        //Javascript
        /// <summary>
        /// The json
        /// </summary>
        Json = 1 << 18,
        //Jsv i.e. TypeSerializer
        /// <summary>
        /// The JSV
        /// </summary>
        Jsv = 1 << 19,
        //e.g. protobuf-net
        /// <summary>
        /// The proto buf
        /// </summary>
        ProtoBuf = 1 << 20,
        //e.g. text/csv
        /// <summary>
        /// The CSV
        /// </summary>
        Csv = 1 << 21,
        /// <summary>
        /// The HTML
        /// </summary>
        Html = 1 << 22,
        /// <summary>
        /// The wire
        /// </summary>
        Wire = 1 << 23,
        /// <summary>
        /// The MSG pack
        /// </summary>
        MsgPack = 1 << 24,
        /// <summary>
        /// The format other
        /// </summary>
        FormatOther = 1 << 25,

        //Different endpoints
        /// <summary>
        /// The HTTP
        /// </summary>
        Http = 1 << 26,
        /// <summary>
        /// The message queue
        /// </summary>
        MessageQueue = 1 << 27,
        /// <summary>
        /// The TCP
        /// </summary>
        Tcp = 1 << 28,
        /// <summary>
        /// The GRPC
        /// </summary>
        Grpc = 1 << 29,
        /// <summary>
        /// The endpoint other
        /// </summary>
        EndpointOther = 1 << 30,

        /// <summary>
        /// The in process
        /// </summary>
        InProcess = 1 << 31, //Service was executed within code (e.g. ResolveService<T>)
    }

    /// <summary>
    /// Enum Network
    /// </summary>
    public enum Network : long
    {
        /// <summary>
        /// The localhost
        /// </summary>
        Localhost = 1 << 0,
        /// <summary>
        /// The local subnet
        /// </summary>
        LocalSubnet = 1 << 1,
        /// <summary>
        /// The external
        /// </summary>
        External = 1 << 2,
    }

    /// <summary>
    /// Enum Security
    /// </summary>
    public enum Security : long
    {
        /// <summary>
        /// The secure
        /// </summary>
        Secure = 1 << 3,
        /// <summary>
        /// The in secure
        /// </summary>
        InSecure = 1 << 4,
    }

    /// <summary>
    /// Enum Http
    /// </summary>
    public enum Http : long
    {
        /// <summary>
        /// The head
        /// </summary>
        Head = 1 << 5,
        /// <summary>
        /// The get
        /// </summary>
        Get = 1 << 6,
        /// <summary>
        /// The post
        /// </summary>
        Post = 1 << 7,
        /// <summary>
        /// The put
        /// </summary>
        Put = 1 << 8,
        /// <summary>
        /// The delete
        /// </summary>
        Delete = 1 << 9,
        /// <summary>
        /// The patch
        /// </summary>
        Patch = 1 << 10,
        /// <summary>
        /// The options
        /// </summary>
        Options = 1 << 11,
        /// <summary>
        /// The other
        /// </summary>
        Other = 1 << 12,
    }

    /// <summary>
    /// Enum CallStyle
    /// </summary>
    public enum CallStyle : long
    {
        /// <summary>
        /// The one way
        /// </summary>
        OneWay = 1 << 13,
        /// <summary>
        /// The reply
        /// </summary>
        Reply = 1 << 14,
    }

    /// <summary>
    /// Enum Format
    /// </summary>
    public enum Format : long
    {
        /// <summary>
        /// The soap11
        /// </summary>
        Soap11 = 1 << 15,
        /// <summary>
        /// The soap12
        /// </summary>
        Soap12 = 1 << 16,
        /// <summary>
        /// The XML
        /// </summary>
        Xml = 1 << 17,
        /// <summary>
        /// The json
        /// </summary>
        Json = 1 << 18,
        /// <summary>
        /// The JSV
        /// </summary>
        Jsv = 1 << 19,
        /// <summary>
        /// The proto buf
        /// </summary>
        ProtoBuf = 1 << 20,
        /// <summary>
        /// The CSV
        /// </summary>
        Csv = 1 << 21,
        /// <summary>
        /// The HTML
        /// </summary>
        Html = 1 << 22,
        /// <summary>
        /// The wire
        /// </summary>
        Wire = 1 << 23,
        /// <summary>
        /// The MSG pack
        /// </summary>
        MsgPack = 1 << 24,
        /// <summary>
        /// The other
        /// </summary>
        Other = 1 << 25,
    }

    /// <summary>
    /// Enum Endpoint
    /// </summary>
    public enum Endpoint : long
    {
        /// <summary>
        /// The HTTP
        /// </summary>
        Http = 1 << 26,
        /// <summary>
        /// The mq
        /// </summary>
        Mq = 1 << 27,
        /// <summary>
        /// The TCP
        /// </summary>
        Tcp = 1 << 28,
        /// <summary>
        /// The other
        /// </summary>
        Other = 1 << 29,
    }

    /// <summary>
    /// Class RequestAttributesExtensions.
    /// </summary>
    public static class RequestAttributesExtensions
    {
        /// <summary>
        /// Determines whether the specified attrs is localhost.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns><c>true</c> if the specified attrs is localhost; otherwise, <c>false</c>.</returns>
        public static bool IsLocalhost(this RequestAttributes attrs)
        {
            return (RequestAttributes.Localhost & attrs) == RequestAttributes.Localhost;
        }

        /// <summary>
        /// Determines whether [is local subnet] [the specified attrs].
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns><c>true</c> if [is local subnet] [the specified attrs]; otherwise, <c>false</c>.</returns>
        public static bool IsLocalSubnet(this RequestAttributes attrs)
        {
            return (RequestAttributes.LocalSubnet & attrs) == RequestAttributes.LocalSubnet;
        }

        /// <summary>
        /// Determines whether the specified attrs is external.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns><c>true</c> if the specified attrs is external; otherwise, <c>false</c>.</returns>
        public static bool IsExternal(this RequestAttributes attrs)
        {
            return (RequestAttributes.External & attrs) == RequestAttributes.External;
        }

        /// <summary>
        /// Converts to format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Format.</returns>
        public static Format ToFormat(this string format)
        {
            try
            {
                return (Format)Enum.Parse(typeof(Format), format.ToUpper().Replace("X-", ""), true);
            }
            catch (Exception)
            {
                return Format.Other;
            }
        }

        /// <summary>
        /// Froms the format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        public static string FromFormat(this Format format)
        {
            var formatStr = format.ToString().ToLowerInvariant();
            if (format == Format.ProtoBuf || format == Format.MsgPack)
                return "x-" + formatStr;
            return formatStr;
        }

        /// <summary>
        /// Converts to format.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>Format.</returns>
        public static Format ToFormat(this Feature feature)
        {
            switch (feature)
            {
                case Feature.Xml:
                    return Format.Xml;
                case Feature.Json:
                    return Format.Json;
                case Feature.Jsv:
                    return Format.Jsv;
                case Feature.Csv:
                    return Format.Csv;
                case Feature.Html:
                    return Format.Html;
                case Feature.MsgPack:
                    return Format.MsgPack;
                case Feature.ProtoBuf:
                    return Format.ProtoBuf;
                case Feature.Soap11:
                    return Format.Soap11;
                case Feature.Soap12:
                    return Format.Soap12;
            }
            return Format.Other;
        }

        /// <summary>
        /// Converts to feature.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Feature.</returns>
        public static Feature ToFeature(this Format format)
        {
            switch (format)
            {
                case Format.Xml:
                    return Feature.Xml;
                case Format.Json:
                    return Feature.Json;
                case Format.Jsv:
                    return Feature.Jsv;
                case Format.Csv:
                    return Feature.Csv;
                case Format.Html:
                    return Feature.Html;
                case Format.MsgPack:
                    return Feature.MsgPack;
                case Format.ProtoBuf:
                    return Feature.ProtoBuf;
                case Format.Soap11:
                    return Feature.Soap11;
                case Format.Soap12:
                    return Feature.Soap12;
            }
            return Feature.CustomFormat;
        }

        /// <summary>
        /// Converts to soapfeature.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns>Feature.</returns>
        public static Feature ToSoapFeature(this RequestAttributes attributes)
        {
            if ((RequestAttributes.Soap11 & attributes) == RequestAttributes.Soap11)
                return Feature.Soap11;
            if ((RequestAttributes.Soap12 & attributes) == RequestAttributes.Soap12)
                return Feature.Soap12;
            return Feature.None;
        }
    }
}