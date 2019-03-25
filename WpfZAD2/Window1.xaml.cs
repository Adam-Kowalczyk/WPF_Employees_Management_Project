using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfZAD2
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            bDate.SelectedDate = DateTime.Today.AddYears(-30);

            this.DataContext = new DefaultValues { Salary = "5000" };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (fName.Text == "" || lName.Text == "" || bCountry.Text == "") return;

            string sex;
            if (isMale.IsChecked == true)
                sex = "Male";
            else
                sex = "Female";

            ((MainWindow)System.Windows.Application.Current.MainWindow).collection.Add(new Employee(fName.Text, lName.Text, sex,(DateTime)bDate.SelectedDate, bCountry.Text, int.Parse(salary.Text), (Currency)salCurr.SelectedIndex, (Role)comRole.SelectedIndex));
            this.Close();

        }
    }
    public class DefaultValues
    {
        public string Salary { get; set; }
    }

    public class StringToIntValidationRule1 : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (int.TryParse(value.ToString(), out i))
            {
                if (i >= 5000)
                    return new ValidationResult(true, null);
                else
                    return new ValidationResult(false,null);

            }

            return new ValidationResult(false,null);
        }
    }
}
