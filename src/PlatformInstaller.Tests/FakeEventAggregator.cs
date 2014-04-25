using System;
using Caliburn.Micro;
using Action = System.Action;

public class FakeEventAggregator : IEventAggregator
{
    public void Subscribe(object instance)
    {
    }

    public void Unsubscribe(object instance)
    {
    }

    public void Publish(object message)
    {
    }

    public void Publish(object message, Action<Action> marshal)
    {
    }

    public Action<Action> PublicationThreadMarshaller { get; set; }

    public bool HandlerExistsFor(Type messageType)
    {
        return true;
    }
}