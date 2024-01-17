using System.Collections.Generic;
using System;

namespace Utilities
{
    public interface IDispatcher
    {
        void Invoke(Action fn);
    }
    public class CustomDispatcher : IDispatcher
    {
        public List<Action> pending = new List<Action>();
        private static CustomDispatcher instance;
        public void Invoke(Action fn)
        {
            lock (pending)
            {
                pending.Add(fn);
            }
        }
        public void InvokePending()
        {
            lock (pending)
            {
                foreach (var action in pending)
                {
                    action();
                }
                pending.Clear();
            }
        }
        public static CustomDispatcher Instance
        {
            get
            {
                if (instance == null)
                {

                    instance = new CustomDispatcher();
                }
                return instance;
            }
        }
    }
}
