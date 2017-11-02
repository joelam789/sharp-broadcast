# sharp-broadcast
A low latency live streaming solution for html5

Features:

1. Support sub-second latency

2. Support 2 video formats: MPEG-1 (with jsmpeg\*) and H.264 (with broadway.js)

3. Support 5 audio formats: PCM(raw, u8, mono), MP3, AAC(ADTS), OGG and OPUS(OggOpus)

4. Support "ws" and "wss" connections

5. Support latest versions of Chrome/Firefox/Safari/Edge\*\* on both desktop and mobile platforms


\* jsmpeg supports both MPEG1 video and MP2 audio since v1.0

\*\* video (both MPEG-1 and H.264) could be played even on IE11


Known issues:

- There is some noise between audio chunks (it should be caused by resampling when decode audio data)
