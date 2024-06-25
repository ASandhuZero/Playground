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

   public override String ToString() 
   {
       return $"tile_image name: {name.ToString()}, tile_image image: {tile_image.ToString()}, pixel representation: {pixel_representation.ToString()}";

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
        List<Image<Bgra32>> rotation_checked_tiles = tiles;
        Console.WriteLine("Currently we do not check for rotations!");

        return tiles;
    }

    private static List<Image<Bgra32>> GetUniqueTiles(List<Image<Bgra32>> tiles, int tilesize)
    {

        List<Image<Bgra32>> unique_tiles = new List<Image<Bgra32>>();

        int number = 0;
        foreach(var tile in tiles)
        {
            if (IsTileUnique(tile, unique_tiles))
            {
                unique_tiles.Add(tile);
            }
            number++;
        }
        Console.WriteLine($"Number of unique tiles: {unique_tiles.Count}");
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

        int inc = 255/tiles.Count;

        for (int i = 0; i < tiles.Count; i++)
        {
            int val = 0 + (inc * i);
            Bgra32 pixel = new Bgra32((byte)val, (byte)val, (byte)val);
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
            tile.name = $"tile_image {i}";

            tiles.Add(tile);
        }

        return tiles;
    }

    private static Image<Bgra32> GeneratePixelBitmap(Image<Bgra32> image, int tilesize, List<Tile> tiles)
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

                Tile selected_tile = tiles[0];
                foreach(var tile in tiles)
                {
                    if (ComparePixels(tile.tile_image, tile_image))
                    {
                        selected_tile = tile;
                        break;
                    }

                }

                pixelBitmap[x,y] = selected_tile.pixel_representation; 

            }
        }
        return pixelBitmap;
    }

    private static Image<Bgra32> UpscaleBitmapToTilemap(Image<Bgra32> bitmap, int tilesize, List<Tile> tiles)
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


    private static bool GenerateTilemapFromBitmap(String name, int width, int height, int seed, List<Tile> tiles)
    {
        int N = 2;
        bool periodicInput = false;
        int symmetry = 1;
        bool ground = false;
        bool periodic = false;
        string heuristicString = "";
        var heuristic = Model.Heuristic.Entropy;
        var model = new OverlappingModel(name, N, width, height, periodicInput, periodic, symmetry, ground, heuristic);

        bool success = false;
        success = model.Run(seed, -1);
        if (success)
        {
            Console.WriteLine("DONE");
            model.SaveWithTiles($"output/{name}.png", tiles);
            return true;
        }
        Console.WriteLine("CONTRADICTION");
        return false;
    }

    public static Dictionary<String, Image<Bgra32>> LoadSampleTileset(String name)
    {
        var files = Directory.GetFiles($"tilesets/{name}");
        Dictionary<String, Image<Bgra32>> tiles = new Dictionary<String, Image<Bgra32>>();
        foreach (var file in files)
        {
            var tile_name = file.Split('/')[2].Split('.')[0];
            Image<Bgra32> tile = Image.Load<Bgra32>(file);
            
            tiles.Add(tile_name, tile);
        }

        return tiles;
    }

    unsafe public static void CompressAndDepcompressTilemap(String name, int[] data, int width, int height, int tilesize, string filename)
    {
        var bitmap_name = $"bitmap_{name}";
        Console.WriteLine($"name: {name}");
        fixed (int* pData = data)
        {
            Random random = new();
            using var image = Image.WrapMemory<Bgra32>(pData, width, height);
            var tile_images = ExtractTiles(image, tilesize);
            var unique_tiles = GetUniqueTiles(tile_images, tilesize);

            var checked_unique_tiles = CheckTilesForRotations(unique_tiles, tilesize);
            List<Tile> tiles = ConvertToTiles(checked_unique_tiles);

            tiles = MapTilesToPixels(tiles);

            //WARNING: You hardcoded the name of the file... Please fix this.
            var original_tiles = LoadSampleTileset("Summer");

            // so now we have to go through the original tiles, compare them with the tiles that we have right now and then create a new mapping.
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

            foreach(var item in tiles)
            {
                Console.WriteLine(item.ToString());
            }
            Image<Bgra32> bitmap = GeneratePixelBitmap(image, tilesize, tiles);
            bitmap.SaveAsPng($"samples/{bitmap_name}.png");

            // Use WFC to generate new bitmap
            int seed = random.Next();

            bool is_map_generated = false;
            while (!is_map_generated)
            {
                is_map_generated = GenerateTilemapFromBitmap(bitmap_name, width/tilesize, height/tilesize, seed, tiles);
                // NOTE: Please refactor this.
                if(!is_map_generated)
                {
                    seed = random.Next();
                }

            }

            using var bitmapFromModel = Image.Load<Bgra32>($"output/bitmap_{name}.png");

            Image<Bgra32> upscaled_image = UpscaleBitmapToTilemap(bitmapFromModel, tilesize, tiles);
            upscaled_image.SaveAsPng($"generated/upscaled_{name}.png");
            Console.WriteLine($"{width/tilesize}, {height/tilesize}");

        }
    }
    // compare rulesets against each other. Do a naive approach which is seeing how similar the rulesets are. For every rule that they share in common that is a plus one, and the total of the rules will then be used to compare. so it will be shared rules / total rules
    public static void GetRuleSimiliarity()
    {
        XElement crafted_rules = XDocument.Load("tilesets/Summer.xml").Root;
        XElement generated_rules = XDocument.Load("Summer 40x40_neighbors.xml").Root;

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

        var generated_rule_count =generated_rules.Descendants("neighbors").Descendants("neighbor").Count(); 
        var similiarity = (float)matching/(float)generated_rule_count;
        Console.WriteLine($"similiarity: {similiarity}");
        // Figure out other evaluations and why the similiarity is so low.


    }
}




