using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ADO
{
    public class MenuFormVM
    {
        public MenuFormVM()
        {
            ManagerSql.ConnectToDB();
            ManagerOleDB.ConnectToDB();
        }        

        private RelayCommand showConnectionsCommand;
        public RelayCommand ShowConnectionsCommand
        {
            get
            {
                return showConnectionsCommand ??
                  (showConnectionsCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowConnectionsForm();
                  }));
            }
        }

        private RelayCommand showCustomersCommand;
        public RelayCommand ShowCustomersCommand
        {
            get
            {
                return showCustomersCommand ??
                  (showCustomersCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowCustomersList();
                  }));
            }
        }

        private RelayCommand showPurchasesCommand;
        public RelayCommand ShowPurchasesCommand
        {
            get
            {
                return showPurchasesCommand ??
                  (showPurchasesCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowPurchasesList();
                  }));
            }
        }
    }

    public class ConnectionsFormVM
    {
        public string ConnectionStringMSSQL { get; set; }
        public string ConnectionStatusMSSQL { get; set; }
        public string ConnectionStringMSAccess { get; set; }
        public string ConnectionStatusMSAccess { get; set; }

        public ConnectionsFormVM(List<string> connectionInfo)
        {
            if (connectionInfo != null)
            {
                if (connectionInfo.Count == 4)
                {
                    ConnectionStringMSSQL = connectionInfo[0];
                    ConnectionStatusMSSQL = connectionInfo[1];
                    ConnectionStringMSAccess = connectionInfo[2];
                    ConnectionStatusMSAccess = connectionInfo[3];
                }
            }
        }
    }

    public class CustomersListVM
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public Customer SelectedCustomer { get; set; }

        private void FillCustomers()            
        {
            List<Customer> clients = ManagerSql.GetCustomers();

            foreach (Customer client in clients)
            {
                Customers.Add(client);
            }            
        }

        public void UpdateCustomers()
        {
            Customers.Clear();
            FillCustomers();
        }

        public CustomersListVM()
        {          
            Customers = new ObservableCollection<Customer>();
            FillCustomers();

            WindowsManager.CurrentCustomersListVM = this;
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowNewCustomerForm();
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedCustomer}"))
                      {
                          WindowsManager.DeleteCustomer(SelectedCustomer.ID);
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedCustomer}"))
                      {
                          WindowsManager.ShowCustomerForm(SelectedCustomer.ID);
                      }
                  }));
            }
        }
    }

    public class CustomerFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<Purchase> Purchases { get; set; }
        public Purchase SelectedPurchase { get; set; }

        private void FillPurchases()
        {
            List<Purchase> purchases = ManagerOleDB.GetPurchasesByEmail(Email);

            foreach (Purchase purchase in purchases)
            {
                Purchases.Add(purchase);
            }
        }

        public void UpdatePurchases()
        {
            Purchases.Clear();
            FillPurchases();
        }

        private string id;
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged($"{nameof(ID)}");
            }
        }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string WindowName { get; set; }

        public CustomerFormVM(Customer customer, string windowName)
        {
            ID = customer.ID;
            Surname = customer.Surname;
            Name = customer.Name;
            Patronymic = customer.Patronymic;
            PhoneNumber = customer.PhoneNumber;
            Email = customer.Email;

            WindowName = windowName;

            Purchases = new ObservableCollection<Purchase>();
            FillPurchases();

            WindowsManager.CurrentCustomerFormVM = this;
        }

        public CustomerFormVM(string windowName)
        {
            WindowName = windowName;

            Purchases = new ObservableCollection<Purchase>();

            WindowsManager.CurrentCustomerFormVM = this;
        }

        private string EliminateNull(string userInput)
        {
            userInput = (userInput == null) ? String.Empty : userInput.Trim();

            return userInput;
        }

        private void PrepareFields()
        {
            ID = EliminateNull(ID);
            Surname = EliminateNull(Surname);
            Name = EliminateNull(Name);
            Patronymic = EliminateNull(Patronymic);
            PhoneNumber = EliminateNull(PhoneNumber);
            Email = EliminateNull(Email);
        }

        private bool CheckFields()
        {
            bool status = true;

            PrepareFields();

            if (Surname.Length == 0 || Name.Length == 0 ||
                        Patronymic.Length == 0 || Email.Length == 0)
            {
                status = false;
                WindowsManager.ShowErrorMessageBox("Пустым может быть только поле \"Номер телефона\"");
            }

            return status;
        }

        private Customer GetCustomerFromForm()
        {
            Customer customer = new Customer(ID, Surname, Name, Patronymic, PhoneNumber, Email);

            return customer;
        }

        private void EditOrAddCustomer()
        {
            Customer customer = GetCustomerFromForm();

            if (!string.IsNullOrEmpty($"{customer.ID}"))
            {
                ManagerSql.EditCustomer(customer);
            }
            else
            {
                ID = ManagerSql.AddCustomer(customer);
            }
        }

        private RelayCommand writeCommand;
        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand ??
                  (writeCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckFields();

                      if (status == true)
                      {
                          EditOrAddCustomer();
                      }
                  }));
            }
        }

        private RelayCommand okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return okCommand ??
                  (okCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckFields();

                      if (status == true)
                      {
                          EditOrAddCustomer();

                          foreach (Window window in App.Current.Windows)
                          {
                              if (window.Name == WindowName)
                              {
                                  window.Close();
                              }
                          }
                      }
                  }));
            }
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowNewPurchaseForm(Email, WindowName);
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.DeletePurchase(SelectedPurchase.ID);                          
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.ShowPurchaseForm(SelectedPurchase.ID, WindowName);
                      }
                  }));
            }
        }
    }

    public class PurchasesListVM
    {
        public ObservableCollection<Purchase> Purchases { get; set; }
        public Purchase SelectedPurchase { get; set; }

        public string WindowName { get; set; }

        private void FillPurchases()            
        {
            List<Purchase> purchases = ManagerOleDB.GetPurchases();

            foreach (Purchase purchase in purchases)
            {
                Purchases.Add(purchase);
            }
        }

        public void UpdatePurchases()
        {
            Purchases.Clear();
            FillPurchases();
        }

        public PurchasesListVM(string windowName)
        {
            Purchases = new ObservableCollection<Purchase>();
            FillPurchases();

            WindowName = windowName;

            WindowsManager.CurrentPurchasesListVM = this;
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowNewPurchaseForm(String.Empty, WindowName);
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.DeletePurchase(SelectedPurchase.ID);
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.ShowPurchaseForm(SelectedPurchase.ID, WindowName);
                      }
                  }));
            }
        }
    }

    public class PurchaseFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string id;
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged($"{nameof(ID)}");
            }
        }

        public string Email { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string WindowName { get; set; }
        public bool EmailIsEnabled { get; set; }

        public PurchaseFormVM(Purchase purchase, string windowName)
        {
            ID = purchase.ID;
            Email = purchase.Email;
            ProductCode = purchase.ProductCode;
            ProductName = purchase.ProductName;

            WindowName = windowName;
            EmailIsEnabled = true;
        }

        public PurchaseFormVM(string email, string windowName)
        {
            WindowName = windowName;           

            if (!string.IsNullOrEmpty($"{email}"))
            {
                Email = email;
                EmailIsEnabled = false;
            }
            else
            {
                EmailIsEnabled = true;
            }
        }

        private string EliminateNull(string userInput)
        {
            userInput = (userInput == null) ? String.Empty : userInput.Trim();

            return userInput;
        }

        private void PrepareFields()
        {
            ID = EliminateNull(ID);
            Email = EliminateNull(Email);
            ProductCode = EliminateNull(ProductCode);
            ProductName = EliminateNull(ProductName);
        }

        private bool CheckFields()
        {
            bool status = true;

            PrepareFields();

            if (Email.Length == 0 || ProductCode.Length == 0 ||
                        ProductName.Length == 0)
            {
                status = false;
                WindowsManager.ShowErrorMessageBox("Все поля должны быть заполнены");
            }

            return status;
        }

        private Purchase GetPurchaseFromForm()
        {
            Purchase purchase = new Purchase(ID, Email, ProductCode, ProductName);

            return purchase;
        }

        private void EditOrAddPurchase()
        {
            Purchase purchase = GetPurchaseFromForm();

            if (!string.IsNullOrEmpty($"{purchase.ID}"))
            {
                ManagerOleDB.EditPurchase(purchase);
            }
            else
            {
                ID = ManagerOleDB.AddPurchase(purchase);
            }
        }

        private RelayCommand writeCommand;
        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand ??
                  (writeCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckFields();

                      if (status == true)
                      {
                          EditOrAddPurchase();
                      }
                  }));
            }
        }

        private RelayCommand okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return okCommand ??
                  (okCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckFields();

                      if (status == true)
                      {
                          EditOrAddPurchase();

                          foreach (Window window in App.Current.Windows)
                          {
                              if (window.Name == WindowName)
                              {
                                  window.Close();
                              }
                          }
                      }
                  }));
            }
        }
    }
}
