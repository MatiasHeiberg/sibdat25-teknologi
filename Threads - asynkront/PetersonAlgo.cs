using System;
using System.Diagnostics;
using System.Threading;

namespace Threads___asynkront
{
    public class PetersonAlgo
    {
        private static int state = 5;
        private static object _lockObject = new object();
        private static Mutex _m = new Mutex();

        public static void RunAlgoLock()
        {
            Thread t0 = new Thread(RunMe0);
            Thread t1 = new Thread(RunMe1);

            t0.IsBackground = true;
            t1.IsBackground = true;

            t0.Start();
            t1.Start();

            Console.ReadLine(); // Forventes ikke at rammes med det samme pga afventer brugerinput
        }

        static void RunMe0()
        {
            int i = 0;
            while (true)
            {
                lock (_lockObject)
                {
                    // critical region
                    if (state == 5)
                    {
                        state++;
                        Trace.Assert(state == 6, "Race condition in loop " + i);
                    }
                    state = 5;
                }
                i++;
            }
        }

        static void RunMe1()
        {
            int i = 0;
            while (true)
            {
                lock (_lockObject)
                {
                    // critical region
                    if (state == 5)
                    {
                        state++;
                        Trace.Assert(state == 6, "Race condition in loop " + i);
                    }
                    state = 5;
                }
                i++;
            }
        }

        public static void RunAlgoMutex()
        {
            Thread t0 = new Thread(RunMeMutex0);
            Thread t1 = new Thread(RunMeMutex1);

            t0.IsBackground = true;
            t1.IsBackground = true;

            t0.Start();
            t1.Start();

            Console.ReadLine(); // Forventes ikke at rammes med det samme pga afventer brugerinput
        }

        static void RunMeMutex0()
        {
            int i = 0;
            while (true)
            {
                _m.WaitOne(); // Overload: WaitOne(x) hvor x er antal milisekunder der skal ventes før tråden får mutex'en. 
                try
                {
                    // critical region
                    if (state == 5)
                    {
                        state++;
                        Trace.Assert(state == 6, "Race condition in loop " + i);
                    }
                    state = 5;
                }
                finally
                {
                    _m.ReleaseMutex();
                }
                i++;
            }
        }

        static void RunMeMutex1()
        {
            int i = 0;
            while (true)
            {
                _m.WaitOne(); // Overload: WaitOne(x) hvor x er antal milisekunder der skal ventes før tråden får mutex'en. 
                try
                {
                    // critical region
                    if (state == 5)
                    {
                        state++;
                        Trace.Assert(state == 6, "Race condition in loop " + i);
                    }
                    state = 5;
                }
                finally
                {
                    _m.ReleaseMutex();
                }
                i++;
            }
        }
    }
}

