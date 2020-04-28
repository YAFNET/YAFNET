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

namespace YAF.Web.Controls
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;

    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The web profiler.
    /// </summary>
    [ToolboxData("<{0}:WebProfiler runat=server></{0}:WebProfiler>")]
    public class WebProfiler : BaseControl
    {
        #region Public Properties

        /// <summary>
        /// The profiler URL.
        /// </summary>
        private static string ProfilerURL => BuildLink.GetLink(ForumPages.Admin_Profiler);

        /// <summary>
        /// Gets or sets the session objects.
        /// </summary>
        private static Hashtable SessionObjects
        {
            get
            {
                BoardContext.Current.Get<HttpSessionStateBase>()["_webprofiler"] ??= new Hashtable();
                return (Hashtable)BoardContext.Current.Get<HttpSessionStateBase>()["_webprofiler"];
            }
            set => BoardContext.Current.Get<HttpSessionStateBase>()["_webprofiler"] = value;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The render control.
        /// </summary>
        /// <param name="writer">
        /// The <paramref name="writer"/>.
        /// </param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            this.Get<HttpSessionStateBase>()["_webprofiler"] ??= new Hashtable();

            writer.Write("<ul class=\"nav\">");

            // TODO : Active item
            writer.Write(
                !this.Get<HttpContextBase>().Request.QueryString.Exists("detail")
                    ? $"<li class\"nav-item\"><a class=\"nav-link active\" href='{ProfilerURL}'>web profiler</a></li>"
                    : $"<li class\"nav-item\"><a class=\"nav-link\" href='{ProfilerURL}'>web profiler</a></li>");

            writer.Write(
                $"<li class\"nav-item\"><a class=\"nav-link\" data-toggle=\"tooltip\" title=\"A page displaying grand totals of the application state, all session states and the web application cache along with server variables\" href='{ProfilerURL}?detail=Summary'>server summary</a></li>");
            writer.Write(
                $"<li class\"nav-item\"><a class=\"nav-link\" data-toggle=\"tooltip\" title=\"All server cache objects and their sizes\" href='{ProfilerURL}?detail=Cache'>server cache</a></li>");
            writer.Write(
                $"<li class\"nav-item\"><a class=\"nav-link\" data-toggle=\"tooltip\" title=\"All objects and their sizes contained within the application state\" href='{ProfilerURL}?detail=ApplicationState'>application state</a></li>");
            writer.Write(
                $"<li class\"nav-item\"><a class=\"nav-link\" data-toggle=\"tooltip\" title=\"A list of all the active sessions with their session id's, total amount of objects total of the object sizes\" href='{ProfilerURL}?detail=SessionState'>session state</a></li>");
            writer.Write(
                $"<li class\"nav-item\"><a class=\"nav-link\" data-toggle=\"tooltip\" title=\"View objects stored in the current web session\" href='{ProfilerURL}?detail=SessionState&sessionID=j{this.Get<HttpSessionStateBase>().SessionID}'>current session</a></li>");
            writer.Write("</ul>");

            if (this.Get<HttpContextBase>().Request.QueryString["detail"] == null)
            {
                writer.Write("<h5 class=\"card-title\">Web Profiler</h6>");
                writer.Write(
                    "<p class=\"card-text\">The WebProfiler was created to assist a developer or a server administrator to monitor and analyze the way an Asp.Net web application utilizes memory. In order for the profiler to calculate the sizes of objects stored in memory, the object class definitions need to be declared using the<em> [Serializable]</em> class attribute. Objects which are not serializable, will not add towards calculating the size of objects as it is displayed in the profiler.</p>");
                writer.Write(
                    "<p class=\"card-text\">Objects which are not serializable may still be analyzed by using the drill down hyperlinks to view the contents, but sizes will be displayed as 0. Please bear in mind that the profiler uses <em>reflection</em> and <em>serialization</em> techniques to calculate object contents and sizes and may take a while to compute</p>");
            }
            else if (this.Get<HttpContextBase>().Request.QueryString["detail"] != "ApplicationState")
            {
                if (this.Get<HttpContextBase>().Request.QueryString["detail"] != "SessionState")
                {
                    if (this.Get<HttpContextBase>().Request.QueryString["detail"] != "Cache")
                    {
                        if (this.Get<HttpContextBase>().Request.QueryString["detail"] != "Summary")
                        {
                            return;
                        }

                        writer.Write(GetApplicationStateTotals());
                        writer.Write(GetCacheTotals());
                        writer.Write(GetSessionStateTotals());
                        writer.Write(GetWebInformation());
                    }
                    else if (this.Get<HttpContextBase>().Request.QueryString["key"] == null)
                    {
                        writer.Write(GetCacheTable());
                    }
                    else
                    {
                        var key = this.Get<HttpContextBase>().Request.QueryString["key"];
                        writer.Write(
                            this.Get<HttpContextBase>().Request.QueryString["hash"] == null
                                ? GetCacheObjectTable(key)
                                : GetHashObjectTable(
                                    int.Parse(this.Get<HttpContextBase>().Request.QueryString["hash"])));
                    }
                }
                else if (this.Get<HttpContextBase>().Request.QueryString["sessionID"] == null)
                {
                    writer.Write(GetSessionStatesTable());
                }
                else
                {
                    var sessionID = this.Get<HttpContextBase>().Request.QueryString["sessionID"];
                    if (this.Get<HttpContextBase>().Request.QueryString["key"] == null)
                    {
                        writer.Write(GetSessionTable(sessionID));
                    }
                    else
                    {
                        var key = this.Get<HttpContextBase>().Request.QueryString["key"];
                        writer.Write(
                            this.Get<HttpContextBase>().Request.QueryString["hash"] == null
                                ? GetSessionObjectTable(sessionID, key)
                                : GetHashObjectTable(
                                    int.Parse(this.Get<HttpContextBase>().Request.QueryString["hash"])));
                    }
                }
            }
            else if (this.Get<HttpContextBase>().Request.QueryString["key"] == null)
            {
                writer.Write(GetApplicationTable());
            }
            else
            {
                var key = this.Get<HttpContextBase>().Request.QueryString["key"];
                writer.Write(
                    this.Get<HttpContextBase>().Request.QueryString["hash"] == null
                        ? GetApplicationObjectTable(key)
                        : GetHashObjectTable(int.Parse(this.Get<HttpContextBase>().Request.QueryString["hash"])));
            }
        }

        /// <summary>
        /// The build object tree.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string BuildObjectTree(object value)
        {
            var str = BoardContext.Current.Get<HttpContextBase>().Request.Url.ToString();
            if (str.Contains("&hash"))
            {
                var length = str.LastIndexOf("&", StringComparison.Ordinal);
                str = str.Substring(0, length);
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<ul class=\"list-group\">");
            var properties = value.GetType().GetProperties();
            var num1 = 0;
            var propertyInfoArray = properties;
            var index1 = 0;
            while (true)
            {
                if (index1 < propertyInfoArray.Length)
                {
                    var propertyInfo1 = propertyInfoArray[index1];

                    stringBuilder.Append("<li class=\"list-group-item\">");
                    stringBuilder.Append(propertyInfo1.Name);

                    var indexParameters = propertyInfo1.GetIndexParameters();
                    var obj1 = (object)null;
                    if (indexParameters.Length != 0)
                    {
                        try
                        {
                            if (num1 == 0)
                            {
                                var propertyInfo2 = properties.FirstOrDefault(i => i.Name == "Count");
                                if (propertyInfo2 != null)
                                {
                                    num1 = (int)propertyInfo2.GetValue(value, null);
                                }
                            }

                            var num2 = 0;
                            while (true)
                            {
                                if (num2 < num1)
                                {
                                    var index2 = new object[] { num2 };
                                    var obj2 = propertyInfo1.GetValue(value, index2);
                                    obj1 = obj2;
                                    if (obj2 != null)
                                    {
                                        if (!obj2.GetType().IsValueType && obj2.GetType().Name != "String" &&
                                            obj2.GetType().Name != "Guid" && obj2.GetType().Name != "DateTime")
                                        {
                                            stringBuilder.Append(BuildObjectTree(obj2));
                                        }
                                        else
                                        {
                                            stringBuilder.Append(obj2);
                                        }
                                    }

                                    ++num2;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            num1 = 0;
                        }
                        catch (Exception ex)
                        {
                            stringBuilder.Append(
                                ex.InnerException == null ? "Value could not be loaded" : ex.InnerException.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            var obj2 = propertyInfo1.GetValue(value, null);
                            obj1 = obj2;
                            if (obj2 == null || obj2.GetType().IsArray)
                            {
                                if (obj2 != null)
                                {
                                    var enumerator = (obj2 as Array).GetEnumerator();
                                    try
                                    {
                                        while (true)
                                        {
                                            if (enumerator.MoveNext())
                                            {
                                                var current = enumerator.Current;
                                                if (!current.GetType().IsValueType &&
                                                    current.GetType().Name != "String" &&
                                                    current.GetType().Name != "Guid" &&
                                                    current.GetType().Name != "DateTime")
                                                {
                                                    stringBuilder.Append(BuildObjectTree(current));
                                                }
                                                else
                                                {
                                                    stringBuilder.Append(current);
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        if (enumerator is IDisposable disposable)
                                        {
                                            disposable.Dispose();
                                        }
                                    }
                                }
                            }
                            else if (!obj2.GetType().IsValueType && obj2.GetType().Name != "String" &&
                                     obj2.GetType().Name != "Guid" && obj2.GetType().Name != "DateTime")
                            {
                                if (!SessionObjects.Contains(obj2.GetHashCode()))
                                {
                                    SessionObjects.Add(obj2.GetHashCode(), obj2);
                                }

                                var strArray1 = new string[7];
                                strArray1[0] = "<a href='";
                                strArray1[1] = str;
                                strArray1[2] = "&hash=";
                                var strArray2 = strArray1;
                                var hashCode = obj2.GetHashCode();
                                strArray2[3] = hashCode.ToString();
                                strArray2[4] = "'>";
                                strArray2[5] = obj2.ToString();
                                strArray2[6] = "</a>";
                                stringBuilder.Append(string.Concat(strArray2));
                            }
                            else
                            {
                                if (propertyInfo1.Name == "Count")
                                {
                                    num1 = (int)obj2;
                                }

                                stringBuilder.Append(obj2);
                            }
                        }
                        catch (Exception ex)
                        {
                            stringBuilder.Append(
                                ex.InnerException == null ? "Value could not be loaded" : ex.InnerException.Message);
                        }
                    }

                    //stringBuilder.Append("</td>");
                    //stringBuilder.Append("<td>");
                    stringBuilder.Append(GetObjectStringSize(obj1));
                    //stringBuilder.Append("</td>");
                    stringBuilder.Append("</li>");
                    ++index1;
                }
                else
                {
                    break;
                }
            }

            stringBuilder.Append("</ul>");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// The convert to string size.
        /// </summary>
        /// <param name="byteSize">
        /// The byte size.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ConvertToStringSize(long byteSize)
        {
            var str = "0 byte(s)";
            try
            {
                var num = (decimal)byteSize;
                if (!(num <= new decimal(1024)))
                {
                    if (num <= new decimal(1024) || !(num <= new decimal(1048576)))
                    {
                        if (!(num <= new decimal(1048576)) && num <= new decimal(1073741824))
                        {
                            str = $"{Math.Round(num / new decimal(1024) / new decimal(1024), 2)} MB";
                        }
                    }
                    else
                    {
                        str = $"{Math.Round(num / new decimal(1024), 2)} KB";
                    }
                }
                else
                {
                    str = $"{num} byte(s)";
                }

                return str;
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// The get active sessions.
        /// </summary>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        private static Hashtable GetActiveSessions()
        {
            var hashtable1 = new Hashtable();

            var obj = GetAspNetInternalCacheObj();

            dynamic fieldInfo = obj.GetType().GetField("_caches", BindingFlags.NonPublic | BindingFlags.Instance);

            object[] objArray;
            if (fieldInfo != null)
            {
                objArray = (object[])fieldInfo.GetValue(obj);
            }
            else
            {
                //If server uses "_cachesRefs" to store session info
                fieldInfo = obj.GetType().GetField("_cachesRefs", BindingFlags.NonPublic | BindingFlags.Instance);
                objArray = fieldInfo.GetValue(obj);
            }

            var index = 0;
            while (true)
            {
                if (index < objArray.Length)
                {
                    var target = objArray[index].GetType().GetProperty("Target")?.GetValue(objArray[index], null);

                    var hashtable2 = (Hashtable)target.GetType().GetField(
                        "_entries",
                        BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);
                    lock (hashtable2)
                    {
                        var local_6 = hashtable2.GetEnumerator();
                        try
                        {
                            while (true)
                            {
                                if (local_6.MoveNext())
                                {
                                    var local_7 = (DictionaryEntry)local_6.Current;
                                    var local_8 = local_7.Value.GetType().GetProperty(
                                        "Value",
                                        BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(local_7.Value, null);

                                    if (local_8?.GetType().ToString() != "System.Web.SessionState.InProcSessionState")
                                    {
                                        continue;
                                    }

                                    var local_9 = (SessionStateItemCollection)local_8.GetType().GetField(
                                        "_sessionItems",
                                        BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(local_8);

                                    if (local_9 == null)
                                    {
                                        continue;
                                    }

                                    var local_10 = local_7.Value.GetType()
                                        .GetProperty("Key", BindingFlags.Instance | BindingFlags.NonPublic)
                                        ?.GetValue(local_7.Value, null).ToString();
                                    if (!hashtable1.Contains(local_10))
                                    {
                                        hashtable1.Add(local_10, local_9);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            if (local_6 is IDisposable local_11)
                            {
                                local_11.Dispose();
                            }
                        }
                    }

                    ++index;
                }
                else
                {
                    break;
                }
            }

            return hashtable1;
        }

        /// <summary>
        /// The get application object table.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetApplicationObjectTable(string key)
        {
            var stringBuilder = new StringBuilder();
            var obj = BoardContext.Current.Get<HttpApplicationStateBase>()[key];
            if (obj == null)
            {
                stringBuilder.Append("The object was not found. Was it removed?");
            }
            else
            {
                stringBuilder.Append("<h5 class=\"card-title\">");
                stringBuilder.Append($"Application state object value(s) - {key}");
                stringBuilder.Append("</h5>");
                lock (obj) stringBuilder.Append(BuildObjectTree(obj));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get application state totals.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetApplicationStateTotals()
        {
            BoardContext.Current.Get<HttpApplicationStateBase>()["Avbob.Security.User"] =
                BoardContext.Current.Get<HttpSessionStateBase>()["Avbob.Security.User"];
            var stringBuilder = new StringBuilder();
            var count = BoardContext.Current.Get<HttpApplicationStateBase>().Keys.Count;
            var byteSize = 0L;
            var keys = BoardContext.Current.Get<HttpApplicationStateBase>().Keys;
            lock (keys)
            {
                var local_5 = keys.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (local_5.MoveNext())
                        {
                            var local_6 = (string)local_5.Current;

                            var local_7 = BoardContext.Current.Get<HttpApplicationStateBase>()[local_6];
                            byteSize += GetObjectByteSize(local_7);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (local_5 is IDisposable local_8)
                    {
                        local_8.Dispose();
                    }
                }
            }

            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Application state object totals");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<div class=\"table-responsive\">");
            stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");
            stringBuilder.Append("<thead class=\"thead-dark\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Total amount of objects");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Byte(s)");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Size");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("");
            stringBuilder.Append("</th>");

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(count.ToString());
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(byteSize.ToString());
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(ConvertToStringSize(byteSize));
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            if (count > 0)
            {
                stringBuilder.Append($"<a href='{ProfilerURL}?detail=ApplicationState'>view detail...</a>");
            }

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get application table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetApplicationTable()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Application state objects");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<div class=\"table-responsive\">");
            stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");
            stringBuilder.Append("<thead class=\"thead-dark\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Name");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Type");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Value");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Byte(s)");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Size");
            stringBuilder.Append("</th>");

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");


            lock (BoardContext.Current.Get<HttpApplicationStateBase>().Keys)
            {
                var local_4 = BoardContext.Current.Get<HttpApplicationStateBase>().Keys.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (local_4.MoveNext())
                        {
                            var local_5 = (string)local_4.Current;
                            var local_6 = new StringBuilder();
                            try
                            {
                                var local_7 = BoardContext.Current.Get<HttpApplicationStateBase>()[local_5];

                                if (local_7 == null)
                                {
                                    continue;
                                }

                                local_6.AppendLine("<tbody>");
                                local_6.AppendLine("<tr>");
                                if (!local_7.GetType().IsValueType && local_7.GetType().Name != "String" &&
                                    local_7.GetType().Name != "Guid" && local_7.GetType().Name != "DateTime")
                                {
                                    local_6.AppendLine("<td>");
                                    var local_8 = new[]
                                    {
                                        "<a href='", ProfilerURL, "?detail=ApplicationState&object=&key=", local_5,
                                        "'>"
                                    };
                                    local_6.AppendLine(string.Concat(local_8));
                                    local_6.AppendLine(local_5);
                                    local_6.AppendLine("</a>");
                                    local_6.AppendLine("</td>");
                                }
                                else
                                {
                                    local_6.AppendLine("<td>");
                                    local_6.AppendLine(local_5);
                                    local_6.AppendLine("</td>");
                                }

                                local_6.AppendLine("<td>");
                                local_6.AppendLine(local_7.GetType().Name);
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("<td>");
                                local_6.AppendLine(local_7.ToString());
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("<td>");
                                var local_9 = GetObjectByteSize(local_7);
                                local_6.AppendLine(local_9.ToString());
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("<td>");
                                local_6.AppendLine(GetObjectStringSize(local_7));
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("</tr>");
                                stringBuilder.Append(local_6);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (local_4 is IDisposable local_12)
                    {
                        local_12.Dispose();
                    }
                }
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get cache object table.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetCacheObjectTable(string key)
        {
            var stringBuilder = new StringBuilder();
            var obj = HttpRuntime.Cache[key];
            if (obj == null)
            {
                stringBuilder.Append("The cache object was not found. Did it expire?");
            }
            else
            {
                stringBuilder.Append("<h5 class=\"card-title\">");
                stringBuilder.Append($"Cache object value(s) - {key}");
                stringBuilder.Append("</h5>");
                stringBuilder.Append(BuildObjectTree(obj));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get cache table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetCacheTable()
        {
            var stringBuilder = new StringBuilder();
            GetActiveSessions();
            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Cache objects");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<div class=\"table-responsive\">");
            stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");
            stringBuilder.Append("<thead class=\"thead-dark\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Name");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Type");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Value");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Byte(s)");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Size");
            stringBuilder.Append("</th>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");
            lock (HttpRuntime.Cache)
            {
                var local_4 = HttpRuntime.Cache.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (local_4.MoveNext())
                        {
                            var local_5 = (DictionaryEntry)local_4.Current;
                            var local_6 = new StringBuilder();
                            try
                            {
                                local_6.AppendLine("<tbody>");
                                local_6.AppendLine("<tr>");
                                if (!local_5.Value.GetType().IsValueType && local_5.Value.GetType().Name != "String" &&
                                    local_5.Value.GetType().Name != "Guid" &&
                                    local_5.Value.GetType().Name != "DateTime")
                                {
                                    local_6.AppendLine("<td>");
                                    var local_7 = new[]
                                    {
                                        "<a href='", ProfilerURL, "?detail=Cache&object=&key=",
                                        local_5.Key.ToString(), "'>"
                                    };
                                    local_6.AppendLine(string.Concat(local_7));
                                    local_6.AppendLine(local_5.Key.ToString());
                                    local_6.AppendLine("</a>");
                                    local_6.AppendLine("</td>");
                                }
                                else
                                {
                                    local_6.AppendLine("<td>");
                                    local_6.AppendLine(local_5.Value.ToString());
                                    local_6.AppendLine("</td>");
                                }

                                local_6.AppendLine("<td>");
                                local_6.AppendLine(local_5.Value.GetType().Name);
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("<td>");
                                local_6.AppendLine(local_5.Value.ToString());
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("<td>");
                                var local_8 = GetObjectByteSize(local_5.Value);
                                local_6.AppendLine(local_8.ToString());
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("<td>");
                                local_6.AppendLine(GetObjectStringSize(local_5.Value));
                                local_6.AppendLine("</td>");
                                local_6.AppendLine("</tr>");
                                stringBuilder.Append(local_6);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (local_4 is IDisposable local_11)
                    {
                        local_11.Dispose();
                    }
                }
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get cache totals.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetCacheTotals()
        {
            var stringBuilder = new StringBuilder();
            var count = HttpRuntime.Cache.Count;
            var byteSize = 0L;
            lock (HttpRuntime.Cache)
            {
                var local_4 = HttpRuntime.Cache.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (local_4.MoveNext())
                        {
                            var local_5 = (DictionaryEntry)local_4.Current;
                            try
                            {
                                var local_6 = HttpRuntime.Cache[local_5.Key.ToString()];
                                byteSize += GetObjectByteSize(local_6);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (local_4 is IDisposable local_7)
                    {
                        local_7.Dispose();
                    }
                }
            }

            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Cache object totals");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<div class=\"table-responsive\">");
            stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");
            stringBuilder.Append("<thead class=\"thead-dark\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Total amount of objects");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Byte(s)");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Size");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("");
            stringBuilder.Append("</th>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");


            stringBuilder.Append("<tbody>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(count.ToString());
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(byteSize.ToString());
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(ConvertToStringSize(byteSize));
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            if (count > 0)
            {
                stringBuilder.Append($"<a href='{ProfilerURL}?detail=Cache'>view detail...</a>");
            }

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get hash object table.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetHashObjectTable(int hash)
        {
            var stringBuilder = new StringBuilder();
            if (!SessionObjects.Contains(hash))
            {
                stringBuilder.Append("The object was not found. Did it expire?");
            }
            else
            {
                stringBuilder.AppendLine("<h5 class=\"card-title\">");
                stringBuilder.Append($"Object value(s) - {hash}");
                stringBuilder.Append("</h5>");
                stringBuilder.Append(BuildObjectTree(SessionObjects[hash]));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get object byte size.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private static long GetObjectByteSize(object obj)
        {
            var num = 0L;
            try
            {
                if (obj != null)
                {
                    if (obj.GetType().IsSerializable)
                    {
                        var memoryStream = new MemoryStream();
                        new BinaryFormatter().Serialize(memoryStream, obj);
                        num = memoryStream.ToArray().LongLength;
                    }
                }
            }
            catch
            {
            }

            return num;
        }

        /// <summary>
        /// The get object string size.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetObjectStringSize(object obj)
        {
            var str = "0 byte(s)";
            try
            {
                if (obj != null)
                {
                    if (!obj.GetType().IsSerializable)
                    {
                        str = "Object not marked serializable";
                    }
                    else
                    {
                        var memoryStream = new MemoryStream();
                        new BinaryFormatter().Serialize(memoryStream, obj);
                        str = ConvertToStringSize(memoryStream.ToArray().LongLength);
                    }
                }
            }
            catch (Exception ex)
            {
                str = !ex.Message.Contains("not marked as serializable")
                    ? ex.Message
                    : "Object not marked serializable";
            }

            return str;
        }

        /// <summary>
        /// The get session object table.
        /// </summary>
        /// <param name="sessionID">
        /// The session id.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSessionObjectTable(string sessionID, string key)
        {
            var stringBuilder = new StringBuilder();
            var activeSessions = GetActiveSessions();
            if (!activeSessions.Contains(sessionID))
            {
                stringBuilder.Append("The session was not found. Did it expire?");
            }
            else
            {
                var stateItemCollection = (SessionStateItemCollection)activeSessions[sessionID];
                stringBuilder.Append("<h5 class=\"card-title\">");
                stringBuilder.Append($"Session state object value(s) - {key}");
                stringBuilder.Append("</h5>");
                var keys = stateItemCollection.Keys;
                lock (keys)
                {
                    var local_5 = keys.GetEnumerator();
                    try
                    {
                        while (true)
                        {
                            if (local_5.MoveNext())
                            {
                                var local_6 = (string)local_5.Current;
                                if (key == "_webprofiler" || local_6 != key)
                                {
                                    continue;
                                }

                                var local_7 = stateItemCollection[key];
                                stringBuilder.Append(BuildObjectTree(local_7));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        if (local_5 is IDisposable local_8)
                        {
                            local_8.Dispose();
                        }
                    }
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get session state totals.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSessionStateTotals()
        {
            var stringBuilder = new StringBuilder();
            var activeSessions = GetActiveSessions();
            var count = activeSessions.Count;
            var byteSize = 0L;
            lock (activeSessions)
            {
                var local_6 = activeSessions.GetEnumerator();
                try
                {
                    while (true)
                    {
                        var local_18_3 = true;
                        if (local_6.MoveNext())
                        {
                            var local_7 = (DictionaryEntry)local_6.Current;
                            try
                            {
                                var local_8 = (SessionStateItemCollection)local_7.Value;
                                var local_9 = local_8.Keys;
                                lock (local_9)
                                {
                                    var local_10 = local_9.GetEnumerator();
                                    try
                                    {
                                        while (true)
                                        {
                                            if (local_10.MoveNext())
                                            {
                                                var local_11 = (string)local_10.Current;
                                                try
                                                {
                                                    if (local_11 == "_webprofiler")
                                                    {
                                                        continue;
                                                    }

                                                    var local_12 = local_8[local_11];
                                                    byteSize += GetObjectByteSize(local_12);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        if (local_10 is IDisposable local_1)
                                        {
                                            local_1.Dispose();
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (local_6 is IDisposable local_1_1)
                    {
                        local_1_1.Dispose();
                    }
                }
            }

            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Active session state totals");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<div class=\"table-responsive\">");
            stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");
            stringBuilder.Append("<thead class=\"thead-dark\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Total amount of active sessions");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Byte(s)");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Size");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("");
            stringBuilder.Append("</th>");
            stringBuilder.Append("</tr>");

            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(count.ToString());
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(byteSize.ToString());
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append(ConvertToStringSize(byteSize));
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td>");
            if (count > 0)
            {
                stringBuilder.Append($"<a href='{ProfilerURL}?detail=SessionState'>view detail...</a>");
            }

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get session states table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSessionStatesTable()
        {
            var stringBuilder = new StringBuilder();

            var activeSessions = GetActiveSessions();

            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Active sessions");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<div class=\"table-responsive\">");
            stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");
            stringBuilder.Append("<thead class=\"thead-dark\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Internal session ID");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Item count");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Byte(s)");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("Size");
            stringBuilder.Append("</th>");
            stringBuilder.Append("<th scope=\"col\">");
            stringBuilder.Append("");
            stringBuilder.Append("</th>");
            stringBuilder.Append("</tr>");
            lock (activeSessions)
            {
                var local_4 = activeSessions.GetEnumerator();
                try
                {
                    while (true)
                    {
                        var local_23_3 = true;
                        if (local_4.MoveNext())
                        {
                            var local_5 = (DictionaryEntry)local_4.Current;
                            try
                            {
                                var local_6 = (SessionStateItemCollection)local_5.Value;
                                var local_8 = (local_6.Keys.Count - 1).ToString();
                                var local_9 = 0L;
                                var local_10 = new StringBuilder();
                                try
                                {
                                    var local_11 = local_6.Keys;
                                    lock (local_11)
                                    {
                                        var local_12 = local_11.GetEnumerator();
                                        try
                                        {
                                            while (true)
                                            {
                                                if (local_12.MoveNext())
                                                {
                                                    var local_13 = (string)local_12.Current;

                                                    try
                                                    {
                                                        if (local_13 == "_webprofiler")
                                                        {
                                                            continue;
                                                        }

                                                        var local_14 = local_6[local_13];
                                                        local_9 += GetObjectByteSize(local_14);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            if (local_12 is IDisposable local_1)
                                            {
                                                local_1.Dispose();
                                            }
                                        }
                                    }

                                    local_10.AppendLine("<tbody>");
                                    local_10.AppendLine("<tr>");
                                    local_10.AppendLine("<td>");
                                    local_10.AppendLine(local_5.Key.ToString());
                                    local_10.AppendLine("</td>");
                                    local_10.AppendLine("<td>");
                                    local_10.AppendLine(local_8);
                                    local_10.AppendLine("</td>");
                                    local_10.AppendLine("<td>");
                                    local_10.AppendLine(local_9.ToString());
                                    local_10.AppendLine("</td>");
                                    local_10.AppendLine("<td>");
                                    local_10.AppendLine(ConvertToStringSize(local_9));
                                    local_10.AppendLine("</td>");
                                    local_10.AppendLine("<td>");
                                    var local_15 = new[]
                                    {
                                        "<a href='", ProfilerURL, "?detail=SessionState&sessionID=",
                                        local_5.Key.ToString(), "'>view detail...</a>"
                                    };
                                    local_10.AppendLine(string.Concat(local_15));
                                    local_10.AppendLine("</td>");
                                    local_10.AppendLine("</tr>");
                                    stringBuilder.Append(local_10);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    if (local_4 is IDisposable local_1_1)
                    {
                        local_1_1.Dispose();
                    }
                }
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get session table.
        /// </summary>
        /// <param name="sessionID">
        /// The session id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSessionTable(string sessionID)
        {
            var stringBuilder = new StringBuilder();
            var activeSessions = GetActiveSessions();
            if (!activeSessions.Contains(sessionID))
            {
                stringBuilder.Append("The session was not found. Did it expire?");
            }
            else
            {
                var stateItemCollection = (SessionStateItemCollection)activeSessions[sessionID];

                stringBuilder.Append("<h5 class=\"card-title\">");
                stringBuilder.Append("Session state objects");
                stringBuilder.Append("</h6>");

                stringBuilder.Append("<div class=\"table-responsive\">");
                stringBuilder.Append("<table class=\"table tablesorter table-bordered table-striped\">");

                stringBuilder.Append("<thead class=\"thead-dark\">");

                stringBuilder.Append("<tr>");
                stringBuilder.Append("<th scope=\"col\">");
                stringBuilder.Append("Name");
                stringBuilder.Append("</th>");
                stringBuilder.Append("<th scope=\"col\">");
                stringBuilder.Append("Type");
                stringBuilder.Append("</th>");
                stringBuilder.Append("<th scope=\"col\">");
                stringBuilder.Append("Value");
                stringBuilder.Append("</th>");
                stringBuilder.Append("<th scope=\"col\">");
                stringBuilder.Append("Byte(s)");
                stringBuilder.Append("</th>");
                stringBuilder.Append("<th scope=\"col\">");
                stringBuilder.Append("Size");
                stringBuilder.Append("</th>");
                stringBuilder.Append("</tr>");

                stringBuilder.Append("<tbody>");
                var keys = stateItemCollection.Keys;
                lock (keys)
                {
                    var local_6 = keys.GetEnumerator();
                    try
                    {
                        while (true)
                        {
                            if (local_6.MoveNext())
                            {
                                var local_7 = (string)local_6.Current;
                                var local_8 = new StringBuilder();
                                try
                                {
                                    if (local_7 == "_webprofiler")
                                    {
                                        continue;
                                    }

                                    var local_9 = stateItemCollection[local_7];
                                    local_8.AppendLine("<tr>");
                                    if (!local_9.GetType().IsValueType && local_9.GetType().Name != "String" &&
                                        local_9.GetType().Name != "Guid" && local_9.GetType().Name != "DateTime")
                                    {
                                        local_8.AppendLine("<td>");
                                        var local_10 = new[]
                                        {
                                            "<a href='", ProfilerURL, "?detail=SessionState&sessionID=", sessionID,
                                            "&key=", local_7, "'>"
                                        };
                                        local_8.AppendLine(string.Concat(local_10));
                                        local_8.AppendLine(local_7);
                                        local_8.AppendLine("</a>");
                                        local_8.AppendLine("</td>");
                                    }
                                    else
                                    {
                                        local_8.AppendLine("<td>");
                                        local_8.AppendLine(local_7);
                                        local_8.AppendLine("</td>");
                                    }

                                    local_8.AppendLine("<td>");
                                    local_8.AppendLine(local_9.GetType().Name);
                                    local_8.AppendLine("</td>");
                                    local_8.AppendLine("<td>");
                                    local_8.AppendLine(local_9.ToString());
                                    local_8.AppendLine("</td>");
                                    local_8.AppendLine("<td>");
                                    var local_11 = GetObjectByteSize(local_9);
                                    local_8.AppendLine(local_11.ToString());
                                    local_8.AppendLine("</td>");
                                    local_8.AppendLine("<td>");
                                    local_8.AppendLine(GetObjectStringSize(local_9));
                                    local_8.AppendLine("</td>");
                                    local_8.AppendLine("</tr>");
                                    stringBuilder.Append(local_8);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        if (local_6 is IDisposable local_14)
                        {
                            local_14.Dispose();
                        }
                    }
                }

                stringBuilder.Append("</tbody>");
                stringBuilder.Append("</table>");
                stringBuilder.Append("</div>");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The get web information.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetWebInformation()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<h5 class=\"card-title\">");
            stringBuilder.Append("Web application runtime");
            stringBuilder.Append("</h5>");

            stringBuilder.Append("<ul class=\"list-group list-group-flush\">");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Application ID:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AppDomainAppId);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Application Path:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AppDomainAppPath);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Virtual Path:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AppDomainAppVirtualPath);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Domain ID:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AppDomainId);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Script Physical Path:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AspClientScriptPhysicalPath);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Script Virtual Path:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AspClientScriptVirtualPath);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Application Install Directory:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.AspInstallDirectory);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Bin Directory:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.BinDirectory);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Clr Install Directory:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.ClrInstallDirectory);
            stringBuilder.Append("</li>");

            stringBuilder.Append("<li class=\"list-group-item\">");
            stringBuilder.Append("<strong>Codegen Directory:</strong>&nbsp;");
            stringBuilder.Append(HttpRuntime.CodegenDir);
            stringBuilder.Append("</li>");

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }

        // attempt to get Asp.Net internal cache
        // adapted from https://stackoverflow.com/a/46554310/1086134
        /// <summary>
        /// The get asp net internal cache obj.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object GetAspNetInternalCacheObj()
        {
            object aspNetCacheInternal;

            var cacheInternalPropInfo = typeof(HttpRuntime).GetProperty(
                "CacheInternal",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (cacheInternalPropInfo != null)
            {
                aspNetCacheInternal = cacheInternalPropInfo.GetValue(null, null);
                return aspNetCacheInternal;
            }

            // At some point, after some .NET Framework's security update, that internal member disappeared.
            // https://stackoverflow.com/a/45045160
            // 
            // We need to look for internal cache otherwise.
            //
            var cacheInternalFieldInfo = HttpRuntime.Cache.GetType().GetField(
                "_internalCache",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (cacheInternalFieldInfo == null)
                return null;

            var httpRuntimeInternalCache = cacheInternalFieldInfo.GetValue(HttpRuntime.Cache);
            var httpRuntimeInternalCacheField = httpRuntimeInternalCache.GetType().GetField(
                "_cacheInternal",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (httpRuntimeInternalCacheField == null)
                return null;

            aspNetCacheInternal = httpRuntimeInternalCacheField.GetValue(httpRuntimeInternalCache);
            return aspNetCacheInternal;
        }

        #endregion
    }
}