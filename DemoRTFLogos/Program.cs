using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace DemoRTFLogos
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Base.rtf");
            var logo = Path.Combine(Environment.CurrentDirectory, "Logo.png");
            var image = GetImage(logo);
            if (!string.IsNullOrEmpty(image))
            {
                var newFile = Path.Combine(Environment.CurrentDirectory, string.Format("{0}.rtf", Guid.NewGuid()));
                string doc;
                using (var sr = new StreamReader(path))
                {
                    doc = sr.ReadToEnd();
                    sr.Close();
                }
                doc = doc.Replace("NIGGA", image);
                using (var sw = new StreamWriter(newFile))
                {
                    sw.WriteLine(doc);
                    sw.Close();
                }
                Process.Start(newFile);
            }
        }

        private static string GetImage(string path)
        {
            if (!File.Exists(path)) throw new ArgumentNullException();

            const int twip = 1440;
            const int hmm = 2540;
            const int dpi = 96;

            const string pictJpg = @"\pict\jpegblip\picw{0}\pich{1}\picwgoa{2}\pichgoa{3}\hex {4}90c28a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a0028a28a00fffd9";
            const string pictPng = @"\pict\pngblip\picw{0}\pich{1}\picwgoa{2}\pichgoa{3}\hex {4}ff308d938e611be8c7443980c92dae25dce197fd3b46cc58b34f32d4b7a5a43d334edbc365bc8bc7ff0b300044bbc9ba180f30320000000049454e44ae426082";

            var img = Image.FromFile(path);
            var image = new FileInfo(path);

            var imageString = ConvertToStringBuilder(BitConverter.ToString(File.ReadAllBytes(path)).Replace("-", string.Empty)).ToString();
            var width = Math.Round((Convert.ToDouble(img.Width) / dpi) * hmm);
            var height = Math.Round((Convert.ToDouble(img.Height) / dpi) * hmm);
            var wGoal = Math.Round((Convert.ToDouble(img.Width) / dpi) * twip);
            var hGoal = Math.Round((Convert.ToDouble(img.Height) / dpi) * twip);
            var text = image.Extension == ".png"
                ? string.Format(pictPng, width, height, wGoal, hGoal, imageString)
                : string.Format(pictJpg, width, height, wGoal, hGoal, imageString);
            return "{" + text + "}";
        }

        private static StringBuilder ConvertToStringBuilder(string text, int maxLen = 128)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException();
            var sb = new StringBuilder();
            var maxRows = Math.Round(Convert.ToDecimal(text.Length / maxLen), MidpointRounding.AwayFromZero);
            for (int x = 0; x < maxRows; x++)
            {
                var skip = x * maxLen;
                var diff = text.Length - (skip + maxLen);
                var total = maxLen <= diff ? maxLen : diff;
                var line = text.Substring(skip, total).ToLowerInvariant();
                if (diff > maxLen)
                    sb.AppendLine(line);
                else
                    sb.Append(line);
            }
            return sb;
        }
    }
}