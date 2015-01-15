using Xunit;

namespace AhoCorasick.Net.Tests
{
    public class AhoCorasickTreeTests
    {
        [Fact]
        public void CanCreate()
        {
            var sut = new AhoCorasickTree(null);
            Assert.NotNull(sut);
        }
    }
}