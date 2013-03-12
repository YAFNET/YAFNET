/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  using YAF.Types;

  /// <summary>
  /// The recaptcha response.
  /// </summary>
  public class RecaptchaResponse
  {
    #region Constants and Fields

    /// <summary>
    ///   The invalid solution.
    /// </summary>
    public static readonly RecaptchaResponse InvalidSolution = new RecaptchaResponse(false, "incorrect-captcha-sol");

    /// <summary>
    ///   The recaptcha not reachable.
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
    /// The equals.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The equals.
    /// </returns>
    public override bool Equals([NotNull] object obj)
    {
      var response = (RecaptchaResponse)obj;
      if (response == null)
      {
        return false;
      }

      return (response.IsValid == this.IsValid) && (response.ErrorCode == this.ErrorCode);
    }

    /// <summary>
    /// The get hash code.
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