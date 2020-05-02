// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="StaticMappingTransform.cs">
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
    using System.Collections.Specialized;

    /// <summary>
    /// Default RewriteMapper, reads its maps from config.
    /// The mapping is CASE-INSENSITIVE.
    /// </summary>
    public sealed class StaticMappingTransform : IRewriteTransform
    {
        /// <summary>
        /// Gets the map.
        /// </summary>
        private readonly StringDictionary map;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMappingTransform"/> class. 
        /// Default constructor.
        /// </summary>
        /// <param name="name">
        /// The name of the mapping.
        /// </param>
        /// <param name="map">
        /// The mappings.
        /// </param>
        public StaticMappingTransform(string name, StringDictionary map)
        {
            this.Name = name;
            this.map = map;
        }
        
        /// <summary>
         /// Gets the name of the action.
         /// </summary>
        public string Name { get; }

        /// <summary>
        /// Maps the specified value in the specified map to its replacement value.
        /// </summary>
        /// <param name="input">The value being mapped.</param>
        /// <returns>The value mapped to, or null if no mapping could be performed.</returns>
        public string ApplyTransform(string input)
        {
            return this.map[input];
        }
    }
}
