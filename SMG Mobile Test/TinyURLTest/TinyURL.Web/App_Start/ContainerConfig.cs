using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using TinyURL.Data.Services;

namespace TinyURL.Web
{
    public class ContainerConfig
    {
        public static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<SqlUploadedImageData>()
                .As<IUploadedImage>()
                .InstancePerRequest();
            builder.RegisterType<TinyURLDbContext>().InstancePerRequest();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}