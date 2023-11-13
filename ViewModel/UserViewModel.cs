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
        //private ObservableCollection<User> _List;
        //public ObservableCollection<User> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        //private ObservableCollection<UserRole> _ListUserRole;
        //public ObservableCollection<UserRole> ListUserRole { get => _ListUserRole; set { _ListUserRole = value; OnPropertyChanged(); } }

        //private User _SelectedItem;
        //public User SelectedItem
        //{
        //    get => _SelectedItem; set
        //    {
        //        _SelectedItem = value;
        //        OnPropertyChanged();
        //        if (SelectedItem != null)
        //        {
        //            DisplayName = SelectedItem.DisplayName;
        //            UserName = SelectedItem.UserName;
        //            SelectedUserRole = SelectedItem.IdRoleNavigation;
        //            SelectedUserRoleText = SelectedItem.IdRoleNavigation.DisplayName;
        //        }
        //    }
        //}

        //private string _SelectedUserRoleText;
        //public string SelectedUserRoleText { get => _SelectedUserRoleText; set { _SelectedUserRoleText = value; OnPropertyChanged(); } }

        //private string _DisplayName;
        //public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        //private string _UserName;
        //public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        //private string _Note;
        //public string Note { get => _Note; set { _Note = value; OnPropertyChanged(); } }


        //private UserRole _SelectedUserRole;
        //public UserRole SelectedUserRole { get => _SelectedUserRole; set { _SelectedUserRole = value; OnPropertyChanged(); } }

        //private int _CurrentRole;
        //public int CurrentRole { get => _CurrentRole; set { _CurrentRole = value; OnPropertyChanged(); } }

        //private MainWindow _MainWindowTemp;
        //public MainWindow MainWindowTemp { get => _MainWindowTemp; set { _MainWindowTemp = value; OnPropertyChanged(); } }

        //private bool _ValidateErrorDisplayName;
        //public bool ValidateErrorDisplayName { get => _ValidateErrorDisplayName; set { _ValidateErrorDisplayName = value; OnPropertyChanged(); } }

        //private bool _ValidateErrorUserName;
        //public bool ValidateErrorUserName { get => _ValidateErrorUserName; set { _ValidateErrorUserName = value; OnPropertyChanged(); } }


        ///*Chức năng nút*/
        //public ICommand AddCommand { get; set; }
        //public ICommand EditCommand { get; set; }
        //public ICommand DeleteCommand { get; set; }
        //public ICommand ChangePasswordCommand { get; set; }

        //public ICommand ImageCommand { get; set; }

        //public ICommand CloseImageCommand { get; set; }

        //public UserViewModel()
        //{
        //    string PassWordDefault = "password";
        //    string PassWordDefaultEncode;
        //    var ListUser = DataProvider.Instance.DB.Users;
        //    LoadAndReset(ListUser, PassWordDefaultEncode);
        //    var ListUserRoles = DataProvider.Instance.DB.UserRoles;
        //    if (ListUserRoles != null)
        //    {
        //        ListUserRole = new ObservableCollection<UserRole>(ListUserRoles);



        //    }



        //    AddCommand = new RelayCommand<ListView>((p) => {
        //        if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(UserName) || SelectedUserRole == null || ValidateErrorUserName == true || ValidateErrorDisplayName == true)
        //        {
        //            return false;
        //        }

        //        return true;
        //    }, (p) => {
        //        Boolean flag = true;
        //        foreach (var item in List)
        //        {
        //            if (UserName == item.UserName)
        //            {
        //                MessageBox.Show("Tên đăng nhập đã tồn tại! Vui lòng nhập tên khác.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        //                flag = false;
        //                break;
        //            }

        //        }
        //        if (flag)
        //        {
        //            var user = DataProvider.Instance.DB.Users.Add(new User()
        //            {
        //                DisplayName = DisplayName,
        //                IDRole = SelectedUserRole.ID,
        //                UserName = UserName,
        //                PassWord = PassWordDefaultEncode,
        //                Image = FileName,
        //                Note = "* Mật khẩu mặc định là \"password\". Nhấn đổi mật khẩu để thay đổi."
        //            });
        //            DataProvider.Instance.DB.SaveChanges();
        //            List.Add(user);

        //            var temp = List[List.IndexOf(user)];
        //            List[List.IndexOf(user)] = List[0];
        //            List[0] = temp;
        //            p.SelectedItem = List[List.IndexOf(user)];
        //        }
        //    });

        //    EditCommand = new RelayCommand<ListView>((p) =>
        //    {
        //        if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(UserName) || SelectedUserRole == null || ValidateErrorUserName == true || ValidateErrorDisplayName == true || SelectedItem == null)
        //        {
        //            return false;
        //        }
        //        var displayList = DataProvider.Instance.DB.Users.Where(x => x.ID == SelectedItem.ID);
        //        if (displayList == null || displayList.Count() != 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }, (p) =>
        //    {
        //        string notMessage = "* Mật khẩu mặc định là \"password\". Nhấn đổi mật khẩu để thay đổi.";
        //        var user = DataProvider.Instance.DB.Users.Where(pp => pp.ID == SelectedItem.ID).SingleOrDefault();
        //        user.DisplayName = DisplayName;
        //        user.UserName = UserName;
        //        user.UserRole = SelectedUserRole;
        //        user.PassWord = SelectedItem.PassWord;
        //        user.Image = FileName;
        //        user.Note = notMessage;
        //        if (SelectedItem.PassWord != PassWordDefaultEncode)
        //        {
        //            notMessage = null;
        //            user.Note = null;
        //        }
        //        DataProvider.Instance.DB.SaveChanges();
        //        for (int i = 0; i < List.Count(); i++)
        //        {
        //            if (List[i].ID == SelectedItem.ID)
        //            {
        //                List[i] = new User() { ID = SelectedItem.ID, DisplayName = DisplayName, UserName = UserName, UserRole = SelectedUserRole, PassWord = SelectedItem.PassWord, Note = notMessage, Image = FileName };
        //                SelectedItem = List[i];
        //                p.SelectedItem = SelectedItem;
        //                break;
        //            }
        //        }
        //        MessageBox.Show("Dữ liệu đã được sửa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        //    });


        //    DeleteCommand = new RelayCommand<object>((p) =>
        //    {
        //        CurrentRole = Globals.IDUserRole;
        //        var userAll = List;
        //        var checkLastManager = DataProvider.Instance.DB.Users.Where(x => (x.UserRole.ID == 1) && (x.DisplayName != null)).ToList();
        //        var checkLastAdmin = DataProvider.Instance.DB.Users.Where(x => (x.UserRole.ID == 4) && (x.DisplayName != null)).ToList();

        //        if (SelectedItem == null)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            if (checkLastAdmin != null)
        //            {
        //                if (checkLastAdmin.Count == 1 && SelectedItem.ID == checkLastAdmin[0].ID)
        //                {
        //                    return false;
        //                }
        //            }
        //            if (checkLastManager != null)
        //            {
        //                if (checkLastManager.Count == 1 && SelectedItem.ID == checkLastManager[0].ID)
        //                {
        //                    if (CurrentRole == 4)
        //                        return true;
        //                    return false;
        //                }
        //            }
        //        }

        //        return true;
        //    }, (p) =>
        //    {
        //        if (MessageBox.Show("Bạn có chắc muốn xóa dữ liệu này?",
        //                    "Cautions", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
        //        {

        //            var user = DataProvider.Instance.DB.Users.Where(x => x.ID == SelectedItem.ID).SingleOrDefault();

        //            user.DisplayName = null;
        //            DataProvider.Instance.DB.SaveChanges();
        //            foreach (var item in List)
        //            {
        //                if (item.ID == user.ID)
        //                {
        //                    List.Remove(item);
        //                    break;
        //                }

        //            }
        //            //List.Remove(user);
        //            MessageBox.Show("Xóa thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        SelectedItem = null;
        //    });

        //    //ChangePasswordCommand = new RelayCommand<Window>((p) => {
        //    //    if (SelectedItem == null)
        //    //        return false;
        //    //    return true;
        //    //}, (p) => {
        //    //    ChangePasswordWindow window = new ChangePasswordWindow();

        //    //    var dataContext = window.DataContext as ChangePasswordViewModel;
        //    //    dataContext.SelectedItem = SelectedItem;
        //    //    window.ShowDialog();
        //    //    ListUser = DataProvider.Instance.DB.Users;
        //    //    LoadAndReset(ListUser, PassWordDefaultEncode);

        //    //});

        //}

        ////public void LoadAndReset(User ListUser, string PassWordDefaultEncode)
        ////{
        ////    if (ListUser != null)
        ////    {
        ////        List = new ObservableCollection<User>(ListUser);
        ////        for (int i = 0; i < List.Count(); i++)
        ////        {
        ////            if (List[i].DisplayName == null)
        ////            {
        ////                List.Remove(List[i]);
        ////                i--;
        ////            }

        ////        }
        ////        for (int i = 0; i < List.Count(); i++)
        ////        {
        ////            if (List[i].PassWord == PassWordDefaultEncode)
        ////            {
        ////                List[i].Note = "* Mật khẩu mặc định là \"password\". Nhấn đổi mật khẩu để thay đổi.";
        ////            }
        ////            else
        ////            {
        ////                List[i].Note = "";
        ////            }
        ////        }
        ////    }
        ////}

       

    }
}
