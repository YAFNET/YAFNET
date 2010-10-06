///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Permissions;
using System.Drawing.Design;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DialogButton
    {
        private string text = "";
        private string commandName = "";
        private string commandArgument = "";
        private string onClientClick = "";

        /// <summary>
        /// Gets/Sets the command name
        /// </summary>
        [Category("Action")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the command name")]
        public string CommandName
        {
            get { return commandName; }
            set { commandName = value; }
        }

        /// <summary>
        /// Gets/Sets the command arguments
        /// </summary>
        [Category("Action")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the command arguments")]
        public string CommandArgument
        {
            get { return commandArgument; }
            set { commandArgument = value; }
        }

        /// <summary>
        /// Gets/Sets the Button's client event handler
        /// </summary>
        [Category("Behavior")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the Button's client event handler")]
        public string OnClientClick
        {
            get { return onClientClick; }
            set { onClientClick = value; }
        }

        /// <summary>
        /// Gets/Sets the display text of button
        /// </summary>
        [Category("Data")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the display text of button")]
        [Localizable(true)]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


    }
}
