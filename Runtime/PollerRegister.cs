using System;
using System.Collections.Generic;

namespace Rehawk.Lifecycle
{
    public class PollerRegister
    {
        private readonly Queue<IInitializable> initializables = new Queue<IInitializable>();
        private readonly List<ITickable> tickables = new List<ITickable>();
        private readonly List<IFixedTickable> fixedTickables = new List<IFixedTickable>();
        private readonly List<ILateTickable> lateTickables = new List<ILateTickable>();
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public IReadOnlyList<ITickable> Tickables
        {
            get { return tickables; }
        }

        public IReadOnlyList<IFixedTickable> FixedTickables
        {
            get { return fixedTickables; }
        }

        public IReadOnlyList<ILateTickable> LateTickables
        {
            get { return lateTickables; }
        }

        public IReadOnlyList<IDisposable> Disposables
        {
            get { return disposables; }
        }

        public void Add<T>(T instance)
        {
            if (instance is IInitializable initializable)
            {
                initializables.Enqueue(initializable);
            }
            
            if (instance is ITickable tickable)
            {
                tickables.Add(tickable);
            }
            
            if (instance is IFixedTickable fixedTickable)
            {
                fixedTickables.Add(fixedTickable);
            }

            if (instance is ILateTickable lateTickable)
            {
                lateTickables.Add(lateTickable);
            }

            if (instance is IDisposable disposable)
            {
                disposables.Add(disposable);
            }
        }

        public bool TryGetNextInitializable(out IInitializable initializable)
        {
            if (initializables.Count == 0)
            {
                initializable = null;
                return false;
            }
            
            initializable = initializables.Dequeue();
            return true;
        }

        public void Clear()
        {
            initializables.Clear();
            tickables.Clear();
            fixedTickables.Clear();
            lateTickables.Clear();
            disposables.Clear();
        }
    }
}