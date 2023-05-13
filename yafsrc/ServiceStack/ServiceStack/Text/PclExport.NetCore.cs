// ***********************************************************************
// <copyright file="PclExport.NetCore.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if NET7_0_OR_GREATER && !NETSTANDARD2_0

using System;
using ServiceStack.Text;
using ServiceStack.Text.Common;

namespace ServiceStack 
{
    public class Net7PclExport : NetStandardPclExport
    {
        public Net7PclExport()
        {
            this.PlatformName = Platforms.Net7;
            ReflectionOptimizer.Instance = EmitReflectionOptimizer.Provider;            
        }

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
}

#endif