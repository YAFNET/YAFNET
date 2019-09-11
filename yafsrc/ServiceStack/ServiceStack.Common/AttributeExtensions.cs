// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Reflection;

namespace ServiceStack
{
    using System.ComponentModel;

    public static class AttributeExtensions
    {
        public static string GetDescription(this Type type)
        {
            var apiAttr = type.FirstAttribute<ApiAttribute>();
            if (apiAttr != null)
                return apiAttr.Description;

            var componentDescAttr = type.FirstAttribute<DescriptionAttribute>();
            if (componentDescAttr != null)
                return componentDescAttr.Description;

            var ssDescAttr = type.FirstAttribute<DataAnnotations.DescriptionAttribute>();
            return ssDescAttr?.Description;
        }

        public static string GetDescription(this MemberInfo mi)
        {
            var apiAttr = mi.FirstAttribute<ApiMemberAttribute>();
            if (apiAttr != null)
                return apiAttr.Description;

            var componentDescAttr = mi.FirstAttribute<DescriptionAttribute>();
            if (componentDescAttr != null)
                return componentDescAttr.Description;

            var ssDescAttr = mi.FirstAttribute<DataAnnotations.DescriptionAttribute>();
            return ssDescAttr?.Description;
        }

        public static string GetDescription(this ParameterInfo pi)
        {
            var componentDescAttr = pi.FirstAttribute<DescriptionAttribute>();
            if (componentDescAttr != null)
                return componentDescAttr.Description;

            var ssDescAttr = pi.FirstAttribute<DataAnnotations.DescriptionAttribute>();
            return ssDescAttr?.Description;
        }
    }
}
