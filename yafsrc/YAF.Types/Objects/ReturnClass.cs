namespace YAF.Types.Objects
{
    using System;

    /// <summary>
    /// the HTML elements class.
    /// </summary>
    [Serializable]
    public class ReturnClass
    {
        #region Properties

        /// <summary>
        ///  Gets or sets the Album/Image's Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///   Gets or sets the album/image's new Title/Caption
        /// </summary>
        public string NewTitle { get; set; }

        #endregion
    }
}
