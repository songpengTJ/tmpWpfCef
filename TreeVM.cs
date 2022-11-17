using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        private void init() {
            Tool.pack.data.cars.ForEach(p => {
                p.PropertyChanged += delegate (object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    if (!e.PropertyName.Equals("IsSelected")) 
                        return;
                    OnPropertyChanged("ListCars");
                };
            });
        }

        public List<Car> ListCars { get => Tool.pack.data.cars.Where(p => p.IsSelected  ).ToList(); }



    }
}
