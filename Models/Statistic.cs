using ProjectPRN221.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN221.Models
{
    public class Statistic : BaseViewModel
    {
        private Object _Object;
        public Object Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private int ordinalNumbers;
        public int OrdinalNumbers { get => ordinalNumbers; set { ordinalNumbers = value; OnPropertyChanged(); } }

        private int input;
        public int Input { get => input; set { input = value; OnPropertyChanged(); } }

        private int output;
        public int Output { get => output; set { output = value; OnPropertyChanged(); } }

        private int inventory;
        public int Inventory { get => inventory; set { inventory = value; OnPropertyChanged(); } }

        private double inputPrice;
        public double InputPrice { get => inputPrice; set { inputPrice = value; OnPropertyChanged(); } }

        private double ouputPrice;
        public double OuputPrice { get => ouputPrice; set { ouputPrice = value; OnPropertyChanged(); } }

        private double inventoryPrice;
        public double InventoryPrice { get => inventoryPrice; set { inventoryPrice = value; OnPropertyChanged(); } }

        private double revenuePrice;
        public double RevenuePrice { get => revenuePrice; set { revenuePrice = value; OnPropertyChanged(); } }
    }
}
