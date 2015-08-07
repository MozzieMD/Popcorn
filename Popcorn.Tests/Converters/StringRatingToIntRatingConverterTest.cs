using NUnit.Framework;
using Ploeh.AutoFixture;
using Popcorn.Converters;

namespace Popcorn.Tests.Converters
{
    [TestFixture]
    [Timeout(1000)]
    public class StringRatingToIntRatingConverterTest
    {
        [Test]
        public void ConvertValidString()
        {
            var converter = new StringRatingToIntRatingConverter();
            var value = "100";

            var result = converter.Convert(value, null, null, null);

            Assert.That(result, Is.EqualTo((double) 50));
            Assert.That(result, Is.TypeOf<double>());
        }
    }
}