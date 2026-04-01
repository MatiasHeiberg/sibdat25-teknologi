using System.Security.Cryptography.X509Certificates;

namespace App;

public class Program
{
    public static void Main(string[] args)
    {
        // Oprettede simple linjer kode for at aflæse hvordan det oversættes til assembly.
        
        var num = 1;
        var str = "test";
        var dec = 1.0m;

        num += 1;
        dec -= 2;
        dec += num;
        
        // Brugt delegate Comparison til at sortere en liste med custom logik/metoder.

        static int FilterByDESC(int a, int b)
        {
            if (a < b) return 1;
            if (a > b) return -1;
            return 0;
        }

        static int FilterByASC(int a, int b)
        {
            if (a < b) return -1;
            if (a > b) return 1;
            return 0;
        }

        List<int> list = [1, 5, 23, 6432, 0, 2];
        list.Sort(new Comparison<int>(FilterByASC));
        list.Sort(new Comparison<int>(FilterByDESC));

        // Lav 2 metoder

        int Add(int a, int b) =>  a + b;
        int Sub(int a, int b) =>  a - b;

        // Opret et array af delegates med samme signatur som ovenstående
        
        Func<int, int, int>[] ops = [Add, Sub];

        // Opret metode der bruger egen metode til at printe til consollen
        void printToConsole<T>(T value) => Console.WriteLine(value);
        void printOperations(Action<int> print, Func<int, int, int>[] ops, (int x,int y)[] nums)
        {
            foreach (var (x, y) in nums)
            {
                foreach (var op in ops)
                    print(op(x, y));

                Console.WriteLine("\n-----\n");
            }
        }

        (int x, int y)[] nums = [(2, 2), (5, 7), (10, 25)];
        printOperations(printToConsole<int>, ops, nums);
    }
}