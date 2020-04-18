using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainInfo;
using TrainInfo.Debuggers;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace JrhTrainInfoAndroid.Resources.layout
{
    internal class AppHomeFragment : Android.Support.V4.App.Fragment
    {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.AppHome, container, false);
        }

        private LinearLayout favoriteStationLayout;
        private LinearLayout favoriteLineLayout;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            favoriteStationLayout = view.FindViewById<LinearLayout>(Resource.Id.FavoriteStationLinearLayout);
            favoriteLineLayout = view.FindViewById<LinearLayout>(Resource.Id.FavoriteLineLinearLayout);
            var doRedirect = view.FindViewById<CheckBox>(Resource.Id.DoRedirectCheckBox);
            doRedirect.CheckedChange += DoRedirect_CheckedChange;

            UserConfigManager.ValueChanged += UserConfigManager_ValueChanged;
            RenderFavoriteData();
        }

        private void DoRedirect_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                TrainInfoReader.SetRedirect(new InternalSavedDataReader());
            }
            else
            {
                TrainInfoReader.ClearRedirect();
            }
        }

        private void UserConfigManager_ValueChanged(object sender, EventArgs e)
        {
            if (!(Context is null))
            {
                RenderFavoriteData();
            }
        }

        private void RenderFavoriteData()
        {
            favoriteStationLayout.RemoveAllViews();
            favoriteLineLayout.RemoveAllViews();

            var favoriteStations = UserConfigManager.GetFavoriteStations();
            var favoriteLines = UserConfigManager.GetFavoriteJehLines();

            var stationButtonLayout = new HierarchyButtonLayout(Context);
            var lineButtonLayout = new HierarchyButtonLayout(Context);

            var stationButtons = favoriteStations.Select(str =>
            {
                var button = new HierarchyButtonLayout.HierarchyTextButton()
                {
                    Text = str.Name,
                    ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                };
                button.Click += FavoriteStationButton_Click;
                return button;
            });

            var lineButtons = favoriteLines.Select(line =>
            {
                var button = new HierarchyButtonLayout.HierarchyTextButton()
                {
                    Text = line.GetName(),
                    ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                };
                button.Click += FavoriteLineButton_Click;
                return button;
            });

            stationButtonLayout.RootButton.AddChildren(stationButtons);
            lineButtonLayout.RootButton.AddChildren(lineButtons);

            favoriteStationLayout.AddView(stationButtonLayout.Build());
            favoriteLineLayout.AddView(lineButtonLayout.Build());
        }

        private void FavoriteLineButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(Context, new TrainPositionActicity().Class);
            intent.PutExtra("Line", ((HierarchyButtonLayout.HierarchyTextButton)sender).Text);
            StartActivity(intent);
        }

        private void FavoriteStationButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(Context, new TrainTimeActivity().Class);
            intent.PutExtra("station", ((HierarchyButtonLayout.HierarchyTextButton)sender).Text);
            StartActivity(intent);
        }


        /*
        private Station nowdeleting;
        private void FavoriteStationButton_LongClick(object sender, View.LongClickEventArgs e)
        {
            var button = (Button)sender;
            nowdeleting = StationReader.GetStationByName(button.Text);
            new AlertDialog.Builder(Activity)
                .SetTitle("削除確認")
                .SetMessage($"{button.Text}をお気に入りから削除してよろしいですか？")
                .SetPositiveButton("OK", DialogResult)
                .SetNegativeButton("Cancel", DialogResult)
                .Show();
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
        }
        */
    }
}