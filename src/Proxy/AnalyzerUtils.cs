using System;
using System.IO;

namespace Proxy
{
    /// <summary>
    /// Analyzer utility class
    /// </summary>
    public static class AnalyzerUtils
    {
        /// <summary>
        /// Get the Request Type from an URL. No URL validation is done,
        /// Method is only based on file extension (suffix)
        /// </summary>
        /// <param name="url">An URL</param>
        /// <returns>The Request type</returns>
        public static RequestType GetRequestType(string url)
        {
            if (url.EndsWith(".ts"))
            {
                return RequestType.SEGMENT;
            }
            else if (url.EndsWith(".m3u8") || url.EndsWith(".m3u"))
            {
                return RequestType.MANIFEST;
            }
            else
            {
                return RequestType.UNKNOWN;
            }
        }

        /// <summary>
        /// Patch a manifest by changing absolute resources URLs to relative (to the manifest itself)
        /// </summary>
        /// <param name="manifestUri">The Manifest URI</param>
        /// <param name="content">The Manifest content</param>
        /// <returns>A patched content (or null if content does not need to be patched)</returns>
        public static string PatchToRelative(Uri manifestUri, string content)
        {
            var authority = manifestUri.GetLeftPart(UriPartial.Authority);
            bool isPatched = false;

            var replace = new StringWriter();
            using (var reader = new StringReader(content))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    // basic validation of the line beginning and end
                    if (line.StartsWith(authority) && GetRequestType(line) != RequestType.UNKNOWN)
                    {
                        try
                        {
                            // All the magic is in this standard library method
                            var relative = manifestUri.MakeRelativeUri(new System.Uri(line)).ToString();
                            line = relative;
                            isPatched = true;
                        }
                        catch (System.Exception)
                        {
                            // Pass, it was not possible to convert the URI to a relative, either
                            // because of format or not relative to the request
                        }
                    }
                    // line can be the original or the patched one, in any case write
                    replace.WriteLine(line);
                }
                return isPatched ? replace.ToString() : null;
            }
        }
    }
}