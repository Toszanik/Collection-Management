using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CollectorManagementSystem
{
    public class CollectionItem : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public ObservableCollection<Item> Items { get; set; }

        public CollectionItem(string name)
        {
            Name = name;
            Items = new ObservableCollection<Item>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}