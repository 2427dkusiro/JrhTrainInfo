using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

using System;

using TrainInfo;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace JrhTrainInfoAndroid
{
    [Activity(Label = "TrainPositionActicity")]
    public class TrainPositionActicity : Activity
    {
        private LinearLayout trainPostionLinearLayout;
        private JrhLine jrhLine;
        private SwipeRefreshLayout swipeRefreshLayout;
        private TextView GettedTimeTextView;
        private bool IsFavorited;
        private ImageView favoriteButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TrainPosition);
            Window.SetStatusBarColor(new Android.Graphics.Color(Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.colorPrimary)));

            var line = Intent.GetStringExtra("Line");
            jrhLine = JrhLineCreater.FromString(line);

            trainPostionLinearLayout = FindViewById<LinearLayout>(Resource.Id.TrainpositionLinearLayout);
            var backButton = FindViewById<ImageView>(Resource.Id.BackButton);
            favoriteButton = FindViewById<ImageView>(Resource.Id.FavoriteButton);
            var TitleTextView = FindViewById<TextView>(Resource.Id.ToolbarText);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh);
            GettedTimeTextView = FindViewById<TextView>(Resource.Id.GetedTimeTextView);

            favoriteButton.Click += FavoriteButton_Click;
            backButton.Click += BackButton_Click;
            TitleTextView.Text = jrhLine.GetName();
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;

            IsFavorited = UserConfigManager.IsFavoriteLine(jrhLine);
            favoriteButton.SetImageResource(IsFavorited ? Resource.Drawable.FavoritedIcon : Resource.Drawable.AddFavoriteIcon);

            RenderData(jrhLine);
        }

        private void FavoriteButton_Click(object sender, EventArgs e)
        {
            if (IsFavorited)
            {
                UserConfigManager.DeletefavoriteLine(jrhLine);
                var toast = Toast.MakeText(this, $"{jrhLine.GetName()}を削除しました", ToastLength.Short);
                toast.SetGravity(GravityFlags.Bottom, 0, 0);
                toast.Show();
                IsFavorited = false;
            }
            else
            {
                UserConfigManager.AddfavoriteLine(jrhLine);
                var toast = Toast.MakeText(this, $"{jrhLine.GetName()}を追加しました", ToastLength.Short);
                toast.SetGravity(GravityFlags.Bottom, 0, 0);
                toast.Show();
                IsFavorited = true;
            }
            favoriteButton.SetImageResource(IsFavorited ? Resource.Drawable.FavoritedIcon : Resource.Drawable.AddFavoriteIcon);
        }

        private void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            RenderData(jrhLine);
            swipeRefreshLayout.Refreshing = false;
        }

        private async void RenderData(JrhLine jrhLine)
        {
            trainPostionLinearLayout.RemoveAllViews();

            var (arrivalData, departureData) = await TrainInfoReader.GetTrainPositionDataAsync(jrhLine);

            for (var i = 0; i < arrivalData.Count; i++)
            {
                var lineRangeLinearLayout = new LinearLayout(this)
                {
                    Orientation = Orientation.Horizontal
                };

                var stationAreaLayout = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical
                };
                var stationLayoutParams = new LinearLayout.LayoutParams(0, -1)
                {
                    Weight = 2
                };

                if (arrivalData[i].Item1.IsStation)
                {
                    lineRangeLinearLayout.SetBackgroundColor(Android.Graphics.Color.Argb(0xff, 0xea, 0xea, 0xea));

                    var textView = new TextView(this)
                    {
                        Text = arrivalData[i].Item1.StartPos.Name,
                        TextSize = 16,
                    };
                    textView.SetPadding(30, 20, 0, 0);
                    textView.Typeface = Typeface.DefaultBold;
                    stationAreaLayout.AddView(textView);
                }
                else
                {
                    lineRangeLinearLayout.SetBackgroundColor(Android.Graphics.Color.Argb(0xff, 0xff, 0xff, 0xff));
                }

                lineRangeLinearLayout.AddView(stationAreaLayout, stationLayoutParams);

                var arrLayoutParams = new LinearLayout.LayoutParams(0, -1)
                {
                    Weight = 3
                };
                var arrTrainLinearLayout = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical
                };

                var depLayoutParams = new LinearLayout.LayoutParams(0, -1)
                {
                    Weight = 3
                };
                var depTrainLinearLayout = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical
                };

                foreach (var arr in arrivalData[i].Item2)
                {
                    var textView = new TextView(this)
                    {
                        Text = $"↑{arr.Name.TrainType.GetName()} {arr.Destination.Name}行",
                    };
                    arrTrainLinearLayout.AddView(textView);
                }

                foreach (var dep in departureData[i].Item2)
                {
                    var textView = new TextView(this)
                    {
                        Text = $"↓{dep.Name.TrainType.GetName()} {dep.Destination.Name}行",
                    };
                    depTrainLinearLayout.AddView(textView);
                }

                lineRangeLinearLayout.AddView(arrTrainLinearLayout, arrLayoutParams);
                lineRangeLinearLayout.AddView(depTrainLinearLayout, depLayoutParams);

                trainPostionLinearLayout.AddView(lineRangeLinearLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 250));
            }

            GettedTimeTextView.Text = $"{ DateTime.Now.ToString()} 現在の情報";
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}