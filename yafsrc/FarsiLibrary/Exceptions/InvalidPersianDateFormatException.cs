// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPersianDateFormatException.cs" company="">
//   
// </copyright>
// <summary>
//   The invalid persian date format exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FarsiLibrary.Utils.Exceptions
{
    #region

    using System;

    #endregion

    /// <summary>
    /// The invalid persian date format exception.
    /// </summary>
    public class InvalidPersianDateFormatException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPersianDateFormatException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InvalidPersianDateFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPersianDateFormatException"/> class.
        /// </summary>
        public InvalidPersianDateFormatException()
        {
        }

        #endregion
    }
}