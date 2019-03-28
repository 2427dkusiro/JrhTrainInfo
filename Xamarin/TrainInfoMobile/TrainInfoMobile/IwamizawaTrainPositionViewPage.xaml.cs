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
using System.Diagnostics;

namespace TrainInfoMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IwamizawaTrainPositionViewPage : ContentPage
    {
        public IwamizawaTrainPositionViewPage()
        {
            InitializeComponent();
        }

        private bool IsFirst = true;
        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (IsFirst)
            {
                await SetData(JrhLine.Hakodate_Iwamizawa);
                IsFirst = false;
            }
        }

        public async Task SetData(JrhLine jrhLine)
        {
            var (arrData, depData) = await TrainInfoReader.GetTrainDataByLine(jrhLine);
            var station = LineDataReader.GetStations(jrhLine);
            TrainPositionViewController.SetLabels(arrData, depData, station, MainGrid);
        }
    }
}