

namespace YAF.Utils.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    public static class ImageHelper
    {
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
        public  static  string GetImageParameters(Uri uriPath, out long length)
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
                    return String.Empty;
                }
                finally
                {
                    if (img != null)
                    {
                        img.Dispose();
                    }
                }
                stream.Close();
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
            contentType = String.Empty;
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
    }
}
