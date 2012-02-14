/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Web;

  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The app init task manager.
  /// </summary>
  [ExportService(ServiceLifetimeScope.Singleton)]
  public class AppInitTaskManager : BaseTaskModuleManager, IHandleEvent<HttpApplicationInitEvent>, IHaveServiceLocator
  {
    #region Constants and Fields

    /// <summary>
    ///   The _app instance.
    /// </summary>
    private HttpApplication _appInstance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInitTaskManager"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public AppInitTaskManager([NotNull] IServiceLocator serviceLocator, [NotNull] ILogger logger)
    {
      this.ServiceLocator = serviceLocator;
      this.Logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order
    {
      get
      {
        return 5;
      }
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start a non-running task -- will set the <see cref="HttpApplication"/> instance.
    /// </summary>
    /// <param name="instanceName">
    /// Unique name of this task
    /// </param>
    /// <param name="start">
    /// Task to run
    /// </param>
    public override void StartTask([NotNull] string instanceName, [NotNull] IBackgroundTask start)
    {
      CodeContracts.ArgumentNotNull(instanceName, "instanceName");
      CodeContracts.ArgumentNotNull(start, "start");

      if (this._appInstance == null)
      {
        return;
      }

      // add and start this module...
      if (!start.IsRunning && !this.TaskExists(instanceName))
      {
        this.Logger.Debug("Starting Task {0} Under User {1}...".FormatWith(instanceName, Environment.UserName));

        // setup and run...
        this.Get<IInjectServices>().Inject(start);
        start.Run();

        // add it after so that IsRunning is set first...
        this.AddTask(instanceName, start);
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<HttpApplicationInitEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] HttpApplicationInitEvent @event)
    {
      this._appInstance = @event.HttpApplication;

      // wire up provider so that the task module can be found...
      this.Get<CurrentTaskModuleProvider>().Instance = this;

      // create intermittent cleanup task...
      this.StartTask("CleanUpTask", new CleanUpTask { TaskManager = this });

      foreach (var instance in this.Get<IEnumerable<IStartTasks>>())
      {
        try
        {
          instance.Start(this);
        }
        catch (Exception ex)
        {
          this.Logger.Fatal(ex, "Failed to start: {0}".FormatWith(instance.GetType().Name));
        }
      }
    }

    #endregion

    #endregion
  }
}