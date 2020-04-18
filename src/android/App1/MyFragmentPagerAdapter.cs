using Android.Support.V4.App;
using App1.Resources.layout;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace App1
{
    public class MyFragmentPagerAdapter : FragmentPagerAdapter
    {
        public MyFragmentPagerAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {

        }
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return null;
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new AppHomeFragment();
                case 1:
                    return new TrainTimeStationSearchFragment();
                case 2:
                    return new TrainPositionLineSearchFragment();
                default:
                    return new SampleFragment();
            }
        }
        public override int Count => 4;
    }
}