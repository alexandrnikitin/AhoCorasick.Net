using System;

using Xunit;
using Xunit.Extensions;

namespace AhoCorasick.Net.Tests
{
    public class AhoCorasickTreeTests
    {
        [Fact]
        public void CanCreate()
        {
            var sut = new AhoCorasickTreeBasedOnArray(new[] { "ab", "ab" });
        }

        [Theory]
        [InlineData("d", false)]
        [InlineData("bce", false)]
        [InlineData("bcd", true)]
        [InlineData("abcd", true)]
        public void Contains(string str, bool expected)
        {
            var sut = new AhoCorasickTreeBasedOnArray(new[] { "ab", "abc", "bcd" });
            Assert.Equal(expected, sut.Contains(str));
        }

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData(new string[] { }, typeof(ArgumentException))]
        public void InvalidKeywords(string[] keywords, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => new AhoCorasickTree(keywords));
        }

        [Fact]
        public void Performance()
        {
        }
    }
}