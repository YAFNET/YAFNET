//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  /// <summary>
  /// The growth of the Accordion
  /// </summary>
  public enum AccordionSizeModes
  {
    /// <summary>
    ///   The Accordion grows/shrinks without restriction. 
    ///   This can cause other elements on your page to move up and down with it.
    /// </summary>
    None, 

    /// <summary>
    ///   The Accordion always stays the exact same size as its Height property. 
    ///   This will cause the content to be expanded or shrunk if it isn't the right size.
    /// </summary>
    Fill, 

    /// <summary>
    ///   The Accordion never grows larger than the value specified by its Height property. 
    ///   This will cause the content to scroll if it is too large to be displayed.
    /// </summary>
    Limit
  }
}