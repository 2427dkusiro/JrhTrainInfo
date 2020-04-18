using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang.Annotation;
using Org.Apache.Http.Client;

namespace JrhTrainInfoAndroid
{
    class TapToDeployLayout
    {
        public TapToDeployLayout(Context context)
        {
            this.context = context;
            resultView = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical,
            };
            InitializeLayoutParams();
        }

        private readonly Context context;

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;

                if (rendered)
                {
                    InitializeTitleView();
                    RenderView();
                }
            }
        }

        private const int TextSize = 20;

        private View view;
        public View View
        {
            get => view;
            set
            {
                view = value;
                LinearLayout linearLayout = new LinearLayout(context);
                linearLayout.AddView(value, MP_WP_LayoutParams);
                userView = linearLayout;

                if (rendered)
                {
                    InitializeTitleView();
                    RenderView();
                }
            }
        }

        private View userView;

        private Color color;
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                if (rendered)
                {
                    InitializeTitleView();
                    RenderView();
                }
            }
        }

        public bool IsDeployed { get; private set; } = true;

        private bool rendered = false;

        private LinearLayout resultView;

        private RelativeLayout.LayoutParams titleTextViewLayoutParams;
        private RelativeLayout.LayoutParams deployIconlayoutParams;

        private ViewGroup.LayoutParams MP_WP_LayoutParams;

        private View deployTitleView;
        private View deployedTitleView;

        public void SetStatus(bool isDeployed)
        {
            if (isDeployed)
            {
                userView.Visibility = ViewStates.Visible;
            }
            else
            {
                userView.Visibility = ViewStates.Gone;
            }
        }

        public View GetView()
        {
            InitializeTitleView();
            RenderView();
            return resultView;
        }

        private void InitializeLayoutParams()
        {
            titleTextViewLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            titleTextViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            titleTextViewLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            titleTextViewLayoutParams.LeftMargin = 20;
            titleTextViewLayoutParams.TopMargin = 10;
            titleTextViewLayoutParams.BottomMargin = 10;

            deployIconlayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            deployIconlayoutParams.AddRule(LayoutRules.CenterVertical);
            deployIconlayoutParams.AddRule(LayoutRules.AlignParentEnd);
            deployIconlayoutParams.Height = 60;
            deployIconlayoutParams.Width = 60;
            deployIconlayoutParams.RightMargin = 20;

            MP_WP_LayoutParams = new ViewGroup.LayoutParams(-1, -2);
        }

        private void InitializeTitleView()
        {
            deployedTitleView = RenderTitle(true);
            deployTitleView = RenderTitle(false);
        }

        private View RenderTitle(bool isDeployed)
        {
            RelativeLayout titleRelativeLayout = new RelativeLayout(context);
            titleRelativeLayout.SetBackgroundColor(color);
            titleRelativeLayout.Click += TitleRelativeLayout_Click;

            TextView titleTextView = new TextView(context)
            {
                Text = title,
                TextSize = TextSize,
            };
            titleRelativeLayout.AddView(titleTextView, titleTextViewLayoutParams);

            var deployImageView = new ImageView(context);
            deployImageView.SetImageResource(isDeployed ? Resource.Drawable.DeployedIcon : Resource.Drawable.DeployIcon);
            deployImageView.SetScaleType(ImageView.ScaleType.FitXy);
            deployImageView.SetAdjustViewBounds(true);
            titleRelativeLayout.AddView(deployImageView, deployIconlayoutParams);

            return titleRelativeLayout;
        }

        private void RenderView()
        {
            resultView.RemoveAllViews();

            resultView.AddView(IsDeployed ? deployedTitleView : deployTitleView, MP_WP_LayoutParams);

            resultView.AddView(userView, MP_WP_LayoutParams);
        }

        private void TitleRelativeLayout_Click(object sender, EventArgs e)
        {
            IsDeployed = !IsDeployed;
            SetStatus(IsDeployed);
            RenderView();
        }
    }
}