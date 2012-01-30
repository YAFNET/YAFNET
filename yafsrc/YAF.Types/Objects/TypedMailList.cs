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
  using System.Data;

  #endregion

  /// <summary>
  /// The typed mail list.
  /// </summary>
  [Serializable]
  public class TypedMailList
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedMailList"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedMailList([NotNull] DataRow row)
    {
      this.MailID = row.Field<int?>("MailID");
      this.FromUser = row.Field<string>("FromUser");
      this.ToUser = row.Field<string>("ToUser");
      this.Created = row.Field<DateTime?>("Created");
      this.Subject = row.Field<string>("Subject");
      this.Body = row.Field<string>("Body");
      this.FromUserName = row.Field<string>("FromUserName");
      this.ToUserName = row.Field<string>("ToUserName");
      this.BodyHtml = row.Field<string>("BodyHtml");
      this.SendTries = row.Field<int?>("SendTries");
      this.SendAttempt = row.Field<DateTime?>("SendAttempt");
      this.ProcessID = row.Field<int?>("ProcessID");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedMailList"/> class.
    /// </summary>
    /// <param name="mailid">
    /// The mailid.
    /// </param>
    /// <param name="fromuser">
    /// The fromuser.
    /// </param>
    /// <param name="touser">
    /// The touser.
    /// </param>
    /// <param name="created">
    /// The created.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <param name="fromusername">
    /// The fromusername.
    /// </param>
    /// <param name="tousername">
    /// The tousername.
    /// </param>
    /// <param name="bodyhtml">
    /// The bodyhtml.
    /// </param>
    /// <param name="sendtries">
    /// The sendtries.
    /// </param>
    /// <param name="sendattempt">
    /// The sendattempt.
    /// </param>
    /// <param name="processid">
    /// The processid.
    /// </param>
    public TypedMailList(
      int? mailid, [NotNull] string fromuser, [NotNull] string touser,
      DateTime? created, [NotNull] string subject, [NotNull] string body, [NotNull] string fromusername, [NotNull] string tousername, [NotNull] string bodyhtml,
      int? sendtries,
      DateTime? sendattempt,
      int? processid)
    {
      this.MailID = mailid;
      this.FromUser = fromuser;
      this.ToUser = touser;
      this.Created = created;
      this.Subject = subject;
      this.Body = body;
      this.FromUserName = fromusername;
      this.ToUserName = tousername;
      this.BodyHtml = bodyhtml;
      this.SendTries = sendtries;
      this.SendAttempt = sendattempt;
      this.ProcessID = processid;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets BodyHtml.
    /// </summary>
    public string BodyHtml { get; set; }

    /// <summary>
    /// Gets or sets Created.
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets FromUser.
    /// </summary>
    public string FromUser { get; set; }

    /// <summary>
    /// Gets or sets FromUserName.
    /// </summary>
    public string FromUserName { get; set; }

    /// <summary>
    /// Gets or sets MailID.
    /// </summary>
    public int? MailID { get; set; }

    /// <summary>
    /// Gets or sets ProcessID.
    /// </summary>
    public int? ProcessID { get; set; }

    /// <summary>
    /// Gets or sets SendAttempt.
    /// </summary>
    public DateTime? SendAttempt { get; set; }

    /// <summary>
    /// Gets or sets SendTries.
    /// </summary>
    public int? SendTries { get; set; }

    /// <summary>
    /// Gets or sets Subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets ToUser.
    /// </summary>
    public string ToUser { get; set; }

    /// <summary>
    /// Gets or sets ToUserName.
    /// </summary>
    public string ToUserName { get; set; }

    #endregion
  }
}