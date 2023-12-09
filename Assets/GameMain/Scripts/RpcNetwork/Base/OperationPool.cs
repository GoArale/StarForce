using System;
using System.Collections.Concurrent;

namespace GameMain.RpcNetwork
{
    public class OperationPool<T>
    {
        private ConcurrentBag<T> m_Pool;
        private Func<T> m_Create;
        private Action<T> m_Clean;

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