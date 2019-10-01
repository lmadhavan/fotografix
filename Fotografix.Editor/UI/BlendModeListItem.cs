namespace Fotografix.Editor.UI
{
    public sealed class BlendModeListItem
    {
        public static readonly BlendModeListItem Separator = new BlendModeListItem();

        private BlendModeListItem()
        {
            this.IsSeparator = true;
        }

        public BlendModeListItem(BlendMode blendMode, string name)
        {
            this.BlendMode = blendMode;
            this.Name = name;
        }

        public BlendMode BlendMode { get; }
        public string Name { get; }
        public bool IsSeparator { get; }
    }
}
