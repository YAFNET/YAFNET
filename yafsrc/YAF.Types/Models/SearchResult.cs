/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

    [Serializable]
    public partial class SearchResult
    {
        public SearchResult()
        {
            this.OnCreated();
        }

        partial void OnCreated();

        public int ForumID { get; set; }

        public int TopicID { get; set; }

        public string Topic { get; set; }

        public int UserID { get; set; }

        public string Name { get; set; }

        public int MessageID { get; set; }

        public DateTime Posted { get; set; }

        public string Message { get; set; }

        public int Flags { get; set; }
    }
}