using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCef.Models;

namespace WpfCef.Dialogs
{
    public class DlgTrvScanVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #region UI Elements
        public string ret { get; set; }
        public string DownloadTitle { get; set; }
        public int ProgressValue { get; set; }
        #endregion

        public void Init()
        {
            OnPropertyChanged("DownloadTitle");
            OnPropertyChanged("ProgressValue");

            _handle.ContinueWith(pre => scan());
        }

        private Task _handle = Task.CompletedTask;
        private TimeSpan _handleTimeout = TimeSpan.FromMilliseconds(1);

        public int iTotal { get; set; } = 1;
        public int iCurrent { get; set; } = 0;
        public ItemContainerGenerator icg { get; set; }
        public Dictionary<string, TreeViewItem> dic { get; set; }

        private async Task scan() {
            await scanTreeViewItems(icg, true, true, dic);
            await scanTreeViewItems(icg, false, true, dic);
            await scanTreeViewItems(icg, true, false, dic);

            ret = "123";
            OnPropertyChanged("ret");
        }

        public async Task scanTreeViewItems(ItemContainerGenerator icg, bool isExpanded, bool isGoon , Dictionary<string, TreeViewItem> tviDic)
        {
            if (icg == null) return;
            foreach (object item in icg.Items)
            {
                var tvi = icg.ContainerFromItem(item) as TreeViewItem;
                if (tvi == null) continue;
                var tib = item as TreeItemBase;
                if (tib == null || tib.id == null) continue;
                bool isOK = tviDic.TryAdd(tib.id, tvi);
                if (isOK) iCurrent++;
                if (item.GetType() == typeof(Cust))
                {
                    if (isOK)
                    {
                        System.Diagnostics.Debug.Print(iCurrent + ">>>>>>>>>>>>>>>>");
                        ProgressValue = iCurrent * 100 / iTotal;
                        DownloadTitle = "数据预加载：" + iCurrent + " / " + iTotal;
                        OnPropertyChanged("ProgressValue");
                        OnPropertyChanged("DownloadTitle");
                        await Task.Delay(_handleTimeout).ConfigureAwait(true);
                    }

                    Cust cust = (Cust)item;
                    if (cust.carsize == 0)
                        continue;
                    
                    Application.Current.Dispatcher.Invoke( delegate(){
                        tvi.IsExpanded = isExpanded;
                        //tvi.ExpandSubtree();
                        tvi.UpdateLayout();
                    });

                    if (isGoon)//是否递归
                        await scanTreeViewItems(tvi.ItemContainerGenerator, isExpanded, isGoon , tviDic);
                }
            }
            System.Diagnostics.Debug.Print(iCurrent + ">>>>>>>>>>>>>>>>");
        }


    }
}
