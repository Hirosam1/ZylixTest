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

namespace ZylixTest
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public Conteudo ctd;

        public EditWindow(ref Conteudo ctd)
        {
            this.ctd = ctd;
            InitializeComponent();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            ctd.ID = IDLabel.Text;
            ctd.Description = DescLabel.Text;
            ctd.Value = ValLabel.Text;
            ctd.Comments = ComLabel.Text;
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            IDLabel.Text = ctd.ID;
            DescLabel.Text = ctd.Description;
            ValLabel.Text = ctd.Value;
            ComLabel.Text = ctd.Comments;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
