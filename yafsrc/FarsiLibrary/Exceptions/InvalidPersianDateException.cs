// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPersianDateException.cs" company="">
//   
// </copyright>
// <summary>
//   The invalid persian date exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FarsiLibrary.Utils.Exceptions
{
    #region

    using System;

    #endregion

    /// <summary>
    /// The invalid persian date exception.
    /// </summary>
    public class InvalidPersianDateException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPersianDateException"/> class.
        /// </summary>
        public InvalidPersianDateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPersianDateException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InvalidPersianDateException(string message)
            : base(message)
        {
        }

        #endregion
    }
}