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
                Console.WriteLine("Invalid Input. Please use Numbers Only (No letters or symbols"); 
            }                                   //Did you mean )" instead of ")? It cuts off the console command output prematurely.

            //Boolean for Age-dependent Messgae

            // Let's have more age ranges added:
            // 1 - 3: "Hello, {name},\nlet's go to the nursery."
            // 4 - 12: "Hello, {name},\nlet's go to primary school."
            // 13 - 17: "Hello, {name},\nlet's go to high school."
            // >75: "Hello, {name},\nlet's go to the nursery home."

            Console.WriteLine("\n");
            if (age > 18)
            {
                Console.WriteLine($"Hello, {name},\nlet's go for a drink.");
            }
           
            else if (age == 18)
            {
                Console.WriteLine($"Hello, {name},\nlet's have fun this year.");

            }

            else if (age < 18)
            {
                Console.WriteLine($"Hello, {name},\nlet's go to class.");
            }


            //Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            return; // Added return statement to explicitly indicate the end of Main method
        }
    }
}
