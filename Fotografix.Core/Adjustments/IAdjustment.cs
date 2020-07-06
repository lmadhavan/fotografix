using System;
using System.ComponentModel;

namespace Fotografix.Adjustments
{
    public interface IAdjustment : INotifyPropertyChanged, IDisposable
    {
        void Apply(IAdjustmentContext adjustmentContext);
    }
}
