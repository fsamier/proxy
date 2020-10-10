namespace Proxy
{
    /// <summary>
    /// Enumerator for Proxy flow. IN represents messages from VLC to Proxy, 
    /// and OUT from Proxy (after CDN forwarding) to VLC.
    /// </summary>
    public enum Flow
    {
        IN,
        OUT
    }

    /// <summary>
    /// Type of Request in Proxy.
    /// </summary>
    public enum RequestType
    {
        MANIFEST,
        SEGMENT,
        UNKNOWN,
    }
}