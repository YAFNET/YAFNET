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
namespace YAF.Core.Tasks
{
    #region Using

    using System;
    using System.Diagnostics;

    using YAF.Types.Attributes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Sends Email in the background.
    /// </summary>
    public class MailSendTask : IntermittentBackgroundTask
    {
        #region Constants and Fields

        /// <summary>
        ///   The _send mail threaded.
        /// </summary>
        [Inject]
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MailSendTask" /> class.
        /// </summary>
        public MailSendTask()
        {
            // set the unique value...
            var rand = new Random();

            // set interval values...
            this.RunPeriodMs = (rand.Next(30) + 15) * 1000;
            this.StartDelayMs = (rand.Next(30) + 15) * 1000;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            Debug.WriteLine("Running Send Mail Thread Under {0}...".FormatWith(Environment.UserName));

            this.ServiceLocator.Get<ISendMailThreaded>().SendThreaded();
        }

        #endregion
    }
}