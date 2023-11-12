using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectPRN221.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand UnitCommand { get; set; }
        public ICommand SuplierCommand { get; set; }
        public ICommand CustomerCommand { get; set; }
        public ICommand ObjectCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand InputCommand { get; set; }
        public ICommand OutputCommand { get; set; }

        public ICommand StatisticCommand { get; set; }

        public ICommand FilterCommand { get; set; }



        private ObservableCollection<Inventory> inventoryList;
        public ObservableCollection<Inventory> InventoryList
        {
            get => inventoryList;
            set
            {
                inventoryList = value;
                OnPropertyChanged();
            }
        }

        private Statistic statistic;
        public Statistic Statistics { get => statistic; set { statistic = value; OnPropertyChanged(); } }
        // mọi thứ xử lý sẽ nằm trong này
        public MainViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
                if (p == null) return;
                p.Hide();

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                if (loginWindow.DataContext == null) return;

                var loginVM = loginWindow.DataContext as LoginViewModel;
                if (loginVM.IsLogin)
                {
                    p.Show();
                    LoadInventoryData(false);
                }
                else
                {
                    p.Close();
                }
            }
              );

            UnitCommand = new RelayCommand<object>((p) => { return true; }, (p) => { UnitWindow wd = new UnitWindow(); wd.ShowDialog(); });
            SuplierCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SuplierWindow wd = new SuplierWindow(); wd.ShowDialog(); });
            CustomerCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CustomerWindow wd = new CustomerWindow(); wd.ShowDialog(); });
            ObjectCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ObjectWindow wd = new ObjectWindow(); wd.ShowDialog(); });
            UserCommand = new RelayCommand<object>((p) => { return true; }, (p) => { UserWindow wd = new UserWindow(); wd.ShowDialog(); });
            InputCommand = new RelayCommand<object>((p) => { return true; }, (p) => { InputWindow wd = new InputWindow(); wd.ShowDialog(); });
            OutputCommand = new RelayCommand<object>((p) => { return true; }, (p) => { OutputWindow wd = new OutputWindow(); wd.ShowDialog(); });
            //var a  = DataProvider.Instance.DB.Users.ToList();
            //MessageBox.Show(DataProvider.Instance.DB.Users.First().DisplayName);
            StatisticCommand = new RelayCommand<Axis>(
                (p) =>
                {
                    if (SelectedKindDate == null || SelectedDate == null)
                        return false;
                    return true;
                }, (p) =>
                {
                    switch (SelectedKindDate)
                    {
                        case "Năm":
                            p.MaxValue = 12;
                            LoadChartByYear();
                            break;
                        case "Tháng":
                            p.MaxValue = LoadChartByMonth();
                            p.LabelsRotation = 40;
                            break;
                        case "Tuần":
                            p.MaxValue = 7;
                            LoadChartByWeek();
                            break;
                        default:
                            LoadChartByDay();
                            break;
                    }
                });
            ListKindDate = new List<string>() { };
            ListKindDate.Add("Năm");
            ListKindDate.Add("Tháng");
            ListKindDate.Add("Tuần");
            ListKindDate.Add("Ngày");

            ListKindObject = new List<string>() { };
            ListKindObject.Add("Tên");
            ListKindObject.Add("Sản phẩm bán chạy nhất");
            ListKindObject.Add("Sản phẩm ít người mua nhất");

            FilterCommand = new RelayCommand<object>(
                (p) =>
                {
                    if (SelectedKindObject == null && Filter == null)
                        return false;
                    return true;
                }, (p) =>
                {

                    LoadFilter(SelectedKindObject);

                });

        }



        #region property cho charts
        private List<DataForCharts> _DataForChart;
        public List<DataForCharts> DataForChart { get => _DataForChart; set { _DataForChart = value; OnPropertyChanged(); } }

        private SeriesCollection _PieChartSeriesCollection;
        public SeriesCollection PieChartSeriesCollection { get => _PieChartSeriesCollection; set { _PieChartSeriesCollection = value; OnPropertyChanged(); } }

        private SeriesCollection _PieChartSeriesCollectionForSP;
        public SeriesCollection PieChartSeriesCollectionForSP { get => _PieChartSeriesCollectionForSP; set { _PieChartSeriesCollectionForSP = value; OnPropertyChanged(); } }

        private SeriesCollection _CartesianChartSeriesCollection;
        public SeriesCollection CartesianChartSeriesCollection { get => _CartesianChartSeriesCollection; set { _CartesianChartSeriesCollection = value; OnPropertyChanged(); } }


        private SeriesCollection _AxisY;
        public SeriesCollection AxisY { get => _AxisY; set { _AxisY = value; OnPropertyChanged(); } }

        private String[] _Labels;
        public String[] Labels { get => _Labels; set { _Labels = value; OnPropertyChanged(); } }

        private LegendLocation _Legend;
        public LegendLocation Legend { get => _Legend; set { _Legend = value; OnPropertyChanged(); } }

        private Visibility _VisibilityOfTitle;
        public Visibility VisibilityOfTitle { get => _VisibilityOfTitle; set { _VisibilityOfTitle = value; OnPropertyChanged(); } }

        private DateTime _DateBeginInventory;
        public DateTime DateBeginInventory { get => _DateBeginInventory; set { _DateBeginInventory = value; OnPropertyChanged(); } }

        public Dictionary<string, int> DictionaryOfData { get; private set; }

        private int _MaxValue;
        public int MaxValue { get => _MaxValue; set { _MaxValue = value; OnPropertyChanged(); } }

        private List<String> _ListKindDate;
        public List<String> ListKindDate { get => _ListKindDate; set { _ListKindDate = value; OnPropertyChanged(); } }

        private DateTime? _SelectedDate;
        public DateTime? SelectedDate { get => _SelectedDate; set { _SelectedDate = value; OnPropertyChanged(); } }

        private String _SelectedKindDate;
        public String SelectedKindDate { get => _SelectedKindDate; set { _SelectedKindDate = value; OnPropertyChanged(); } }



        #endregion

        #region Lọc
        private bool _ReadOnly;
        public bool ReadOnly { get => _ReadOnly; set { _ReadOnly = value; OnPropertyChanged(); } }

        private string _Filter;
        public string Filter { get => _Filter; set { _Filter = value; OnPropertyChanged(); } }

        private List<string> _ListKindObject;
        public List<string> ListKindObject { get => _ListKindObject; set { _ListKindObject = value; OnPropertyChanged(); } }

        private string _SelectedKindObject;
        public string SelectedKindObject
        {
            get => _SelectedKindObject;
            set
            {
                _SelectedKindObject = value;
                OnPropertyChanged();
                if (SelectedKindObject != null && SelectedKindObject != "Tên")
                {
                    Filter = "";
                    ReadOnly = true;
                }
                else
                {
                    ReadOnly = false;
                }
            }
        }

        public void LoadFilter(String kindObject)
        {
            if (SelectedKindObject != null && SelectedKindObject != "Tên")
            {
                LoadInventoryData(true);

                if (kindObject == "Sản phẩm bán chạy nhất")
                {
                    var Max = InventoryList.Max(p => p.CountOutput);
                    var ListFilter = InventoryList.Where(p => p.CountOutput == Max).ToList();
                    InventoryList.Clear();
                    foreach (Inventory item in ListFilter)
                    {
                        InventoryList.Add(item);
                    }
                }
                else if (kindObject == "Sản phẩm ít người mua nhất")
                {
                    var Min = InventoryList.Min(p => p.CountOutput);
                    var ListFilter = InventoryList.Where(p => p.CountOutput == Min).ToList();
                    InventoryList.Clear();
                    foreach (Inventory item in ListFilter)
                    {
                        InventoryList.Add(item);
                    }
                }

            }
            else
            {
                LoadInventoryData(true);
                 
                if (Filter == null)
                {
                    MessageBox.Show("Vui lòng nhập tên sản phẩm!");
                }
                else
                {
                    var ListFilter = InventoryList
                    .Where(p => p.Object.DisplayName.ToLower().Replace(" ", "").Contains(Filter.ToLower().Replace(" ", "")))
                    .ToList();
                    InventoryList.Clear();
                    
                    
                    foreach (Inventory item in ListFilter)
                    {
                        InventoryList.Add(item);
                    }
                    if (ListFilter.Count == 0)
                    {
                        MessageBox.Show("Không có sản phẩm phù hợp!");
                    }
                }
                

            }
        }
        #endregion


        public void LoadInventoryData(bool flag)
        {
            InventoryList = new ObservableCollection<Inventory>();
            Statistics = new Statistic();
            var objectList = DataProvider.Instance.DB.Objects;
            int luongNhap = 0;
            int luongXuat = 0;


            double tongTienTon = 0;
            double tongTienLai = 0;

            var dataInputInfo = DataProvider.Instance.DB.InputInfos;
            var dataOutputInfo = DataProvider.Instance.DB.OutputInfos;

            int i = 1;

            foreach (var item in objectList)
            {
                double tongTienXuat = 0;
                double tongTienNhap = 0;

                var inputList = DataProvider.Instance.DB.InputInfos.Where(r => r.IdObject == item.Id);
                var outputList = DataProvider.Instance.DB.OutputInfos.Where(r => r.IdObject == item.Id);

                int sumInput = 0;
                int sumOutput = 0;

                double tienNhap = 0;
                double tienXuat = 0;
                foreach (var objectItem in dataInputInfo)
                {
                    if (objectItem.IdObject == item.Id)
                    {
                        tienNhap = (double)(objectItem.Count * objectItem.InputPrice);
                        tongTienNhap += tienNhap;
                    }
                }

                foreach (var objectItem in dataOutputInfo)
                {
                    if (objectItem.IdObject == item.Id)
                    {
                        tienXuat = (double)(objectItem.SumPrice);
                        tongTienXuat += tienXuat;
                    }
                }

                if (inputList != null && inputList.Count() != 0)
                {
                    sumInput = (int)inputList.Sum(r => r.Count);
                    tienNhap = (double)inputList.Sum(p => p.InputPrice);

                }
                if (outputList != null && outputList.Count() != 0)
                {
                    sumOutput = (int)outputList.Sum(r => r.Count);
                }

                Inventory inventory = new Inventory();
                inventory.OrdinalNumber = i;
                inventory.Count = sumInput - sumOutput;
                inventory.Object = item;
                inventory.CountInput = sumInput;
                inventory.CountOutput = sumOutput;
                inventory.CountInventory = sumInput - sumOutput;
                inventory.MoneyInput = tongTienNhap;
                inventory.MoneyOutput = tongTienXuat;

                if (tongTienXuat - tongTienNhap > 0)
                {
                    inventory.MoneyEarn = tongTienXuat - tongTienNhap;
                }
                else
                    inventory.MoneyEarn = 0;
                inventory.Object = item;
                InventoryList.Add(inventory);
                i++;

                Statistics.Input += inventory.CountInput;
                Statistics.Output += inventory.CountOutput;
                Statistics.InputPrice += inventory.MoneyInput;
                Statistics.OuputPrice += inventory.MoneyOutput;
                Statistics.Inventory += inventory.CountInventory;
                //Statistics.InventoryPrice += inventory.MoneyInventory;
                Statistics.RevenuePrice += inventory.MoneyEarn;

            }
            if (!flag)
            {
                LoadChart(Statistics);
            }
           




        }
        #region các hàm xử lý chart
        public void LoadChart(Statistic statistic)
        {
            /*================================Test chart====================================*/
            PieChartSeriesCollection = new SeriesCollection();
            DictionaryOfData = new Dictionary<string, int>();
            float tong = (statistic.Inventory + statistic.Output);
            float phanTranHT = (statistic.Inventory / tong) * 100;
            float phanTranHB = 100 - phanTranHT;
            DictionaryOfData.Add("Hàng tồn kho", statistic.Inventory);
            DictionaryOfData.Add("Hàng đã bán", statistic.Output);
            foreach (KeyValuePair<string, int> pair in DictionaryOfData)
            {

                PieChartSeriesCollection.Add(
                    new PieSeries
                    {
                        Title = $"{(pair.Value / tong) * 100}% ({pair.Key})",
                        Values = new ChartValues<int> { pair.Value },
                        DataLabels = false
                    });
            }
            DictionaryOfData.Clear();
            if (InventoryList != null)
            {

                float tongHangCon = statistic.Inventory;
                PieChartSeriesCollectionForSP = new SeriesCollection();
                foreach (Inventory item in InventoryList)
                {
                    DictionaryOfData.Add(item.Object.DisplayName, item.CountInventory);
                }
                foreach (KeyValuePair<string, int> pair in DictionaryOfData)
                {

                    PieChartSeriesCollectionForSP.Add(
                        new PieSeries
                        {
                            Title = $"{(pair.Value / tongHangCon) * 100}% ({pair.Key})",
                            Values = new ChartValues<int> { pair.Value },
                            DataLabels = false

                        });
                }
                VisibilityOfTitle = Visibility.Collapsed;
                Legend = LegendLocation.Bottom;
                if (PieChartSeriesCollectionForSP.Count() > 3)
                {
                    Legend = LegendLocation.None;
                    VisibilityOfTitle = Visibility.Visible;
                }
            }



            /*============================bieu do cot======================================*/

            MaxValue = 7;
            LoadChartByWeek();

            /*=============================================================================*/

        }

        public void LoadChartByYear()
        {
            MaxValue = 12;
            DictionaryOfData = new Dictionary<string, int>();
            CartesianChartSeriesCollection = new SeriesCollection();
            CartesianChartSeriesCollection.Add(
                new ColumnSeries
                {
                    Title = "Tổng hàng bán được",
                    Values = new ChartValues<int> { }
                });

            if (SelectedDate != null)
            {
                var groupByYear = DataProvider.Instance.DB.OutputInfos.Where(p => p.IdOutputNavigation.DateOutput.Value.Year == SelectedDate.Value.Year);
                var groupByMonth = groupByYear.GroupBy(p => p.IdOutputNavigation.DateOutput.Value.Month).
                          Select(pp => new
                          {
                              pp.Key,
                              SUM = pp.Sum(ppp => ppp.Count)
                          }).ToList();
                if (groupByMonth.Count() != 0)
                {
                    for (int i = 1; i <= MaxValue; i++)
                    {
                        bool flag = false;
                        for (int j = 0; j < groupByMonth.Count(); j++)
                        {
                            if (groupByMonth[j].Key == i)
                            {
                                CartesianChartSeriesCollection[0].Values.Add(groupByMonth[j].SUM);
                                flag = true;
                                break;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (flag == false)
                        {
                            CartesianChartSeriesCollection[0].Values.Add(0);
                        }
                    }
                }
            }
            Labels = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };

        }



        public int LoadChartByMonth()
        {

            DictionaryOfData = new Dictionary<string, int>();
            CartesianChartSeriesCollection = new SeriesCollection();
            CartesianChartSeriesCollection.Add(
                new ColumnSeries
                {
                    Title = "Tổng hàng bán được",
                    Values = new ChartValues<int> { }
                });

            #region tinh day of month
            Labels = new string[32];

            for (int i = 0; i < 31; i++)
            {
                Labels[i] = ("Ngày " + (i + 1));
            }
            if (DateTime.DaysInMonth(SelectedDate.Value.Year, SelectedDate.Value.Month).Equals(31))
            {
                MaxValue = 31;
            }
            else if (DateTime.DaysInMonth(SelectedDate.Value.Year, SelectedDate.Value.Month).Equals(30))
            {
                MaxValue = 30;
            }
            else if (DateTime.DaysInMonth(SelectedDate.Value.Year, SelectedDate.Value.Month).Equals(29))
            {
                MaxValue = 29;
            }
            else
            {
                MaxValue = 28;
            }
            #endregion

            if (SelectedDate != null)
            {
                var groupByYear = DataProvider.Instance.DB.OutputInfos.Where(p => p.IdOutputNavigation.DateOutput.Value.Year == SelectedDate.Value.Year);
                var groupByMonth = groupByYear.Where(p => p.IdOutputNavigation.DateOutput.Value.Month == SelectedDate.Value.Month);
                var groupByDay = groupByMonth.GroupBy(p => p.IdOutputNavigation.DateOutput.Value.Day).
                          Select(pp => new
                          {
                              pp.Key,
                              SUM = pp.Sum(ppp => ppp.Count)
                          }).ToList();
                if (groupByDay.Count() != 0)
                {
                    for (int i = 1; i <= MaxValue; i++)
                    {
                        bool flag = false;
                        for (int j = 0; j < groupByDay.Count(); j++)
                        {
                            if (groupByDay[j].Key == i)
                            {
                                CartesianChartSeriesCollection[0].Values.Add(groupByDay[j].SUM);
                                flag = true;
                                break;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (flag == false)
                        {
                            CartesianChartSeriesCollection[0].Values.Add(0);
                        }
                    }
                }



            }
            return MaxValue;
            //Labels = new[] { "Ngày 1", "Ngày 2", "Ngày 3", "Ngày 4", "Ngày 5", "Ngày 6", "Ngày 7", "Ngày 8", "Ngày 9", "Ngày 10", "Ngày 11", "Ngày 12", "Ngày 13", "Ngày 14", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12", "Ngày 12" };
        }

        public void LoadChartByWeek()
        {
            SelectedDate = DateTime.Now;
            SelectedKindDate = "Tuần";
            //string[] weekDays = new CultureInfo("en-us").DateTimeFormat.DayNames.;
            //swap<string>(weekDays, 0, weekDays.Length - 1);
            //swap<string>(weekDays, 0, weekDays.Length - 1);
            string[] weekDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            DictionaryOfData = new Dictionary<string, int>();
            CartesianChartSeriesCollection = new SeriesCollection();
            CartesianChartSeriesCollection.Add(
                new ColumnSeries
                {
                    Title = "Tổng hàng bán được",
                    Values = new ChartValues<int> { }
                });

            if (SelectedDate != null)
            {
                var groupByYear = DataProvider.Instance.DB.OutputInfos.Where(p => p.IdOutputNavigation.DateOutput.Value.Year == SelectedDate.Value.Year);
                var groupByMonth = groupByYear.Where(p => p.IdOutputNavigation.DateOutput.Value.Month == SelectedDate.Value.Month);

                var monday = SelectedDate.Value.AddDays(-(int)SelectedDate.Value.DayOfWeek + (int)DayOfWeek.Monday);
                var sunday = SelectedDate.Value.AddDays(7 - (int)SelectedDate.Value.DayOfWeek);

                var groupByDay = groupByMonth.Where(p => (p.IdOutputNavigation.DateOutput.Value >= monday) && (p.IdOutputNavigation.DateOutput.Value <= sunday)).ToList();
                var groupByWeek = groupByDay.GroupBy(p => p.IdOutputNavigation.DateOutput.Value.DayOfWeek).
                Select(pp => new
                {
                    pp.Key,
                    SUM = pp.Sum(ppp => ppp.Count)
                }).ToList();
                if (groupByWeek.Count() != 0)
                {
                    for (int i = 0; i < weekDays.Length; i++)
                    {
                        bool flag = false;
                        for (int j = 0; j < groupByWeek.Count(); j++)
                        {
                            if (groupByWeek[j].Key.ToString() == weekDays[i])
                            {
                                CartesianChartSeriesCollection[0].Values.Add(groupByWeek[j].SUM);
                                flag = true;
                                break;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (flag == false)
                        {
                            CartesianChartSeriesCollection[0].Values.Add(0);
                        }
                    }
                }
                Labels = new[] { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            }
        }


        public void LoadChartByDay()
        {
            string[] weekDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            DictionaryOfData = new Dictionary<string, int>();
            CartesianChartSeriesCollection = new SeriesCollection();

            if (SelectedDate != null)
            {
                var groupByYear = DataProvider.Instance.DB.OutputInfos.Where(p => p.IdOutputNavigation.DateOutput.Value.Year == SelectedDate.Value.Year);
                var groupByMonth = groupByYear.Where(p => p.IdOutputNavigation.DateOutput.Value.Month == SelectedDate.Value.Month);
                var groupByDay = groupByMonth.Where(p => p.IdOutputNavigation.DateOutput.Value.Day == SelectedDate.Value.Day);
                var groupByObjectName = groupByDay.GroupBy(p => p.IdObjectNavigation.DisplayName).
                Select(pp => new
                {
                    pp.Key,
                    SUM = pp.Sum(ppp => ppp.Count)
                }).ToList();
                Labels = new String[groupByObjectName.Count];
                groupByObjectName = groupByObjectName.OrderBy(o => o.SUM).ToList();
                if (groupByObjectName.Count != 0)
                {
                    for (int i = 0; i < groupByObjectName.Count; i++)
                    {
                        CartesianChartSeriesCollection.Add(
                        new ColumnSeries
                        {
                            Title = groupByObjectName[i].Key,
                            Values = new ChartValues<int> { }
                        });
                        CartesianChartSeriesCollection[i].Values.Add(groupByObjectName[i].SUM);
                        //Labels[i] = groupByObjectName[i].Key;
                    }
                }

            }
        }
        #endregion




    }
}
