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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainInfo;
using TrainInfo.Models;
using TrainInfo.Stations;

namespace TrainPositionView
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty statusTextProperty
            = DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(UserControl1));

        public string StatusText
        {
            get { return (string)GetValue(statusTextProperty); }
            set { SetValue(statusTextProperty, value); }
        }

        public async Task SetData(JrhLine jrhLine)
        {
            var (arrData, depData) = await TrainInfoReader.GetTrainDataByLine(jrhLine);
            var station = LineDataReader.GetStations(jrhLine);
            SetLabels(arrData, depData, station, MainDataGrid);
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


                if (dataIndex % 2 == 0)
                {
                    //必ず背景色設定コードが一番前
                    int i = 0;
                    do
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Rectangle rectangle = new Rectangle()
                            {
                                Fill = Brushes.WhiteSmoke
                            };
                            targetDataGrid.Children.Add(rectangle);
                            Grid.SetRow(rectangle, rowIndex + i);
                            Grid.SetColumn(rectangle, j);
                        }
                        i++;
                    } while (i < trainCount);
                    //背景色指定コードここまで

                    Label stationLabel = new Label()
                    {
                        Content = station[dataIndex / 2],
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    targetDataGrid.Children.Add(stationLabel);
                    Grid.SetRow(stationLabel, rowIndex);
                    Grid.SetColumn(stationLabel, 1);
                }

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

                if (trainCount > 1)
                    rowIndex += trainCount;
                else
                    rowIndex++;
            }
        }
    }
}
