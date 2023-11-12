using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace ProjectPRN221.ViewModel
{
    public class SuplierViewModel : BaseViewModel
    {
        private ObservableCollection<Suplier> list;
        public ObservableCollection<Suplier> List
        {
            get => list;
            set
            {
                list = value;
                OnPropertyChanged();
            }
        }
        private Suplier selectedItem;
        public Suplier SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    Address = SelectedItem.Address;
                    Phone = SelectedItem.Phone;
                    Email = SelectedItem.Email;
                    MoreInfo = SelectedItem.MoreInfo;
                    ContractDate = SelectedItem.ContractDate;
                    
                }
            }
        }

        private string displayName;
        public string DisplayName { get => displayName; set { displayName = value; OnPropertyChanged(); } }

        private string address;
        public string Address { get => address; set { address = value; OnPropertyChanged(); } }

        private string phone;
        public string Phone { get => phone; set { phone = value; OnPropertyChanged(); } }

        private string email;
        public string Email { get => email; set { email = value; OnPropertyChanged(); } }

        private string moreInfo;
        public string MoreInfo { get => moreInfo; set { moreInfo = value; OnPropertyChanged(); } }

        private DateTime? contractdate;
        public DateTime? ContractDate { get => contractdate; set { contractdate = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public SuplierViewModel()
        {
                List = new ObservableCollection<Suplier>(DataProvider.Instance.DB.Supliers);
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName))
                    return false;
                var displayList = DataProvider.Instance.DB.Supliers.Where(r => r.DisplayName == displayName);
                if (displayList == null || displayList.Count() != 0)
                    return false;

                return true;
            }, (p) =>
            {

                var newSuplier = new Suplier()
                {
                    DisplayName = DisplayName,
                    Address = Address,
                    Phone = Phone,
                    Email = Email,
                    MoreInfo = MoreInfo,
                    ContractDate = ContractDate
                };
                DataProvider.Instance.DB.Supliers.Add(newSuplier);
                DataProvider.Instance.DB.SaveChanges();
                List.Add(newSuplier);

            });

            EditCommand = new RelayCommand<ListView>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || SelectedItem == null)
                    return false;
                var displayList = DataProvider.Instance.DB.Supliers.Where(r => r.DisplayName == displayName);
                if (displayList == null || displayList.Count() != 0)
                    return false;
                return true;
            }, (p) =>
            {
                var editSuplier = DataProvider.Instance.DB.Supliers.Where(r => r.Id == SelectedItem.Id).SingleOrDefault();
                editSuplier.DisplayName = DisplayName;
                editSuplier.Address = Address;
                editSuplier.Phone = Phone;
                editSuplier.Email = Email;
                editSuplier.MoreInfo = MoreInfo;
                editSuplier.ContractDate = ContractDate;
                DataProvider.Instance.DB.SaveChanges();
                SelectedItem.DisplayName = DisplayName;
                SelectedItem.Address = Address;
                SelectedItem.Phone = Phone;
                SelectedItem.Email = Email;
                SelectedItem.MoreInfo = MoreInfo;
                SelectedItem.ContractDate = ContractDate;
                List = new ObservableCollection<Suplier>(DataProvider.Instance.DB.Supliers);
               
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa nhà cung cấp này?",
                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var suplier = DataProvider.Instance.DB.Supliers.Where(r => r.Id == SelectedItem.Id).SingleOrDefault();
                    var dataObject = DataProvider.Instance.DB.Objects.Where(r => r.IdSuplier == SelectedItem.Id).ToList();

                    var dataInputInfo = DataProvider.Instance.DB.InputInfos.Where(r => r.IdObjectNavigation.IdSuplier == SelectedItem.Id).ToList();

                    var dataOutputInfo = DataProvider.Instance.DB.OutputInfos.Where(r => r.IdObjectNavigation.IdSuplier == SelectedItem.Id).ToList();


                    if (dataOutputInfo != null && dataOutputInfo.Count() != 0)//Xóa ID của object trong Out
                    {
                        foreach (var item in dataOutputInfo)
                        {
                            DataProvider.Instance.DB.OutputInfos.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }


                    if (dataInputInfo != null && dataInputInfo.Count() != 0)//Xóa ID của object trong In
                    {
                        foreach (var item in dataInputInfo)
                        {
                            DataProvider.Instance.DB.InputInfos.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    if (dataObject != null && dataObject.Count() != 0)//Xóa ID của suplier trong object
                    {
                        foreach (var item in dataObject)
                        {
                            DataProvider.Instance.DB.Objects.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    foreach (var item in List)
                    {
                        if (item.Id == suplier.Id)
                        {
                            List.Remove(item);
                            break;
                        }

                    }
                    DataProvider.Instance.DB.Supliers.Remove(suplier);//xóa suplier
                    DataProvider.Instance.DB.SaveChanges();
                    //List.Remove(suplier);
                    MessageBox.Show("Xóa thành công");
                    SelectedItem = null;
                }
            });
        }
    }
}
