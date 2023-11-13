using Microsoft.Win32;
using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace ProjectPRN221.ViewModel
{
    public class ObjectViewModel : BaseViewModel
    {
        private ObservableCollection<Models.Object> _List;
        public ObservableCollection<Models.Object> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Unit> _ListUnit;
        public ObservableCollection<Unit> ListUnit { get => _ListUnit; set { _ListUnit = value; OnPropertyChanged(); } }


        private ObservableCollection<Suplier> _ListSuplier;
        public ObservableCollection<Suplier> ListSuplier { get => _ListSuplier; set { _ListSuplier = value; OnPropertyChanged(); } }

        private ObservableCollection<OutputInfo> _ListOutputInfo;
        public ObservableCollection<OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<InputInfo> _ListInputInfo;
        public ObservableCollection<InputInfo> ListInputInfo { get => _ListInputInfo; set { _ListInputInfo = value; OnPropertyChanged(); } }

        private Models.Object _SelectedItem;
        public Models.Object SelectedItem
        {
            get => _SelectedItem; 
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    QRCode = SelectedItem.Qrcode;
                    BarCode = SelectedItem.BarCode;
                    SelectedUnit = SelectedItem.IdUnitNavigation;
                    SelectedSuplier = SelectedItem.IdSuplierNavigation;
                    SelectedSuplierText = SelectedItem.IdSuplierNavigation.DisplayName;
                    SelectedUnitText = SelectedItem.IdUnitNavigation.DisplayName;
                }
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _QRCode;
        public string QRCode { get => _QRCode; set { _QRCode = value; OnPropertyChanged(); } }

        private string _BarCode;
        public string BarCode { get => _BarCode; set { _BarCode = value; OnPropertyChanged(); } }

        private Unit _SelectedUnit;
        public Unit SelectedUnit { get => _SelectedUnit; set { _SelectedUnit = value; OnPropertyChanged(); } }

        private Suplier _SelectedSuplier;
        public Suplier SelectedSuplier { get => _SelectedSuplier; set { _SelectedSuplier = value; OnPropertyChanged(); } }

        private string _SelectedUnitText;
        public string SelectedUnitText { get => _SelectedUnitText; set { _SelectedUnitText = value; OnPropertyChanged(); } }

        private string _SelectedSuplierText;
        public string SelectedSuplierText { get => _SelectedSuplierText; set { _SelectedSuplierText = value; OnPropertyChanged(); } }




        /*Chức năng nút*/
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ObjectViewModel()
        {

            var ListObject = DataProvider.Instance.DB.Objects;
            if (ListObject != null)
            {
                List = new ObservableCollection<Models.Object>(ListObject);
            }

            var ListUnits = DataProvider.Instance.DB.Units;
            if (ListUnits != null)
            {
                ListUnit = new ObservableCollection<Unit>(ListUnits);
            }

            var ListSupliers = DataProvider.Instance.DB.Supliers;
            if (ListSupliers != null)
            {
                ListSuplier = new ObservableCollection<Suplier>(ListSupliers);
            }

            var ListOutputInfos = DataProvider.Instance.DB.OutputInfos;
            if (ListOutputInfos != null)
            {
                ListOutputInfo = new ObservableCollection<OutputInfo>(ListOutputInfos);
            }

            var ListInputInfos = DataProvider.Instance.DB.InputInfos;
            if (ListInputInfos != null)
            {
                ListInputInfo = new ObservableCollection<InputInfo>(ListInputInfos);
            }

            AddCommand = new RelayCommand<ListView>((p) => {
                if (string.IsNullOrEmpty(DisplayName) || SelectedUnit == null || SelectedSuplier == null )
                {
                    return false;
                }
                return true;
            }, (p) => {
                bool flag = true;
                foreach (var item in List)
                {
                    if (DisplayName == item.DisplayName)
                    {
                        MessageBox.Show("Tên sản phẩm đã tồn tại!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                        flag = false;
                        break;
                    }

                }
                if (flag)
                {
                    // nham string voi int cuu voi huhu
                    //var lastObjectId = DataProvider.Instance.DB.Objects.OrderByDescending(o => o.Id).FirstOrDefault().Id;
                    //var newId = lastObjectId + 1;
                    var newObject = new Models.Object()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DisplayName = DisplayName,
                        IdSuplier = SelectedSuplier.Id,
                        IdUnit = SelectedUnit.Id,
                        Qrcode = QRCode,
                        BarCode = BarCode
                    };
                    DataProvider.Instance.DB.Objects.Add(newObject);
                    DataProvider.Instance.DB.SaveChanges();
                    List.Add(newObject);
                    SelectedItem = newObject;
                    
                }
            });

            EditCommand = new RelayCommand<ListView>((p) => {

                if (string.IsNullOrEmpty(DisplayName) || SelectedItem == null || SelectedUnit == null || SelectedSuplier == null )//string.IsNullOrEmpty(QRCode) || string.IsNullOrEmpty(BarCode) ||
                {
                    return false;
                }
                var displayList = DataProvider.Instance.DB.Objects.Where(x => x.Id == SelectedItem.Id);
                if (displayList == null || displayList.Count() != 0)
                {
                    return true;
                }
                return false;
            }, (p) => {

                var objects = DataProvider.Instance.DB.Objects.Where(pp => pp.Id == SelectedItem.Id).SingleOrDefault();
                objects.DisplayName = DisplayName;
                objects.IdSuplier = SelectedSuplier.Id;
                objects.IdUnit = SelectedUnit.Id;
                objects.Qrcode = QRCode;
                objects.BarCode = BarCode;
                DataProvider.Instance.DB.SaveChanges();
                SelectedItem.DisplayName = DisplayName;
                SelectedItem.IdSuplierNavigation.DisplayName = SelectedSuplier.DisplayName;
                SelectedItem.IdUnitNavigation.DisplayName = SelectedUnit.DisplayName;
                SelectedItem.Qrcode = QRCode;
                SelectedItem.BarCode = BarCode;
                for (int i = 0; i < List.Count(); i++)
                {
                    if (List[i].Id == SelectedItem.Id)
                    {
                        List[i] = new Models.Object()
                        {
                            Id = SelectedItem.Id,
                            DisplayName = DisplayName,
                            IdSuplierNavigation = SelectedSuplier,
                            IdUnitNavigation = SelectedUnit,
                            Qrcode = QRCode,
                            BarCode = BarCode
                        };
                        SelectedItem = List[i];
                        
                        break;
                    }
                }
                MessageBox.Show("Dữ liệu đã được sửa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            });

            DeleteCommand = new RelayCommand<ListView>((p) => {
                if (SelectedItem == null)
                {
                    return false;
                }
                //var displayList = DataProvider.Instance.DB.Units.Where(x => x.ID == SelectedItem.ID);
                //if (displayList == null || displayList.Count() == 0)
                //{
                //    return false;
                //}
                return true;
            }, (p) => {
                if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?",
                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var Object = DataProvider.Instance.DB.Objects.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    var dataInputInfo = DataProvider.Instance.DB.InputInfos.Where(x => x.IdObject == SelectedItem.Id).ToList();
                    var dataOutputInfo = DataProvider.Instance.DB.OutputInfos.Where(x => x.IdObject == SelectedItem.Id).ToList();

                    if (dataOutputInfo != null && dataOutputInfo.Count() != 0)//Xóa ID của Object trong OutputInfo
                    {
                        foreach (var item in dataOutputInfo)
                        {
                            DataProvider.Instance.DB.OutputInfos.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    var dataInOut = DataProvider.Instance.DB.OutputInfos.Where(x => x.IdObject == SelectedItem.Id).ToList();
                    if (dataInOut != null && dataInOut.Count() != 0)//Xóa ID của In trong Out
                    {
                        foreach (var item in dataInOut)
                        {
                            DataProvider.Instance.DB.OutputInfos.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }

                    if (dataInputInfo != null && dataInputInfo.Count() != 0)//Xóa ID của Object trong InputInfo
                    {
                        foreach (var item in dataInputInfo)
                        {
                            DataProvider.Instance.DB.InputInfos.Remove(item);

                            DataProvider.Instance.DB.SaveChanges();
                        }
                    }
                    foreach (var item in List)
                    {
                        if (item.Id == Object.Id)
                        {
                            List.Remove(item);
                            break;
                        }

                    }
                    DataProvider.Instance.DB.Objects.Remove(Object);
                    DataProvider.Instance.DB.SaveChanges();
                    //List.Remove(Object);

                    MessageBox.Show("Xóa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedItem = null;
                }
            });



        }
    }
}

