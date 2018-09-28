using System;
using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    public interface IDxRegionalBookListener
    {
        void OnChanged(DxPriceLevelBook book);
    }
}
