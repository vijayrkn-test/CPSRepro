using System;
using System.Diagnostics;

namespace ClassLibrary1
{
    public class Test
    {
        public string GetValue()
        {
            Debug.Assert(false, "Should never get here");
            return "Release!!";
        }
    }
}
