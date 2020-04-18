using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.Views;
using Android.Widget;

namespace App1
{
    public partial class HierarchyButtonLayout
    {
        public HierarchyButtonLayout(Context context)
        {
            RootButton = new RootHierarchyButton();
            BackGroundColor = new byte[] { 255, 245, 245, 245 };
            PressedBackGroundColor = new byte[] { 255, 215, 215, 215 };
            this.context = context;

            mainLinearLayout = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical
            };

            currentParent = RootButton;
        }

        static HierarchyButtonLayout()
        {
            InitializeLayoutParams();
        }

        private static void InitializeLayoutParams()
        {
            buttonLayoutParams = new ViewGroup.LayoutParams(RelativeLayout.LayoutParams.MatchParent, 180);
            horizontalLineLayoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 3);
        }

        /// <summary>
        /// ルート要素を表すボタンを取得します。このボタンは実際には表示されず、このボタンの小要素が最上位要素となります。
        /// </summary>
        public RootHierarchyButton RootButton { get; private set; }

        /// <summary>
        /// 戻るボタンを表示するかどうかを表す値を取得または設定します。
        /// 既定値は<code>true</code>です。
        /// </summary>
        public bool ShowBackButton { get; set; } = true;

        /// <summary>
        /// ARGB形式のボタン背景色を表す配列を取得します。
        /// </summary>
        public byte[] BackGroundColor { get; private set; }

        /// <summary>
        /// ARGB形式のボタンが押されたときのボタン背景色を表す配列を取得します。
        /// </summary>
        public byte[] PressedBackGroundColor { get; private set; }

        private readonly Dictionary<int, HierarchyButton> buttonDictionary = new Dictionary<int, HierarchyButton>();
        private HierarchyButton currentParent;

        private readonly Context context;
        private LinearLayout mainLinearLayout;

        private static ViewGroup.LayoutParams buttonLayoutParams;
        private static ViewGroup.LayoutParams horizontalLineLayoutParams;


        /// <summary>
        /// 階層ボタンを構築します。
        /// </summary>
        /// <returns>階層ボタンを表す<see cref="View"/>。</returns>
        public View Build()
        {
            BuildLayout(RootButton);
            return mainLinearLayout;
        }

        /// <summary>
        /// 一つ前の階層に戻ることを試みます。
        /// </summary>
        /// <returns>戻る処理が成功したかどうかを表す値</returns>
        public bool Back()
        {
            if (currentParent.Parent is null)
            {
                return false;
            }
            else
            {
                currentParent = currentParent.Parent;
                BuildLayout(currentParent);
                return true;
            }
        }

        private void BuildLayout(HierarchyButton parentHierarchyButton)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            mainLinearLayout.RemoveAllViews();
            var IsFirst = true;

            if (ShowBackButton && (currentParent.Parent != null))
            {
                var backButton = new BackHierarchyButton(parentHierarchyButton);
                mainLinearLayout.AddView(CreateRelativeLayoutButton(backButton), buttonLayoutParams);
                IsFirst = false;
            }

            foreach (var button in parentHierarchyButton.Children)
            {
                if (!IsFirst)
                {
                    mainLinearLayout.AddView(CreateUnderLine(), horizontalLineLayoutParams);
                }

                var layout = CreateRelativeLayoutButton(button);
                mainLinearLayout.AddView(layout, buttonLayoutParams);

                IsFirst = false;
            }

            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }


        private ViewGroup CreateRelativeLayoutButton(HierarchyButton hierarchyButton)
        {
            var buttonRelativeLayout = hierarchyButton.BuildView(context);
            buttonRelativeLayout.SetPadding(50, 0, 50, 0);
            buttonRelativeLayout.Clickable = true;

            var stateListDrawable = new StateListDrawable();
            stateListDrawable.AddState(new int[] { -Android.Resource.Attribute.StatePressed }, new ColorDrawable(Android.Graphics.Color.Argb
                (BackGroundColor[0], BackGroundColor[1], BackGroundColor[2], BackGroundColor[3])));
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StatePressed }, new ColorDrawable(Android.Graphics.Color.Argb
                (PressedBackGroundColor[0], PressedBackGroundColor[1], PressedBackGroundColor[2], PressedBackGroundColor[3])));

            buttonRelativeLayout.Background = stateListDrawable;
            buttonRelativeLayout.Click += ButtonRelativeLayout_Click;

            var id = View.GenerateViewId();
            buttonRelativeLayout.Id = id;
            buttonDictionary.Add(id, hierarchyButton);

            /*
            if (showUnderLine)
            {
                var horizontalLine = new View(context);
                horizontalLine.SetBackgroundColor(Android.Graphics.Color.Argb(255, 180, 180, 180));
                buttonRelativeLayout.AddView(horizontalLine, horizontalLineLayoutParams);
            }
            */

            return buttonRelativeLayout;
        }

        private View CreateUnderLine()
        {
            var horizontalLine = new View(context);
            horizontalLine.SetBackgroundColor(Android.Graphics.Color.Argb(255, 180, 180, 180));
            return horizontalLine;
        }

        private void ButtonRelativeLayout_Click(object sender, EventArgs e)
        {
            var clickedLayout = (RelativeLayout)sender;
            var clickedButton = buttonDictionary[clickedLayout.Id];

            Debug.WriteLine($"Button Clicked! {clickedButton.Description}");

            var args = new HierarchyButtonClickEventArgs();
            clickedButton.OnClick(args);

            currentParent = args.NextCurrentHierarchyButton ?? currentParent;
            if (args.LayoutChanged)
            {
                BuildLayout(currentParent);
            }
        }

        public class HierarchyButtonClickEventArgs : EventArgs
        {
            public HierarchyButton NextCurrentHierarchyButton { get; set; }
            public bool LayoutChanged { get; set; }
        }

        public class RootHierarchyButton : HierarchyButton
        {
            public override string Description { get; set; }

            public RootHierarchyButton() { }

            public override void OnClick(HierarchyButtonClickEventArgs e)
            {
                throw new InvalidOperationException();
            }

            public override ViewGroup BuildView(Context context)
            {
                throw new InvalidOperationException();
            }

            public override event EventHandler Click;
        }
    }
}