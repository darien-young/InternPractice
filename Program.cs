using System;
using System.ComponentModel.Design;
using System.Linq;

namespace InternConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //This program already works. Your task is to change what happens after the input,
            //and to ensure the correct datatypes are being used for the inputs.
            string name = "";
            while (true)
            {
                // Ask for user's name
                Console.Write("Enter your name: ");
                name = Console.ReadLine();

                //Checking for only letter inputs
                if (!string.IsNullOrWhiteSpace(name) && name.All(char.IsLetter))
                {
                    break;
                }

                Console.WriteLine("Invalid Input. Please use letters only (no numbers or symbols)");

            }

            //Setting age integer to then be updated by string parsing
            int age = 0;
            while (true)
            {
                // Ask for user's age
                Console.Write("Enter your age: ");
                string ageInput = Console.ReadLine();

                //checking for only numeric inputs
                if (int.TryParse(ageInput, out age) && age >= 0)
                {
                    break;
                }
                Console.WriteLine("Invalid Input. Please use Numbers Only (No letters or symbols)"); 
            }                                   //Did you mean )" instead of ")? It cuts off the console command output prematurely.
                                                // I meant )") actually. I needed a closing bracket both before ant after the close quote.

            //Boolean for Age-dependent Message

            // Let's have more age ranges added:
            // 1 - 3: "Hello, {name},\nlet's go to the nursery."
            // 4 - 12: "Hello, {name},\nlet's go to primary school."
            // 13 - 17: "Hello, {name},\nlet's go to high school."
            // >75: "Hello, {name},\nlet's go to the nursery home."
            //max: readded the age18-based conditions as extra lines of output to the applicable cases
            // -- while adding new cases for those unapplicable ages.
            Console.WriteLine("\n");
            switch (age)
            {
                case int n when (n >= 1 && n <= 3):
                    Console.WriteLine($"Hello, {name},\nlet's go to the nursery.");
                    Console.WriteLine($"Hello, {name},\nlet's go to class.");
                    break;
                case int n when (n >= 4 && n <= 12):
                    Console.WriteLine($"Hello, {name},\nlet's go to Primary School.");
                    Console.WriteLine($"Hello, {name},\nlet's go to class.");
                    break;
                case int n when (n >= 13 && n <= 17):
                    Console.WriteLine($"Hello, {name},\nlet's go to High School.");
                    Console.WriteLine($"Hello, {name},\nlet's go to class.");
                    break;
                case int n when (n > 75):
                    Console.WriteLine($"Hello, {name},\nlet's go to the nursery home.");
                    Console.WriteLine($"Hello, {name},\nlet's go for a drink.");
                    break;
                case int n when (n > 18):
                    Console.WriteLine($"Hello, {name},\nlet's go for a drink.");
                    break;

                case int n when (n == 18):
                    Console.WriteLine($"Hello, {name},\nlet's have fun this year.");
                    break;
            }

                    //Console.WriteLine(); 
                    //Fixed
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            return; // Added return statement to explicitly indicate the end of Main method
                    // still needs two keystrokes on this laptop to exit. 
        }
    }
}
