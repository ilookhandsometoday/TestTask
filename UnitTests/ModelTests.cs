using NUnit.Framework;
using TestTask;
using System.Threading.Tasks;
namespace UnitTests
{
    public class ModelTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetContentLength_DogPicture_164005()
        {
            //arrange
            Model model = new Model();
            string url = "https://i.pinimg.com/originals/77/a8/d5/77a8d552e2b48c1876cede11a7d89c95.jpg";

            //act
            long? result = model.GetContentLength(url);

            //assert
            Assert.AreEqual(result, 164005L);
        }
    }
}