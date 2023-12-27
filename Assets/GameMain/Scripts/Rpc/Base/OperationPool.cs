using System;
using System.Collections.Concurrent;

namespace GameMain.Rpc
{
    public class OperationPool<T>
    {
        private readonly ConcurrentBag<T> m_Pool;
        private readonly Func<T> m_Create;
        private readonly Action<T> m_Clean;

        public OperationPool(Func<T> create, Action<T> clean, int count)
        {
            m_Pool = new ConcurrentBag<T>();
            m_Create = create;
            m_Clean = clean;

            for (var i = 0; i < count; i++)
            {
                m_Pool.Add(m_Create());
            }
        }
        
        public T Take()
        {
            if (m_Pool.TryTake(out T obj))
            {
                return obj;
            }

            return m_Create();
        }

        public void Add(T obj)
        {
            m_Clean(obj);
            m_Pool.Add(obj);
        }
    }
}