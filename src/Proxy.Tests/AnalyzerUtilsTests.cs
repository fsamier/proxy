using System;
using Xunit;

namespace Proxy.Tests
{
    public class AnalyzerUtilsTests
    {
        [Theory]
        [InlineData("test.ts", RequestType.SEGMENT)]
        [InlineData("test.m3u8", RequestType.MANIFEST)]
        [InlineData("test.m3u", RequestType.MANIFEST)]
        [InlineData("test.tss", RequestType.UNKNOWN)]
        [InlineData("test", RequestType.UNKNOWN)]
        public void GetRequestTypeTest(string url, RequestType expected)
        {
            var result = AnalyzerUtils.GetRequestType(url);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void PatchToRelativeTest_NoUri()
        {
            var content = "Nothing important\nAt all";
            var manifestUri = new Uri("http://localhost");

            var result = AnalyzerUtils.PatchToRelative(manifestUri, content);
            Assert.Null(result);
        }

        // As Method mostly relies on System.Uri.MakeRelativeUri, no need to include
        // tests to validade that relative URIs are correctly generated
        [Fact]
        public void PatchToRelativeTest_AbsoluteUri()
        {
            var content = @"#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:3
#EXT-X-MEDIA-SEQUENCE:3205244
#EXT-X-DISCONTINUITY-SEQUENCE:0
#EXTINF:2.0,
http://wowza-test.domainname.io/liveOriginAbsolute/_definst_/sintel-live.smil/media-uidwhtlta_b1128000_3205244.ts
#EXTINF:2.0,
http://wowza-test.domainname.io/liveOriginAbsolute/_definst_/sintel-live.smil/media-uidwhtlta_b1128000_3205245.ts
#EXTINF:2.0,
http://wowza-test.domainname.io/liveOriginAbsolute/_definst_/sintel-live.smil/media-uidwhtlta_b1128000_3205246.ts
";

            var manifestUri = new Uri("http://wowza-test.domainname.io/liveOriginAbsolute/_definst_/sintel-live.smil/chunklist_b1128000.m3u8");

            var result = AnalyzerUtils.PatchToRelative(manifestUri, content);

            Assert.NotNull(result);
            Assert.Equal(@"#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:3
#EXT-X-MEDIA-SEQUENCE:3205244
#EXT-X-DISCONTINUITY-SEQUENCE:0
#EXTINF:2.0,
media-uidwhtlta_b1128000_3205244.ts
#EXTINF:2.0,
media-uidwhtlta_b1128000_3205245.ts
#EXTINF:2.0,
media-uidwhtlta_b1128000_3205246.ts
", result);
        }
    }
}
