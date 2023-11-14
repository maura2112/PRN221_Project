using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN221.ViewModel
{
    public class OutputViewModel : BaseViewModel
    {
        private ObservableCollection<Inventory> _InventoryList;
        public ObservableCollection<Inventory> InventoryList { get => _InventoryList; set { _InventoryList = value; OnPropertyChanged(); } }

        private ObservableCollection<OutputInfo> _List;
        public ObservableCollection<OutputInfo> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Models.Object> _ListObject;
        public ObservableCollection<Models.Object> ListObject { get => _ListObject; set { _ListObject = value; OnPropertyChanged(); } }

        private ObservableCollection<Customer> _ListCustomer;
        public ObservableCollection<Customer> ListCustomer { get => _ListCustomer; set { _ListCustomer = value; OnPropertyChanged(); } }

        private Statistic _Statistic;
        public Statistic Statistic { get => _Statistic; set { _Statistic = value; OnPropertyChanged(); } }

        private Customer _SelectedCustomer;
        public Customer SelectedCustomer { get => _SelectedCustomer; set { _SelectedCustomer = value; OnPropertyChanged(); } }

        private Models.Object _SelectedObject;
        public Models.Object SelectedObject
        {
            get => _SelectedObject;
            set
            {
                _SelectedObject = value;
                OnPropertyChanged();
                if (SelectedObject != null)
                {
                    var priceMax = DataProvider.Instance.DB.InputInfos.Where(x => (x.IdObject == SelectedObject.Id)).Max(x => x.OutputPrice);
                    //foreach(var item in OuputPrice1)
                    //{
                    //    if(item.OutputPrice>max)
                    //    {
                    //        max = (double)item.OutputPrice;
                    //    }    
                    //}
                    var OuputPrice = DataProvider.Instance.DB.InputInfos.Where(x => (x.IdObject == SelectedObject.Id) && (x.OutputPrice == priceMax)).FirstOrDefault();
                    if (OuputPrice != null)
                    {
                        PriceObject = OuputPrice;
                    }
                    else
                    {
                        PriceObject = null;
                    }
                }
            }
        }

        private OutputInfo _SelectedItem;
        public OutputInfo SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    SelectedObject = SelectedItem.IdObjectNavigation;
                    SelectedCustomer = SelectedItem.IdCustomerNavigation;
                    DateOutput = SelectedItem.IdOutputNavigation.DateOutput;
                    Count = SelectedItem.Count;
                    Status = SelectedItem.Status;
                    OutputPrice = SelectedItem.SumPrice;
                    SelectedCustomerText = SelectedItem.IdCustomerNavigation.DisplayName;
                    //SelectedObjectText = SelectedItem.IdObjectNavigation.DisplayName;
                }
            }
        }

        private string _SelectedObjectText;
        public string SelectedObjectText { get => _SelectedObjectText; set { _SelectedObjectText = value; OnPropertyChanged(); } }

        private string _SelectedCustomerText;
        public string SelectedCustomerText { get => _SelectedCustomerText; set { _SelectedCustomerText = value; OnPropertyChanged(); } }

        private int? _Count;
        public int? Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private InputInfo _PriceObject;
        public InputInfo PriceObject { get => _PriceObject; set { _PriceObject = value; OnPropertyChanged(); } }

        private double? _InputPrice;
        public double? InputPrice { get => _InputPrice; set { _InputPrice = value; OnPropertyChanged(); } }

        private double? _OutputPrice;
        public double? OutputPrice { get => _OutputPrice; set { _OutputPrice = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        private string _Total;
        public string Total { get => _Total; set { _Total = value; OnPropertyChanged(); } }

        private DateTime? _DateOutput;
        public DateTime? DateOutput { get => _DateOutput; set { _DateOutput = value; OnPropertyChanged(); } }

        private string _ComboBoxText;
        public string ComboBoxText { get => _ComboBoxText; set { _ComboBoxText = value; OnPropertyChanged(); } }


        /*Chức năng nút*/
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }


        public OutputViewModel()
        {
            //Input = new ObservableCollection<Model.Input>(DataProvider.Instance.DB.Inputs);

                List = new ObservableCollection<OutputInfo>(DataProvider.Instance.DB.OutputInfos.Include(r => r.IdOutputNavigation).ToList());


            var ListObjects = DataProvider.Instance.DB.Objects;
            if (ListObjects != null)
            {
                ListObject = new ObservableCollection<Models.Object>(ListObjects);
            }

            var ListCustomers = DataProvider.Instance.DB.Customers;
            if (ListCustomers != null)
            {
                ListCustomer = new ObservableCollection<Customer>(ListCustomers);
            }

            AddCommand = new RelayCommand<object>((p) =>
            {

                if (SelectedObject == null || Count == null || Count == 0 || SelectedCustomer == null)
                    return false;

                return true;

            }, (p) =>
            {

                if (CountObject() < Count)
                {
                    MessageBox.Show("Số lượng sản phẩm còn lại trong kho: " + CountObject() );
                }
                else
                {
                    var dataObjectByID = DataProvider.Instance.DB.Objects.Where(x => x.Id == SelectedObject.Id).SingleOrDefault();
                    var dataInputInfoByObject = DataProvider.Instance.DB.InputInfos.Where(x => x.IdObject == dataObjectByID.Id).First();
                    Customer customer;
                    if (SelectedCustomer == null)
                    {
                        string customerName = "";
                        if (string.IsNullOrWhiteSpace(ComboBoxText))
                            customerName = "Khách hàng chưa có tên";
                        else
                            customerName = ComboBoxText;
                        customer = new Customer()
                        {
                            DisplayName = customerName,
                            
                        };
                        DataProvider.Instance.DB.Customers.Add(customer);
                    }
                    else
                    {
                        customer = SelectedCustomer;
                    }

                    var user = DataProvider.Instance.DB.Customers.Where(x => x.Id == SelectedCustomer.Id).First();
                    var output = new Output()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DateOutput = DateOutput
                    };
                    var outputInfo = new OutputInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdOutput = output.Id,
                        IdOutputNavigation = output,
                        IdObject = SelectedObject.Id,
                        Count = Count,
                        Status = Status,
                        IdCustomer = user.Id,
                        IdObjectNavigation = SelectedObject,
                        SumPrice = (int?)(Count * PriceObject.OutputPrice),
                    };
                    DataProvider.Instance.DB.OutputInfos.Add(outputInfo);
                    DataProvider.Instance.DB.SaveChanges();
                    try
                    {

                        List.Add(outputInfo);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });

            EditCommand = new RelayCommand<ListView>((p) =>
            {
                if (SelectedObject == null || SelectedItem == null || Count == 0  )
                    return false;

                var displayList = DataProvider.Instance.DB.Outputs.Where(x => x.Id == SelectedItem.IdOutput);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return true;

            }, (p) =>
            {
                if (CountObject() < Count)
                {
                    MessageBox.Show("Số lượng sản phẩm còn lại trong kho: " + CountObject());
                }
                else
                {
                    var dataObjectByID = DataProvider.Instance.DB.Objects.Where(x => x.Id == SelectedObject.Id).SingleOrDefault();
                    var dataInputInfoByObject = DataProvider.Instance.DB.InputInfos.Where(x => x.IdObject == dataObjectByID.Id).First();
                    var outputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.Id == SelectedItem.Id).FirstOrDefault();
                    outputInfo.IdOutputNavigation = SelectedItem.IdOutputNavigation;
                    outputInfo.IdObjectNavigation = SelectedObject;
                    outputInfo.IdObject = SelectedObject.Id;
                    outputInfo.Count = Count;
                    outputInfo.Status = Status;
                    outputInfo.SumPrice = (int?)(Count * PriceObject.OutputPrice);
                    outputInfo.IdOutputNavigation.DateOutput = DateOutput;
                    DataProvider.Instance.DB.SaveChanges();
                    for (int i = 0; i < List.Count(); i++)
                    {
                        if (List[i].Id == SelectedItem.Id)
                        {
                            List[i] = new OutputInfo()
                            {
                                Id = SelectedItem.Id,
                                IdOutputNavigation = List[i].IdOutputNavigation,
                                IdObject = SelectedObject.Id,
                                Count = Count,                          
                                IdCustomerNavigation = SelectedItem.IdCustomerNavigation,
                                Status = Status,
                                IdObjectNavigation = SelectedObject,
                                SumPrice = (int?)(PriceObject.OutputPrice * Count)
                            };
                            SelectedItem = List[i];
                            //p.SelectedItem = SelectedItem;
                            //break;
                        }
                    }
                    MessageBox.Show("Sửa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            });

            DeleteCommand = new RelayCommand<InputInfo>((p) =>
            {
                if (SelectedItem == null || List == null)
                    return false;

                var displayList = DataProvider.Instance.DB.Outputs.Where(x => x.Id == SelectedItem.IdOutput);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return true;

            }, (p) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa phiếu xuất này?",
                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var outputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    var listOutput = DataProvider.Instance.DB.OutputInfos.Where(x => x.IdOutput == SelectedItem.IdOutput).ToList();
                    Output output = new Output();
                    if (listOutput != null && listOutput.Count() != 0)//Xóa ID của Object trong InputInfo
                    {
                        var IDOutput = listOutput[0].IdOutput;
                        output = DataProvider.Instance.DB.Outputs.Where(x => x.Id == IDOutput).First();
                        DataProvider.Instance.DB.OutputInfos.Remove(outputInfo);
                        DataProvider.Instance.DB.SaveChanges();
                    }

                    foreach (var item in List)
                    {
                        if (item.Id == outputInfo.Id)
                        {
                            List.Remove(item);
                            break;
                        }

                    }
                    //List.Remove(outputInfo);
                    if (listOutput.Count() == 1)
                    {
                        DataProvider.Instance.DB.Outputs.Remove(output);
                        DataProvider.Instance.DB.SaveChanges();
                    }
                    SelectedItem = null;
                    MessageBox.Show("Xóa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });


        }

        int CountObject()
        {
            InventoryList = new ObservableCollection<Inventory>();
            Statistic = new Statistic();
            var dataObjectByDisplayName = DataProvider.Instance.DB.Objects.Where(x => x.DisplayName == SelectedObject.DisplayName).ToList();
            int input = 0;
            int output = 0;


            foreach (var item in dataObjectByDisplayName)
            {
                var dataInputInfo = DataProvider.Instance.DB.InputInfos.Where(x => x.IdObject == item.Id);
                var dataOutputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.IdObject == item.Id);

                int sumInputInfo = 0;
                int sumOutputInfor = 0;


                if (dataInputInfo != null && dataInputInfo.Count() > 0)
                {
                    sumInputInfo = (int)dataInputInfo.Sum(x => x.Count);

                    input += sumInputInfo;
                }

                if (dataOutputInfo != null && dataOutputInfo.Count() > 0)
                {
                    sumOutputInfor = (int)dataOutputInfo.Sum(x => x.Count);
                    output += sumOutputInfor;
                }
            }

            Statistic.Inventory = input - output;
            return Statistic.Inventory;
        }
    }
}
