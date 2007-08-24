/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Security;
using System.Web.Security;

namespace YAF.Classes.Utils
{
  static public class Security
  {
    /// <summary>
    /// Function that verifies a string is an integer value or it redirects to invalid "info" page.
    /// Used as a security feature against invalid values submitted to the page.
    /// </summary>
    /// <param name="longValue">The string value to test</param>
    /// <returns>The converted long value</returns>
    public static long StringToLongOrRedirect(string longValue)
    {
      long value = 0;

      try
      {
        value = long.Parse( longValue );
      }
      catch
      {
        // it's an invalid request. Redirect to the info page on invalid requests.
        yaf_BuildLink.Redirect( ForumPages.info, "i=6" );
      }

      return value;
    }
  }
}
