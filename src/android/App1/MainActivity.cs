using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using TrainInfo;
using TrainInfo.Debuggers;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using SearchView = Android.Support.V7.Widget.SearchView;

namespace JrhTrainInfoAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Window.SetStatusBarColor(new Android.Graphics.Color(Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.colorPrimary)));

            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.mainTopToolbar2));

            var myFragmentPagerAdapter = new MyFragmentPagerAdapter(SupportFragmentManager);
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewPager1);
            viewPager.Adapter = myFragmentPagerAdapter;

            var tabLayout = FindViewById<Android.Support.Design.Widget.TabLayout>(Resource.Id.tabLayout1);
            tabLayout.SetupWithViewPager(viewPager);

            var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
            for (var i = 0; i < 3; i++)
            {
                var tab = tabLayout.GetTabAt(i);
                var tab1View = inflater.Inflate(GetTabLayout(i), null);
                tab.SetCustomView(tab1View);
            }
        }

        /// <summary>
        /// タブの位置番号から表示すべきタブを返します。
        /// </summary>
        /// <param name="index">タブの位置を表す番号。</param>
        /// <returns></returns>
        private int GetTabLayout(int index)
        {
            switch (index)
            {
                case 0:
                    return Resource.Layout.CustomTabLayout_HomeTab;
                case 1:
                    return Resource.Layout.CustomTabLayout_TrainTimeTab;
                case 2:
                    return Resource.Layout.CustomTabLayout_TrainPositionTab;

                default:
                    throw new NotSupportedException();
            }
        }

        SearchView searchView;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.search1, menu);

            var searchItem = menu.FindItem(Resource.Id.action_search);

            searchView = searchItem.ActionView.JavaCast<SearchView>();

            if (!(searchView is null))
            {
                searchView.QueryTextSubmit += (sender, args) =>
                {
                    Toast.MakeText(this, "You searched: " + args.Query, ToastLength.Short).Show();
                };
            }
            return base.OnCreateOptionsMenu(menu);

        }
    }

    [Obsolete("デバッグ専用")]
    internal class SampleFragment : Android.Support.V4.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var textView = new TextView(Activity)
            {
                Gravity = GravityFlags.Center,
                Text = "未実装のタブです"
            };

            return textView;
        }
    }
}