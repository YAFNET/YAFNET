//****************************************************
//
//		Source Control Author : Justin Wendlandt
//			jwendl@hotmail.com
//
//		Control Protected by the Creative Commons License
//		http://creativecommons.org/licenses/by-nc-sa/2.0/
//
//****************************************************
namespace yaf
{
	using System;
	using System.IO;
	using System.Web;
	using System.Text.RegularExpressions;
	using System.Collections;

	public class HighLighter
	{
		// To Replace Enter with <br />
		private bool replaceEnter;
		public bool ReplaceEnter
		{
			get { return replaceEnter; }
			set { replaceEnter = value; }
		}

		// Default Constructor
		public HighLighter()
		{
			replaceEnter = false;
		}

		public string colorText(string tmpCode, string pathToDefFile, string language)
		{
			language = language.ToLower();
			if(language=="c#" || language=="csharp")
				language = "cs";

			language = language.Replace("\"", "");
			// language = language.Replace("&#8220;", "");

			string tmpOutput = "";
			string comments = "";
			bool valid = true;

			ArrayList alKeyWords = new ArrayList();
			ArrayList alKeyTypes = new ArrayList();

			// Read def file.
			try
			{
				StreamReader sr = new StreamReader(pathToDefFile + language.ToString() + ".def");
				string tmpLine = "";
				string curFlag = "";
				while (sr.Peek() != -1)
				{
					tmpLine = sr.ReadLine();
					if (tmpLine != "")
					{
						if (tmpLine.Substring(0, 1) == "-")
						{
							// Ignore these lines and set the Current Flag
							if (tmpLine.ToLower().IndexOf("keywords") > 0)
							{
								curFlag = "keywords";
							}

							if (tmpLine.ToLower().IndexOf("keytypes") > 0)
							{
								curFlag = "keytypes";
							}

							if (tmpLine.ToLower().IndexOf("comments") > 0)
							{
								curFlag = "comments";
							}
						}
						else
						{
							if (curFlag == "keywords")
							{
								alKeyWords.Add(tmpLine);
							}
							
							if (curFlag == "keytypes")
							{
								alKeyTypes.Add(tmpLine);
							}

							if (curFlag == "comments")
							{
								comments = tmpLine;
							}
						}
					}
				}
				sr.Close();
			}
			catch (Exception ex)
			{
				string foobar = ex.ToString();
				tmpOutput = "<span class=\"errors\">There was an error opening file " + pathToDefFile + language.ToString() + ".def...</span>";
				valid = false;
				throw new ApplicationException(string.Format("There was an error opening file {0}{1}.def",pathToDefFile,language),ex);
			}

			if (valid == true)
			{
				// Replace Comments
				int lineNum = 0;
				ArrayList thisComment = new ArrayList();
				MatchCollection mColl = Regex.Matches(tmpCode, comments, RegexOptions.Multiline|RegexOptions.IgnoreCase);
				foreach (Match m in mColl)
				{
					thisComment.Add(m.ToString());
					tmpCode = tmpCode.Replace(m.ToString(), "[ReplaceComment" + lineNum++ + "]");
				}

				// Replace Strings
				lineNum = 0;
				ArrayList thisString = new ArrayList();
				string thisMatch = "\"((\\\\\")|[^\"(\\\\\")]|)+\"";
				mColl = Regex.Matches(tmpCode, thisMatch, RegexOptions.Singleline|RegexOptions.IgnoreCase);
				foreach (Match m in mColl)
				{
					thisString.Add(m.ToString());
					tmpCode = tmpCode.Replace(m.ToString(), "[ReplaceString" + lineNum++ + "]");
				}

				// Replace Chars
				lineNum = 0;
				ArrayList thisChar = new ArrayList();
				mColl = Regex.Matches(tmpCode, "\'.*?\'", RegexOptions.Singleline|RegexOptions.IgnoreCase);
				foreach (Match m in mColl)
				{
					thisChar.Add(m.ToString());
					tmpCode = tmpCode.Replace(m.ToString(), "[ReplaceChar" + lineNum++ + "]");
				}

				// Replace KeyWords
				string[] KeyWords = new String[alKeyWords.Count];
				alKeyWords.CopyTo(KeyWords);
				string tmpKeyWords = "(?<replacethis>" + String.Join("|", KeyWords) + ")";
				tmpCode = Regex.Replace(tmpCode, "\\b" + tmpKeyWords + "\\b(?<!//.*)", "<span class=\"keyword\">${replacethis}</span>");

				// Replace KeyTypes
				string[] KeyTypes = new String[alKeyTypes.Count];
				alKeyTypes.CopyTo(KeyTypes);
				string tmpKeyTypes = "(?<replacethis>" + String.Join("|", KeyTypes) + ")";
				tmpCode = Regex.Replace(tmpCode, "\\b" + tmpKeyTypes + "\\b(?<!//.*)", "<span class=\"keytype\">${replacethis}</span>");

				lineNum = 0;
				foreach (string m in thisChar)
				{
					tmpCode = tmpCode.Replace("[ReplaceChar" + lineNum++ + "]", "<span class=\"string\">" + m.ToString() + "</span>");
				}

				lineNum = 0;
				foreach (string m in thisString)
				{
					tmpCode = tmpCode.Replace("[ReplaceString" + lineNum++ + "]", "<span class=\"string\">" + m.ToString() + "</span>");
				}

				lineNum = 0;
				foreach (string m in thisComment)
				{
					tmpCode = tmpCode.Replace("[ReplaceComment" + lineNum++ + "]", "<span class=\"comment\">" + m.ToString() + "</span>");
				}

				// Replace Numerics
				tmpCode = Regex.Replace(tmpCode, "(\\d{1,12}\\.\\d{1,12}|\\d{1,12})", "<span class=\"integer\">$1</span>");

				if (replaceEnter == true)
				{
					tmpCode = Regex.Replace(tmpCode, "\r", "");
					tmpCode = Regex.Replace(tmpCode, "\n", "<br />" + Environment.NewLine);
				}

				tmpCode = Regex.Replace(tmpCode, "  ", "&nbsp;&nbsp;");
				tmpCode = Regex.Replace(tmpCode, "\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");

				// Create Output
				tmpOutput = "<div class=\"codestuff\">" + Environment.NewLine;
				tmpOutput += "<style type=\"text/css\">" + Environment.NewLine;
				tmpOutput += "<!--" + Environment.NewLine;
				//tmpOutput += ".codestuff { background-color : #eeeeee; border:solid 1px #000000; padding:5px; font-family: Courier New; font-size: 12px}" + Environment.NewLine;
				tmpOutput += ".codestuff .keytype { color : #FF9933; font-weight : normal; }" + Environment.NewLine;
				tmpOutput += ".codestuff .keyword { color : #224FFF; font-weight : normal; }" + Environment.NewLine;
				tmpOutput += ".codestuff .integer { color : #FF0032; }" + Environment.NewLine;
				tmpOutput += ".codestuff .comment { color : #008100; }" + Environment.NewLine;
				tmpOutput += ".codestuff .errors { color : #FF0000; font-weight : bold; }" + Environment.NewLine;
				tmpOutput += ".codestuff .string { color : #FF0022; }" + Environment.NewLine;
				tmpOutput += "//-->" + Environment.NewLine;
				tmpOutput += "</style>" + Environment.NewLine;
				tmpOutput += tmpCode;
				tmpOutput += "</div>" + Environment.NewLine;
			}
			return tmpOutput;
		}
	}
}

