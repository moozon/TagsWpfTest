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

namespace TagsWpfTest
{
    /// <summary>
    /// Логика взаимодействия для NewTagWindow.xaml
    /// </summary>
    public partial class NewTagWindow : Window
    {
        

        public NewTagWindow()
        {
            InitializeComponent();
        }

        public string NewName { get; set; }
        public string NewType { get; set; }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.NewName = nameTextBox.Text;
            this.NewType = typeTextBox.Text;
            this.Hide();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
 
    }
}
