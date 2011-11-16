// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbCommandExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The db command extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Dynamic;
	using System.Linq;

	using YAF.Types;

	/// <summary>
	/// The db command extensions.
	/// </summary>
	public static class DbCommandExtensions
	{
		#region Public Methods

		/// <summary>
		/// Extension method for adding in a bunch of parameters
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="parameters">
		/// </param>
		/// <param name="excludeNames">
		/// List of parameter exclusions.
		/// </param>
		public static void AddDynamicParams(
			[NotNull] this DbCommand cmd, [NotNull] dynamic parameters, [NotNull] params string[] excludeNames)
		{
			CodeContracts.ArgumentNotNull(cmd, "cmd");
			CodeContracts.ArgumentNotNull(parameters, "parameters");

			IDictionary<string, object> dictionary = ((object)parameters).AnyToDictionary();

			var excludeList = new List<string>();

			if (excludeNames != null)
			{
				excludeList.AddRange(excludeNames.Select(x => x.ToLower()));
			}

			foreach (var item in dictionary.Where(x => !excludeList.Contains(x.Key.ToLower())))
			{
				AddParam(cmd, item);
			}
		}

		/// <summary>
		/// Extension method for adding a parameter
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="item">
		/// The item.
		/// </param>
		public static void AddParam([NotNull] this DbCommand cmd, [CanBeNull] object item)
		{
			CodeContracts.ArgumentNotNull(cmd, "cmd");

			if (item is KeyValuePair<string, object>)
			{
				AddParam(cmd, (KeyValuePair<string, object>)item);
			}
			else
			{
				AddParam(cmd, new KeyValuePair<string, object>(null, item));
			}
		}

		/// <summary>
		/// Extension method for adding a parameter
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <param name="item">
		/// The item.
		/// </param>
		public static void AddParam([NotNull] this DbCommand cmd, [NotNull] string name, [CanBeNull] object item)
		{
			CodeContracts.ArgumentNotNull(cmd, "cmd");
			CodeContracts.ArgumentNotNull(name, "name");

			AddParam(cmd, new KeyValuePair<string, object>(name, item));
		}

		/// <summary>
		/// Extension for adding single parameter named or automatically named by number (0, 1, 2, 3, 4, etc.)
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="param">
		/// The param.
		/// </param>
		public static void AddParam([NotNull] this DbCommand cmd, KeyValuePair<string, object> param)
		{
			CodeContracts.ArgumentNotNull(cmd, "cmd");

			var item = param.Value;

			var p = cmd.CreateParameter();

			p.ParameterName = "{0}".FormatWith(param.Key.IsSet() ? param.Key : cmd.Parameters.Count.ToString());

			if (item == null)
			{
				p.Value = DBNull.Value;
			}
			else
			{
				if (item is Guid)
				{
					p.Value = item.ToString();
					p.DbType = DbType.String;
					p.Size = 4000;
				}
				else if (item.GetType() == typeof(ExpandoObject))
				{
					var d = (IDictionary<string, object>)item;
					p.Value = d.Values.FirstOrDefault();
				}
				else
				{
					p.Value = item;
				}

				if (item is string)
				{
					var asString = item as string;

					if (asString.Length < 4000)
					{
						p.Size = 4000;
					}
					else
					{
						p.Size = -1;
					}
				}
			}

			cmd.Parameters.Add(p);
		}

		/// <summary>
		/// Extension method for adding in a bunch of parameters
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public static void AddParams([NotNull] this DbCommand cmd, [NotNull] params object[] args)
		{
			CodeContracts.ArgumentNotNull(cmd, "cmd");
			CodeContracts.ArgumentNotNull(args, "args");

			foreach (var item in args)
			{
				AddParam(cmd, item);
			}
		}

		/// <summary>
		/// The create output parameter.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="parameterName">
		/// The parameter name.
		/// </param>
		/// <param name="dbType">
		/// The db type.
		/// </param>
		/// <param name="size">
		/// The size.
		/// </param>
		/// <param name="direction">
		/// The direction.
		/// </param>
		public static void CreateOutputParameter([NotNull] this DbCommand cmd, [NotNull] string parameterName, 
		                                         DbType dbType, 
		                                         int size = 0, 
		                                         ParameterDirection direction = ParameterDirection.Output)
		{
			CodeContracts.ArgumentNotNull(cmd, "cmd");
			CodeContracts.ArgumentNotNull(parameterName, "parameterName");

			var p = cmd.CreateParameter();

			p.ParameterName = parameterName;
			p.DbType = dbType;
			p.Size = size;
			p.Direction = direction;

			cmd.Parameters.Add(p);
		}

		#endregion
	}
}