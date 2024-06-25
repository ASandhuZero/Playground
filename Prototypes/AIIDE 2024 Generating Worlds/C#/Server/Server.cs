using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Xml.Linq;
using System.Text.Json;

// TODO:Make sure to refactor this codebase so it's easier to navigate.
// Honestly make sure to refactor this code base

// WARNING: we really should have some cli input that chooses the model that 
// we want to use
public class Specsheet
{
  public int width { get; set; }
  public int height { get; set; }
  public String? spec { get; set; } 

  public override String ToString()
  {
    return $"width : {width}, height : {height}, spec : {spec}";
  }
}

public class TileArray
{
  public String? tiles { get; set; }
} 

public class TileConvertor
{

  public static int GenerateBitmapFromTilemap(int tilesize)
  {
    // TODO: the code below needs some heavy refactoring, but we will do that after the paper gets submitted.
    for(int i = 10; i <= 40; i=i+10)
    {
      var name = $"Summer {i}x{i}";
      Console.WriteLine($"< {name}");

      //WARNING: this is hardcoded and could cause some issues later on
      var (tilemap, SX, SY) = BitmapHelper.LoadBitmap($"output/{name}.png");
      var MX = SX;
      var MY = SY;
      // Continue. I don't know what is going on but something is happening
      // now that I am trying to use this nonsense solution... I think that I need to 
      BitmapHelper.CompressAndDepcompressTilemap(name, tilemap, MX, MY, tilesize, $"output/Tiles.png");
    }

    // Now we need

    return 1;
  }
}


class MyTcpListener
{
  public void Test(int model_type)
  {
    
    Specsheet testsheet = new Specsheet();
    testsheet.width = 10;
    testsheet.height = 10;
    testsheet.spec = "test";
    // okay so test needs to change and the way to change it is like this:
    // This function should only generate one tile map... 
    // then we should have some for loop that is calling the generatetilemap code, inputting the names of the tilemap to generate, or they are generated through the model
    // TODO: Remember to add back in the code below.
    GenerateTilemap(testsheet, model_type);

    var converted_tiles = TileConvertor.GenerateBitmapFromTilemap(48);
    // I think this needs to be rewritten.
    // as in that we shoudl firs
    Console.WriteLine("Test has finished");
    BitmapHelper.GetRuleSimiliarity();
  } 

  public void Start()
  {
    TcpListener server = null;
    try
    {
      // Set the TcpListener on port 13000.
      Int32 port = 13000;
      String host = "127.0.0.1";
      IPAddress localAddr = IPAddress.Parse(host);

      // TcpListener server = new TcpListener(port);
      server = new TcpListener(localAddr, port);

      // Start listening for client requests.
      server.Start();

      // Buffer for reading data
      Byte[] bytes = new Byte[256];
      String data = null;

      Boolean listening = true;
      // Enter the listening loop.
      while(true)
      {
        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        using TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected!");

        data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        int i;

        // Loop to receive all the data sent by the client.
        while(listening)
        {
          // Translate data bytes to a ASCII string.
          i = stream.Read(bytes, 0, bytes.Length);
          data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

          Console.WriteLine("Received: {0}", data);
          // TODO: Transform data into XML or json and give to WFC.
          Specsheet specData = JsonSerializer.Deserialize<Specsheet>(data);

          //WARNING: You're going to forget this at some point, but 
          // this generatetilemap needs to have the commandline argument for 
          // the model passed in, right now we are just giving it a value
          TileArray tarray = GenerateTilemap(specData, 0); // This is the tilemap that is returned to Love2D
          tarray.tiles = tarray.tiles == null ? "" : tarray.tiles;
          byte[] msg = System.Text.Encoding.ASCII.GetBytes(tarray.tiles);

          // Send back response
          stream.Write(msg, 0, msg.Length);
          Console.WriteLine("Sent: {0} at {1}", msg, DateTime.Now);
          // TODO: Huh, maybe shouldn't have the listening boolean value 
          // just thrashing on and off.
          listening = false;
        }
        client.Close();
        listening = true;
      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
      server.Stop();
    }

    Console.WriteLine("\nHit enter to continue...");
    Console.Read();
  }

  // TODO: Change method so data from specsheet can pass in through server 
  // function callback.
  static TileArray GenerateTilemap(Specsheet data, int model_type)
  {
    Console.WriteLine(model_type);
    // TODO: There's got to be a better name for this I think.
    TileArray tarray = new TileArray();
    Stopwatch sw = Stopwatch.StartNew();
    var folder = System.IO.Directory.CreateDirectory("output");
    foreach (var file in folder.GetFiles()) { file.Delete(); }

    Random random = new();
    XDocument xdoc = XDocument.Load("samples.xml");

    foreach (XElement xelem in xdoc.Root.Elements("overlapping", "simpletiled"))
    {
      Model model;
      string name = xelem.Get<string>("name");
      Console.WriteLine($"< {name}");

      bool isOverlapping = xelem.Name == "overlapping";
      int size = xelem.Get("tilesize", isOverlapping ? 48 : 24);
      int width = xelem.Get("width", data.width != 0 ? data.width : size);
      int height = xelem.Get("height", data.height != 0 ? data.height : size);
      bool periodic = xelem.Get("periodic", false);
      string heuristicString = xelem.Get<string>("heuristic");
      var heuristic = heuristicString == "Scanline" ? Model.Heuristic.Scanline : (heuristicString == "MRV" ? Model.Heuristic.MRV : Model.Heuristic.Entropy);

      if (isOverlapping)
      {
        int N = xelem.Get("N", 3);
        bool periodicInput = xelem.Get("periodicInput", true);
        int symmetry = xelem.Get("symmetry", 8);
        bool ground = xelem.Get("ground", false);

        model = new OverlappingModel(name, N, width, height, periodicInput, periodic, symmetry, ground, heuristic);
      }
      else
      {
        string subset = xelem.Get<string>("subset");
        bool blackBackground = xelem.Get("blackBackground", false);

        model = new SimpleTiledModel(name, subset, width, height, periodic, blackBackground, heuristic);
      }

      for (int i = 0; i < xelem.Get("screenshots", 2); i++)
      {
        for (int k = 0; k < 10; k++)
        {
          Console.Write("> ");
          int seed = random.Next();
          bool success = model.Run(seed, xelem.Get("limit", -1));
          if (success)
          {
            Console.WriteLine("DONE");
            model.Save($"output/{name} {width}x{height}.png");
            if (model is SimpleTiledModel stmodel && xelem.Get("textOutput", false))
            {
              System.IO.File.WriteAllText($"output/{name} {seed}.txt", stmodel.TextOutput());
              tarray.tiles = stmodel.TileArrayOutput();
            }
            break;
          }
          else Console.WriteLine("CONTRADICTION");
        }
      }
    }
    Console.WriteLine($"time = {sw.ElapsedMilliseconds}");
    return tarray;
  }
}


