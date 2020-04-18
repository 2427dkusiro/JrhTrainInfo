using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace JrhTrainInfoAndroid.Resources.layout
{
    public class TrainPositionLineSearchFragment : Android.Support.V4.App.Fragment
    {
        private LinearLayout linearLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.MainTabFragmentLayout_TrainPositionLineSearch, container, false);
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            linearLayout = view.FindViewById<LinearLayout>(Resource.Id.TrainPosLineSearchLinearLayout);

            var hierarchyButtonLayout = new HierarchyButtonLayout(Context);
            CreateButtons(hierarchyButtonLayout.RootButton);
            linearLayout.AddView(hierarchyButtonLayout.Build());
        }

        private static readonly JrhLine[] firstButtons = new[] { JrhLine.Hakodate_Iwamizawa, JrhLine.Hakodate_Otaru, JrhLine.Chitose_Tomakomai, JrhLine.Sassyo, JrhLine.Sekisyo };

        private void CreateButtons(HierarchyButtonLayout.RootHierarchyButton rootHierarchyButton)
        {
            var buttons = firstButtons.Select(line =>
             {
                 var button = new HierarchyButtonLayout.HierarchyTextButton()
                 {
                     Text = line.GetName(),
                     ArrowSignDirection = HierarchyButtonLayout.HierarchyTextButton.ArrowSignDirections.Right,
                 };
                 button.Click += LineButton_Click;
                 return button;
             });
            rootHierarchyButton.AddChildren(buttons);
        }

        private void LineButton_Click(object sender, EventArgs e)
        {
            var button = (HierarchyButtonLayout.HierarchyTextButton)sender;
            var intent = new Intent(Context, new TrainPositionActicity().Class);
            intent.PutExtra("Line", button.Text);
            StartActivity(intent);
        }
    }
}