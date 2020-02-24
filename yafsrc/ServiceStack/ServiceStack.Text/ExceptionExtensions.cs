// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;

namespace ServiceStack
{
    public static class ExceptionExtensions
    {
        public static Exception GetInnerMostException(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
    }
}