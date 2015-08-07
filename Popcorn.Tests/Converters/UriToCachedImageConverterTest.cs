using System.Windows.Media.Imaging;
using NUnit.Framework;
using Popcorn.Converters;

namespace Popcorn.Tests.Converters
{
    [TestFixture]
    [Timeout(1000)]
    public class UriToCachedImageConverterTest
    {
        [Test]
        public void ConvertURi()
        {
            var converter = new UriToCachedImageConverter();
            var value = "http://www.google.com/";

            var result = converter.Convert(value, null, null, null);
            Assert.That(result, Is.TypeOf<BitmapImage>());

            var image = (BitmapImage) result;
            Assert.That(image.UriSource.ToString(), Is.EqualTo(value));
        }
    }
}