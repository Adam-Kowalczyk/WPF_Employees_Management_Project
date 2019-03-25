using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfZAD2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Employee> collection { get; set; }
        public MainWindow()
        {
            InitializeComponent();

        }
        public string filePath;
        public bool hasChanged;

        static public Window1 addWindow;

        private void SaveToFile()
        {
            if (collection == null) return;

            List<string> saveData = new List<string>();
            saveData.Add("First Name;Last Name;Sex;Birth Date;Birth Country;Salary;SalaryCurrency;CompanyRole");

            foreach (var employee in collection)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(employee.FirstName);
                builder.Append(";");
                builder.Append(employee.LastName);
                builder.Append(";");
                builder.Append(employee.Sex);
                builder.Append(";");
                builder.Append(employee.BirthDate.ToShortDateString());
                builder.Append(";");
                builder.Append(employee.BirthCountry);
                builder.Append(";");
                builder.Append(employee.Salary.ToString());
                builder.Append(";");
                builder.Append(((int)employee.SalaryCurrency).ToString());
                builder.Append(";");
                builder.Append(((int)employee.CompanyRole).ToString());

                saveData.Add(builder.ToString());
            }
            System.IO.File.WriteAllLines(filePath, saveData.ToArray());
            hasChanged = false;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(hasChanged && collection!=null)
            {
                var caption = "Save changes";
                var message = "Do you want to save changes before closing?";
                var result = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
                if (result == MessageBoxResult.Yes)
                {                  
                    SaveToFile();
                }
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV (*.csv)|*.csv";
            ofd.ShowDialog();
            string pathToFile = ofd.FileName;
            filePath = ofd.FileName;
            string[] data = System.IO.File.ReadAllLines(pathToFile);
            List<Employee> employees = new List<Employee>();
            for (int i = 1; i < data.Length; i++)
            {
                var oneRecord = data[i].Split(';');
                employees.Add(new Employee(oneRecord[0], oneRecord[1], oneRecord[2], DateTime.ParseExact(oneRecord[3], "dd.MM.yyyy", CultureInfo.InvariantCulture), oneRecord[4], int.Parse(oneRecord[5]), (Currency)int.Parse(oneRecord[6]), (Role)int.Parse(oneRecord[7])));

            }           
            collection = new ObservableCollection<Employee>(employees);
            this.DataContext = collection;
            hasChanged = false;

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (collection == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";

            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName == "") return;
            filePath = saveFileDialog.FileName;           
            SaveToFile();                                   
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (collection == null || hasChanged == false) System.Windows.Application.Current.Shutdown();
            
            var caption="Save changes";
            var message = "Do you want to save changes before closing?";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel);
            if(result== MessageBoxResult.Cancel)
            {
                return;
            }
            if (result == MessageBoxResult.Yes)
            {
                SaveToFile();
                
            }

            System.Windows.Application.Current.Shutdown();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            int index = mainList.Items.IndexOf(mainList.SelectedItem);
            if (index == 0) return;
            var prevItem = mainList.Items.GetItemAt(index - 1);
            var Item = mainList.Items.GetItemAt(index);
            collection.RemoveAt(index - 1);
            collection.Insert(index, (Employee)prevItem);

            this.DataContext = collection;
            hasChanged = true;

        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            int index = mainList.Items.IndexOf(mainList.SelectedItem);
            if (index == mainList.Items.Count - 1) return;
             
            var nextItem = mainList.Items.GetItemAt(index + 1);
            var Item = mainList.Items.GetItemAt(index);
            collection.RemoveAt(index);
            collection.Insert(index + 1, (Employee)Item);
            mainList.SelectedIndex = index + 1;
            this.DataContext = collection;
            hasChanged = true;

        }

        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (collection == null) return;
            if(addWindow!=null)
            {               
                addWindow.WindowState = WindowState.Normal;
                return;
            }
            addWindow = new Window1();
            addWindow.Closed += (o, args) => addWindow = null;
            

            addWindow.Show();
            
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            int index = mainList.Items.IndexOf(mainList.SelectedItem);
            if (index == mainList.Items.Count - 1)
                mainList.SelectedIndex = index - 1;           
            collection.RemoveAt(index);                        
            this.DataContext = collection;
            hasChanged = true;
            mainList.SelectedIndex = index;
        }
    }

    class NameAndSurToFull : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var Name = (string)values[0];
            var SurName = (string)values[1];

            return $"{Name} {SurName}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum Currency { PLN, USD, EUR, GBP, NOK }
    public enum Role { Worker, SeniorWorker, IT, Manager, Director, CEO }
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthCountry { get; set; }
        public int Salary { get; set; }
        public Currency SalaryCurrency { get; set; }
        public Role CompanyRole { get; set; }



        public Employee(string firstName, string lastName, string sex, DateTime birthDate, string birthCountry, int salary, Currency salaryCurrency, Role companyRole)
        {
            FirstName = firstName;
            LastName = lastName;
            Sex = sex;
            BirthDate = birthDate;
            BirthCountry = birthCountry;
            Salary = salary;
            SalaryCurrency = salaryCurrency;
            CompanyRole = companyRole;
        }
    }

    class CurrToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Currency v = (Currency)value;
            return (int)v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (int)value;
            return (Currency)i;
        }
    }

    class RoleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Role v = (Role)value;
            return (int)v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (int)value;
            return (Role)i;
        }
    }


    public class StringToIntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (int.TryParse(value.ToString(), out i))
            {
                if (i >= 5000)
                    return new ValidationResult(true, null);
                else
                    return new ValidationResult(false, "Salary cannot be lower than 5000.");

            }

            return new ValidationResult(false, "Value should be integer.");
        }
    }

    public class RoleTemplateSelector:DataTemplateSelector
    {
        public DataTemplate DefaultnDataTemplate { get; set; }
        public DataTemplate CeoDataTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Employee employee;
            employee = item as Employee;
            
            if(employee!=null && employee.CompanyRole==Role.CEO)
            {
                return CeoDataTemplate;
            }
            return DefaultnDataTemplate;
        }
    }
}
