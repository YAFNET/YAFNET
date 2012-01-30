/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The display date time.
  /// </summary>
  public class DisplayDateTime : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _controlHtml.
    /// </summary>
    protected string _controlHtml = @"<abbr class=""timeago"" title=""{0}"">{1}</abbr>";

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets DateTime.
    /// </summary>
    public object DateTime
    {
      get
      {
        if (this.ViewState["DateTime"] == null)
        {
          return null;
        }

        return this.ViewState["DateTime"];
      }

      set
      {
        this.ViewState["DateTime"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets Format.
    /// </summary>
    public DateTimeFormat Format
    {
      get
      {
        if (this.ViewState["Format"] == null)
        {
          return DateTimeFormat.Both;
        }

        return this.ViewState["Format"].ToEnum<DateTimeFormat>();
      }

      set
      {
        this.ViewState["Format"] = value;
      }
    }

    /// <summary>
    ///   Gets AsDateTime.
    /// </summary>
    protected DateTime AsDateTime
    {
      get
      {
        if (this.DateTime != null)
        {
          try
          {
            return Convert.ToDateTime(this.DateTime);
          }
          catch (InvalidCastException)
          {
            // not useable...            
          }
        }

        return System.DateTime.MinValue;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      if (this.Visible && this.DateTime != null)
      {
        writer.Write(
          this._controlHtml.FormatWith(
            this.AsDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"), this.Get<IDateTime>().Format(this.Format, this.DateTime)));
        writer.WriteLine();
      }
    }

    #endregion
  }
}