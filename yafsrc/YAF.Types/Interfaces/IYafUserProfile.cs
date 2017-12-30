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
namespace YAF.Types.Interfaces
{
  using System;

  /// <summary>
  /// The yaf user profile interface.
  /// </summary>
  public interface IYafUserProfile
  {
    #region Properties

    /// <summary>
    /// Gets or sets AIM.
    /// </summary>
    string AIM { get; set; }

    /// <summary>
    /// Gets or sets Birthday.
    /// </summary>
    DateTime Birthday { get; set; }

    /// <summary>
    /// Gets or sets Blog.
    /// </summary>
    string Blog { get; set; }

    /// <summary>
    /// Gets or sets BlogServicePassword.
    /// </summary>
    string BlogServicePassword { get; set; }

    /// <summary>
    /// Gets or sets BlogServiceUrl.
    /// </summary>
    string BlogServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets BlogServiceUsername.
    /// </summary>
    string BlogServiceUsername { get; set; }

    /// <summary>
    /// Gets or sets Gender.
    /// </summary>
    int Gender { get; set; }

    /// <summary>
    /// Gets or sets Google.
    /// </summary>
    string Google { get; set; }

    /// <summary>
    /// Gets or sets Google ID.
    /// </summary>
    string GoogleId { get; set; }

    /// <summary>
    /// Gets or sets Homepage.
    /// </summary>
    string Homepage { get; set; }

    /// <summary>
    /// Gets or sets ICQ.
    /// </summary>
    string ICQ { get; set; }

    /// <summary>
    /// Gets or sets Facebook.
    /// </summary>
    string Facebook { get; set; }

    /// <summary>
    /// Gets or sets the facebook Id, 
    /// private Field only for SSO
    /// </summary>
    /// <value>
    /// The facebook Id.
    /// </value>
    string FacebookId { get; set; }

    /// <summary>
    /// Gets or sets Twitter.
    /// </summary>
    string Twitter { get; set; }

    /// <summary>
    /// Gets or sets Twitter Id, 
    /// private Field only for SSO
    /// </summary>
    string TwitterId { get; set; }

    /// <summary>
    /// Gets or sets Interests.
    /// </summary>
    string Interests { get; set; }

    /// <summary>
    /// Gets or sets Location.
    /// </summary>
    string Location { get; set; }

    /// <summary>
    /// Gets or sets Country.
    /// </summary>
    string Country { get; set; }

    /// <summary>
    /// Gets or sets Region or State(US).
    /// </summary>
    string Region { get; set; }

    /// <summary>
    /// Gets or sets a City.
    /// </summary>
    string City { get; set; }

    /// <summary>
    /// Gets or sets MSN.
    /// </summary>
    string MSN { get; set; }

    /// <summary>
    /// Gets or sets Occupation.
    /// </summary>
    string Occupation { get; set; }

    /// <summary>
    /// Gets or sets RealName.
    /// </summary>
    string RealName { get; set; }

    /// <summary>
    /// Gets or sets Skype.
    /// </summary>
    string Skype { get; set; }

    /// <summary>
    /// Gets or sets XMPP.
    /// </summary>
    string XMPP { get; set; }

    /// <summary>
    /// Gets or sets YIM.
    /// </summary>
    string YIM { get; set; }

    /// <summary>
    /// Gets or sets Last Synced With DNN.
    /// </summary>
    DateTime LastSyncedWithDNN { get; set; }

    #endregion
  }
}