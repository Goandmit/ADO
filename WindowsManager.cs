using System;
using System.Collections.Generic;
using System.Windows;

namespace ADO
{
    public static class WindowsManager
    {        
        private static int purchaseFormIndex = 0;        

        public static CustomersListVM CurrentCustomersListVM { get; set; }
        public static PurchasesListVM CurrentPurchasesListVM { get; set; }
        public static CustomerFormVM CurrentCustomerFormVM { get; set; }

        public static void ShowConnectionsForm()
        {
            ManagerSql.UpdateConnectionStatus();
            ManagerOleDB.UpdateConnectionStatus();

            List<string> connectionInfo = new List<string>
            {
                ManagerSql.ConnectionStringMSSQL,
                ManagerSql.ConnectionStatusMSSQL,
                ManagerOleDB.ConnectionStringMSAccess,
                ManagerOleDB.ConnectionStatusMSAccess
            };

            ConnectionsForm connectionsForm = new ConnectionsForm
            {
                DataContext = new ConnectionsFormVM(connectionInfo)
            };

            connectionsForm.Show();

            foreach (Window window in App.Current.Windows)
            {
                if (window is MenuForm)
                {
                    connectionsForm.Owner = window;
                    break;
                }
            }

            connectionsForm.Closed += SubordinateWindow_Closed;
        }

        public static void SubordinateWindow_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is MenuForm)
                {
                    window.Activate();
                    break;
                }
            }
        }

        #region Customers

        public static void ShowCustomersList()
        {
            bool alreadyOpen = false;

            foreach (Window window in App.Current.Windows)
            {
                if (window is CustomersList)
                {
                    alreadyOpen = true;
                    break;
                }
            }

            if (alreadyOpen == false)
            {
                CustomersList customersList = new CustomersList
                {
                    DataContext = new CustomersListVM()
                };

                customersList.Show();

                foreach (Window window in App.Current.Windows)
                {
                    if (window is MenuForm)
                    {
                        customersList.Owner = window;
                        break;
                    }
                }

                customersList.Closed += SubordinateWindow_Closed;
            }            
        }

        public static void ShowNewCustomerForm()
        {
            bool alreadyOpen = false;

            foreach (Window window in App.Current.Windows)
            {
                if (window is CustomerForm)
                {
                    alreadyOpen = true;
                    break;
                }
            }

            if (alreadyOpen == false)
            {
                CustomerForm customerForm = new CustomerForm();
                string windowName = "CustomerForm";
                customerForm.DataContext = new CustomerFormVM(windowName);
                customerForm.Show();

                customerForm.Name = windowName;
                customerForm.Closed += CustomerForm_Closed;
            }            
        }

        public static void ShowCustomerForm(string id)
        {
            bool alreadyOpen = false;

            foreach (Window window in App.Current.Windows)
            {
                if (window is CustomerForm)
                {
                    alreadyOpen = true;
                    break;
                }
            }

            if (alreadyOpen == false)
            {
                CustomerForm customerForm = new CustomerForm();
                Customer customer = ManagerSql.GetCustomer(id);
                string windowName = "CustomerForm";
                customerForm.DataContext = new CustomerFormVM(customer, windowName);
                customerForm.Show();

                customerForm.Name = windowName;
                customerForm.Closed += CustomerForm_Closed;
            }           
        }

        public static void DeleteCustomer(string id)
        {
            ManagerSql.DeleteCustomer(id);
            
            CurrentCustomersListVM.UpdateCustomers();
        }

        public static void CustomerForm_Closed(object sender, EventArgs e)
        {
            CurrentCustomersListVM.UpdateCustomers();
        }

        #endregion

        #region Purchases

        public static void ShowPurchasesList()
        {
            bool alreadyOpen = false;

            foreach (Window window in App.Current.Windows)
            {
                if (window is PurchasesList)
                {
                    alreadyOpen = true;
                    break;
                }
            }

            if (alreadyOpen == false)
            {
                string windowName = $"PurchasesList";

                PurchasesList purchasesList = new PurchasesList
                {
                    DataContext = new PurchasesListVM(windowName)
                };

                purchasesList.Show();
                purchasesList.Name = windowName;

                foreach (Window window in App.Current.Windows)
                {
                    if (window is MenuForm)
                    {
                        purchasesList.Owner = window;
                        break;
                    }
                }

                purchasesList.Closed += SubordinateWindow_Closed;
            }           
        }

        public static void ShowNewPurchaseForm(string email, string ownerName)
        {
            PurchaseForm purchaseForm = new PurchaseForm();
            string windowName = $"PurchaseForm{++purchaseFormIndex}";
            purchaseForm.DataContext = new PurchaseFormVM(email, windowName);
            purchaseForm.Show();

            purchaseForm.Name = windowName;           
            purchaseForm.Closed += PurchaseForm_Closed;

            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == ownerName)
                {
                    purchaseForm.Owner = window;
                }
            }
        }

        public static void ShowPurchaseForm(string id, string ownerName)
        {
            PurchaseForm purchaseForm = new PurchaseForm();
            Purchase purchase = ManagerOleDB.GetPurchase(id);
            string windowName = $"PurchaseForm{++purchaseFormIndex}";
            purchaseForm.DataContext = new PurchaseFormVM(purchase, windowName);
            purchaseForm.Show();

            purchaseForm.Name = windowName;            
            purchaseForm.Closed += PurchaseForm_Closed;

            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == ownerName)
                {
                    purchaseForm.Owner = window;
                }
            }
        }

        public static void DeletePurchase(string id)
        {
            ManagerOleDB.DeletePurchase(id);

            if (CurrentPurchasesListVM != null)
            {
                CurrentPurchasesListVM.UpdatePurchases();
            }

            if (CurrentCustomerFormVM != null)
            {
                CurrentCustomerFormVM.UpdatePurchases();
            }
        }

        public static void PurchaseForm_Closed(object sender, EventArgs e)
        {
            if (purchaseFormIndex > 0)
            {
                purchaseFormIndex--;
            }

            if (CurrentPurchasesListVM != null)
            {
                CurrentPurchasesListVM.UpdatePurchases();
            }

            if (CurrentCustomerFormVM != null)
            {
                CurrentCustomerFormVM.UpdatePurchases();
            }
        }        

        #endregion

        public static void ShowErrorMessageBox(string text)
        {
            MessageBox.Show(text,
                        "Операция не выполнена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK,
                        MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
