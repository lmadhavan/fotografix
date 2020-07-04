namespace Fotografix.Testing
{
    public class FakeLayer : Layer
    {
        public new void RaiseContentChanged()
        {
            base.RaiseContentChanged();
        }
    }
}
