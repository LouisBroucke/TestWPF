using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Models.Wenskaart wenskaart = new Models.Wenskaart();
            ViewModel.WenskaartVM viewmodel = new ViewModel.WenskaartVM(wenskaart);
            Views.WenskaartView view = new Views.WenskaartView();

            view.DataContext = viewmodel;
            view.Show();
        }
    }
}
