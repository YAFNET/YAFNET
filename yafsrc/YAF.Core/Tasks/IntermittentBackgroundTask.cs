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
namespace YAF.Core.Tasks
{
    using System.Security.Principal;
    using System.Threading;

    using YAF.Types.Extensions;
    
    /// <summary>
    /// The intermittent background task.
    /// </summary>
    public class IntermittentBackgroundTask : BaseBackgroundTask
    {
        /// <summary>
        /// The _intermittent timer.
        /// </summary>
        protected Timer _intermittentTimer;

        /// <summary>
        /// The _primary thread identity
        /// </summary>
        private WindowsIdentity _primaryThreadIdentity;

        /// <summary>
        /// Gets or sets StartDelayMs.
        /// </summary>
        public long StartDelayMs { get; set; }

        /// <summary>
        /// Gets or sets RunPeriodMs.
        /// </summary>
        public long RunPeriodMs { get; set; }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
        }

        /// <summary>
        /// The run.
        /// </summary>
        public override void Run()
        {
            if (this.IsRunning)
            {
                return;
            }

            // keep the context...
            this._primaryThreadIdentity = WindowsIdentity.GetCurrent();

            // we're running this thread now...
            this.IsRunning = true;

            this.Logger.Debug("Starting Background Task {0} Now", this.GetType().Name);

            // create the timer...);
            this._intermittentTimer = new Timer(this.TimerCallback, null, this.StartDelayMs, this.RunPeriodMs);
        }

        /// <summary>
        /// The timer callback.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        protected virtual void TimerCallback(object sender)
        {
            if (!Monitor.TryEnter(this))
            {
                return;
            }

            WindowsImpersonationContext impersonationContext = null;

            if (this._primaryThreadIdentity != null)
            {
                impersonationContext = this._primaryThreadIdentity.Impersonate();
            }

            try
            {
                this.RunOnce();
            }
            finally
            {
                Monitor.Exit(this);

                impersonationContext?.Undo();
            }
        }
    }
}