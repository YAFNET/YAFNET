/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Utils.Helpers
{
    #region Using

    using System;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// The validation helper.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Checks if string is an valid email address.
        /// </summary>
        /// <param name="email">
        /// The email string to check
        /// </param>
        /// <returns>
        /// A bool value indicating whether the value is a valid email
        /// </returns>
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(
                email, @"^([0-9a-z]+[-._+&])*[0-9a-z]+@([-0-9a-z]+[.])+[a-z]{2,6}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checks if string is an valid email address (xmpp).
        /// </summary>
        /// <param name="xmpp">
        /// The xmpp string to check
        /// </param>
        /// <returns>
        /// A bool value indicating whether the value is a valid xmpp
        /// </returns>
        public static bool IsValidXmpp(string xmpp)
        {
            return IsValidEmail(xmpp);
        }

        /// <summary>
        /// Checks if string is an valid Url.
        /// </summary>
        /// <param name="url">
        /// The url string to check
        /// </param>
        /// <returns>
        /// A bool value indicating whether the value is a valid Url
        /// </returns>
        public static bool IsValidURL(string url)
        {
            try
            {
                new Uri(url, UriKind.Absolute);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if string is an valid integer
        /// </summary>
        /// <param name="intstr">
        /// The value to check.
        /// </param>
        /// <returns>
        /// A bool value indicating whether the value is a valid  Integer
        /// </returns>
        public static bool IsValidInt(string intstr)
        {
            int value;
            return int.TryParse(intstr, out value);
        }

        /// <summary>
        /// Check if String is a Number
        /// </summary>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns>
        /// A bool value indicating whether the value is a valid Number
        /// </returns>
        public static bool IsNumeric(string valueToCheck)
        {
            double dummy;

            return double.TryParse(valueToCheck, System.Globalization.NumberStyles.Any, null, out dummy);
        }

        /// <summary>
        /// Checks if string is an valid integer
        /// </summary>
        /// <param name="intstr">
        /// The intstr.
        /// </param>
        /// <param name="lowerBound">
        /// The lower Bound.
        /// </param>
        /// <param name="upperBound">
        /// The upper Bound.
        /// </param>
        /// <returns>
        /// A bool value indicating whether the value is a valid integer
        /// </returns> 
        public static bool IsValidInt(string intstr, int lowerBound, int upperBound)
        {
            int value;

            if (int.TryParse(intstr, out value))
            {
                return value >= lowerBound && value <= upperBound;
            }

            return false;
        }

        /// <summary>
        /// The value is a valid positive Int16.
        /// </summary>
        /// <param name="intstr">
        /// The intstr.
        /// </param>
        /// <returns>
        /// A bool value indicating whether the value is a positive valid Int16.
        /// </returns>
        public static bool IsValidPosShort(string intstr)
        {
            short value;
            return short.TryParse(intstr, out value) && value >= 0;
        }
    }
}