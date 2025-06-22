using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmCross.Plugin.Messenger;
using Registers.Classes;
using Registers.Classes.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Registers.ViewModels.Templates
{
    public abstract partial class DataViewModel<TDataType, TFormViewModel> : ObservableObject
        where TDataType : IdItem
        where TFormViewModel : IFormViewModel<TDataType>
    {
        // member variables
        [ObservableProperty]
        private ObservableCollection<TDataType> _filteredItems = new();

        protected readonly IMvxMessenger Messenger;

        // search bar text
        public string? SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                UpdateFilteredItems();
            }
        }
        private string? _searchText;

        public bool MatchAllWords
        {
            get => _matchAllWords;
            set
            {
                _matchAllWords = value;
                UpdateFilteredItems();
            }
        }
        private bool _matchAllWords;

        // =================================================================
        // abstract functions that need to be overriden: View Model & Events & Message & Filter
        protected abstract TFormViewModel CreateFormViewModel(TDataType? existingItem = null);

        protected abstract System.Windows.Controls.UserControl CreateMessagePayload(TFormViewModel formVM);

        protected abstract Func<TDataType, bool> GetFilter();

        // =================================================================
        // Constructor
        public DataViewModel(IMvxMessenger messenger)
        {
            Messenger = messenger;
        }

        // =================================================================
        // Fiter Items
        public void UpdateFilteredItems()
        {
            var allItems = DataRepository.Instance.Get<TDataType>();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? allItems
                : allItems.Where(GetFilter() ?? (_ => true));

            FilteredItems = new ObservableCollection<TDataType>(filtered);
        }

        // =================================================================
        // Load
        [RelayCommand]
        public void Load()
        {
            UpdateFilteredItems();
        }

        // =================================================================
        // Add
        [RelayCommand]
        protected void AddNewItem()
        {
            var formVM = CreateFormViewModel();
            formVM.Saved += item =>
            {
                DataRepository.Instance.Add(item);
                UpdateFilteredItems();
                Messenger.Publish(new MiddlePanelMessage(this, null));
            };
            
            formVM.Canceled += () =>
            {
                Messenger.Publish(new MiddlePanelMessage(this, null));
            };
            Messenger.Publish(new MiddlePanelMessage(this, CreateMessagePayload(formVM)));
        }

        // =================================================================
        // Edit
        [RelayCommand]
        protected void EditItem(TDataType item)
        {
            var formVM = CreateFormViewModel(item);
            formVM.Saved += updated =>
            {
                DataRepository.Instance.Update(updated);
                UpdateFilteredItems();
                Messenger.Publish(new MiddlePanelMessage(this, null));
            };
            formVM.Removed += id =>
            {
                var toRemove = DataRepository.Instance.Get<TDataType>().FirstOrDefault(x => (x as dynamic).Id == id);
                if (toRemove != null)
                    DataRepository.Instance.Remove(toRemove);

                UpdateFilteredItems();
                Messenger.Publish(new MiddlePanelMessage(this, null));
            };
            
            formVM.Canceled += () =>
            {
                Messenger.Publish(new MiddlePanelMessage(this, null));
            };

            Messenger.Publish(new MiddlePanelMessage(this, CreateMessagePayload(formVM)));
        }
    }
}
