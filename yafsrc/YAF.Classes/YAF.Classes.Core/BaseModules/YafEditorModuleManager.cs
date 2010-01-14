/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Editors
{
  using System;
  using System.Data;
  using System.Web.Compilation;

  using Modules;

  /// <summary>
  /// The yaf editor module manager.
  /// </summary>
  public class YafEditorModuleManager : YafModuleManager<BaseForumEditor>
  {
    /// <summary>
    /// Prevents a default instance of the <see cref="YafEditorModuleManager"/> class from being created.
    /// </summary>
    private YafEditorModuleManager()
      : base("YAF.Editors", "YAF.Editors.IBaseEditorModule")
    {
      if (this.ModuleClassTypes == null)
      {
        // re-add these modules...
        this.AddModules(BuildManager.CodeAssemblies);
      }
    }

    /// <summary>
    /// The get editor instance.
    /// </summary>
    /// <param name="moduleId">
    /// The module id.
    /// </param>
    /// <returns>
    /// </returns>
    public BaseForumEditor GetEditorInstance(int moduleId)
    {
      this.Load();

      // find the module (LINQ would be nice here)...
      foreach (BaseForumEditor editor in this.Modules)
      {
        if (editor.ModuleId == moduleId)
        {
          return editor;
        }
      }

      // not found
      return null;
    }

    /// <summary>
    /// The get editors table.
    /// </summary>
    /// <returns>
    /// </returns>
    public DataTable GetEditorsTable()
    {
      this.Load();

      using (var dt = new DataTable("Editors"))
      {
        dt.Columns.Add("Value", Type.GetType("System.Int32"));
        dt.Columns.Add("Name", Type.GetType("System.String"));

        foreach (BaseForumEditor editor in this.Modules)
        {
          if (editor.Active)
          {
            dt.Rows.Add(
              new object[]
                {
                  editor.ModuleId, editor.Description
                });
          }
        }

        return dt;
      }
    }
  }
}