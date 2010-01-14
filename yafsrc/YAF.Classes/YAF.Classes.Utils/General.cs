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
using System;
using System.Reflection;
using System.Security;
using System.Web;
using System.Xml;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// Summary description for General Utils.
  /// </summary>
  public static class General
  {
    /// <summary>
    /// The get safe raw url.
    /// </summary>
    /// <returns>
    /// The get safe raw url.
    /// </returns>
    public static string GetSafeRawUrl()
    {
      return GetSafeRawUrl(HttpContext.Current.Request.RawUrl);
    }

    /// <summary>
    /// Cleans up a URL so that it doesn't contain any problem characters.
    /// </summary>
    /// <param name="url">
    /// </param>
    /// <returns>
    /// The get safe raw url.
    /// </returns>
    public static string GetSafeRawUrl(string url)
    {
      string tProcessedRaw = url;
      tProcessedRaw = tProcessedRaw.Replace("\"", string.Empty);
      tProcessedRaw = tProcessedRaw.Replace("<", "%3C");
      tProcessedRaw = tProcessedRaw.Replace(">", "%3E");
      tProcessedRaw = tProcessedRaw.Replace("&", "%26");
      return tProcessedRaw.Replace("'", string.Empty);
    }

    /// <summary>
    /// Helper function for the Language TimeZone XML.
    /// </summary>
    /// <param name="node">
    /// </param>
    /// <returns>
    /// The get hour offset from node.
    /// </returns>
    public static decimal GetHourOffsetFromNode(XmlNode node)
    {
      // calculate hours -- can use prefix of either UTC or GMT...
      decimal hours = 0;

      try
      {
        hours = Convert.ToDecimal(node.Attributes["tag"].Value.Replace("UTC", string.Empty).Replace("GMT", string.Empty));
      }
      catch (FormatException ex)
      {
        hours = Convert.ToDecimal(node.Attributes["tag"].Value.Replace(".", ",").Replace("UTC", string.Empty).Replace("GMT", string.Empty));
      }

      return hours;
    }

    /// <summary>
    /// The trace resources.
    /// </summary>
    /// <returns>
    /// The trace resources.
    /// </returns>
    public static string TraceResources()
    {
      Assembly a = Assembly.GetExecutingAssembly();

      // get a list of resource names from the manifest
      string[] resNames = a.GetManifestResourceNames();

      // populate the textbox with information about our resources
      // also look for images and put them in our arraylist
      string txtInfo = string.Empty;

      txtInfo += String.Format("Found {0} resources\r\n", resNames.Length);
      txtInfo += "----------\r\n";
      foreach (string s in resNames)
      {
        txtInfo += s + "\r\n";
      }

      txtInfo += "----------\r\n";

      return txtInfo;
    }

    /* Ederon : 9/12/2007 */

    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(object value, object checkAgainst)
    {
      return BinaryAnd(SqlDataLayerConverter.VerifyInt32(value), SqlDataLayerConverter.VerifyInt32(checkAgainst));
    }

    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(int value, int checkAgainst)
    {
      return (value & checkAgainst) == checkAgainst;
    }

    /// <summary>
    /// The encode message.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The encode message.
    /// </returns>
    public static string EncodeMessage(string message)
    {
      if (message.IndexOf('<') >= 0)
      {
        return HttpUtility.HtmlEncode(message);
      }

      return message;
    }

    /// <summary>
    /// Gets the current ASP.NET Hosting Security Level.
    /// </summary>
    /// <returns>
    /// </returns>
    public static AspNetHostingPermissionLevel GetCurrentTrustLevel()
    {
      foreach (AspNetHostingPermissionLevel trustLevel in
        new[]
          {
            AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High, AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low, 
            AspNetHostingPermissionLevel.Minimal
          })
      {
        try
        {
          new AspNetHostingPermission(trustLevel).Demand();
        }
        catch (SecurityException)
        {
          continue;
        }

        return trustLevel;
      }

      return AspNetHostingPermissionLevel.None;
    }

    /// <summary>
    /// Compares two messages.
    /// </summary>
    /// <param name="originalMessage">
    /// Original message text.
    /// </param>
    /// <param name="newMessage">
    /// New message text.
    /// </param>
    /// <returns>
    /// True if messages differ, false if they are identical.
    /// </returns>
    public static bool CompareMessage(object originalMessage, object newMessage)
    {
      return (String) originalMessage != (String) newMessage;
    }
  }
}