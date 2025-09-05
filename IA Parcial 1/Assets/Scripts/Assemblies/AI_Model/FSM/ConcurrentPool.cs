using System;
using System.Collections.Concurrent;

namespace FSM
{
    public class ConcurrentPool
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentStack<IReseteable>> concurrentPool =
            new ConcurrentDictionary<Type, ConcurrentStack<IReseteable>>();

        public static TReseteable Get<TReseteable>() where TReseteable : IReseteable, new()
        {
            if (!concurrentPool.ContainsKey(typeof(TReseteable)))
                concurrentPool.TryAdd(typeof(TReseteable), new ConcurrentStack<IReseteable>());

            TReseteable value;
            if (concurrentPool[typeof(TReseteable)].Count > 0)
            {
                concurrentPool[typeof(TReseteable)].TryPop(out IReseteable reseteable);
                value = (TReseteable)reseteable;
            }
            else
            {
                value = new TReseteable();
            }

            return value;
        }

        public static void Release<TReseteable>(TReseteable obj) where TReseteable : IReseteable, new()
        {
            obj.Reset();
            concurrentPool[typeof(TReseteable)].Push(obj);
        }
    }
}