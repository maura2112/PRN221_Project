using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectPRN221.ViewModel
{
    public class UnitViewModel : BaseViewModel
    {
        private ObservableCollection<Unit> list;
        public ObservableCollection<Unit> List
        {
            get => list;
            set
            {
                list = value;
                OnPropertyChanged();
            }
        }

        private Unit selectedItem;
        public Unit SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                }
            }
        }

        private string displayName;
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                OnPropertyChanged();

            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public UnitViewModel()
        {
            List = new ObservableCollection<Unit>(DataProvider.Instance.DB.Units);
            AddCommand = new RelayCommand<object>((p) =>
            {
                if(string.IsNullOrEmpty(DisplayName))
                    return false;
                var displayList = DataProvider.Instance.DB.Units.Where(r=>r.DisplayName == displayName);
                if(displayList == null || displayList.Count() != 0)
                    return false;

                return true;
                
            }, (p) =>
            {
                var newUnit = new Unit() { DisplayName = DisplayName };
                DataProvider.Instance.DB.Units.Add(newUnit);
                DataProvider.Instance.DB.SaveChanges();
                List.Add(newUnit);

            });
            EditCommand = new RelayCommand<ListView>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || SelectedItem == null)
                    return false;
                var displayList = DataProvider.Instance.DB.Units.Where(r => r.DisplayName == displayName);
                if (displayList == null || displayList.Count() != 0)
                    return false;

                return true;

            }, (p) =>
            {
                var editUnit = DataProvider.Instance.DB.Units.Where(r => r.Id == SelectedItem.Id).SingleOrDefault();
                editUnit.DisplayName = DisplayName;
                DataProvider.Instance.DB.SaveChanges();
                SelectedItem.DisplayName = DisplayName;
                List = new ObservableCollection<Unit>(DataProvider.Instance.DB.Units);

            });
            DeleteCommand = new RelayCommand<object>((p) => {
                if (SelectedItem == null)
                {
                    return false;
                }
                //var displayList = DataProvider.Ins.DB.Units.Where(x => x.ID == SelectedItem.ID);
                //if (displayList == null || displayList.Count() == 0)
                //{
                //    return false;
                //}
                return true;
            }, (p) => {

                if (MessageBox.Show("Bạn có chắc muốn xóa đơn vị đo này?",
                                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var unit = DataProvider.Instance.DB.Units.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    var dataObject = DataProvider.Instance.DB.Objects.Where(x => x.IdUnit == SelectedItem.Id).ToList();
                    //String IDObject = dataObject[0].ID;
                    var dataInputInfo = DataProvider.Instance.DB.InputInfos.Where(x => x.IdObjectNavigation.IdUnit == SelectedItem.Id).ToList();
                    //String IDInputInfo = dataInputInfo[0].ID;
                    var dataOutputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.IdObjectNavigation.IdUnit == SelectedItem.Id).ToList();


                    if (dataOutputInfo != null && dataOutputInfo.Count() != 0)//Xóa ID của object trong Out
                    {
                        foreach (var item in dataOutputInfo)
                        {
                            DataProvider.Instance.DB.OutputInfos.Remove(item);
                            //ListOutputInfo.Remove(item);
                            //ListOutputInfo.Clear();
                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    if (dataInputInfo != null && dataInputInfo.Count() != 0)//Xóa ID của object trong In
                    {
                        foreach (var item in dataInputInfo)
                        {
                            DataProvider.Instance.DB.InputInfos.Remove(item);
                            //ListInputInfo.Remove(item);
                            //ListInputInfo.Clear();
                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    if (dataObject != null && dataObject.Count() != 0)//Xóa ID của unit trong object
                    {
                        foreach (var item in dataObject)
                        {
                            DataProvider.Instance.DB.Objects.Remove(item);
                            //ListObject.Remove(item);
                            //ListObject.Clear();
                            DataProvider.Instance.DB.SaveChanges();

                        }
                    }
                    foreach (var item in List)
                    {
                        if (item.Id == unit.Id)
                        {
                            List.Remove(item);
                            break;
                        }

                    }
                    DataProvider.Instance.DB.Units.Remove(unit);//Xóa unit
                    DataProvider.Instance.DB.SaveChanges();
                    //List.Remove(unit);

                    MessageBox.Show("Xóa thành công");
                    SelectedItem = null;
                    
                }
            });
        }
    }
}
