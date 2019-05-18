using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Configuration;
using ServiceStack.Messaging;
using ServiceStack.Redis;
using ServiceStack.Web;

namespace ServiceStack
{
    //implemented by PageBase and 
    public interface IHasServiceStackProvider
    {
        IServiceStackProvider ServiceStackProvider { get; }
    }

    public interface IServiceStackProvider : IDisposable
    {
        void SetResolver(IResolver resolver);
        IResolver GetResolver();
        IAppSettings AppSettings { get; }
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }
        ICacheClient Cache { get; }
        IDbConnection Db { get; }
        IRedisClient Redis { get; }
        IMessageProducer MessageProducer { get; }
        IAuthRepository AuthRepository { get; }
        ISessionFactory SessionFactory { get; }
        ISession SessionBag { get; }
        bool IsAuthenticated { get; }
        IAuthSession GetSession(bool reload = false);
        TUserSession SessionAs<TUserSession>();
        void ClearSession();
        T TryResolve<T>();
        T ResolveService<T>();

        IServiceGateway Gateway { get; }

        object Execute(IRequest request);

        [Obsolete("Use Gateway")]
        object Execute(object requestDto);

        [Obsolete("Use Gateway")]
        TResponse Execute<TResponse>(IReturn<TResponse> requestDto);

        [Obsolete("Use Gateway")]
        void PublishMessage<T>(T message);
    }

    //Add extra functionality common to ASP.NET ServiceStackPage or ServiceStackController
    public static class ServiceStackProviderExtensions
    {
        public static bool IsAuthorized(this IHasServiceStackProvider hasProvider, AuthenticateAttribute authAttr)
        {
            if (authAttr == null)
                return true;

            var authSession = hasProvider.ServiceStackProvider.GetSession();
            return authSession != null && authSession.IsAuthenticated;
        }

        public static bool HasAccess(
            this IHasServiceStackProvider hasProvider,
            ICollection<RequiredRoleAttribute> roleAttrs,
            ICollection<RequiresAnyRoleAttribute> anyRoleAttrs,
            ICollection<RequiredPermissionAttribute> permAttrs,
            ICollection<RequiresAnyPermissionAttribute> anyPermAttrs)
        {
            if (roleAttrs.Count + anyRoleAttrs.Count + permAttrs.Count + anyPermAttrs.Count == 0)
                return true;

            var authSession = hasProvider.ServiceStackProvider.GetSession();
            if (authSession == null || !authSession.IsAuthenticated)
                return false;

            var httpReq = hasProvider.ServiceStackProvider.Request;
            var userAuthRepo = HostContext.AppHost.GetAuthRepository(hasProvider.ServiceStackProvider.Request);
            using (userAuthRepo as IDisposable)
            {
                var hasRoles = roleAttrs.All(x => x.HasAllRoles(httpReq, authSession, userAuthRepo));
                if (!hasRoles)
                    return false;

                var hasAnyRole = anyRoleAttrs.All(x => x.HasAnyRoles(httpReq, authSession, userAuthRepo));
                if (!hasAnyRole)
                    return false;

                var hasPermssions = permAttrs.All(x => x.HasAllPermissions(httpReq, authSession, userAuthRepo));
                if (!hasPermssions)
                    return false;

                var hasAnyPermission = anyPermAttrs.All(x => x.HasAnyPermissions(httpReq, authSession, userAuthRepo));
                if (!hasAnyPermission)
                    return false;

                return true;
            }
        }
    }
}