using System.Windows;
using WpfCef.Dialogs;

namespace WpfCef
{
    /// <summary>
    /// Tree.xaml 的交互逻辑
    /// </summary>
    public partial class Tree : Window
    {
        public Tree()
        {
            InitializeComponent();
            init();
        }

        private TreeVM treevm = null;
        private void init() {
            Tool.parseJson();

            treevm = new TreeVM();
            treevm.PageDG = this.DataGridPos;
            treevm.PageTV = this.trvFamilies;
            DataContext = treevm;

            Loaded += Tree_Loaded;
        }

        private void Tree_Loaded(object sender, RoutedEventArgs e)
        {
            DlgTrvScanVM vm = new DlgTrvScanVM();
            vm.DownloadTitle = "";
            vm.icg = trvFamilies.ItemContainerGenerator;
            vm.dic = treevm.tviDic;
            vm.iTotal = treevm.calNodeCount();

            DlgDownload dlg = new DlgDownload() { DataContext = vm };
            vm.Init();
            dlg.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            treevm.CmdFilterCars();
        }
    }
}
