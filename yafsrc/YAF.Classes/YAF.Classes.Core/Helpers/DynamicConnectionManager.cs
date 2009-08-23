using System;
using System.Collections.Generic;
using System.Text;
using YAF.Classes.Data;

namespace YAF.Classes.Core
{
	public class YafDynamicDBConnManager : YafDBConnManager
	{
		public override string ConnectionString
		{
			get
			{
				if ( YafContext.Current.Vars.ContainsKey( "ConnectionString" ) )
				{
					return YafContext.Current.Vars["ConnectionString"] as string;
				}

				return Config.ConnectionString;
			}
		}
	}
}
