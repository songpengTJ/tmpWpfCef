using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace WpfCef.Models
{
    public class TreeItemBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TreeItemBase()
        {
            Members = new ObservableCollection<TreeItemBase>();
        }
        public virtual string Title { get; set; } = "Unknown";
        public virtual string ImageName { get; set; } = "";
        public virtual int type { get; set; }
        public virtual string id { get; set; } = "";

        public ObservableCollection<TreeItemBase> Members { get; set; }

        private bool _IsSelected = true;
        public bool IsSelected
        {
            get => _IsSelected;
            set => Da(value);
        }
        protected void Da(bool b)
        {
            bool bInvoke = b != _IsSelected;
            _IsSelected = b;

            if (Members != null && Members.Count > 0)
            {
                for (int i = 0; i < Members.Count; i++)
                {
                    Members[i].Da(b);
                }
            }
            //if(bInvoke)
            NotifyPropertyChanged("IsSelected");
        }
    }
}
