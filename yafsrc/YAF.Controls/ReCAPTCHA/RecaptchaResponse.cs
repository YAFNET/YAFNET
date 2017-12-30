/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls.ReCAPTCHA
{
    using YAF.Types;

    /// <summary>
  /// The reCAPTCHA response.
  /// </summary>
  public class RecaptchaResponse
  {
    #region Constants and Fields

    /// <summary>
    ///   The invalid solution.
    /// </summary>
    public static readonly RecaptchaResponse InvalidSolution = new RecaptchaResponse(false, "incorrect-captcha-sol");

    /// <summary>
    ///   The reCAPTCHA not reachable.
    /// </summary>
    public static readonly RecaptchaResponse RecaptchaNotReachable = new RecaptchaResponse(
      false, "recaptcha-not-reachable");

    /// <summary>
    ///   The valid.
    /// </summary>
    public static readonly RecaptchaResponse Valid = new RecaptchaResponse(true, string.Empty);

    /// <summary>
    ///   The error code.
    /// </summary>
    private readonly string errorCode;

    /// <summary>
    ///   The is valid.
    /// </summary>
    private readonly bool isValid;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RecaptchaResponse"/> class.
    /// </summary>
    /// <param name="isValid">
    /// The is valid.
    /// </param>
    /// <param name="errorCode">
    /// The error code.
    /// </param>
    internal RecaptchaResponse(bool isValid, [NotNull] string errorCode)
    {
      this.isValid = isValid;
      this.errorCode = errorCode;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets ErrorCode.
    /// </summary>
    public string ErrorCode
    {
      get
      {
        return this.errorCode;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsValid.
    /// </summary>
    public bool IsValid
    {
      get
      {
        return this.isValid;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="recaptchaResponse">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    /// The equals.
    /// </returns>
    public override bool Equals([NotNull] object recaptchaResponse)
    {
      var response = (RecaptchaResponse)recaptchaResponse;
      if (response == null)
      {
        return false;
      }

      return (response.IsValid == this.IsValid) && (response.ErrorCode == this.ErrorCode);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// The get hash code.
    /// </returns>
    public override int GetHashCode()
    {
      return this.IsValid.GetHashCode() ^ this.ErrorCode.GetHashCode();
    }

    #endregion
  }
}