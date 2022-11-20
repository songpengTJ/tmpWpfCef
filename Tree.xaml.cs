using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
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
            this.DataContext = treevm;

            this.Loaded += Tree_Loaded;
            
        }

        private void Tree_Loaded(object sender, RoutedEventArgs e)
        {
            c(this.trvFamilies.ItemContainerGenerator, true);
        }

        private void CollapseAll(ItemContainerGenerator containerGenerator, bool IsGoon, bool IsExpanded)
        {
            foreach (object childItem in containerGenerator.Items)
            {
                TreeViewItem tvi = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (tvi == null) continue;
                tvi.IsExpanded = IsExpanded;
                tvi.IsSelected = IsExpanded;
                if (IsGoon)
                    CollapseAll(tvi.ItemContainerGenerator, IsGoon, IsExpanded);
            }
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }


        private void ExpandTree() {
            foreach (object item in this.trvFamilies.Items)
            {
                TreeViewItem treeItem = this.trvFamilies.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    ExpandAll(treeItem, true);
                //treeItem.IsExpanded = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            c(this.trvFamilies.ItemContainerGenerator,true);
            MessageBox.Show("123456");
        }

        private void c(ItemContainerGenerator icg, bool expand) { 
            if(icg == null) return;   
            foreach (object item in icg.Items)
            {
                TreeViewItem tvi = icg.ContainerFromItem(item) as TreeViewItem;
                if (tvi == null) continue;
                TreeItemBase tib = item as TreeItemBase;
                treevm.tviDic.TryAdd(tib.id, tvi);

                if (item.GetType() == typeof(Cust))
                {
                    Cust cust = (Cust)item;
                    if (cust.carsize == 0) 
                        continue;
                    tvi.IsExpanded = true;
                    tvi.UpdateLayout();
                    c(tvi.ItemContainerGenerator, expand);
                }
            }
        }

    }
}
