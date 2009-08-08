using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Yaf Header Interface
	/// </summary>
	public interface IYafHeader
	{
		bool SimpleRender
		{
			get;
			set;
		}
		string RefreshURL
		{
			get;
			set;
		}

		int RefreshTime
		{
			get;
			set;
		}

		Control ThisControl
		{
			get;
		}
	}

	/// <summary>
	/// Yaf Footer Interface
	/// </summary>
	public interface IYafFooter
	{
		bool SimpleRender
		{
			get;
			set;
		}

		Control ThisControl
		{
			get;
		}
	}
}
