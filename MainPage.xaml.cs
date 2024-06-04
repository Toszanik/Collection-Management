using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CollectorManagementSystem
{
    public partial class MainPage : ContentPage
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
        public ObservableCollection<CollectionItem> Collections { get; set; } = new ObservableCollection<CollectionItem>();

        public MainPage()
        {
            InitializeComponent();
            LoadCollections();
            CollectionsView.ItemsSource = Collections;
            Debug.WriteLine(filePath);
        }

        public async Task LoadCollections()
        {
            Collections.Clear();
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                CollectionItem currentCollection = null;

                foreach (var line in lines)
                {
                    var parts = line.Split("|||");
                    if (parts.Length == 1)
                    {
                        currentCollection = new CollectionItem(parts[0]);
                        Collections.Add(currentCollection);
                    }
                    else if (parts.Length == 2 && currentCollection != null)
                    {
                        currentCollection.Items.Add(new Item
                        {
                            Id = parts[0],
                            Name = parts[1]
                        });
                    }
                }
            }
        }

        public async Task SaveCollections()
        {
            var lines = new ObservableCollection<string>();

            foreach (var collection in Collections)
            {
                lines.Add(collection.Name);
                foreach (var item in collection.Items)
                {
                    lines.Add($"{item.Id}|||{item.Name}");
                }
            }

            await File.WriteAllLinesAsync(filePath, lines);
        }

        private async void AddCollectionButton_Clicked(object sender, EventArgs e)
        {
            string newCollectionName = await DisplayPromptAsync("New Collection", "Enter the name of the new collection:");
            if (!string.IsNullOrWhiteSpace(newCollectionName))
            {
                Collections.Add(new CollectionItem(newCollectionName));
                await SaveCollections();
            }
        }

        private async void ChangeNameButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var collection = button.BindingContext as CollectionItem;
            string newName = await DisplayPromptAsync("Change Collection Name", "Enter new name for the collection:", initialValue: collection.Name);
            if (!string.IsNullOrWhiteSpace(newName))
            {
                collection.Name = newName;
                await SaveCollections();
            }
        }

        private async void DeleteCollectionButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var collection = button.BindingContext as CollectionItem;
            bool confirm = await DisplayAlert("Delete Collection", $"Are you sure you want to delete {collection.Name}?", "Yes", "No");
            if (confirm)
            {
                Collections.Remove(collection);
                await SaveCollections();
            }
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var collection = button.BindingContext as CollectionItem;

            string newItem = await DisplayPromptAsync("Add Item", "Enter name for the new item:");
            if (!string.IsNullOrWhiteSpace(newItem) && collection != null)
            {
                collection.Items.Add(new Item
                {
                    Id = Item.generateID(),
                    Name = newItem
                }) ;

                await SaveCollections();
            }
        }

        private async void EditItem_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as Item;
            var collection = Collections.FirstOrDefault(c => c.Items.Contains(item));

            string newName = await DisplayPromptAsync("Edit Item", "Enter new name for the item:", initialValue: item.Name);
            if (!string.IsNullOrWhiteSpace(newName) && collection != null)
            {
                var targetItem = collection.Items.FirstOrDefault(i => i.Id == item.Id);
                if (targetItem != null)
                {
                    targetItem.Name = newName;
                    await SaveCollections();
                }
            }
        }

        private async void DeleteItem_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as Item;
            var collection = Collections.FirstOrDefault(c => c.Items.Contains(item));

            if (collection != null)
            {
                collection.Items.Remove(item);
                await SaveCollections();
            }
        }
    }
}
