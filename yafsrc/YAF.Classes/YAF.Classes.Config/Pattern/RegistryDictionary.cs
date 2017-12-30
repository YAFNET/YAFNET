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

namespace YAF.Classes.Pattern
{
    using System;
    using System.Collections.Generic;

    using YAF.Types.Extensions;

    /// <summary>
    ///     Provides a method for automatic overriding of a base hash...
    /// </summary>
    public class RegistryDictionaryOverride : RegistryDictionary
    {
        #region Fields

        /// <summary>
        ///     The _default get override.
        /// </summary>
        private bool _defaultGetOverride = true;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether DefaultGetOverride.
        /// </summary>
        public bool DefaultGetOverride
        {
            get
            {
                return this._defaultGetOverride;
            }

            set
            {
                this._defaultGetOverride = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether DefaultSetOverride.
        /// </summary>
        public bool DefaultSetOverride { get; set; }

        /// <summary>
        ///     Gets or sets OverrideDictionary.
        /// </summary>
        public RegistryDictionary OverrideDictionary { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get value.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public override T GetValue<T>(string name, T defaultValue)
        {
            return this.GetValue(name, defaultValue, this.DefaultGetOverride);
        }

        /// <summary>
        ///     The get value.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <param name="allowOverride">
        ///     The allow override.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public virtual T GetValue<T>(string name, T defaultValue, bool allowOverride)
        {
            if (allowOverride
                && this.OverrideDictionary != null
                && this.OverrideDictionary.ContainsKey(name)
                && this.OverrideDictionary[name] != null)
            {
                return this.OverrideDictionary.GetValue(name, defaultValue);
            }

            // just pull the value from this dictionary...
            return base.GetValue(name, defaultValue);
        }

        /// <summary>
        ///     The set value.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public override void SetValue<T>(string name, T value)
        {
            this.SetValue(name, value, this.DefaultSetOverride);
        }

        /// <summary>
        ///     The set value.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="setOverrideOnly">
        ///     The set override only.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public virtual void SetValue<T>(string name, T value, bool setOverrideOnly)
        {
            if (this.OverrideDictionary != null)
            {
                if (setOverrideOnly)
                {
                    // just set the override dictionary...
                    this.OverrideDictionary.SetValue(name, value);
                    return;
                }

                if (this.OverrideDictionary.ContainsKey(name) && this.OverrideDictionary[name] != null)
                {
                    // set the overriden value to null/erase it...
                    this.OverrideDictionary.SetValue(name, (T)Convert.ChangeType(null, typeof(T)));
                }
            }

            // save new value in the base...
            base.SetValue(name, value);
        }

        #endregion
    }

    /// <summary>
    ///     The registry dictionary.
    /// </summary>
    public class RegistryDictionary : Dictionary<string, object>
    {
        public RegistryDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The get value.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public virtual T GetValue<T>(string name, T defaultValue)
        {
            if (!this.ContainsKey(name))
            {
                return defaultValue;
            }

            object value = this[name];

            if (value == null)
            {
                return defaultValue;
            }

            Type objectType = typeof(T);

            if (objectType.BaseType == typeof(Enum))
            {
                return (T)Enum.Parse(objectType, value.ToString());
            }

            // special handling for boolean...
            if (objectType == typeof(bool))
            {
                int i;
                return int.TryParse(value.ToString(), out i)
                           ? (T)Convert.ChangeType(Convert.ToBoolean(i), typeof(T))
                           : (T)Convert.ChangeType(Convert.ToBoolean(value), typeof(T));
            }

            // special handling for int values...
            if (objectType == typeof(int))
            {
                return (T)Convert.ChangeType(Convert.ToInt32(value), typeof(T));
            }

            return this[name].ToType<T>();
        }

        /// <summary>
        ///     The set value.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public virtual void SetValue<T>(string name, T value)
        {
            var objectType = typeof(T);
            var stringValue = value.ToType<string>();

            if (objectType == typeof(bool) || objectType.BaseType == typeof(Enum))
            {
                stringValue = Convert.ToString(Convert.ToInt32(value));
            }

            this[name] = stringValue;
        }

        #endregion
    }
}