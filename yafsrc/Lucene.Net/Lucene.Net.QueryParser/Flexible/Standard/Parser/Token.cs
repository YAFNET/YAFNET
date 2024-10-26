﻿using System;

namespace YAF.Lucene.Net.QueryParsers.Flexible.Standard.Parser
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// Describes the input token stream.
    /// </summary>
    // LUCENENET: It is no longer good practice to use binary serialization. 
    // See: https://github.com/dotnet/corefx/issues/23584#issuecomment-325724568
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Token
    {
        /// <summary>
        /// An integer that describes the kind of this token.  This numbering
        /// system is determined by JavaCCParser, and a table of these numbers is
        /// stored in the file ...Constants.java.
        /// </summary>
        public int Kind { get; set; }

        /// <summary>The line number of the first character of this Token.</summary>
        public int BeginLine { get; set; }
        /// <summary>The column number of the first character of this Token.</summary>
        public int BeginColumn { get; set; }
        /// <summary>The line number of the last character of this Token.</summary>
        public int EndLine { get; set; }
        /// <summary>The column number of the last character of this Token.</summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// The string image of the token.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// A reference to the next regular (non-special) token from the input
        /// stream.  If this is the last token from the input stream, or if the
        /// token manager has not read tokens beyond this one, this field is
        /// set to null.  This is true only if this token is also a regular
        /// token.  Otherwise, see below for a description of the contents of
        /// this field.
        /// </summary>
        public Token Next { get; set; }

        /// <summary>
        /// This field is used to access special tokens that occur prior to this
        /// token, but after the immediately preceding regular (non-special) token.
        /// If there are no such special tokens, this field is set to null.
        /// When there are more than one such special token, this field refers
        /// to the last of these special tokens, which in turn refers to the next
        /// previous special token through its specialToken field, and so on
        /// until the first special token (whose specialToken field is null).
        /// The next fields of special tokens refer to other special tokens that
        /// immediately follow it (without an intervening regular token).  If there
        /// is no such token, this field is null.
        /// </summary>
        public Token SpecialToken { get; set; }

        /// <summary>
        /// An optional attribute value of the Token.
        /// Tokens which are not used as syntactic sugar will often contain
        /// meaningful values that will be used later on by the compiler or
        /// interpreter. This attribute value is often different from the image.
        /// Any subclass of Token that actually wants to return a non-null value can
        /// override this method as appropriate.
        /// </summary>
        public virtual object Value => null;

        /// <summary>
        /// No-argument constructor
        /// </summary>
        public Token() { }

        /// <summary>
        /// Constructs a new token for the specified Image.
        /// </summary>
        public Token(int kind)
            : this(kind, null)
        {
        }

        /// <summary>
        /// Constructs a new token for the specified Image and Kind.
        /// </summary>
        public Token(int kind, string image)
        {
            this.Kind = kind;
            this.Image = image;
        }

        /// <summary>
        /// Returns the image.
        /// </summary>
        public override string ToString()
        {
            return Image;
        }

        /// <summary>
        /// Returns a new <see cref="Token"/> object, by default. However, if you want, you
        /// can create and return subclass objects based on the value of <paramref name="ofKind"/>.
        /// Simply add the cases to the switch for all those special cases.
        /// For example, if you have a subclass of <see cref="Token"/> called IDToken that
        /// you want to create if <paramref name="ofKind"/> is ID, simply add something like :
        /// <code>
        ///     case MyParserConstants.ID : return new IDToken(ofKind, image);
        /// </code>
        /// to the following switch statement. Then you can cast matchedToken
        /// variable to the appropriate type and use sit in your lexical actions.
        /// </summary>
        public static Token NewToken(int ofKind, string image)
        {
            switch (ofKind)
            {
                default: return new Token(ofKind, image);
            }
        }

        public static Token NewToken(int ofKind)
        {
            return NewToken(ofKind, null);
        }
    }
}
