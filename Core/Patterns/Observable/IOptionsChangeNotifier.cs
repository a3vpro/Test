using System;
using System.Collections.Generic;
using System.Text;
using VisionNet.Core.Abstractions;

namespace VisionNet.Core.Patterns
{
    public interface IOptionsChangeNotifier<TOptions>
        where TOptions : class, IOptions
    {
        void SubscribeOptionsChanged(string id, EventHandler<TOptions> handler);
        void UnsubscribeOptionsChanged(string id, EventHandler<TOptions> handler);
    }
}
