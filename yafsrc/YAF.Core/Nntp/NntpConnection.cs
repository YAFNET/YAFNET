/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Nntp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;

    using YAF.Types.Extensions;
    using YAF.Types.Objects.Nntp;

    /// <summary>
    /// The nntp connection.
    /// </summary>
    public class NntpConnection : IDisposable
    {
        #region Private variables

        /// <summary>
        /// The password.
        /// </summary>
        private string password;

        /// <summary>
        /// The sr.
        /// </summary>
        private StreamReader sr;

        /// <summary>
        /// The sw.
        /// </summary>
        private StreamWriter sw;

        /// <summary>
        /// The tcp client.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// The timeout.
        /// </summary>
        private int timeout;

        /// <summary>
        /// The username.
        /// </summary>
        private string username;

        /// <summary>
        /// The on request.
        /// </summary>
        private event OnRequestDelegate onRequest;

        #endregion

        #region Public accessors

        /// <summary>
        /// Gets or sets Timeout.
        /// </summary>
        public int Timeout
        {
            get => this.timeout;

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this.timeout = value;
                this.tcpClient.SendTimeout = this.timeout;
                this.tcpClient.ReceiveTimeout = this.timeout;
            }
        }

        /// <summary>
        /// Gets ConnectedServer.
        /// </summary>
        public string ConnectedServer { get; private set; }

        /// <summary>
        /// Gets ConnectedGroup.
        /// </summary>
        public Newsgroup ConnectedGroup { get; private set; }

        /// <summary>
        /// Gets Port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The on request.
        /// </summary>
        public event OnRequestDelegate OnRequest
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add => this.onRequest += value;

            [MethodImpl(MethodImplOptions.Synchronized)]
            remove => this.onRequest -= value;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// The reset.
        /// </summary>
        private void Reset()
        {
            this.ConnectedServer = null;
            this.ConnectedGroup = null;
            this.username = null;
            this.password = null;
            if (this.tcpClient != null)
            {
                try
                {
                    this.sw.Close();
                    this.sr.Close();
                    this.tcpClient.Close();
                }
                catch
                {
                }
            }

            this.tcpClient = new TcpClient { SendTimeout = this.timeout, ReceiveTimeout = this.timeout };
        }

        /// <summary>
        /// The make request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NntpException">
        /// </exception>
        private Response MakeRequest(string request)
        {
            while (true)
            {
                if (request != null)
                {
                    this.sw.WriteLine(request);
                    this.onRequest?.Invoke($"SEND: {request}");
                }

                int code;

                // vzrus: Here can be an IO exception
                // No connecting news server or the connection was broken because of an invalid request.
                var line = this.sr.ReadLine();

                if (this.onRequest != null && line != null)
                {
                    this.onRequest($"RECEIVE: {line}");
                }

                try
                {
                    code = int.Parse(line.Substring(0, 3));
                }
                catch (NullReferenceException)
                {
                    this.Reset();
                    throw new NntpException(line, request);
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.Reset();
                    throw new NntpException(line, request);
                }
                catch (ArgumentNullException)
                {
                    this.Reset();
                    throw new NntpException(line, request);
                }
                catch (FormatException)
                {
                    this.Reset();
                    throw new NntpException(line, request);
                }

                if (code != 480)
                {
                    return new Response(code, line.Length >= 5 ? line.Substring(4) : null, request);
                }

                if (this.SendIdentity())
                {
                    continue;
                }

                return new Response(code, line.Length >= 5 ? line.Substring(4) : null, request);
            }
        }

        /// <summary>
        /// The get header.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <returns>
        /// The <see cref="ArticleHeader"/>.
        /// </returns>
        private ArticleHeader GetHeader(out MIMEPart part)
        {
            string response;
            var header = new ArticleHeader();
            string name = null;
            header.ReferenceIds = new string[0];
            part = null;
            while ((response = this.sr.ReadLine()) != null && response != string.Empty)
            {
                var m = Regex.Match(response, @"^\s+(\S+)$");
                string value;
                if (m.Success)
                {
                    value = m.Groups[1].ToString();
                }
                else
                {
                    var i = response.IndexOf(':');
                    if (i == -1)
                    {
                        continue;
                    }

                    name = response.Substring(0, i).ToUpper();
                    value = response.Substring(i + 1);
                }

                switch (name)
                {
                    case "REFERENCES":
                        var values = value.Trim().Split(' ');
                        var values2 = header.ReferenceIds;
                        header.ReferenceIds = new string[values.Length + values2.Length];
                        values.CopyTo(header.ReferenceIds, 0);
                        values2.CopyTo(header.ReferenceIds, values.Length);
                        break;
                    case "SUBJECT":
                        header.Subject += NntpUtil.Base64HeaderDecode(value);
                        break;
                    case "DATE":
                        // vzrus: 31.03.10 dateTime and tz conversion
                        header.Date = NntpUtil.DecodeUTC(value, out var offTz);
                        header.TimeZoneOffset = offTz;
                        break;
                    case "FROM":
                        header.From += NntpUtil.Base64HeaderDecode(value);
                        break;
                    case "NNTP-POSTING-HOST":
                        header.PostingHost += value;
                        break;
                    case "LINES":
                        header.LineCount = int.Parse(value);
                        break;
                    case "MIME-VERSION":
                        part = new MIMEPart();
                        part.ContentType = "TEXT/PLAIN";
                        part.Charset = "US-ASCII";
                        part.ContentTransferEncoding = "7BIT";
                        part.Filename = null;
                        part.Boundary = null;
                        break;
                    case "CONTENT-TYPE":
                        if (part != null)
                        {
                            m = Regex.Match(response, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                part.ContentType = m.Groups[1].ToString();
                            }

                            m = Regex.Match(response, @"BOUNDARY=""?([^""\s;]+)", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                part.Boundary = m.Groups[1].ToString();
                                part.EmbeddedPartList = new ArrayList();
                            }

                            m = Regex.Match(response, @"CHARSET=""?([^""\s;]+)", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                part.Charset = m.Groups[1].ToString();
                            }

                            m = Regex.Match(response, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                part.Filename = m.Groups[1].ToString();
                            }
                        }

                        break;
                    case "CONTENT-TRANSFER-ENCODING":
                        if (part != null)
                        {
                            m = Regex.Match(
                                response,
                                @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)",
                                RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                part.ContentTransferEncoding = m.Groups[1].ToString();
                            }
                        }

                        break;
                }
            }

            return header;
        }

        /// <summary>
        /// The get normal body.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The <see cref="ArticleBody"/>.
        /// </returns>
        private ArticleBody GetNormalBody(string messageId)
        {
            var buff = new char[1];
            string response;
            var list = new ArrayList();
            var sb = new StringBuilder();
            this.sr.Read(buff, 0, 1);

            while ((response = this.sr.ReadLine()) != null)
            {
                if (buff[0] == '.')
                {
                    if (response == string.Empty)
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(response);
                    }
                }
                else
                {
                    Match m;
                    if ((buff[0] == 'B' || buff[0] == 'b')
                        && (m = Regex.Match(response, @"^EGIN \d\d\d (.+)$", RegexOptions.IgnoreCase)).Success)
                    {
                        var ms = new MemoryStream();
                        while ((response = this.sr.ReadLine()) != null
                               && (response.Length != 3 || response.ToUpper() != "END"))
                        {
                            NntpUtil.UUDecode(response, ms);
                        }

                        ms.Seek(0, SeekOrigin.Begin);
                        var bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        var attach = new NntpAttachment($"{messageId} - {m.Groups[1]}", m.Groups[1].ToString(), bytes);
                        list.Add(attach);
                        ms.Close();
                    }
                    else
                    {
                        sb.Append(buff[0]);
                        sb.Append(response);
                    }
                }

                sb.Append('\n');
                this.sr.Read(buff, 0, 1);
            }

            var ab = new ArticleBody
                         {
                             IsHtml = false,
                             Text = sb.ToString(),
                             Attachments = (NntpAttachment[])list.ToArray(typeof(NntpAttachment))
                         };
            return ab;
        }

        /// <summary>
        /// The get mime body.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <returns>
        /// The <see cref="ArticleBody"/>.
        /// </returns>
        private ArticleBody GetMIMEBody(string messageId, MIMEPart part)
        {
            ArticleBody body;
            try
            {
                NntpUtil.DispatchMIMEContent(this.sr, part, ".");
                var sb = new StringBuilder();
                var attachmentList = new ArrayList();
                body = new ArticleBody { IsHtml = true };
                this.ConvertMIMEContent(messageId, part, sb, attachmentList);
                body.Text = sb.ToString();
                body.Attachments = (NntpAttachment[])attachmentList.ToArray(typeof(NntpAttachment));
            }
            finally
            {
                if (((NetworkStream)this.sr.BaseStream).DataAvailable)
                {
                    string line;
                    while ((line = this.sr.ReadLine()) != null && line != ".")
                    {
                    }
                }
            }

            return body;
        }

        /// <summary>
        /// The convert mime content.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="attachmentList">
        /// The attachment list.
        /// </param>
        private void ConvertMIMEContent(string messageId, MIMEPart part, StringBuilder sb, IList attachmentList)
        {
            var m = Regex.Match(part.ContentType, @"MULTIPART", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                part.EmbeddedPartList.Cast<MIMEPart>().ForEach(
                    subPart => this.ConvertMIMEContent(messageId, subPart, sb, attachmentList));

                return;
            }

            m = Regex.Match(part.ContentType, @"TEXT", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                sb.Append(part.Text);
                sb.Append("<hr>");
                return;
            }

            var attachment = new NntpAttachment($"{messageId} - {part.Filename}", part.Filename, part.BinaryData);
            attachmentList.Add(attachment);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpConnection"/> class.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NntpConnection()
        {
            this.timeout = 5000;
            this.Reset();
        }

        /// <summary>
        /// The connect server.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ConnectServer(string server, int port)
        {
            if (this.ConnectedServer != null && this.ConnectedServer != server)
            {
                this.Disconnect();
            }

            if (this.ConnectedServer == server)
            {
                return;
            }

            this.tcpClient.Connect(server, port);
            var stream = this.tcpClient.GetStream();
            if (stream == null)
            {
                throw new NntpException("Fail to setup connection.");
            }

            this.sr = new StreamReader(stream, Encoding.Default);
            this.sw = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            var res = this.MakeRequest(null);
            if (res.Code != 200 && res.Code != 201)
            {
                this.Reset();
                throw new NntpException(res.Code);
            }

            this.ConnectedServer = server;
            this.Port = port;
        }

        /// <summary>
        /// The provide identity.
        /// </summary>
        /// <param name="userName">
        /// The username.
        /// </param>
        /// <param name="passWord">
        /// The password.
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ProvideIdentity(string userName, string passWord)
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            this.username = userName;
            this.password = passWord;
        }

        /// <summary>
        /// The send identity.
        /// </summary>
        /// <returns>
        /// The send identity.
        /// </returns>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendIdentity()
        {
            if (this.username == null)
            {
                return false;
            }

            var res = this.MakeRequest($"AUTHINFO USER {this.username}");
            if (res.Code == 381)
            {
                res = this.MakeRequest($"AUTHINFO PASS {this.password}");
            }

            if (res.Code == 281)
            {
                return true;
            }

            this.Reset();
            throw new NntpException(res.Code, "AUTHINFO PASS ******");
        }

        /// <summary>
        /// The connect group.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Newsgroup ConnectGroup(string group)
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            if (this.ConnectedGroup != null && this.ConnectedGroup.Group == group)
            {
                return this.ConnectedGroup;
            }

            var res = this.MakeRequest($"GROUP {group}");

            if (!res.Code.Equals(211))
            {
                this.ConnectedGroup = null;
                throw new NntpException(res.Code, res.Request);
            }

            var values = res.Message.Split(' ');
            this.ConnectedGroup = new Newsgroup(group, int.Parse(values[1]), int.Parse(values[2]));

            return this.ConnectedGroup;
        }

        /// <summary>
        /// The get group list.
        /// </summary>
        /// <returns>
        /// The <see cref="ArrayList"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetGroupList()
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            var res = this.MakeRequest("LIST");
            if (res.Code != 215)
            {
                throw new NntpException(res.Code, res.Request);
            }

            var list = new ArrayList();
            string response;
            while ((response = this.sr.ReadLine()) != null && response != ".")
            {
                var values = response.Split(' ');
                list.Add(new Newsgroup(values[0], int.Parse(values[2]), int.Parse(values[1])));
            }

            return list;
        }

        /// <summary>
        /// The get article id.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The get article id.
        /// </returns>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetArticleId(string messageId)
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            if (this.ConnectedGroup == null)
            {
                throw new NntpException("No connecting newsgroup.");
            }

            var res = this.MakeRequest($"STAT {messageId}");
            if (res.Code != 223)
            {
                throw new NntpException(res.Code, res.Request);
            }

            var i = res.Message.IndexOf(' ');
            return int.Parse(res.Message.Substring(0, i));
        }

        /// <summary>
        /// The get article list.
        /// </summary>
        /// <param name="low">
        /// The low.
        /// </param>
        /// <param name="high">
        /// The high.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NntpException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList<Article> GetArticleList(int low, int high)
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            if (this.ConnectedGroup == null)
            {
                throw new NntpException("No connecting newsgroup.");
            }

            var res = this.MakeRequest($"XOVER {low}-{high}");
            if (res.Code != 224)
            {
                throw new NntpException(res.Code, res.Request);
            }

            var list = new List<Article>();
            string response;
            while ((response = this.sr.ReadLine()) != null && response != ".")
            {
                Article article;
                try
                {
                    article = new Article { Header = new ArticleHeader() };
                    var values = response.Split('\t');

                    // article id...
                    article.ArticleId = int.Parse(values[0]);

                    // subject
                    article.Header.Subject = NntpUtil.Base64HeaderDecode(values[1]);

                    // from
                    article.Header.From = NntpUtil.Base64HeaderDecode(values[2]);

                    // date
                    var i = values[3].IndexOf(',');
                    article.Header.Date = NntpUtil.DecodeUTC(
                        values[3].Substring(i + 1, values[3].Length - 7 - i),
                        out var offTz);
                    article.Header.TimeZoneOffset = offTz;

                    // message id
                    article.MessageId = values[4];

                    // reference ids
                    article.Header.ReferenceIds = values[5].Trim().Length == 0 ? new string[0] : values[5].Split(' ');

                    if (values.Length < 8 || values[7] == null || values[7].Trim() == string.Empty)
                    {
                        article.Header.LineCount = 0;
                    }
                    else
                    {
                        article.Header.LineCount = int.Parse(values[7]);
                    }

                    // no body...
                    article.Body = null;
                }
                catch (Exception e)
                {
                    throw new Exception(response, e);
                }

                list.Add(article);
            }

            return list;
        }

        /// <summary>
        /// The get article.
        /// </summary>
        /// <param name="articleId">
        /// The article id.
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Article GetArticle(int articleId)
        {
            return this.GetArticle(articleId.ToString());
        }

        /// <summary>
        /// The get article.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Article GetArticle(string messageId)
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            if (this.ConnectedGroup == null)
            {
                throw new NntpException("No connecting newsgroup.");
            }

            var article = new Article();
            var res = this.MakeRequest($"Article {messageId}");
            if (res.Code != 220)
            {
                throw new NntpException(res.Code);
            }

            var i = res.Message.IndexOf(' ');
            article.ArticleId = int.Parse(res.Message.Substring(0, i));
            var end = res.Message.Substring(i, res.Message.Length - i - 1).Trim().IndexOf(' ');
            if (end == -1)
            {
                end = res.Message.Length - (i + 1);
            }

            article.MessageId = res.Message.Substring(i + 1, end);

            article.Header = this.GetHeader(out var part);
            article.MimePart = part;

            article.Body = article.MimePart == null
                               ? this.GetNormalBody(article.MessageId)
                               : this.GetMIMEBody(article.MessageId, article.MimePart);

            return article;
        }

        /// <summary>
        /// The post article.
        /// </summary>
        /// <param name="article">
        /// The article.
        /// </param>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PostArticle(Article article)
        {
            if (this.ConnectedServer == null)
            {
                throw new NntpException("No connecting newsserver.");
            }

            if (this.ConnectedGroup == null)
            {
                throw new NntpException("No connecting newsgroup.");
            }

            var res = this.MakeRequest("POST");
            if (res.Code != 340)
            {
                throw new NntpException(res.Code, res.Request);
            }

            var sb = new StringBuilder();

            sb.Append("From: ");
            sb.Append(article.Header.From);
            sb.Append("\r\nNewsgroups: ");
            sb.Append(this.ConnectedGroup.Group);

            if (article.Header.ReferenceIds != null && article.Header.ReferenceIds.Length != 0)
            {
                sb.Append("\r\nReference: ");
                sb.Append(string.Join(" ", article.Header.ReferenceIds));
            }

            sb.Append("\r\nSubject: ");
            sb.Append(article.Header.Subject);
            sb.Append("\r\n\r\n");
            article.Body.Text = article.Body.Text.Replace("\n.", "\n..");
            sb.Append(article.Body.Text);
            sb.Append("\r\n.\r\n");
            res = this.MakeRequest(sb.ToString());
            if (res.Code != 240)
            {
                throw new NntpException(res.Code, res.Request);
            }
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        /// <exception cref="NntpException">
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Disconnect()
        {
            if (this.ConnectedServer != null)
            {
                if (((NetworkStream)this.sr.BaseStream).DataAvailable)
                {
                    string response;
                    while ((response = this.sr.ReadLine()) != null && response != ".")
                    {
                    }
                }

                var res = this.MakeRequest("QUIT");
                if (res.Code != 205)
                {
                    throw new NntpException(res.Code, res.Request);
                }
            }

            this.Reset();
        }

        #endregion

        #region Nested type: Response

        /// <summary>
        /// The response.
        /// </summary>
        private class Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Response"/> class.
            /// </summary>
            /// <param name="code">
            /// The code.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="request">
            /// The request.
            /// </param>
            public Response(int code, string message, string request)
            {
                this.Code = code;
                this.Message = message;
                this.Request = request;
            }

            /// <summary>
            /// Gets the Code.
            /// </summary>
            public int Code { get; }

            /// <summary>
            /// Gets the Message.
            /// </summary>
            public string Message { get; }

            /// <summary>
            /// Gets the Request.
            /// </summary>
            public string Request { get; }
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Disconnect();
        }
    }
}