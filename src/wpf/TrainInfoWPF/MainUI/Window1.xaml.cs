using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
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
using TrainInfo;
using TrainInfoWPF.CustomControls;
using TrainInfoWPF.TabUI.StationDataViewer;

namespace TrainInfoWPF.MainUI
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.AddHandler(ClosableTabItem.TabClosedEvent, new RoutedEventHandler(CloseTab));
        }

        private void CloseTab(object sender, RoutedEventArgs args)
        {
            if (args.Source is TabItem tabItem)
            {
                if (tabItem.Parent is TabControl tabControl)
                {
                    tabControl.Items.Remove(tabItem);
                }
            }
        }

        private async void StationDataWindowOpenMenu_Click(object sender, RoutedEventArgs args)
        {
            StationDataSelectDialog stationDataSelectDialog = new StationDataSelectDialog();
            stationDataSelectDialog.ShowDialog();

            var station = stationDataSelectDialog.SelectedStation;
            var trainData = await TrainInfoReader.GetTrainDataAsync(station.StationID);

            StationDataView stationDataView = new StationDataView(trainData);
            AddTab(stationDataView);
        }

        private void AddTab(object content)
        {
            MainTab.Items.Add(new ClosableTabItem()
            {
                Content = content,
                Header = "タイトル(仮)",
            });
        }
    }
}
