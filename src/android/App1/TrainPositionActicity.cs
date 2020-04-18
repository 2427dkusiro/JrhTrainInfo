using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using TrainInfo;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace App1
{
    [Activity(Label = "TrainPositionActicity")]
    public class TrainPositionActicity : Activity
    {
        private LinearLayout trainPostionLinearLayout;
        private JrhLine jrhLine;
        private SwipeRefreshLayout swipeRefreshLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TrainPosition);
            // Create your application here
            trainPostionLinearLayout = FindViewById<LinearLayout>(Resource.Id.TrainpositionLinearLayout);
            var line = Intent.GetStringExtra("Line");

            var backButton = FindViewById<Button>(Resource.Id.BackButton);
            backButton.Click += BackButton_Click;

            //呼び出し元で選択された路線を解析
            jrhLine = JrhLineCreater.FromString(line);
            var TitleTextView = FindViewById<TextView>(Resource.Id.ToolbarText);
            TitleTextView.Text = jrhLine.GetName();

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh);
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;

            RenderData(jrhLine);
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
                        Text = "↑" + arr.Destination.Name + "行"
                    };
                    arrTrainLinearLayout.AddView(textView);
                }

                foreach (var dep in departureData[i].Item2)
                {
                    var textView = new TextView(this)
                    {
                        Text = "↓" + dep.Destination.Name + "行"
                    };
                    depTrainLinearLayout.AddView(textView);
                }

                lineRangeLinearLayout.AddView(arrTrainLinearLayout, arrLayoutParams);
                lineRangeLinearLayout.AddView(depTrainLinearLayout, depLayoutParams);

                trainPostionLinearLayout.AddView(lineRangeLinearLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 250));
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}