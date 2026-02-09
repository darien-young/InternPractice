using System;
using System.Text;


namespace InternConsoleApp
{
    public static class AgeCategoryHelper
    {

        // ENUM FOR AGE CATEGORIES
        public enum AgeCategory
        {
            Infant, //0-2
            Child, //3-12  
            Teenager,//13-17
            YoungAdult,//18-24
            Adult,//25-64
            Senior//65+
        }


        // AGE CATEGORY FUNCTION - to determine age category based on age
        public static AgeCategory GetCategory(int age)
        {
            if (age >= 0 && age <= 2) return AgeCategory.Infant;
            if (age >= 3 && age <= 12) return AgeCategory.Child;
            if (age >= 13 && age <= 17) return AgeCategory.Teenager;
            if (age >= 18 && age <= 24) return AgeCategory.YoungAdult;
            if (age >= 25 && age <= 64) return AgeCategory.Adult;
            if (age >= 65) return AgeCategory.Senior;
            return AgeCategory.Adult; // Default case, should not reach here
        }


        //ENUM NAME CONVERSION FUNCTION - converts names like "YoungAdult" to "Young Adult" 
        public static string PrettyCategoryName(AgeCategory cat)
        {
            string s = cat.ToString();
            var sb = new StringBuilder();
            foreach (char ch in s)
            {
                if (char.IsUpper(ch) && sb.Length > 0) sb.Append(' ');
                sb.Append(ch);
            }
            return sb.ToString();
        }


        //CALCULATE AGE FUNCTION -- Function to calculate age from birth year
        public static int CalculateAge(int birthYear)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            int age = currentDate.Year - birthYear;
            return age;
        }
    }
}