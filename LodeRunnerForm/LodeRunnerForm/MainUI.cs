using System.IO;
using System.Collections.Generic;

namespace LodeRunnerForm
{
    public class MainUI
    {
        public static List<string> DirectoryContain()
        {
            List<string> levels = new List<string>();
            DirectoryInfo di = new DirectoryInfo("Data\\Levels");
            int i = 1;
            foreach (var fi in di.GetFiles("*.json"))
            {
                levels.Add(fi.Name.Substring(0, fi.Name.Length - 5));
                i++;
            }
            return levels;
        }
    }
}