namespace Fotografix.Xmp
{
    public sealed class XmpProperty
    {
        public XmpProperty(string namespaceUri, string name)
        {
            this.NamespaceUri = namespaceUri;
            this.Name = name;
        }

        public string NamespaceUri { get; }
        public string Name { get; }
    }
}
