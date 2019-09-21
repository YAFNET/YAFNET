// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="LowerTransform.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Transforms
{
    using System;
    using System.Threading;

    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Transforms the input to lower case.
    /// </summary>
    public sealed class LowerTransform : IRewriteTransform
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public string Name => Constants.TransformLower;

        /// <summary>
        /// Applies a transformation to the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The transformed string.</returns>
        public string ApplyTransform(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.ToLower(Thread.CurrentThread.CurrentCulture);
        }
    }
}
