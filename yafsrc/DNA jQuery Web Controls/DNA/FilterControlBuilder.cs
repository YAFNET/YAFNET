//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI
{
  #region Using

  using System;
  using System.Collections;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// Subclass this class to create a ControlBuilder that
  ///   only allows specific child types/tags and strips out any
  ///   literal strings as well.
  /// </summary>
  public abstract class FilterControlBuilder : ControlBuilder
  {
    #region Constants and Fields

    /// <summary>
    /// The _ tag type table.
    /// </summary>
    private readonly Hashtable _TagTypeTable; // Tag to Type relationship table

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterControlBuilder"/> class. 
    ///   Initializes a new instance of a FilterControlBuilder.
    /// </summary>
    public FilterControlBuilder()
    {
      // Create the table
      this._TagTypeTable = new Hashtable();

      // Fill the table with tag to type relationships
      this.FillTagTypeTable();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a tagname to type entry.
    /// </summary>
    /// <param name="tagName">
    /// The tag name.
    /// </param>
    /// <param name="type">
    /// The type.
    /// </param>
    public void Add(string tagName, Type type)
    {
      this._TagTypeTable.Add(tagName.ToLower(), type);
    }

    /// <summary>
    /// Allows subclasses to override the rejection of literal strings.
    /// </summary>
    /// <returns>
    /// false to reject literals.
    /// </returns>
    public virtual bool AllowLiterals()
    {
      // Ignore all literals
      return false;
    }

    /// <summary>
    /// Rejects whitespace.
    /// </summary>
    /// <returns>
    /// false to reject whitespace.
    /// </returns>
    public override bool AllowWhitespaceLiterals()
    {
      // Ignore whitespace literals
      return false;
    }

    /// <summary>
    /// Rejects appending literal strings.
    /// </summary>
    /// <param name="s">
    /// The string.
    /// </param>
    public override void AppendLiteralString(string s)
    {
      if (this.AllowLiterals())
      {
        base.AppendLiteralString(s);
      }
      else
      {
        s = s.Trim();
        if (s != String.Empty)
        {
          throw new Exception("InvalidLiteralString:" + s);
        }
      }
    }

    /// <summary>
    /// Determines a type given a tag name.
    /// </summary>
    /// <param name="tagName">
    /// The tagname.
    /// </param>
    /// <param name="attribs">
    /// Attributes.
    /// </param>
    /// <returns>
    /// The type of the tag.
    /// </returns>
    public override Type GetChildControlType(string tagName, IDictionary attribs)
    {
      // Let the base class have the tagname
      Type baseType = base.GetChildControlType(tagName, attribs);
      if (baseType != null)
      {
        // If the type returned is valid, then return it
        if (this._TagTypeTable.ContainsValue(baseType))
        {
          return baseType;
        }
      }

      // Allows children without runat=server to be added
      // and to limit to specific types
      string szTagName = tagName.ToLower();
      int colon = szTagName.IndexOf(':');
      if ((colon >= 0) && (colon < (szTagName.Length + 1)))
      {
        // Separate the tagname from the namespace
        szTagName = szTagName.Substring(colon + 1, szTagName.Length - colon - 1);
      }

      // Find Type associated with tagname
      object obj = this._TagTypeTable[szTagName];

      // Return the Type if found
      if ((obj != null) && (obj is Type))
      {
        return (Type)obj;
      }

      // No Type was found, throw an exception
      throw new Exception("InvalidChildTagName:" + tagName);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Implement this method and use Add to add
    ///   key value pairs that translate a tag name to
    ///   a type.
    /// </summary>
    /// <example>
    /// Add("tagname", typeof(TagType));
    /// </example>
    protected abstract void FillTagTypeTable();

    #endregion
  }
}