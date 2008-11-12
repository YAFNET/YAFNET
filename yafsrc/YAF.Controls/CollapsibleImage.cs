/* Yet Another Forum.NET
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
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public class CollapsibleImage : ImageButton
	{
		public YafContext PageContext
		{
			get
			{
				if ( this.Site != null && this.Site.DesignMode == true )
				{
					// design-time, return null...
					return null;
				}
				return YafContext.Current;
			}
		}

		public CollapsibleImage()
		{

		}

		protected override void OnClick( ImageClickEventArgs e )
		{
			// toggle the status...
			Mession.PanelState.TogglePanelState( PanelID, DefaultState );

			base.OnClick( e );
		}

		protected override void OnPreRender( EventArgs e )
		{
			// setup inital image state...
			this.ImageUrl = PageContext.Theme.GetCollapsiblePanelImageURL( PanelID, DefaultState );
			UpdateAttachedVisibility();

			base.OnPreRender( e );
		}

		protected void UpdateAttachedVisibility()
		{
			if ( GetAttachedControl() != null )
			{
				// modify the visability depending on the status...
				GetAttachedControl().Visible = ( PanelState == PanelSessionState.CollapsiblePanelState.Expanded );
			}			
		}

		protected Control GetAttachedControl()
		{
			Control control = null;

			if ( !String.IsNullOrEmpty( AttachedControlID ))
			{
				// attempt to find this control...
				control = Parent.FindControl( AttachedControlID ) as Control;
			}

			return control;
		}

		public string PanelID
		{
			get
			{
				if ( ViewState ["PanelID"] != null )
					return ViewState ["PanelID"].ToString();

				return null;
			}
			set { ViewState ["PanelID"] = value; }
		}

		public PanelSessionState.CollapsiblePanelState PanelState
		{
			get
			{
				return Mession.PanelState [PanelID];
			}
			set
			{
				Mession.PanelState [PanelID] = value;
			}			
		}

		public PanelSessionState.CollapsiblePanelState DefaultState
		{
			get
			{
				PanelSessionState.CollapsiblePanelState defaultState = PanelSessionState.CollapsiblePanelState.Expanded;

				if ( ViewState ["DefaultState"] != null )
					defaultState = ( PanelSessionState.CollapsiblePanelState ) ViewState["DefaultState"];

				if ( defaultState == PanelSessionState.CollapsiblePanelState.None )
				{
					defaultState = PanelSessionState.CollapsiblePanelState.Expanded;
				}

				return defaultState;
			}
			set
			{
				ViewState ["DefaultState"] = value;
			}			
		}

		public string AttachedControlID
		{
			get
			{
				if ( ViewState ["AttachedControlID"] != null )
					return ViewState ["AttachedControlID"].ToString();

				return null;
			}
			set { ViewState ["AttachedControlID"] = value; }			
		}
	}
}
