namespace CrazyEaters.Optimization
{
    using Godot;
    using Godot.Collections;
    using System;
    using System.Collections;

    public class ResourceQueue : Godot.Object
    {

        Thread thread;
        Mutex mutex;
        Semaphore sem;

        int timeMax = 100;

        Queue queue;
        Dictionary pending;

        public void _lock(Godot.Object _caller)
        {
            mutex.Lock();
        }

        public void _unlock(Godot.Object _caller)
        {
            mutex.Unlock();
        }

        public void _post(Godot.Object _caller)
        {
            sem.Post();
        }

        public void _wait(Godot.Object _caller)
        {
            sem.Wait();
        }

        public void Start()
        {
            mutex = new Mutex();
            sem = new Semaphore();
            thread = new Thread();
            thread.Start(this, nameof(ThreadFunc), 0);
        }

        public void ThreadFunc()
        {
            while (true) {
                ThreadProcess();
            }
        }

        public void ThreadProcess()
        {
            
        }

    }
}