// See https://aka.ms/new-console-template for more information

using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string[] lines = { "First line", "Second line", "Third line" };

        string curPath = Directory.GetCurrentDirectory();
        Console.WriteLine(curPath);
        // Set a variable to the Document path.
        Console.WriteLine(Directory.GetCurrentDirectory());
        // Console.WriteLine(Environment.SpecialFolder.MyDocuments);
        string docPath = 
            curPath;
            // Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);  

        // Write the string array to a new file named "WriteLines.txt"
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
        {
            foreach(string line in lines) outputFile.WriteLine(line);
        }
    }
}
