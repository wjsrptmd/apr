using System.Diagnostics;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Runtime.InteropServices;

namespace apr.ViewModels;

public class NoseOverlayService
{
    public async Task<Result> AddRudolphNoseAsync(string imagePath, string imageServiceDir)
    {
        Result result = new Result();

        try
        {
            // 1. detect_nose 실행
            Console.WriteLine("image path {0}", imagePath);
            string exeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "detect_nose.exe"
                : "detect_nose";
            Console.WriteLine(exeName);
            string exeFilePath = System.IO.Path.Combine(imageServiceDir, exeName);
            string datPath = System.IO.Path.Combine(imageServiceDir, "shape_predictor_68_face_landmarks.dat");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exeFilePath,
                    Arguments = $"{imagePath} {datPath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string logFileName = $"{timestamp}.log";
            string logFilePath = System.IO.Path.Combine(imageServiceDir, "Data", logFileName);

            using (var writer = new StreamWriter(logFilePath, append: false))
            {
                writer.WriteLine($"[Command] {exeFilePath} {imagePath} {datPath}");
                writer.WriteLine($"[Timestamp] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                process.Start();

                string line;
                while ((line = process.StandardOutput.ReadLine()) is not null)
                {
                    writer.WriteLine(line);
                }

                process.WaitForExit();
            }


            Console.WriteLine($"Executing: {process.StartInfo.FileName} {process.StartInfo.Arguments}");

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            // 2. 결과 파싱
            Console.WriteLine(output);
            var match = Regex.Match(output, @"centerX=(\d+),\s*centerY=(\d+),\s*radius=(\d+)");
            if (!match.Success)
            {
                result.message = "Failed to parse detect_nose result";
                return result;
            }

            int centerX = int.Parse(match.Groups[1].Value);
            int centerY = int.Parse(match.Groups[2].Value);
            int radius = int.Parse(match.Groups[3].Value);

            // 3. ImageSharp 이미지 처리
            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                var center = new PointF(centerX, centerY);
                float radiusF = (float)radius;

                var brush = new RadialGradientBrush(
                    center,
                    radiusF,
                    GradientRepetitionMode.None,
                    new ColorStop(0f, Color.White),
                    new ColorStop(0.5f, Color.Red),
                    new ColorStop(1f, Color.DarkRed)
                );

                var noseCircle = new EllipsePolygon(center, radiusF);
                image.Mutate(ctx => ctx.Fill(brush, noseCircle));

                string extension = System.IO.Path.GetExtension(imagePath);
                string outputPath = System.IO.Path.Combine(imageServiceDir, "Data", $"output{extension}");
                await image.SaveAsync(outputPath);
                result.path = outputPath;
            }
        }
        catch (Exception ex)
        {
            result.message = ex.Message;
        }

        return result;
    }
}
