// ***********************************************************************
// <copyright file="ParseUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text.Common
{
    using System;

    /// <summary>
    /// Class ParseUtils.
    /// </summary>
    internal static class ParseUtils
    {
        /// <summary>
        /// Gets the special parse method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ParseStringDelegate.</returns>
        public static ParseStringDelegate GetSpecialParseMethod(Type type)
        {
            if (type == typeof(Uri))
                return x => new Uri(x.FromCsvField());

            //Warning: typeof(object).IsInstanceOfType(typeof(Type)) == True??
            if (type.IsInstanceOfType(typeof(Type)))
                return ParseType;

            if (type == typeof(Exception))
                return x => new Exception(x);

            return type.IsInstanceOf(typeof(Exception)) ? DeserializeTypeUtils.GetParseMethod(type) : null;
        }

        /// <summary>
        /// Parses the type.
        /// </summary>
        /// <param name="assemblyQualifiedName">Name of the assembly qualified.</param>
        /// <returns>Type.</returns>
        public static Type ParseType(string assemblyQualifiedName)
        {
            return AssemblyUtils.FindType(assemblyQualifiedName.FromCsvField());
        }

        /// <summary>
        /// Tries the parse enum.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="str">The string.</param>
        /// <returns>System.Object.</returns>
        public static object TryParseEnum(Type enumType, string str)
        {
            if (str == null)
                return null;

            if (JsConfig.TextCase == TextCase.SnakeCase)
            {
                string[] names = Enum.GetNames(enumType);
                if (Array.IndexOf(names, str) == -1)    // case sensitive ... could use Linq Contains() extension with StringComparer.InvariantCultureIgnoreCase instead for a slight penalty
                    str = str.Replace("_", "");
            }

            var enumInfo = CachedTypeInfo.Get(enumType).EnumInfo;
            return enumInfo.Parse(str);
        }
    }

}