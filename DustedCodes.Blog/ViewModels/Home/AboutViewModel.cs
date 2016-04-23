using System;

namespace DustedCodes.Blog.ViewModels.Home
{
    public sealed class AboutViewModel : BaseViewModel
    {
        public AboutViewModel() : base("About")
        {
        }

        public static double YearsLivingInLondon
        {
            get
            {
                var dateMovedToLondon = new DateTime(2012, 10, 16);
                var timeLivingInLondon = DateTime.Now - dateMovedToLondon;
                const double daysPerYear = 365.242;
                return timeLivingInLondon.TotalDays / daysPerYear;
            }
        }
    }
}