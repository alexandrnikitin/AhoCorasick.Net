using System;

namespace AhoCorasick.Net.Benchmarks
{
    public class RandomString
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private readonly Random _random = new Random();

        public string GetRandomString(int length)
        {
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = Chars[_random.Next(Chars.Length)];
            }

            return new string(stringChars);
        }
    }
}