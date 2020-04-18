using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace App1.Resources.layout
{
    public class TrainPositionLineSearchFragment : Android.Support.V4.App.Fragment
    {
        private LinearLayout linearLayout;
        private RelativeLayout[] relativeLayouts;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.MainTabFragmentLayout_TrainPositionLineSearch, container, false);
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            linearLayout = view.FindViewById<LinearLayout>(Resource.Id.TrainPosLineSearchLinearLayout);
            CreateButtons();
        }

        private void CreateButtons()
        {
            var jrhLine = new[] { JrhLine.Hakodate_Iwamizawa, JrhLine.Hakodate_Otaru, JrhLine.Chitose_Tomakomai, JrhLine.Sassyo };
            relativeLayouts = CreateRelativeLayoutButton(jrhLine.Length, LineButton_Click);

            var relativeLayoutLayoutParams = new LinearLayout.LayoutParams(-1, -2);
            var textViewLayoutParams = new RelativeLayout.LayoutParams(-1, -2);
            textViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            textViewLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            textViewLayoutParams.LeftMargin = 40;

            for (var i = 0; i < jrhLine.Length; i++)
            {
                var textView = new TextView(Context)
                {
                    Text = jrhLine[i].GetName(),
                    TextSize = 18,
                };
                relativeLayouts[i].AddView(textView, textViewLayoutParams);
                linearLayout.AddView(relativeLayouts[i], relativeLayoutLayoutParams);
            }
        }

        /// <summary>
        /// タップ可能な<c>RelativeLayout</c>を作成します。
        /// </summary>
        /// <param name="count">作成する個数。</param>
        /// <param name="eh">タップされた場合に実行されるイベントハンドラ。</param>
        /// <returns></returns>
        private RelativeLayout[] CreateRelativeLayoutButton(int count, EventHandler eh)
        {
            var relativeLayouts = new RelativeLayout[count];
            var IsFirst = true;

            for (var i = 0; i < count; i++)
            {
                var HorizontalLineLayoutParams = new ViewGroup.LayoutParams(-1, -2)
                {
                    Height = 3
                };

                var relativeLayout = new RelativeLayout(Context);
                relativeLayout.SetMinimumHeight(180);
                relativeLayout.SetPadding(50, 0, 50, 0);

                var stateListDrawable = new StateListDrawable();
                stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StatePressed }, new ColorDrawable(Android.Graphics.Color.Argb(255, 215, 215, 215)));
                stateListDrawable.AddState(new int[] { -Android.Resource.Attribute.StatePressed }, new ColorDrawable(Android.Graphics.Color.Argb(255, 245, 245, 245)));

                relativeLayout.Clickable = true;
                relativeLayout.Background = stateListDrawable;
                relativeLayout.Click += eh;

                if (IsFirst)
                {
                    IsFirst = false;
                }
                else
                {
                    var horizontalLine = new View(Context);
                    horizontalLine.SetBackgroundColor(Android.Graphics.Color.Argb(255, 180, 180, 180));
                    horizontalLine.SetMinimumHeight(3);
                    relativeLayout.AddView(horizontalLine, HorizontalLineLayoutParams);
                }
                relativeLayouts[i] = relativeLayout;
            }
            return relativeLayouts;
        }

        private void LineButton_Click(object sender, EventArgs e)
        {
            var relativeLayout = (RelativeLayout)sender;
            var textView = relativeLayout.GetChildAt(relativeLayout.ChildCount - 1) as TextView;
            if (textView != null)
            {
                var intent = new Intent(Context, new TrainPositionActicity().Class);
                intent.PutExtra("Line", textView.Text);
                StartActivity(intent);
            }
        }
    }
}