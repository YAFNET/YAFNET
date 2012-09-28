/* YetAnotherForum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Extensions;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    /// The load message.
    /// </summary>
    public class LoadMessage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _load string list.
        /// </summary>
        private readonly List<MessageNotification> _loadStringList = new List<MessageNotification>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadMessage" /> class.
        /// </summary>
        public LoadMessage()
        {
            if (this.SessionLoadString.Any())
            {
                // get this as the current "loadstring"
                this._loadStringList.AddRange(this.SessionLoadString);

                // session load string no longer needed
                this.SessionLoadString.Clear();
            }

            YafContext.Current.Unload += this.Current_Unload;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets LoadString.
        /// </summary>
        /*public string LoadString
        {
            get
            {
                return !this.LoadStringList.Any() ? string.Empty : this.LoadStringDelimited("\r\n");
            }
        }*/

        /// <summary>
        ///   Gets LoadStringList.
        /// </summary>
        [NotNull]
        public List<MessageNotification> LoadStringList
        {
            get
            {
                return this._loadStringList;
            }
        }

        /*
        /// <summary>
        ///   Gets StringJavascript.
        /// </summary>
        public string StringJavascript
        {
            get
            {
                return CleanJsString(this.LoadString);
            }
        }*/

        /// <summary>
        /// Gets the session load string.
        /// </summary>
        protected List<MessageNotification> SessionLoadString
        {
            get
            {
                if (YafContext.Current.Get<HttpSessionStateBase>()["LoadStringList"] == null)
                {
                    YafContext.Current.Get<HttpSessionStateBase>()["LoadStringList"] = new List<MessageNotification>();
                }

                return YafContext.Current.Get<HttpSessionStateBase>()["LoadStringList"] as List<MessageNotification>;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cleans the js string.
        /// </summary>
        /// <param name="jsString">The js string.</param>
        /// <returns>
        /// The clean js string.
        /// </returns>
        [NotNull]
        public static string CleanJsString([NotNull] string jsString)
        {
            string message = jsString;
            message = message.Replace("\\", "\\\\");
            message = message.Replace("'", "\\'");
            message = message.Replace("\r\n", "\\r\\n");
            message = message.Replace("\n", "\\n");
            message = message.Replace("\"", "\\\"");
            return message;
        }

        /// <summary>
        /// AddLoadMessage creates a message that will be returned on the next page load.
        /// </summary>
        /// <param name="message">The message you wish to display.</param>
        /// <param name="messageType">Type of the message.</param>
        public void Add([NotNull] string message, MessageTypes messageType)
        {
            this.LoadStringList.Add(new MessageNotification(message, messageType));
        }

        /// <summary>
        /// AddLoadMessageSession creates a message that will be returned on the next page.
        /// </summary>
        /// <param name="message">The message you wish to display.</param>
        /// <param name="messageType">Type of the message.</param>
        public void AddSession([NotNull] string message, MessageTypes messageType)
        {
            // add it too the session list...
            this.SessionLoadString.Add(new MessageNotification(message, messageType));
        }

        /// <summary>
        /// Clear the Load String (error) List
        /// </summary>
        public void Clear()
        {
            this.LoadStringList.Clear();
        }

        /*
        /// <summary>
        /// Loads the string delimited.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>
        /// The load string delimited.
        /// </returns>
        public string LoadStringDelimited([NotNull] string delimiter)
        {
            return !this.LoadStringList.Any()
                       ? string.Empty
                       : this.LoadStringList.Aggregate((current, next) => current + delimiter + next);
        }
        */

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>Returns the Current Message</returns>
        public MessageNotification GetMessage()
        {
            return !this.LoadStringList.Any()
                       ? null
                       : this.LoadStringList.First();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear the load message...
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Current_Unload([NotNull] object sender, [NotNull] EventArgs e)
        {
            // clear the load message...
            this.Clear();
        }

        #endregion
    }
}