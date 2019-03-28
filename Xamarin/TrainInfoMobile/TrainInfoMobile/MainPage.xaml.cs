using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;

using System.Diagnostics;

namespace TrainInfoMobile
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainTab_Appearing(object sender, System.EventArgs e)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(30 * 1000);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Debug.WriteLine("データ再取得開始...");
                        switch (CurrentPage)
                        {
                            case IwamizawaTrainPositionViewPage iwamizawaTrainPositionViewPage:
                                iwamizawaTrainPositionViewPage.SetData(TrainInfo.Models.JrhLine.Hakodate_Iwamizawa);
                                break;
                            case OtaruTrainPositionViewPage otaruTrainPositionViewPage:
                                otaruTrainPositionViewPage.SetData(TrainInfo.Models.JrhLine.Hakodate_Otaru);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    });
                }
            });
        }
    }
}
