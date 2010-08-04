/* Yet Another Forum.net
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
namespace YAF.Classes.Extensions
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class EnumerableExtensions
  {
    /// <summary>
    /// Iterates through a generic list type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
      foreach (var item in list.ToList())
      {
        action(item);
      }
    }

    /// <summary>
    /// Iterates through a list with a isFirst flag.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="action"></param>
    public static void ForEachFirst<T>(this IEnumerable<T> list,  Action<T, bool> action)
    {
      bool isFirst = true;
      foreach (var item in list.ToList())
      {
        action(item, isFirst);
        isFirst = false;
      }
    }

    /// <summary>
    /// Iterates through a list with a index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="action"></param>
    public static void ForEachIndex<T>(this IEnumerable<T> list, Action<T, int> action)
    {
      int i = 0;
      foreach (var item in list.ToList())
      {
        action(item, i++);
      }
    }    
  }
}