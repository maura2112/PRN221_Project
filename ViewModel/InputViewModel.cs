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
    public class InputViewModel : BaseViewModel
    {
        private ObservableCollection<InputInfo> _List;
        public ObservableCollection<InputInfo> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        //private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        //public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Models.Object> _ListObject;
        public ObservableCollection<Models.Object> ListObject { get => _ListObject; set { _ListObject = value; OnPropertyChanged(); } }

        //private ObservableCollection<Input> _Input;
        //public ObservableCollection<Input> Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }

        private Models.Object _SelectedObject;
        public Models.Object SelectedObject { get => _SelectedObject; set { _SelectedObject = value; OnPropertyChanged(); } }

        private Input _SelectedInput;
        public Input SelectedInput
        {
            get => _SelectedInput;
            set
            {
                _SelectedInput = value;
                OnPropertyChanged();
                if (SelectedInput != null)
                {
                    //MessageBox.Show(SelectedInput.ID);
                }

            }
        }



        private InputInfo _SelectedItem;
        public InputInfo SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    SelectedObject = SelectedItem.IdObjectNavigation;
                    SelectedInput = SelectedItem.IdInputNavigation;
                    SelectedInputDate = SelectedItem.IdInputNavigation.DateInput;
                    SelectedObjectText = SelectedItem.IdObjectNavigation.DisplayName;
                    Count = SelectedItem.Count;
                    Status = SelectedItem.Status;
                    InputPrice = SelectedItem.InputPrice;
                    OutputPrice = SelectedItem.OutputPrice;
                }
            }
        }

        private int? _Count;
        public int? Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private double? _InputPrice;
        public double? InputPrice { get => _InputPrice; set { _InputPrice = value; OnPropertyChanged(); } }

        private double? _OutputPrice;
        public double? OutputPrice { get => _OutputPrice; set { _OutputPrice = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        private string _SelectedObjectText;
        public string SelectedObjectText { get => _SelectedObjectText; set { _SelectedObjectText = value; OnPropertyChanged(); } }

        private DateTime? _SelectedInputDate;
        public DateTime? SelectedInputDate
        {
            get => _SelectedInputDate;
            set
            {
                _SelectedInputDate = value;
                OnPropertyChanged();
                if (SelectedInputDate != null)
                {
                    //MessageBox.Show(SelectedInputDate.ToString());
                    SelectedInput.DateInput = SelectedInputDate;
                }
            }
        }


        /*Chức năng nút*/
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }


        public InputViewModel()
        {




            List = new ObservableCollection<InputInfo>(DataProvider.Instance.DB.InputInfos.Include(r => r.IdInputNavigation).ToList());

            var ListObjects = DataProvider.Instance.DB.Objects;
            if (ListObjects != null)
            {
                ListObject = new ObservableCollection<Models.Object>(ListObjects);
            }

            SelectedInput = new Input();

            AddCommand = new RelayCommand<object>((p) =>
            {

                if (SelectedObject == null || SelectedInput.DateInput == null || Count == 0)
                    return false;

                return true;

            }, (p) =>
            {
                if (InputPrice > OutputPrice)
                {
                    MessageBox.Show("Giá xuất phải lớn hơn giá nhập!");
                }
                else
                {
                    var input = new Input()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DateInput = SelectedInputDate
                    };

                    var inputInfo = new InputInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdObject = SelectedObject.Id,
                        IdInput = input.Id,
                        Count = Count,
                        InputPrice = InputPrice,
                        OutputPrice = OutputPrice,
                        Status = Status
                    };

                    DataProvider.Instance.DB.Inputs.Add(input);
                    DataProvider.Instance.DB.InputInfos.Add(inputInfo);
                    try
                    {
                        DataProvider.Instance.DB.SaveChanges();
                        List.Add(inputInfo);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });

            EditCommand = new RelayCommand<ListView>((p) =>
            {
                if (SelectedItem == null || SelectedObject == null || SelectedInput == null || Count == 0)
                    return false;

                var displayList = DataProvider.Instance.DB.InputInfos.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                if (InputPrice > OutputPrice)
                {
                    MessageBox.Show("Giá xuất phải lớn hơn giá nhập");
                }
                else
                {
                    var inputInfo = DataProvider.Instance.DB.InputInfos.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    inputInfo.IdObject = SelectedObject.Id;
                    inputInfo.IdInput = SelectedInput.Id;

                    //InputInfo.idInput = Input.id;
                    inputInfo.Count = Count;
                    inputInfo.InputPrice = InputPrice;
                    inputInfo.OutputPrice = OutputPrice;
                    inputInfo.Status = Status;
                    DataProvider.Instance.DB.SaveChanges();
                    for (int i = 0; i < List.Count(); i++)
                    {
                        if (List[i].Id == SelectedItem.Id)
                        {
                            //SelectedInput = DataProvider.Instance.DB.Inputs.Where(x => x.ID == SelectedItem.ID).SingleOrDefault();
                            List[i] = new InputInfo()
                            {
                                Id = SelectedItem.Id,
                                IdObjectNavigation = SelectedObject,
                                IdInputNavigation = SelectedInput,
                                Count = Count,
                                InputPrice = InputPrice,
                                OutputPrice = OutputPrice,
                                Status = Status
                            };
                            //p.SelectedItem = List[i];
                            //break;
                        }
                    }
                    MessageBox.Show("Sửa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            });

            DeleteCommand = new RelayCommand<InputInfo>((p) =>
            {
                if (SelectedItem == null || SelectedObject == null || SelectedInput == null)
                    return false;

                var displayList = DataProvider.Instance.DB.InputInfos.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa phiếu xuất này?",
                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var inputInfo = DataProvider.Instance.DB.InputInfos.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    var dataOutputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.Id == SelectedItem.Id).ToList();
                    var dataInput = DataProvider.Instance.DB.Inputs.Where(x => x.Id == SelectedItem.IdInputNavigation.Id).ToList();

                    if (dataOutputInfo != null && dataOutputInfo.Count() != 0)
                    {
                        foreach (var item in dataOutputInfo)
                        {
                            DataProvider.Instance.DB.OutputInfos.Remove(item);
                            DataProvider.Instance.DB.SaveChanges();
                            //ListOutputInfo.Remove(item);
                        }
                        //dataOutputInfo.Clear();
                    }

                    DataProvider.Instance.DB.InputInfos.Remove(inputInfo);
                    List.Remove(inputInfo);
                    DataProvider.Instance.DB.SaveChanges();

                    if (dataInput != null && dataInput.Count() != 0)
                    {
                        foreach (var item in dataInput)
                        {
                            DataProvider.Instance.DB.Inputs.Remove(item);
                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }
                    MessageBox.Show("Xóa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedItem = null;
                }
            });
        }
    }
}
