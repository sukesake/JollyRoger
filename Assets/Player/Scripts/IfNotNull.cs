using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct IfNotNull<T> : IEnumerable<T>
    where T : class
{
    public T _orginalValue;


    /// <summary>
    /// dont use this. work in progress.
    /// </summary>
    /// <param name="orginalValue"></param>
    public IfNotNull(T orginalValue)
    {
        _orginalValue = orginalValue;
    }



    public T Value
    {
        get { return _orginalValue; }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Enumerable.Repeat(_orginalValue, _orginalValue == null ? 0 : 1).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class MaybeExtensions
{
    public static IfNotNull<T> IfNotNull<T>(this T instance)
        where T : class
    {
        return new IfNotNull<T>(instance);
    }

    public static T Then<T>(this IfNotNull<T> instance) where T : class
    {
        return instance.Value ?? instance._orginalValue;
    }
}