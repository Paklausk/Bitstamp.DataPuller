using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace Bitstamp.DataPuller
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;

            this.serviceInstaller1.ServicesDependedOn = Settings.Instance.ServiceDependencies.ToArray();
            this.serviceInstaller1.Description = "Windows based service for Bitstamp exchange price pulling.";
            this.serviceInstaller1.ServiceName = Settings.Instance.ServiceName;
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(new Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});

        }

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}
