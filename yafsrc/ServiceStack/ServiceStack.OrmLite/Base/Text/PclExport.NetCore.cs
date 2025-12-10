// ***********************************************************************
// <copyright file="PclExport.NetCore.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// 
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Base.Text.NetStandardPclExport" />
public class NetPclExport : NetStandardPclExport
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetPclExport"/> class.
    /// </summary>
    public NetPclExport()
    {
        this.PlatformName = Platforms.Net10;
        ReflectionOptimizer.Instance = EmitReflectionOptimizer.Provider;
    }

    /// <summary>
    /// Gets the js reader parse string span method.
    /// </summary>  public Net6PclExport()
    /// <typeparam name="TSerializer">The type of the t serializer.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns>ParseStringSpanDelegate.</returns>
    public override ParseStringSpanDelegate GetJsReaderParseStringSpanMethod<TSerializer>(Type type)
    {
        if (type.IsAssignableFrom(typeof(System.Dynamic.IDynamicMetaObjectProvider)) ||
            type.HasInterface(typeof(System.Dynamic.IDynamicMetaObjectProvider)))
        {
            return DeserializeDynamic<TSerializer>.ParseStringSpan;
        }

        return null;
    }
}