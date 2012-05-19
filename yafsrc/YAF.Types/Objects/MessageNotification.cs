/* Yet Another Forum.NET
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
namespace YAF.Types.Objects
{
    #region Using

    using System;

    using YAF.Types.Constants;

    #endregion

    /// <summary>
    /// The Message Notification
    /// </summary>
    [Serializable]
    public class MessageNotification
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotification"/> class.
        /// </summary>
        public MessageNotification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotification"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        public MessageNotification(string message, MessageTypes messageType)
        {
            this.Message = message;
            this.MessageType = messageType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public MessageTypes MessageType { get; set; }

        #endregion
    }
}