using System;

namespace EventChannels
{
    public class EventChannel<EventType>
    {
        Action<EventType> action;

        public void Raise(EventType type)
        {
            action?.Invoke(type);
        }
    }
}