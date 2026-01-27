using System;

namespace InternConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //This program already works. Your task is to change what happens after the input,
            //and to ensure the correct datatypes are being used for the inputs.

            // Ask for user's name
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            // Ask for user's age
            Console.Write("Enter your age: ");
            int age = int.Parse(Console.ReadLine());

            // Generic output 
            Console.WriteLine();
            Console.WriteLine($"Hello {name}!");
            Console.WriteLine($"You are {age} years old.");

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
