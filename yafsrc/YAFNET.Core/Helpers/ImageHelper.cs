/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

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
    /// <param name="imageFormat">
    /// The Image Format
    /// </param>
    /// <param name="x">
    ///     The image width.
    /// </param>
    /// <param name="y">
    ///     The image height.
    /// </param>
    /// <returns>
    /// A resized image stream Stream.
    /// </returns>
    public static MemoryStream GetResizedImage(Image image, IImageFormat imageFormat, long x, long y)
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

        image.Mutate(context => context.Resize((int)newWidth, (int)newHeight, KnownResamplers.Lanczos3));

        var resized = new MemoryStream();

        image.Save(resized, imageFormat);

        return resized;
    }
}