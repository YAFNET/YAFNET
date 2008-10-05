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
using System.Text;
using System.IO;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Classes.UI
{
	/// <summary>
	/// Provides a way to handle layers of replacements rules
	/// </summary>
	public class ReplaceRules : ICloneable
	{
		private List<BaseReplaceRule> _rulesList;
		private bool _needSort = false;

		public ReplaceRules()
		{
			_rulesList = new List<BaseReplaceRule>();
		}

		public List<BaseReplaceRule> RulesList
		{
			get { return _rulesList; }
		}

		public void AddRule( BaseReplaceRule newRule )
		{
			_rulesList.Add( newRule );
			_needSort = true;
		}

		public void Process( ref string text )
		{
			// sort the rules according to rank...
			if ( _needSort ) { _rulesList.Sort(); _needSort = false; }

			// make the replacementCollection for this instance...
			HtmlReplacementCollection mainCollection = new HtmlReplacementCollection();

			// apply all rules...
			foreach ( BaseReplaceRule rule in _rulesList )
			{
				rule.Replace( ref text, ref mainCollection );
			}

			// reconstruct the html
			mainCollection.Reconstruct( ref text );
		}

		#region ICloneable Members

		/// <summary>
		/// This clone method is a Deep Clone -- including all data.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ReplaceRules copyReplaceRules = new ReplaceRules();
			// move the rules over...
			BaseReplaceRule [] ruleArray = new BaseReplaceRule [this._rulesList.Count];
			this._rulesList.CopyTo( ruleArray );
			copyReplaceRules._rulesList.InsertRange( 0, ruleArray );
			copyReplaceRules._needSort = this._needSort;

			return copyReplaceRules;
		}

		#endregion
	}

	/// <summary>
	/// Base class for all replacement rules.
	/// Provides compare functionality based on the rule rank.
	/// Override replace to handle replacement differently.
	/// </summary>
	public class BaseReplaceRule : IComparable
	{
		public int RuleRank = 50;

		public virtual void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			throw new NotImplementedException( "Not Implemented in Base Class" );
		}

		public virtual string RuleDescription
		{
			get { return string.Empty; }
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

		public override string RuleDescription
		{
			get { return String.Format( "Find = \"{0}\"", _find ); }
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

		public SimpleRegexReplaceRule( Regex regExSearch, string regExReplace )
		{
			_regExSearch = regExSearch;
			_regExReplace = regExReplace;
		}

		protected virtual string GetInnerValue( string innerValue )
		{
			return innerValue;
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			StringBuilder sb = new StringBuilder( text );

			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				string tStr = _regExReplace.Replace( "${inner}", GetInnerValue( m.Groups ["inner"].Value ) );

				// pulls the htmls into the replacement collection before it's inserted back into the main text
				replacement.GetReplacementsFromText( ref tStr );

				// remove old bbcode...
				sb.Remove( m.Groups [0].Index, m.Groups [0].Length );

				// insert replaced value(s)
				sb.Insert( m.Groups [0].Index, tStr );

				//text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );

				m = _regExSearch.Match( sb.ToString() );
			}

			text = sb.ToString();
		}

		public override string RuleDescription
		{
			get { return String.Format( "RegExSearch = \"{0}\"", _regExSearch ); }
		}
	}

	/// <summary>
	/// For basic regex with no variables
	/// </summary>
	public class SingleRegexReplaceRule : SimpleRegexReplaceRule
	{
		public SingleRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions )
			: base( regExSearch, regExReplace, regExOptions )
		{
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			StringBuilder sb = new StringBuilder( text );

			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				// just replaces with no "inner"
				int replaceIndex = replacement.AddReplacement( new HtmlReplacementBlock( _regExReplace ) );

				// remove old bbcode...
				sb.Remove( m.Groups [0].Index, m.Groups [0].Length );

				// insert replaced value(s)
				sb.Insert( m.Groups [0].Index, replacement.GetReplaceValue( replaceIndex ) );

				//text = text.Substring( 0, m.Groups [0].Index ) + replacement.GetReplaceValue( replaceIndex ) + text.Substring( m.Groups [0].Index + m.Groups [0].Length );

				m = _regExSearch.Match( sb.ToString() );
			}

			text = sb.ToString();
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

		public VariableRegexReplaceRule( Regex regExSearch, string regExReplace, string [] variables, string [] varDefaults, int truncateLength )
			: base( regExSearch, regExReplace )
		{
			_variables = variables;
			_variableDefaults = varDefaults;
			_truncateLength = truncateLength;
		}

		public VariableRegexReplaceRule( Regex regExSearch, string regExReplace, string [] variables, string [] varDefaults )
			: this( regExSearch, regExReplace, variables, varDefaults, 0 )
		{

		}

		public VariableRegexReplaceRule( Regex regExSearch, string regExReplace, string [] variables )
			: this( regExSearch, regExReplace, variables, null, 0 )
		{

		}

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

		/// <summary>
		/// Override to change default variable handling...
		/// </summary>
		/// <param name="variableName"></param>
		/// <param name="variableValue"></param>
		/// <returns></returns>
		protected virtual string ManageVariableValue( string variableName, string variableValue )
		{
			return variableValue;
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			StringBuilder sb = new StringBuilder( text );

			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				StringBuilder innerReplace = new StringBuilder( _regExReplace );
				int i = 0;

				foreach ( string tVar in _variables )
				{
					string tValue = m.Groups [tVar].Value;

					if ( _variableDefaults != null && tValue.Length == 0 )
					{
						// use default instead
						tValue = _variableDefaults [i];
					}

					innerReplace.Replace( "${" + tVar + "}", ManageVariableValue( tVar, tValue ) );
					i++;
				}

				innerReplace.Replace( "${inner}", m.Groups ["inner"].Value );

				if ( _truncateLength > 0 )
				{
					// special handling to truncate urls
					innerReplace.Replace( "${innertrunc}", General.TruncateMiddle( m.Groups ["inner"].Value, _truncateLength ) );
				}

				// pulls the htmls into the replacement collection before it's inserted back into the main text
				replacement.GetReplacementsFromText( ref innerReplace );

				// remove old bbcode...
				sb.Remove( m.Groups [0].Index, m.Groups [0].Length );

				// insert replaced value(s)
				sb.Insert( m.Groups [0].Index, innerReplace.ToString() );

				//text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );

				m = _regExSearch.Match( sb.ToString() );
			}

			text = sb.ToString();
		}
	}

	/// <summary>
	/// For the font size with replace
	/// </summary>
	public class FontSizeRegexReplaceRule : VariableRegexReplaceRule
	{
		public FontSizeRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions )
			: base( regExSearch, regExReplace, regExOptions, new string [] { "size" }, new string [] { "5" } )
		{
			RuleRank = 25;
		}
		private string GetFontSize( string inputStr )
		{
			int [] sizes = { 50, 70, 80, 90, 100, 120, 140, 160, 180 };
			int size = 5;

			// try to parse the input string...
			int.TryParse( inputStr, out size );

			if ( size < 1 ) size = 1;
			if ( size > sizes.Length ) size = 5;

			return sizes [size - 1].ToString() + "%";
		}

		protected override string ManageVariableValue( string variableName, string variableValue )
		{
			if ( variableName == "size" ) return GetFontSize( variableValue );
			return variableValue;
		}
	}

	/// <summary>
	/// For the font size with replace
	/// </summary>
	public class PostTopicRegexReplaceRule : VariableRegexReplaceRule
	{
		public PostTopicRegexReplaceRule( string regExSearch, string regExReplace, RegexOptions regExOptions )
			: base( regExSearch, regExReplace, regExOptions, new string [] { "post", "topic" } )
		{
			RuleRank = 200;
		}
		protected override string ManageVariableValue( string variableName, string variableValue )
		{
			if ( variableName == "post" || variableName == "topic" )
			{
				int id = 0;
				if ( int.TryParse( variableValue, out id ) )
				{
					if ( variableName == "post" )
						return YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#post{0}", id );
					else if ( variableName == "topic" )
						return YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", id );
				}
			}
			return variableValue;
		}
	}

	/// <summary>
	/// Simple code block regular express replace
	/// </summary>
	public class CodeRegexReplaceRule : SimpleRegexReplaceRule
	{
		public CodeRegexReplaceRule( Regex regExSearch, string regExReplace )
			: base( regExSearch, regExReplace )
		{
			// default high rank...
			RuleRank = 2;
		}

		/// <summary>
		/// This just overrides how the inner value is handled
		/// </summary>
		/// <param name="innerValue"></param>
		/// <returns></returns>
		protected override string GetInnerValue( string innerValue )
		{
			innerValue = innerValue.Replace( "  ", "&nbsp; " );
			innerValue = innerValue.Replace( "  ", " &nbsp;" );
			innerValue = innerValue.Replace( "\t", "&nbsp; &nbsp;&nbsp;" );
			innerValue = innerValue.Replace( "[", "&#91;" );
			innerValue = innerValue.Replace( "]", "&#93;" );
			innerValue = innerValue.Replace( "<", "&lt;" );
			innerValue = innerValue.Replace( ">", "&gt;" );
			innerValue = innerValue.Replace( "\r\n", "<br/>" );
			return innerValue;
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				string tStr = _regExReplace.Replace( "${inner}", GetInnerValue( m.Groups ["inner"].Value ) );

				int replaceIndex = replacement.AddReplacement( new HtmlReplacementBlock( tStr ) );
				text = text.Substring( 0, m.Groups [0].Index ) + replacement.GetReplaceValue( replaceIndex ) + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = _regExSearch.Match( text );
			}
		}
	}

	/// <summary>
	/// Syntax Highlighted code block regular express replace
	/// </summary>
	public class SyntaxHighlightedCodeRegexReplaceRule : SimpleRegexReplaceRule
	{
		private HighLighter _syntaxHighlighter = new HighLighter();

		public SyntaxHighlightedCodeRegexReplaceRule( Regex regExSearch, string regExReplace )
			: base( regExSearch, regExReplace )
		{
			_syntaxHighlighter.ReplaceEnter = true;
			RuleRank = 1;
		}

		/// <summary>
		/// This just overrides how the inner value is handled
		/// </summary>
		/// <param name="innerValue"></param>
		/// <returns></returns>
		protected override string GetInnerValue( string innerValue )
		{
			return innerValue;
		}

		public override void Replace( ref string text, ref HtmlReplacementCollection replacement )
		{
			Match m = _regExSearch.Match( text );
			while ( m.Success )
			{
				string inner = _syntaxHighlighter.ColorText( GetInnerValue( m.Groups ["inner"].Value ), HttpContext.Current.Server.MapPath( YafForumInfo.ForumFileRoot + "defs/" ), m.Groups ["language"].Value );
				string tStr = _regExReplace.Replace( "${inner}", inner );

				// pulls the htmls into the replacement collection before it's inserted back into the main text
				int replaceIndex = replacement.AddReplacement( new HtmlReplacementBlock( tStr ) );

				text = text.Substring( 0, m.Groups [0].Index ) + replacement.GetReplaceValue( replaceIndex ) + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
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
		private string _replaceFormat = "÷ñÒ{1}êÖ{0}õæ÷";
		private int _randomInstance;

		public HtmlReplacementCollection()
		{
			_replacementDictionary = new Dictionary<int, HtmlReplacementBlock>();
			_rgxHtml = new Regex( @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", _options );

			RandomizeInstance();
		}

		/// <summary>
		/// get a random number for the instance
		/// so it's harder to guess the replacement format
		/// </summary>
		public void RandomizeInstance()
		{
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
			StringBuilder sb = new StringBuilder( text );

			foreach ( int index in _replacementDictionary.Keys )
			{
				sb.Replace( GetReplaceValue( index ), _replacementDictionary [index].Tag );
			}

			text = sb.ToString();
		}

		/// <summary>
		/// Pull replacement blocks from the text
		/// </summary>
		/// <param name="html"></param>
		public void GetReplacementsFromText( ref string strText )
		{
			StringBuilder sb = new StringBuilder( strText );

			Match m = _rgxHtml.Match( strText );
			while ( m.Success )
			{
				// add it to the list...
				int index = this.AddReplacement( new HtmlReplacementBlock( m.Groups [0].Value ) );

				// replacement lookup code
				string replace = GetReplaceValue( index );

				// remove the replaced item...
				sb.Remove( m.Groups [0].Index, m.Groups [0].Length );

				// insert the replaced value back in...
				sb.Insert( m.Groups [0].Index, replace );

				//text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );

				m = _rgxHtml.Match( sb.ToString() );
			}

			strText = sb.ToString();
		}

		public void GetReplacementsFromText( ref StringBuilder sb )
		{
			Match m = _rgxHtml.Match( sb.ToString() );
			while ( m.Success )
			{
				// add it to the list...
				int index = this.AddReplacement( new HtmlReplacementBlock( m.Groups [0].Value ) );

				// replacement lookup code
				string replace = GetReplaceValue( index );

				// remove the replaced item...
				sb.Remove( m.Groups [0].Index, m.Groups [0].Length );

				// insert the replaced value back in...
				sb.Insert( m.Groups [0].Index, replace );

				//text = text.Substring( 0, m.Groups [0].Index ) + replace + text.Substring( m.Groups [0].Index + m.Groups [0].Length );

				m = _rgxHtml.Match( sb.ToString() );
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
