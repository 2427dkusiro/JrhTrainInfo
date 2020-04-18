using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using TrainInfo;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;


namespace App1
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

        private RelativeLayout.LayoutParams destNameTextViewLayoutParams;
        private RelativeLayout.LayoutParams deployIconlayoutParams;
        private LinearLayout.LayoutParams trainTypeIconlayoutParams;
        private ViewGroup.LayoutParams MP_WP_LayoutParams;
        private LinearLayout.LayoutParams trainDestLayoutParams;
        private LinearLayout.LayoutParams trainTimeLayoutParams;
        private LinearLayout.LayoutParams trainNameLayoutParams;

        private LinearLayout[] linearLayouts;
        private bool[] visiablityData;

        private const int textSize_Normal = 16;
        private const int textSize_Title = 20;
        private const int showCountMax = 4;

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
            destNameTextViewLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            destNameTextViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            destNameTextViewLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            destNameTextViewLayoutParams.LeftMargin = 40;

            deployIconlayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            deployIconlayoutParams.AddRule(LayoutRules.CenterVertical);
            deployIconlayoutParams.AddRule(LayoutRules.AlignParentEnd);
            deployIconlayoutParams.Height = 60;
            deployIconlayoutParams.Width = 60;
            deployIconlayoutParams.RightMargin = 40;

            trainTypeIconlayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            {
                Height = 50,
                Width = 50,
                Gravity = GravityFlags.Center,
                LeftMargin = 20
            };

            MP_WP_LayoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            trainDestLayoutParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            };

            trainTimeLayoutParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                Gravity = GravityFlags.Right,
                RightMargin = 20
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
                data = await TrainInfoReader.GetTrainDataAsync(station.StationID);
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

            if (linearLayouts == null)
            {
                linearLayouts = new LinearLayout[trainDataFile.DepartureTrainDatas.Count];
            }

            if (visiablityData == null)
            {
                visiablityData = new bool[trainDataFile.DepartureTrainDatas.Count];
            }

            var i = 0;
            //テキストの配置
            foreach (var keyValue in trainDataFile.DepartureTrainDatas)
            {
                var depData = keyValue.Value;
                var showCount = new int[] { depData.Count, showCountMax }.Min();

                var destTypeGroupLinearLayout = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical
                };
                destTypeGroupLinearLayout.SetMinimumHeight(120);
                linearLayouts[i] = destTypeGroupLinearLayout;

                #region タイトル部生成
                var destTypeRelativeLayout = new RelativeLayout(this);
                destTypeRelativeLayout.SetBackgroundColor(Android.Graphics.Color.AliceBlue);
                destTypeRelativeLayout.Click += DestTypeRelativeLayout_Click;

                var destTypeNameTextView = new TextView(this)
                {
                    Text = keyValue.Key.GetName(),
                    TextSize = textSize_Title,
                };
                destTypeRelativeLayout.AddView(destTypeNameTextView, destNameTextViewLayoutParams);

                var deployImageView = new ImageView(this);
                deployImageView.SetImageResource(visiablityData[i] ? Resource.Drawable.DeployedIcon : Resource.Drawable.DeployIcon);
                deployImageView.SetScaleType(ImageView.ScaleType.FitXy);
                deployImageView.SetAdjustViewBounds(true);
                destTypeRelativeLayout.AddView(deployImageView, deployIconlayoutParams);

                destTypeGroupLinearLayout.AddView(destTypeRelativeLayout, MP_WP_LayoutParams);
                #endregion

                for (var j = 0; j < showCount; j++)
                {
                    var train = depData[j];

                    var trainDataLinearLayout = new LinearLayout(this)
                    {
                        Orientation = Orientation.Vertical,
                        Visibility = visiablityData[i] ? ViewStates.Visible : ViewStates.Gone
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
                    };
                    trainGaiyoLayout.AddView(trainNameTextView, trainNameLayoutParams);

                    var destNameTextView = new TextView(this)
                    {
                        Text = train.Destination.GetShortName(),
                        TextSize = textSize_Normal,
                    };
                    trainGaiyoLayout.AddView(destNameTextView, trainDestLayoutParams);

                    var trainTimeTextView = new TextView(this)
                    {
                        Text = train.Time.ToString("HH:mm"),
                        TextSize = textSize_Normal,
                        TextAlignment = TextAlignment.TextEnd,
                    };
                    trainGaiyoLayout.AddView(trainTimeTextView, trainTimeLayoutParams);

                    trainDataLinearLayout.AddView(trainGaiyoLayout, MP_WP_LayoutParams);
                    destTypeGroupLinearLayout.AddView(trainDataLinearLayout, MP_WP_LayoutParams);
                }

                mainLinearLayout.AddView(destTypeGroupLinearLayout, MP_WP_LayoutParams);

                i++;
            }
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

        private void DestTypeRelativeLayout_Click(object sender, EventArgs e)
        {
            var layout = ((RelativeLayout)sender).Parent as LinearLayout;
            var index = Array.IndexOf(linearLayouts, layout);
            visiablityData[index] = !visiablityData[index];

            var childCount = layout.ChildCount;
            if (childCount > 1)
            {
                for (var i = 1; i < childCount; i++)
                {
                    var child = layout.GetChildAt(i);
                    if (child.Visibility == ViewStates.Visible)
                    {
                        child.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        child.Visibility = ViewStates.Visible;
                    }
                }
            }
            RenderData();
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