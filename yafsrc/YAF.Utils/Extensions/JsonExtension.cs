/* Yet Another Forum.net
 * Copyright (C) 2006-2011 Jaben Cargman
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
	#region Using

	using System;
	using System.IO;
	using System.Runtime.Serialization.Json;
	using System.Text;

	using YAF.Types;

	#endregion

	/// <summary>
	/// Json Extension
	/// </summary>
	public static class JsonExtension
	{
		#region Public Methods

		/// <summary>
		/// Deserialises the specified json.
		/// </summary>
		/// <typeparam name="T">
		/// The Object
		/// </typeparam>
		/// <param name="json">
		/// The json.
		/// </param>
		/// <returns>
		/// Deserialised Json Strin
		/// </returns>
		public static T Deserialise<T>([NotNull] string json)
		{
			var obj = Activator.CreateInstance<T>();
			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				var serializer = new DataContractJsonSerializer(obj.GetType());
				obj = (T)serializer.ReadObject(ms);
				return obj;
			}
		}

		/// <summary>
		/// Froms the json.
		/// </summary>
		/// <typeparam name="TType">
		/// The type of the type.
		/// </typeparam>
		/// <param name="json">
		/// The json.
		/// </param>
		/// <returns>
		/// Deserialised Json Strin
		/// </returns>
		public static TType FromJson<TType>([NotNull] this string json)
		{
			return Deserialise<TType>(json);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T">
		/// The Object
		/// </typeparam>
		/// <param name="obj">
		/// The obj.
		/// </param>
		/// <returns>
		/// Serialised Json String
		/// </returns>
		[NotNull]
		public static string Serialize<T>(T obj)
		{
			var serializer = new DataContractJsonSerializer(obj.GetType());
			using (var ms = new MemoryStream())
			{
				serializer.WriteObject(ms, obj);
				return Encoding.Default.GetString(ms.ToArray());
			}
		}

		/// <summary>
		/// To Json String Extension
		/// </summary>
		/// <param name="obj">
		/// The obj.
		/// </param>
		/// <returns>
		/// Serialised Json String
		/// </returns>
		[NotNull]
		public static string ToJson([NotNull] this object obj)
		{
			return Serialize(obj);
		}

		#endregion
	}
}