namespace Fotografix.Uwp.BlendModes
{
    public sealed class BlendModeListItemConverter : ValueConverter<BlendMode, BlendModeListItem>
    {
        private readonly BlendModeList blendModeList;

        public BlendModeListItemConverter(BlendModeList blendModeList)
        {
            this.blendModeList = blendModeList;
        }

        public override BlendModeListItem Convert(BlendMode value)
        {
            return blendModeList[value];
        }

        public override BlendMode ConvertBack(BlendModeListItem value)
        {
            return value.BlendMode;
        }
    }
}