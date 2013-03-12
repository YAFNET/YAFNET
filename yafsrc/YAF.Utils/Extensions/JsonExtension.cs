/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
            /*var serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }*/

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
