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
namespace YAF.Types.Interfaces.Extensions
{
    #region Using

    using System.Collections.Generic;

    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The i service locator extensions.
    /// </summary>
    public static class IServiceLocatorExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IServiceLocator serviceLocator)
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");

            return (TService)serviceLocator.Get(typeof(TService));
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator. 
        /// </param>
        /// <param name="named">
        /// The named. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IServiceLocator serviceLocator, [NotNull] string named)
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");
            CodeContracts.ArgumentNotNull(named, "named");

            return (TService)serviceLocator.Get(typeof(TService), named);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator. 
        /// </param>
        /// <param name="parameters">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IServiceLocator serviceLocator, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");
            CodeContracts.ArgumentNotNull(parameters, "parameters");

            return (TService)serviceLocator.Get(typeof(TService), parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator. 
        /// </param>
        /// <param name="named">
        /// The named. 
        /// </param>
        /// <param name="parameters">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IServiceLocator serviceLocator, [NotNull] string named, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");
            CodeContracts.ArgumentNotNull(named, "named");
            CodeContracts.ArgumentNotNull(parameters, "parameters");

            return (TService)serviceLocator.Get(typeof(TService), named, parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IHaveServiceLocator haveLocator)
        {
            CodeContracts.ArgumentNotNull(haveLocator, "haveLocator");

            return haveLocator.ServiceLocator.Get<TService>();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator. 
        /// </param>
        /// <param name="named">
        /// The named. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IHaveServiceLocator haveLocator, [NotNull] string named)
        {
            CodeContracts.ArgumentNotNull(haveLocator, "haveLocator");
            CodeContracts.ArgumentNotNull(named, "named");

            return haveLocator.ServiceLocator.Get<TService>(named);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator. 
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IHaveServiceLocator haveLocator, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.ArgumentNotNull(haveLocator, "haveLocator");
            CodeContracts.ArgumentNotNull(parameters, "parameters");

            return haveLocator.ServiceLocator.Get<TService>(parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator. 
        /// </param>
        /// <param name="named">
        /// The named. 
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IHaveServiceLocator haveLocator, [NotNull] string named, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.ArgumentNotNull(haveLocator, "haveLocator");
            CodeContracts.ArgumentNotNull(named, "named");
            CodeContracts.ArgumentNotNull(parameters, "parameters");

            return haveLocator.ServiceLocator.Get<TService>(named, parameters);
        }

        /// <summary>
        /// The get repository.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRepository"/>.
        /// </returns>
        public static IRepository<T> GetRepository<T>([NotNull] this IHaveServiceLocator serviceLocator)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");

            return serviceLocator.Get<IRepository<T>>();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator. 
        /// </param>
        /// <param name="instance">
        /// The instance. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get. 
        /// </returns>
        public static bool TryGet<TService>([NotNull] this IServiceLocator serviceLocator, out TService instance)
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");

            object tempInstance;

            instance = default(TService);

            if (serviceLocator.TryGet(typeof(TService), out tempInstance))
            {
                instance = (TService)tempInstance;

                return true;
            }

            return false;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator. 
        /// </param>
        /// <param name="named">
        /// The named. 
        /// </param>
        /// <param name="instance">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get. 
        /// </returns>
        public static bool TryGet<TService>(
            [NotNull] this IServiceLocator serviceLocator, [NotNull] string named, out TService instance)
        {
            CodeContracts.ArgumentNotNull(serviceLocator, "serviceLocator");
            CodeContracts.ArgumentNotNull(named, "named");

            object tempInstance;

            instance = default(TService);

            if (serviceLocator.TryGet(typeof(TService), named, out tempInstance))
            {
                instance = (TService)tempInstance;

                return true;
            }

            return false;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator. 
        /// </param>
        /// <param name="instance">
        /// The instance. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get. 
        /// </returns>
        public static bool TryGet<TService>([NotNull] this IHaveServiceLocator haveLocator, out TService instance)
        {
            CodeContracts.ArgumentNotNull(haveLocator, "haveLocator");

            return haveLocator.ServiceLocator.TryGet(out instance);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator. 
        /// </param>
        /// <param name="named">
        /// The named. 
        /// </param>
        /// <param name="instance">
        /// The instance. 
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get. 
        /// </returns>
        public static bool TryGet<TService>(
            [NotNull] this IHaveServiceLocator haveLocator, [NotNull] string named, out TService instance)
        {
            CodeContracts.ArgumentNotNull(haveLocator, "haveLocator");
            CodeContracts.ArgumentNotNull(named, "named");

            return haveLocator.ServiceLocator.TryGet(named, out instance);
        }

        #endregion
    }
}