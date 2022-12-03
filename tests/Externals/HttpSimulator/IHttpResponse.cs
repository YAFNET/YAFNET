/* httpcontext-simulator 
 * a simulator used to simulate http context during integration testing
 *
 * Copyright (C) Phil Haack 
 * http://code.google.com/p/httpcontext-simulator/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

namespace HttpSimulator
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;

    /// <summary>
    /// The HttpResponse interface.
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether Buffer.
        /// </summary>
        bool Buffer { get; set; }

        /// <summary>
        /// Gets Cache.
        /// </summary>
        HttpCachePolicy Cache { get; }

        /// <summary>
        /// Gets or sets CacheControl.
        /// </summary>
        string CacheControl { get; set; }

        /// <summary>
        /// Gets or sets Charset.
        /// </summary>
        string Charset { get; set; }

        /// <summary>
        /// Gets or sets ContentEncoding.
        /// </summary>
        Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Gets or sets ContentType.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets Cookies.
        /// </summary>
        HttpCookieCollection Cookies { get; }

        /// <summary>
        /// Gets or sets Expires.
        /// </summary>
        int Expires { get; set; }

        /// <summary>
        /// Gets or sets Filter.
        /// </summary>
        Stream Filter { get; set; }

        /// <summary>
        /// Gets or sets HeaderEncoding.
        /// </summary>
        Encoding HeaderEncoding { get; set; }

        /// <summary>
        /// Gets Headers.
        /// </summary>
        NameValueCollection Headers { get; }

        /// <summary>
        /// Gets Output.
        /// </summary>
        TextWriter Output { get; }

        /// <summary>
        /// Gets OutputStream.
        /// </summary>
        Stream OutputStream { get; }

        /// <summary>
        /// Gets or sets RedirectLocation.
        /// </summary>
        string RedirectLocation { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        string Status { get; set; }

        /// <summary>
        /// Gets or sets StatusCode.
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        /// The add header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void AddHeader(string name, string value);

        /// <summary>
        /// The append header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void AppendHeader(string name, string value);

        /// <summary>
        /// The clear.
        /// </summary>
        void Clear();

        /// <summary>
        /// The clear content.
        /// </summary>
        void ClearContent();

        /// <summary>
        /// The clear headers.
        /// </summary>
        void ClearHeaders();

        /// <summary>
        /// The close.
        /// </summary>
        void Close();

        /// <summary>
        /// The disable kernel cache.
        /// </summary>
        void DisableKernelCache();

        /// <summary>
        /// The end.
        /// </summary>
        void End();

        /// <summary>
        /// The flush.
        /// </summary>
        void Flush();

        /// <summary>
        /// The pics.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Pics(string value);

        /// <summary>
        /// The redirect.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        void Redirect(string url);

        /// <summary>
        /// The redirect.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="endResponse">
        /// The end response.
        /// </param>
        void Redirect(string url, bool endResponse);

        /// <summary>
        /// The set cookie.
        /// </summary>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        void SetCookie(HttpCookie cookie);

        /// <summary>
        /// The transmit file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        void TransmitFile(string filename);

        /// <summary>
        /// The transmit file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        void TransmitFile(string filename, long offset, long length);

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="ch">
        /// The ch.
        /// </param>
        void Write(char ch);

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        void Write(object obj);

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        void Write(string s);

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        void Write(char[] buffer, int index, int count);

        /// <summary>
        /// The write file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        void WriteFile(string filename);

        /// <summary>
        /// The write file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="readIntoMemory">
        /// The read into memory.
        /// </param>
        void WriteFile(string filename, bool readIntoMemory);

        /// <summary>
        /// The write file.
        /// </summary>
        /// <param name="fileHandle">
        /// The file handle.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        void WriteFile(IntPtr fileHandle, long offset, long size);

        /// <summary>
        /// The write file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        void WriteFile(string filename, long offset, long size);

        /// <summary>
        /// The write substitution.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        void WriteSubstitution(HttpResponseSubstitutionCallback callback);

        // Properties
    }
}