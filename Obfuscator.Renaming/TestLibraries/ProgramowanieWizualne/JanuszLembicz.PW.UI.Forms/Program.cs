#region

using System;
using System.Configuration;
using System.Windows.Forms;
using JanuszLembicz.PW.Properties;
using JanuszLembicz.Utils;

#endregion

namespace JanuszLembicz.PW
{
    internal static class Program
    {
        static Program()
        {
            ObjectFactory.GetInstance.AssemblyName = Settings.Default.DAOFactoryDll;
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //if (!config.ConnectionStrings.SectionInformation.IsProtected)
            //{
            //    config.ConnectionStrings.SectionInformation.ProtectSection(null);
            //    config.ConnectionStrings.SectionInformation.ForceSave = true;
            //    config.Save(ConfigurationSaveMode.Modified);
            //}
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}