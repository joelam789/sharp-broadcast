
	function SimpleWebSocketStreamPlayer (videoOptions, audioOptions) {
	
		this.videoPlayer = new SimpleWebVideoStreamPlayer(videoOptions);
		this.audioPlayer = (audioOptions == undefined || audioOptions == null) ? null : new SimpleWebAudioStreamPlayer(audioOptions);
		
		this.url = "";
		
		this.mediaInfo = "";
		this.videoInfo = "";
		this.audioInfo = "";
		
		this.socket = null;
		
		this.audioFilter = null;
		this.videoContainer = null;
		
		this.close = function() {
			this.videoPlayer.enabled = false;
			if (this.audioPlayer != null) this.audioPlayer.enabled = false;
			if (this.socket != null) {
				this.socket.close()
				this.socket = null;
			}
			this.videoPlayer.clear();
			if (this.audioPlayer != null) this.audioPlayer.clear();
		};
		
		this.open = function(url) {
			
			this.close();
			
			this.url = url;
			this.socket = new WebSocket(url);
			this.socket.binaryType = "arraybuffer";
			this.socket.onmessage = function(event) {
				var data = event.data;
				if (typeof data != "string") {
					var binArr = new Uint8Array(data);
					if (binArr[0] == 0 && binArr[1] == 0 && binArr[2] == 0 && binArr[3] == 1) {
						this.videoPlayer.decode(data);
					} else {
						if (this.audioPlayer != null) this.audioPlayer.decode(data);
					}
				} else {
					this.mediaInfo = data;
					console.log("media info: " + data);
					var mixinfoparts = data.split('|');
					var vinfopart = mixinfoparts[0];
					var ainfopart = mixinfoparts.length > 1 ? mixinfoparts[1] : "";
					if (vinfopart.indexOf("h264") < 0) {
						vinfopart = mixinfoparts.length > 1 ? mixinfoparts[1] : "";
						ainfopart = mixinfoparts[0];
					}
					this.videoInfo = vinfopart;
					console.log("video info: " + vinfopart);
					if (this.videoPlayer.domNode != null && this.videoPlayer.domNode.parentNode != null) {
						this.videoPlayer.updateMediaInfo(vinfopart);
					}
					this.audioInfo = ainfopart;
					console.log("audio info: " + ainfopart);
					if (this.audioPlayer != null) this.audioPlayer.updateMediaInfo(ainfopart);
				}
			}.bind(this);
			this.socket.onopen = function() {
				if (this.audioPlayer != null) this.audioPlayer.open(this.audioFilter);
				if (this.videoContainer != null) this.videoContainer.appendChild(this.videoPlayer.domNode);
				if (this.onOpen != undefined && this.onOpen != null) this.onOpen();
			}.bind(this);
			this.socket.onclose = function() {
				if (this.audioPlayer != null) this.audioPlayer.close();
				if (this.onClose != undefined && this.onClose != null) this.onClose();
			}.bind(this);
			
			this.videoPlayer.enabled = true;
		};
		
		this.pause = function() {
			if (this.socket == null || this.socket.readyState != 1) return;
			this.videoPlayer.enabled = false;
			if (this.audioPlayer != null) this.audioPlayer.enabled = false;
		};
		
		this.resume = function() {
			if (this.socket == null || this.socket.readyState != 1) return;
			this.videoPlayer.enabled = true;
			if (this.audioPlayer != null) this.audioPlayer.enabled = true;
		};
		
		this.isPlaying = function() {
			if (this.socket == null || this.socket.readyState != 1) return false;
			if (this.videoPlayer.enabled) return true;
			if (this.audioPlayer != null && this.audioPlayer.enabled) return true;
			return false;
		};
		
		this.playDummySound = function() {
			if (this.audioPlayer != null) {
				if (!this.audioPlayer.isPlayingDummy) this.audioPlayer.playDummy();
			}
		};
		
		this.videoPlayer.onRenderFirstFrameComplete = function () {
			if (this.audioPlayer != null) this.audioPlayer.enabled = true;
			if (this.onPlay != undefined && this.onPlay != null) this.onPlay();
		}.bind(this);
		
	}
