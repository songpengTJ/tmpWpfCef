using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private void init() {
            Tool.parseJson();
            Tool.pack.data.cust = Tool.pack.data.cust.OrderBy(s => s.custcode).ToList();
            TreeItemBase rootNode = Tool.pack.data.cust.First();
            var dic = new Dictionary<string, TreeItemBase>();
            Tool.pack.data.cust.ForEach(delegate (Cust p) {
                string cc = p.custcode;
                dic[cc] = p;
                p.carsize = Tool.pack.data.cars.FindAll(p => { return p.custcode.StartsWith(cc); }).Count();
            });

            Tool.pack.data.cust.ForEach(delegate (Cust p) {
                string parentid = p.qryParentId();
                if (!dic.ContainsKey(parentid)) 
                    return;
                dic[parentid].Members.Add(p);
            });

            Tool.pack.data.cars.ForEach(delegate (Car p) {
                string parentid = p.custcode;
                if (!dic.ContainsKey(parentid))
                    return;
                dic[parentid].Members.Add(p);
            });

            ObservableCollection<TreeItemBase> list = new ObservableCollection<TreeItemBase>();
            list.Add(rootNode);
            trvFamilies.ItemsSource = list;


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

    }
}
