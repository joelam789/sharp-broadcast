
	function SimpleWebAudioStreamPlayer (options) {
	
		// normally we should share the context ...
		this.context = options.audioContext != undefined && options.audioContext != null ? options.audioContext : null;
		
		if (this.context == null) {
			window.AudioContext = window.AudioContext       ||
								  window.webkitAudioContext ||
								  window.mozAudioContext    ||
								  window.oAudioContext      ||
								  window.msAudioContext;

			if (window.AudioContext) this.context = new AudioContext();
		}
		
		this.useWorker = options.useWorker != undefined && options.useWorker != null ? options.useWorker : true;
		this.audioWorkerFile = options.workerFile != undefined && options.workerFile != null && options.workerFile.length > 0 
										? options.workerFile : "audio-stream-process.js";
		
		// set it > 0 if you need to make "delay" to sync video (when audio is faster)
		this.preloadedDataQueueSize = options.preloadedDataQueueSize != undefined && options.preloadedDataQueueSize != null 
										? options.preloadedDataQueueSize : 0;
		
		// for ScriptProcessorNode with no webworker
		this.audioDataBlockSize = options.audioDataBlockSize != undefined && options.audioDataBlockSize != null 
										? options.audioDataBlockSize : 1024;
		
		this.outputChannelCount = options.outputChannelCount != undefined && options.outputChannelCount != null 
										? options.outputChannelCount : 2;
		
		// the dummy objects should be used for IOS only ...
		this.dummyAudioBuffer = null;
		this.dummyAudioSource = null;
		this.isPlayingDummy = false;
		
		this.customNode = null;
		this.gainNode = this.context != null ? this.context.createGain() : null;
		if (this.gainNode) this.gainNode.connect(this.context.destination);
		
		this.workerNode = null;
		this.nodeWorker = null;
		
		this.enabled = true;
		this.needErrorLog = false;
		
		this.inputChannelCount = 2;
		this.audioCurrentRemains = null;
		this.audioPlayingBuffers = [];
		
		this.preloadedDataQueue = [];
		
		this.mediaInfo = "undefined";
		
		this.audioDataQueueSize = 128; // audio cache size
		
		this.clear = function() {
			this.preloadedDataQueue = [];
			this.audioPlayingBuffers = [];
			this.audioCurrentRemains = null;
		};
		
		this.volume = function(value) {
			if (!isNaN(value)) {
				if (this.gainNode != null) {
					if (this.gainNode.gain.setValueAtTime && this.context)
						this.gainNode.gain.setValueAtTime(value, this.context.currentTime);
					else this.gainNode.gain.value = value;
					return value;
				}
			} else {
				if (this.gainNode != null) return this.gainNode.gain.value;
			}
			return 0;
		};
		
		this.updateMediaInfo = function(infoText) {
			if (this.context == null) return;
			var posLeft = infoText.indexOf("(");
			if (posLeft > 0) this.mediaInfo = infoText.substring(0, posLeft);
			else this.mediaInfo = infoText;
		};
		
		this.bufferToArray = function(buffer) { // incoming data is an ArrayBuffer
			var u8arrayData = new Uint8Array(buffer);
			var outputArrayData = new Float32Array(u8arrayData.length);
			for (var i = 0; i < u8arrayData.length; i++) {
				outputArrayData[i] = (u8arrayData[i] - 128) / 128.0; // convert audio to float
			}
			return outputArrayData; // return the Float32Array
		};
		
		this.setupBuffers = function(intputBuffer, remainBuffer, outputBuffers, outputBufferSize) {

			var totalInput = new Float32Array(intputBuffer.length + remainBuffer.length);
			totalInput.set(intputBuffer);
			if (remainBuffer.length > 0) totalInput.set(remainBuffer, intputBuffer.length);
			
			var outputRemainSize = totalInput.length % outputBufferSize;
			var outputBufferCount = (totalInput.length - outputRemainSize) / outputBufferSize;
			
			var beginPos = 0;
			var totalCurrentInput = Array.prototype.slice.call(totalInput); // ios need this...
			for (var i=0; i<outputBufferCount; i++) {
				outputBuffers[outputBuffers.length] = totalCurrentInput.slice(beginPos, beginPos+outputBufferSize);
				beginPos = beginPos+outputBufferSize;
			}
			
			var finalRemain = null;
			if (outputRemainSize > 0) finalRemain = totalCurrentInput.slice(beginPos);
			else finalRemain = new Float32Array(0);
			
			return finalRemain;
		};
		
		this.prepareAudioBuffer = function(buffer, channelCount) {
			
			try {
				
				this.inputChannelCount = channelCount <= 0 ? buffer.numberOfChannels : channelCount;
		
				var newbuffer = [];

				if (this.audioCurrentRemains == null) {
					this.audioCurrentRemains = new Array();
					for (var i=0; i<this.inputChannelCount; i++) 
						this.audioCurrentRemains[this.audioCurrentRemains.length] = new Float32Array(0);
				}
				
				for (var i=0; i<this.inputChannelCount; i++) {
									
					var intputBuffer = channelCount <= 0 ? buffer.getChannelData(i) : buffer;
					
					var remainBuffer = this.audioCurrentRemains[i];
					var outputBuffers = new Array();
					
					var finalRemain = this.setupBuffers(intputBuffer, remainBuffer, outputBuffers, this.audioDataBlockSize);
					
					this.audioCurrentRemains[i] = finalRemain;
					newbuffer[newbuffer.length] = outputBuffers;
				}
				
				if (newbuffer.length > 0) {
					
					var audioBuffers = [];
					var foundEmpty = false;
					while (!foundEmpty) {
						var usefulbuffers = [];
						for (var i=0; i<this.inputChannelCount; i++) {
							if (newbuffer[i].length <= 0) {
								foundEmpty = true;
								break;
							}
							usefulbuffers[usefulbuffers.length] = newbuffer[i].shift();
						}
						if (usefulbuffers.length == this.inputChannelCount) 
							audioBuffers[audioBuffers.length] = usefulbuffers;
						
					}
					
					if (this.audioDataQueueSize > 0 && this.audioPlayingBuffers.length > this.audioDataQueueSize)
						this.audioPlayingBuffers.splice((this.audioPlayingBuffers.length - 1) / 2, 
															this.audioPlayingBuffers.length / 2);
					for (var nbidx=0; nbidx<audioBuffers.length; nbidx++) {
						this.audioPlayingBuffers[this.audioPlayingBuffers.length] = audioBuffers[nbidx];
					}
					
				}
			
			} catch (bufferingerror) {
				if (this.needErrorLog) console.log("audio buffering error: " + bufferingerror);
			}
		};
		
		this.decode = function(data) {
		
			if (this.workerNode == null || this.enabled !== true) return;
			
			if (this.preloadedDataQueueSize > 0) {
				this.preloadedDataQueue[this.preloadedDataQueue.length] = data;
				if (this.preloadedDataQueue.length <= this.preloadedDataQueueSize) return;
			}
			
			if (this.useWorker) {
				this.workerNode.postMessage( {streamData: this.preloadedDataQueue.length > 0 ? this.preloadedDataQueue.shift() : data, 
												mediaDesc: this.mediaInfo,
												maxQueueSize: this.audioDataQueueSize,
												errorLogFlag: this.needErrorLog ? 1 : 0} );
			} else {
				
				if (this.mediaInfo.indexOf('pcm') >= 0) {
					this.prepareAudioBuffer(this.bufferToArray(this.preloadedDataQueue.length > 0 ? this.preloadedDataQueue.shift() : data),
											this.inputChannelCount);
				} else {
					this.context.decodeAudioData(this.preloadedDataQueue.length > 0 ? this.preloadedDataQueue.shift() : data, function(buffer) {
						this.prepareAudioBuffer(buffer, 0);
					}.bind(this),
					function(err) {
						if (this.needErrorLog) console.log("decode audio error: " + err);
					}.bind(this));
				}
				
			}
		};
		
		this.open = function(extraNode) {
			
			try {
				
				if (this.context == undefined || this.context == null) throw "Web Audio Context not found";
				
				if (extraNode != undefined) this.customNode = extraNode;

				if (this.useWorker) {
					var gotoload = this.context.createAudioWorker(this.audioWorkerFile);
					gotoload.then(function(factory) {
						
						this.nodeWorker = factory;
						
						// we do not care about the input channels, so just let input get 2 channels by default
						this.workerNode = this.nodeWorker.createNode(2, this.outputChannelCount);

						if (this.customNode != null) {
							this.workerNode.connect(this.customNode);
							this.customNode.connect(this.gainNode);
						} else {
							this.workerNode.connect(this.gainNode);
						}
						
						//this.gainNode.connect(this.context.destination);

					}.bind(this));
				} else {
					// just let input get 2 channels, since we would use only output channels in function "onaudioprocess"
					this.workerNode = this.context.createScriptProcessor(this.audioDataBlockSize, 2, this.outputChannelCount);
					this.workerNode.onaudioprocess = this.processAudioData.bind(this);
					
					if (this.customNode != null) {
						this.workerNode.connect(this.customNode);
						this.customNode.connect(this.gainNode);
					} else {
						this.workerNode.connect(this.gainNode);
					}
					
					//this.gainNode.connect(this.context.destination);
				}
				
			} catch (e) {
				console.error(e);
				var errorMsg = "Web Audio API or Web Worker is not supported by this browser";
				throw new Error(errorMsg);
			}
		};
		
		this.processAudioData = function(e) {

			var bufferData = null;
			var bufferArray = this.audioPlayingBuffers;
			if (bufferArray != null && bufferArray.length > 0) bufferData = bufferArray.shift();

			var ichannels = [];
			var ochannels = [];
			var inbuf = e.inputBuffer;
			var outbuf = e.outputBuffer;
			var channelCount = outbuf.numberOfChannels;
			for (var cidx=0; cidx<channelCount; cidx++) {
				ichannels[ichannels.length] = inbuf.getChannelData(cidx);
				ochannels[ochannels.length] = outbuf.getChannelData(cidx);
			}
			
			if (bufferData == null) {
				for (var cidx1=0; cidx1<ochannels.length; cidx1++) {
					ochannels[cidx1].fill(0.0);
				}
				//console.log('GAP!');
				return;
			} else {
				var validAudioChannelCount = ochannels.length > bufferData.length ? bufferData.length : ochannels.length;
				for (var cidx2=0; cidx2<validAudioChannelCount; cidx2++) {
					ochannels[cidx2].set(bufferData[cidx2]);
				}
			}

		};
		
		this.playDummy = function() {
			this.isPlayingDummy = true;
			if (this.dummyAudioSource == null) {
				this.dummyAudioBuffer = this.context.createBuffer(2, 1, this.context.sampleRate);
				this.dummyAudioSource = this.context.createBufferSource();
				this.dummyAudioSource.buffer = this.dummyAudioBuffer;
				if (this.gainNode != null) this.dummyAudioSource.connect(this.gainNode);
				else this.dummyAudioSource.connect(this.context.destination);
				if (this.dummyAudioSource != null) {
					if (this.dummyAudioSource.start) this.dummyAudioSource.start();
					else this.dummyAudioSource.noteOn();
				}
			}
		};
		
		this.stopPlayingDummy = function() {
			if (this.dummyAudioSource != null) {
				if (this.dummyAudioSource.stop) this.dummyAudioSource.stop();
				else this.dummyAudioSource.noteOff();
				this.dummyAudioSource.disconnect();
				this.dummyAudioBuffer = null;
				this.dummyAudioSource = null;
			}
			this.isPlayingDummy = false;
		};
		
		this.close = function() {
			//this.stopPlayingDummy();
			if (this.nodeWorker != null) this.nodeWorker.terminate();
			if (this.workerNode != null) this.workerNode.disconnect();
			if (this.customNode != null) this.customNode.disconnect();
			//if (this.gainNode != null) this.gainNode.disconnect();
			this.clear();
		};
	}
