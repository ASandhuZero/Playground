// See https://aka.ms/new-console-template for more information

static class Program 
{
    static void Main()
    {
        MyTcpListener server = new MyTcpListener();
        server.Start();
        Console.WriteLine("hello world! line 8.");
    }
}
