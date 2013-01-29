/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Types.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the BBCode table.
    /// </summary>
    [Serializable]
    public partial class BBCode : IEntity, IHaveBoardID, IHaveID
    {
        partial void OnCreated();

        public BBCode()
        {
            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("BBCodeID")]
        public int ID { get; set; }

        public int BoardID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string OnClickJS { get; set; }

        public string DisplayJS { get; set; }

        public string EditJS { get; set; }

        public string DisplayCSS { get; set; }

        public string SearchRegex { get; set; }

        public string ReplaceRegex { get; set; }

        public string Variables { get; set; }

        public bool? UseModule { get; set; }

        public string ModuleClass { get; set; }

        public int ExecOrder { get; set; }

        #endregion
    }
}