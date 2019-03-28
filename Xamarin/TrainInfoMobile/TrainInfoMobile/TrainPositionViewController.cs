using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrainInfo;
using TrainInfo.Models;
using TrainInfo.Stations;

namespace TrainInfoMobile
{
    class TrainPositionViewController
    {
        public static void SetLabels(TrainDataWithPosition arrData, TrainDataWithPosition depData, Station[] station, Grid targetDataGrid)
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
                        Text = $"↓{dep[i].Name.TrainType.GetName()} {dep[i].Destination.Name}行",
                        HorizontalTextAlignment = TextAlignment.Start,
                    };
                    targetDataGrid.Children.Add(depLabel);
                    Grid.SetRow(depLabel, rowIndex + i);
                    Grid.SetColumn(depLabel, 0);
                }

                for (int i = 0; i < arr.Count; i++)
                {
                    Label arrLabel = new Label()
                    {
                        Text = $"↑{arr[i].Name.TrainType.GetName()} {arr[i].Destination.Name}行",
                        HorizontalTextAlignment = TextAlignment.End,
                    };
                    targetDataGrid.Children.Add(arrLabel);
                    Grid.SetRow(arrLabel, rowIndex + i);
                    Grid.SetColumn(arrLabel, 2);
                }

                if (dataIndex % 2 == 0)
                {
                    Label stationLabel = new Label()
                    {
                        Text = station[dataIndex / 2].Name,
                        HorizontalTextAlignment = TextAlignment.Center,
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

    }
}
