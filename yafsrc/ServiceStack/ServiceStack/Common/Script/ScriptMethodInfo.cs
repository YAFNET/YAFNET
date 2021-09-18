// ***********************************************************************
// <copyright file="ScriptMethodInfo.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// Class ScriptMethodInfo.
    /// </summary>
    public class ScriptMethodInfo
    {
        /// <summary>
        /// The method information
        /// </summary>
        private readonly MethodInfo methodInfo;
        /// <summary>
        /// The parameters
        /// </summary>
        private readonly ParameterInfo[] @params;
        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <returns>MethodInfo.</returns>
        public MethodInfo GetMethodInfo() => methodInfo;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => methodInfo.Name;
        /// <summary>
        /// Gets the first parameter.
        /// </summary>
        /// <value>The first parameter.</value>
        public string FirstParam => @params.FirstOrDefault()?.Name;
        /// <summary>
        /// Gets the first type of the parameter.
        /// </summary>
        /// <value>The first type of the parameter.</value>
        public string FirstParamType => @params.FirstOrDefault()?.ParameterType.Name;
        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public string ReturnType => methodInfo.ReturnType?.Name;
        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        /// <value>The parameter count.</value>
        public int ParamCount => @params.Length;

        /// <summary>
        /// Gets the remaining parameters.
        /// </summary>
        /// <value>The remaining parameters.</value>
        public string[] RemainingParams => @params.Length > 1
            ? @params.Skip(1).Select(x => x.Name).ToArray()
            : TypeConstants.EmptyStringArray;
        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        /// <value>The parameter names.</value>
        public string[] ParamNames => @params.Select(x => x.Name).ToArray();
        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <value>The parameter types.</value>
        public string[] ParamTypes => @params.Select(x => x.ParameterType.Name.ToString()).ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptMethodInfo"/> class.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="params">The parameters.</param>
        /// <exception cref="System.ArgumentNullException">methodInfo</exception>
        /// <exception cref="System.ArgumentNullException">params</exception>
        public ScriptMethodInfo(MethodInfo methodInfo, ParameterInfo[] @params)
        {
            this.methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            this.@params = @params ?? throw new ArgumentNullException(nameof(@params));
        }

        /// <summary>
        /// Gets the script methods.
        /// </summary>
        /// <param name="scriptMethodsType">Type of the script methods.</param>
        /// <param name="where">The where.</param>
        /// <returns>List&lt;ScriptMethodInfo&gt;.</returns>
        public static List<ScriptMethodInfo> GetScriptMethods(Type scriptMethodsType, Func<MethodInfo, bool> where = null)
        {
            var filters = scriptMethodsType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var to = filters
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetParameters().Length)
                .Where(x => x.DeclaringType != typeof(ScriptMethods) && x.DeclaringType != typeof(object))
                .Where(m => !m.IsSpecialName);

            if (where != null)
                to = to.Where(where);

            return to.Select(Create).ToList();
        }

        /// <summary>
        /// Creates the specified mi.
        /// </summary>
        /// <param name="mi">The mi.</param>
        /// <returns>ScriptMethodInfo.</returns>
        public static ScriptMethodInfo Create(MethodInfo mi)
        {
            var pis = mi.GetParameters()
                .Where(x => x.ParameterType != typeof(ScriptScopeContext)).ToArray();

            return new ScriptMethodInfo(mi, pis);
        }

        /// <summary>
        /// Gets the return.
        /// </summary>
        /// <value>The return.</value>
        public string Return => ReturnType != null && ReturnType != nameof(StopExecution) ? " -> " + ReturnType : "";

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>The body.</value>
        public string Body => ParamCount == 0
            ? $"{Name}"
            : ParamCount == 1
                ? $"|> {Name}"
                : $"|> {Name}(" + string.Join(", ", RemainingParams) + $")";

        /// <summary>
        /// Gets the script signature.
        /// </summary>
        /// <value>The script signature.</value>
        public string ScriptSignature => ParamCount == 0
            ? $"{Name}{Return}"
            : ParamCount == 1
                ? $"{FirstParam} |> {Name}{Return}"
                : $"{FirstParam} |> {Name}(" + string.Join(", ", RemainingParams) + $"){Return}";

        /// <summary>
        /// The signature
        /// </summary>
        private string signature;
        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <value>The signature.</value>
        public string Signature
        {
            get
            {
                if (signature != null)
                    return signature;

                var sb = StringBuilderCache.Allocate()
                    .Append(Name);

                if (@params.Length > 0)
                {
                    sb.Append(" (");
                    for (var i = 0; i < @params.Length; i++)
                    {
                        sb.Append(i > 0 ? ", " : "")
                            .Append(@params[i].ParameterType.Name)
                            .Append(" ")
                            .Append(@params[i].Name);
                    }
                    sb.Append(")");
                }
                sb.Append(Return);

                return signature = StringBuilderCache.ReturnAndFree(sb);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => Signature;

        /// <summary>
        /// Converts to scriptmethodtype.
        /// </summary>
        /// <returns>ScriptMethodType.</returns>
        public ScriptMethodType ToScriptMethodType() => new ScriptMethodType
        {
            Name = Name,
            ParamNames = ParamNames,
            ParamTypes = ParamTypes,
            ReturnType = ReturnType,
        };
    }
}
