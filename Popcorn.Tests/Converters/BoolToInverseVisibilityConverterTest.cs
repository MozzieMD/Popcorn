using System.Globalization;
using System.Windows;
using NUnit.Framework;
using Popcorn.Converters;

namespace Popcorn.Tests.Converters
{
    [TestFixture]
    [Timeout(1000)]
    public class BoolToInverseVisibilityConverterTest
    {
        [Test]
        public void BoolToInverseConvertTest()
        {
            var converter = new BoolToInverseVisibilityConverter();
            Assert.That(
                converter.Convert(true, typeof (Visibility), null, CultureInfo.CurrentUICulture)
                    .Equals(Visibility.Visible));
            Assert.That(
                converter.Convert(false, typeof (Visibility), null, CultureInfo.CurrentUICulture)
                    .Equals(Visibility.Collapsed));
        }
    }
}