#region

using System;
using System.Web;
using JanuszLembicz.PW.Properties;
using JanuszLembicz.Utils;

#endregion

namespace JanuszLembicz.PW.UI.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ObjectFactory.GetInstance.AssemblyName = Settings.Default.DAOFactoryDll;
        }

        protected void Session_Start(object sender, EventArgs e) {}

        protected void Application_BeginRequest(object sender, EventArgs e) {}

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {}

        protected void Application_Error(object sender, EventArgs e) {}

        protected void Session_End(object sender, EventArgs e) {}

        protected void Application_End(object sender, EventArgs e) {}
    }
}