/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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