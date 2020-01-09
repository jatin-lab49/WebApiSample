// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Gary Ewan Park">
//   Copyright (c) Gary Ewan Park, 2014, All rights reserved.
// </copyright>
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Web.Http;

using Autofac.Integration.WebApi;
using Gep13.Sample.Api.Mappers;
using Gep13.Sample.Common;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Gep13.Sample.Api.Startup))]

namespace Gep13.Sample.Api
{
    /// <summary>
    /// Startup Class used to initiate the startup of the Web API
    /// </summary>
    public partial class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var config = new HttpConfiguration();
            config.EnableCors();

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //// config.Filters.Add(new AuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var containerBuilder = AutofacBootstrapper.Configure();

            containerBuilder.RegisterApiControllers(Assembly.Load("Gep13.Sample.Api"));

            var container = containerBuilder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            AutoMapperConfiguration.Configure();

            app.UseAutofacMiddleware(container);

            app.UseWebApi(config);
        }
    }
}