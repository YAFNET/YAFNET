/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
	using System.Configuration;
	using System.Data.Common;
	using System.Linq;
	using System.Reflection;

	using Autofac;
	using Autofac.Core;

	using YAF.Classes;
	using YAF.Core.BBCode;
	using YAF.Core.Data;
	using YAF.Core.Nntp;
	using YAF.Core.Services;
	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The module for all singleton scoped items...
	/// </summary>
	public class YafBaseContainerModule : IModule, IHaveComponentRegistry
	{
		#region Properties

		/// <summary>
		///   Gets or sets ComponentRegistry.
		/// </summary>
		public IComponentRegistry ComponentRegistry { get; set; }

		/// <summary>
		///   Gets or sets ExtensionAssemblies.
		/// </summary>
		public IList<Assembly> ExtensionAssemblies { get; protected set; }

		#endregion

		#region Implemented Interfaces

		#region IModule

		/// <summary>
		/// Apply the module to the component registry.
		/// </summary>
		/// <param name="componentRegistry">
		/// Component registry to apply configuration to.
		/// </param>
		public void Configure([NotNull] IComponentRegistry componentRegistry)
		{
			CodeContracts.ArgumentNotNull(componentRegistry, "componentRegistry");

			this.ComponentRegistry = componentRegistry;

		    this.ExtensionAssemblies = new YafModuleScanner().GetModules("YAF*.dll").OrderByDescending(x => x.GetAssemblySortOrder()).ToList();

			// handle registration...
			this.RegisterExternalModules();

			// external first...
			this.RegisterDynamicServices(this.ExtensionAssemblies.Where(a => a != Assembly.GetExecutingAssembly()));

			// internal bindings next...
			this.RegisterDynamicServices(new[] { Assembly.GetExecutingAssembly() });

			this.RegisterBasicBindings();
			this.RegisterEventBindings();
		    this.RegisterMembershipProviders();
			this.RegisterServices();
			this.RegisterModules();
			this.RegisterPages();
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// The register basic bindings.
		/// </summary>
		private void RegisterBasicBindings()
		{
			var builder = new ContainerBuilder();

			builder.Register(x => this.ExtensionAssemblies).Named<IList<Assembly>>("ExtensionAssemblies").SingleInstance();

			builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>().InstancePerLifetimeScope();

			RegisterDataBindings(builder);

		    // system
			builder.RegisterType<LocalHostedFileSystem>().As<IFileSystem>().InstancePerLifetimeScope().PreserveExistingDefaults();

			// Http Application Base
			builder.RegisterType<CurrentHttpApplicationStateBaseProvider>().SingleInstance().PreserveExistingDefaults();
			builder.Register(k => k.Resolve<CurrentHttpApplicationStateBaseProvider>().Instance).ExternallyOwned().PreserveExistingDefaults();

			// Task Module
			builder.RegisterType<CurrentTaskModuleProvider>().SingleInstance().PreserveExistingDefaults();
			builder.Register(k => k.Resolve<CurrentTaskModuleProvider>().Instance).ExternallyOwned().PreserveExistingDefaults();

			builder.RegisterType<YafNntp>().As<INewsreader>().InstancePerLifetimeScope().PreserveExistingDefaults();

			// optional defaults.
			builder.RegisterType<YafSendMail>().As<ISendMail>().SingleInstance().PreserveExistingDefaults();

			builder.RegisterType<YafSendNotification>().As<ISendNotification>().InstancePerLifetimeScope().PreserveExistingDefaults();

			builder.RegisterType<YafDigest>().As<IDigest>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<DefaultUserDisplayName>().As<IUserDisplayName>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<DefaultUrlBuilder>().As<IUrlBuilder>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafBBCode>().As<IBBCode>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafFormatMessage>().As<IFormatMessage>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafDBBroker>().As<IDBBroker>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafAvatars>().As<IAvatars>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<CurrentBoardId>().As<IHaveBoardId>().InstancePerLifetimeScope().PreserveExistingDefaults();
		    builder.RegisterType<YafReadTrackCurrentUser>().As<IReadTrackCurrentUser>().InstancePerYafContext().PreserveExistingDefaults();

			// cache bindings.
			builder.RegisterType<StaticLockObject>().As<IHaveLockObject>().SingleInstance().PreserveExistingDefaults();
		    builder.RegisterType<HttpRuntimeCache>().As<IDataCache>().SingleInstance().PreserveExistingDefaults();

			// Shared object store -- used for objects local only
		    builder.RegisterType<HttpRuntimeCache>().As<IObjectStore>().SingleInstance().PreserveExistingDefaults();

			builder.RegisterType<YafSession>().As<IYafSession>().InstancePerLifetimeScope().PreserveExistingDefaults();
		    builder.RegisterType<YafBadWordReplace>().As<IBadWordReplace>().SingleInstance().PreserveExistingDefaults();

			builder.RegisterType<YafPermissions>().As<IPermissions>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafDateTime>().As<IDateTime>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafFavoriteTopic>().As<IFavoriteTopic>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafUserIgnored>().As<IUserIgnored>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.RegisterType<YafBuddy>().As<IBuddy>().InstancePerLifetimeScope().PreserveExistingDefaults();

			// needs to be "instance per dependancy" so that each new request gets a new ScripBuilder.
			builder.RegisterType<JavaScriptBuilder>().As<IScriptBuilder>().InstancePerDependency().PreserveExistingDefaults();

			// builder.RegisterType<RewriteUrlBuilder>().Named<IUrlBuilder>("rewriter").InstancePerLifetimeScope();
			builder.RegisterType<YafStopWatch>().As<IStopWatch>().InstancePerMatchingLifetimeScope(YafLifetimeScope.Context).PreserveExistingDefaults();

			// localization registration...
			builder.RegisterType<LocalizationProvider>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.Register(k => k.Resolve<LocalizationProvider>().Localization).PreserveExistingDefaults();

			// theme registration...
			builder.RegisterType<ThemeProvider>().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.Register(k => k.Resolve<ThemeProvider>().Theme).PreserveExistingDefaults();

			// replace rules registration...
			builder.RegisterType<ProcessReplaceRulesProvider>().AsSelf().As<IReadOnlyProvider<IProcessReplaceRules>>().InstancePerLifetimeScope().PreserveExistingDefaults();

			builder.Register((k, p) => k.Resolve<ProcessReplaceRulesProvider>(p).Instance).InstancePerLifetimeScope().PreserveExistingDefaults();

			// module resolution bindings...
			builder.RegisterGeneric(typeof(StandardModuleManager<>)).As(typeof(IModuleManager<>)).InstancePerLifetimeScope();

			// background emailing...
			builder.RegisterType<YafSendMailThreaded>().As<ISendMailThreaded>().SingleInstance().PreserveExistingDefaults();

			// board settings...
			builder.RegisterType<CurrentBoardSettings>().AsSelf().InstancePerMatchingLifetimeScope(YafLifetimeScope.Context).PreserveExistingDefaults();
			builder.Register(k => k.Resolve<CurrentBoardSettings>().Instance).ExternallyOwned().PreserveExistingDefaults();

			this.UpdateRegistry(builder);
		}

	    private static void RegisterDataBindings(ContainerBuilder builder)
	    {
	        // data
	        builder.RegisterType<DbAccessProvider>().As<IDbAccessProvider>().SingleInstance();
	        builder.Register(c => c.Resolve<IDbAccessProvider>().Instance).As<IDbAccess>().InstancePerDependency().PreserveExistingDefaults();
	        builder.Register((c, p) => DbProviderFactories.GetFactory(p.TypedAs<string>())).ExternallyOwned().PreserveExistingDefaults();
	        builder.RegisterType<DynamicDbFunction>().As<IDbFunction>().InstancePerLifetimeScope().PreserveExistingDefaults();
	    }

	    /// <summary>
		/// Register event bindings
		/// </summary>
		private void RegisterEventBindings()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>().InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(FireEvent<>)).As(typeof(IFireEvent<>)).InstancePerLifetimeScope();

			//// scan assemblies for events to wire up...
			//builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray()).AsClosedTypesOf(typeof(IHandleEvent<>)).
			//  AsImplementedInterfaces().InstancePerLifetimeScope();

			this.UpdateRegistry(builder);
		}

		/// <summary>
		/// The register external modules.
		/// </summary>
		private void RegisterExternalModules()
		{
			var builder = new ContainerBuilder();

		    var modules = this.ExtensionAssemblies
                .Where(a => a != Assembly.GetExecutingAssembly())
                .FindModules<IModule>()
                .Select(m => Activator.CreateInstance(m) as IModule);

			modules.ForEach(m => builder.RegisterModule(m));

			this.UpdateRegistry(builder);
		}

		/// <summary>
		/// The register services.
		/// </summary>
		/// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
		private void RegisterDynamicServices([NotNull] IEnumerable<Assembly> assemblies)
		{
			CodeContracts.ArgumentNotNull(assemblies, "assemblies");

			var builder = new ContainerBuilder();
			var classes = assemblies.FindClassesWithAttribute<ExportServiceAttribute>();
			var exclude = new List<Type> { typeof(IDisposable), typeof(IHaveServiceLocator), typeof(IHaveLocalization) };

			foreach (var c in classes)
			{
				var built = builder.RegisterType(c).As(c);

				var exportAttribute = c.GetAttribute<ExportServiceAttribute>();

				if (exportAttribute != null && exportAttribute.RegisterSpecifiedTypes != null && exportAttribute.RegisterSpecifiedTypes.Length > 0)
				{
					// only register types provided...
					foreach (var regType in exportAttribute.RegisterSpecifiedTypes)
					{
						built.As(regType);
					}
				}
				else
				{
					// register all associated interfaces including inheritated interfaces!
					foreach (var regType in c.GetInterfaces().Where(i => !exclude.Contains(i)))
					{
						built.As(regType);
					}
				}

				if (exportAttribute != null && built != null)
				{
					if (exportAttribute.Named.IsSet())
					{
						built = built.Named(exportAttribute.Named, c.GetType());
					}

					switch (exportAttribute.ServiceLifetimeScope)
					{
						case ServiceLifetimeScope.Singleton:
							built.SingleInstance();
							break;

						case ServiceLifetimeScope.Transient:
							built.ExternallyOwned();
							break;

						case ServiceLifetimeScope.OwnedByContainer:
							built.OwnedByLifetimeScope();
							break;

						case ServiceLifetimeScope.InstancePerScope:
							built.InstancePerLifetimeScope();
							break;

						case ServiceLifetimeScope.InstancePerDependancy:
							built.InstancePerDependency();
							break;

						case ServiceLifetimeScope.InstancePerContext:
							built.InstancePerMatchingLifetimeScope(YafLifetimeScope.Context);
							break;
					}

					built.PreserveExistingDefaults();
				}
			}

			this.UpdateRegistry(builder);
		}

		/// <summary>
		/// Register identity providers
		/// </summary>
		private void RegisterMembershipProviders()
		{
			var builder = new ContainerBuilder();

			// membership
			builder.RegisterType<CurrentMembershipProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
		    builder.Register(x => x.Resolve<CurrentMembershipProvider>().Instance).ExternallyOwned().PreserveExistingDefaults();

			// roles
			builder.RegisterType<CurrentRoleProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.Register(x => x.Resolve<CurrentRoleProvider>().Instance).ExternallyOwned().PreserveExistingDefaults();

			// profiles
			builder.RegisterType<CurrentProfileProvider>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();
			builder.Register(x => x.Resolve<CurrentProfileProvider>().Instance).ExternallyOwned().PreserveExistingDefaults();

			this.UpdateRegistry(builder);
		}

		/// <summary>
		/// The register modules.
		/// </summary>
		private void RegisterModules()
		{
			var builder = new ContainerBuilder();

			// forum modules...
			builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray())
                .AssignableTo<IBaseForumModule>()
                .As<IBaseForumModule>()
                .InstancePerLifetimeScope();

			// editor modules...
			builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray())
                .AssignableTo<ForumEditor>()
                .As<ForumEditor>()
                .InstancePerLifetimeScope();

			this.UpdateRegistry(builder);
		}

		/// <summary>
		/// The register pages
		/// </summary>
		private void RegisterPages()
		{
			var builder = new ContainerBuilder();

			builder.RegisterAssemblyTypes(this.ExtensionAssemblies.ToArray())
                .AssignableTo<ILocatablePage>()
                .AsImplementedInterfaces()
                .SingleInstance();

			this.UpdateRegistry(builder);
		}

		/// <summary>
		/// The register services.
		/// </summary>
		private void RegisterServices()
		{
			var builder = new ContainerBuilder();

			builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AssignableTo<IStartupService>()
                .As<IStartupService>()
                .InstancePerLifetimeScope();

		    builder.Register(x => x.Resolve<IEnumerable<IStartupService>>()
                .FirstOrDefault(t => t is StartupInitializeDb) as StartupInitializeDb)
                .InstancePerLifetimeScope();

			this.UpdateRegistry(builder);
		}

		#endregion
	}
}