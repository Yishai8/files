using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows;
using Client.ViewModels;

namespace Client
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e) //override base implemantion of BootstrapperBase.OnStartup();
        {
            DisplayRootViewFor<MainViewModel>(); 
        }
    }
}
