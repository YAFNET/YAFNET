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
namespace YAF.Utils
{
  #region Using

  using System;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Text;

  using YAF.Types;
  using YAF.Types.Extensions;

    #endregion

  /// <summary>
  /// The hash helper.
  /// </summary>
  public static class HashHelper
  {
    #region Enums

    /// <summary>
    /// The hash algorithm type.
    /// </summary>
    public enum HashAlgorithmType
    {
      /// <summary>
      /// The m d 5.
      /// </summary>
      MD5, 

      /// <summary>
      /// The sh a 1.
      /// </summary>
      SHA1, 

      /// <summary>
      /// The sh a 256.
      /// </summary>
      SHA256, 

      /// <summary>
      /// The sh a 384.
      /// </summary>
      SHA384, 

      /// <summary>
      /// The sh a 512.
      /// </summary>
      SHA512
    }

    /// <summary>
    /// The hash case type.
    /// </summary>
    public enum HashCaseType
    {
      /// <summary>
      /// The upper.
      /// </summary>
      Upper, 

      /// <summary>
      /// The lower.
      /// </summary>
      Lower
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a password buffer from salt and password ready for hashing/encrypting
    /// </summary>
    /// <param name="salt">
    /// Salt to be applied to hashing algorithm
    /// </param>
    /// <param name="clearString">
    /// Clear string to hash
    /// </param>
    /// <param name="standardComp">
    /// Use Standard asp.net membership method of creating the buffer
    /// </param>
    /// <returns>
    /// Salted Password as Byte Array 
    /// </returns>
    [NotNull]
    public static byte[] GeneratePasswordBuffer([NotNull] string salt, [NotNull] string clearString, bool standardComp)
    {
      byte[] unencodedBytes = Encoding.Unicode.GetBytes(clearString);
      byte[] saltBytes = Convert.FromBase64String(salt);
      var buffer = new byte[unencodedBytes.Length + saltBytes.Length];

      if (standardComp)
      {
        // Compliant with ASP.NET Membership method of hash/salt
        Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
        Buffer.BlockCopy(unencodedBytes, 0, buffer, saltBytes.Length, unencodedBytes.Length);
      }
      else
      {
        Buffer.BlockCopy(unencodedBytes, 0, buffer, 0, unencodedBytes.Length);
        Buffer.BlockCopy(saltBytes, 0, buffer, unencodedBytes.Length - 1, saltBytes.Length);
      }

      return buffer;
    }

    /// <summary>
    /// Hashes a clear string to the given hashtype
    /// </summary>
    /// <param name="clearString">
    /// Clear string to hash
    /// </param>
    /// <param name="hashAlgorithmType">
    /// hash Algorithm to be used
    /// </param>
    /// <param name="salt">
    /// Salt to be applied to hashing algorithm
    /// </param>
    /// <param name="useSalt">
    /// Should salt be applied to hashing algorithm
    /// </param>
    /// <param name="hashHex">
    /// The hash Hex.
    /// </param>
    /// <param name="hashCaseType">
    /// The hash Case.
    /// </param>
    /// <param name="hashRemoveChars">
    /// The hash Remove Chars.
    /// </param>
    /// <param name="standardComp">
    /// The standard Comp.
    /// </param>
    /// <returns>
    /// Hashed String as Hex or Base64 
    /// </returns>
    public static string Hash(
      [NotNull] string clearString, 
      HashAlgorithmType hashAlgorithmType = HashAlgorithmType.SHA1, 
      [CanBeNull] string salt = null, 
      bool hashHex = true, 
      HashCaseType hashCaseType = HashCaseType.Upper, 
      [CanBeNull] string hashRemoveChars = null, 
      bool standardComp = true)
    {
      CodeContracts.VerifyNotNull(clearString, "clearString");

      byte[] buffer;

      if (salt.IsSet())
      {
        buffer = GeneratePasswordBuffer(salt, clearString, standardComp);
      }
      else
      {
        byte[] unencodedBytes = Encoding.UTF8.GetBytes(clearString); // UTF8 used to maintain compatibility
        buffer = new byte[unencodedBytes.Length];
        Buffer.BlockCopy(unencodedBytes, 0, buffer, 0, unencodedBytes.Length);
      }

      byte[] hashedBytes = Hash(buffer, hashAlgorithmType); // Hash

      string hashedString = hashHex ? hashedBytes.ToHexString() : Convert.ToBase64String(hashedBytes);

      // Adjust the case of the hash output
      switch (hashCaseType)
      {
        case HashCaseType.Upper:
          hashedString = hashedString.ToUpper();
          break;
        case HashCaseType.Lower:
          hashedString = hashedString.ToLower();
          break;
      }

      if (hashRemoveChars.IsSet())
      {
        hashedString = hashRemoveChars.Aggregate(
          hashedString, (current, removeChar) => current.Replace(removeChar.ToString(), string.Empty));
      }

      return hashedString;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Hashes clear bytes to given hashtype
    /// </summary>
    /// <param name="clearBytes">
    /// Clear bytes to hash
    /// </param>
    /// <param name="hashAlgorithmType">
    /// hash Algorithm to be used
    /// </param>
    /// <returns>
    /// Hashed bytes 
    /// </returns>
    [NotNull]
    private static byte[] Hash([NotNull] byte[] clearBytes, [NotNull] HashAlgorithmType hashAlgorithmType)
    {
      CodeContracts.VerifyNotNull(clearBytes, "clearBytes");

      return HashAlgorithm.Create(hashAlgorithmType.GetStringValue()).ComputeHash(clearBytes);
    }

    #endregion
  }
}