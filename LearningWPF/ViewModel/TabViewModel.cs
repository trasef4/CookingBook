using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LearningWPF.Model;
using LearningWPF.Service;
using LearningWPF.Util;
using System.IO;

namespace LearningWPF.ViewModel
{
    public class TabViewModel : PropertyChange
    {
        #region Tabs

        private ICategoryDataService _categoryDataSevice;

        private ItemTab _selectedCategory;
        private IDialogService _dialogService;
        /// <summary>
        /// Коллекция для хранения TabItem`ов
        /// </summary>
        public ObservableCollectionEx<ItemTab> ItemTabs { get; set; }
        public ItemTab SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }
        public ICommand AddTabCommand { get; private set; }

        private object _currentViewRecipe;
        public object CurrentViewRecipe
        {
            get { return _currentViewRecipe; }
            set { OnPropertyChanged(ref _currentViewRecipe, value); }
        }

        private void AddTab()
        {
            ItemTab item = new ItemTab
            {
                Category = "Категория"
            };
            ItemTabs.Add(item);
            SelectedCategory = item;
            _categoryDataSevice.SaveCategories(ItemTabs);
        }
        public ICommand RemoveTabCommand { get; private set; }
        private void RemoveTab()
        {
            ItemTabs.Remove(SelectedCategory);
        }     

        public ICommand SaveTabCommand { get; private set; }
        private void SaveCategory()
        {
            //TODO: При нажатии "Enter" => выходить из режима редактирования
            if(_selectedCategory.Category.Length == 0)
            {
                MessageBox.Show("Неоходимо указать название категории");
                SelectedCategory.Category = "Категория";
            }
            else
            {
                var saveCat = from i in ItemTabs
                              where i.IsEditTabMode == true
                              select i;
                foreach(var item in saveCat)
                {
                    item.IsEditTabMode = false;
                }
                OnPropertyChanged("_selectedCategory");
                _categoryDataSevice.SaveCategories(ItemTabs);
            }
        }
        public ICommand SortCategoryCommand { get; private set; }
        public void Sort()
        {
            ItemTabs.Sort(p => p.Category);
        }
        public void LoadCategory(IEnumerable<ItemTab> itemTabs)
        {
            ItemTabs = new ObservableCollectionEx<ItemTab>(itemTabs);
            OnPropertyChanged("ItemTabs");
        }
        public ICommand BackupCommand { get; private set; }
        public void Backup()
        {
            string Path = _dialogService.SaveFile();
            if (Path == null) {  }
            else { File.Copy("Recipe.json", $@"{Path}.json"); }
        }

        public TabViewModel(ICategoryDataService categoryDataSevice, IDialogService dialogService)
        {
            SortCategoryCommand = new RelayCommand(Sort);
            BackupCommand = new RelayCommand(Backup);
            ItemTabs = new ObservableCollectionEx<ItemTab>();
            AddTabCommand = new RelayCommand(AddTab);
            RemoveTabCommand = new RelayCommand(RemoveTab);
            SaveTabCommand = new RelayCommand(SaveCategory);
            _categoryDataSevice = categoryDataSevice;
            _dialogService = dialogService;
        }
        #endregion
    }
}

