// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Utilities
{
	/// <summary>
	/// Helper class for dealing with types.
	/// </summary>
	internal sealed class TypeHelper
	{
		private TypeHelper()
		{
		}

		/// <summary>
		/// Loads and activates a type
		/// </summary>
		/// <param name="fullTypeName">The full name of the type to activate "TypeName, AssemblyName"</param>
		/// <param name="args">Arguments to pass to the constructor</param>
		/// <returns>The object</returns>
		public static object Activate(string fullTypeName, object[] args)
		{
			string[] components = fullTypeName.Split(",".ToCharArray(), 2);
			if (components.Length != 2)
			{
				throw new ArgumentOutOfRangeException("fullTypeName", fullTypeName, MessageProvider.FormatString(Message.FullTypeNameRequiresAssemblyName));
			}

			return Activate(components[1].Trim(), components[0].Trim(), args);
		}

		/// <summary>
		/// Loads and activates a type
		/// </summary>
		/// <param name="assemblyName">The assembly name</param>
		/// <param name="typeName">The type name</param>
		/// <param name="args">Arguments to pass to the constructor</param>
		/// <returns>The object</returns>
		public static object Activate(string assemblyName, string typeName, object[] args)
		{
			if (assemblyName.Length == 0)
			{
				throw new ArgumentOutOfRangeException("assembly", assemblyName, MessageProvider.FormatString(Message.AssemblyNameRequired));
			}

			if (typeName.Length == 0)
			{
				throw new ArgumentOutOfRangeException("typeName", typeName, MessageProvider.FormatString(Message.TypeNameRequired));
			}

			return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName, false, 0, null, args, null, null, null);
		}
	}
}
