using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using TrainInfo;
using TrainInfo.Stations;
using TrainInfo.TrainPositions;

namespace TrainPositionView
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1() => InitializeComponent();

        public static readonly DependencyProperty statusTextProperty
            = DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(UserControl1));

        public string StatusText
        {
            get => (string)GetValue(statusTextProperty);
            set => SetValue(statusTextProperty, value);
        }

        public async Task SetData(JrhLine jrhLine)
        {
            var (arrData, depData) = await TrainInfoReader.GetTrainPositionDataAsync(jrhLine);
            var station = LineDataReader.GetStations(jrhLine);
            SetLabels(arrData, depData, MainDataGrid);
        }

        private void SetLabels(TrainPositionData arrData, TrainPositionData depData, Grid targetDataGrid)
        {
            targetDataGrid.Children.Clear();
            targetDataGrid.RowDefinitions.Clear();

            var rowIndex = 0;

            for (var i = 0; i < arrData.Count; i++)
            {
                var (lineRange, arr) = arrData[i];
                var (_, dep) = depData[i];

                var trainCount = new[] { arr.Count, dep.Count }.Max();

                var addCount = 0;
                do
                {
                    targetDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    addCount++;
                } while (addCount < trainCount);

                if (lineRange.IsStation)
                {
                    //必ず背景色設定コードが一番前
                    var k = 0;
                    do
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            var rectangle = new Rectangle()
                            {
                                Fill = Brushes.WhiteSmoke
                            };
                            targetDataGrid.Children.Add(rectangle);
                            Grid.SetRow(rectangle, rowIndex + k);
                            Grid.SetColumn(rectangle, j);
                        }

                        k++;
                    } while (k < trainCount);
                    //背景色指定コードここまで

                    var stationLabel = new Label()
                    {
                        Content = lineRange,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    targetDataGrid.Children.Add(stationLabel);
                    Grid.SetRow(stationLabel, rowIndex);
                    Grid.SetColumn(stationLabel, 1);
                }

                for (var j = 0; j < dep.Count; j++)
                {
                    var depLabel = new Label()
                    {
                        Content = $"↓{dep[j].Name.ToString()} {dep[j].Destination.Name}行",
                        HorizontalAlignment = HorizontalAlignment.Right,
                    };
                    targetDataGrid.Children.Add(depLabel);
                    Grid.SetRow(depLabel, rowIndex + j);
                    Grid.SetColumn(depLabel, 0);
                }

                for (var j = 0; j < arr.Count; j++)
                {
                    var arrLabel = new Label()
                    {
                        Content = $"↑{arr[j].Name.ToString()} {arr[j].Destination.Name}行",
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                    targetDataGrid.Children.Add(arrLabel);
                    Grid.SetRow(arrLabel, rowIndex + j);
                    Grid.SetColumn(arrLabel, 2);
                }

                if (trainCount > 1)
                {
                    rowIndex += trainCount;
                }
                else
                {
                    rowIndex++;
                }
            }
        }
    }
}
