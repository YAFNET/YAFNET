/*
COPYRIGHT

Character transliteration tables:

Copyright 2001, Sean M. Burke <sburke@cpan.org>, all rights reserved.

Python code:

Copyright 2009, Tomaz Solc <tomaz@zemanta.com>

CSharp code:

Copyright 2010, Oleg Usanov <oleg@usanov.net>

The programs and documentation in this dist are distributed in the
hope that they will be useful, but without any warranty; without even
the implied warranty of merchantability or fitness for a particular
purpose.

This library is free software; you can redistribute it and/or modify
it under the same terms as Perl.
*/

namespace YAF.Utils.Helpers.StringUtils
{
    #region Using

    using System.Text;

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// ASCII transliterations of Unicode text
    /// </summary>
    public static partial class Unidecoder
    {
        #region Public Methods

        /// <summary>
        /// Transliterate an UniCode object into an ASCII string
        /// </summary>
        /// <remarks>
        /// Unidecode(u"\u5317\u4EB0") == "Bei Jing "
        /// </remarks>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// Returns the translated string
        /// </returns>
        public static string Unidecode(this string input)
        {
            if (input.IsNotSet())
            {
                return input;
            }

            var output = new StringBuilder();
            foreach (var c in input.ToCharArray())
            {
                if (c < 0x80)
                {
                    output.Append(c);
                    continue;
                }

                var h = c >> 8;
                var l = c & 0xff;

                output.Append(characters.ContainsKey(h) ? characters[h][l] : string.Empty);
            }

            return output.ToString();
        }

        #endregion
    }
}