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
  #region Using

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  using YAF.Classes.Core;

  #endregion

  /// <summary>
  /// Generic Module Management (Plugin) class.
  /// </summary>
  /// <typeparam name="T">
  /// Type of Modules the Manager Manages
  /// </typeparam>
  public abstract class YafModuleManager<T>
    where T : class
  {
    #region Constants and Fields

    /// <summary>
    /// The _cache name.
    /// </summary>
    protected string _cacheName = String.Empty;

    /// <summary>
    /// The _loaded.
    /// </summary>
    protected bool _loaded = false;

    /// <summary>
    /// The _module class factories.
    /// </summary>
    protected Dictionary<Type, YafFactory> _moduleClassFactories = null;

    /// <summary>
    /// Generic Type for the Instance Factory.
    /// </summary>
    private static Type genericFactoryType = typeof(YafGenericFactory<>);

    /// <summary>
    /// The _module base type.
    /// </summary>
    private string _moduleBaseType;

    /// <summary>
    /// The _module class types.
    /// </summary>
    private List<Type> _moduleClassTypes = null;

    /// <summary>
    /// The _module namespace.
    /// </summary>
    private string _moduleNamespace;

    /// <summary>
    /// The _modules.
    /// </summary>
    private List<T> _modules = new List<T>();

    #endregion

    #region Constructors and Destructors

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
      this.ModuleNamespace = moduleNamespace;
      this.ModuleBaseType = moduleBaseType;
    }

    #endregion

    #region Events

    /// <summary>
    /// Called when the modules are loaded.
    /// </summary>
    public event EventHandler<EventArgs> LoadModules;

    /// <summary>
    /// Called when the modules are unloaded.
    /// </summary>
    public event EventHandler<EventArgs> UnloadModules;

    #endregion

    #region Properties

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
    /// Gets CacheName.
    /// </summary>
    protected string CacheName
    {
      get
      {
        return "YafModuleManager_" + this.ModuleBaseType;
      }
    }

    /// <summary>
    /// Gets or sets ModuleClassFactories.
    /// </summary>
    protected Dictionary<Type, YafFactory> ModuleClassFactories
    {
      get
      {
        if (this._moduleClassFactories == null && YafContext.Current.Cache[this.CacheName + "_factory"] != null)
        {
          this._moduleClassFactories =
            YafContext.Current.Cache[this.CacheName + "_factory"] as Dictionary<Type, YafFactory>;
        }

        return this._moduleClassFactories;
      }

      set
      {
        if (value == null)
        {
          YafContext.Current.Cache.Remove(this.CacheName + "_factory");
        }
        else
        {
          YafContext.Current.Cache[this.CacheName + "_factory"] = value;
        }

        this._moduleClassFactories = value;
      }
    }

    /// <summary>
    /// Gets or sets ModuleClassTypes.
    /// </summary>
    protected List<Type> ModuleClassTypes
    {
      get
      {
        if (this._moduleClassTypes == null && YafContext.Current.Cache[this.CacheName] != null)
        {
          this._moduleClassTypes = YafContext.Current.Cache[this.CacheName] as List<Type>;
        }

        return this._moduleClassTypes;
      }

      set
      {
        if (value == null)
        {
          YafContext.Current.Cache.Remove(this.CacheName);
        }
        else
        {
          YafContext.Current.Cache[this.CacheName] = value;
        }

        this._moduleClassTypes = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Loads CreatesInstance of all modules -- should only be called once.
    /// </summary>
    public void Load()
    {
      if (!this._loaded)
      {
        // are factories loaded?
        if (this.ModuleClassFactories == null)
        {
          // load factories...
          this.LoadFactories();
        }

        // create instances of the classes...
        this.CreateModuleClasses();

        this._loaded = true;

        if (this.LoadModules != null)
        {
          this.LoadModules(this, new EventArgs());
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

        if (this.UnloadModules != null)
        {
          this.UnloadModules(this, new EventArgs());
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Finds modules (classes) in the supplied assemblies.
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
      var baseInterface = Type.GetType(moduleBaseInterface);

      // get classes...
      foreach (Assembly assembly in assemblies)
      {
        foreach (Module module in assembly.GetModules())
        {
          var types = module.GetTypes().ToList();

          foreach (Type modClass in types.Where(t => t.Namespace != null && t.Namespace.Equals(moduleNamespace)))
          {
            // don't add any abstract classes...
            if (modClass.IsAbstract)
            {
              continue;
            }

            // verify it implements the interface...
            if (modClass.GetInterfaces().Any(i => i.Equals(baseInterface)))
            {
              // it does, add this class
              moduleClassTypes.Add(modClass);
            }
          }
        }
      }

      return moduleClassTypes;
    }

    /// <summary>
    /// Add modules to the module manager.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    protected void AddModules(IList assemblies)
    {
      if (this.ModuleClassTypes == null)
      {
        this.ModuleClassTypes = FindModules(assemblies, this.ModuleNamespace, this.ModuleBaseType);
      }
      else
      {
        this.ModuleClassTypes.AddRange(FindModules(assemblies, this.ModuleNamespace, this.ModuleBaseType));
      }
    }

    /// <summary>
    /// Add modules to the module manager.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    protected void AddModules(List<Assembly> assemblies)
    {
      if (this.ModuleClassTypes == null)
      {
        this.ModuleClassTypes = FindModules(assemblies, this.ModuleNamespace, this.ModuleBaseType);
      }
      else
      {
        this.ModuleClassTypes.AddRange(FindModules(assemblies, this.ModuleNamespace, this.ModuleBaseType));
      }
    }

    /// <summary>
    /// Creates all the classes in <see cref="ModuleClassFactories"/>.
    /// </summary>
    protected void CreateModuleClasses()
    {
      foreach (var factoryKey in this.ModuleClassFactories.Keys)
      {
        this._modules.Add((T)this.ModuleClassFactories[factoryKey].Create());
      }
    }

    /// <summary>
    /// Loads the types into the generic object factory for speed.
    /// </summary>
    protected void LoadFactories()
    {
      if (this.ModuleClassFactories == null)
      {
        this.ModuleClassFactories = new Dictionary<Type, YafFactory>();

        foreach (Type module in this.ModuleClassTypes)
        {
          // create cached factories for the classes...
          if (!this.ModuleClassFactories.ContainsKey(module))
          {
            this.ModuleClassFactories.Add(module, new YafFactory(module));
          }
        }
      }
    }

    #endregion

    /// <summary>
    /// The factory.
    /// </summary>
    protected class YafFactory
    {
      #region Constants and Fields

      /// <summary>
      /// The generic factory.
      /// </summary>
      private YafGenericFactoryBase _yafGenericFactory;

      #endregion

      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="YafFactory"/> class.
      /// </summary>
      /// <param name="t">
      /// The t.
      /// </param>
      public YafFactory(Type t)
      {
        Type initialisedFactoryType = genericFactoryType.MakeGenericType(new[] { typeof(T), t });
        this._yafGenericFactory = (YafGenericFactoryBase)Activator.CreateInstance(initialisedFactoryType);
      }

      #endregion

      #region Public Methods

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

      #endregion
    }

    /// <summary>
    /// The generic factory.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    private class YafGenericFactory<T> : YafGenericFactoryBase
      where T : class, new()
    {
      #region Public Methods

      /// <summary>
      /// The create.
      /// </summary>
      /// <returns>
      /// </returns>
      public T Create()
      {
        return new T();
      }

      /// <summary>
      /// The create object.
      /// </summary>
      /// <returns>
      /// The create object.
      /// </returns>
      public override object CreateObject()
      {
        return this.Create();
      }

      #endregion
    }

    /// <summary>
    /// The generic factory base.
    /// </summary>
    private abstract class YafGenericFactoryBase
    {
      #region Public Methods

      /// <summary>
      /// The create object.
      /// </summary>
      /// <returns>
      /// The create object.
      /// </returns>
      public abstract object CreateObject();

      #endregion
    }
  }
}