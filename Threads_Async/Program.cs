using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Exrecise
{
    class Program
    {
        static List<int> myList = new List<int>();
        delegate void NumberChanger(int n);

        static int count = 0;
        static object baton = new object();

        static byte[] values = new byte[500000000];
        static long[] portionResults;
        static int portionSize;

        static void GenerateInts()
        {
            var rand = new Random(987);
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (byte)rand.Next(10);
            }
        }

        static void SumYourPortion(object portionNumber)
        {
            long sum = 0;
            int portionNumberAsInt = (int)portionNumber;
            int baseIndex = portionNumberAsInt * portionSize;
            for (int i = baseIndex; i < baseIndex + portionSize; i++)
            {
                sum += values[i];
            }
            portionResults[portionNumberAsInt] = sum;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("asdsad");
            Console.WriteLine("thread in main is :" + Thread.CurrentThread.ManagedThreadId);
            Thread thread2 = new Thread(someMethod);
            thread2.Start();

            caller();
            var thread1 = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("thread in main is :" + thread1);
            Console.WriteLine("asdsad");

            HandelingThreads();

            NumberChanger nc = delegate (int x)
            {
                Console.WriteLine("Anonymous Method: {0}", x);
            };
            nc(10);
            nc = new NumberChanger(AddNum);
            nc(5);

            FillValues();
            foreach(var item in Filter())
            {
                Console.WriteLine(item);
            }
        }


        public static void HandelingThreads()
        {
            portionResults = new long[Environment.ProcessorCount];
            portionSize = values.Length / Environment.ProcessorCount;
            GenerateInts();
            Console.WriteLine("summing..");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            long total = 0;
            for (int i = 0; i < values.Length; i++)
            {
                total += values[i];
            }
            watch.Stop();
            Console.WriteLine("total values is :" + total);
            Console.WriteLine("time to sum :" + watch.Elapsed);

            watch.Reset();
            watch.Start();

            Thread[] threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                threads[i] = new Thread(SumYourPortion);
                threads[i].Start(i);
            }
            for (int i = 0; i < Environment.ProcessorCount; i++)
                threads[i].Join();
            long total2 = 0;
            for (int i = 0; i < Environment.ProcessorCount; i++)
                total2 += portionResults[i];
            watch.Stop();
            Console.WriteLine("total values is: " + total2);
            Console.WriteLine("time to sum: " + watch.Elapsed);

            var thread1 = new Thread(IncrementCount);
            var thread2 = new Thread(IncrementCount);
            thread1.Start();
            Thread.Sleep(500);
            thread2.Start();
        }

        static void IncrementCount()
        {
            while (true)
            {
                lock (baton)
                {
                    int temp = count;
                    Thread.Sleep(1000);
                    count = temp + 1;
                    Console.WriteLine(
                        "Thread ID " + Thread.CurrentThread.ManagedThreadId +
                        " Increment count to : " + count);
                }
                Thread.Sleep(1000);
            }

        }

        private static async void caller()
        {
            Task<int> task = new Task<int>(CountCharacters);
            task.Start();
            Console.WriteLine("task number is: " + task.Id);
            var thread = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine("Processing file in thread: " + thread + " Please wait....");
            int count = await task;
            Console.WriteLine(count);
            Console.WriteLine("finished!!!");
        }

        private static int CountCharacters()
        {
            int count = 0;
            for (int i = 0; i < 500; i++)
            {
                count++;
            }
            Console.WriteLine("CountCharacters method thread is: " + Thread.CurrentThread.ManagedThreadId);
            return count;
        }

        static void someMethod()
        {
            Console.WriteLine("do something in thread number: " + Thread.CurrentThread.ManagedThreadId);
        }

        static void FillValues()
        {
            myList.Add(1);
            myList.Add(2);
            myList.Add(3);
            myList.Add(4);
            myList.Add(5);
        }

        static IEnumerable<int> Filter()
        {
            foreach (int i in myList)
            {
                if (i > 3)
                {
                    yield return i;
                }
            }
        }

        static IEnumerable<int> RunningTotal()
        {
            int runningtotal = 0;
            foreach (int i in myList)
            {
                runningtotal += i;
                yield return runningtotal;
            }
        }


        public static void AddNum(int p)
        {
            int num = 15;
            num += p;
            Console.WriteLine("Named Method: {0}", num);
        }
    }
}
