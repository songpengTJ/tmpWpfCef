using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfCef.Dialogs
{
    /// <summary>
    /// DlgDownload.xaml 的交互逻辑
    /// </summary>
    public partial class DlgDownload : Window
    {
        public DlgDownload()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TBRet.Text)) 
                return;
            this.Close();
        }
    }
}
