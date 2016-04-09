using AhoCorasick.Net.Benchmarks.Sandbox;

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
        private readonly AhoCorasickTreeHashBased _sut2;

        public SmallTreeBenchmark()
        {
            var randomString = new RandomString();
            var keywords = new string[NumberOfKeywords];
            for (int i = 0; i < NumberOfKeywords; i++)
            {
                keywords[i] = randomString.GetRandomString(LengthOfKeyword);
            }
            _sut = new AhoCorasickTree(keywords);
            _sut2 = new AhoCorasickTreeHashBased(keywords);

            _keyword = keywords[0];
        }

        [Benchmark(Baseline = true)]
        
        public bool ContainsSmallWord()
        {
            return _sut.Contains(_keyword);
        }

        [Benchmark]
        public bool ContainsSmallWordImproved()
        {
            return _sut2.Contains(_keyword);
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