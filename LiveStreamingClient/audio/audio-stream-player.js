
	function SimpleWebAudioStreamPlayer (options) {
	
		// normally we should share the context ...
		this.context = options.audioContext != undefined && options.audioContext != null ? options.audioContext : null;
		
		this.useWorker = options.useWorker != undefined && options.useWorker != null ? options.useWorker : true;
		this.audioWorkerFile = options.workerFile != undefined && options.workerFile != null && options.workerFile.length > 0 
										? options.workerFile : "audio-stream-process.js";
		
		// set it > 0 if you need to make "delay" to sync video (when audio is faster)
		this.streamDataQueueSize = options.streamDataQueueSize != undefined && options.streamDataQueueSize != null 
										? options.streamDataQueueSize : 0;
		
		// for ScriptProcessorNode with no webworker
		this.audioDataBlockSize = options.audioDataBlockSize != undefined && options.audioDataBlockSize != null 
										? options.audioDataBlockSize : 1024;
		
		// the dummy objects should be used for IOS only ...
		this.dummyAudioBuffer = null;
		this.dummyAudioSource = null;
		this.isPlayingDummy = false;
		
		this.workerNode = null;
		this.nodeWorker = null;
		
		this.enabled = true;
		this.needErrorLog = false;
		
		this.audioChannelCount = 2;
		this.audioCurrentRemains = null;
		this.audioPlayingBuffers = [];
		
		this.streamDataQueue = [];
		
		this.audioDataQueueSize = 128; // audio cache size
		
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
		
		this.decode = function(data) {
		
			if (this.workerNode == null || this.enabled !== true) return;
			
			if (this.streamDataQueueSize > 0) {
				this.streamDataQueue[streamDataQueue.length] = data;
				if (this.streamDataQueue.length <= this.streamDataQueueSize) return;
			}
			
			if (this.useWorker) {
				this.workerNode.postMessage( {streamData: this.streamDataQueue.length > 0 ? this.streamDataQueue.shift() : data, 
												maxQueueSize: this.audioDataQueueSize,
												errorLogFlag: this.needErrorLog ? 1 : 0} );
			} else {
				
				this.context.decodeAudioData(this.streamDataQueue.length > 0 ? this.streamDataQueue.shift() : data, function(buffer) {
					
					try {
					
						this.audioChannelCount = buffer.numberOfChannels;
				
						var newbuffer = [];

						if (this.audioCurrentRemains == null) {
							this.audioCurrentRemains = new Array();
							for (var i=0; i<this.audioChannelCount; i++) 
								this.audioCurrentRemains[this.audioCurrentRemains.length] = new Float32Array(0);
						}
						
						for (var i=0; i<this.audioChannelCount; i++) {
											
							var intputBuffer = buffer.getChannelData(i);
							
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
								for (var i=0; i<this.audioChannelCount; i++) {
									if (newbuffer[i].length <= 0) {
										foundEmpty = true;
										break;
									}
									usefulbuffers[usefulbuffers.length] = newbuffer[i].shift();
								}
								if (usefulbuffers.length == this.audioChannelCount) 
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
				}.bind(this),
				function(err) {
					if (this.needErrorLog) console.log("decode audio error: " + err);
				}.bind(this));
		
				
			}
		};
		
		this.open = function() {
			
			try {

				if (this.context == null) {
					window.AudioContext = window.AudioContext       ||
										  window.webkitAudioContext ||
										  window.mozAudioContext    ||
										  window.oAudioContext      ||
										  window.msAudioContext;

					this.context = new AudioContext();
				}

				if (this.useWorker) {
					var gotoload = this.context.createAudioWorker(this.audioWorkerFile);
					gotoload.then(function(factory) {
						this.nodeWorker = factory;
						this.workerNode = this.nodeWorker.createNode(2, 2);
						this.workerNode.connect(this.context.destination);
					}.bind(this));
				} else {
					this.workerNode = this.context.createScriptProcessor(this.audioDataBlockSize, 2, 2);
					this.workerNode.onaudioprocess = this.processAudioData.bind(this);
					this.workerNode.connect(this.context.destination);
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
				for (var cidx2=0; cidx2<ochannels.length; cidx2++) {
					ochannels[cidx2].set(bufferData[cidx2]);
				}
			}

		};
		
		this.playDummy = function() {
			this.isPlayingDummy = true;
			if (this.dummyAudioSource == null) {
				this.dummyAudioBuffer = this.context.createBuffer(2, 1, 44100);
				this.dummyAudioSource = this.context.createBufferSource();
				this.dummyAudioSource.buffer = this.dummyAudioBuffer;
				this.dummyAudioSource.connect(this.workerNode);
				if (this.dummyAudioSource != null) {
					if (this.dummyAudioSource.start) this.dummyAudioSource.start();
					else this.dummyAudioSource.noteOn();
				}
			}
		};
		
		this.close = function() {
			if (this.dummyAudioSource != null) {
				if (this.dummyAudioSource.stop) this.dummyAudioSource.stop();
				else this.dummyAudioSource.noteOff();
				this.dummyAudioSource.disconnect();
				this.dummyAudioBuffer = null;
				this.dummyAudioSource = null;
				this.isPlayingDummy = false;
			}
			if (this.nodeWorker != null) this.nodeWorker.terminate();
			if (this.workerNode != null) this.workerNode.disconnect();
		};
	}
