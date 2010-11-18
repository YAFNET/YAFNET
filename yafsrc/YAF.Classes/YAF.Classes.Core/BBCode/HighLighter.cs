//****************************************************
//		Source Control Author : Justin Wendlandt
//			jwendl@hotmail.com
//		Control Protected by the Creative Commons License
//		http://creativecommons.org/licenses/by-nc-sa/2.0/
//****************************************************

namespace YAF.Classes.Core.BBCode
{
    #region Using

    using System;
    using System.Text;



    #endregion

    /// <summary>
    /// The high lighter.
    /// </summary>
    public class HighLighter
    {
        /* Ederon : 6/16/2007 - conventions */

        // To Replace Enter with <br />
        #region Constants and Fields

        /// <summary>
        ///   The _replace enter.
        /// </summary>
        private bool _replaceEnter;

        #endregion

        // Default Constructor
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "HighLighter" /> class.
        /// </summary>
        public HighLighter()
        {
            this._replaceEnter = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether ReplaceEnter.
        /// </summary>
        public bool ReplaceEnter
        {
            get
            {
                return this._replaceEnter;
            }

            set
            {
                this._replaceEnter = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The color text.
        /// </summary>
        /// <param name="tmpCode">
        /// The tmp code.
        /// </param>
        /// <param name="pathToDefFile">
        /// The path to def file.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The color text.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public string ColorText(string tmpCode, string pathToDefFile, string language)
        {
            language = language.ToLower();

            language = language.Replace("\"", string.Empty);

            var tmpOutput = new StringBuilder();

            // Create Output
            tmpOutput.AppendFormat("<pre class=\"brush:{0};\">{1}", language, Environment.NewLine);
            tmpOutput.Append(tmpCode);
            tmpOutput.AppendFormat("</pre>{0}", Environment.NewLine);

            return tmpOutput.ToString();
        }

        #endregion
    }
}