/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Core.Nntp
{
  using System.IO;

  /// <summary>
  /// The attachment.
  /// </summary>
  public class Attachment
  {
    /// <summary>
    /// The binary data.
    /// </summary>
    private byte[] binaryData;

    /// <summary>
    /// The filename.
    /// </summary>
    private string filename;

    /// <summary>
    /// The id.
    /// </summary>
    private string id;

    /// <summary>
    /// Initializes a new instance of the <see cref="Attachment"/> class.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="filename">
    /// The filename.
    /// </param>
    /// <param name="binaryData">
    /// The binary data.
    /// </param>
    public Attachment(string id, string filename, byte[] binaryData)
    {
      this.id = id;
      this.filename = filename;
      this.binaryData = binaryData;
    }

    /// <summary>
    /// Gets Id.
    /// </summary>
    public string Id
    {
      get
      {
        return this.id;
      }
    }

    /// <summary>
    /// Gets Filename.
    /// </summary>
    public string Filename
    {
      get
      {
        return this.filename;
      }
    }

    /// <summary>
    /// Gets BinaryData.
    /// </summary>
    public byte[] BinaryData
    {
      get
      {
        return this.binaryData;
      }
    }

    /// <summary>
    /// The save as.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    public void SaveAs(string path)
    {
      this.SaveAs(path, false);
    }

    /// <summary>
    /// The save as.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <param name="isOverwrite">
    /// The is overwrite.
    /// </param>
    public void SaveAs(string path, bool isOverwrite)
    {
      FileStream fs = null;
      if (isOverwrite)
      {
        fs = new FileStream(path, FileMode.Create);
      }
      else
      {
        fs = new FileStream(path, FileMode.CreateNew);
      }

      fs.Write(this.binaryData, 0, this.binaryData.Length);
      fs.Close();
    }
  }
}