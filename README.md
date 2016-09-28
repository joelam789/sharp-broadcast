# sharp-broadcast
A low latency live streaming solution for html5

Features:

1. Support sub-second latency

2. Support 2 video formats: MPEG-1 (with jsmpeg) and H.264 (with broadway.js)

3. Support 3 audio formats: MP3, AAC(ADTS) and OPUS(OggOpus)

4. Support "ws" and "wss" connections

5. Support latest versions of Chrome/Firefox/Safari/Edge on both desktop and mobile platforms

Known issues

- There is some noise between audio chunks (It should be caused by resampling when decode audio data)
- Only Firefox does not support AAC(ADTS)
- Only Firefox supports OggOpus
