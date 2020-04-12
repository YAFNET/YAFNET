/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils.Helpers.ImageUtils
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

    // Public properties (all read-only).
    /// <summary>
    ///   The family name.
    /// </summary>
    private string familyName;

    #endregion

    // For generating random numbers.

    // ====================================================================
    // Initializes a new instance of the CaptchaImage class using the
    // specified text, width and height.
    // ====================================================================
    #region Constructors and Destructors

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
      this.Text = s;
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
    public int Height { get; private set; }

    /// <summary>
    ///   Gets Image.
    /// </summary>
    public Bitmap Image { get; private set; }

    /// <summary>
    ///   Gets Text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///   Gets Width.
    /// </summary>
    public int Width { get; private set; }

    #endregion

    #region Implemented Interfaces

    #region IDisposable

    /// <summary>
    /// The i disposable. dispose.
    /// </summary>
    void IDisposable.Dispose()
    {
      GC.SuppressFinalize(this);
      this.Image.Dispose();
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
      var bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);

      // Create a graphics object for drawing.
      var g = Graphics.FromImage(bitmap);
      g.SmoothingMode = SmoothingMode.AntiAlias;
      var rect = new Rectangle(0, 0, this.Width, this.Height);

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
        size = g.MeasureString(this.Text, font);
      }
      while (size.Width > rect.Width);

      // Set up the text format.
      var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

      // Create a path using the text and warp it randomly.
      var path = new GraphicsPath();
      path.AddString(this.Text, font.FontFamily, (int)font.Style, font.Size, rect, format);
      var v = 4F;
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
      var m = Math.Max(rect.Width, rect.Height);
      for (var i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
      {
        var x = r.Next(rect.Width);
        var y = r.Next(rect.Height);
        var w = r.Next(m / (this.random.Next(1000) + 50));
        var h = r.Next(m / (this.random.Next(1000) + 50));

        g.FillEllipse(hatchBrush, x, y, w, h);
      }

      double noise = this.random.Next(35) + 35;

      var maxDim = Math.Max(rect.Width, rect.Height);
      var radius = (int)(maxDim * noise / 3000);
      var maxGran = (int)(rect.Width * rect.Height / (100 - (noise >= 90 ? 90 : noise)));
      for (var i = 0; i < maxGran; i++)
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
        var lines = (int)_lines / 30 + 1;
        using (var pen = new Pen(hatchBrush, 1))
          for (var i = 0; i < lines; i++)
          {
            var pointsLine = new PointF[lines > 2 ? lines - 1 : 2];
            for (var j = 0; j < pointsLine.Length; j++)
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
      this.Image = bitmap;
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
        throw new ArgumentOutOfRangeException(nameof(width), width, "Argument out of range, must be greater than zero.");
      }

      if (height <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(height), height, "Argument out of range, must be greater than zero.");
      }

      this.Width = width;
      this.Height = height;
    }

    // ====================================================================
    // Sets the font used for the image text.
    // ====================================================================
    /// <summary>
    /// The set family name.
    /// </summary>
    /// <param name="name">
    /// The family name.
    /// </param>
    private void SetFamilyName([NotNull] string name)
    {
      // If the named font is not installed, default to a system font.
      try
      {
        var font = new Font(this.familyName, 14F);
        this.familyName = name;
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