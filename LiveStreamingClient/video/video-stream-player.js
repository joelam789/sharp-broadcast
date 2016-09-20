
	function SimpleWebVideoStreamPlayer (broadwayOptions) {
	
		// for the details of the options, see https://github.com/mbebenita/Broadway
		this.player = new Player(broadwayOptions);
		this.player.proxyPlayer = this;
		
		this.canvas = this.player.canvas;
		this.domNode = this.player.domNode;
		
		this.enabled = true;
		this.isFirstFrameComplete = false;
		
		this.frameInterval = 40; // 25fps by default
		this.renderTimer = null;
		
		this.streamDataQueue = [];
		this.streamDataQueueSize = 0; // set it > 0 if you need to make "delay" to sync audio (when video is faster)
		
		this.videoDataQueue = [];
		this.videoDataQueueSize = 8; // video cache size
		
		this.player.renderFrame = function(vdata) {
			this.proxyPlayer.videoDataQueue[this.proxyPlayer.videoDataQueue.length] = vdata;
			while (this.proxyPlayer.videoDataQueue.length > this.proxyPlayer.videoDataQueueSize)
				this.proxyPlayer.videoDataQueue.shift();
		};
		
		this.player.onRenderFrameComplete = function(vdata) {
			
			if (this.proxyPlayer.isFirstFrameComplete === false) {
				this.proxyPlayer.isFirstFrameComplete = true;
				if (this.proxyPlayer.onRenderFirstFrameComplete){
					this.proxyPlayer.onRenderFirstFrameComplete();
				}
			}
			
			if (this.proxyPlayer.onRenderFrameComplete){
				this.proxyPlayer.onRenderFrameComplete(vdata);
			}
		}
		
		this.decode = function(data) {
			
			if (this.enabled == false) return;
		
			if (this.streamDataQueueSize > 0) {
				this.streamDataQueue[this.streamDataQueue.length] = data;
				if (this.streamDataQueue.length <= this.streamDataQueueSize) return;
			}
			
			this.player.decode(Array.prototype.slice.apply(new Uint8Array(
								this.streamDataQueue.length > 0 ? this.streamDataQueue.shift() : data)));
		};
		
		this.updateFrameInterval = function(fps) {
			if (fps != undefined && fps != null) {
				if (typeof fps === "number") {
					if (fps > 0) this.frameInterval = 1000 / fps;
				} else if (typeof fps === "string" && fps.length > 0) {
					var vinfofps = 0;
					var vinfoparts = fps.split('@');
					vinfoparts = vinfoparts[0].split('x');
					if (vinfoparts.length >= 3) vinfofps = parseInt(vinfoparts[2]);
					else if (vinfoparts.length == 1) vinfofps = parseInt(vinfoparts[0]);
					if (vinfofps > 0) this.frameInterval = 1000 / vinfofps;
				}
			}
			if (this.renderTimer != null) {
				clearInterval(this.renderTimer);
				this.renderTimer = null;
			}
			console.log("video frame interval: " + this.frameInterval);
			this.isFirstFrameComplete = false;
			this.renderTimer = setInterval(this.renderFunc.bind(this), this.frameInterval);
		};
		
		this.renderFunc = function() {
			var vdataobj = this.videoDataQueue.shift();
			if (vdataobj != null) {
				if (this.player.webgl){
				  this.player.renderFrameWebGL(vdataobj);
				}else{
				  this.player.renderFrameRGB(vdataobj);
				};
			}
		};
	}
