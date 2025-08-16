using System;
using UnityEngine;

namespace Rehawk.Lifecycle
{
    public class LifecyclePoller : MonoBehaviour
    {
        private PollerRegister register;
        
        public void Setup(PollerRegister register)
        {
            this.register = register;
        }
        
        private void Start()
        {
            HandleInitializables();
        }

        private void Update()
        {
            HandleInitializables();

            for (int i = 0; i < register.Tickables.Count; i++)
            {
                ITickable tickable = register.Tickables[i];
                tickable.Tick();
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < register.FixedTickables.Count; i++)
            {
                IFixedTickable fixedTickable = register.FixedTickables[i];
                fixedTickable.FixedTick();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < register.LateTickables.Count; i++)
            {
                ILateTickable lateTickable = register.LateTickables[i];
                lateTickable.LateTick();
            }
        }

        private void OnApplicationQuit()
        {
            for (int i = 0; i < register.Disposables.Count; i++)
            {
                IDisposable disposable = register.Disposables[i];
                disposable.Dispose();
            }
        }

        private void HandleInitializables()
        {
            while (register.TryGetNextInitializable(out IInitializable initializable))
            {
                initializable.Initialize();
            }
        }
    }
}