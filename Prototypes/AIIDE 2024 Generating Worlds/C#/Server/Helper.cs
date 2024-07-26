// Copyright (C) 2016 Maxim Gumin, The MIT License (MIT)

using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

static class Helper
{
    public static int Random(this double[] weights, double r)
    {
        double sum = 0;
        for (int i = 0; i < weights.Length; i++) sum += weights[i];
        double threshold = r * sum;

        double partialSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            partialSum += weights[i];
            if (partialSum >= threshold) return i;
        }
        return 0;
    }

    public static long ToPower(this int a, int n)
    {
        long product = 1;
        for (int i = 0; i < n; i++) product *= a;
        return product;
    }

    public static T Get<T>(this XElement xelem, string attribute, T defaultT = default)
    {
        XAttribute a = xelem.Attribute(attribute);
        return a == null ? defaultT : (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(a.Value);
    }

    public static IEnumerable<XElement> Elements(this XElement xelement, params string[] names) => xelement.Elements().Where(e => names.Any(n => n == e.Name));
}

public class Tile
{
   public Bgra32 pixel_representation { get; set; }
   public Image<Bgra32>? tile_image { get; set; }
   public String? name { get; set; }
   public float weight { get; set; }

   public override String ToString() 
   {
       return $"tile_image filename: {name.ToString()},\n tile_image image: {tile_image.ToString()},\n pixel representation: {pixel_representation.ToString()},\n weight : {weight}";

   }
}
static class BitmapHelper
{
    public static (int[], int, int) LoadBitmap(string filename)
    {
        using var image = Image.Load<Bgra32>(filename);
        int width = image.Width, height = image.Height;
        int[] result = new int[width * height];
        image.CopyPixelDataTo(MemoryMarshal.Cast<int, Bgra32>(result));
        return (result, width, height);
    }

    unsafe public static void SaveBitmap(int[] data, int width, int height, string filename)
    {
        fixed (int* pData = data)
        {
            using var image = Image.WrapMemory<Bgra32>(pData, width, height);
            image.SaveAsPng(filename);
        }
    }


    private static Image<Bgra32> Extract(Image<Bgra32> sourceImage, Rectangle sourceArea)
    {
        Image<Bgra32> targetImage = new(sourceArea.Width, sourceArea.Height);
        int height = sourceArea.Height;
        sourceImage.ProcessPixelRows(targetImage, (sourceAccessor, targetAccessor) =>
        {
            for (int i = 0; i < height; i++)
            {
                Span<Bgra32> sourceRow = sourceAccessor.GetRowSpan(sourceArea.Y + i);
                Span<Bgra32> targetRow = targetAccessor.GetRowSpan(i);

                sourceRow.Slice(sourceArea.X, sourceArea.Width).CopyTo(targetRow);
            }
        });

        return targetImage;
    }

    private static List<Image<Bgra32>> CheckTilesForRotations(List<Image<Bgra32>> tiles, int tilesize)
    {
        // TODO: So for this function we are going to have to create some kind of rotation code, that way each tile_image can be rotated all four ways, and once they are rotated, we will check the rotations against the other unique tile_images. 
        // if one of the tile_images is just a rotation, we will count that and give whatever tile_image a note about having a rotation... Maybe at this point we should
        // so we have two options... we can create the tileset with all of the rotations and just call it a day, or we can try to figure out how to do rotations...
        List<Image<Bgra32>> rotation_checked_tiles = tiles;
        Console.WriteLine("Currently we do not check for rotations!");

        return tiles;
    }

    // okay this needs to be rewritten so we can also get the weight of each tile_image as well to then 
    // put in the xml file
    // we are going to need some way to figure out if the tile is unique.
    // if it is unique then add it to collection
    // but if it is not unique, then we need to increment a total count of that tile.
    // but need to some way to keep around the count of the unique tile.
    // okay so we are going to do it twice, that way I don't have to think about it.
    private static List<Image<Bgra32>> GetUniqueTiles(List<Image<Bgra32>> tile_images, int tilesize)
    {

        List<Image<Bgra32>> unique_tiles = new List<Image<Bgra32>>();

        foreach(var tile_image in tile_images)
        {
            if (IsTileUnique(tile_image, unique_tiles))
            {
                unique_tiles.Add(tile_image);
            }
            else 
            {

            }
        }
        return unique_tiles;
    }

    private static bool ComparePixels(Image<Bgra32> tile, Image<Bgra32> known_tile)
    {

        int height = tile.Height;
        Bgra32[] pixelArray = new Bgra32[height * height];
        tile.CopyPixelDataTo(pixelArray);

        Bgra32[] comparePixelArray = new Bgra32[height * height];
        known_tile.CopyPixelDataTo(comparePixelArray);

        for (int i = 0; i < pixelArray.Length; i++)
        {
            if (!pixelArray[i].Equals(comparePixelArray[i]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsTileUnique(Image<Bgra32> tile, List<Image<Bgra32>> unique_tiles)
    {
        if (0 == unique_tiles.Count)
        {
            return true;
        }
        foreach (var entry in unique_tiles)
        {
            if (ComparePixels(tile, entry))
            {
                return false;
            }
        }
        return true;
    }

    private static List<Image<Bgra32>> ExtractTiles(Image<Bgra32> sourceImage, int tilesize)
    {
        int _xTileAmount = sourceImage.Width/tilesize;
        int _yTileAmount = sourceImage.Height/tilesize;

        List<Image<Bgra32>> tiles = new List<Image<Bgra32>>();
        List<Image<Bgra32>> unique_tile = new List<Image<Bgra32>>();
        Image<Bgra32> tile = new(sourceImage.Width, sourceImage.Height);

        for (int y = 0; y < _yTileAmount; y++)
        {
            for (int x = 0; x < _xTileAmount; x++)
            {
                Rectangle sourceArea = new Rectangle(new Point(x*tilesize,y*tilesize), new Size(tilesize));

                tile = Extract(sourceImage, sourceArea);
                tiles.Add(tile);
            }
        }
        return tiles;
    }

    private static List<Tile> MapTilesToPixels(List<Tile> tiles)
    {

        int r = 0, g = 0, b = 0;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (r < 255)
            {
                r=r+1; 
            }
            else if (g < 255)
            {
                g=g+1; 
                r=0; 
            }
            else if (b < 255)
            {
                b=b+1; 
                r=0; 
                g=0; 
            }
            Bgra32 pixel = new Bgra32((byte)r, (byte)g, (byte)b);
            tiles[i].pixel_representation = pixel;
        }
        return tiles;

    }
    
    private static List<Tile> ConvertToTiles(List<Image<Bgra32>> tile_images)
    {

        List<Tile> tiles = new List<Tile>();

        for (int i = 0; i < tile_images.Count; i++)
        {
            var image = tile_images[i];
            var tile = new Tile();
            tile.tile_image = image;
            tile.name = $"{i}";
            tile.weight = 0;

            tiles.Add(tile);
        }

        return tiles;
    }

    private static Image<Bgra32> DownscaleImageToBitmap(Image<Bgra32> image, int tilesize, List<Tile> tiles)
    {
        int _xTileAmount = image.Width/tilesize;
        int _yTileAmount = image.Height/tilesize;


        Image<Bgra32> tile_image = new(image.Width, image.Height);
        Image<Bgra32> pixelBitmap = new(_xTileAmount, _yTileAmount);

        for (int y = 0; y < _yTileAmount; y++)
        {
            for (int x = 0; x < _xTileAmount; x++)
            {
                Rectangle sourceArea = new Rectangle(new Point(x*tilesize,y*tilesize), new Size(tilesize));

                tile_image = Extract(image, sourceArea);

                Tile tile_match = tiles[0];
                foreach(var tile in tiles)
                {
                    if (ComparePixels(tile.tile_image, tile_image))
                    {
                        tile_match = tile;
                        break;
                    }

                }

                pixelBitmap[x,y] = tile_match.pixel_representation; 

            }
        }
        return pixelBitmap;
    }

    private static Image<Bgra32> UpscaleBitmapToImage(Image<Bgra32> bitmap, int tilesize, List<Tile> tiles)
    {
        int _xTileAmount = bitmap.Width * tilesize;
        int _yTileAmount = bitmap.Height * tilesize;
        
        List<Image<Bgra32>> tile_images = new List<Image<Bgra32>>();

        Image<Bgra32> tilemap = new Image<Bgra32>(bitmap.Width * tilesize, bitmap.Height * tilesize); 
        
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {

                Bgra32 pixel = bitmap[x,y];
                foreach(var tile in tiles)
                {
                    if (pixel.Equals(tile.pixel_representation))
                    {
                        tilemap.Mutate(o => o
                            .DrawImage(tile.tile_image, new Point(tilesize * x, tilesize * y), 1f)
                        );
                        break;
                    }
                }
            }
        }
        return tilemap;
    }

    public static Dictionary<String, Image<Bgra32>> GenerateRotations(String name, String tilename, Image<Bgra32> tile, int card)
    {
        Directory.CreateDirectory("test");
        Directory.CreateDirectory($"test/{name}");


        var generated_tiles = new Dictionary<String, Image<Bgra32>>();
        generated_tiles.Add($"{tilename}", tile);
        for (int i = 0; i < card; i++)
        {
            // tile.Mutate(img => img.Rotate(RotateMode.Rotate90));
            // tile.SaveAsPng($"test/{filename}/{tilename} {i}.png");
            generated_tiles.Add($"{tilename} {i}", tile);
        }
        return generated_tiles;

    }
    // we need to get cardinality as well
    public static Dictionary<String, Image<Bgra32>> LoadSampleTileset(String filename, bool rotate)
    {

        var xroot = XElement.Load($"{filename}.xml");
        var xtiles = xroot.Element("tiles");

        var xtilescard = new Dictionary<String, int>();
        foreach(var xtile in xtiles.Elements("tile"))
        {
            var cardinality = 1;
            char sym = xtile.Get("symmetry", 'X');
            if (sym == 'L')
            {
                cardinality = 4;
            }
            else if (sym == 'T')
            {
                cardinality = 4;
            }
            else if (sym == 'I')
            {
                cardinality = 2;
            }
            else if (sym == '\\')
            {
                cardinality = 2;
            }
            else if (sym == 'F')
            {
                cardinality = 8;
            }
            else
            {
                cardinality = 1;
            }
            xtilescard[xtile.Get("name", "")] = cardinality; 
        }
        var files = Directory.GetFiles($"{filename}");
        Dictionary<String, Image<Bgra32>> tiles = new Dictionary<String, Image<Bgra32>>();
        foreach (var file in files)
        {
            var tile_name = file.Split('/')[2].Split('.')[0];
            var prefix = tile_name.Split(' ')[0];
            var card = xtilescard[prefix];

            Image<Bgra32> tile = Image.Load<Bgra32>(file);
            
            if (!xroot.Get("unique", false))
            {
                var rot_tiles = GenerateRotations(filename, prefix, tile, card);
                foreach(var rot_tile in rot_tiles)
                {
                    if(tiles.TryGetValue(rot_tile.Key, out Image<Bgra32> index))
                    {continue;}
                    tiles.Add(rot_tile.Key, rot_tile.Value);
                }
            }
            else
            {
                tiles.Add(tile_name, tile);
            }
        }
        return tiles;
    }


    public static List<Tile> GetTileWeights(List<Tile> tiles, List<Image<Bgra32>> tile_images)
    {
        foreach (var tile_image in tile_images)
        {

            foreach(var tile in tiles)
            {
                if ((ComparePixels(tile_image, tile.tile_image)))
                {
                    tile.weight++;
                }

            }
        }
        return tiles;
    }

    unsafe public static void CompressAndDepcompressTilemap(String name, int[] data, int width, int height, int tilesize, string filename)
    {
        var bitmap_name = $"bitmap_{name}";
        Console.WriteLine($"filename: {name}");
        fixed (int* pData = data)
        {
            Random random = new();
            using var image = Image.WrapMemory<Bgra32>(pData, width, height);
            var tile_images = ExtractTiles(image, tilesize);
            var unique_tiles = GetUniqueTiles(tile_images, tilesize);

            var checked_unique_tiles = CheckTilesForRotations(unique_tiles, tilesize);
            List<Tile> tiles = ConvertToTiles(checked_unique_tiles);

            tiles = GetTileWeights(tiles, tile_images);

            tiles = MapTilesToPixels(tiles);

            //WARNING: You hardcoded the filename of the file... Please fix this.
            var original_tiles = LoadSampleTileset("Summer", false);

            // so now we have to go through the original tile_images, compare them with the tile_images that we have right now and then create a new mapping.
            foreach(var item in original_tiles)
            {
                foreach(var generated_tile in tiles)
                {
                    if(ComparePixels(generated_tile.tile_image, item.Value))
                    {
                        generated_tile.name = item.Key;
                    }
                }
            }

            // Downscaling image to get bitmap
            // Image<Bgra32> bitmap = DownscaleImageToBitmap(image, tilesize, tiles);
            // bitmap.SaveAsPng($"samples/{bitmap_name}.png");
            //
            // // Use WFC to generate new bitmap
            // int seed = random.Next();
            //
            // bool is_map_generated = false;
            // while (!is_map_generated)
            // {
            //     is_map_generated = GenerateTilemapFromBitmap(bitmap_name, width/tilesize, height/tilesize, seed, tiles, filename);
            //     // NOTE: Please refactor this.
            //     if(!is_map_generated)
            //     {
            //         seed = random.Next();
            //     }
            //
            // }

            Console.WriteLine("WARNING WARNING WARNING there is no generation happening here, you have commented out the code.");
            using var bitmapFromModel = Image.Load<Bgra32>($"output/bitmap_{name}.png");

            Image<Bgra32> upscaled_image = UpscaleBitmapToImage(bitmapFromModel, tilesize, tiles);
            upscaled_image.SaveAsPng($"generated/upscaled_{name}.png");

        }
    }

    public static void ShowAdditionalRules(XElement crafted_rules, XElement generated_rules)
    {

        int matching = 0;

        XElement inferred_rules = new XElement("rules");
        Console.WriteLine(crafted_rules.Descendants("neighbors").Descendants("neighbor").Count());
        foreach (XElement xtile in crafted_rules.Element("neighbors").Elements("neighbor"))
        {

            var right = (string)xtile.Attribute("right");
            var left = (string)xtile.Attribute("left");
            XElement inferred_rule = new XElement("neighbor");
            inferred_rule.SetAttributeValue("left", right);
            inferred_rule.SetAttributeValue("right", left);

            inferred_rules.Add(inferred_rule);
        }

        foreach (XElement item in inferred_rules.Elements("neighbor"))
        {

            crafted_rules.Element("neighbors").Add(item);
            
        }
        Console.WriteLine(crafted_rules.Descendants("neighbors").Descendants("neighbor").Count());

        foreach (XElement xtile in crafted_rules.Element("neighbors").Elements("neighbor"))
        {
            foreach (XElement item in generated_rules.Element("neighbors").Elements("neighbor"))
            {

                if (XNode.DeepEquals(xtile, item))
                {
                    matching++;
                    break;
                }
            }
        }
        // comparing rulesets
        var generated_rule_count =generated_rules.Descendants("neighbors").Descendants("neighbor").Count(); 
        var crafted_rule_count =crafted_rules.Descendants("neighbors").Descendants("neighbor").Count(); 

        var similiarity = (float)matching/(float)crafted_rule_count;
        Console.WriteLine($"similiarity between rulesets : {similiarity}");
        var total = (float)matching/(float)generated_rule_count;
        Console.WriteLine($"total generated matching: {total}");

    }
    // compare rulesets against each other. Do a naive approach which is seeing how similar the rulesets are. For every rule that they share in common that is a plus one, and the total of the rules will then be used to compare. so it will be shared rules / total rules
    public static void GetRuleSimiliarity(String name, XElement crafted_rules, XElement generated_rules, bool output)
    {
        // XElement crafted_rules = XDocument.Load("tilesets/Summer.xml").Root;
        // XElement generated_rules = XDocument.Load("Summer 40x40_neighbors.xml").Root;

        int matching = 0;

        foreach (XElement xtile in crafted_rules.Element("neighbors").Elements("neighbor"))
        {
            foreach (XElement item in generated_rules.Element("neighbors").Elements("neighbor"))
            {

                if (XNode.DeepEquals(xtile, item))
                {
                    matching++;
                    break;
                }
            }
        }
        // comparing rulesets
        var generated_rule_count =generated_rules.Descendants("neighbors").Descendants("neighbor").Count(); 
        var crafted_rule_count =crafted_rules.Descendants("neighbors").Descendants("neighbor").Count(); 

        var similiarity = (float)matching/(float)crafted_rule_count;
        var total = (float)matching/(float)generated_rule_count;
        if(output)
        {
           
            Console.WriteLine($"Rule similiarity of compiled rules, {name}");
            Console.WriteLine($"total similiarity matching: {similiarity}");
            Console.WriteLine($"total generated matching: {total}");
            Console.WriteLine($"generated ruleset count: {generated_rule_count}");
            Console.WriteLine($"crafted ruleset count: {crafted_rule_count}");
        }

    }

    public static void DragonQuestTest(int model_type, string filepath, string filename, string extension, int tilesize, int width)
    {
        Console.WriteLine($"< {filename}");
        var name = filename;

        // this is assuming we are going to do an overlapping generation first
        var image = Image.Load<Bgra32>($"{filepath}/{name} {width}x{width}{extension}");

        int src_height = image.Height;
        int src_width = image.Width;

        Console.WriteLine("Extracting tile_images");
        var tile_images = ExtractTiles(image, tilesize);

        Console.WriteLine("Getting unique tile_images");
        List<Image<Bgra32>> unique_tiles = GetUniqueTiles(tile_images, tilesize);


        Console.WriteLine($"Saving tile_images to tiles/{name}");
        int count = 0;
        if (!Directory.Exists($"tiles/{name}"))
        {
            Directory.CreateDirectory($"tiles/{name}");
        }
        foreach (var item in unique_tiles)
        {
            item.SaveAsPng($"tiles/{name}/{count}.png");
            count++;
        }
        
        Console.WriteLine("Creating tile_images in list format for later computation.");
        List<Tile> tiles = ConvertToTiles(unique_tiles);

        Console.WriteLine("Getting tile weights");
        tiles = GetTileWeights(tiles, tile_images);
        float max = 0;

        foreach (var tile in tiles)
        {
            max = tile.weight > max ? tile.weight : max;
        }

        Console.WriteLine("adjust weight and create a new tile xelement");


        // check to see if there is an
        var original_tiles = LoadSampleTileset($"{name}", true);

        // so now we have to go through the original tile_images, compare them with the tile_images that we have right now and then create a new mapping.
        foreach(var item in original_tiles)
        {
            foreach(var generated_tile in tiles)
            {
                if(ComparePixels(generated_tile.tile_image, item.Value))
                {
                    generated_tile.name = item.Key;
                }
            }
        }

        Console.WriteLine($"=============tiles have been outputted===================================");


        XElement xtiles = new XElement("tiles");
        foreach (var tile in tiles)
        {
            tile.weight = tile.weight / max;
            max = tile.weight > max ? tile.weight : max;
            XElement xtile = new XElement("tile");
            xtile.SetAttributeValue("filename", tile.name);
            xtile.SetAttributeValue("weight", tile.weight);
            xtiles.Add(xtile);
        }
        
        Console.WriteLine("Saving tiles in an XML format");

        xtiles.Save($"tiles/{name}/tiles.xml");
        Console.WriteLine("Mapping tile_image to pixels.");
        tiles = MapTilesToPixels(tiles);

        Console.WriteLine("Downscaling image to bitmap");

        Image<Bgra32> bitmap = DownscaleImageToBitmap(image, tilesize, tiles);
        Console.WriteLine("Saving bitmap");
        var bitmap_name = $"bitmap_{name}";

        bitmap.SaveAsPng($"samples/bitmaps/{name} {width}x{width}.png");

        Random random = new();
        int seed = random.Next();

        Console.WriteLine("Generating rules from overworld image");

        // NOTE: Putting overlapping code inside of all this nonsense because generateTilemapFromBitmap is just a bad function at this point.
        int N = 2;
        bool periodicInput = false;
        int symmetry = 1;
        int height = width;
        bool ground = false;
        bool periodic = false;
        string heuristicString = "";
        var heuristic = Model.Heuristic.Entropy;

        var model = new OverlappingModel("samples/bitmaps", name, N, width, height, periodicInput, periodic, symmetry, ground, heuristic);

        bool success = false;
        success = model.Run(seed, -1);

        if (success)
        {
            Console.WriteLine("DONE");
            model.Save($"samples/bitmaps/{name}.png");

            Console.WriteLine("Overlapping image and rules have been saved");
        }
        Console.WriteLine("CONTRADICTION");

        // while (!is_map_generated)
        // {
        //     // is_map_generated = GenerateTilemapFromBitmap(bitmap_name, width/tilesize, height/tilesize, seed, tiles, filename);
        //     if(!is_map_generated)
        //     {
        //         seed = random.Next();
        //     }
        //
        // }
        // we need to save the output as well

        using var bitmapFromModel = Image.Load<Bgra32>($"samples/bitmaps/{name} {width}x{width}.png");

        Image<Bgra32> upscaled_image = UpscaleBitmapToImage(bitmapFromModel, tilesize, tiles);
        upscaled_image.SaveAsPng($"generated/upscaled_{name} {width}.png");
        Console.WriteLine($"{src_width/tilesize}, {src_height/tilesize}");


        // now you want to generate the tiles and neighbors xml right here and then give that to the xelem
        XElement rules = new XElement("set");
        rules.SetAttributeValue("unique", true);
        XElement neighbors = model.ExtractRules(tiles);
        // we should first check to see if there are tiles that already exist from a ruleset and 
        Console.WriteLine(name);
        if (File.Exists($"tilesets/{name}.xml"))
        {
            XElement existing_tiles = XElement.Load($"tilesets/{name}.xml").Element("tiles");
            rules.Add(existing_tiles);
            //we should do a some transformation here and make sure we get the actual tile names... or just make sure 
            //we do that at some point, honestly
        }
        else 
        {
            rules.Add(xtiles);
        }
        rules.Add(neighbors);

        rules.Save($"generated/rules/{name}.xml");
        // now generate with the simple tile_image map
        // uhhhh the below might be out of date and that's fine, just make sure to delete it at somepoint.
        Console.WriteLine("Now generating output with the tileset that overlapping had generated.");
        // XElement xelem = XElement.Load("alefgard_neighbors.xml");
        // Console.WriteLine(xelem);
        XElement xelem = rules;
        Console.WriteLine(xelem);
        string subset = xelem.Get<string>("subset");
        bool blackBackground = xelem.Get("blackBackground", false);
        periodic = false;
        heuristic = Model.Heuristic.Entropy;

        //TODO: I don't know if I want this here but lets keep it here for right now, and I think it should be fine... lmao
        int div = 3;
        bool shouldDiv = false;
        if (shouldDiv)
        {
            div = (2*8);
        }
        Console.WriteLine($"Generating with simpletiledmodel; div: {div}, adjusted height: {src_height/div}, adjusted width: {src_width/div}");
        //WARNING: We are now passing in the filepath into the simpletiledmodel as well, and I don't know if that is a good thing but I think it will work lmao
        var tile_model = new SimpleTiledModel("generated/rules", name, subset, width/div, width/div, periodic, blackBackground, heuristic);


        success = false;

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

    public static void SimpleTileTest(int model_type, string filepath, string filename, string extension, int tilesize, int width, bool hastiles)
    {
        var rules = XElement.Load($"generated/rules/{filename}.xml");
        var name = filename; 
        XElement xelem = rules;
        string subset = xelem.Get<string>("subset"); bool blackBackground = xelem.Get("blackBackground", false);
        var periodic = false;
        var heuristic = Model.Heuristic.Entropy;

        var tileloc = hastiles ? "tilesets" : "tiles";
        var tile_model = new SimpleTiledModel("generated/rules", name, subset, width, width, periodic, blackBackground, heuristic, tileloc);

        Random random = new();
        int seed = random.Next();

        var success = false;

        while(!success)
        {

            try
            {
                seed = random.Next();
                success = tile_model.Run(seed, -1);
                Console.WriteLine($"success: {success}");
                tile_model.Save($"generated/simpletiled/{name} {width}x{width}.png");
                if (tile_model is SimpleTiledModel stmodel && xelem.Get("textOutput", false))
                {
                  System.IO.File.WriteAllText($"output/{name} {seed}.txt", stmodel.TextOutput());
                }
            }
            catch
            {

                success = false;
            }
        }
        Console.WriteLine("Generated new simpletiledtile_model output, now saving as image");


        Console.WriteLine($"generated/simpletiled/{name} {width}x{width}.png has been saved"); 
    }

    // ... all we want to test is just the overlapping model right there and extract the rules basically
    // right now this function seems to be doing a little too much and idk if I like that.
    // we really should be saving tiles in there own function
    public static void OverlappingTestWithTestObject(int model_type, String nameseed, String filepath, String extension, bool isTileset, bool output)
    {
        String[] name_arr = nameseed.Split();
        var name = name_arr[0];
        var patharr = name.Split('/');
        var path = patharr[0];
        name = patharr[1];
        var seedname = name_arr[1];


        Image<Bgra32> image;
        // this should be the last time that we use the filenames and seed filename
        image = Image.Load<Bgra32>($"{filepath}/{name} {seedname}");

        int height = image.Height;
        int width = image.Width;
        int tilesize = 0;

        // Console.WriteLine("Extracting tile_images for bitmap downscaling");
        // var tile_images = ExtractTiles(image, test.tilesize);
        var tile_images = new List<Image<Bgra32>>();
        var og_tiles = LoadSampleTileset($"{name}", true);

        foreach(var tile in og_tiles)
        {
            tile_images.Add(tile.Value);
        }

        tilesize = tile_images[0].Width;

        // really really we should be pulling all of these things out into their own functions, this is getting a little annoying, but keep working through it.
        if(output)
        {
            Console.WriteLine("Getting unique tile_images");
        }
        List<Image<Bgra32>> unique_tiles = GetUniqueTiles(tile_images, tilesize);

        if (output)
        { 
            Console.WriteLine($"Saving tile_images to tiles/{name}");
        }
        int count = 0;
        if (!Directory.Exists($"tiles/{name}"))
        {
            Directory.CreateDirectory($"tiles/{name}");
        }
        foreach (var item in unique_tiles)
        {
            item.SaveAsPng($"tiles/{name}/{count}.png");
            count++;
        }
        if (output)
        {
            Console.WriteLine("===== Tiles have been saved! ===== ");
            Console.WriteLine("Creating tile_images in list format for later computation.");
        }
        
        List<Tile> tiles = ConvertToTiles(unique_tiles);

        if(output)
        {
            Console.WriteLine("Getting tile weights");
        }

        tiles = GetTileWeights(tiles, tile_images);
        float max = 0;

        foreach (var tile in tiles)
        {
            max = tile.weight > max ? tile.weight : max;
        }

        if(output)
        {
            Console.WriteLine("adjust weight and create a new tile xelement");
        }

        /// we should have the tile set location be passed into the function, the tile extraction should be its own function.
        if (isTileset)
        {
            var original_tiles = LoadSampleTileset($"{name}", true);
            foreach(var item in original_tiles)
            {
                foreach(var generated_tile in tiles)
                {
                    if(ComparePixels(generated_tile.tile_image, item.Value))
                    {
                        generated_tile.name = item.Key;
                    }
                }
            }
        }
        if(output)
        {
            Console.WriteLine($"=============tiles have been created===================================");
        }

        XElement xtiles = new XElement("tiles");
        foreach (var tile in tiles)
        {
            // tile.weight = tile.weight / max > 0.1f ? tile.weight / max : 0.1f;
            float weight = tile.weight / max;
            if (weight < 0.1f)
            {
                weight = 0.1f;

            }
            tile.weight = weight;
            XElement xtile = new XElement("tile");
            xtile.SetAttributeValue("filename", tile.name);
            xtile.SetAttributeValue("weight", tile.weight);
            xtiles.Add(xtile);
        }
        
        if(output)
        {
            Console.WriteLine("Saving tiles in an XML format");
        }

        xtiles.Save($"tiles/{name}/tiles.xml");
        if (isTileset)
        {
            var original_tiles = LoadSampleTileset($"{name}", true);
            foreach(var item in original_tiles)
            {
                foreach(var generated_tile in tiles)
                {
                    if(ComparePixels(generated_tile.tile_image, item.Value))
                    {
                        generated_tile.name = item.Key;
                    }
                }
            }
        }
        if(output)
        {
            Console.WriteLine("Mapping tile_image to pixels.");
        }
        tiles = MapTilesToPixels(tiles);

        if(output)
        {
            Console.WriteLine("================ Downscaling image to bitmap");
        }
        Image<Bgra32> bitmap = DownscaleImageToBitmap(image, tilesize, tiles);
        if(output)
        {
            Console.WriteLine("Saving bitmap");
        }
        var bitmap_name = $"bitmap_{name}";

        bitmap.SaveAsPng($"samples/bitmaps/{name}.png");

        Random random = new();
        int seed = random.Next();

        if(output)
        {
            Console.WriteLine($"Generating rules from {name} image by running the overlapping model now");
        }

        // NOTE: Putting overlapping code inside of all this nonsense because generateTilemapFromBitmap is just a bad function at this point.
        int N = 2;
        bool periodicInput = false;
        int symmetry = 1;
        bool ground = false;
        bool periodic = false;
        string heuristicString = "";
        var heuristic = Model.Heuristic.Entropy;

        var model = new OverlappingModel("samples/bitmaps", name, N, width/tilesize, height/tilesize, periodicInput, periodic, symmetry, ground, heuristic);

        bool success = false;
        success = model.Run(seed, -1);

        for (int i = 0; i < 1000; i++)
        {

            if (success)
            {
                Console.WriteLine("DONE");
                model.Save($"samples/bitmaps/{name}.png");

                // oh no this is bad code because it seems that I just save the neighbors here... I shouldn't just 
                // save the rules but return them so that way I ca
                if(output)
                {
                    Console.WriteLine($"Overlapping bitmap has been saved at samples/bitmaps/{name}.png and rules have been saved at generated/rules/{name}.xml");
                }
                break;
            } else 
            {
                // Console.WriteLine("CONTRADICTION");
            }
        }
        // we need to save the output as well


        using var bitmapFromModel = Image.Load<Bgra32>($"samples/bitmaps/{name}.png");

        Image<Bgra32> upscaled_image = UpscaleBitmapToImage(bitmapFromModel, tilesize, tiles);
        upscaled_image.SaveAsPng($"generated/upscaled/{name} {width}.png");


        // now you want to generate the tiles and neighbors xml right here and then give that to the xelem
        XElement rules = new XElement("set");
        rules.SetAttributeValue("unique", true);
        // we should first check to see if there are tiles that already exist from a ruleset and 
        XElement neighbors = model.ExtractRules(tiles);
        if (File.Exists($"tilesets/{name}.xml"))
        {
            XElement existing_tiles = XElement.Load($"tilesets/{name}.xml").Element("tiles");
            rules.Add(existing_tiles);
            //we should do a some transformation here and make sure we get the actual tile names... or just make sure 
            //we do that at some point, honestly
        }
        else 
        {
            rules.Add(xtiles);
        }
        rules.Add(neighbors);

        if(output)
        {
            Console.WriteLine("===============saving generated rules===================");
        }
        rules.Save($"generated/rules/{name}/{name} {seed}.xml");
        XElement xelem = rules;
    }
    public static bool OverlappingTest(int model_type, string filepath, string filename, string extension, int tilesize, bool isTileset)
    {
        Console.WriteLine($"< {filename}");

        var name = filename;

        Image<Bgra32> image;
        image = Image.Load<Bgra32>($"{filepath}/{name}{extension}");

        int height = image.Height;
        int width = image.Width;
        Console.WriteLine($"height: {height}, width: {width}");

        Console.WriteLine("Extracting tile_images");
        var tile_images = ExtractTiles(image, tilesize);

        Console.WriteLine("Getting unique tile_images");
        List<Image<Bgra32>> unique_tiles = GetUniqueTiles(tile_images, tilesize);


        Console.WriteLine($"Saving tile_images to tiles/{name}");
        int count = 0;
        if (!Directory.Exists($"tiles/{name}"))
        {
            Directory.CreateDirectory($"tiles/{name}");
        }
        foreach (var item in unique_tiles)
        {
            item.SaveAsPng($"tiles/{name}/{count} 0.png");
            count++;
        }
        Console.WriteLine("===== Tiles have been saved! ===== ");
        Console.WriteLine("");
        
        Console.WriteLine("Creating tile_images in list format for later computation.");
        List<Tile> tiles = ConvertToTiles(unique_tiles);

        Console.WriteLine("Getting tile weights");
        tiles = GetTileWeights(tiles, tile_images);
        float max = 0;

        foreach (var tile in tiles)
        {
            max = tile.weight > max ? tile.weight : max;
        }

        Console.WriteLine("adjust weight and create a new tile xelement");

        if (isTileset)
        {

            XElement existing_tiles = XElement.Load($"tilesets/{name}.xml").Element("tiles");
            var original_tiles = LoadSampleTileset($"tilesets/{name}", true);
            foreach(var item in original_tiles)
            {
                foreach(var generated_tile in tiles)
                {
                    if(ComparePixels(generated_tile.tile_image, item.Value))
                    {
                        generated_tile.name = item.Key;
                    }
                }
            }
        }
        Console.WriteLine($"=============tiles have been created===================================");

        var min = 0.6f;
        XElement xtiles = new XElement("tiles");
        foreach (var tile in tiles)
        {
            // tile.weight = tile.weight / max;
            float weight = tile.weight / max;
            if (weight < min)
            {
                weight = min;

            }
            tile.weight = weight;
            XElement xtile = new XElement("tile");
            xtile.SetAttributeValue("name", tile.name);
            xtile.SetAttributeValue("weight", tile.weight);
            xtiles.Add(xtile);
        }
        
        if(isTileset)
        {
            var xroot = XElement.Load($"tilesets/{name}.xml");
            xtiles = xroot.Element("tiles");
            Console.WriteLine("hello there");
        }
        Console.WriteLine("Saving tiles in an XML format");

        // xtiles.Save($"tiles/{name}/tiles.xml");
        Console.WriteLine("Mapping tile_image to pixels.");
        tiles = MapTilesToPixels(tiles);

        Console.WriteLine("Downscaling image to bitmap");

        Image<Bgra32> bitmap = DownscaleImageToBitmap(image, tilesize, tiles);
        Console.WriteLine("Saving bitmap");
        var bitmap_name = $"bitmap_{name}";

        bitmap.SaveAsPng($"samples/bitmaps/{name}.png");

        Random random = new();
        int seed = random.Next();

        Console.WriteLine("Generating rules from overworld image by running the overlapping model now");

        // NOTE: Putting overlapping code inside of all this nonsense because generateTilemapFromBitmap is just a bad function at this point.
        // lmao all of this code is bad code
        int N = 2;
        bool periodicInput = false;
        int symmetry = 1;
        bool ground = false;
        bool periodic = false;
        string heuristicString = "";
        var heuristic = Model.Heuristic.Entropy;

        var model = new OverlappingModel("samples/bitmaps", name, N, width/tilesize, height/tilesize, periodicInput, periodic, symmetry, ground, heuristic);

        bool success = false;
        while(!success)
        {
            try 
            {
                seed = random.Next();
                success = model.Run(seed, -1);
                if (success)
                {

                    Console.WriteLine("DONE");
                    success = model.SaveImg($"generated/bitmaps/{name}.png");

                } else 
                {
                    Console.WriteLine("CONTRADICTION");
                }
            }
            catch (Exception e)
            {
                success=false;
            }

        }

        Console.WriteLine("Overlapping image and rules have been saved");
        // we need to save the output as well

        using var bitmapFromModel = Image.Load<Bgra32>($"generated/bitmaps/{name}.png");

        Image<Bgra32> upscaled_image = UpscaleBitmapToImage(bitmapFromModel, tilesize, tiles);
        upscaled_image.SaveAsPng($"generated/upscaled/{name} {width}.png");
        Console.WriteLine($"{width/tilesize}, {height/tilesize}");


        // now you want to generate the tiles and neighbors xml right here and then give that to the xelem
        XElement rules = new XElement("set");
        rules.SetAttributeValue("unique", true);
        XElement neighbors = model.ExtractRules(tiles);
        // we should first check to see if there are tiles that already exist from a ruleset and 
        if (File.Exists($"tilesets/{name}.xml"))
        {
            XElement existing_tiles = XElement.Load($"tilesets/{name}.xml").Element("tiles");
            rules.Add(existing_tiles);
            //we should do a some transformation here and make sure we get the actual tile names... or just make sure 
            //we do that at some point, honestly
            
        }
        else 
        {
            rules.Add(xtiles);
        }
        rules.Add(neighbors);

        Console.WriteLine("===============saving generated rules===================");
        rules.Save($"generated/rules/{name}.xml");
        XElement xelem = new XElement("set");
        Console.WriteLine($"generated/rules/{name}.xml");
        // xelem.SetAttributeValue("unique", true);
        // xelem.Add(rules.Element("neighbors"));
        // xelem.Save($"generated/rules/{name}.xml");
        return success;
    }

}

