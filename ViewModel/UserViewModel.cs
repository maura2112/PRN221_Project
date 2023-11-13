using Microsoft.Win32;
using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace ProjectPRN221.ViewModel
{
    public class UserViewModel : BaseViewModel
    {
        private ObservableCollection<User> _List;
        public ObservableCollection<User> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<UserRole> _ListUserRole;
        public ObservableCollection<UserRole> ListUserRole { get => _ListUserRole; set { _ListUserRole = value; OnPropertyChanged(); } }

        private User _SelectedItem;
        public User SelectedItem
        {
            get => _SelectedItem; set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    UserName = SelectedItem.UserName;
                    SelectedUserRole = SelectedItem.IdRoleNavigation;
                    //SelectedUserRoleText = SelectedItem.IdRoleNavigation.DisplayName;
                }
            }
        }

        private string _SelectedUserRoleText;
        public string SelectedUserRoleText { get => _SelectedUserRoleText; set { _SelectedUserRoleText = value; OnPropertyChanged(); } }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _Note;
        public string Note { get => _Note; set { _Note = value; OnPropertyChanged(); } }


        private UserRole _SelectedUserRole;
        public UserRole SelectedUserRole { get => _SelectedUserRole; set { _SelectedUserRole = value; OnPropertyChanged(); } }

        private int _CurrentRole;
        public int CurrentRole { get => _CurrentRole; set { _CurrentRole = value; OnPropertyChanged(); } }

        private MainWindow _MainWindowTemp;
        public MainWindow MainWindowTemp { get => _MainWindowTemp; set { _MainWindowTemp = value; OnPropertyChanged(); } }


        /*Chức năng nút*/
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public ICommand ImageCommand { get; set; }

        public ICommand CloseImageCommand { get; set; }

        public UserViewModel()
        {

            string PassWordDefault = "password";
            string PassWordDefaultEncode;
            List = new ObservableCollection<User>(DataProvider.Instance.DB.Users);
            var ListUserRoles = DataProvider.Instance.DB.UserRoles;
            if (ListUserRoles != null)
            {
                ListUserRole = new ObservableCollection<UserRole>(ListUserRoles);
            }



            AddCommand = new RelayCommand<ListView>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(UserName) || SelectedUserRole == null)
                {
                    return false;
                }

                return true;
            }, (p) =>
            {
                Boolean flag = true;
                foreach (var item in List)
                {
                    if (UserName == item.UserName)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại!");
                        flag = false;
                        break;
                    }

                }
                if (flag)
                {
                    var user = new User()
                    {
                        DisplayName = DisplayName,
                        IdRole = SelectedUserRole.Id,
                        UserName = UserName,
                        Password = PassWordDefault
                        
                    };
                    DataProvider.Instance.DB.Users.Add(user);
                    DataProvider.Instance.DB.SaveChanges();
                    List.Add(user);

                    var temp = List[List.IndexOf(user)];
                    List[List.IndexOf(user)] = List[0];
                    List[0] = temp;
                    //p.SelectedItem = List[List.IndexOf(user)];
                    MessageBox.Show("Thêm người dùng thành công, mật khẩu mặc định là 'password' ");
                }
            });

            EditCommand = new RelayCommand<ListView>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(UserName) || SelectedUserRole == null || SelectedItem == null)
                {
                    return false;
                }
                var displayList = DataProvider.Instance.DB.Users.Where(x => x.Id == SelectedItem.Id);
                if (displayList == null || displayList.Count() != 0)
                {
                    return true;
                }
                return false;
            }, (p) =>
            {
                var user = DataProvider.Instance.DB.Users.Where(pp => pp.Id == SelectedItem.Id).SingleOrDefault();
                user.DisplayName = DisplayName;
                user.UserName = UserName;
                user.IdRoleNavigation = SelectedUserRole;

                DataProvider.Instance.DB.SaveChanges();
                for (int i = 0; i < List.Count(); i++)
                {
                    if (List[i].Id == SelectedItem.Id)
                    {
                        List[i] = new User()
                        {
                            Id = SelectedItem.Id,
                            DisplayName = DisplayName,
                            UserName = UserName,
                            IdRoleNavigation = SelectedUserRole
                        };
                        SelectedItem = List[i];
                        //p.SelectedItem = SelectedItem;
                        break;
                    }
                }
                MessageBox.Show("Sửa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (MessageBox.Show("Bạn có chắc muốn xóa người dùng này?",
                            "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {

                    var user = DataProvider.Instance.DB.Users.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();

                    user.DisplayName = null;
                    DataProvider.Instance.DB.SaveChanges();
                    foreach (var item in List)
                    {
                        if (item.Id == user.Id)
                        {
                            List.Remove(item);
                            DataProvider.Instance.DB.Users.Remove(item);
                            DataProvider.Instance.DB.SaveChanges();
                            break;
                        }

                    }
                    List.Remove(user);
                    MessageBox.Show("Xóa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                SelectedItem = null;
            });



        }





    }
}
