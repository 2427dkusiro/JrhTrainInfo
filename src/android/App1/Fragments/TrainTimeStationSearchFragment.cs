using System;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace App1.Resources.layout
{
    internal class TrainTimeStationSearchFragment : Android.Support.V4.App.Fragment
    {
        private LinearLayout lineLinearLayout;
        private readonly JrhLine[] firstLines = new[] { JrhLine.Hakodate_Iwamizawa, JrhLine.Hakodate_Otaru, JrhLine.Chitose_Tomakomai, JrhLine.Sassyo };
        private readonly JrhLine[] otherLines = new[] { JrhLine.Sekisyo, JrhLine.Nemuro_Furano, JrhLine.Nemuro_Obihiro };
        private HierarchyButtonLayout hierarchyButtonLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.MainTabFragmentLayout_TrainTimeStationSearch, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var searchButton = view.FindViewById<Button>(Resource.Id.StationSearchButton);
            searchButton.Click += SearchButton_Click;

            lineLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.LineLinearLayout);

            view.FocusableInTouchMode = true;
            view.RequestFocus();
            view.KeyPress += View_KeyPress;

            hierarchyButtonLayout = new HierarchyButtonLayout(Context);
            SetButons(hierarchyButtonLayout.RootButton);
            hierarchyButtonLayout.RootButton.Description = "路線選択";

            lineLinearLayout.AddView(hierarchyButtonLayout.Build());
        }

        /// <summary>
        /// 「戻る」キーが押されたときに駅選択を一段階戻す処理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action != KeyEventActions.Down)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = hierarchyButtonLayout.Back();
            }
        }

        /// <summary>
        /// 「検索」ボタンが押されたときの処理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, EventArgs e)
        {
            var editText = View.FindViewById<EditText>(Resource.Id.StaionNameEditText);
            var textView = View.FindViewById<TextView>(Resource.Id.StationSearchErrorText);
            var name = editText.Text;
            if (name.EndsWith("駅"))
            {
                name = name.Substring(0, name.Length - 1);
            }

            var station = StationReader.GetStationByName(name);
            if (station == null)
            {
                textView.Text = $"駅名「{name}」が見つかりません。";
            }
            else
            {
                var intent = new Intent(Context, new TrainTimeActivity().Class);
                intent.PutExtra("station", station.Name);
                StartActivity(intent);
            }
        }

        /// <summary>
        /// 階層ボタンを初期化する処理。
        /// </summary>
        /// <param name="hierarchyButton"></param>
        private void SetButons(HierarchyButtonLayout.RootHierarchyButton hierarchyButton)
        {
            foreach (var line in firstLines)
            {
                var firstButton = new HierarchyButtonLayout.HierarchyTextButton()
                {
                    Text = line.GetName(),
                    ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                };
                firstButton.AddChildren(LineDataReader.GetStations(line).Select(std =>
                {
                    var stationButton = new HierarchyButtonLayout.HierarchyTextButton()
                    {
                        Text = std.Name,
                        ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                    };
                    stationButton.Click += StationButton_Click;
                    return stationButton;
                }));
                hierarchyButton.AddChild(firstButton);
            }

            var otherButton = new HierarchyButtonLayout.HierarchyTextButton()
            {
                Text = "その他の路線",
                ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
            };
            foreach (var othline in otherLines)
            {
                var othLineButton = new HierarchyButtonLayout.HierarchyTextButton()
                {
                    Text = othline.GetName(),
                    ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                };
                othLineButton.AddChildren(LineDataReader.GetStations(othline).Select(std =>
                {
                    var stationButton = new HierarchyButtonLayout.HierarchyTextButton()
                    {
                        Text = std.Name,
                        ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                    };
                    stationButton.Click += StationButton_Click;
                    return stationButton;
                }));
                otherButton.AddChild(othLineButton);
            }

            hierarchyButton.AddChild(otherButton);
        }

        /// <summary>
        /// 階層ボタン中の駅が押されたときの処理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationButton_Click(object sender, EventArgs e)
        {
            var button = (HierarchyButtonLayout.HierarchyTextButton)sender;
            var intent = new Intent(Context, new TrainTimeActivity().Class);
            intent.PutExtra("station", button.Text);
            StartActivity(intent);
        }

    }
}