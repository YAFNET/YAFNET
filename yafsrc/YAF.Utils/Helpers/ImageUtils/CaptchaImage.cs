/* Yet Another Forum.net
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
namespace YAF.Utils
{
  #region Using

  using System;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Drawing.Imaging;

  using YAF.Types;

  #endregion

  /// <summary>
  /// CaptchaImage Class
  ///   Thanks to "prujohn" on the YAF Forum for his work.
  /// </summary>
  public class CaptchaImage : IDisposable
  {
    #region Constants and Fields

    /// <summary>
    /// The random.
    /// </summary>
    private readonly Random random;

    /// <summary>
    ///   The text.
    /// </summary>
    private readonly string text;

    // Public properties (all read-only).
    /// <summary>
    ///   The family name.
    /// </summary>
    private string familyName;

    /// <summary>
    ///   The height.
    /// </summary>
    private int height;

    /// <summary>
    ///   The image.
    /// </summary>
    private Bitmap image;

    /// <summary>
    ///   The width.
    /// </summary>
    private int width;

    #endregion

    // For generating random numbers.

    // ====================================================================
    // Initializes a new instance of the CaptchaImage class using the
    // specified text, width and height.
    // ====================================================================
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CaptchaImage"/> class.
    /// </summary>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    public CaptchaImage([NotNull] string s, int width, int height)
      : this(s, width, height, string.Empty)
    {
    }

    // ====================================================================
    // Initializes a new instance of the CaptchaImage class using the
    // specified text, width, height and font family.
    // ====================================================================
    /// <summary>
    /// Initializes a new instance of the <see cref="CaptchaImage"/> class.
    /// </summary>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    /// <param name="familyName">
    /// The family name.
    /// </param>
    public CaptchaImage([NotNull] string s, int width, int height, [NotNull] string familyName)
    {
      this.text = s;
      this.random = new Random((int)DateTime.Now.Ticks);
      this.SetDimensions(width, height);
      this.SetFamilyName(familyName);
      this.GenerateImage();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Height.
    /// </summary>
    public int Height
    {
      get
      {
        return this.height;
      }
    }

    /// <summary>
    ///   Gets Image.
    /// </summary>
    public Bitmap Image
    {
      get
      {
        return this.image;
      }
    }

    /// <summary>
    ///   Gets Text.
    /// </summary>
    public string Text
    {
      get
      {
        return this.text;
      }
    }

    /// <summary>
    ///   Gets Width.
    /// </summary>
    public int Width
    {
      get
      {
        return this.width;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IDisposable

    /// <summary>
    /// The i disposable. dispose.
    /// </summary>
    void IDisposable.Dispose()
    {
      GC.SuppressFinalize(this);
      this.image.Dispose();
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The generate image.
    /// </summary>
    private void GenerateImage()
    {
      var r = new Random();

      // Create a new 32-bit bitmap image.
      var bitmap = new Bitmap(this.width, this.height, PixelFormat.Format32bppArgb);

      // Create a graphics object for drawing.
      Graphics g = Graphics.FromImage(bitmap);
      g.SmoothingMode = SmoothingMode.AntiAlias;
      var rect = new Rectangle(0, 0, this.width, this.height);

      var randomLineColor = this.random.Next(40) + 200;

      // Fill in the background.
      var hatchBrush = new HatchBrush(
        HatchStyle.SmallConfetti, Color.FromArgb(randomLineColor, randomLineColor, randomLineColor), Color.White);
      g.FillRectangle(hatchBrush, rect);

      // Set up the text font.
      SizeF size;
      float fontSize = rect.Height + 1;
      Font font;

      // Adjust the font size until the text fits within the image.
      do
      {
        fontSize--;
        font = new Font(this.familyName, fontSize, FontStyle.Bold);
        size = g.MeasureString(this.text, font);
      }
      while (size.Width > rect.Width);

      // Set up the text format.
      var format = new StringFormat();
      format.Alignment = StringAlignment.Center;
      format.LineAlignment = StringAlignment.Center;

      // Create a path using the text and warp it randomly.
      var path = new GraphicsPath();
      path.AddString(this.text, font.FontFamily, (int)font.Style, font.Size, rect, format);
      float v = 4F;
      PointF[] points = {
                          new PointF(r.Next(rect.Width) / v, r.Next(rect.Height) / v), 
                          new PointF(rect.Width - r.Next(rect.Width) / v, r.Next(rect.Height) / v), 
                          new PointF(r.Next(rect.Width) / v, rect.Height - r.Next(rect.Height) / v), 
                          new PointF(rect.Width - r.Next(rect.Width) / v, rect.Height - r.Next(rect.Height) / v)
                        };
      var matrix = new Matrix();
      matrix.Translate(0F, 0F);
      path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

      var randomColor = Color.FromArgb(
        this.random.Next(100) + 100, this.random.Next(100) + 100, this.random.Next(100) + 100);
      var randomBackground = Color.FromArgb(
        20 + this.random.Next(100), 20 + this.random.Next(100), 20 + this.random.Next(100));

      // Draw the text.
      hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, randomColor, randomBackground);
      g.FillPath(hatchBrush, path);

      // Add some random noise.
      int m = Math.Max(rect.Width, rect.Height);
      for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
      {
        int x = r.Next(rect.Width);
        int y = r.Next(rect.Height);
        int w = r.Next(m / (this.random.Next(1000) + 50));
        int h = r.Next(m / (this.random.Next(1000) + 50));

        g.FillEllipse(hatchBrush, x, y, w, h);
      }

      double noise = this.random.Next(35) + 35;

      int maxDim = Math.Max(rect.Width, rect.Height);
      var radius = (int)(maxDim * noise / 3000);
      var maxGran = (int)(rect.Width * rect.Height / (100 - (noise >= 90 ? 90 : noise)));
      for (int i = 0; i < maxGran; i++)
      {
        g.FillEllipse(
          hatchBrush, 
          this.random.Next(rect.Width), 
          this.random.Next(rect.Height), 
          this.random.Next(radius), 
          this.random.Next(radius));
      }

      double _lines = this.random.Next(25) + 15;

      if (_lines > 0)
      {
        int lines = ((int)_lines / 30) + 1;
        using (var pen = new Pen(hatchBrush, 1))
          for (int i = 0; i < lines; i++)
          {
            var pointsLine = new PointF[lines > 2 ? lines - 1 : 2];
            for (int j = 0; j < pointsLine.Length; j++)
            {
              pointsLine[j] = new PointF(this.random.Next(rect.Width), this.random.Next(rect.Height));
            }

            g.DrawCurve(pen, pointsLine, 1.75F);
          }
      }

      // Clean up.
      font.Dispose();
      hatchBrush.Dispose();

      g.Dispose();

      // Set the image.
      this.image = bitmap;
    }

    // ====================================================================
    // Sets the image width and height.
    // ====================================================================
    /// <summary>
    /// The set dimensions.
    /// </summary>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    private void SetDimensions(int width, int height)
    {
      // Check the width and height.
      if (width <= 0)
      {
        throw new ArgumentOutOfRangeException("width", width, "Argument out of range, must be greater than zero.");
      }

      if (height <= 0)
      {
        throw new ArgumentOutOfRangeException("height", height, "Argument out of range, must be greater than zero.");
      }

      this.width = width;
      this.height = height;
    }

    // ====================================================================
    // Sets the font used for the image text.
    // ====================================================================
    /// <summary>
    /// The set family name.
    /// </summary>
    /// <param name="familyName">
    /// The family name.
    /// </param>
    private void SetFamilyName([NotNull] string familyName)
    {
      // If the named font is not installed, default to a system font.
      try
      {
        var font = new Font(this.familyName, 14F);
        this.familyName = familyName;
        font.Dispose();
      }
      catch
      {
        this.familyName = FontFamily.GenericSerif.Name;
      }
    }

    #endregion

    // ====================================================================
    // Creates the bitmap image.
    // ====================================================================
  }
}