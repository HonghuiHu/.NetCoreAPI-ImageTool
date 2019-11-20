using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Images.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        // GET: api/Images
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Images/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Images
        [HttpPost]
        public string Post([FromForm] IFormCollection formCollection)
        {
            if (formCollection.Files.Count == 0)
            {
                throw new Exception("没有选择文件！");
            }

            //这里接收来自前台上传的图片（这里是直接从前台传过来的， 也可以通过URL自己读取file文件，最终是个file文件就行）
            var file = formCollection.Files[0];

            //文件将要压缩的比例（file.ContentLength是当前文件的大小，这里默认将文件压缩为1024kb，可以自己调）
          //  double compressionRatio = 1024 * 1024 / Convert.ToDouble(file.Length);
          //  compressionRatio = Math.Round(compressionRatio, 2);

            //上传文件转为byte数组
            byte[] fileByte = new byte[file.Length];
            
            file.OpenReadStream().Read(fileByte, 0, Convert.ToInt32(file.Length));
            //上传文件的byte数组转为Stream
            MemoryStream ms = new MemoryStream(fileByte);
            Image img = Image.FromStream(ms);

            //按比例计算新的宽高
            int toWidth = 100;//Convert.ToInt32(img.Width * compressionRatio);
            int toHeight = 100;//Convert.ToInt32(img.Height * compressionRatio);

            //按照新的宽高用画布重新画一张
            Bitmap bitmap = new Bitmap(toWidth, toHeight);
            Graphics g = Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, toWidth, toHeight), new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.GraphicsUnit.Pixel);

            //将画好的bitmap转成stream（不一定费时stream，byte数组什么都可以）
            var fileStream = new MemoryStream();

            using MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            byte[] data = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(data, 0, Convert.ToInt32(stream.Length));
            fileStream = new MemoryStream(data);
            return Convert.ToBase64String(data);


        }

        // PUT: api/Images/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
