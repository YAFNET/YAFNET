/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.CoreTests.Helpers;

using System;
using System.IO;
using System.Runtime.InteropServices;

using HttpSimulator = HttpSimulator.HttpSimulator;

/// <summary>
/// The mime types tests.
/// </summary>
[TestFixture]
public class MimeTypesTests
{
    /// <summary>
    /// The file match content type test.
    /// </summary>
    [Test]
    public void FileMatchContentType_Test()
    {
        using (new HttpSimulator("/", TestConfig.TestFilesDirectory).SimulateRequest())
        {
            var testFile = Path.GetFullPath(@"..\..\..\testfiles\avatar.png");

            var fileInfo = new FileInfo(testFile);

            Assert.AreEqual(true, MimeTypes.FileMatchContentType(fileInfo.Name, GetMimeFromFile(testFile)), GetMimeFromFile(testFile));
        }
    }

    /// <summary>Finds the MIME from data.</summary>
    /// <param name="pBC">The p bc.</param>
    /// <param name="pwzUrl">The PWZ URL.</param>
    /// <param name="pBuffer">The p buffer.</param>
    /// <param name="cbSize">Size of the cb.</param>
    /// <param name="pwzMimeProposed">The PWZ MIME proposed.</param>
    /// <param name="dwMimeFlags">The dw MIME flags.</param>
    /// <param name="ppwzMimeOut">The PPWZ MIME out.</param>
    /// <param name="dwReserved">The dw reserved.</param>
    /// <returns>System.Int32.</returns>
    [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
    private static extern int FindMimeFromData(
        IntPtr pBC,
        [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
        int cbSize,
        [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
        int dwMimeFlags,
        out IntPtr ppwzMimeOut,
        int dwReserved);

    /// <summary>
    /// The get mime from file.
    /// </summary>
    /// <param name="file">
    /// The file.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private static string GetMimeFromFile(string file)
    {
        if (!File.Exists(file))
        {
            throw new FileNotFoundException(file + " not found");
        }

        var maxContent = (int)new FileInfo(file).Length;

        if (maxContent > 4096)
        {
            maxContent = 4096;
        }

        var fs = File.OpenRead(file);

        var buf = new byte[maxContent];
        fs.Read(buf, 0, maxContent);
        fs.Close();
        var result = FindMimeFromData(IntPtr.Zero, file, buf, maxContent, null, 0, out var mimeOut, 0);

        if (result != 0)
        {
            throw Marshal.GetExceptionForHR(result);
        }

        var mime = Marshal.PtrToStringUni(mimeOut);
        Marshal.FreeCoTaskMem(mimeOut);
        return mime;
    }
}