// See https://aka.ms/new-console-template for more information

using System.Xml.Linq;

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
        MyTcpListener server = new MyTcpListener();
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
        String[] names = [
            "Castle",
            "Circles",
            "Circuit",
            "Knots",
            "Summer",
        ];

        // this will reset results
        // foreach(var name in names)
        // {
        //     Directory.Delete($"generated/rules/{name}", true);
        // }
        System.Environment.Exit(1);

        for (int i = 0; i < 100; i++)
        {
            TestingAlot(names, server, model_type);
        }

    }
    public static void TestingAlot(String[] names, MyTcpListener server, int model_type)
    {
        Specsheet spc = new Specsheet();
        spc.width = 0;
        spc.height = 0;

        server.GenerateTilemap(spc, model_type);

        List<TestObject> tests = GenerateTilesObjects(names);
        // string filepath, string filename, string extension, int tilesize, bool isTileset)
        var files = Directory.GetFiles("output/", "*.png");
        foreach(var test in tests)
        {
            foreach(var file in files)
            {
                if (file.Contains(test.name))
                {
                    test.files.Add(file);
                }
            }
        }
        foreach(var test in tests)
        {
            foreach(var file in test.files)
            {
                try{
                    BitmapHelper.OverlappingTestWithTestObject(model_type, file, test.filepath, test.extension, test.isTileset);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }


        // yeah okay, so now we need to go through each of the files within the generated fiels and test their similarity
        foreach(var test in tests)
        {

            XElement tempneighbors = new XElement("neighbors");
            // so now we get the names from test and then we get the files within their generated directory 
            // then we just iterate through all those names, and test.
            var rulefiles = Directory.GetFiles($"generated/rules/{test.name}/", "*.xml");

            XElement crafted_rules = XElement.Load($"tilesets/{test.name}.xml");
            foreach( var rulefile in rulefiles)
            {

                Console.WriteLine($"getting rule similiarity of {test.name}");
                XElement generated_rules = XElement.Load($"{rulefile}");
                BitmapHelper.GetRuleSimiliarity(crafted_rules, generated_rules);
                Console.WriteLine();

                // the issue with this approach is that you have duplicates within the set... you need to get rid of them somehow.
                tempneighbors.Add(generated_rules.Element("neighbors").Elements("neighbor"));
            }

            Console.WriteLine($"temp: {tempneighbors.Elements("neighbor").Count()}");

            var distinctHash = new Dictionary<String, XElement>();
            foreach(var distinct in tempneighbors.Elements("neighbor").Distinct())
            {
                if(!distinctHash.TryGetValue(distinct.ToString(), out XElement index))
                {
                    distinctHash.Add(distinct.ToString(), distinct);
                }
            }
            XElement compiled_unique_neighbors = new XElement("neighbors");
            foreach(var item in distinctHash)
            {
                compiled_unique_neighbors.Add(item.Value);
            }
            Console.WriteLine($"unique rules: {compiled_unique_neighbors.Elements("neighbor").Count()}");

            // we need to make this into an xml doc, I think
            XElement root = new XElement("set");

            root.Add(compiled_unique_neighbors);
            BitmapHelper.GetRuleSimiliarity(crafted_rules, root);


        }
        // then after we go through all of files, then we can generate the compliation rules and then do one more generated test.

    }   

    public static List<TestObject> GenerateTilesObjects(String[] names)
    {
        List<TestObject> tests = new List<TestObject>();

        foreach(var name in names)
        {
            TestObject testObject = new TestObject();
            testObject.filepath = "output";
            testObject.extension = ".png";
            testObject.isTileset = true;
            testObject.files = new List<String>();
            testObject.name = name;
            tests.Add(testObject);
        }
        return tests;
    }
    public static void TestGemini()
    {
        var name = "Summer_gemini";
        var width = 40;
        var rules = XElement.Load($"tilesets/Summer_gemini.xml");
        XElement xelem = rules;
        string subset = xelem.Get<string>("subset");
        bool blackBackground = xelem.Get("blackBackground", false);
        var periodic = false;
        var heuristic = Model.Heuristic.Entropy;

        var tile_model = new SimpleTiledModel("tilesets/", "Summer_gemini", subset, 5, 5, periodic, blackBackground, heuristic);

        Random random = new();
        int seed = random.Next();

        var success = false;

        while(!success)
        {
            success = tile_model.Run(seed, -1);
            Console.WriteLine($"success: {success}");
        }
        Console.WriteLine("Generated new simpletiledtile_model output, now saving as image");

        tile_model.Save($"generated/simpletiled/{name} {width}x{width}.png");
        if (tile_model is SimpleTiledModel stmodel && xelem.Get("textOutput", false))
        {
          System.IO.File.WriteAllText($"output/{name} {seed}.txt", stmodel.TextOutput());
        }

        Console.WriteLine($"generated/simpletiled/{name} {width}x{width}.png has been saved"); 
    }

    public static void Tester(int model_type)
    {
        // Console.WriteLine("The current model_type running is: {0}", model_name);
        //TODO: at some point finish this code so that way you can choose which model you want to run from the commandline... but I didn't realize how much of an under taking it actually was going to be.

        MyTcpListener server = new MyTcpListener();
        // at some point I want to pass in some json that I can then run to see what is going on
        // such as 
        // {
        //  file : name,
        //  craftedrules: false
        //  .
        //  .
        //  .
        //  and so on, because I want some quick automated ways to test everything
        // server.Test(model_type);
        // we are going to want two types of test, the first will be focused on generating simpletiledmodel output, but also realize that whatever
        // you write here is going to be used later on as well.
        // Seeing as you will have this kind of stack
        // (generate from rules to create new synthetic data)
        // (generate from image to get synthetic rules that can be used for secondary generation)
        // (evaluate the synthetic rules against the crafted rules, assuming that we have some ground truth to compare to)
        // (generate from syntehtic rules 
        //soooo this test will also need to know if there are going to be tiles that the algorithm must know about or if the generated artifacts are fine enough. 

        var groundRulesExist = false;
        var width = 20;
        var filepath = "learning_set";
        var filename = $"Summer";
        var extension = ".png";
        var tilesize = 48;
        if (!groundRulesExist)
        {
            width = 20;
            filepath = "learning_set";
            filename = $"Summer";
            extension = ".png";
            tilesize = 48;
            BitmapHelper.OverlappingTest(model_type, filepath, filename, extension, tilesize, true);
            // BitmapHelper.DragonQuestTest(model_type, filepath, filename, extension, tilesize, width);
            // server.Start();
        } 
        // else
        // {
        filename = "alefgard";
        extension = ".gif";
        tilesize = 16;
        width = 10;
        Console.WriteLine("==============begining the overlapping test=====================");
        BitmapHelper.OverlappingTest(model_type, filepath, filename, extension, tilesize, false);
        // now that we have generated rules, we want to test those rules out.
        BitmapHelper.SimpleTileTest(model_type, filepath, filename, extension, tilesize, width);
        // }

        // NOTE: when we are generating the rules, let's get some stats about the new rule origin point, that might be something interesting to spin in the paper, but first let's write more about the paper that way we can see what we need.
        // The point of this is to find the context of why the rule was generated and what rule did so.
        
        // we are going to take a break and just focus on doing some preliminary writing.
        // at some point we are going to need to make sure we can run these on all the simpletiled rulesets and do a comparsion and see what we get out.
        // WARNING: additional game maps have been added into the testing suite, those are the ones that we should do a no_ground_truth function call on to generate the rules and then use those rules
        // if there is time, see if you can extract the top 10 patterns of a map and output them to show the users what they are most likely going to generate
        // we should ABSOLUTELY be getting back the pattern amount per each of these runs 
        // automate testing as we should be able to tune the rule similiarity numbers to get back best guesses of how large the image should be given the amount of unique patterns exist within the image.
        // Generate legend of zelda levels as well.
        // We should try generating an output that is made from a red and black checkerboard and see what happens.
        var files = Directory.GetFiles("generated/rules/", "*.xml");
        foreach (var item in files)
        {
            if(item.Contains("Summer"))
            {

                Console.WriteLine($"========filename: {item}=========");
                XElement generated_rules = XDocument.Load(item).Root;
                XElement crafted_rules = XDocument.Load("tilesets/Summer.xml").Root;
                BitmapHelper.GetRuleSimiliarity(crafted_rules, generated_rules);
                BitmapHelper.ShowAdditionalRules(crafted_rules, generated_rules);
            }
        }
    }
}

