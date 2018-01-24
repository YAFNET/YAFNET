/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Core.Services
{
  using System.Diagnostics;

  using YAF.Types.Interfaces;

  /// <summary>
  /// The yaf stop watch.
  /// </summary>
  public class YafStopWatch : IStopWatch
  {
    /// <summary>
    /// The _stop watch.
    /// </summary>
    private readonly Stopwatch _stopWatch = new Stopwatch();

    /// <summary>
    /// Gets Watch.
    /// </summary>
    public Stopwatch Watch
    {
      get
      {
        return this._stopWatch;
      }
    }

    /// <summary>
    /// Gets Duration.
    /// </summary>
    public double Duration
    {
      get
      {
        var duration = (double) this._stopWatch.ElapsedMilliseconds/1000.0;
        return duration;
      }
    }

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
      this._stopWatch.Start();
    }

    /// <summary>
    /// The stop.
    /// </summary>
    public void Stop()
    {
      this._stopWatch.Stop();
    }

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      this._stopWatch.Reset();
    }
  }
}