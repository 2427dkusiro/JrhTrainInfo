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
using System.Data;

using TrainInfo;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// StationDataViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class TrainDataFileViewer : Window
    {
        public TrainDataFileViewer()
        {
            InitializeComponent();
        }

        public void RenderData(TrainDataFile trainDataFile)
        {
            Title = $"{trainDataFile.Station} - {trainDataFile.GetedDateTime.ToString()}";

            int i = 0;
            foreach (var trainDataSet in trainDataFile.DepartureTrainDatas)
            {
                Label label = new Label()
                {
                    Content = $"{trainDataSet.DestType.GetName()}",
                    VerticalAlignment = VerticalAlignment.Center,
                };

                DataGridGenerator dataGridGenerator = new DataGridGenerator();
                dataGridGenerator.AddColumn("列車名", trainDataSet.TrainDatas.Select(td => td.Name.ToString()).ToArray(), 2);
                dataGridGenerator.AddColumn("行先", trainDataSet.TrainDatas.Select(td => td.Destination.Name).ToArray(), 1);
                dataGridGenerator.AddColumn("出発時刻", trainDataSet.TrainDatas.Select(td => td.Time.ToString("HH:mm")).ToArray(), 1);
                dataGridGenerator.AddColumn("現在地", trainDataSet.TrainDatas.Select(td => td.NowPosition?.ToString() ?? "出発前").ToArray(), 2);
                dataGridGenerator.AddColumn("運行状況", trainDataSet.TrainDatas.Select(td => td.Condition.ToString()).ToArray(), 2);

                DataGrid dataGrid = dataGridGenerator.Finalize();

                DiparturesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
                DiparturesGrid.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(label, i);
                Grid.SetRow(dataGrid, i + 1);

                DiparturesGrid.Children.Add(label);
                DiparturesGrid.Children.Add(dataGrid);

                i += 2;
            }

            i = 0;
            foreach (var trainDataSet in trainDataFile.ArrivalTrainDatas)
            {
                Label label = new Label()
                {
                    Content = $"{trainDataSet.DestType.GetName()}",
                    VerticalAlignment = VerticalAlignment.Center,
                };

                DataGridGenerator dataGridGenerator = new DataGridGenerator();
                dataGridGenerator.AddColumn("列車名", trainDataSet.TrainDatas.Select(td => td.Name.ToString()).ToArray(), 2);
                dataGridGenerator.AddColumn("行先", trainDataSet.TrainDatas.Select(td => td.Destination.Name).ToArray(), 1);
                dataGridGenerator.AddColumn("到着時刻", trainDataSet.TrainDatas.Select(td => td.Time.ToString("HH:mm")).ToArray(), 1);
                dataGridGenerator.AddColumn("現在地", trainDataSet.TrainDatas.Select(td => td.NowPosition?.ToString() ?? "出発前").ToArray(), 2);
                dataGridGenerator.AddColumn("運行状況", trainDataSet.TrainDatas.Select(td => td.Condition.ToString()).ToArray(), 2);

                DataGrid dataGrid = dataGridGenerator.Finalize();

                ArrivalsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
                ArrivalsGrid.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(label, i);
                Grid.SetRow(dataGrid, i + 1);

                ArrivalsGrid.Children.Add(label);
                ArrivalsGrid.Children.Add(dataGrid);

                i += 2;
            }
        }
    }

    class DataGridGenerator
    {
        private DataGrid dataGrid;
        private DataTable dataTable;

        public DataGridGenerator()
        {
            dataGrid = new DataGrid()
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
            };
            dataTable = new DataTable();
        }

        List<string[]> columns = new List<string[]>();
        public void AddColumn(string title, string[] column)
        {
            dataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = title,
                Binding = new Binding(title)
            });
            dataTable.Columns.Add(title, typeof(string));

            columns.Add(column);
        }

        public void AddColumn(string title, string[] column, double size)
        {
            dataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = title,
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
                Binding = new Binding(title)
            });
            dataTable.Columns.Add(title, typeof(string));

            columns.Add(column);
        }

        /*
        public void AddTextColumn(string title)
        {
            dataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = title,
                Binding = new Binding(title),
            });

            dataTable.Columns.Add(title, typeof(string));
        }

        public void AddRow(IEnumerable<(string title, string value)> row)
        {
            DataRow dataRow = dataTable.NewRow();
            foreach (var (title, value) in row)
            {
                dataRow[title] = value;
            }
            dataTable.Rows.Add(dataRow);
        }
        */

        public DataGrid Finalize()
        {
            if (columns != null)
            {
                for (int i = 0; i < columns[0].Length; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int j = 0; j < columns.Count; j++)
                    {
                        dataRow[j] = columns[j][i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            dataGrid.DataContext = dataTable;
            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding());
            return dataGrid;
        }
    }
}
