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
                //if (p.carsize > 0)
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

        public int calNodeCount() {
            return calNodeCount(_rootNode);
        }
        public int calNodeCount(TreeItemBase itm) {
            int _icount = 1;
            if(itm.Members!=null)
            for (int i = 0; i < itm.Members.Count; i++) {
                TreeItemBase subitm = itm.Members[i];
                _icount += calNodeCount(subitm);
            }
            return _icount;
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
                /*
                TreeViewItem tvi2 = ContainerFromItem(tv.ItemContainerGenerator, car);
                if (tvi2 != null) 
                {
                    tvi2.BringIntoView();
                    tvi2.IsSelected = true;
                }
                */                
                if (tviDic.ContainsKey(car.id))
                {
                    TreeViewItem tvi = tviDic[car.id];
                    tvi.BringIntoView();
                    tvi.IsSelected = true;
                }
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
        ///  递归扫描TREEVIEW节点
        /// </summary>
        /// <param name="icg"></param>
        /// <param name="expand"></param>
        public void scanTreeViewItems(ItemContainerGenerator icg, bool isExpanded ,bool isGoon)
        {
            if (icg == null) return;
            foreach (object item in icg.Items)
            {
                var tvi = icg.ContainerFromItem(item) as TreeViewItem;
                if (tvi == null) continue;
                var tib = item as TreeItemBase;
                if (tib == null || tib.id == null) continue;
                tviDic.TryAdd(tib.id, tvi);

                if (item.GetType() == typeof(Cust))
                {
                    Cust cust = (Cust)item;
                    if (cust.carsize == 0)
                        continue;
                    tvi.IsExpanded = isExpanded;
                    tvi.UpdateLayout();
                    
                    if(isGoon)//是否递归
                        scanTreeViewItems(tvi.ItemContainerGenerator, isExpanded,isGoon);
                }
            }
        }

        private string _kwvid;
        public string kwvid { get => _kwvid;
            set {
                _kwvid = value;
                CmdFilterCars();
            }
        }
        public List<Car> FilterCars { get; set; }

        public void CmdFilterCars() {
            FilterCars = Tool.pack.data.cars.Where(p =>{
                return p.vid.Contains(kwvid); 
            }).OrderBy(p => p.vid).Take(10).ToList();
            OnPropertyChanged("FilterCars");
        }

    }
}
