/* YetAnotherForum.NET
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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace YAF.TranslateApp
{
    public static class Translator
    {
        /// <summary>
        /// Translates the text.
        /// </summary>
        /// <param name="sInput">The input.</param>
        /// <param name="sLanguagePair">The language pair.</param>
        /// <returns></returns>
        public static string TranslateText(string sInput, string sLanguagePair)
        {
            return TranslateText(sInput, sLanguagePair, Encoding.UTF7);
        }

        /// <summary>
        /// Translate Text using Google Translate
        /// </summary>
        /// <param name="sInput">The string you want translated</param>
        /// <param name="sLanguagePair">2 letter Language Pair, delimited by "|". 
        /// e.g. "en|da" language pair means to translate from English to Danish</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Translated to String</returns>
        public static string TranslateText(string sInput, string sLanguagePair, Encoding encoding)
        {
            string sUrl = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", sInput, sLanguagePair);

            string sResult;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = encoding;
                    sResult = wc.DownloadString(sUrl);
                }

                Match m = Regex.Match(sResult, "(?<=<span id=result_box class=\"short_text\"><span[^>]+>)(.*?)(?=</span>)");

                sResult = m.Success ? m.Value : sInput;
            }
            catch (Exception)
            {
                sResult = sInput;
            }

            return sResult;
        }
    }
}