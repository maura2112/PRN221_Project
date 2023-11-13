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
    public class CustomerViewModel : BaseViewModel
    {
        private ObservableCollection<Customer> _List;
        public ObservableCollection<Customer> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private Customer _SelectedItem;
        public Customer SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    Phone = SelectedItem.Phone;
                    Email = SelectedItem.Email;
                    Address = SelectedItem.Address;
                    MoreInfo = SelectedItem.MoreInfo;
                    ContractDate = SelectedItem.ContractDate;
                }
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }


        private string _Phone;
        public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }


        private string _Address;
        public string Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }


        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }


        private string _MoreInfo;
        public string MoreInfo { get => _MoreInfo; set { _MoreInfo = value; OnPropertyChanged(); } }


        private DateTime? _ContractDate;
        public DateTime? ContractDate { get => _ContractDate; set { _ContractDate = value; OnPropertyChanged(); } }


        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public CustomerViewModel()
        {
            List = new ObservableCollection<Customer>(DataProvider.Instance.DB.Customers);

            AddCommand = new RelayCommand<object>((p) =>
            {
                return true;

            }, (p) =>
            {
                var Customer = new Customer()
                {
                    DisplayName = DisplayName,
                    Phone = Phone,
                    Address = Address,
                    Email = Email,
                    ContractDate = ContractDate,
                    MoreInfo = MoreInfo
                };

                DataProvider.Instance.DB.Customers.Add(Customer);
                DataProvider.Instance.DB.SaveChanges();

                List.Add(Customer);
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Instance.DB.Customers.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var Customer = DataProvider.Instance.DB.Customers.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Customer.DisplayName = DisplayName;
                Customer.Phone = Phone;
                Customer.Address = Address;
                Customer.Email = Email;
                Customer.ContractDate = ContractDate;
                Customer.MoreInfo = MoreInfo;
                DataProvider.Instance.DB.SaveChanges();
                SelectedItem.DisplayName = DisplayName;
                SelectedItem.Address = Address;
                SelectedItem.Phone = Phone;
                SelectedItem.Email = Email;
                SelectedItem.MoreInfo = MoreInfo;
                SelectedItem.ContractDate = ContractDate;
                for (int i = 0; i < List.Count(); i++)
                {
                    if (List[i].Id == SelectedItem.Id)
                    {
                        List[i] = new Customer()
                        {
                            Id = SelectedItem.Id,
                            DisplayName = DisplayName,
                            Address = Address,
                            Phone = Phone,
                            Email = Email,
                            MoreInfo = MoreInfo,
                            ContractDate = ContractDate
                        };
                        SelectedItem = List[i];

                        break;
                    }
                }
                MessageBox.Show("Sửa thành công");
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
                if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?",
                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var customer = DataProvider.Instance.DB.Customers.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    //var dataOuput = DataProvider.Instance.DB.Outputs.Where(x => x.Id == SelectedItem.Id);
                    var dataOutputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.IdCustomerNavigation.Id == SelectedItem.Id).ToList();
                    if (dataOutputInfo != null && dataOutputInfo.Count() != 0)//Xóa ID của Output trong OutputInfo
                    {
                        foreach (var item in dataOutputInfo)
                        {
                            DataProvider.Instance.DB.OutputInfos.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    foreach (var item in List)
                    {
                        if (item.Id == customer.Id)
                        {
                            List.Remove(item);
                            break;
                        }

                    }

                    DataProvider.Instance.DB.Customers.Remove(customer);
                    DataProvider.Instance.DB.SaveChanges();
                    //List.Remove(customer);
                    MessageBox.Show("Xóa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedItem = null;
                }
            });
        }
    }
}
