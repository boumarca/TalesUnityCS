using System;

namespace Framework.Common
{
    public interface IEventRegistrator<T> 
    {
        public void RegisterEvent(EventHandler<T> eventHandler);
    }
}
