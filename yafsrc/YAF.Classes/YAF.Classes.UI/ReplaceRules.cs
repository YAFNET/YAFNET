/* Yet Another Forum.net
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Classes.UI
{
	/// <summary>
	/// Provides a way to handle layers of replacements rules
	/// </summary>
	public class ReplaceRules
	{
		private List<BaseReplaceRule> _rulesList;
		private HtmlReplacementCollection _mainCollection;

		public ReplaceRules()
		{
			_rulesList = new List<BaseReplaceRule>();
			_mainCollection = new HtmlReplacementCollection();
		}

		public List<BaseReplaceRule> RulesList
		{
			get { return _rulesList; }
		}

		public void AddRule( BaseReplaceRule newRule )
		{
			_rulesList.Add( newRule );
		}

		public void Process( ref string text )
		{
			// sort the rules according to rank...
			_rulesList.Sort(); 
			// apply all rules...
			foreach ( BaseReplaceRule rule in _rulesList )
			{
				rule.Replace( ref text, ref this._mainCollection );
			}
			// reconstruct the html
			_mainCollection.Reconstruct( ref text );
		}
	}

	/// <summary>
	/// Base class for all replacement rules.
	/// Provides compare functionality based on the rule rank.
	/// Override replace to handle replacement differently.
	/// </summary>
	public class BaseReplaceRule : IComparable
	{
		public int RuleRank = 0;

		public virtual void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			throw new NotImplementedException( "Not Implemented in Base Class" );
		}

		#region IComparable Members

		public int CompareTo( object obj )
		{
			if ( obj is BaseReplaceRule )
			{
				BaseReplaceRule otherRule = obj as BaseReplaceRule;

				if ( this.RuleRank > otherRule.RuleRank ) return 1;
				else if ( this.RuleRank < otherRule.RuleRank ) return -1;
				return 0;
			}
			else
			{
				throw new ArgumentException( "Object is not of type BaseReplaceRule." );
			}
		}

		#endregion
	}

	/// <summary>
	/// Not regular expression, just a simple replace
	/// </summary>
	public class SimpleReplaceRule : BaseReplaceRule
	{
		private string _find;
		private string _replace;

		public SimpleReplaceRule( string find, string replace )
		{
			_find = find;
			_replace = replace;
			// lower the rank by default
			this.RuleRank = 100;
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			int index = -1;

			do
			{
				index = text.IndexOf( _find );
				if ( index >= 0 )
				{
					// replace it...
					int replaceIndex = replacement.AddReplacement( new HtmlReplacementBlock( _replace ) );
					text = text.Substring( 0, index ) + replacement.GetReplaceValue( replaceIndex ) + text.Substring( index + _find.Length );
				}
			} while ( index >= 0 );			
		}
	}

	/// <summary>
	/// For basic regex with no variables
	/// </summary>
	public class SimpleRegexReplaceRule : BaseReplaceRule
	{
		protected Regex _regExSearch;
		protected string _regExReplace;

		public SimpleRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions )
		{
			_regExSearch = new Regex( regExSearch, regExOptions );
			_regExReplace = regExReplace;
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				string tStr = _regExReplace.Replace( "${inner}", m.Groups ["inner"].Value );

				// pulls the htmls into the replacement collection before it's inserted back into the main text
				replacement.GetReplacementsFromText( ref tStr );
				
				text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = _regExSearch.Match( text );
			}
		}
	}

	/// <summary>
	/// For complex regex with variable/default and truncate support
	/// </summary>
	public class VariableRegexReplaceRule : SimpleRegexReplaceRule
	{
		protected string [] _variables = null;
		protected string [] _variableDefaults = null;
		protected int _truncateLength = 0;

		public VariableRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions, string [] variables, string [] varDefaults, int truncateLength )
			: base( regExSearch, regExReplace, regExOptions )
		{
			_variables = variables;
			_variableDefaults = varDefaults;
			_truncateLength = truncateLength;
		}

		public VariableRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions, string [] variables, string [] varDefaults )
			: this( regExSearch, regExReplace, regExOptions, variables, varDefaults, 0 )
		{

		}

		public VariableRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions, string [] variables )
			: this( regExSearch, regExReplace, regExOptions, variables, null, 0 )
		{
			
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				string tStr = _regExReplace;
				int i = 0;

				foreach ( string tVar in _variables )
				{
					string tValue = m.Groups [tVar].Value;

					if ( _variableDefaults != null && tValue.Length == 0 )
					{
						// use default instead
						tValue = _variableDefaults [i];
					}

					tStr = tStr.Replace( "${" + tVar + "}", tValue );
					i++;
				}

				tStr = tStr.Replace( "${inner}", m.Groups ["inner"].Value );

				if ( _truncateLength > 0 )
				{
					// special handling to truncate urls
					tStr = tStr.Replace( "${innertrunc}", General.TruncateMiddle( m.Groups ["inner"].Value, _truncateLength ) );
				}

				// pulls the htmls into the replacement collection before it's inserted back into the main text
				replacement.GetReplacementsFromText( ref tStr );

				// add it back into the text
				text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = _regExSearch.Match( text );
			}
		}
	}

	/// <summary>
	/// Handles the collection of replacement tags and can also pull the HTML out of the text making a new replacement tag
	/// </summary>
	public class HtmlReplacementCollection
	{
		private Dictionary<int, HtmlReplacementBlock> _replacementDictionary;
		private int _currentIndex = 0;
		private RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
		private Regex _rgxHtml;
		private string _replaceFormat = "[ @CODE({1})INDEX({0})CODE({1})INDEX@ ]";
		private int _randomInstance;

		public HtmlReplacementCollection()
		{
			_replacementDictionary = new Dictionary<int, HtmlReplacementBlock>();
			_rgxHtml = new Regex( @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", _options );
			// get a random number for the instance
			// so it's harder to guess the replacement format since it changes
			// each instance of the class
			Random rand = new Random();
			_randomInstance = rand.Next();
		}

		public Dictionary<int, HtmlReplacementBlock> ReplacementDictionary
		{
			get
			{
				return _replacementDictionary;
			}
		}

		public int AddReplacement( HtmlReplacementBlock newItem )
		{
			_replacementDictionary.Add( _currentIndex, newItem );
			return _currentIndex++;
		}

		public string GetReplaceValue( int index )
		{
			return String.Format( _replaceFormat, index, _randomInstance );
		}

		/// <summary>
		/// Reconstructs the text from the collection elements...
		/// </summary>
		/// <param name="text"></param>
		public void Reconstruct( ref string text )
		{
			foreach ( int index in _replacementDictionary.Keys )
			{
				text = text.Replace( GetReplaceValue( index ), _replacementDictionary [index].Tag );
			}
		}

		/// <summary>
		/// Pull replacement blocks from the text
		/// </summary>
		/// <param name="html"></param>
		public void GetReplacementsFromText( ref string text )
		{
			Match m = _rgxHtml.Match( text );
			while ( m.Success )
			{				
				// add it to the list...
				int index = this.AddReplacement( new HtmlReplacementBlock( m.Groups [0].Value ) );

				// replacement lookup code
				string replace = GetReplaceValue( index );

				text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = _rgxHtml.Match( text );
			}			
		}
	}

	/// <summary>
	/// Simple class that doesn't do anything except store a tag.
	/// Why a class? Because I may want to add to it someday...
	/// </summary>
	public class HtmlReplacementBlock
	{
		private string _tag;

		public HtmlReplacementBlock( string tag )
		{
			_tag = tag;
		}

		public string Tag
		{
			get { return _tag; }
		}
	}
}
