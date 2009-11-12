/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Diagnostics;
using YAF.Classes.Core;

namespace YAF.Modules
{
  /// <summary>
  /// The mail sending module.
  /// </summary>
  [YafModule("Mail Queue Starting Module", "Tiny Gecko", 1)]
  public class MailSendingModule : IBaseModule
  {
    /// <summary>
    /// The _key name.
    /// </summary>
    private const string _keyName = "MailSendTask";

    /// <summary>
    /// The _forum control obj.
    /// </summary>
    private object _forumControlObj;

    #region IBaseModule Members

    /// <summary>
    /// Gets or sets ForumControlObj.
    /// </summary>
    public object ForumControlObj
    {
      get
      {
        return this._forumControlObj;
      }

      set
      {
        this._forumControlObj = value;
      }
    }

    /// <summary>
    /// The init.
    /// </summary>
    public void Init()
    {
      // hook the page init for mail sending...
      YafContext.Current.AfterInit += new EventHandler<EventArgs>(Current_AfterInit);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    /// <summary>
    /// The current_ after init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_AfterInit(object sender, EventArgs e)
    {
      // add the mailing task if it's not already added...
      if (YafTaskModule.Current != null && !YafTaskModule.Current.TaskExists(_keyName))
      {
        // start it...
        YafTaskModule.Current.StartTask(_keyName, new MailSendTask());
      }
    }
  }
}

namespace YAF.Classes.Core
{
  /// <summary>
  /// Sends Email in the background.
  /// </summary>
  public class MailSendTask : IntermittentBackgroundTask
  {
    /// <summary>
    /// The _send mail threaded.
    /// </summary>
    protected YafSendMailThreaded _sendMailThreaded = new YafSendMailThreaded();

    /// <summary>
    /// The _unique id.
    /// </summary>
    protected int _uniqueId = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailSendTask"/> class.
    /// </summary>
    public MailSendTask()
    {
      // set the unique value...
      var rand = new Random();
      this._uniqueId = rand.Next();

      // set interval values...
      RunPeriodMs = (rand.Next(10) + 5)*1000;
      StartDelayMs = (rand.Next(10) + 5)*1000;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      Debug.WriteLine("Running Send Mail Thread...");

      // send thread handles it's own exception...
      this._sendMailThreaded.SendThreaded(this._uniqueId);
    }
  }
}