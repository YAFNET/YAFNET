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
using System.Data;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Editors;

namespace YAF.Editors
{
	public abstract class RichClassEditor : BaseForumEditor
	{
		protected bool _init;
		protected Type _typEditor;
		protected Control _editor;
		protected string _styleSheet;

		protected RichClassEditor()
		{
			_init = false;
			_styleSheet = string.Empty;
			_editor = null;
			_typEditor = null;
		}

		protected RichClassEditor(string classBinStr)
		{
			_init = false;
			_styleSheet = string.Empty;
			_editor = null;

			try
			{
				_typEditor = Type.GetType(classBinStr, true);
			}
			catch (Exception x)
			{
/*
#if DEBUG
				throw new Exception( "Unable to load editor class/dll: " + classBinStr + " Exception: " + x.Message );
#else
				YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Error);
#endif
*/ 
			}
		}

		protected bool InitEditorObject()
		{
			try
			{
				if (!_init && _typEditor != null)
				{
					// create instance of main class
					_editor = (System.Web.UI.Control)Activator.CreateInstance(_typEditor);
					_init = true;
				}
			}
			catch (Exception)
			{
				// dll is not accessible
				return false;
			}
			return true;
		}

		protected Type GetInterfaceInAssembly(Assembly cAssembly, string className)
		{
			Type[] types = cAssembly.GetTypes();
			foreach (Type typ in types)
			{
				// dynamically create or activate(if exist) object
				if (typ.FullName == className)
				{
					return typ;
				}
			}
			return null;
		}

		#region Properties

		public override bool Active
		{
			get
			{
				return _typEditor != null;
			}
		}

		virtual protected string SafeID
		{
			get
			{
				if (_init)
				{
					return _editor.ClientID.Replace("$", "_");
				}
				return string.Empty;
			}
		}

		public bool IsInitialized
		{
			get { return _init; }
		}

		public override string StyleSheet
		{
			get { return _styleSheet; }
			set { _styleSheet = value; }
		}

		public override bool UsesHTML
		{
			get { return true; }
		}
		public override bool UsesBBCode
		{
			get { return false; }
		}
		#endregion

	}
}