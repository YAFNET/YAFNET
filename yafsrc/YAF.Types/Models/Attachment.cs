/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Types.Models
{
    using System;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The attachment.
    /// </summary>
    [Serializable]
    public partial class Attachment : IEntity, IHaveID
    {
        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class.
        /// </summary>
        public Attachment()
        {
            this.OnCreated();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [AutoIncrement]
        [Alias("AttachmentID")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        //[Default(0)]
        public int MessageID { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the bytes.
        /// </summary>
        [Required]
        public int Bytes { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [References(typeof(User))]
        [Required]
        [Default(0)]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the downloads.
        /// </summary>
        [Required]
        public int Downloads { get; set; }

        /// <summary>
        /// Gets or sets the file data.
        /// </summary>
        public byte[] FileData { get; set; }

        #endregion
    }
}