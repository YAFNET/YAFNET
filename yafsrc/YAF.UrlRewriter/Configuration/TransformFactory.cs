// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="TransformFactory.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Configuration
{
    using System;
    using System.Collections.Generic;

    using YAF.UrlRewriter.Transforms;

    /// <summary>
    /// Factory for creating transforms.
    /// </summary>
    public class TransformFactory
    {
        /*
        /// <summary>
        /// Adds a transform.
        /// </summary>
        /// <param name="transformType">The type of the transform.</param>
        public void AddTransform(string transformType)
        {
            AddTransform((IRewriteTransform)TypeHelper.Activate(transformType, null));
        }
         */

        /// <summary>
        /// Adds a transform.
        /// </summary>
        /// <param name="transform">The transform object.</param>
        public void Add(IRewriteTransform transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            this._transforms.Add(transform.Name, transform);
        }

        /// <summary>
        /// Gets a transform by name.
        /// </summary>
        /// <param name="name">The transform name.</param>
        /// <returns>The transform object.</returns>
        public IRewriteTransform GetTransform(string name)
        {
            return this._transforms.ContainsKey(name)
                ? this._transforms[name]
                : null;
        }

        /// <summary>
        /// The _transforms.
        /// </summary>
        private readonly IDictionary<string, IRewriteTransform> _transforms = new Dictionary<string, IRewriteTransform>();
    }
}
