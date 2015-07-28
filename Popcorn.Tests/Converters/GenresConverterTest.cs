using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Popcorn.Converters;
using TMDbLib.Objects.General;

namespace Popcorn.Tests.Converters
{
    [TestFixture]
    [Timeout(1000)]
    public class GenresConverterTest
    {
        [Test]
        public void GenresConvertTest()
        {
            var converter = new GenresConverter();
            var value = new Fixture().Create<List<string>>();
            var converted = converter.Convert(value, null, null, CultureInfo.CurrentUICulture);

            Assert.That(converted, Is.Not.Null);

            foreach (var item in value)
            {
                Assert.That(converted,Is.StringContaining(item));
            }
        }
    }
}
