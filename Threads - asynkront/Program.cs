namespace Threads___asynkront
{
    internal class Program
    {
        private bool _controller = true;
        static void Main(string[] args)
        {
            //Program.Opgave1();
            //Program.Opgave2();
            Program.Opgave3();
        }
        public static void Opgave1()
        {
            // Lav en thread der kalder en metode der printer til skærmen
            // Tilpas metoden så den holdes i live (while løkke)
            // Udvid så den kan lukkes ned sikkert (ikke brug while(true))
            Program obj = new Program();
            var thread = new Thread(() => Method(obj));
            thread.Start();
            Console.ReadLine();
            obj._controller = false;

            static void Method(Program obj)
            {
                while (obj._controller)
                    Console.WriteLine("Second thread is running");
                Console.WriteLine("Second thread terminated");
            }
        }

        public static void Opgave2()
        {
            // Håndtere tråde og data på mindst 3 ud af de 4 måder vist i slides 

            // Løsning 1: Parameterized Thread der bruger injected data. Dette er den gamle metode hvor dataen skal injektes (med ThreadStart()) som Object typen og castes inde i metoden. 
            ParameterizedThreadStart();

            // Løsning 2: Wrap metoden ind i en klasse med en private data variabel der initialiseres via konstruktoren. Klienten ejer Thread objektet og injekter dataen.
            // Ikke implementeret

            // Løsning 3: Ligesom løsning 1, men med lambda frem for ThreadStart(). Dette er den moderne syntaks for inline threads. For mere komplekse implementereinger bruges løsning 4.
            LambdaThread();

            // Løsning 4: Pak tråd ind i en "worker" klasse. Dataen/animationen er injected men den kunne også være eget af worker klassen.
            InvokeWorker();

            static void LambdaThread()
            {
                string msg = "Hello World";
                Thread T = new Thread(() => ThreadWithParameters(msg));

                T.Start();

                static void ThreadWithParameters(string message) => Console.WriteLine(message);
            }

            static void InvokeWorker()
            {
                string[] animation = ["|", "/", "-", "\\"];
                var tokenSource = new CancellationTokenSource();
                var worker = new Animator(animation, tokenSource.Token);
                worker.Start();

                Console.ReadLine();
                tokenSource.Cancel();
            }

            static void ParameterizedThreadStart()
            {
                string msg = "Hello World";
                Thread T = new Thread(ThreadWithParameters); // ThreadWithParameters bliver implicit kaldt til ThreadStart() delegaten.

                T.Start(msg);

                static void ThreadWithParameters(object o)
                {
                    string message = (string)o;
                    Console.WriteLine(message);
                }
            }
            

        }

        public static void Opgave3()
        {
            // Ved at lade T1 være en background thread lukker den ned når main thread stopper. Hvilket den gør når dens call stack er tom => efter M2 er færdig.
            Thread T1 = new Thread(() => M1());
            Thread T2 = new Thread(() => M2());

            T1.IsBackground = true;
            T1.Start();
            T2.Start();

            void M1() { while (true) Console.WriteLine('.'); }
            void M2() { for (int i = 0; i < 1000; i++) Console.WriteLine('x'); }
        }
    }
}
