# Proxy for VLC
## Installation
- Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download)
## Run
To run the proxy, type in root directory (the one containing `Proxy.sln` file)

`dotnet run --project src/Proxy -- --forward <url>`

where `<url>` is the base URL to capture.
Then open in VLC `http://localhost:5000/path/to/manifest.m3u8` (use port `5001` for a secure connection)

For instance,

```
> dotnet run --project src/Proxy --forward https://bitdash-a.akamaihd.net/

> vlc https://localhost:5001/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8
```

Logs should look like the following on the console:
```
[15:11:20 INF] [IN][MANIFEST] https://localhost:5001/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa_video_720_2400000.m3u8
[15:11:20 INF] [OUT][MANIFEST] https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa_video_720_2400000.m3u8 (16 ms)
[15:11:20 INF] [IN][SEGMENT] https://localhost:5001/content/MI201109210084_1/video/720_2400000/hls/segment_0.ts
[15:11:21 INF] [OUT][SEGMENT] https://bitdash-a.akamaihd.net/content/MI201109210084_1/video/720_2400000/hls/segment_0.ts (318 ms)
[15:11:21 INF] [IN][SEGMENT] https://localhost:5001/content/MI201109210084_1/audio/1_stereo_128000/hls/segment_0.ts
[15:11:21 INF] [OUT][SEGMENT] https://bitdash-a.akamaihd.net/content/MI201109210084_1/audio/1_stereo_128000/hls/segment_0.ts (280 ms)
[15:11:22 INF] [TRACK SWITCH]
[15:11:22 INF] [IN][MANIFEST] https://localhost:5001/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa_video_1080_4800000.m3u8
```

## Test
Some unit-tests are present, run them with `dotnet test` command.