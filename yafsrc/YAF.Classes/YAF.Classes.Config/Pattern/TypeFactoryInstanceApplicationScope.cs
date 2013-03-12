/* YetAnotherForum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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