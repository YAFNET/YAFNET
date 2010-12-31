/* Yet Another Forum.NET
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

namespace YAF.Core.Services
{
    #region Using

    using System.Text;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Jquery Java Script Builder
    /// </summary>
    public class JavaScriptBuilder : IScriptBuilder
    {
        /// <summary>
        /// The Script
        /// </summary>
        protected StringBuilder scripts = new StringBuilder();

        /// <summary>
        /// Gets StringBuilder Scripts
        /// </summary>
        public StringBuilder Scripts
        {
            get { return this.scripts; }
        }

        /// <summary>
        /// Add the script to document.ready event
        /// </summary>
        /// <param name="javasScript">
        /// script body which need to register
        /// </param>
        /// <returns>
        /// The jquery document ready script.
        /// </returns>
        public IScriptBuilder JqueryDocumentReadyScript([NotNull] string javasScript)
        {
            CodeContracts.ArgumentNotNull(javasScript, "javasScript");

            this.scripts.AppendFormat("{0}().ready(function() {{", Config.JQueryAlias);
            this.scripts.Append(javasScript);
            this.scripts.Append("});");

            return this;
        }

        /// <summary>
        /// Add a Complete Java Script Function
        /// </summary>
        /// <param name="javasScript">
        /// the Complete Java Script Function
        /// </param>
        /// <returns>
        /// The jquery Function script.
        /// </returns>
        public IScriptBuilder AddFunctionComplete([NotNull] string javasScript)
        {
            CodeContracts.ArgumentNotNull(javasScript, "javasScript");

            // Dirty Way to replace the jquery Variable(Alias) Method from the Script
            if (javasScript.Contains("jQuery"))
            {
                javasScript = javasScript.Replace("jQuery", Config.JQueryAlias);
            }

            this.scripts.Append(javasScript);

            return this;
        }

        /// <summary>
        /// Add A JavaScript Function
        /// </summary>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        /// <param name="functionParams">
        /// The function params.
        /// </param>
        /// <param name="functionScript">
        /// The function script.
        /// </param>
        /// <returns>
        /// The Script
        /// </returns>
        public IScriptBuilder AddFunction([NotNull] string functionName, [CanBeNull] string[] functionParams, [NotNull] string functionScript)
        {
            CodeContracts.ArgumentNotNull(functionName, "functionName");
            CodeContracts.ArgumentNotNull(functionScript, "functionScript");

            this.scripts.Append("function");

            if (!string.IsNullOrEmpty(functionName))
            {
                this.scripts.AppendFormat(" {0}", functionName);
            }

            this.scripts.Append("(");

            if (functionParams != null)
            {
                if (functionParams.Length > 0)
                {
                    this.scripts.Append(string.Join(",", functionParams));
                }
            }

            this.scripts.Append(")");
            this.scripts.AppendFormat("{{ {0} }}", functionScript);

            return this;
        }

        /// <summary>
        /// Add the script jQuery.noConflict();
        /// </summary>
        /// <returns>
        /// The jquery no conflict.
        /// </returns>
        public IScriptBuilder JqueryNoConflict()
        {
            this.scripts.AppendFormat("{0}.noConflict();", Config.JQueryAlias);
            return this;
        }

        /// <summary>
        /// Get the script's result as String
        /// </summary>
        /// <returns>
        /// The Complete Script
        /// </returns>
        public string Build()
        {
            return this.scripts.ToString();
        }
    }
}