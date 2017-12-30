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

namespace YAF.Utils.Extensions
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Json Extension
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="json">The json.</param>
        /// <returns>Deserialised Json Strin</returns>
        public static TType FromJson<TType>(this string json)
        {
            return Deserialise<TType>(json);
        }

        /// <summary>
        /// Deserialises the specified json.
        /// </summary>
        /// <typeparam name="T">The Object</typeparam>
        /// <param name="json">The json.</param>
        /// <returns>Deserialised Json Strin</returns>
        public static T Deserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object that will be serialized.</param>
        /// <returns>
        /// Serialised Json String
        /// </returns>
        public static string Serialize(object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// To Json String Extension
        /// </summary>
        /// <param name="obj">
        /// The object that will be serialized.
        /// </param>
        /// <returns>
        /// Serialised Json String
        /// </returns>
        public static string ToJson(this object obj)
        {
            return Serialize(obj);
        }
    }
}
