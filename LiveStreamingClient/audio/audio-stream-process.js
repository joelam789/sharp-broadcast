
onnodecreate = function(e) {

	var node = e.node;
	node.buffers = [];
	
	//console.log(node);
	
	if (node.context == undefined || node.context == null) {
		if (node.__target__ != undefined && node.__target__ != null)
			node.context = node.__target__.context;
	}
	
	node.audioErrorLogFlag = 0;
	node.mediaInfo = "undefined";
	
	node.inputChannelCount = 2;
	node.audioCurrentRemains = null;
	
	node.audioDataQueueSize = 128; // audio cache size
	node.audioDataBlockSize = 128; // block-size is defined to be 128 sample-frames (see https://www.w3.org/TR/webaudio)
	
	node.bufferToArray = function(buffer) { // incoming data is an ArrayBuffer
		var u8arrayData = new Uint8Array(buffer);
		var outputArrayData = new Float32Array(u8arrayData.length);
		for (var i = 0; i < u8arrayData.length; i++) {
			outputArrayData[i] = (u8arrayData[i] - 128) / 128.0; // convert audio to float
		}
		return outputArrayData; // return the Float32Array
	};
	
	node.setupBuffers = function(intputBuffer, remainBuffer, outputBuffers, outputBufferSize) {

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
	
	node.prepareAudioBuffer = function(buffer, channelCount) {
		
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
				
				if (this.audioDataQueueSize > 0 && this.buffers.length > this.audioDataQueueSize)
					this.buffers.splice((this.buffers.length - 1) / 2, this.buffers.length / 2);
				for (var idx=0; idx<audioBuffers.length; idx++) {
					this.buffers[this.buffers.length] = audioBuffers[idx];
				}
				
			}
		
		} catch (bufferingerror) {
			if (this.audioErrorLogFlag > 0) console.log("audio buffering error: " + bufferingerror);
		}
	
	}
	
	node.onmessage = function(evt) {
		
		node.mediaInfo = evt.data.mediaDesc;
		node.audioErrorLogFlag = evt.data.errorLogFlag;
		node.audioDataQueueSize = evt.data.maxQueueSize;
		
		if (node.mediaInfo.indexOf('pcm') >= 0) {
			node.prepareAudioBuffer(node.bufferToArray(evt.data.streamData), node.inputChannelCount);
		} else {
			node.context.decodeAudioData(evt.data.streamData, function(buffer) {
				node.prepareAudioBuffer(buffer, 0);
			},
			function(err) {
				if (node.audioErrorLogFlag > 0) console.log("decode audio error: " + err);
			});
		}
	};
	
};

onaudioprocess = function(e) {

	var node = e.node;
	var bufferData = null;
	
	if (node.buffers != null && node.buffers.length > 0) bufferData = node.buffers.shift();

	if (bufferData == null) {
		for (var channel=0; channel<e.outputs[0].length; channel++) {
			e.outputs[0][channel].fill(0);
		}
		//console.log('GAP!');
		return;
	} else {
		var validAudioChannelCount = e.outputs[0].length > bufferData.length ? bufferData.length : e.outputs[0].length;
		for (var channel=0; channel<validAudioChannelCount; channel++) {
			e.outputs[0][channel].set(bufferData[channel]);
		}
	}
};
