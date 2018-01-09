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

namespace YAF.Core.Extensions
{
    #region Using

    using System;
    using System.Globalization;

    using FarsiLibrary.Utils;

    using PersianCalendar = System.Globalization.PersianCalendar;

    #endregion

    public static class CultureExtensions
    {
        /// <summary>
        /// The fa culture
        /// </summary>
        private static CultureInfo faCulture;

        /// <summary>
        /// The ar culture
        /// </summary>
        private static CultureInfo arCulture;

        /// <summary>
        /// The internalfa culture
        /// </summary>
        private static CultureInfo internalfaCulture;

        /// <summary>
        /// The pc.
        /// </summary>
        private static readonly PersianCalendar pc = new PersianCalendar();

        /// <summary>
        /// Gets the persian calendar.
        /// </summary>
        public static Calendar PersianCalendar
        {
            get
            {
                return pc;
            }
        }

        /// <summary>
        /// Gets the Currently selected UICulture
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return CultureInfo.CurrentUICulture;
            }
        }

        /// <summary>
        /// Instance of Farsi culture
        /// </summary>
        public static CultureInfo FarsiCulture
        {
            get
            {
                if (faCulture == null)
                    faCulture = new CultureInfo("fa-IR");

                return faCulture;
            }
        }

        /// <summary>
        /// Instance of Persian Culture with correct date formatting.
        /// </summary>
        public static CultureInfo PersianCulture
        {
            get
            {
                if (internalfaCulture == null)
                    internalfaCulture = new PersianCultureInfo();

                return internalfaCulture;
            }
        }
       public static bool IsFarsiCulture()
        {
            return IsFarsiCulture(CurrentCulture);
        }

        public static bool IsFarsiCulture(this CultureInfo culture)
        {
            return culture.Name.Equals(FarsiCulture.Name) || culture.Name.Equals(PersianCulture.Name)
                                                          || culture.Name.Equals(
                                                              "fa",
                                                              StringComparison.InvariantCultureIgnoreCase);
        }

    }
}