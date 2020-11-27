using System;
using System.Collections.Generic;

namespace Service.Core
{
    public interface IObjectPool
    {
        void Clear();
    }

    public class ObjectPool<T> where T : class, IObjectPool, new()
    {
        Stack<T> _pool;
        object _lockPool = new object();


        public int Count
        {
            get { return _pool.Count; }
        }


        public ObjectPool()
        {
            _pool = new Stack<T>();
        }


        public ObjectPool(int capacity)
        {
            _pool = new Stack<T>(capacity);

            for(int i = 0; i < capacity; i++)
            {
                T obj = new T();
                lock (_lockPool)
                {
                    _pool.Push(obj);
                }
            }
        }


        public void Push(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null");
            }


            obj.Clear();
            lock (_lockPool)
            {
                _pool.Push(obj);
            }
        }


        public T Pop()
        {
            lock (_lockPool)
            {
                if (_pool.Count == 0)
                {
                    throw new ArgumentNullException("Object cannot be null");
                }


                return _pool.Pop();
            }
        }
    }
}
