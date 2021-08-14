using Fotografix.Adjustments;
using System;

namespace Fotografix.Editor.Layers
{
    public sealed class NewAdjustmentLayerCommand : SynchronousDocumentCommand
    {
        public override bool CanExecute(Document document, object parameter)
        {
            return parameter is Type type && type.IsSubclassOf(typeof(Adjustment));
        }

        public override void Execute(Document document, object parameter)
        {
            var type = (Type)parameter;
            var adjustment = (Adjustment)Activator.CreateInstance(type);

            Layer layer = new Layer(adjustment) { Name = type.Name };
            document.Image.Layers.Add(layer);
        }
    }
}
