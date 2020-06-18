namespace Fotografix.UI.BlendModes
{
    public partial class BlendModeListResourceDictionary
    {
        public BlendModeListResourceDictionary()
        {
            InitializeComponent();

            BlendModeList blendModeList = BlendModeList.Create();
            this["BlendModeList"] = blendModeList;
            this["BlendModeListItemConverter"] = new BlendModeListItemConverter(blendModeList);
        }
    }
}
