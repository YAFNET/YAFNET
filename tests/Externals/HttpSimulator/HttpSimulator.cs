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
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.SessionState;

    /// <summary>
    /// The http verb.
    /// </summary>
    public enum HttpVerb
    {
        /// <summary>
        /// The get.
        /// </summary>
        GET, 

        /// <summary>
        /// The head.
        /// </summary>
        HEAD, 

        /// <summary>
        /// The post.
        /// </summary>
        POST, 

        /// <summary>
        /// The put.
        /// </summary>
        PUT, 

        /// <summary>
        /// The delete.
        /// </summary>
        DELETE, 
    }

    /// <summary>
    /// Useful class for simulating the HttpContext. This does not actually make an HttpRequest, it merely simulates the state that your code would be in "as if" handling a request. Thus the HttpContext.Current property is populated.
    /// </summary>
    public class HttpSimulator : IDisposable
    {
        /// <summary>
        /// The default physical app path.
        /// </summary>
        private const string DefaultPhysicalAppPath = @"c:\InetPub\wwwRoot\";

        /// <summary>
        /// The _form vars.
        /// </summary>
        private readonly NameValueCollection formVars = new NameValueCollection();

        /// <summary>
        /// The headers.
        /// </summary>
        private readonly NameValueCollection headers = new NameValueCollection();

        /// <summary>
        /// The referer.
        /// </summary>
        private Uri referer;

        /// <summary>
        /// The application path.
        /// </summary>
        private string applicationPath = "/";

        /// <summary>
        /// The builder.
        /// </summary>
        private StringBuilder builder;

        /// <summary>
        /// The physical application path.
        /// </summary>
        private string physicalApplicationPath = DefaultPhysicalAppPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSimulator"/> class.
        /// </summary>
        public HttpSimulator()
            : this("/", DefaultPhysicalAppPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSimulator"/> class.
        /// </summary>
        /// <param name="applicationPath">
        /// The application path.
        /// </param>
        public HttpSimulator(string applicationPath)
            : this(applicationPath, DefaultPhysicalAppPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSimulator"/> class.
        /// </summary>
        /// <param name="applicationPath">
        /// The application path.
        /// </param>
        /// <param name="physicalApplicationPath">
        /// The physical application path.
        /// </param>
        public HttpSimulator(string applicationPath, string physicalApplicationPath)
        {
            this.ApplicationPath = applicationPath;
            this.PhysicalApplicationPath = physicalApplicationPath;
        }

        /// <summary>
        /// Gets or sets The same thing as the IIS Virtual directory. It's what gets returned by Request.ApplicationPath.
        /// </summary>
        /// <value>
        /// The application path.
        /// </value>
        public string ApplicationPath
        {
            get => this.applicationPath;

            set
            {
                this.applicationPath = value ?? "/";
                this.applicationPath = NormalizeSlashes(this.applicationPath);
            }
        }

        /// <summary>
        /// Gets Host.
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Gets LocalPath.
        /// </summary>
        public string LocalPath { get; private set; }

        /// <summary>
        ///   Gets the Portion of the URL after the application.
        /// </summary>
        public string Page { get; private set; }

        /// <summary>
        /// Gets or sets the Physical path to the application (used for simulation purposes).
        /// </summary>
        /// <value>
        /// The physical application path.
        /// </value>
        public string PhysicalApplicationPath
        {
            get => this.physicalApplicationPath;

            set
            {
                this.physicalApplicationPath = value ?? DefaultPhysicalAppPath;

                // strip trailing backslashes.
                this.physicalApplicationPath = StripTrailingBackSlashes(this.physicalApplicationPath) + @"\";
            }
        }

        /// <summary>
        ///   Gets the Physical path to the requested file (used for simulation purposes).
        /// </summary>
        public string PhysicalPath { get; private set; } = DefaultPhysicalAppPath;

        /// <summary>
        /// Gets Port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        ///   Returns the text from the response to the simulated request.
        /// </summary>
        public string ResponseText => (this.builder ?? new StringBuilder()).ToString();

        /// <summary>
        /// Gets or sets ResponseWriter.
        /// </summary>
        public TextWriter ResponseWriter { get; set; }

        /// <summary>
        /// Gets WorkerRequest.
        /// </summary>
        public SimulatedHttpRequest WorkerRequest { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current = null;
            }
        }

        /// <summary>
        /// Sets a form variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public HttpSimulator SetFormVariable(string name, string value)
        {
            // TODO: Change this ordering requirement.
            if (this.WorkerRequest != null)
            {
                throw new InvalidOperationException("Cannot set form variables after calling Simulate().");
            }

            this.formVars.Add(name, value);

            return this;
        }

        /// <summary>
        /// Sets a header value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public HttpSimulator SetHeader(string name, string value)
        {
            // TODO: Change this ordering requirement.
            if (this.WorkerRequest != null)
            {
                throw new InvalidOperationException("Cannot set headers after calling Simulate().");
            }

            this.headers.Add(name, value);

            return this;
        }

        /// <summary>
        /// Sets the referer for the request. Uses a fluent interface.
        /// </summary>
        /// <param name="referer">
        /// </param>
        /// <returns>
        /// </returns>
        public HttpSimulator SetReferer(Uri referer)
        {
            this.WorkerRequest?.SetReferer(referer);

            this.referer = referer;
            return this;
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a GET request.
        /// </summary>
        /// <remarks>
        /// Simulates a request to http://localhost/
        /// </remarks>
        public HttpSimulator SimulateRequest()
        {
            return this.SimulateRequest(new Uri("http://localhost/"));
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a GET request.
        /// </summary>
        /// <param name="url">
        /// </param>
        public HttpSimulator SimulateRequest(bool isMobileDevice)
        {
            return this.SimulateRequest(new Uri("http://localhost/"), isMobileDevice);
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a GET request.
        /// </summary>
        /// <param name="url">
        /// </param>
        public HttpSimulator SimulateRequest(Uri url, bool isMobileDevice = false)
        {
            return this.SimulateRequest(url, HttpVerb.GET, isMobileDevice);
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <returns></returns>
        public HttpSimulator SimulateRequest(Uri url, HttpVerb httpVerb, bool isMobileDevice = false)
        {
            return this.SimulateRequest(url, httpVerb, null, null, isMobileDevice);
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a POST request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formVariables">The form variables.</param>
        /// <returns></returns>
        public HttpSimulator SimulateRequest(Uri url, NameValueCollection formVariables)
        {
            return this.SimulateRequest(url, HttpVerb.POST, formVariables, null);
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a POST request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="formVariables">The form variables.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public HttpSimulator SimulateRequest(Uri url, NameValueCollection formVariables, NameValueCollection headers)
        {
            return this.SimulateRequest(url, HttpVerb.POST, formVariables, headers);
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public HttpSimulator SimulateRequest(Uri url, HttpVerb httpVerb, NameValueCollection headers)
        {
            return this.SimulateRequest(url, httpVerb, null, headers);
        }

        /// <summary>
        /// The get hosting environment.
        /// </summary>
        /// <returns></returns>
        protected static HostingEnvironment GetHostingEnvironment()
        {
            HostingEnvironment environment;
            try
            {
                environment = new HostingEnvironment();
            }
            catch (InvalidOperationException)
            {
                // Shoot, we need to grab it via reflection.
                environment = ReflectionHelper.GetStaticFieldValue<HostingEnvironment>(
                    "_theHostingEnvironment", typeof(HostingEnvironment));
            }

            return environment;
        }

        /// <summary>
        /// The normalize slashes.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The normalize slashes.
        /// </returns>
        protected static string NormalizeSlashes(string s)
        {
            if (string.IsNullOrEmpty(s) || s == "/")
            {
                return "/";
            }

            s = s.Replace(@"\", "/");

            // Reduce multiple slashes in row to single.
            var normalized = Regex.Replace(s, "(/)/+", "$1");

            // Strip left.
            normalized = StripPrecedingSlashes(normalized);

            // Strip right.
            normalized = StripTrailingSlashes(normalized);
            return "/" + normalized;
        }

        /// <summary>
        /// The strip preceding slashes.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The strip preceding slashes.
        /// </returns>
        protected static string StripPrecedingSlashes(string s)
        {
            return Regex.Replace(s, "^/*(.*)", "$1");
        }

        /// <summary>
        /// The strip trailing back slashes.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The strip trailing back slashes.
        /// </returns>
        protected static string StripTrailingBackSlashes(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return Regex.Replace(s, @"(.*)\\*$", "$1", RegexOptions.RightToLeft);
        }

        /// <summary>
        /// The strip trailing slashes.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The strip trailing slashes.
        /// </returns>
        protected static string StripTrailingSlashes(string s)
        {
            return Regex.Replace(s, "(.*)/*$", "$1", RegexOptions.RightToLeft);
        }

        /// <summary>
        /// Sets up the HttpContext objects to simulate a request.
        /// </summary>
        /// <param name="url">
        /// </param>
        /// <param name="httpVerb">
        /// </param>
        /// <param name="formVariables">
        /// </param>
        /// <param name="headers">
        /// </param>
        protected virtual HttpSimulator SimulateRequest(
            Uri url, HttpVerb httpVerb, NameValueCollection formVariables, NameValueCollection headers, bool isMobileDevice = false)
        {
            HttpContext.Current = null;

            this.ParseRequestUrl(url);

            if (this.ResponseWriter == null)
            {
                this.builder = new StringBuilder();
                this.ResponseWriter = new StringWriter(this.builder);
            }

            this.SetHttpRuntimeInternals();

            var query = ExtractQueryStringPart(url);

            if (formVariables != null)
            {
                this.formVars.Add(formVariables);
            }

            if (this.formVars.Count > 0)
            {
                httpVerb = HttpVerb.POST; // Need to enforce this.
            }

            if (headers != null)
            {
                this.headers.Add(headers);
            }

            this.WorkerRequest = new SimulatedHttpRequest(
                this.ApplicationPath, 
                this.PhysicalApplicationPath, 
                this.PhysicalPath, 
                this.Page, 
                query, 
                this.ResponseWriter, 
                this.Host, 
                this.Port, 
                httpVerb.ToString());

            this.WorkerRequest.Form.Add(this.formVars);
            this.WorkerRequest.Headers.Add(this.headers);

            if (this.referer != null)
            {
                this.WorkerRequest.SetReferer(this.referer);
            }

            this.InitializeSession(isMobileDevice);

            InitializeApplication();

            Console.WriteLine("host: {0}", this.Host);
            Console.WriteLine("virtualDir: {0}", this.applicationPath);
            Console.WriteLine("page: {0}", this.LocalPath);
            Console.WriteLine("pathPartAfterApplicationPart: {0}", this.Page);
            Console.WriteLine("appPhysicalDir: {0}", this.physicalApplicationPath);
            Console.WriteLine("Request.Url.LocalPath: {0}", HttpContext.Current.Request.Url.LocalPath);
            Console.WriteLine("Request.Url.Host: {0}", HttpContext.Current.Request.Url.Host);
            Console.WriteLine("Request.FilePath: {0}", HttpContext.Current.Request.FilePath);
            Console.WriteLine("Request.Path: " + HttpContext.Current.Request.Path);
            Console.WriteLine("Request.RawUrl: " + HttpContext.Current.Request.RawUrl);
            Console.WriteLine("Request.Url: " + HttpContext.Current.Request.Url);
            Console.WriteLine("Request.Url.Port: " + HttpContext.Current.Request.Url.Port);
            Console.WriteLine("Request.ApplicationPath: " + HttpContext.Current.Request.ApplicationPath);
            Console.WriteLine("Request.PhysicalPath: " + HttpContext.Current.Request.PhysicalPath);
            Console.WriteLine("HttpRuntime.AppDomainAppPath: " + HttpRuntime.AppDomainAppPath);
            Console.WriteLine("HttpRuntime.AppDomainAppVirtualPath: " + HttpRuntime.AppDomainAppVirtualPath);
            Console.WriteLine(
                "HostingEnvironment.ApplicationPhysicalPath: " + HostingEnvironment.ApplicationPhysicalPath);
            Console.WriteLine("HostingEnvironment.ApplicationVirtualPath: " + HostingEnvironment.ApplicationVirtualPath);

            return this;
        }

        /// <summary>
        /// The extract query string part.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The extract query string part.
        /// </returns>
        private static string ExtractQueryStringPart(Uri url)
        {
            var query = url.Query;
            return query.StartsWith("?") ? query.Substring(1) : query;
        }

        /// <summary>
        /// The initialize application.
        /// </summary>
        private static void InitializeApplication()
        {
            var appFactoryType =
                Type.GetType(
                    "System.Web.HttpApplicationFactory, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var appFactory = ReflectionHelper.GetStaticFieldValue<object>("_theApplicationFactory", appFactoryType);
            ReflectionHelper.SetPrivateInstanceFieldValue("_state", appFactory, HttpContext.Current.Application);
        }

        /// <summary>
        /// The right after.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <param name="search">
        /// The search.
        /// </param>
        /// <returns>
        /// The right after.
        /// </returns>
        private static string RightAfter(string original, string search)
        {
            if (search.Length > original.Length || search.Length == 0)
            {
                return original;
            }

            var searchIndex = original.IndexOf(search, 0, StringComparison.InvariantCultureIgnoreCase);

            return searchIndex < 0 ? original : original.Substring(original.IndexOf(search) + search.Length);
        }

        /// <summary>
        /// The initialize session.
        /// </summary>
        /// <param name="isMobileDevice">if set to <c>true</c> [is mobile device].</param>
        private void InitializeSession(bool isMobileDevice = false)
        {
            HttpContext.Current = new HttpContext(this.WorkerRequest);
            HttpContext.Current.Items.Clear();
            var session =
                (HttpSessionState)
                ReflectionHelper.Instantiate(
                    typeof(HttpSessionState), new[] { typeof(IHttpSessionState) }, new FakeHttpSessionState());

            var browserCaps = new HttpBrowserCapabilities();

            var values = new Hashtable(20, StringComparer.OrdinalIgnoreCase) { ["ecmascriptversion"] = "3.0" };

            if (isMobileDevice)
            {
                values["Type"] = "YAFMobile1";
                values["Name"] = "YAFMobile";
                values["IsMobileDevice"] = "True";
                values["Platform"] = "yafiphone";
                values["Is Win16"] = "False";
                values["Is Win32"] = "False";
            }
            else
            {
                values["Type"] = "Chrome18";
                values["Name"] = "Chrome";
                values["IsMobileDevice"] = "False";
                values["Platform"] = "WinNT";
                values["Is Win16"] = "False";
                values["Is Win32"] = "True";
            }

            values["Browser"] = "YAF";
            values["Version"] = "3.0";
            values["Major Version"] = "3";
            values["Minor Version"] = "0";
            values["Is Beta"] = "False";
            values["Is Crawler"] = "False";
            values["Is AOL"] = "False";
            
            values["Supports Frames"] = "True";
            values["Supports Tables"] = "True";
            values["Supports Cookies"] = "True";
            values["Supports VB Script"] = "False";
            values["Supports JavaScript"] = "True";
            values["Supports Java Applets"] = "True";
            values["Supports ActiveX Controls"] = "False";
            values["CDF"] = "False";
            
            values["Crawler"] = "False";

            browserCaps.Capabilities = values;

            HttpContext.Current.Request.Browser = browserCaps;

            HttpContext.Current.Items.Add("AspSession", session);
        }

        /// <summary>
        /// The parse request url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        private void ParseRequestUrl(Uri url)
        {
            if (url == null)
            {
                return;
            }

            this.Host = url.Host;
            this.Port = url.Port;
            this.LocalPath = url.LocalPath;
            this.Page = StripPrecedingSlashes(RightAfter(url.LocalPath, this.ApplicationPath));
            this.PhysicalPath = Path.Combine(this.physicalApplicationPath, this.Page.Replace("/", @"\"));
        }

        /// <summary>
        /// The set http runtime internals.
        /// </summary>
        private void SetHttpRuntimeInternals()
        {
            // We cheat by using reflection.

            // get singleton property value
            var runtime = ReflectionHelper.GetStaticFieldValue<HttpRuntime>("_theRuntime", typeof(HttpRuntime));

            // set app path property value
            ReflectionHelper.SetPrivateInstanceFieldValue("_appDomainAppPath", runtime, this.PhysicalApplicationPath);

            // set app virtual path property value
            const string vpathTypeName = "System.Web.VirtualPath, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            var virtualPath = ReflectionHelper.Instantiate(
                vpathTypeName, new[] { typeof(string) }, new object[] { this.ApplicationPath });
            ReflectionHelper.SetPrivateInstanceFieldValue("_appDomainAppVPath", runtime, virtualPath);

            // set codegen dir property value
            ReflectionHelper.SetPrivateInstanceFieldValue("_codegenDir", runtime, this.PhysicalApplicationPath);

            var environment = GetHostingEnvironment();
            ReflectionHelper.SetPrivateInstanceFieldValue("_appPhysicalPath", environment, this.PhysicalApplicationPath);
            ReflectionHelper.SetPrivateInstanceFieldValue("_appVirtualPath", environment, virtualPath);
            ReflectionHelper.SetPrivateInstanceFieldValue("_configMapPath", environment, new ConfigMapPath(this));
        }

        /// <summary>
        /// The config map path.
        /// </summary>
        public class ConfigMapPath : IConfigMapPath
        {
            /// <summary>
            /// The _request simulation.
            /// </summary>
            private readonly HttpSimulator _requestSimulation;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigMapPath"/> class.
            /// </summary>
            /// <param name="simulation">
            /// The simulation.
            /// </param>
            public ConfigMapPath(HttpSimulator simulation)
            {
                this._requestSimulation = simulation;
            }

            /// <summary>
            /// The get app path for path.
            /// </summary>
            /// <param name="siteID">
            /// The site id.
            /// </param>
            /// <param name="path">
            /// The path.
            /// </param>
            /// <returns>
            /// The get app path for path.
            /// </returns>
            public string GetAppPathForPath(string siteID, string path)
            {
                return this._requestSimulation.ApplicationPath;
            }

            /// <summary>
            /// The get default site name and id.
            /// </summary>
            /// <param name="siteName">
            /// The site name.
            /// </param>
            /// <param name="siteID">
            /// The site id.
            /// </param>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public void GetDefaultSiteNameAndID(out string siteName, out string siteID)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// The get machine config filename.
            /// </summary>
            /// <returns>
            /// The get machine config filename.
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public string GetMachineConfigFilename()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// The get path config filename.
            /// </summary>
            /// <param name="siteID">
            /// The site id.
            /// </param>
            /// <param name="path">
            /// The path.
            /// </param>
            /// <param name="directory">
            /// The directory.
            /// </param>
            /// <param name="baseName">
            /// The base name.
            /// </param>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// The get root web config filename.
            /// </summary>
            /// <returns>
            /// The get root web config filename.
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public string GetRootWebConfigFilename()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// The map path.
            /// </summary>
            /// <param name="siteID">
            /// The site id.
            /// </param>
            /// <param name="path">
            /// The path.
            /// </param>
            /// <returns>
            /// The map path.
            /// </returns>
            public string MapPath(string siteID, string path)
            {
                var page = StripPrecedingSlashes(RightAfter(path, this._requestSimulation.ApplicationPath));
                return Path.Combine(this._requestSimulation.PhysicalApplicationPath, page.Replace("/", @"\"));
            }

            /// <summary>
            /// The resolve site argument.
            /// </summary>
            /// <param name="siteArgument">
            /// The site argument.
            /// </param>
            /// <param name="siteName">
            /// The site name.
            /// </param>
            /// <param name="siteID">
            /// The site id.
            /// </param>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The fake http session state.
        /// </summary>
        public class FakeHttpSessionState : NameObjectCollectionBase, IHttpSessionState
        {
            ///<summary>
            ///  Gets or sets the code-page identifier for the current session.
            ///</summary>
            ///<returns> The code-page identifier for the current session. </returns>
            public int CodePage { get; set; }

            ///<summary>
            ///  Gets a value that indicates whether the application is configured for cookieless sessions.
            ///</summary>
            ///<returns> One of the <see cref="T:System.Web.HttpCookieMode"></see> values that indicate whether the application is configured for cookieless sessions. The default is <see
            ///   cref="F:System.Web.HttpCookieMode.UseCookies"></see> . </returns>
            public HttpCookieMode CookieMode => HttpCookieMode.UseCookies;

            ///<summary>
            ///  Gets a value indicating whether the session ID is embedded in the URL or stored in an HTTP cookie.
            ///</summary>
            ///<returns> true if the session is embedded in the URL; otherwise, false. </returns>
            public bool IsCookieless => false;

            ///<summary>
            ///  Gets a value indicating whether the session was created with the current request.
            ///</summary>
            ///<returns> true if the session was created with the current request; otherwise, false. </returns>
            public bool IsNewSession { get; } = true;

            ///<summary>
            ///  Gets a value indicating whether access to the collection of session-state values is synchronized (thread safe).
            ///</summary>
            ///<returns> true if access to the collection is synchronized (thread safe); otherwise, false. </returns>
            public bool IsSynchronized => true;

            ///<summary>
            ///  Gets or sets the locale identifier (LCID) of the current session.
            ///</summary>
            ///<returns> A <see cref="T:System.Globalization.CultureInfo"></see> instance that specifies the culture of the current session. </returns>
            public int LCID { get; set; }

            ///<summary>
            ///  Gets the current session-state mode.
            ///</summary>
            ///<returns> One of the <see cref="T:System.Web.SessionState.SessionStateMode"></see> values. </returns>
            public SessionStateMode Mode => SessionStateMode.InProc;

            ///<summary>
            ///  Gets the unique session identifier for the session.
            ///</summary>
            ///<returns> The session ID. </returns>
            public string SessionID { get; } = Guid.NewGuid().ToString().Remove(24);

            ///<summary>
            ///  Gets a collection of objects declared by &lt;object Runat="Server" Scope="Session"/&gt; tags within the ASP.NET application file Global.asax.
            ///</summary>
            ///<returns> An <see cref="T:System.Web.HttpStaticObjectsCollection"></see> containing objects declared in the Global.asax file. </returns>
            public HttpStaticObjectsCollection StaticObjects { get; } = new HttpStaticObjectsCollection();

            ///<summary>
            ///  Gets an object that can be used to synchronize access to the collection of session-state values.
            ///</summary>
            ///<returns> An object that can be used to synchronize access to the collection. </returns>
            public object SyncRoot { get; } = new object();

            ///<summary>
            ///  Gets and sets the time-out period (in minutes) allowed between requests before the session-state provider terminates the session.
            ///</summary>
            ///<returns> The time-out period, in minutes. </returns>
            public int Timeout { get; set; } = 30;

            ///<summary>
            ///  Gets a value indicating whether the session is read-only.
            ///</summary>
            ///<returns> true if the session is read-only; otherwise, false. </returns>
            bool IHttpSessionState.IsReadOnly => true;

            ///<summary>
            ///  Gets or sets a session-state item value by name.
            ///</summary>
            ///<returns> The session-state item value specified in the name parameter. </returns>
            ///<param name="name"> The key name of the session-state item value. </param>
            public object this[string name]
            {
                get => this.BaseGet(name);

                set => this.BaseSet(name, value);
            }

            ///<summary>
            ///  Gets or sets a session-state item value by numerical index.
            ///</summary>
            ///<returns> The session-state item value specified in the index parameter. </returns>
            ///<param name="index"> The numerical index of the session-state item value. </param>
            public object this[int index]
            {
                get => this.BaseGet(index);

                set => this.BaseSet(index, value);
            }

            /// <summary>
            /// Ends the current session.
            /// </summary>
            public void Abandon()
            {
                this.BaseClear();
            }

            /// <summary>
            /// Adds a new item to the session-state collection.
            /// </summary>
            /// <param name="name">
            /// The name of the item to add to the session-state collection. 
            /// </param>
            /// <param name="value">
            /// The value of the item to add to the session-state collection. 
            /// </param>
            public void Add(string name, object value)
            {
                this.BaseAdd(name, value);
            }

            /// <summary>
            /// Clears all values from the session-state item collection.
            /// </summary>
            public void Clear()
            {
                this.BaseClear();
            }

            /// <summary>
            /// Copies the collection of session-state item values to a one-dimensional array, starting at the specified index in the array.
            /// </summary>
            /// <param name="array">
            /// The <see cref="T:System.Array"></see> that receives the session values. 
            /// </param>
            /// <param name="index">
            /// The index in array where copying starts. 
            /// </param>
            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Deletes an item from the session-state item collection.
            /// </summary>
            /// <param name="name">
            /// The name of the item to delete from the session-state item collection. 
            /// </param>
            public void Remove(string name)
            {
                this.BaseRemove(name);
            }

            /// <summary>
            /// Clears all values from the session-state item collection.
            /// </summary>
            public void RemoveAll()
            {
                this.BaseClear();
            }

            /// <summary>
            /// Deletes an item at a specified index from the session-state item collection.
            /// </summary>
            /// <param name="index">
            /// The index of the item to remove from the session-state collection. 
            /// </param>
            public void RemoveAt(int index)
            {
                this.BaseRemoveAt(index);
            }
        }
    }
}