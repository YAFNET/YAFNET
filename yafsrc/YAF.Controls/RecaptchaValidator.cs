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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.IO;
  using System.Net;
  using System.Net.Sockets;
  using System.Text;
  using System.Web;

  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The recaptcha validator.
  /// </summary>
  public class RecaptchaValidator
  {
		[Inject]
  	public ILogger Logger { get; set; }

    #region Constants and Fields

    /// <summary>
    ///   The verify url.
    /// </summary>
    private const string VerifyUrl = "http://api-verify.recaptcha.net/verify";

    /// <summary>
    ///   The challenge.
    /// </summary>
    private string challenge;

    /// <summary>
    ///   The remote ip.
    /// </summary>
    private string remoteIp;

    /// <summary>
    ///   The response.
    /// </summary>
    private string response;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets Challenge.
    /// </summary>
    public string Challenge
    {
      get
      {
        return this.challenge;
      }

      set
      {
        this.challenge = value;
      }
    }

    /// <summary>
    ///   Gets or sets PrivateKey.
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    ///   Gets or sets RemoteIP.
    /// </summary>
    /// <exception cref = "ArgumentException">
    /// </exception>
    public string RemoteIP
    {
      get
      {
        return this.remoteIp;
      }

      set
      {
        IPAddress address = IPAddress.Parse(value);
        if ((address == null) ||
            ((address.AddressFamily != AddressFamily.InterNetwork) &&
             (address.AddressFamily != AddressFamily.InterNetworkV6)))
        {
          throw new ArgumentException("Expecting an IP address, got " + address);
        }

        this.remoteIp = address.ToString();
      }
    }

    /// <summary>
    ///   Gets or sets Response.
    /// </summary>
    public string Response
    {
      get
      {
        return this.response;
      }

      set
      {
        this.response = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The validate.
    /// </summary>
    /// <returns>
    /// </returns>
    /// <exception cref="InvalidProgramException">
    /// </exception>
    public RecaptchaResponse Validate()
    {
      string[] strArray;
      this.CheckNotNull(this.PrivateKey, "PrivateKey");
      this.CheckNotNull(this.RemoteIP, "RemoteIp");
      this.CheckNotNull(this.Challenge, "Challenge");
      this.CheckNotNull(this.Response, "Response");
      if ((this.challenge == string.Empty) || (this.response == string.Empty))
      {
        return RecaptchaResponse.InvalidSolution;
      }

      var request = (HttpWebRequest)WebRequest.Create("http://api-verify.recaptcha.net/verify");
      request.ProtocolVersion = HttpVersion.Version10;
      request.Timeout = 0x7530;
      request.Method = "POST";
      request.UserAgent = "reCAPTCHA/ASP.NET";
      request.ContentType = "application/x-www-form-urlencoded";
      string s =
        "privatekey={0}&remoteip={1}&challenge={2}&response={3}".FormatWith(
          new object[]
            {
              HttpUtility.UrlEncode(this.PrivateKey), HttpUtility.UrlEncode(this.RemoteIP), 
              HttpUtility.UrlEncode(this.Challenge), HttpUtility.UrlEncode(this.Response)
            });
      byte[] bytes = Encoding.ASCII.GetBytes(s);
      using (Stream stream = request.GetRequestStream())
      {
        stream.Write(bytes, 0, bytes.Length);
      }

      try
      {
        using (WebResponse response = request.GetResponse())
        {
          using (TextReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
          {
            strArray = reader.ReadToEnd().Split(new char[0]);
          }
        }
      }
      catch (WebException exception)
      {
				this.Logger.Error(exception, "Error in Recaptcha Response for UserID: {0}".FormatWith(YafContext.Current.PageUserID));

        return RecaptchaResponse.RecaptchaNotReachable;
      }

      switch (strArray[0])
      {
        case "true":
          return RecaptchaResponse.Valid;

        case "false":
          return new RecaptchaResponse(false, strArray[1]);
      }

      throw new InvalidProgramException("Unknown status response.");
    }

    #endregion

    #region Methods

    /// <summary>
    /// The check not null.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    private void CheckNotNull([NotNull] object obj, [NotNull] string name)
    {
      if (obj == null)
      {
        throw new ArgumentNullException(name);
      }
    }

    #endregion
  }
}