/* Farsi Library - Working with Dates, Calendars, and DatePickers
 * http://www.codeproject.com/KB/selection/FarsiLibrary.aspx
 * 
 * Copyright (C) Hadi Eskandari
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace FarsiLibrary.Internals
{
    using System;

    /// <summary>
    /// Helper class for guard statements, which allow prettier
    /// code for guard clauses
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Will throw a <see cref="InvalidOperationException"/> if the assertion
        /// is true, with the specificied message.
        /// </summary>
        /// <param name="assertion">if set to <c>true</c> [assertion].</param>
        /// <param name="message">The message.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// Guard.Against(string.IsNullOrEmpty(name), "Name must have a value");
        /// </code>
        /// </example>
        public static void Against(bool assertion, string message)
        {
            if (assertion == false)
                return;
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Will throw exception of type <typeparamref name="TException"/>
        /// with the specified message if the assertion is true
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="assertion">if set to <c>true</c> [assertion].</param>
        /// <param name="message">The message.</param>
        /// <example>
        /// Sample usage:
        /// <code>
        /// <![CDATA[
        /// Guard.Against<ArgumentException>(string.IsNullOrEmpty(name), "Name must have a value");
        /// ]]>
        /// </code>
        /// </example>
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion == false)
                return;
            throw (TException)Activator.CreateInstance(typeof(TException), message);
        }
    }
}
