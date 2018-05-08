/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
                var uri = new Uri(url, UriKind.Absolute);
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