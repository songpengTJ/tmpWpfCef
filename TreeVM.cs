using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfCef.Models;

namespace WpfCef
{
    public class TreeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TreeVM() { init(); }

        private TreeItemBase _rootNode;
        private ObservableCollection<TreeItemBase> _listTreeNode = new ObservableCollection<TreeItemBase>();
        private void init() {

            Tool.pack.data.cust = Tool.pack.data.cust.OrderBy(s => s.custcode).ToList();
            _rootNode = Tool.pack.data.cust.First();
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
                if (p.carsize > 0)
                    dic[parentid].Members.Add(p);
            });

            Tool.pack.data.cars.ForEach(delegate (Car p) {
                string parentid = p.custcode;
                if (!dic.ContainsKey(parentid))
                    return;
                dic[parentid].Members.Add(p);
            });

            _listTreeNode.Add(_rootNode);

            Tool.pack.data.cars.ForEach(p => {
                p.PropertyChanged += delegate (object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    if (!e.PropertyName.Equals("IsSelected")) 
                        return;
                    OnPropertyChanged("ListCars");
                };
            });
        }

        public ObservableCollection<TreeItemBase> listTreeNodes
        {
            get => _listTreeNode;
        }
        public List<Car> ListCars 
        { 
            get => Tool.pack.data.cars.Where(p => p.IsSelected  ).ToList(); 
        }

        private TreeItemBase _SelectedTreeItem;
        public  TreeItemBase  SelectedTreeItem { get=> _SelectedTreeItem;
            set {
                if(value == null) return;
                if(value == _SelectedTreeItem) 
                    return;
                _SelectedTreeItem = value;
                if (_SelectedTreeItem.GetType() == typeof(Car)) 
                {
                    Car car = (Car)_SelectedTreeItem;
                    CarSelected(car, false, true);
                }
            } 
        }

        private Car _SelectedRTGridCar;
        public  Car  SelectedRTGridCar{ 
            get=> _SelectedRTGridCar;
            set {
                if (value == null) return;
                if (_SelectedRTGridCar!= null &&
                    _SelectedRTGridCar.id == value.id)
                    return;
                _SelectedRTGridCar = value;
                CarSelected(value, true, false);
            }
        }

        public Dictionary<string, TreeViewItem> tviDic = new Dictionary<string, TreeViewItem>();
        public TreeView PageTV { get; set; }
        public DataGrid PageDG { get; set; }
        private TreeViewItem ContainerFromItem(ItemContainerGenerator containerGenerator, object item)
        {
            TreeViewItem container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
            if (container != null)
                return container;

            foreach (object childItem in containerGenerator.Items)
            {
                TreeViewItem parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (parent == null)
                    continue;

                container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null)
                    return container;

                container = ContainerFromItem(parent.ItemContainerGenerator, item);
                if (container != null)
                    return container;
            }
            return null;
        }

        private void CarSelected(Car car
            , bool bTV
            , bool bGRID)
        { 
            CarSelected(car, bTV, bGRID, PageTV, PageDG);
        }
        private void CarSelected(Car car
            , bool bTV
            , bool bGRID
            , TreeView tv 
            , DataGrid datagrid 
            )
        {
            // TreeView
            if (bTV)
            {
                TreeViewItem tvi2 = ContainerFromItem(tv.ItemContainerGenerator, car);
                if (tvi2 != null) 
                {
                    tvi2.BringIntoView();
                    tvi2.IsSelected = true;
                }
                /*
                if (tviDic.ContainsKey(car.id))
                {
                    TreeViewItem tvi = tviDic[car.id];
                    tvi.BringIntoView();
                    tvi.IsSelected = true;
                }
                */
            }
            // GridPos
            if (bGRID && car.IsSelected)
            {
                if (datagrid.ItemContainerGenerator.Items.Contains(car))
                {
                    datagrid.SelectedItem = car;
                    datagrid.ScrollIntoView(car);
                    datagrid.UpdateLayout();
                }
            }

            if (tv is null)
            {
                throw new ArgumentNullException(nameof(tv));
            }
        }


        /// <summary>
        /// Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// </returns>
        private TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container != null)
            {
                if (container.DataContext == item)
                {
                    return container as TreeViewItem;
                }

                // Expand the current container
                if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                {
                    container.SetValue(TreeViewItem.IsExpandedProperty, true);
                }

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the
                // virtualizing case even if the item is marked
                // expanded we still need to do this step in order to
                // regenerate the visuals because they may have been virtualized away.

                container.ApplyTemplate();
                ItemsPresenter itemsPresenter =
                    (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter,
                    // so walk the descendents and find the child.
                    itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    }
                }

                Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

                // Ensure that the generator for this panel has been created.
                UIElementCollection children = itemsHostPanel.Children;

                MyVirtualizingStackPanel virtualizingPanel =
                    itemsHostPanel as MyVirtualizingStackPanel;

                for (int i = 0, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (virtualizingPanel != null)
                    {
                        // Bring the item into view so
                        // that the container will be generated.
                        virtualizingPanel.BringIntoView(i);

                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);

                        // Bring the item into view to maintain the
                        // same behavior as with a virtualizing panel.
                        subContainer.BringIntoView();
                    }

                    if (subContainer != null)
                    {
                        // Search the next level for the object.
                        TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
                        if (resultContainer != null)
                        {
                            return resultContainer;
                        }
                        else
                        {
                            // The object is not under this TreeViewItem
                            // so collapse it.
                            subContainer.IsExpanded = false;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        private T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }

    }
}
