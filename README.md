AhoCorasick.net
================

Implementation of Aho-Corasick string matching algorithm on .NET  
TBA

Install
-------
It's available via [nuget package](https://www.nuget.org/packages/AhoCorasick.Net)  
PM> `Install-Package AhoCorasick.Net`

Or nuget package [with sources only](https://www.nuget.org/packages/AhoCorasick.Net.Source)  
PM> `Install-Package AhoCorasick.Net.Source`

Example Usage
-------------
```csharp
var botKeywords = new AhoCorasickTree(new[] { "abot", "apachebench", "googlebot", "libwww-perl", "etc" });
var userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.99 Safari/537.36";
var isBot = botKeywords.Contains(userAgent);
```
