using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StageBound
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Stage> ll=StageBind.LoadLevels();
            foreach(Stage s in ll)
            {
                Console.WriteLine(s);
            }
            for (int i = 0; i < ll.Count; i++)
            {
                Console.WriteLine(ll[i]);
            }
        }
    }
}
