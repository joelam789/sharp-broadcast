
	function SimpleWebSocketStreamPlayer (videoOptions, audioOptions) {
	
		this.video = (videoOptions == null) ? null : new SimpleWebVideoStreamPlayer(videoOptions);
		this.audio = (audioOptions == undefined || audioOptions == null) ? null : new SimpleWebAudioStreamPlayer(audioOptions);
		
		this.url = "";
		
		this.mediaInfo = "";
		this.videoInfo = "";
		this.audioInfo = "";
		
		this.socket = null;
		
		this.audioFilter = null;
		this.videoContainer = null;

		this.pendingUrl = "";
		this.isOpening = false;
		this.isClosing = false;
		this.pendingClosing = false;
		
		this.volume = function(value) {
			if (this.audio != null) {
				return this.audio.volume(value);
			}
			return 0;
		};
		
		this.close = function() {
			if (this.video != null) this.video.enabled = false;
			if (this.audio != null) this.audio.enabled = false;
			if (this.video != null) this.video.clear();
			if (this.audio != null) this.audio.clear();
			this.pendingUrl = "";
			if (this.socket != null) {
				if (!this.isOpening && !this.isClosing) {
					this.pendingClosing = false;
					this.isClosing = true;
					this.socket.close();
				} else if (this.isOpening) {
					this.pendingClosing = true;
				} else if (this.isClosing) {
					this.pendingClosing = false;
				}
				//this.socket = null;
			}
		};
		
		this.open = function(url) {
			
			this.close();

			if (this.isOpening || this.isClosing) {
				this.pendingUrl = url;
				return;
			}
			
			this.url = url;
			this.pendingUrl = "";
			this.isOpening = true;
			this.socket = new WebSocket(url);
			this.socket.binaryType = "arraybuffer";
			this.socket.onmessage = function(event) {
				var data = event.data;
				if (typeof data != "string") {
					var binArr = new Uint8Array(data);
					if (binArr[0] == 0 && binArr[1] == 0 && binArr[2] == 0 && binArr[3] == 1) {
						if (this.video != null) this.video.decode(data);
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
					if (this.video != null && this.video.domNode != null && this.video.domNode.parentNode != null) {
						this.video.updateMediaInfo(vinfopart);
					}
					this.audioInfo = ainfopart;
					console.log("audio info: " + ainfopart);
					if (this.audio != null) this.audio.updateMediaInfo(ainfopart);
					if (this.onMediaChange != null) this.onMediaChange();
				}
			}.bind(this);
			this.socket.onopen = function() {
				this.isOpening = false;
				if (this.pendingClosing) {
					this.pendingClosing = false;
					this.isClosing = true;
					this.socket.close();
					return;
				}
				if (this.audio != null) this.audio.open(this.audioFilter);
				if (this.video != null && this.video.domNode != null && this.videoContainer != null)
					this.videoContainer.appendChild(this.video.domNode);
				if (this.onOpen != undefined && this.onOpen != null) this.onOpen();
				if (this.video == null) {
					if (this.audio != null) this.audio.enabled = true;
					if (this.onPlay != undefined && this.onPlay != null) this.onPlay();
				}
			}.bind(this);
			this.socket.onclose = function() {
				this.isClosing = false;
				if (this.audio != null) this.audio.close();
				if (this.onClose != undefined && this.onClose != null) this.onClose();
				this.socket = null;
				if (this.pendingUrl != undefined && this.pendingUrl != null && this.pendingUrl.length > 0) {
					var nextTargetStreamUrl = this.pendingUrl;
					this.pendingUrl = "";
					this.open(nextTargetStreamUrl);
				}
			}.bind(this);
			this.socket.onerror = function(evt) {
				console.error("Error of web-socket in stream player: ", evt);
				if (this.video != null) this.video.enabled = false;
				if (this.audio != null) this.audio.enabled = false;
				if (this.video != null) this.video.clear();
				if (this.audio != null) this.audio.clear();
				this.pendingUrl = "";
				this.isOpening = false;
				this.isClosing = false;
				this.pendingClosing = false;
				if (this.onError != undefined && this.onError != null) this.onError(evt);
			  }.bind(this);
			
			if (this.video != null) this.video.enabled = true;
		};
		
		this.disable = function() {
			if (this.socket == null || this.socket.readyState != 1) return;
			if (this.video != null) this.video.enabled = false;
			if (this.audio != null) this.audio.enabled = false;
		};
		
		this.enable = function() {
			if (this.socket == null || this.socket.readyState != 1) return;
			if (this.video != null) this.video.enabled = true;
			if (this.audio != null) this.audio.enabled = true;
		};
		
		this.isPlaying = function() {
			if (this.socket == null || this.socket.readyState != 1) return false;
			if (this.video != null && this.video.enabled) return true;
			if (this.audio != null && this.audio.enabled) return true;
			return false;
		};
		
		this.playDummySound = function() {
			if (this.audio != null) {
				if (!this.audio.isPlayingDummy) this.audio.playDummy();
			}
		};
		
		this.stopPlayingDummySound = function() {
			if (this.audio != null) {
				if (this.audio.isPlayingDummy) this.audio.stopPlayingDummy();
			}
		};
		
		this.getVideoStreamSpeedScore = function() {
			return this.isPlaying() && this.video != null && this.video.enabled ? this.video.networkSpeedScore : 0;
		};
		
		if (this.video != null) this.video.onRenderFirstFrameComplete = function () {
			if (this.audio != null) this.audio.enabled = true;
			if (this.onPlay != undefined && this.onPlay != null) this.onPlay();
		}.bind(this);
		
	}
