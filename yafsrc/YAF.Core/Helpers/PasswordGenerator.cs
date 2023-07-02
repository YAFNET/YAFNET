// Written by Paul Seal. Licensed under MIT. Free for private and commercial uses.
// https://codeshare.co.uk/blog/how-to-create-a-random-password-generator-in-c/#final-code

namespace YAF.Core.Helpers;

using System.Security.Cryptography;
using System.Text.RegularExpressions;
using YAF.Core.Services;

/// <summary>
/// The password generator.
/// </summary>
public static class PasswordGenerator
{
    /// <summary>
    /// Generates a random password based on the rules passed in the parameters
    /// </summary>
    /// <param name="includeLowercase">
    /// Boolean to say if lowercase are required
    /// </param>
    /// <param name="includeUppercase">
    /// Boolean to say if uppercase are required
    /// </param>
    /// <param name="includeNumeric">
    /// Boolean to say if numeric are required
    /// </param>
    /// <param name="includeSpecial">
    /// Boolean to say if special characters are required
    /// </param>
    /// <param name="includeSpaces">
    /// Boolean to say if spaces are required
    /// </param>
    /// <param name="lengthOfPassword">
    /// Length of password required. Should be between 8 and 128
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string GeneratePassword(
        bool includeLowercase,
        bool includeUppercase,
        bool includeNumeric,
        bool includeSpecial,
        bool includeSpaces,
        int lengthOfPassword)
    {
        const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
        const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
        const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMERIC_CHARACTERS = "0123456789";
        const string SPECIAL_CHARACTERS = @"!#$%&*@\";
        const string SPACE_CHARACTER = " ";
        const int PASSWORD_LENGTH_MIN = 8;
        const int PASSWORD_LENGTH_MAX = 128;

        if (lengthOfPassword is < PASSWORD_LENGTH_MIN or > PASSWORD_LENGTH_MAX)
        {
            return "Password length must be between 8 and 128.";
        }

        var characterSet = string.Empty;

        if (includeLowercase)
        {
            characterSet += LOWERCASE_CHARACTERS;
        }

        if (includeUppercase)
        {
            characterSet += UPPERCASE_CHARACTERS;
        }

        if (includeNumeric)
        {
            characterSet += NUMERIC_CHARACTERS;
        }

        if (includeSpecial)
        {
            characterSet += SPECIAL_CHARACTERS;
        }

        if (includeSpaces)
        {
            characterSet += SPACE_CHARACTER;
        }

        var password = new char[lengthOfPassword];
        var characterSetLength = characterSet.Length;

        for (var characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
        {
            password[characterPosition] = characterSet[RandomNumberGenerator.GetInt32(1, characterSetLength - 1)];

            var moreThanTwoIdenticalInARow = characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS &&
                                             password[characterPosition] == password[characterPosition - 1] &&
                                             password[characterPosition - 1] == password[characterPosition - 2];

            if (moreThanTwoIdenticalInARow)
            {
                characterPosition--;
            }
        }

        var newPassword = string.Join(null, password);

        while (!PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, newPassword))
        {
            newPassword = GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
        }

        return newPassword;
    }

    /// <summary>
    /// Checks if the password created is valid
    /// </summary>
    /// <param name="includeLowercase">Boolean to say if lowercase are required</param>
    /// <param name="includeUppercase">Boolean to say if uppercase are required</param>
    /// <param name="includeNumeric">Boolean to say if numeric are required</param>
    /// <param name="includeSpecial">Boolean to say if special characters are required</param>
    /// <param name="includeSpaces">Boolean to say if spaces are required</param>
    /// <param name="password">Generated password</param>
    /// <returns>True or False to say if the password is valid or not</returns>
    public static bool PasswordIsValid(
        bool includeLowercase,
        bool includeUppercase,
        bool includeNumeric,
        bool includeSpecial,
        bool includeSpaces,
        string password)
    {
        const string REGEX_LOWERCASE = @"[a-z]";
        const string REGEX_UPPERCASE = @"[A-Z]";
        const string REGEX_NUMERIC = @"[\d]";
        const string REGEX_SPECIAL = @"([!#$%&*@\\])+";
        const string REGEX_SPACE = @"([ ])+";

        var lowerCaseIsValid = !includeLowercase || Regex.IsMatch(password, REGEX_LOWERCASE);
        var upperCaseIsValid = !includeUppercase || Regex.IsMatch(password, REGEX_UPPERCASE);
        var numericIsValid = !includeNumeric || Regex.IsMatch(password, REGEX_NUMERIC);
        var symbolsAreValid = !includeSpecial || Regex.IsMatch(password, REGEX_SPECIAL);
        var spacesAreValid = !includeSpaces || Regex.IsMatch(password, REGEX_SPACE);

        return lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid && spacesAreValid;
    }
}