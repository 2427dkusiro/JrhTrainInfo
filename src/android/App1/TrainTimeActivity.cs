using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

using System;
using System.Linq;
using System.Threading.Tasks;

using TrainInfo;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;


namespace JrhTrainInfoAndroid
{
    [Activity(Label = "TrainTimeActivity")]
    public class TrainTimeActivity : Activity
    {
        private Station station;
        private TrainDataFile trainDataFile;

        private LinearLayout mainLinearLayout;
        private ImageView backButton;
        private ImageView favoriteButton;
        private TextView toolBarText;
        private TextView GettedTimeTextView;
        private SwipeRefreshLayout swipeRefreshLayout;

        private bool IsFavorited;
        private readonly static Color color = Color.AliceBlue;

        private ViewGroup.LayoutParams trainDataLayoutParams;
        private ViewGroup.LayoutParams MP_WP_LayoutParams;

        private LinearLayout.LayoutParams trainTypeIconlayoutParams;
        private LinearLayout.LayoutParams trainDestLayoutParams;
        private LinearLayout.LayoutParams trainTimeLayoutParams;
        private LinearLayout.LayoutParams trainNameLayoutParams;


        private const int textSize_Normal = 16;
        private const int showCountMax = 6;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TrainTime);
            InitializeLayoutParams();
            Window.SetStatusBarColor(new Android.Graphics.Color(Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.colorPrimary)));

            mainLinearLayout = FindViewById<LinearLayout>(Resource.Id.MainLinearLayout);
            backButton = FindViewById<ImageView>(Resource.Id.BackButton);
            favoriteButton = FindViewById<ImageView>(Resource.Id.FavoriteButton);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh);
            toolBarText = FindViewById<TextView>(Resource.Id.ToolbarText);
            GettedTimeTextView = FindViewById<TextView>(Resource.Id.GetedTimeTextView);

            backButton.Click += BackButton_Click;
            favoriteButton.Click += FavoriteButton_Click;
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;

            station = StationReader.GetStationByName(Intent.GetStringExtra("station"));
            if (station is null)
            {
                throw new NotSupportedException();
            }
            else
            {
                IsFavorited = UserConfigManager.IsFavoriteStation(station);
                await ShowData();
                toolBarText.Text = station.Name;
                favoriteButton.SetImageResource(IsFavorited ? Resource.Drawable.FavoritedIcon : Resource.Drawable.AddFavoriteIcon);
            }
        }

        /// <summary>
        /// レイアウトパラメータを初期化します。
        /// </summary>
        private void InitializeLayoutParams()
        {
            trainDataLayoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            trainTypeIconlayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            {
                Height = 50,
                Width = 50,
                Gravity = GravityFlags.Center,
            };

            MP_WP_LayoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            trainDestLayoutParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            };

            trainTimeLayoutParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
            };

            trainNameLayoutParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 2
            };
        }

        /// <summary>
        /// 駅の列車データを取得します。
        /// </summary>
        /// <returns></returns>
        private async Task GetStationData()
        {
            TrainDataFile data = null;
            try
            {
                data = await TrainInfoReader.GetTrainDataAsync(station.StationId);
                trainDataFile = data;
            }
            catch (Exception ex)
            {
                EventHandler<DialogClickEventArgs> eventHandler = (object sender, DialogClickEventArgs e) =>
                {
                    if (e.Which == -1)
                    {
                        GetStationData();
                    }

                    if (e.Which == -2)
                    {
                        Finish();
                    }
                };

                new AlertDialog.Builder(this)
                 .SetTitle("ネットワークエラー")
                 .SetMessage($"データを取得できませんでした。端末のネットワーク接続状況をご確認ください。{System.Environment.NewLine}エラーの詳細:{ex.Message}")
                 .SetPositiveButton("再試行", eventHandler)
                 .SetNegativeButton("戻る", eventHandler)
                 .Show();
                return;
            }
        }

        /// <summary>
        /// データを取得して表示します。
        /// </summary>
        /// <returns></returns>
        private async Task ShowData()
        {
            await GetStationData();
            GettedTimeTextView.Text = $"{trainDataFile.GetedDateTime.ToString()} 現在の情報";
            RenderData();
        }

        /// <summary>
        /// データを表示します。
        /// </summary>
        private void RenderData()
        {
            mainLinearLayout.RemoveAllViews();

            //テキストの配置
            foreach (var keyValue in trainDataFile.DepartureTrainDatas)
            {
                var depData = keyValue.Value;
                var showCount = new int[] { depData.Count, showCountMax }.Min();

                var destTypeGroupedTrainLinearLayout = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical
                };

                for (var j = 0; j < showCount; j++)
                {
                    var trainGaiyoLayout = RenderTrainDataLayout(depData[j]);
                    trainGaiyoLayout.SetPadding(20, 5, 20, 5);
                    destTypeGroupedTrainLinearLayout.AddView(trainGaiyoLayout, trainDataLayoutParams);
                }

                TapToDeployLayout tapToDeployLayout = new TapToDeployLayout(this)
                {
                    Title = keyValue.Key.GetName(),
                    View = destTypeGroupedTrainLinearLayout,
                    Color = color,
                };

                mainLinearLayout.AddView(tapToDeployLayout.GetView(), MP_WP_LayoutParams);
            }
        }

        private LinearLayout RenderTrainDataLayout(TrainData train)
        {
            var parentLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical,
            };

            var trainGaiyoLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal
            };

            var trainTypeIcon = new ImageView(this);
            trainTypeIcon.SetImageResource(GetTrainTypeIcon(train.GetDisplayTrainTypes(station)));
            trainTypeIcon.SetScaleType(ImageView.ScaleType.FitXy);
            trainTypeIcon.SetAdjustViewBounds(true);
            trainGaiyoLayout.AddView(trainTypeIcon, trainTypeIconlayoutParams);

            var trainNameTextView = new TextView(this)
            {
                Text = train.Name.Name,
                TextSize = textSize_Normal,
                //TextAlignment = TextAlignment.Center,
            };
            trainGaiyoLayout.AddView(trainNameTextView, trainNameLayoutParams);

            var destNameTextView = new TextView(this)
            {
                Text = train.Destination.GetShortName(),
                TextSize = textSize_Normal,
                TextAlignment = TextAlignment.Center,
            };
            trainGaiyoLayout.AddView(destNameTextView, trainDestLayoutParams);

            var trainTimeTextView = new TextView(this)
            {
                Text = train.Time.ToString("HH:mm"),
                TextSize = textSize_Normal,
                TextAlignment = TextAlignment.Center,
            };
            trainGaiyoLayout.AddView(trainTimeTextView, trainTimeLayoutParams);

            // 二行目
            var status = new TextView(this)
            {
                TextSize = textSize_Normal,
            };

            if (train.NowPosition is null)
            {
                status.Text = train.Condition.ToString();
            }
            else
            {
                status.Text = $"{train.Condition.ToString()} | {train.NowPosition.ToString()}走行中";
            }

            var param = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            parentLayout.AddView(trainGaiyoLayout, param);
            parentLayout.AddView(status, param);
            return parentLayout;
        }

        private int GetTrainTypeIcon(TrainData.TrainTypes trainTypes)
        {
            switch (trainTypes)
            {
                case TrainData.TrainTypes.Local:
                    return Resource.Drawable.TrainFutsuIcon;
                case TrainData.TrainTypes.Semi_Rapid:
                    return Resource.Drawable.TrainIconKukai2;
                case TrainData.TrainTypes.Become_Semi_Rapid:
                    return Resource.Drawable.TrainIconKukai;
                case TrainData.TrainTypes.Rapid:
                    return Resource.Drawable.TrainIconKaisoku;
                case TrainData.TrainTypes.Ltd_Exp:
                    return Resource.Drawable.TrainIconTokyu;
                default:
                    throw new NotSupportedException();
            }
        }

        private async void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            await ShowData();
            swipeRefreshLayout.Refreshing = false;
        }

        private void FavoriteButton_Click(object sender, EventArgs e)
        {
            if (IsFavorited)
            {
                UserConfigManager.DeleteFavoriteStation(station);
                var toast = Toast.MakeText(this, $"{station.Name}を削除しました", ToastLength.Short);
                toast.SetGravity(GravityFlags.Bottom, 0, 0);
                toast.Show();
                IsFavorited = false;
            }
            else
            {
                UserConfigManager.AddfavoriteStation(station);
                var toast = Toast.MakeText(this, $"{station.Name}を追加しました", ToastLength.Short);
                toast.SetGravity(GravityFlags.Bottom, 0, 0);
                toast.Show();
                IsFavorited = true;
            }
            favoriteButton.SetImageResource(IsFavorited ? Resource.Drawable.FavoritedIcon : Resource.Drawable.AddFavoriteIcon);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}