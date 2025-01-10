using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPoolManager<T> : MonoBehaviour
{
    private readonly Func<T> _objectFactory;
    private readonly ConcurrentBag<T> _objects;
    private readonly int _maxSize;

    public ObjectPoolManager(Func<T> objectFactory, int maxSize)
    {
        _objectFactory = objectFactory;
        _objects = new ConcurrentBag<T>();
        _maxSize = maxSize;
    }

    public T GetObject()
    {
        T obj;
        if (_objects.TryTake(out obj))
        {
            return obj;
        }
        return _objectFactory();
    }

    public void PutObject(T obj)
    {
        if (_objects.Count < _maxSize)
        {
            _objects.Add(obj);
        }
        else
        {
            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }

}
