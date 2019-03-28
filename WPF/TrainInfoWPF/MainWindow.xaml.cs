using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using TrainInfo;
using TrainInfo.Models;
using TrainInfo.Stations;
using System.Diagnostics;

namespace TrainInfoWPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            const string path = @"C:\Users\Kota\Desktop\未知の駅ID.csv";
            Debuger.CodeGenerator.ReadStationCsvFile(path);

            Debuger.DataManager dataManager = new Debuger.DataManager();
            dataManager.Show();
            InitializeComponent();
        }

        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
            if (IwamizawaTab.IsSelected)
            {
                Task.Run(() => Timer(30, () => SetIwamizawaData(), CancellationTokenSource.Token));
            }
            else if (OtaruTab.IsSelected)
            {
                Task.Run(() => Timer(30, () => SetOtaruData(), CancellationTokenSource.Token));
            }
            else if (ChitoseTab.IsSelected)
            {
                Task.Run(() => Timer(30, () => SetChitoseData(), CancellationTokenSource.Token));
            }
            else if (TobetsuTab.IsSelected)
            {
                Task.Run(() => Timer(30, () => SetTobetsuData(), CancellationTokenSource.Token));
            }
        }

        private void Timer(int time, Action action, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Dispatcher.Invoke(action);
                Thread.Sleep(time * 1000);
            }
        }

        private async Task SetIwamizawaData()
        {
            await Iwamizawa.SetData(JrhLine.Hakodate_Iwamizawa);
            Iwamizawa.StatusText = $"取得完了　(取得日時:{DateTime.Now})";
        }

        private async Task SetOtaruData()
        {
            await Otaru.SetData(JrhLine.Hakodate_Otaru);
            Otaru.StatusText = $"取得完了　(取得日時:{DateTime.Now})";
        }

        private async Task SetChitoseData()
        {
            await Chitose.SetData(JrhLine.Chitose_Tomakomai);
            Chitose.StatusText = $"取得完了　(取得日時:{DateTime.Now})";
        }

        private async Task SetTobetsuData()
        {
            await Tobetsu.SetData(JrhLine.Sassyo);
            Tobetsu.StatusText = $"取得完了　(取得日時:{DateTime.Now})";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
        }

        /*
        private async Task SetData(JrhLine jrhLine, Grid target)
        {
            var (arrData, depData) = await TrainInfoReader.GetTrainDataByLine(jrhLine);
            var station = LineDataReader.GetStations(jrhLine);
            SetLabels(arrData, depData, station, target);
        }

        private void SetLabels(TrainDataWithPosition arrData, TrainDataWithPosition depData, Station[] station, Grid targetDataGrid)
        {
            targetDataGrid.Children.Clear();
            targetDataGrid.RowDefinitions.Clear();

            int rangeCount = station.Length * 2 - 1;
            int rowIndex = 0;

            for (int dataIndex = 0; dataIndex < rangeCount; dataIndex++)
            {
                var arr = arrData[(dataIndex + 2) / 2m];
                var dep = depData[(dataIndex + 2) / 2m];

                int trainCount = arr.Count > dep.Count ? arr.Count : dep.Count;


                int addCount = 0;
                do
                {
                    targetDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    addCount++;
                } while (addCount < trainCount);

                for (int i = 0; i < dep.Count; i++)
                {
                    Label depLabel = new Label()
                    {
                        Content = $"↓{dep[i].Name.ToString()} {dep[i].Destination.Name}行",
                        HorizontalAlignment = HorizontalAlignment.Right,
                    };
                    targetDataGrid.Children.Add(depLabel);
                    Grid.SetRow(depLabel, rowIndex + i);
                    Grid.SetColumn(depLabel, 0);
                }

                for (int i = 0; i < arr.Count; i++)
                {
                    Label arrLabel = new Label()
                    {
                        Content = $"↑{arr[i].Name.ToString()} {arr[i].Destination.Name}行",
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                    targetDataGrid.Children.Add(arrLabel);
                    Grid.SetRow(arrLabel, rowIndex + i);
                    Grid.SetColumn(arrLabel, 2);
                }

                if (dataIndex % 2 == 0)
                {
                    Label stationLabel = new Label()
                    {
                        Content = station[dataIndex / 2],
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    targetDataGrid.Children.Add(stationLabel);
                    Grid.SetRow(stationLabel, rowIndex);
                    Grid.SetColumn(stationLabel, 1);
                }

                if (trainCount > 1)
                    rowIndex += trainCount;
                else
                    rowIndex++;
            }
        }
        */
    }
}
