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
        private ObservableCollection<Inventory> inventoryList;
        public ObservableCollection<Inventory> InventoryList { 
            get => inventoryList;
            set
            {
                inventoryList = value;
                OnPropertyChanged();
            }
        }
        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand UnitCommand { get; set; }
        public ICommand SuplierCommand { get; set; }
        public ICommand CustomerCommand { get; set; }
        public ICommand ObjectCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand InputCommand { get; set; }
        public ICommand OutputCommand { get; set; }

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
                    LoadInventoryData();
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


        }

        void LoadInventoryData()
        {
            InventoryList = new ObservableCollection<Inventory>();
            var objectList = DataProvider.Instance.DB.Objects.ToList();
            int i = 1;

            foreach (var item in objectList)
            {
                var inputList = DataProvider.Instance.DB.InputInfos.Where(r => r.IdObject == item.Id);
                var outputList = DataProvider.Instance.DB.OutputInfos.Where(r => r.IdObject == item.Id);

                int sumInput = 0;
                int sumOutput = 0;
                
                if(inputList != null)
                {
                    sumInput = (int)inputList.Sum(r => r.Count);

                }
                if(outputList != null)
                {
                    sumOutput = (int) outputList.Sum(r => r.Count);
                }

                Inventory inventory = new Inventory();
                inventory.OrdinalNumber = i;
                inventory.Count = sumInput - sumOutput;
                inventory.Object = item;

                InventoryList.Add(inventory);

                i++;

            }
            
        }
    }
}
