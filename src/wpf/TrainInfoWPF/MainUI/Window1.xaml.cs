using System;
using System.Collections.Generic;
using System.Deployment.Internal;
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
using TrainInfoWPF.Debuger;
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
            TrainInfoReader.SetRedirect(new TrainInfo.Debuggers.InternalSavedDataReader());
            InitializeComponent();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            var data = await TrainInfoReader.GetTrainDataAsync(91);
            this.AddTab(new StationDataView(data), $"駅列車情報 - {data.Station.Name}");
            MainTab.SelectedIndex = 0;
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
            StationDataView stationDataView = await StationDataView.InitializeWithDialog();
            if (stationDataView != null)
            {
                AddTab(stationDataView, $"駅列車情報 - {stationDataView.TrainDataFile.Station.Name}");
            }
        }

        private void AddTab(Control content, string title)
        {
            MainTab.Items.Add(new ClosableTabItem()
            {
                Content = content,
                Header = title,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            });
        }
    }
}
