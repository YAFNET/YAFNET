/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Modules
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Reflection;
  using YAF.Classes.Core;

  /// <summary>
  /// Generic Module Management (Plugin) class.
  /// </summary>
  /// <typeparam name="T">Type of Modules the Manager Manages
  /// </typeparam>
  public abstract class YafModuleManager<T> where T : class
  {
    /// <summary>
    /// The _cache name.
    /// </summary>
    protected string _cacheName = String.Empty;

    /// <summary>
    /// The _loaded.
    /// </summary>
    protected bool _loaded = false;

    /// <summary>
    /// The _module base type.
    /// </summary>
    private string _moduleBaseType;

    /// <summary>
    /// The _module class types.
    /// </summary>
    private List<Type> _moduleClassTypes = null;

    /// <summary>
    /// The _module class factories.
    /// </summary>
    protected Dictionary<Type, YafFactory> _moduleClassFactories = null;

    /// <summary>
    /// Generic Type for the Instance Factory.
    /// </summary>
    private static Type genericFactoryType = typeof(YafGenericFactory<>);

    /// <summary>
    /// The _module namespace.
    /// </summary>
    private string _moduleNamespace;

    /// <summary>
    /// The _modules.
    /// </summary>
    private List<T> _modules = new List<T>();

    /// <summary>
    /// Initializes a new instance of the <see cref="YafModuleManager{T}"/> class.
    /// </summary>
    /// <param name="moduleNamespace">
    /// The module namespace.
    /// </param>
    /// <param name="moduleBaseType">
    /// The module base type.
    /// </param>
    protected YafModuleManager(string moduleNamespace, string moduleBaseType)
    {
      ModuleNamespace = moduleNamespace;
      ModuleBaseType = moduleBaseType;
    }

    /// <summary>
    /// Base Module Namespace for the Module Manager
    /// </summary>
    public string ModuleNamespace
    {
      get
      {
        return this._moduleNamespace;
      }

      protected set
      {
        this._moduleNamespace = value;
      }
    }

    /// <summary>
    /// Base type of this instance of the Module Manager
    /// </summary>
    public string ModuleBaseType
    {
      get
      {
        return this._moduleBaseType;
      }

      protected set
      {
        this._moduleBaseType = value;
      }
    }

    /// <summary>
    /// Gets CacheName.
    /// </summary>
    protected string CacheName
    {
      get
      {
        return "YafModuleManager_" + ModuleBaseType;
      }
    }

    /// <summary>
    /// Gets or sets ModuleClassTypes.
    /// </summary>
    protected List<Type> ModuleClassTypes
    {
      get
      {
        if (this._moduleClassTypes == null && YafContext.Current.Cache[CacheName] != null)
        {
          this._moduleClassTypes = YafContext.Current.Cache[CacheName] as List<Type>;
        }

        return this._moduleClassTypes;
      }

      set
      {
        if (value == null)
        {
          YafContext.Current.Cache.Remove(CacheName);
        }
        else
        {
          YafContext.Current.Cache[CacheName] = value;
        }

        this._moduleClassTypes = value;
      }
    }

    /// <summary>
    /// Gets or sets ModuleClassFactories.
    /// </summary>
    protected Dictionary<Type, YafFactory> ModuleClassFactories
    {
      get
      {
        if (this._moduleClassFactories == null && YafContext.Current.Cache[CacheName + "_factory"] != null)
        {
          this._moduleClassFactories = YafContext.Current.Cache[CacheName + "_factory"] as Dictionary<Type, YafFactory>;
        }

        return this._moduleClassFactories;
      }

      set
      {
        if (value == null)
        {
          YafContext.Current.Cache.Remove(CacheName + "_factory");
        }
        else
        {
          YafContext.Current.Cache[CacheName + "_factory"] = value;
        }

        this._moduleClassFactories = value;
      }
    }

    /// <summary>
    /// All the modules found by the Module Manager
    /// </summary>
    public List<T> Modules
    {
      get
      {
        return this._modules;
      }
    }

    /// <summary>
    /// Are modules loaded?
    /// </summary>
    public bool Loaded
    {
      get
      {
        return this._loaded;
      }
    }

    /// <summary>
    /// Called when the modules are loaded.
    /// </summary>
    public event EventHandler<EventArgs> LoadModules;

    /// <summary>
    /// Called when the modules are unloaded.
    /// </summary>
    public event EventHandler<EventArgs> UnloadModules;

    /// <summary>
    /// The add modules.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    protected void AddModules(IList assemblies)
    {
      if (ModuleClassTypes == null)
      {
        ModuleClassTypes = FindModules(assemblies, ModuleNamespace, ModuleBaseType);
      }
      else
      {
        ModuleClassTypes.AddRange(FindModules(assemblies, ModuleNamespace, ModuleBaseType));
      }
    }

    /// <summary>
    /// The add modules.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    protected void AddModules(List<Assembly> assemblies)
    {
      if (ModuleClassTypes == null)
      {
        ModuleClassTypes = FindModules(assemblies, ModuleNamespace, ModuleBaseType);
      }
      else
      {
        ModuleClassTypes.AddRange(FindModules(assemblies, ModuleNamespace, ModuleBaseType));
      }
    }

    /// <summary>
    /// Loads the types into the generic object factory for speed.
    /// </summary>
    protected void LoadFactories()
    {
      if (ModuleClassFactories == null)
      {
        ModuleClassFactories = new Dictionary<Type, YafFactory>();

        foreach (Type module in ModuleClassTypes)
        {
          // create cached factories for the classes...
          if (!ModuleClassFactories.ContainsKey(module))
          {
            ModuleClassFactories.Add(module, new YafFactory(module));
          }
        }
      }
    }

    /// <summary>
    /// The find modules.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <param name="moduleNamespace">
    /// The module namespace.
    /// </param>
    /// <param name="moduleBaseInterface">
    /// The module base interface.
    /// </param>
    /// <returns>
    /// </returns>
    protected static List<Type> FindModules(IList assemblies, string moduleNamespace, string moduleBaseInterface)
    {
      var moduleClassTypes = new List<Type>();

      // get classes...
      foreach (Assembly assembly in assemblies)
      {
        var filter = new TypeFilter(BaseModuleFilter);

        foreach (Module module in assembly.GetModules())
        {
          foreach (Type modClass in module.FindTypes(filter, moduleNamespace))
          {
            // don't add any abstract classes...
            if (modClass.IsAbstract)
            {
              continue;
            }

            // YAF.Modules namespace, verify it implements the interface...
            Type[] interfaces = modClass.GetInterfaces();

            foreach (Type inter in interfaces)
            {
              if (inter.ToString() == moduleBaseInterface)
              {
                moduleClassTypes.Add(modClass);
              }
            }
          }
        }
      }

      return moduleClassTypes;
    }

    /// <summary>
    /// Loads (CreatesInstance) of all modules) -- should only be called once.
    /// </summary>
    public void Load()
    {
      if (!this._loaded)
      {
        // are factories loaded?
        if (ModuleClassFactories == null)
        {
          // load factories...
          LoadFactories();
        }

        // create instances of the classes...
        foreach (var factoryKey in ModuleClassFactories.Keys)
        {
          this._modules.Add((T) ModuleClassFactories[factoryKey].Create());
        }

        this._loaded = true;

        if (LoadModules != null)
        {
          LoadModules(this, new EventArgs());
        }
      }
    }

    /// <summary>
    /// Unloads all the modules in the module manager.
    /// </summary>
    public void Unload()
    {
      if (this._loaded)
      {
        this._modules.Clear();
        this._loaded = false;

        if (UnloadModules != null)
        {
          UnloadModules(this, new EventArgs());
        }
      }
    }

    /// <summary>
    /// Helper function that filters modules based on NameSpace
    /// </summary>
    /// <param name="typeObj">
    /// </param>
    /// <param name="criteriaObj">
    /// </param>
    /// <returns>
    /// The base module filter.
    /// </returns>
    public static bool BaseModuleFilter(Type typeObj, object criteriaObj)
    {
      if (typeObj.Namespace == criteriaObj.ToString())
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    #region Embedded Classes

    #region Nested type: YafFactory

    /// <summary>
    /// The factory.
    /// </summary>
    protected class YafFactory
    {
      /// <summary>
      /// The generic factory.
      /// </summary>
      private YafGenericFactoryBase _yafGenericFactory;

      /// <summary>
      /// Initializes a new instance of the <see cref="YafFactory"/> class.
      /// </summary>
      /// <param name="t">
      /// The t.
      /// </param>
      public YafFactory(Type t)
      {
        Type initialisedFactoryType = genericFactoryType.MakeGenericType(
          new Type[]
            {
              typeof(T),
              t
            });
        this._yafGenericFactory = (YafGenericFactoryBase)Activator.CreateInstance(initialisedFactoryType);
      }

      /// <summary>
      /// The create.
      /// </summary>
      /// <returns>
      /// The create.
      /// </returns>
      public object Create()
      {
        return this._yafGenericFactory.CreateObject();
      }
    }

    #endregion

    #region Nested type: YafGenericFactory

    /// <summary>
    /// The generic factory.
    /// </summary>
    private class YafGenericFactory<T> : YafGenericFactoryBase where T : class, new()
    {
      /// <summary>
      /// The create object.
      /// </summary>
      /// <returns>
      /// The create object.
      /// </returns>
      public override object CreateObject()
      {
        return Create();
      }

      /// <summary>
      /// The create.
      /// </summary>
      /// <returns>
      /// </returns>
      public T Create()
      {
        return new T();
      }
    }

    #endregion

    #region Nested type: YafGenericFactoryBase

    /// <summary>
    /// The generic factory base.
    /// </summary>
    private abstract class YafGenericFactoryBase
    {
      /// <summary>
      /// The create object.
      /// </summary>
      /// <returns>
      /// The create object.
      /// </returns>
      public abstract object CreateObject();
    }

    #endregion

    #endregion


  }
}