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

using SkiaSharp;

namespace YAF.Core.Helpers;

using System.IO;

/// <summary>
/// The image helper.
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// Returns resized image stream.
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
        using var resizedBitmap = image.Resize(
            new SKImageInfo((int)newWidth, (int)newHeight),
            SKFilterQuality.High  // High = Lanczos-equivalent quality
        );

        // Encode to WebP and save to MemoryStream
        using var resizedImage = SKImage.FromBitmap(resizedBitmap);
        var resized = new MemoryStream();
        using var encoded = resizedImage.Encode(SKEncodedImageFormat.Webp, 100);
        encoded.SaveTo(resized);
        resized.Position = 0; // reset if you need to read it afterward

        return resized;
    }

    public static SKBitmap LoadImage(Stream stream)
    {
        using var skiaStream = new SKManagedStream(stream);
        return SKBitmap.Decode(skiaStream);
    }
}