namespace Fotografix.Testing
{
    public class FakeLayer : Layer
    {
        public override void Accept(LayerVisitor visitor)
        {
        }

        public new void RaiseContentChanged()
        {
            base.RaiseContentChanged();
        }
    }
}
