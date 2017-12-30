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
namespace YAF.Classes.Pattern
{
  #region Using

  using System;
  using System.Web;

  #endregion

  /// <summary>
  /// The type application factory instance.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public class TypeFactoryInstanceApplicationScope<T> : ITypeFactoryInstance<T>
  {
    #region Constants and Fields

    /// <summary>
    /// The _type instance key.
    /// </summary>
    private string _typeInstanceKey;

    /// <summary>
    /// The _type name.
    /// </summary>
    private string _typeName;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeFactoryInstanceApplicationScope{T}"/> class.
    /// </summary>
    /// <param name="typeName">
    /// The type name.
    /// </param>
    public TypeFactoryInstanceApplicationScope(string typeName)
    {
      this.TypeName = typeName;
      this.TypeInstanceKey = typeName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeFactoryInstanceApplicationScope{T}"/> class. 
    /// For derived classes
    /// </summary>
    protected TypeFactoryInstanceApplicationScope()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets TypeInstanceKey.
    /// </summary>
    protected string TypeInstanceKey
    {
      get
      {
        return this._typeInstanceKey;
      }

      set
      {
        this._typeInstanceKey = value;
      }
    }

    /// <summary>
    /// Gets or sets the TypeName.
    /// </summary>
    public string TypeName
    {
      get
      {
        return this._typeName;
      }

      protected set
      {
        this._typeName = value;
      }
    }

    private object _instance = null;

    /// <summary>
    /// Gets or sets ApplicationInstance.
    /// </summary>
    protected T ApplicationInstance
    {
      get
      {
        if (HttpContext.Current == null)
        {
          return this._instance == null ? default(T) : (T)this._instance;
        }

        return (T)(HttpContext.Current.Application[this.TypeInstanceKey] ?? default(T));
      }

      set
      {
        if (HttpContext.Current == null)
        {
          _instance = value;
        }
        else
        {
          HttpContext.Current.Application[this.TypeInstanceKey] = value;
        }
      }
    }

    #endregion

    #region Implemented Interfaces

    #region ITypeFactoryInstance<T>

    /// <summary>
    /// The create.
    /// </summary>
    /// <returns>
    /// </returns>
    public T Get()
    {
      if (Equals(this.ApplicationInstance, default(T)))
      {
        this.ApplicationInstance = this.CreateTypeInstance();
      }
      
      return this.ApplicationInstance;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The create type instance.
    /// </summary>
    /// <returns>
    /// Created type instance.
    /// </returns>
    private T CreateTypeInstance()
    {
      Type typeGetType = Type.GetType(this.TypeName);

      // validate this type is proper for this type instance.
      if (typeof(T).IsAssignableFrom(typeGetType))
      {
        return (T)Activator.CreateInstance(typeGetType);
      }

      return default(T);
    }

    #endregion
  }
}