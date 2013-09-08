/* Yet Another Forum.net
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

namespace YAF.Utils.Helpers
{
	#region Using

	using System;
	using System.Drawing;
	using System.IO;
	using System.Net;
	using System.Web;

	using YAF.Types.Extensions;

    #endregion

	/// <summary>
	/// The image helper.
	/// </summary>
	public static class ImageHelper
	{
		#region Public Methods

		/// <summary>
		/// From a path, return a byte[] of the image.
		/// </summary>
		/// <param name="uriPath">
		/// External image path. 
		/// </param>
		/// <param name="length">
		/// The image size in bytes. 
		/// </param>
		/// <returns>
		/// The get image parameters. 
		/// </returns>
		public static string GetImageParameters(Uri uriPath, out long length)
		{
			string pseudoMime = string.Empty;
			string contentType = string.Empty;
			using (Stream stream = GetRemoteData(uriPath, out length, out contentType))
			{
				Bitmap img = null;
				try
				{
					img = new Bitmap(stream);

					// no need to set here mime exatly this is reserved for customization.
					pseudoMime = "{0}!{1};{2}".FormatWith(contentType, img.Width, img.Height);
				}
				catch
				{
					return string.Empty;
				}
				finally
				{
					if (img != null)
					{
						img.Dispose();
					}
				}
			}

			return pseudoMime;
		}

		/// <summary>
		/// An image reader to read images on local disk.
		/// </summary>
		/// <param name="path">
		/// The path. 
		/// </param>
		public static Stream GetLocalData(Uri path)
		{
			return new FileStream(path.LocalPath, FileMode.Open);
		}

		/// <summary>
		/// The get remote data.
		/// </summary>
		/// <param name="url">
		/// The url. 
		/// </param>
		/// <param name="length">
		/// The content length in bits. 
		/// </param>
		/// <param name="contentType">
		/// The content type. 
		/// </param>
		/// <returns>
		/// the Stream class. 
		/// </returns>
		public static Stream GetRemoteData(Uri url, out long length, out string contentType)
		{
			string path = url.ToString();
			length = 0;
			contentType = string.Empty;
			try
			{
				if (path.StartsWith("~/"))
				{
					path = "file://" + HttpRuntime.AppDomainAppPath + path.Substring(2, path.Length - 2);
				}

				WebRequest request = WebRequest.Create(new Uri(path));

				WebResponse response = request.GetResponse();
				length = response.ContentLength;
				contentType = response.ContentType;
				return response.GetResponseStream();
			}
			catch
			{
				return new MemoryStream();
			}

			// Don't make the program crash just because we have a picture which failed downloading
		}

		/// <summary>
		/// Returns resized image stream.
		/// </summary>
		/// <param name="img">
		/// The Image. 
		/// </param>
		/// <param name="x">
		/// The image width. 
		/// </param>
		/// <param name="y">
		/// The image height. 
		/// </param>
		/// <returns>
		/// A resized image stream Stream. 
		/// </returns>
		public static Stream GetResizedImageStreamFromImage(Image img, long x, long y)
		{
			Stream resized = null;
			double newWidth = img.Width;
			double newHeight = img.Height;
			if (newWidth > x)
			{
				newHeight = newHeight * x / newWidth;
				newWidth = x;
			}

			if (newHeight > y)
			{
				newWidth = newWidth * y / newHeight;
				newHeight = y;
			}

			// TODO : Save an Animated Gif
			var bitmap = img.GetThumbnailImage((int)newWidth, (int)newHeight, null, IntPtr.Zero);

			resized = new MemoryStream();
			bitmap.Save(resized, img.RawFormat);
			return resized;
		}

		#endregion
	}
}