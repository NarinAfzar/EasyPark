[assembly: WebActivator.PostApplicationStartMethod(typeof(EasyPark.WebUI.App_Start.SimpleInjectorInitializer), "Initialize")]

namespace EasyPark.WebUI.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    using SimpleInjector;
    using SimpleInjector.Extensions;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;
    using DAL.Abstract;
    using DAL.Concrete;
    using Microsoft.AspNet.Identity;
    using DAL.Entities;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.Owin;
    using System.Web;

    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            
            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            //container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
     
        private static void InitializeContainer(Container container)
        {
            container.Register<IDbContext, EFDbContext>(Lifestyle.Scoped);
            container.Register(typeof(IRepository<>), typeof(EFRepository<>), Lifestyle.Scoped);
            container.Register<IUserStore<ApplicationUser>>(() =>
                new UserStore<ApplicationUser>(container.GetInstance<IDbContext>() as DbContext),
                Lifestyle.Scoped);
            container.Register<ApplicationUserManager>(Lifestyle.Scoped);
            container.Register(() =>
                new ApplicationSignInManager(container.GetInstance<ApplicationUserManager>(),
                HttpContext.Current.GetOwinContext().Authentication));
        }
    }
}