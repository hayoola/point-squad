using System;
using System.Diagnostics;

namespace C1Q1
{
    class Program
    {
        private static readonly int[][] Inputs =
        {
            new[] {2, 3, 5, 20},
            new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10},
            new[] {1, 2, 3, 4, 5, 6, 7},
            new[] {4, 18, 22, 10, 4},
            new[] {2, 3, 4, 1, 6},
            new[] {10, 20, 3, 15, 1000, 60, 16}
        };

        private static readonly int[] ExpectedOutputs =
        {
            0,
            0,
            12,
            32,
            9,
            41
        };

        static void Main()
        {
            RunTests();
        }

        static void RunTests()
        {
            var inputs = Inputs;
            var outputs = ExpectedOutputs;
            var allTestsDuration = 0;

            for (int testIndex = 0; testIndex < inputs.Length; testIndex++)
            {
                var input = inputs[testIndex];
                var expected = outputs[testIndex];

                Stopwatch sw = new Stopwatch();

                sw.Start();
                var solution = new Solution();
                var output = solution.Solve(input);
                sw.Stop();

                var duration = (int) sw.Elapsed.TotalMilliseconds;
                allTestsDuration += duration;

                bool success = output == expected;

                if (success)
                {
                    Console.WriteLine($"+ Test {testIndex}: PASSED. Returned {output}. Time taken: {duration:D5}ms");
                }
                else
                {
                    Console.WriteLine($"- Test {testIndex}: FAILED. Expected {expected}, " +
                                      $"returned {output}. Time taken: {duration:D5}ms");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Total test run duration: {allTestsDuration:N0}ms");
            Console.ReadKey();
        }
    }
}