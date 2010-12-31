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

namespace YAF.Types.Interfaces
{
    using System.Text;

    /// <summary>
    /// The Script builder Interface.
    /// </summary>
    public interface IScriptBuilder
    {
        /// <summary>
        /// Gets StringBuilder Scripts
        /// </summary>
        StringBuilder Scripts { get; }

        /// <summary>
        /// Add the script to jQuery document.ready
        /// </summary>
        /// <param name="javasScript">
        /// script body which need to register
        /// </param>
        /// <returns>
        /// The jquery document ready script.
        /// </returns>
        IScriptBuilder JqueryDocumentReadyScript(string javasScript);

        /// <summary>
        /// Add a Complete Java Script Function
        /// </summary>
        /// <param name="javasScript">
        /// the Complete Java Script Function
        /// </param>
        /// <returns>
        /// The jquery Function script.
        /// </returns>
        IScriptBuilder AddFunctionComplete([NotNull] string javasScript);

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
        IScriptBuilder AddFunction(string functionName, string[] functionParams, string functionScript);

        /// <summary>
        /// Add the script jQuery.noConflict();
        /// </summary>
        /// <returns>
        /// The jquery no conflict.
        /// </returns>
        IScriptBuilder JqueryNoConflict();

        /// <summary>
        /// Get the script's result as String
        /// </summary>
        /// <returns>
        /// The Complete Script
        /// </returns>
        string Build();
    }
}
