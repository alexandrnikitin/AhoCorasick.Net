using Xunit;

namespace AhoCorasick.Net.Tests.FieldReports
{
    public class Issue1_BugInFailures
    {
        [Fact]
        public void Test()
        {
            var sut = new AhoCorasickTree(new[] { "abcd", "bc" });
            Assert.True(sut.Contains("abc"));
        }
         
    }
}