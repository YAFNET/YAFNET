/*
 * This code is an extender of JSMin project
 * http://www.crockford.com/javascript/jsmin.html 
 * Please give credit to Douglas Crockford
 * 
 * Haiyan Du, May 22nd, 2009
 * */

namespace YAF.Utils.Helpers.MinifyUtils
{
    #region Using

    using System;
    using System.IO;
    using System.Text;

    #endregion

    /// <summary>
    /// JS Minify Class
    /// </summary>
    public class JSMinify
    {
        #region Constants and Fields

        /// <summary>
        /// The eof.
        /// </summary>
        private const int EOF = -1;

        /// <summary>
        /// The text reader.
        /// </summary>
        private TextReader textReader;

        /// <summary>
        /// The the a.
        /// </summary>
        private int theA;

        /// <summary>
        /// The the b.
        /// </summary>
        private int theB;

        /// <summary>
        /// The the lookahead.
        /// </summary>
        private int theLookahead = EOF;

        /// <summary>
        /// The writer.
        /// </summary>
        private TextWriter writer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Prevents a default instance of the <see cref="JSMinify" /> class from being created.
        /// </summary>
        private JSMinify()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Minifies the specified input code.
        /// </summary>
        /// <param name="inputCode">
        /// The input code. 
        /// </param>
        /// <returns>
        /// The minify. 
        /// </returns>
        public static string Minify(string inputCode)
        {
            var minify = new JSMinify();
            var outputCode = new StringBuilder();

            using (minify.textReader = new StringReader(inputCode))
            {
                using (minify.writer = new StringWriter(outputCode))
                {
                    minify.Pack();
                }
            }

            return outputCode.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// action -- do something! What you do is determined by the argument: 1 Output A. Copy B to A. Get the next B. 2 Copy B to A. Get the next B. (Delete A). 3 Get the next B. (Delete B). action treats a string as a single character. Wow! action recognizes a regular expression if it is preceded by ( or , or =.
        /// </summary>
        /// <param name="d">
        /// The d. 
        /// </param>
        /// <exception cref="Exception">
        /// Error: JSMIN unterminated string literal:
        /// </exception>
        private void Action(int d)
        {
            if (d <= 1)
            {
                this.Put(this.theA);
            }

            if (d <= 2)
            {
                this.theA = this.theB;
                if (this.theA == '\'' || this.theA == '"')
                {
                    for (;;)
                    {
                        this.Put(this.theA);
                        this.theA = this.Get();

                        if (this.theA == this.theB)
                        {
                            break;
                        }

                        if (this.theA <= '\n')
                        {
                            throw new Exception($"Error: JSMIN unterminated string literal: {this.theA}\n");
                        }

                        if (this.theA != '\\')
                        {
                            continue;
                        }

                        this.Put(this.theA);
                        this.theA = this.Get();
                    }
                }
            }

            if (d > 3)
            {
                return;
            }

            this.theB = this.Next();
            if (this.theB != '/' || this.theA != '(' && this.theA != ',' && this.theA != '=' && this.theA != '[' &&
                this.theA != '!' && this.theA != ':' && this.theA != '&' && this.theA != '|' &&
                this.theA != '?' && this.theA != '{' && this.theA != '}' && this.theA != ';' &&
                this.theA != '\n')
            {
                return;
            }

            this.Put(this.theA);
            this.Put(this.theB);

            for (;;)
            {
                this.theA = this.Get();

                if (this.theA == '/')
                {
                    break;
                }

                if (this.theA == '\\')
                {
                    this.Put(this.theA);
                    this.theA = this.Get();
                }
                else if (this.theA <= '\n')
                {
                    throw new Exception(
                        $"Error: JSMIN unterminated Regular Expression literal : {this.theA}.\n");
                }

                this.Put(this.theA);
            }

            this.theB = this.Next();
        }

        /// <summary>
        /// Watch out for lookahead. If the character is a control character, translate it to a space or linefeed.
        /// </summary>
        /// <returns>
        /// return the next character from stdin. 
        /// </returns>
        private int Get()
        {
            var c = this.theLookahead;
            this.theLookahead = EOF;
            if (c == EOF)
            {
                c = this.textReader.Read();
            }

            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }

            return c == '\r' ? '\n' : ' ';
        }

        /// <summary>
        /// Check if Alphanum
        /// </summary>
        /// <param name="c">
        /// The c. 
        /// </param>
        /// <returns>
        /// return true if the character is a letter, digit, underscore, dollar sign, or non-ASCII character. 
        /// </returns>
        private static bool IsAlphanum(int c)
        {
            return c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c == '_' || c == '$'
                   || c == '\\' || c > 126;
        }

        /// <summary>
        /// next -- get the next character, excluding comments. peek() is used to see if a '/' is followed by a '/' or '*'.
        /// </summary>
        /// <returns>
        /// The next. 
        /// </returns>
        /// <exception cref="Exception">
        /// Error: JSMIN Unterminated comment.
        /// </exception>
        private int Next()
        {
            var c = this.Get();
            if (c != '/')
            {
                return c;
            }

            switch (this.Peek())
            {
                case '/':
                {
                    for (;;)
                    {
                        c = this.Get();
                        if (c <= '\n')
                        {
                            return c;
                        }
                    }
                }

                case '*':
                {
                    this.Get();
                    for (;;)
                    {
                        switch (this.Get())
                        {
                            case '*':
                            {
                                if (this.Peek() == '/')
                                {
                                    this.Get();
                                    return ' ';
                                }

                                break;
                            }

                            case EOF:
                            {
                                throw new Exception("Error: JSMIN Unterminated comment.\n");
                            }
                        }
                    }
                }

                default:
                {
                    return c;
                }
            }
        }

        /// <summary>
        /// jsmin -- Copy the input to the output, deleting the characters which are insignificant to JavaScript. Comments will be removed. Tabs will be replaced with spaces. Carriage returns will be replaced with linefeeds. Most spaces and linefeeds will be removed.
        /// </summary>
        private void Pack()
        {
            this.theA = '\n';
            this.Action(3);

            while (this.theA != EOF)
            {
                switch (this.theA)
                {
                    case ' ':
                        {
                            this.Action(IsAlphanum(this.theB) ? 1 : 2);
                            break;
                        }

                    case '\n':
                        {
                            switch (this.theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        this.Action(1);
                                        break;
                                    }

                                case ' ':
                                    {
                                        this.Action(3);
                                        break;
                                    }

                                default:
                                    {
                                        this.Action(IsAlphanum(this.theB) ? 1 : 2);

                                        break;
                                    }
                            }

                            break;
                        }

                    default:
                        {
                            switch (this.theB)
                            {
                                case ' ':
                                    {
                                        if (IsAlphanum(this.theA))
                                        {
                                            this.Action(1);
                                            break;
                                        }

                                        this.Action(3);
                                        break;
                                    }

                                case '\n':
                                    {
                                        switch (this.theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    this.Action(1);
                                                    break;
                                                }

                                            default:
                                                {
                                                    this.Action(IsAlphanum(this.theA) ? 1 : 3);
                                                    break;
                                                }
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        this.Action(1);
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Get the next character without getting it.
        /// </summary>
        /// <returns>
        /// The peek. 
        /// </returns>
        private int Peek()
        {
            this.theLookahead = this.Get();
            return this.theLookahead;
        }

        /// <summary>
        /// Puts the specified c.
        /// </summary>
        /// <param name="c">
        /// The c. 
        /// </param>
        private void Put(int c)
        {
            this.writer.Write((char)c);
        }

        #endregion
    }
}