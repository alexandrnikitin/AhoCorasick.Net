using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace AhoCorasick.Net.Benchmarks
{
    //[Config(typeof(Config))]
    public class SmallTreeBenchmark
    {
        private const int LengthOfKeyword = 8;
        private const int NumberOfKeywords = 100;
        private readonly string _keyword;
        private readonly AhoCorasickTree _sut;

        public SmallTreeBenchmark()
        {
            var randomString = new RandomString();
            var keywords = new string[NumberOfKeywords];
            for (int i = 0; i < NumberOfKeywords; i++)
            {
                keywords[i] = randomString.GetRandomString(LengthOfKeyword);
            }
            _sut = new AhoCorasickTree(keywords);

            _keyword = keywords[0];
        }

        [Benchmark]
        public bool ContainsSmallWord()
        {
            return _sut.Contains(_keyword);
        }

        private class Config : ManualConfig
        {
            public Config()
            {
                Add(
                    new Job
                        {
                            Platform = Platform.X64,
                            Jit = Jit.LegacyJit,
                            LaunchCount = 1,
                            WarmupCount = 3,
                            TargetCount = 3,
                        });
            }
        }
    }
}