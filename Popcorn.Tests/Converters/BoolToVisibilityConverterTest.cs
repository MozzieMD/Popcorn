using System.Globalization;
using System.Windows;
using NUnit.Framework;
using Popcorn.Converters;

namespace Popcorn.Tests.Converters
{
    [TestFixture]
    [Timeout(1000)]
    public class BoolToVisibilityConverterTest
    {
        [Test]
        public void BoolToVisibilityConvertTest()
        {
            var converter = new BoolToVisibilityConverter();
            Assert.That(converter.Convert(false, typeof(Visibility), null, CultureInfo.CurrentUICulture).Equals(Visibility.Visible));
            Assert.That(converter.Convert(true, typeof(Visibility), null, CultureInfo.CurrentUICulture).Equals(Visibility.Collapsed));
        }
    }
}
