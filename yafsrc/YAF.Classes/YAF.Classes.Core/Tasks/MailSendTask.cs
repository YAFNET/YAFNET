/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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