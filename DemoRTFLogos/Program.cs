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

            const string picture = @"\pict\pngblip\picw{0}\pich{1}\picwgoa{2}\pichgoa{3}\hex {4}";

            var img = Image.FromFile(path);

            var imageString = ConvertToStringBuilder(BitConverter.ToString(File.ReadAllBytes(path)).Replace("-", string.Empty)).ToString();
            var width = Math.Round((Convert.ToDouble(img.Width) / dpi) * hmm);
            var height = Math.Round((Convert.ToDouble(img.Height) / dpi) * hmm);
            var wGoal = Math.Round((Convert.ToDouble(img.Width) / dpi) * twip);
            var hGoal = Math.Round((Convert.ToDouble(img.Height) / dpi) * twip);
            return "{" + string.Format(picture, width, height, wGoal, hGoal, imageString) + "}";

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
                sb.AppendLine(text.Substring(skip, total).ToLowerInvariant());
            }
            return sb;
        }
    }
}