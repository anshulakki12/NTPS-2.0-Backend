using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private static readonly Random _random = new Random();
        private readonly IMemoryCache _cache;

        public CaptchaController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("generate")]
        public IActionResult GenerateCaptcha()
        {
            // 1️⃣ Generate CAPTCHA text
            string captchaText = GenerateCaptchaText(5);

            // 2️⃣ Create a unique token
            string token = Guid.NewGuid().ToString();

            // 3️⃣ Store CAPTCHA in MemoryCache for 5 minutes
            _cache.Set(token, captchaText, TimeSpan.FromMinutes(5));

            // 4️⃣ Generate CAPTCHA image
            using var bmp = new Bitmap(160, 60);
            using var g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // Background gradient
            using var brushBackground = new LinearGradientBrush(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                Color.LightGray,
                Color.White,
                45f);
            g.FillRectangle(brushBackground, 0, 0, bmp.Width, bmp.Height);

            // Grid lines
            using var gridPen = new Pen(Color.FromArgb(60, 200, 200, 200), 1);
            for (int i = 0; i < bmp.Width; i += 15)
                g.DrawLine(gridPen, i, 0, i, bmp.Height);
            for (int j = 0; j < bmp.Height; j += 15)
                g.DrawLine(gridPen, 0, j, bmp.Width, j);

            // CAPTCHA text
            string[] fonts = { "Arial", "Calibri", "Tahoma", "Verdana" };
            for (int i = 0; i < captchaText.Length; i++)
            {
                using var font = new Font(fonts[_random.Next(fonts.Length)], 28, FontStyle.Bold);
                using var brush = new SolidBrush(Color.FromArgb(
                    _random.Next(40, 100), _random.Next(40, 100), _random.Next(40, 100)));

                float x = 20 + i * 25;
                float y = _random.Next(5, 15);

                g.TranslateTransform(x, y);
                g.RotateTransform(_random.Next(-10, 10));
                g.DrawString(captchaText[i].ToString(), font, brush, 0, 0);
                g.ResetTransform();
            }

            // Noise lines
            for (int i = 0; i < 3; i++)
            {
                using var pen = new Pen(Color.FromArgb(120, _random.Next(255), _random.Next(255), _random.Next(255)), 2);
                g.DrawCurve(pen, GenerateRandomPoints(bmp.Width, bmp.Height));
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            // 5️⃣ Return image as Base64 along with token
            string base64Image = Convert.ToBase64String(ms.ToArray());
            return Ok(new { Token = token, ImageBase64 = base64Image });
        }

        [HttpPost("validate")]
        public IActionResult ValidateCaptcha([FromBody] CaptchaValidationRequest request)
        {
            if (!_cache.TryGetValue(request.Token, out string storedCaptcha))
                return BadRequest(new { message = "Captcha expired or invalid token." });

            if (!string.Equals(request.CaptchaInput, storedCaptcha, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Invalid captcha." });

            _cache.Remove(request.Token); // remove after successful validation
            return Ok(new { message = "Captcha valid." });
        }

        private string GenerateCaptchaText(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private PointF[] GenerateRandomPoints(int width, int height)
        {
            PointF[] points = new PointF[_random.Next(3, 5)];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF(_random.Next(width), _random.Next(height));
            }
            return points;
        }
    }

    public class CaptchaValidationRequest
    {
        public string CaptchaInput { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
