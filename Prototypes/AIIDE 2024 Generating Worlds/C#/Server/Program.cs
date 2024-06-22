// See https://aka.ms/new-console-template for more information

static class Program 
{
    enum Models
    {
        overlapping = 0,
        simpletiled = 1,
        tilemapping = 2,
    }
    static void Main(string[] args)
    {
        var model_type = 0;
        var model_name = "overlapping";
        foreach (var arg in args)
        {
            if (arg.ToLower() == "simpletiled")
            {
                model_type = (int)Models.simpletiled;
                model_name = "simpletiled";
            }
            else if (arg.ToLower() == "tilemapping")
            {
                model_type = (int)Models.tilemapping;
                model_name = "tilemapping";
            }
        }
        Console.WriteLine("The current model_type running is: {0}", model_name);
        //TODO: at some point finish this code so that way you can choose which model you want to run from the commandline... but I didn't realize how much of an under taking it actually was going to be.

        MyTcpListener server = new MyTcpListener();
        server.Test(model_type);
        // server.Start();
    }
}

