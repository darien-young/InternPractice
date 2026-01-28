using System;
using System.ComponentModel.Design;
using System.Linq;

namespace InternConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Phase 2:
            // Instead of asking for an age, ask for the user's Birth Year and calculate the age based on the current year dynamically before output.

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
            }                                   

            //Boolean for Age-dependent Message

            Console.WriteLine();
            switch (age)
            {
                case int n when (n >= 1 && n <= 3):
                    Console.WriteLine($"Hello, {name},\nlet's go to the nursery.");
                    break;
                case int n when (n >= 4 && n <= 12):
                    Console.WriteLine($"Hello, {name},\nlet's go to Primary School.");
                    break;
                case int n when (n >= 13 && n <= 17):
                    Console.WriteLine($"Hello, {name},\nlet's go to High School.");
                    break;
                case int n when (n == 18):
                    Console.WriteLine($"Hello, {name},\nlet's have fun this year.");
                    break;
                case int n when (n > 18 && n <= 75):
                    Console.WriteLine($"Hello, {name},\nlet's go for a drink.");
                    break;
                case int n when (n > 75):
                    Console.WriteLine($"Hello, {name},\nlet's go to the nursery home.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            return; // Added return statement to explicitly indicate the end of Main method
                    // still needs two keystrokes on this laptop to exit. 
        }
    }
}
