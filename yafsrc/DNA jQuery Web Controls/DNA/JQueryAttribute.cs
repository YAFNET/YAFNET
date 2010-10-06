
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// JQueryAttribute for jQuery Web Control
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class JQueryAttribute : Attribute
    {
        private string name = "";
        private ClientRegisterEvents startEvent = ClientRegisterEvents.ApplicaitonInit;
        private string scriptResourceBaseName = "jQueryNet.";
        private string _assembly = "";
        private string _disposeMethod = "";

        /// <summary>
        /// Gets/Sets the jQuery dispose method name.
        /// </summary>
        /// <remarks>
        /// if this property sets the dispose method for jquery will invoke when application.unload.
        /// </remarks>
        public string DisposeMethod
        {
            get { return _disposeMethod; }
            set { _disposeMethod = value; }
        }

        /// <summary>
        /// Gets/Sets the jQuery scriptResource file base name
        /// </summary>
        public string ScriptResourceBaseName
        {
            get { return scriptResourceBaseName; }
            set { scriptResourceBaseName = value; }
        }

        /// <summary>
        /// Gets/Sets the script resource 's assembly name
        /// </summary>
        public string Assembly
        {
            get { return _assembly; }
            set { _assembly = value; }
        }

        /// <summary>
        /// Gets/Sets the script resource name array
        /// </summary>
        public string[] ScriptResources { get; set; }

        /// <summary>
        /// Gets/Sets the client register event for jQuery 
        /// </summary>
        public ClientRegisterEvents StartEvent
        {
            get { return startEvent; }
            set { startEvent = value; }
        }

        /// <summary>
        /// Gets/Sets the jQuery (plugins/widgets) 's name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
