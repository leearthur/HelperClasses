using System.Collections.Generic;
using System.Reflection;

namespace HelperClasses.Tests.PropertyEnumerator
{
    public class CallbackStub
    {
        public List<CallbackInstance> CallbackObjects { get; } = new List<CallbackInstance>();

        public int Count => CallbackObjects.Count;

        public void Callback(object obj, PropertyInfo propertyInfo)
        {
            CallbackObjects.Add(new CallbackInstance(obj, propertyInfo));
        }
    }

    public class CallbackInstance
    {
        public object Object { get; }
        public PropertyInfo PropertyInfo { get; }

        public CallbackInstance(object obj, PropertyInfo propertyInfo)
        {
            Object = obj;
            PropertyInfo = propertyInfo;
        }
    }
}
