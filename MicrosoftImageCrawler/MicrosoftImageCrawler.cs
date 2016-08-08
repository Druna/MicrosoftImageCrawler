using System;
using System.Drawing;
using System.IO;

namespace MicrosoftImageCrawler
{
    static class MicrosoftImageCrawler
    {
        static int Main(string[] args)
        {
            try
            {
                int width;
                if (!int.TryParse(Properties.Settings.Default.MinWidth, out width))
                {
                    Console.WriteLine("MinWidth parameter with must be number");
                    return 2;
                }

                var pathDest = Properties.Settings.Default.Destination;
                if(!Directory.Exists(pathDest))
                {
                    Directory.CreateDirectory(pathDest);
                }

                var pathSrc = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.Source);

                var dirInfo = new DirectoryInfo(pathSrc);

                var colorBackup = Console.ForegroundColor;

                var listOfFiles = dirInfo.GetFiles();

                var addedCount = 0;

                foreach (var srcInfo in listOfFiles)
                {
                    var fileName = srcInfo.Name;
                    var fileDest = Path.Combine(pathDest, fileName + ".jpg");
                    var fileSrc = srcInfo.FullName;
                    if(File.Exists(fileDest))
                    {
                        var destInfo = new FileInfo(fileDest);
                        if(srcInfo.Length == destInfo.Length)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("= File exist : " + destInfo.Name);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("? Files name <" + destInfo.Name + "> are exist in the source and in the destination directory and they have different length");
                        }
                        continue;
                    }
                    var srcWidth = GetWidth(fileSrc);
                    if (srcWidth < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("- Not image : " + fileName);
                    }
                    else if (srcWidth < width)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("- Image width " + srcWidth + " too small : " + fileName);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("+ Image added : " + fileName + ".jpg");
                        File.Copy(fileSrc, fileDest);
                        addedCount++;
                    }
                }

                if(addedCount > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Processed " + listOfFiles.Length + " files, added " + addedCount + " images");
                    Console.WriteLine("Press any key to... do you know why");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Processed " + listOfFiles.Length + " files, new images not found");
                }
                Console.ForegroundColor = colorBackup;

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        private static int GetWidth(string file)
        {
            try
            {
                int width;
                using (var bmp = Image.FromFile(file))
                {
                    width = bmp.Width;
                }
                return width;
            }
            catch
            {
                return -1;
            }
        }
    }
}