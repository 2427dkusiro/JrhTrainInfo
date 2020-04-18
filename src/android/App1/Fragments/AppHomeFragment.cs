using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using TrainInfo.Stations;

namespace App1.Resources.layout
{
    internal class AppHomeFragment : Android.Support.V4.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.AppHome, container, false);
        }

        private LinearLayout linearLayout;
        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            linearLayout = view.FindViewById<LinearLayout>(Resource.Id.homeMainLinearLayout);

            await RenderFavoriteData();
        }

        private async Task RenderFavoriteData()
        {
            linearLayout.RemoveAllViews();

            var textView = new TextView(Context)
            {
                Text = "お気に入り駅",
                TextSize = 18,
            };
            textView.SetPadding(10, 10, 0, 0);
            linearLayout.AddView(textView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

            var favorite = await GetFavoriteStations();
            foreach (var station in favorite)
            {
                var button = new Button(Context)
                {
                    Text = station.Name,
                };
                button.Click += FavoriteStationButton_Click;
                button.LongClick += FavoriteStationButton_LongClick;
                linearLayout.AddView(button, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
            }
        }

        private void FavoriteStationButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(Context, new TrainTimeActivity().Class);
            intent.PutExtra("station", ((Button)sender).Text);
            StartActivity(intent);
        }

        private string nowdeleting;
        private async void FavoriteStationButton_LongClick(object sender, View.LongClickEventArgs e)
        {
            var button = (Button)sender;
            nowdeleting = button.Text;
            new AlertDialog.Builder(Activity)
                .SetTitle("削除確認")
                .SetMessage($"{button.Text}をお気に入りから削除してよろしいですか？")
                .SetPositiveButton("OK", DialogResult)
                .SetNegativeButton("Cancel", DialogResult)
                .Show();
        }

        private async Task<Station[]> GetFavoriteStations()
        {
            Station[] favorite = null;
            await Task.Run(() => favorite = UserConfigManager.GetFavoriteStations());
            return favorite;
        }

        private void DialogResult(object sender, DialogClickEventArgs dialogClickEventArgs)
        {
            var result = dialogClickEventArgs.Which;
            switch (result)
            {
                case -1:
                    UserConfigManager.DeleteFavoriteStation(nowdeleting);
                    break;
                default:
                    break;
            }
            nowdeleting = null;
            RenderFavoriteData();
        }
    }
}