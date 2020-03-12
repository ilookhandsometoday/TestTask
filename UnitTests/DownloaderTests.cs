using NUnit.Framework;
using TestTask;
using System.Threading.Tasks;
using System.IO;

namespace UnitTests
{
    public class DownloaderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetContentLength_DogPicture_164005()
        {
            //arrange
            Downloader downloader = new Downloader();
            string url = "https://i.pinimg.com/originals/77/a8/d5/77a8d552e2b48c1876cede11a7d89c95.jpg";

            //act
            long? result = await downloader.GetContentLength(url);

            //assert
            Assert.AreEqual(result, 164005L);
        }

        [Test]
        public async Task GetExtension_DogPicture_jpg()
        {
            //arrange
            Downloader downloader = new Downloader();
            string url = "https://i.pinimg.com/originals/77/a8/d5/77a8d552e2b48c1876cede11a7d89c95.jpg";

            //act
            string extension = await downloader.GetExtension(url);

            //assert
            Assert.AreEqual(extension, ".jpg");
        }

        //no DownloadImage unit test because I had some problems with System.Windows.Media.Imaging namespace
    }
}