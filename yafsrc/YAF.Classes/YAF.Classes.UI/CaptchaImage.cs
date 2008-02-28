/* Yet Another Forum.net
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.UI
{
	/// <summary>
	/// CaptchaImage Class
	/// Thanks to "prujohn" on the YAF Forum for his work.
	/// </summary>
	public class CaptchaImage : IDisposable
	{
		// Public properties (all read-only).
		public string Text
		{
			get { return this.text; }
		}
		public Bitmap Image
		{
			get { return this.image; }
		}
		public int Width
		{
			get { return this.width; }
		}
		public int Height
		{
			get { return this.height; }
		}

		// Internal properties.
		private string text;
		private int width;
		private int height;
		private string familyName;
		private Bitmap image;

		// For generating random numbers.

		// ====================================================================
		// Initializes a new instance of the CaptchaImage class using the
		// specified text, width and height.
		// ====================================================================
		public CaptchaImage( string s, int width, int height )
		{
			this.text = s;
			this.SetDimensions( width, height );
			this.GenerateImage();
		}


		// ====================================================================
		// Initializes a new instance of the CaptchaImage class using the
		// specified text, width, height and font family.
		// ====================================================================
		public CaptchaImage( string s, int width, int height, string familyName )
		{
			this.text = s;
			this.SetDimensions( width, height );
			this.SetFamilyName( familyName );
			this.GenerateImage();
		}

		// ====================================================================
		// Sets the image width and height.
		// ====================================================================
		private void SetDimensions( int width, int height )
		{
			// Check the width and height.
			if ( width <= 0 )
				throw new ArgumentOutOfRangeException( "width", width, "Argument out of range, must be greater than zero." );
			if ( height <= 0 )
				throw new ArgumentOutOfRangeException( "height", height, "Argument out of range, must be greater than zero." );
			this.width = width;
			this.height = height;
		}

		// ====================================================================
		// Sets the font used for the image text.
		// ====================================================================
		private void SetFamilyName( string familyName )
		{
			// If the named font is not installed, default to a system font.
			try
			{
				Font font = new Font( this.familyName, 12F );
				this.familyName = familyName;
				font.Dispose();
			}
			catch
			{
				this.familyName = System.Drawing.FontFamily.GenericSerif.Name;
			}
		}

		// ====================================================================
		// Creates the bitmap image.
		// ====================================================================
		private void GenerateImage()
		{
			Random r = new Random();
			// Create a new 32-bit bitmap image.
			Bitmap bitmap = new Bitmap( this.width, this.height, PixelFormat.Format32bppArgb );

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage( bitmap );
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle rect = new Rectangle( 0, 0, this.width, this.height );

			// Fill in the background.
			HatchBrush hatchBrush = new HatchBrush( HatchStyle.SmallConfetti, Color.LightGray, Color.White );
			g.FillRectangle( hatchBrush, rect );

			// Set up the text font.
			SizeF size;
			float fontSize = rect.Height + 1;
			Font font;
			// Adjust the font size until the text fits within the image.
			do
			{
				fontSize--;
				font = new Font( this.familyName, fontSize, FontStyle.Bold );
				size = g.MeasureString( this.text, font );
			} while ( size.Width > rect.Width );

			// Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			// Create a path using the text and warp it randomly.
			GraphicsPath path = new GraphicsPath();
			path.AddString( this.text, font.FontFamily, ( int ) font.Style, font.Size, rect, format );
			float v = 4F;
			PointF [] points =
			{
				new PointF(r.Next(rect.Width) / v, r.Next(rect.Height) / v),
				new PointF(rect.Width - r.Next(rect.Width) / v, r.Next(rect.Height) / v),
				new PointF(r.Next(rect.Width) / v, rect.Height - r.Next(rect.Height) / v),
				new PointF(rect.Width - r.Next(rect.Width) / v, rect.Height - r.Next(rect.Height) / v)
			};
			Matrix matrix = new Matrix();
			matrix.Translate( 0F, 0F );
			path.Warp( points, rect, matrix, WarpMode.Perspective, 0F );

			// Draw the text.
			hatchBrush = new HatchBrush( HatchStyle.LargeConfetti, Color.LightSkyBlue, Color.DarkGray );
			g.FillPath( hatchBrush, path );
			
			// Add some random noise.
			int m = Math.Max( rect.Width, rect.Height );
			for ( int i = 0; i < ( int ) ( rect.Width * rect.Height / 30F ); i++ )
			{
				int x = r.Next( rect.Width );
				int y = r.Next( rect.Height );
				int w = r.Next( m / 50 );
				int h = r.Next( m / 50 );
				g.FillEllipse( hatchBrush, x, y, w, h );
			}

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();

			// Set the image.
			this.image = bitmap;
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			GC.SuppressFinalize( this );
			this.image.Dispose();
		}

		#endregion
	}
}
