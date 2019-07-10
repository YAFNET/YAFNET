﻿// 
//   MarkdownDeep - http://www.toptensoftware.com/markdowndeep
//	 Copyright (C) 2010-2011 Topten Software
// 
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this product except in 
//   compliance with the License. You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License is 
//   distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//   See the License for the specific language governing permissions and limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownDeep
{
	/*
	 * StringScanner is a simple class to help scan through an input string.
	 * 
	 * Maintains a current position with various operations to inspect the current
	 * character, skip forward, check for matches, skip whitespace etc...
	 */
	public class StringScanner
	{
		// Constructor
		public StringScanner()
		{
		}

		// Constructor
		public StringScanner(string str)
		{
			Reset(str);
		}

		// Constructor
		public StringScanner(string str, int pos)
		{
			Reset(str, pos);
		}

		// Constructor
		public StringScanner(string str, int pos, int len)
		{
			Reset(str, pos, len);
		}

		// Reset
		public void Reset(string str)
		{
			Reset(str, 0, str?.Length ?? 0);
		}

		// Reset
		public void Reset(string str, int pos)
		{
			Reset(str, pos, str?.Length - pos ?? 0);
		}

		// Reset
		public void Reset(string str, int pos, int len)
		{
			if (str == null)
				str = "";
			if (len < 0)
				len = 0;
			if (pos < 0)
				pos = 0;
			if (pos > str.Length)
				pos = str.Length;

			this.input = str;
			this.start = pos;
			this.pos = pos;
			this.end = pos + len;

			if (end > str.Length)
				end = str.Length;
		}

		// Get the entire input string
		public string input { get; private set; }

        // Get the character at the current position
		public char current
		{
			get
			{
				if (pos < start || pos >= end)
					return '\0';
				else
					return this.input[pos];
			}
		}

		// Get/set the current position
		public int position
		{
			get => pos;
            set => pos = value;
        }

		// Get the remainder of the input 
		// (use this in a watch window while debugging :)
		public string remainder => Substring(position);

        // Skip to the end of file
		public void SkipToEof()
		{
			pos = end;
		}


		// Skip to the end of the current line
		public void SkipToEol()
		{
			while (pos < end)
			{
				var ch=this.input[pos];
				if (ch=='\r' || ch=='\n')
					break;
				pos++;
			}
		}

		// Skip if currently at a line end
		public bool SkipEol()
		{
			if (pos < end)
			{
				var ch = this.input[pos];
				if (ch == '\r')
				{
					pos++;
					if (pos < end && this.input[pos] == '\n')
						pos++;
					return true;
				}

				else if (ch == '\n')
				{
					pos++;
					if (pos < end && this.input[pos] == '\r')
						pos++;
					return true;
				}
			}

			return false;
		}

		// Skip to the next line
		public void SkipToNextLine()
		{
			SkipToEol();
			SkipEol();
		}

		// Get the character at offset from current position
		// Or, \0 if out of range
		public char CharAtOffset(int offset)
		{
			var index = pos + offset;
			
			if (index < start)
				return '\0';
			if (index >= end)
				return '\0';
			return this.input[index];
		}

		// Skip a number of characters
		public void SkipForward(int characters)
		{
			pos += characters;
		}

		// Skip a character if present
		public bool SkipChar(char ch)
		{
			if (current == ch)
			{
				SkipForward(1);
				return true;
			}

			return false;	
		}

		// Skip a matching string
		public bool SkipString(string str)
		{
			if (DoesMatch(str))
			{
				SkipForward(str.Length);
				return true;
			}

			return false;
		}

		// Skip a matching string
		public bool SkipStringI(string str)
		{
			if (DoesMatchI(str))
			{
				SkipForward(str.Length);
				return true;
			}

			return false;
		}

		// Skip any whitespace
		public bool SkipWhitespace()
		{
			if (!char.IsWhiteSpace(current))
				return false;
			SkipForward(1);

			while (char.IsWhiteSpace(current))
				SkipForward(1);

			return true;
		}

		// Check if a character is space or tab
		public static bool IsLineSpace(char ch)
		{
			return ch == ' ' || ch == '\t';
		}

		// Skip spaces and tabs
		public bool SkipLinespace()
		{
			if (!IsLineSpace(current))
				return false;
			SkipForward(1);

			while (IsLineSpace(current))
				SkipForward(1);

			return true;
		}

		// Does current character match something
		public bool DoesMatch(char ch)
		{
			return current == ch;
		}

		// Does character at offset match a character
		public bool DoesMatch(int offset, char ch)
		{
			return CharAtOffset(offset) == ch;
		}

		// Does current character match any of a range of characters
		public bool DoesMatchAny(char[] chars)
		{
			for (var i = 0; i < chars.Length; i++)
			{
				if (DoesMatch(chars[i]))
					return true;
			}
			return false;
		}

		// Does current character match any of a range of characters
		public bool DoesMatchAny(int offset, char[] chars)
		{
			for (var i = 0; i < chars.Length; i++)
			{
				if (DoesMatch(offset, chars[i]))
					return true;
			}
			return false;
		}

		// Does current string position match a string
		public bool DoesMatch(string str)
		{
			for (var i = 0; i < str.Length; i++)
			{
				if (str[i] != CharAtOffset(i))
					return false;
			}
			return true;
		}

		// Does current string position match a string
		public bool DoesMatchI(string str)
		{
			return string.Compare(str, Substring(position, str.Length), true) == 0;
		}

		// Extract a substring
		public string Substring(int start)
		{
			return this.input.Substring(start, end-start);
		}

		// Extract a substring
		public string Substring(int start, int len)
		{
			if (start + len > end)
				len = end - start;

			return this.input.Substring(start, len);
		}

		// Scan forward for a character
		public bool Find(char ch)
		{
			if (pos >= end)
				return false;

			// Find it
			var index = this.input.IndexOf(ch, pos);
			if (index < 0 || index>=end)
				return false;

			// Store new position
			pos = index;
			return true;
		}

		// Find any of a range of characters
		public bool FindAny(char[] chars)
		{
			if (pos >= end)
				return false;

			// Find it
			var index = this.input.IndexOfAny(chars, pos);
			if (index < 0 || index>=end)
				return false;

			// Store new position
			pos = index;
			return true;
		}

		// Forward scan for a string
		public bool Find(string find)
		{
			if (pos >= end)
				return false;

			var index = this.input.IndexOf(find, pos);
			if (index < 0 || index > end-find.Length)
				return false;

			pos = index;
			return true;
		}

		// Forward scan for a string (case insensitive)
		public bool FindI(string find)
		{
			if (pos >= end)
				return false;

			var index = this.input.IndexOf(find, pos, StringComparison.OrdinalIgnoreCase);
			if (index < 0 || index >= end - find.Length)
				return false;

			pos = index;
			return true;
		}

		// Are we at eof?
		public bool eof => pos >= end;

        // Are we at eol?
		public bool eol => IsLineEnd(current);

        // Are we at bof?
		public bool bof => pos == start;

        // Mark current position
		public void Mark()
		{
			mark = pos;
		}

		// Extract string from mark to current position
		public string Extract()
		{
			if (mark >= pos)
				return "";

			return this.input.Substring(mark, pos - mark);
		}

		// Skip an identifier
		public bool SkipIdentifier(ref string identifier)
		{
			var savepos = position;
			if (!Utils.ParseIdentifier(this.input, ref pos, ref identifier))
				return false;
			if (pos >= end)
			{
				pos = savepos;
				return false;
			}
			return true;
		}

		public bool SkipFootnoteID(out string id)
		{
			var savepos = position;

			SkipLinespace();

			Mark();

			while (true)
			{
				var ch = current;
				if (char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' || ch == ':' || ch == '.' || ch == ' ')
					SkipForward(1);
				else
					break;
			}

			if (position > mark)
			{
				id = Extract().Trim();
				if (!String.IsNullOrEmpty(id))
				{
					SkipLinespace();
					return true;
				}
			}

			position = savepos;
			id = null;
			return false;
		}

		// Skip a Html entity (eg: &amp;)
		public bool SkipHtmlEntity(ref string entity)
		{
			var savepos = position;
			if (!Utils.SkipHtmlEntity(this.input, ref pos, ref entity))
				return false;
			if (pos > end)
			{
				pos = savepos;
				return false;
			}
			return true;
		}

		// Check if a character marks end of line
		public static bool IsLineEnd(char ch)
		{
			return ch == '\r' || ch == '\n' || ch=='\0';
		}

		bool IsUrlChar(char ch)
		{
			switch (ch)
			{
				case '+':
				case '&':
				case '@':
				case '#':
				case '/':
				case '%':
				case '?':
				case '=':
				case '~':
				case '_':
				case '|':
				case '[':
				case ']':
				case '(':
				case ')':
				case '!':
				case ':':
				case ',':
				case '.':
				case ';':
					return true;

				default:
					return Char.IsLetterOrDigit(ch);
			}
		}

		// Attributes
        int start;
		int pos;
		int end;
		int mark;
	}
}
