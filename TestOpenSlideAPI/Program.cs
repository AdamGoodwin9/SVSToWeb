using OpenSlideCs;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestOpenSlideAPI
{
    class Program
    {
        static void GetJpg(OpenSlide openSlide, int level, int row, int col, string filename, string outputname)
        {
            using (var stream = openSlide.GetJpg(filename, $"_files/{level}/{row}_{col}.jpeg"))
            {
                if (stream.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    Console.WriteLine($"{outputname}.jpeg successfully created!");
                    File.WriteAllBytes($"{outputname}.jpeg", buffer.ToArray());
                }
                else
                {
                    Console.WriteLine($"{outputname}.jpeg failed...");
                }
            }
        }

        static void GetDZI(OpenSlide openSlide, long width, long height, string filename, string outputname)
        {
            using (var stream = openSlide.GetDZI(filename, out width, out height))
            {
                if (stream.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    Console.WriteLine($"{outputname}.dzi successfully created!");
                    File.WriteAllBytes($"{outputname}.dzi", buffer.ToArray());
                }
                else
                {
                    Console.WriteLine($"{outputname}.dzi failed...");
                }
            }
        }

        static void Main(string[] args)
        {
            string filename = "CMU-1-Small-Region";
            var openSlide = new OpenSlide();

            //var test = openSlide.GetMPP("CMU-1-Small-Region.svs"); //MPP = microns per pixel
            

            SizeL[] levels = openSlide.Levels($"{filename}.svs");

            List<SizeL> dimensions = openSlide.Dimensions($"{filename}.svs");

            SizeL maxDimensions = dimensions[dimensions.Count-1];


            //Loads DZI file
            GetDZI(openSlide, maxDimensions.Width, maxDimensions.Height, $"{filename}.svs", filename);

            for (int level = 0; level < levels.Length; level++)
            {
                Directory.CreateDirectory($"{filename}_files/{level}");

                SizeL levelInfo = levels[level];

                for (int row = 0; row < levelInfo.Width; row++)
                {
                    for (int col = 0; col < levelInfo.Height; col++)
                    {
                        GetJpg(openSlide, level, row, col, $"{filename}.svs", $"{filename}_files/{level}/{row}_{col}");
                    }
                }
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}