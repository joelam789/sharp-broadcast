
	function SimpleWebVideoStreamPlayer (broadwayOptions) {
	
		// for the details of the options, see https://github.com/mbebenita/Broadway
		this.player = new Player(broadwayOptions);
		this.player.proxyPlayer = this;
		
		this.initOptions = broadwayOptions;
		
		this.canvas = this.player.canvas;
		this.domNode = this.player.domNode;
		
		this.enabled = true;
		this.isFirstFrameComplete = false;
		
		this.incomingDataSizeQueue = [];
		this.currentFrameDataSize = 0;
		this.minKeyFrameDataSize = 0;
		
		this.feedingDetectionTimes = 0;
		this.hungryTimes = 0;
		this.networkSpeedScore = 0; // 0~100, small means lagging
		
		this.frameInterval = 40; // 25fps by default
		this.renderTimer = null;
		
		this.preloadedDataQueue = [];
		this.preloadedDataQueueSize = 0; // set it > 0 if you need to make "delay" to sync audio (when video is faster)
		
		this.videoDataQueue = [];
		this.videoDataQueueSize = 8; // video cache size
		
		this.width = 0;
		this.height = 0;
		
		this.playerRenderFrame = function(vdata) {
			this.proxyPlayer.videoDataQueue[this.proxyPlayer.videoDataQueue.length] = vdata;
			while (this.proxyPlayer.videoDataQueue.length > this.proxyPlayer.videoDataQueueSize) {
				this.proxyPlayer.videoDataQueue.shift();
				this.proxyPlayer.incomingDataSizeQueue.shift();
			}
		};
		
		this.playerOnRenderFrameComplete = function(vdata) {
			
			if (this.proxyPlayer.isFirstFrameComplete === false) {
				if (this.proxyPlayer.minKeyFrameDataSize <= 0
					|| this.proxyPlayer.currentFrameDataSize >= this.proxyPlayer.minKeyFrameDataSize) {
					this.proxyPlayer.isFirstFrameComplete = true;
					if (this.proxyPlayer.onRenderFirstFrameComplete){
						this.proxyPlayer.onRenderFirstFrameComplete();
					}
				}			
			}
			
			if (this.proxyPlayer.onRenderFrameComplete){
				this.proxyPlayer.onRenderFrameComplete(vdata);
			}
		};
		
		this.player.renderFrame = this.playerRenderFrame.bind(this.player);
		this.player.onRenderFrameComplete = this.playerOnRenderFrameComplete.bind(this.player);
		
		this.clear = function() {
			this.preloadedDataQueue = [];
			this.videoDataQueue = [];
			this.incomingDataSizeQueue = [];
		};
		
		this.reload = function(w, h) {
			
			if (this.player == null) return;
			
			var isactivenow = this.enabled;
			this.enabled = false;
			
			this.preloadedDataQueue = [];
			this.videoDataQueue = [];
			this.incomingDataSizeQueue = [];
			
			var currentpnode = null;
			
			if (this.player.worker != undefined && this.player.worker != null) this.player.worker.terminate();
			if (this.player.domNode != undefined && this.player.domNode != null && this.player.domNode.parentNode != null) {
				currentpnode = this.player.domNode.parentNode;
				currentpnode.removeChild(this.player.domNode);
			}
			
			if (!isNaN(w) && !isNaN(h)) {
				this.initOptions.size = {width: w, height: h};
				this.initOptions.visibleSize = {width: w, height: h};
			}
			
			this.player = new Player(this.initOptions);
			this.player.proxyPlayer = this;
			
			this.player.renderFrame = this.playerRenderFrame.bind(this.player);
			this.player.onRenderFrameComplete = this.playerOnRenderFrameComplete.bind(this.player);
			
			this.networkSpeedScore = 0;
			this.hungryTimes = 0;
			this.feedingDetectionTimes = 0;
			this.isFirstFrameComplete = false;
			
			this.canvas = this.player.canvas;
			this.domNode = this.player.domNode;
			
			this.width = this.canvas.width;
			this.height = this.canvas.height;
			
			if (currentpnode != null) {
				if (currentpnode.style.width) this.domNode.style.width = "100%";
				if (currentpnode.style.height) this.domNode.style.height = "100%";
				currentpnode.appendChild(this.domNode);
				//currentpnode.style.overflow = "hidden";
				//currentpnode.style.width = this.videoWidth + "px";
				//currentpnode.style.height = this.videoHeight + "px";
			}
			
			this.enabled = isactivenow;
			
		};
		
		
		this.decode = function(data) {
			
			if (this.enabled == false) return;
		
			if (this.preloadedDataQueueSize > 0) {
				this.preloadedDataQueue[this.preloadedDataQueue.length] = data;
				if (this.preloadedDataQueue.length <= this.preloadedDataQueueSize) return;
			}
			
			var processingVideoData = this.preloadedDataQueue.length > 0 ? this.preloadedDataQueue.shift() : data;
			this.incomingDataSizeQueue[this.incomingDataSizeQueue.length] = processingVideoData.byteLength;

			this.player.decode(Array.prototype.slice.apply(new Uint8Array(processingVideoData)));
		};
		
		this.updateFrameInterval = function(fps) {
			if (isNaN(fps)) return;
			if (fps <= 0) return;
			this.frameInterval = 1000 / fps;
			if (this.renderTimer != null) {
				clearInterval(this.renderTimer);
				this.renderTimer = null;
			}
			console.log("video frame interval: " + this.frameInterval);
			this.hungryTimes = 0;
			this.feedingDetectionTimes = 0;
			this.isFirstFrameComplete = false;
			this.renderTimer = setInterval(this.renderFunc.bind(this), this.frameInterval);
		};
		
		this.updateMediaInfo = function(mediaInfo) {
			var videoInfo = mediaInfo + "";
			var posLeft = videoInfo.indexOf("(");
			var posRight = videoInfo.indexOf(")");
			if (posLeft >= 0 && posRight > posLeft)
				videoInfo = videoInfo.substring(posLeft+1, posRight);
			
			if (videoInfo.length <= 0) return;
			
			var vinfow = 0;
			var vinfoh = 0;
			var vinfofps = 0;
			var vinfoparts = videoInfo.split('@');
			vinfoparts = vinfoparts[0].split('x');
			if (vinfoparts.length >= 3) {
				vinfow = parseInt(vinfoparts[0]);
				vinfoh = parseInt(vinfoparts[1]);
				vinfofps = parseInt(vinfoparts[2]);
			} else if (vinfoparts.length == 1) vinfofps = parseInt(vinfoparts[0]);

			if (vinfofps > 0) this.updateFrameInterval(vinfofps);
			if (vinfow > 0 && vinfoh > 0) this.reload(vinfow, vinfoh);
			else this.reload();
		};
		
		this.renderFunc = function() {
			
			this.feedingDetectionTimes++;
			
			var vdataobj = this.videoDataQueue.shift();
			var vdatasize = this.incomingDataSizeQueue.shift();
			
			if (vdataobj == null) this.hungryTimes++;
			
			if (this.feedingDetectionTimes * this.frameInterval >= 1000) { // check it every 1 second
				this.networkSpeedScore = Math.round((this.feedingDetectionTimes - this.hungryTimes)/(this.feedingDetectionTimes + 0.01) * 100);
				this.feedingDetectionTimes = 0;
				this.hungryTimes = 0;
			}
			
			if (vdataobj == null) return;
			
			if (this.isFirstFrameComplete === false 
				&& this.minKeyFrameDataSize > 0 
				&& this.minKeyFrameDataSize > vdatasize) return;
			
			this.currentFrameDataSize = vdatasize;
			
			if (this.player.webgl){
			  this.player.renderFrameWebGL(vdataobj);
			}else{
			  this.player.renderFrameRGB(vdataobj);
			};
		};
	}
