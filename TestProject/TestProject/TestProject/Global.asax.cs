using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TestProject.Repositories;
using TestProject.Formatters;

namespace TestProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static FileSystem fileSystem { get; private set; }

        protected void Application_Start()
        {
            
            string rootOnDrive = "C:\\TestProject";
                             
            string envRoot = Environment.GetEnvironmentVariable("TEST_PROJECT_ROOT");
            if (envRoot != null)
            {
                rootOnDrive = envRoot;
            }

            fileSystem = new FileSystemOnDrive(rootOnDrive);
            fileSystem.Open();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Formatters.Insert(0, new StreamMediaTypeFormatter());
        }

        protected void Application_End()
        {
            fileSystem.Close();
        }
    }
}
