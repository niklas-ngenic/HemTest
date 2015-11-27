using ArtistInfoRepository;
using Autofac;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Autofac.Integration.WebApi;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using System.Web.Http.ExceptionHandling;

namespace ArtistInfoApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            app.AddNLogExceptionLogger(config);
            app.SetupRoutes(config);
            app.SetupDependencies(config);
            app.SetCamelCaseJson(config);
            app.SetJsonToIgnoreNullValues(config);
            app.AddLogRequestResponse();
            app.UseWebApi(config);
        }
    }

    public static class SetupDependenciesExtension
    {
        public static void SetupDependencies(this IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MusicBrainzClient>().As<IArtistInfoClient>();
            builder.RegisterType<CoverArtArchiveClient>().As<IAlbumCoverArtClient>();
            builder.RegisterType<WikipediaArtistInfoClient>().As<IArtistDescriptionClient>();
            builder.RegisterType<MusicBrainzDataAggregator>().As<IArtistInfoAggregator>();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
        }
    }

    public static class SetupRoutesExtension
    {
        public static void SetupRoutes(this IAppBuilder app, HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            SetRoutes(config);
        }

        public static void SetRoutes(HttpConfiguration config)
        {
            // Default fallback route
            config.Routes.MapHttpRoute(
                name: "DefaultRoute",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional }
                );
        }
    }

    public static class SetupServices
    {
        public static void AddNLogExceptionLogger(this IAppBuilder app, HttpConfiguration config)
        {
            config.Services.Add(typeof(IExceptionLogger), new NLogExceptionLogger());
        }
    }

    public static class SetupFormattersExtension
    {
        public static void SetCamelCaseJson(this IAppBuilder app, HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
        }

        public static void SetJsonToIgnoreNullValues(this IAppBuilder app, HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        }
    }
}