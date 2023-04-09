using Windows.ApplicationModel;

namespace Fotografix
{
    public sealed class AboutViewModel
    {
        private readonly Package package;

        public AboutViewModel()
        {
            this.package = Package.Current;
        }

        public string AppTitle => package.DisplayName;

        public string AppTitleWithVersion
        {
            get
            {
                var ver = package.Id.Version;
                return $"{AppTitle} {ver.Major}.{ver.Minor}.{ver.Build}";
            }
        }

        public string ReleaseNotesUri => "https://github.com/lmadhavan/fotografix/releases";
        public string SupportUri => "https://github.com/lmadhavan/fotografix/discussions";
        public string RateAndReviewUri => "ms-windows-store://review/?ProductId=9NBZQK3WVN38";
        public string LicenseUri => "https://raw.githubusercontent.com/lmadhavan/fotografix/master/LICENSE.txt";
    }
}
