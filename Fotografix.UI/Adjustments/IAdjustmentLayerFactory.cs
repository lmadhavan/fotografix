using System;
using System.Collections.Generic;
using System.Text;

namespace Fotografix.UI.Adjustments
{
    public interface IAdjustmentLayerFactory
    {
        AdjustmentLayer CreateAdjustmentLayer();
    }
}
