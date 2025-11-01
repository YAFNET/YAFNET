using System;
using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using YAF.Types.Extensions;

namespace YAF.Tests.Utils;

public class CustomStartupFilter(string ipAddress) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            if (ipAddress.IsNotSet())
            {
                next(app);
            }

            app.UseMiddleware<FakeRemoteIpAddressMiddleware>(ipAddress);

            next(app);
        };
    }
}

public class FakeRemoteIpAddressMiddleware(RequestDelegate next, string ipAddress)
{
    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);

        await next(httpContext);
    }
}