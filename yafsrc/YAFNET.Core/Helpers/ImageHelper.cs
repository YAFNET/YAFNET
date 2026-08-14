/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Core.Helpers;

using System;
using System.IO;

using SkiaSharp;

/// <summary>
/// The image helper.
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// The quality used when encoding images as WebP.
    /// </summary>
    private const int WebpQuality = 90;

    /// <summary>
    /// Returns resized image stream, encoded as WebP.
    /// </summary>
    /// <param name="image">
    ///     The Image.
    /// </param>
    /// <param name="x">
    ///     The image width.
    /// </param>
    /// <param name="y">
    ///     The image height.
    /// </param>
    /// <returns>
    /// A resized image stream.
    /// </returns>
    public static MemoryStream GetResizedImage(SKBitmap image, long x, long y)
    {
        double newWidth = image.Width;
        double newHeight = image.Height;

        if (newWidth > x)
        {
            newHeight = newHeight * x / newWidth;
            newWidth = x;
        }

        if (newHeight > y)
        {
            newWidth = newWidth * y / newHeight;
            newHeight = y;
        }

        // Resize
        var info = new SKImageInfo((int)newWidth, (int)newHeight);

        using var resizedImage = image.Resize(info, new SKSamplingOptions(SKCubicResampler.Mitchell));

        if (resizedImage is null)
        {
            throw new InvalidOperationException("Unable to resize image.");
        }

        // Save the result
        var resized = new MemoryStream();

        SaveAsWebp(resizedImage, resized);

        return resized;
    }

    /// <summary>
    /// Encodes the image as WebP into the given stream.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="stream">The destination stream.</param>
    public static void SaveAsWebp(SKBitmap image, Stream stream)
    {
        Save(image, SKEncodedImageFormat.Webp, stream);
    }

    /// <summary>
    /// Encodes the image using the given format into the given stream.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="format">The encoded image format.</param>
    /// <param name="stream">The destination stream.</param>
    public static void Save(SKBitmap image, SKEncodedImageFormat format, Stream stream)
    {
        if (!image.Encode(stream, format, WebpQuality))
        {
            throw new InvalidOperationException($"Unable to encode image as {format}.");
        }
    }
}