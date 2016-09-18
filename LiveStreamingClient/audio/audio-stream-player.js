
	function SimpleWebAudioStreamPlayer (audioctx) {
	
		// normally we should share the context ...
		this.context = audioctx != undefined && audioctx != null ? audioctx : null;
		
		this.dummyAudioBuffer = null;
		this.dummyAudioSource = null;
		this.isPlayingDummy = false;
		
		this.workerNode = null;
		this.nodeWorker = null;
		
		this.enabled = true;
		this.needErrorLog = false;
		
		this.streamDataQueue = [];
		this.streamDataQueueSize = 0;
		
		this.audioDataQueueSize = 256;
		
		this.decode = function(data) {
		
			if (this.workerNode == null || this.enabled !== true) return;
			
			if (this.streamDataQueueSize > 0) {
				this.streamDataQueue.push(data);
				if (this.streamDataQueue.length <= this.streamDataQueueSize) return;
			}
			
			this.workerNode.postMessage( {streamData: this.streamDataQueue.length > 0 ? this.streamDataQueue.shift() : data, 
											maxQueueSize: this.audioDataQueueSize,
											errorLogFlag: this.needErrorLog ? 1 : 0} );
		};
		
		this.open = function(workerScriptFile, inputQueueLen) {
		
			var audioWorkerSourceFile = workerScriptFile != undefined 
										&& workerScriptFile != null 
										&& workerScriptFile.length > 0 
										? workerScriptFile : "audio-stream-process.js";
			
			try {

				if (this.context == null) {
					window.AudioContext = window.AudioContext       ||
										  window.webkitAudioContext ||
										  window.mozAudioContext    ||
										  window.oAudioContext      ||
										  window.msAudioContext;

					this.context = new AudioContext();
				}
				
				if (inputQueueLen != undefined && inputQueueLen != null && !isNaN(inputQueueLen)) this.streamDataQueueSize = inputQueueLen;

				var gotoload = this.context.createAudioWorker(audioWorkerSourceFile);
				gotoload.then(function(factory) {
					this.nodeWorker = factory;
					this.workerNode = this.nodeWorker.createNode(1, 1);
					this.workerNode.connect(this.context.destination);
				}.bind(this));
				
			} catch (e) {
				console.error(e);
				var errorMsg = "Web Audio API or Web Worker is not supported by this browser";
				throw new Error(errorMsg);
			}
		};
		
		this.playDummy = function() {
			if (this.dummyAudioSource == null) {
				this.dummyAudioBuffer = this.context.createBuffer(1, 1, 44100);;
				this.dummyAudioSource = this.context.createBufferSource();
				this.dummyAudioSource.buffer = this.dummyAudioBuffer;				
				this.dummyAudioSource.connect(this.workerNode);
				if (this.dummyAudioSource != null) {
					if (this.dummyAudioSource.start) this.dummyAudioSource.start();
					else this.dummyAudioSource.noteOn();
					this.isPlayingDummy = true;
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
