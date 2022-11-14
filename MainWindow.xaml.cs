using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCef
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyBrowser.Loaded += MyBrowser_Loaded;
        }

        private void MyBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            MyBrowser.ExecuteScriptAsync("alert('123')");
            //throw new NotImplementedException();
        }

        private void BTNGO_Click(object sender, RoutedEventArgs e)
        {
           BTNGO.IsEnabled = false;
           Task< LoadUrlAsyncResponse> t1 =  MyBrowser.LoadUrlAsync(TBURL.Text);
           t1.ContinueWith(t => {
               MyBrowser.ExecuteScriptAsync("alert('123')");
               BTNGO.IsEnabled = true;
           });
        }
    }
}
