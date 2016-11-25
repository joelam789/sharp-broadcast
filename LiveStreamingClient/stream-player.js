
	function SimpleWebSocketStreamPlayer (videoOptions, audioOptions) {
	
		this.video = new SimpleWebVideoStreamPlayer(videoOptions);
		this.audio = (audioOptions == undefined || audioOptions == null) ? null : new SimpleWebAudioStreamPlayer(audioOptions);
		
		this.url = "";
		
		this.mediaInfo = "";
		this.videoInfo = "";
		this.audioInfo = "";
		
		this.socket = null;
		
		this.audioFilter = null;
		this.videoContainer = null;
		
		this.volume = function(value) {
			if (this.audio != null) {
				return this.audio.volume(value);
			}
			return 0;
		};
		
		this.close = function() {
			this.video.enabled = false;
			if (this.audio != null) this.audio.enabled = false;
			if (this.socket != null) {
				this.socket.close()
				this.socket = null;
			}
			this.video.clear();
			if (this.audio != null) this.audio.clear();
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
						this.video.decode(data);
					} else {
						if (this.audio != null) this.audio.decode(data);
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
					if (this.video.domNode != null && this.video.domNode.parentNode != null) {
						this.video.updateMediaInfo(vinfopart);
					}
					this.audioInfo = ainfopart;
					console.log("audio info: " + ainfopart);
					if (this.audio != null) this.audio.updateMediaInfo(ainfopart);
					if (this.onMediaChange != null) this.onMediaChange();
				}
			}.bind(this);
			this.socket.onopen = function() {
				if (this.audio != null) this.audio.open(this.audioFilter);
				if (this.videoContainer != null) this.videoContainer.appendChild(this.video.domNode);
				if (this.onOpen != undefined && this.onOpen != null) this.onOpen();
			}.bind(this);
			this.socket.onclose = function() {
				if (this.audio != null) this.audio.close();
				if (this.onClose != undefined && this.onClose != null) this.onClose();
			}.bind(this);
			
			this.video.enabled = true;
		};
		
		this.disable = function() {
			if (this.socket == null || this.socket.readyState != 1) return;
			this.video.enabled = false;
			if (this.audio != null) this.audio.enabled = false;
		};
		
		this.enable = function() {
			if (this.socket == null || this.socket.readyState != 1) return;
			this.video.enabled = true;
			if (this.audio != null) this.audio.enabled = true;
		};
		
		this.isPlaying = function() {
			if (this.socket == null || this.socket.readyState != 1) return false;
			if (this.video.enabled) return true;
			if (this.audio != null && this.audio.enabled) return true;
			return false;
		};
		
		this.playDummySound = function() {
			if (this.audio != null) {
				if (!this.audio.isPlayingDummy) this.audio.playDummy();
			}
		};
		
		this.getVideoStreamSpeedScore = function() {
			return this.isPlaying() && this.video.enabled ? this.video.networkSpeedScore : 0;
		};
		
		this.video.onRenderFirstFrameComplete = function () {
			if (this.audio != null) this.audio.enabled = true;
			if (this.onPlay != undefined && this.onPlay != null) this.onPlay();
		}.bind(this);
		
	}
