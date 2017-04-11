using System;
using ClassLibrary1;

namespace ConsoleApp149
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = new Test().GetValue();
            Console.WriteLine($"{text}");
        }
    }
}