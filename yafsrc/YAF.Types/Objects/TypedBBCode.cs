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
namespace YAF.Types.Objects
{
  #region Using

  using System;
  using System.Data;

  #endregion

  /// <summary>
  /// The typed bb code.
  /// </summary>
  [Serializable]
  public class TypedBBCode
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedBBCode"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedBBCode([NotNull] DataRow row)
    {
      this.BBCodeID = row.Field<int?>("BBCodeID");
      this.BoardID = row.Field<int?>("BoardID");
      this.Name = row.Field<string>("Name");
      this.Description = row.Field<string>("Description");
      this.OnClickJS = row.Field<string>("OnClickJS");
      this.DisplayJS = row.Field<string>("DisplayJS");
      this.EditJS = row.Field<string>("EditJS");
      this.DisplayCSS = row.Field<string>("DisplayCSS");
      this.SearchRegex = row.Field<string>("SearchRegex");
      this.ReplaceRegex = row.Field<string>("ReplaceRegex");
      this.Variables = row.Field<string>("Variables");
      this.UseModule = row.Field<bool?>("UseModule");
      this.ModuleClass = row.Field<string>("ModuleClass");
      this.ExecOrder = row.Field<int?>("ExecOrder");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedBBCode"/> class.
    /// </summary>
    /// <param name="bbcodeid">
    /// The bbcodeid.
    /// </param>
    /// <param name="boardid">
    /// The boardid.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="onclickjs">
    /// The onclickjs.
    /// </param>
    /// <param name="displayjs">
    /// The displayjs.
    /// </param>
    /// <param name="editjs">
    /// The editjs.
    /// </param>
    /// <param name="displaycss">
    /// The displaycss.
    /// </param>
    /// <param name="searchregex">
    /// The searchregex.
    /// </param>
    /// <param name="replaceregex">
    /// The replaceregex.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="usemodule">
    /// The usemodule.
    /// </param>
    /// <param name="moduleclass">
    /// The moduleclass.
    /// </param>
    /// <param name="execorder">
    /// The execorder.
    /// </param>
    public TypedBBCode(
      int? bbcodeid, 
      int? boardid, [NotNull] string name, [NotNull] string description, [NotNull] string onclickjs, [NotNull] string displayjs, [NotNull] string editjs, [NotNull] string displaycss, [NotNull] string searchregex, [NotNull] string replaceregex, [NotNull] string variables, 
      bool? usemodule, [NotNull] string moduleclass, 
      int? execorder)
    {
      this.BBCodeID = bbcodeid;
      this.BoardID = boardid;
      this.Name = name;
      this.Description = description;
      this.OnClickJS = onclickjs;
      this.DisplayJS = displayjs;
      this.EditJS = editjs;
      this.DisplayCSS = displaycss;
      this.SearchRegex = searchregex;
      this.ReplaceRegex = replaceregex;
      this.Variables = variables;
      this.UseModule = usemodule;
      this.ModuleClass = moduleclass;
      this.ExecOrder = execorder;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets BBCodeID.
    /// </summary>
    public int? BBCodeID { get; set; }

    /// <summary>
    /// Gets or sets BoardID.
    /// </summary>
    public int? BoardID { get; set; }

    /// <summary>
    /// Gets or sets Description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets DisplayCSS.
    /// </summary>
    public string DisplayCSS { get; set; }

    /// <summary>
    /// Gets or sets DisplayJS.
    /// </summary>
    public string DisplayJS { get; set; }

    /// <summary>
    /// Gets or sets EditJS.
    /// </summary>
    public string EditJS { get; set; }

    /// <summary>
    /// Gets or sets ExecOrder.
    /// </summary>
    public int? ExecOrder { get; set; }

    /// <summary>
    /// Gets or sets ModuleClass.
    /// </summary>
    public string ModuleClass { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets OnClickJS.
    /// </summary>
    public string OnClickJS { get; set; }

    /// <summary>
    /// Gets or sets ReplaceRegex.
    /// </summary>
    public string ReplaceRegex { get; set; }

    /// <summary>
    /// Gets or sets SearchRegex.
    /// </summary>
    public string SearchRegex { get; set; }

    /// <summary>
    /// Gets or sets UseModule.
    /// </summary>
    public bool? UseModule { get; set; }

    /// <summary>
    /// Gets or sets Variables.
    /// </summary>
    public string Variables { get; set; }

    #endregion
  }
}