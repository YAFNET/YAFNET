/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using YAF.Classes.Core;
using YAF.Controls;
using YAF.Modules;

namespace YAF.Editors
{
	/// <summary>
	/// IBaseEditorModule Interface for Editor classes.
	/// </summary>
	internal interface IBaseEditorModule
	{
		bool Active
		{
			get;
		}

		string Description
		{
			get;
		}

		int ModuleId
		{
			get;
		}	
	}

	/// <summary>
	/// Summary description for BaseForumEditor.
	/// </summary>
	public abstract class BaseForumEditor : BaseControl, IBaseEditorModule
	{
		protected RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
		protected string _baseDir = string.Empty;

		public new string ResolveUrl( string relativeUrl )
		{
			if ( _baseDir != string.Empty )
				return _baseDir + relativeUrl;

			return base.ResolveUrl( relativeUrl );
		}

		protected virtual string Replace( string txt, string match, string replacement )
		{
			while (Regex.IsMatch(txt, match, _options)) txt = Regex.Replace(txt, match, replacement, _options);
			return txt;
		}

		#region Virtual Properties

		public abstract string Text
		{
			get;
			set;
		}

		public virtual string BaseDir
		{
			set
			{
				_baseDir = value;
				if ( !_baseDir.EndsWith( "/" ) )
					_baseDir += "/";
			}
		}
		public virtual string StyleSheet
		{
			get { return string.Empty; }
			set { ;	}
		}

		public virtual bool UsesHTML
		{
			get { return false; }
		}

		public virtual bool UsesBBCode
		{
			get { return false; }
		}
		#endregion

		#region IBaseEditorModule Members

		public abstract bool Active
		{
			get;
		}

		public abstract string Description
		{
			get;
		}

		public virtual int ModuleId
		{
			get
			{
				return this.Description.GetHashCode();
			}
		}

		#endregion
	}

	public class YafEditorModuleManager : YafModuleManager<BaseForumEditor>
	{
		YafEditorModuleManager()
			: base("YAF.Editors", "YAF.Editors.IBaseEditorModule")
		{
			if ( ModuleClassTypes == null )
			{
				// re-add these modules...
				base.AddModules( BuildManager.CodeAssemblies );
			}
		}

		public BaseForumEditor GetEditorInstance( int moduleId )
		{
			Load();

			// find the module (LINQ would be nice here)...
      foreach ( BaseForumEditor editor in Modules)
      {
				if (editor.ModuleId == moduleId)
      	{
      		return editor;
      	}
      }

			// not found
			return null;
		}

		public DataTable GetEditorsTable()
		{
			Load();

			using (DataTable dt = new DataTable("Editors"))
			{
				dt.Columns.Add("Value", Type.GetType("System.Int32"));
				dt.Columns.Add("Name", Type.GetType("System.String"));

				foreach ( BaseForumEditor editor in Modules )
				{
					if ( editor.Active )
					{
						dt.Rows.Add(new object[] { editor.ModuleId, editor.Description });
					}
				}
				return dt;
			}
		}
	}
}
