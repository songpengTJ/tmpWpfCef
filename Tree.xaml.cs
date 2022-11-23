using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfCef.Dialogs;
using WpfCef.Models;

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
            scanTreeViewItems(trvFamilies.ItemContainerGenerator, false, true);
            scanTreeViewItems(trvFamilies.ItemContainerGenerator, true, false);
        }
        private void Cb_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            cmbvidlist.IsDropDownOpen = true;
        }

        private void scanTreeViewItems(ItemContainerGenerator icg, bool isExpanded, bool isGoon)
        {
            if (icg == null) return;
            foreach (object item in icg.Items)
            {
                var tvi = icg.ContainerFromItem(item) as TreeViewItem;
                if (tvi == null) continue;
                if (item.GetType() == typeof(Cust))
                {
                    Cust cust = (Cust)item;
                    if (cust.carsize == 0)
                        continue;
                    tvi.IsExpanded = isExpanded;
                    tvi.UpdateLayout();
                    if (isGoon)//是否递归
                        scanTreeViewItems(tvi.ItemContainerGenerator, isExpanded, isGoon);
                }
            }
        }
    
    }
}
