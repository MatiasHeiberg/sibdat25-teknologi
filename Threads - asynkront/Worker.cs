using System;
using System.Collections.Generic;
using System.Text;

namespace Threads___asynkront
{
    public class Animator
    {
        private string[] _data;
        private readonly Thread _internalThread;
        private readonly CancellationToken _running;

        public Animator(string[] data, CancellationToken token)
        {
            _data = data;
            _internalThread = new Thread(Method);
            _running = token;
        }

        private void Method()
        {
            while (!_running.IsCancellationRequested)
                Array.ForEach<string>(_data, s =>
                {
                    Console.WriteLine($"{s}\n");
                    Thread.Sleep(85);
                });
        }

        public void Start() => _internalThread.Start();
    }
}
