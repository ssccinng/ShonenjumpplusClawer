// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

string eps = "14079602755157014730";
var url = $"https://shonenjumpplus.com/episode/{eps}";

var imgUrls = await GetImgUrlAsync(url);
string folder = $"images/{eps}";
if (!Directory.Exists(folder)) {
    Directory.CreateDirectory(folder);
}
using HttpClient fs = new HttpClient();
int idx = 0;
foreach (var imgUrl in imgUrls) {
    var img = await (await fs.GetAsync(imgUrl)).Content.ReadAsByteArrayAsync();
    Bitmap image = new Bitmap(new MemoryStream(img));
    Bitmap newImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    using (Graphics g = Graphics.FromImage(newImage))
    {
        g.DrawImage(image, new Rectangle(0, 0, newImage.Width, newImage.Height));
    }
    image.Dispose();
    image = newImage;
    FixImage( image);
    image.Save($"{folder}/{idx++}.png", ImageFormat.Png);;
    // await File.WriteAllBytesAsync($"{folder}/{idx++}.jpg", img);
    // break;
}
async Task<List<string>> GetImgUrlAsync(string url) {
    Regex regex = new Regex(@"https://cdn-ak-img.shonenjumpplus.com/public/page.+?&quot;");
    List<string> imgUrls = new List<string>();
    using var client = new HttpClient();
    var html = await client.GetAsync(url).Result.Content.ReadAsStringAsync();
    var matches = regex.Matches(html);
    foreach (Match match in matches) {
        imgUrls.Add(match.Groups[0].Value.Replace("&quot;", ""));
    }

    return imgUrls;
}   

void FixImage( Bitmap image) {
    int[][] dx = new int[][] {
        new int[] { 0, 1, 1, 0 },
        new int[] { 2, 3, 3, 2 },
        new int[] { 0, 3, 3, 0 },
        new int[] { 0, 2, 2, 0 },
        new int[] { 1, 2, 2, 1 },
        new int[] { 1, 3, 3, 1 },

    };
    for (int i = 0; i < dx.Length; ++i) {
        SwapRegion(image, GetRect(dx[i][0], dx[i][1], image.Width - 28, image.Height - 15), 
        
        GetRect(dx[i][2], dx[i][3], image.Width - 28, image.Height - 15));
    }



}

Rectangle GetRect(int x, int y, int width, int height) {
    return new Rectangle(width / 4 * x, height / 4 * y, width / 4, 
    height / 4);
}

// 交换图片某两个区域
// Path: ShonenjumpplusClawer\Program.cs

void SwapRegion(  Bitmap bitmap, Rectangle region1, Rectangle region2) {
    System.Console.WriteLine(region1);
    System.Console.WriteLine(region2);
    using var region1Img = bitmap.Clone(region1, bitmap.PixelFormat);
    using var region2Img = bitmap.Clone(region2, bitmap.PixelFormat);
    // region1Img.Save("test1.jpg");
    // region2Img.Save("test2.jpg");
    using Graphics g = Graphics.FromImage(bitmap);
    g.DrawImage(region1Img, region2);
    g.DrawImage(region2Img, region1);
}