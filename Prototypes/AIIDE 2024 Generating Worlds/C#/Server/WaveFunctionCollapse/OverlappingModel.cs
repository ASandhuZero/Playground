// Copyright (C) 2016 Maxim Gumin, The MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class OverlappingModel : Model
{
    List<byte[]> patterns;
    List<int> colors;

    // after we generate this xml, we then need to do a tileset remappping somehow. I think we could just call a function within GenerateXML that would deal with this, but also I think that after we generate the XML, then we can go through and replace the pixel values with their corresponding tile values.
    // ... yeah that makes sense.
    public XElement GenerateXML(Dictionary<int, HashSet<int>> pixel_associations, Dictionary<int, String> tileColorMapping)
    {

        //TODO: Okay at some point we are going to want to model the Summer.xml for hwo to create the xml
        XElement neighbor_ruleset = new XElement("set");
        XElement neighbors = new XElement("neighbors");
        foreach(var entry in pixel_associations)
        {
            foreach(var item in entry.Value)
            {
                // we need to change the entry key and item to the tile names but we can deal with that in a second, I think.
                XElement neighbor = new XElement("neighbor");
                neighbor.SetAttributeValue("left", tileColorMapping[entry.Key]);
                neighbor.SetAttributeValue("right", tileColorMapping[item]);
                neighbors.Add(neighbor);
            }
        }
        neighbor_ruleset.Add(neighbors);
        Console.WriteLine(neighbor_ruleset.ToString());
        return neighbor_ruleset;
    }

    public OverlappingModel(string name, int N, int width, int height, bool periodicInput, bool periodic, int symmetry, bool ground, Heuristic heuristic)
        : base(width, height, N, periodic, heuristic)
    {
        //NOTE: You should make a new pixel to pattern map here.
        var (bitmap, SX, SY) = BitmapHelper.LoadBitmap($"samples/{name}.png");
        byte[] sample = new byte[bitmap.Length];
        // colors are actually the pixel colors themselves, so follow that.
        colors = new List<int>();
        for (int i = 0; i < sample.Length; i++)
        {
            int color = bitmap[i];
            int k = 0;
            for (; k < colors.Count; k++)
            {
                if (colors[k] == color)
                {
                    break;
                }
            }
            if (k == colors.Count)
            {
                colors.Add(color);
            }
            sample[i] = (byte)k;
        }

        static byte[] pattern(Func<int, int, byte> f, int N)
        {
            byte[] result = new byte[N * N];
            for (int y = 0; y < N; y++)
            {
                for (int x = 0; x < N; x++)
                {
                    result[x + y * N] = f(x, y);
                }
            }
            return result;
        };

        static byte[] rotate(byte[] p, int N) => pattern((x, y) => p[N - 1 - y + x * N], N);
        static byte[] reflect(byte[] p, int N) => pattern((x, y) => p[N - 1 - x + y * N], N);

        // I think hash ensures that each pattern has it's own unique identifier
        // ahhh, so I am starting to see why there are so many patterns, basically each pixel will have a pattern and that will capture all relationships, the problem is that since the patterns are NxN rather than looking at the adjacent pixels and taking that pixel value and mapping it to a relationship dictionary, WFC will look at the filter that is being passed through the image, then from those patterns, it will know that if a specific pixel is there, then it should only allow the other patterns around it.
        static long hash(byte[] p, int C)
        {
            long result = 0, power = 1;
            for (int i = 0; i < p.Length; i++)
            {
                result += p[p.Length - 1 - i] * power;
                power *= C;
            }
            return result;
        };

        patterns = new();
        Dictionary<long, int> patternIndices = new();
        List<double> weightList = new();

        int xmax = periodicInput ? SX : SX - N + 1;
        int ymax = periodicInput ? SY : SY - N + 1;

        for (int y = 0; y < ymax; y++) for (int x = 0; x < xmax; x++)
        {
            byte[][] ps = new byte[8][];

            // this is the sample pixel that is being pulled. That sample pixel is then being learned about through the pattern function.
            // so this is insane, apparently this anonymous function is accessing THE ADAJACENT NEIGHBORS. Because we have x defined here which is the pixel that we are on, and the dx and dy come from the pattern function which maps to 0 through N
            // okay okay okay yes for sure, the pattern is where it's getting back information about the pixel associations,
            ps[0] = pattern((dx, dy) => sample[(x + dx) % SX + (y + dy) % SY * SX], N);
            ps[1] = reflect(ps[0], N);
            ps[2] = rotate(ps[0], N);
            ps[3] = reflect(ps[2], N);
            ps[4] = rotate(ps[2], N);
            ps[5] = reflect(ps[4], N);
            ps[6] = rotate(ps[4], N);
            ps[7] = reflect(ps[6], N);

            for (int k = 0; k < symmetry; k++)
            {
                byte[] p = ps[k];
                long h = hash(p, colors.Count);

                if (patternIndices.TryGetValue(h, out int index))
                {
                    weightList[index] = weightList[index] + 1;
                }
                else
                {
                    patternIndices.Add(h, weightList.Count);
                    weightList.Add(1.0);
                    patterns.Add(p);
                }
            }
        }

        // We need to output the pixel_associations dictionary to a file
        // So we should generate XML file that should like the samples
        // so we should make a function that generates an XML file

        // this is where the patterns are placed, so T is the number of all the patterns that have been learned.
        weights = weightList.ToArray();
        T = weights.Length;
        this.ground = ground;

        // I think this is what causes the subset for each pattern. As in pattern N must agree with all its surrounding patterns
        static bool agrees(byte[] p1, byte[] p2, int dx, int dy, int N)
        {
            int xmin = dx < 0 ? 0 : dx; 
            int xmax = dx < 0 ? dx + N : N;
            int ymin = dy < 0 ? 0 : dy;
            int ymax = dy < 0 ? dy + N : N;

            for (int y = ymin; y < ymax; y++)
            {
                for (int x = xmin; x < xmax; x++)
                {
                    if (p1[x + N * y] != p2[x - dx + N * (y - dy)])
                    {
                        return false;
                    }
                }
            }
            return true;
        };

        propagator = new int[4][][];
        for (int d = 0; d < 4; d++)
        {
            propagator[d] = new int[T][];
            for (int t = 0; t < T; t++)
            {
                List<int> list = new();
                for (int t2 = 0; t2 < T; t2++)
                {
                    if (agrees(patterns[t], patterns[t2], dx[d], dy[d], N))
                    {
                        list.Add(t2);
                    }
                }
                propagator[d][t] = new int[list.Count];
                for (int c = 0; c < list.Count; c++)
                {
                    propagator[d][t][c] = list[c];
                }
            }
        }
    }

    public override void Save(string filename)
    {
        var boole = observed[0] >= 0;
        foreach (var item in observed)
        {
            Console.WriteLine($"item: {item}");
            if (item == -1)
            {
                boole = false;
            }
            
        }
        int[] bitmap = new int[MX * MY];
        if (boole)
        {
            for (int y = 0; y < MY; y++)
            {
                int dy = y < MY - N + 1 ? 0 : N - 1;
                for (int x = 0; x < MX; x++)
                {
                    int dx = x < MX - N + 1 ? 0 : N - 1;
                    int i = x - dx + (y - dy) * MX;
                    int j = dx + dy * N;

                    var observed_index = observed[i];
                    var pattern_index = patterns[observed_index][j];
                    bitmap[x + y * MX] = colors[pattern_index];
                }
            }
        }
        else
        {
            for (int i = 0; i < wave.Length; i++)
            {
                int contributors = 0, r = 0, g = 0, b = 0;
                int x = i % MX, y = i / MX;
                for (int dy = 0; dy < N; dy++) for (int dx = 0; dx < N; dx++)
                    {
                        int sx = x - dx;
                        if (sx < 0) sx += MX;

                        int sy = y - dy;
                        if (sy < 0) sy += MY;

                        int s = sx + sy * MX;
                        if (!periodic && (sx + N > MX || sy + N > MY || sx < 0 || sy < 0))
                        { 
                            continue;
                        }
                        for (int t = 0; t < T; t++)
                        {
                            if (wave[s][t])
                            {
                                contributors++;
                                int argb = colors[patterns[t][dx + dy * N]];
                                r += (argb & 0xff0000) >> 16;
                                g += (argb & 0xff00) >> 8;
                                b += argb & 0xff;
                            }
                        }
                    }
                bitmap[i] = unchecked((int)0xff000000 | ((r / contributors) << 16) | ((g / contributors) << 8) | b / contributors);
            }
        }
        BitmapHelper.SaveBitmap(bitmap, MX, MY, filename);
    }

    public void SaveImageAndRules(string filename, List<Tile> tiles)
    {
        var boole = observed[0] >= 0;
        foreach (var item in observed)
        {
            Console.WriteLine($"item: {item}");
            if (item == -1)
            {
                boole = false;
            }
            
        }
        int[] bitmap = new int[MX * MY];
        if (boole)
        {
            for (int y = 0; y < MY; y++)
            {
                int dy = y < MY - N + 1 ? 0 : N - 1;
                for (int x = 0; x < MX; x++)
                {
                    int dx = x < MX - N + 1 ? 0 : N - 1;
                    int i = x - dx + (y - dy) * MX;
                    int j = dx + dy * N;

                    var observed_index = observed[i];
                    var pattern_index = patterns[observed_index][j];
                    bitmap[x + y * MX] = colors[pattern_index];
                }
            }
        }
        else
        {
            for (int i = 0; i < wave.Length; i++)
            {
                int contributors = 0, r = 0, g = 0, b = 0;
                int x = i % MX, y = i / MX;
                for (int dy = 0; dy < N; dy++) for (int dx = 0; dx < N; dx++)
                    {
                        int sx = x - dx;
                        if (sx < 0) sx += MX;

                        int sy = y - dy;
                        if (sy < 0) sy += MY;

                        int s = sx + sy * MX;
                        if (!periodic && (sx + N > MX || sy + N > MY || sx < 0 || sy < 0))
                        { 
                            continue;
                        }
                        for (int t = 0; t < T; t++)
                        {
                            if (wave[s][t])
                            {
                                contributors++;
                                int argb = colors[patterns[t][dx + dy * N]];
                                r += (argb & 0xff0000) >> 16;
                                g += (argb & 0xff00) >> 8;
                                b += argb & 0xff;
                            }
                        }
                    }
                bitmap[i] = unchecked((int)0xff000000 | ((r / contributors) << 16) | ((g / contributors) << 8) | b / contributors);
            }
        }
        BitmapHelper.SaveBitmap(bitmap, MX, MY, filename);


        var tileColorMapping = new Dictionary<int, String>(); 
        // NOTE: Additional code that I added. 
        // Now we need to get the name of the tiles and have that be the thing that we save.
        foreach (var item in colors)
        {

            Bgra32 pixel = new Bgra32((byte)item, (byte)item, (byte)item);
            foreach(var tile in tiles)
            {
                if(tile.pixel_representation.Equals(pixel))
                {
                    tileColorMapping.Add(item, tile.name);
                }
            }
        }
        foreach (var item in tileColorMapping)
        {
            Console.WriteLine($"item: {item.Key}, content: {item.Value}");
        }

        var pixel_associations = new Dictionary<int, HashSet<int>>();

        for(int i = 0; i < patterns.Count; i++)
        {
            var entry = patterns[i];
            var key = colors[entry[0]];
            if (pixel_associations.ContainsKey(key))
            {
                pixel_associations[key].Add(colors[entry[1]]);
            }
            else
            {
                var right_side_pixels = new HashSet<int>();
                pixel_associations.Add(colors[entry[0]], right_side_pixels);
            }
        }
        XElement neighbors = GenerateXML(pixel_associations, tileColorMapping);
        var XMLfilename = filename.Split('.');

        //TODO: I know this is tech debt, but alas here it is.
        neighbors.Save(XMLfilename[0].Split('_')[1] + "_neighbors.xml");
    }
}
