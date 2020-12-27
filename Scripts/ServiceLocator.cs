using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> _abstractionMap = new Dictionary<Type, object>();

    public static void Bind<T>(T service)
    {
        _abstractionMap.Add(typeof(T), service);
    }

    public static void Locate<T>(ref T service)
    {
        Object ret;
        bool success = _abstractionMap.TryGetValue(typeof(T), out ret);

        if (success)
            service = (T)ret;
    }
}