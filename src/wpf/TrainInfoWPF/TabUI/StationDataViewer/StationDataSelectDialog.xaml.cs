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
using TrainInfo.Stations;

namespace TrainInfoWPF.TabUI.StationDataViewer
{
    /// <summary>
    /// StationDataSelectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class StationDataSelectDialog : Window
    {
        public StationDataSelectDialog()
        {
            InitializeComponent();
        }

        public Station SelectedStation { get; set; }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var station = StationReader.GetStationByName(StationSerchTextBox.Text);
            if(station is null)
            {
                MessageBox.Show($"{StationSerchTextBox.Text}は見つかりませんでした");
            }
            else
            {
                SelectedStation = station;
                this.Close();
            }
        }
    }

    public abstract class StationDataSource
    {
        
    }
}
