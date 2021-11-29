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
            
            foreach (ITickable tickable in register.Tickables)
            {
                tickable.Tick();
            }   
        }

        private void FixedUpdate()
        {
            foreach (IFixedTickable fixedTickable in register.FixedTickables)
            {
                fixedTickable.FixedTick();
            } 
        }

        private void LateUpdate()
        {
            foreach (ILateTickable lateTickable in register.LateTickables)
            {
                lateTickable.LateTick();
            } 
        }

        private void OnDestroy()
        {
            foreach (IDisposable disposable in register.Disposables)
            {
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