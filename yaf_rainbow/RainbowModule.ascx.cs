namespace yaf_rainbow
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Rainbow.UI;
	using Rainbow.UI.WebControls;
	using Rainbow.UI.DataTypes;
	using Rainbow.Configuration;
	using yaf;

	/// <summary>
	///		Summary description for RainbowModule.
	/// </summary>
	public class RainbowModule : PortalModuleControl
	{
		public override Guid GuidID
		{
			get
			{
				return new Guid("{5F582DF5-5567-438d-8E2D-07D012EE80FB}");
			}
		}
		protected override void OnInit(System.EventArgs e)
		{
			Load += new EventHandler(RainbowModule_Load);
			base.OnInit(e);
		}
		public RainbowModule()
		{
			Rainbow.Configuration.SettingItem boardID = new Rainbow.Configuration.SettingItem(new Rainbow.UI.DataTypes.IntegerDataType());
			boardID.Required = true;
			boardID.Order = 1;
			boardID.Value = "1";
			this._baseSettings.Add("BoardID",boardID);
			Rainbow.Configuration.SettingItem categoryID = new Rainbow.Configuration.SettingItem(new Rainbow.UI.DataTypes.IntegerDataType());
			categoryID.Required = false;
			categoryID.Order = 2;
			//categoryID.Value = string.Empty;
			this._baseSettings.Add("CategoryID",categoryID);
		}
		private void RainbowModule_Load(object sender, EventArgs e)
		{
			base.Cacheable = false;
			this.ModuleConfiguration.Cacheable = false;

			foreach(Control _c in this.Controls)
			{
				if(_c.GetType()==typeof(yaf.Forum)) 
				{
					Forum f = (Forum)_c;
					try 
					{
						f.BoardID = int.Parse(Settings["BoardID"].ToString());
						if(Settings["CategoryID"].ToString()!=string.Empty)
							f.CategoryID = int.Parse(Settings["CategoryID"].ToString());
					}
					catch(Exception)
					{
						f.BoardID = 1;
					}
					break;
				}
			}
		}
		public override bool Cacheable
		{
			get
			{
				return false;
			}
		}
	}
}
