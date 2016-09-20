
onnodecreate = function(e) {

	var node = e.node;
	node.buffers = [];
	
	//console.log(node);
	
	if (node.context == undefined || node.context == null) {
		if (node.__target__ != undefined && node.__target__ != null)
			node.context = node.__target__.context;
	}
	
	node.audioErrorLogFlag = 0;
	
	node.audioChannelCount = 2;
	node.audioCurrentRemains = null;
	
	node.audioDataQueueSize = 128; // audio cache size
	node.audioDataBlockSize = 128; // block-size is defined to be 128 sample-frames (see https://www.w3.org/TR/webaudio)
	
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
	
	node.onmessage = function(evt) {
		
		node.audioErrorLogFlag = evt.data.errorLogFlag;
		node.audioDataQueueSize = evt.data.maxQueueSize;
		node.context.decodeAudioData(evt.data.streamData, function(buffer) {
			
			try {
			
				node.audioChannelCount = buffer.numberOfChannels;
		
				var newbuffer = [];

				if (node.audioCurrentRemains == null) {
					node.audioCurrentRemains = new Array();
					for (var i=0; i<node.audioChannelCount; i++) 
						node.audioCurrentRemains[node.audioCurrentRemains.length] = new Float32Array(0);
				}
				
				for (var i=0; i<node.audioChannelCount; i++) {
									
					var intputBuffer = buffer.getChannelData(i);
					
					var remainBuffer = node.audioCurrentRemains[i];
					var outputBuffers = new Array();
					
					var finalRemain = node.setupBuffers(intputBuffer, remainBuffer, outputBuffers, node.audioDataBlockSize);
					
					node.audioCurrentRemains[i] = finalRemain;
					newbuffer[newbuffer.length] = outputBuffers;
				}
				
				if (newbuffer.length > 0) {
					
					var audioBuffers = [];
					var foundEmpty = false;
					while (!foundEmpty) {
						var usefulbuffers = [];
						for (var i=0; i<node.audioChannelCount; i++) {
							if (newbuffer[i].length <= 0) {
								foundEmpty = true;
								break;
							}
							usefulbuffers[usefulbuffers.length] = newbuffer[i].shift();
						}
						if (usefulbuffers.length == node.audioChannelCount) 
							audioBuffers[audioBuffers.length] = usefulbuffers;
						
					}
					
					if (node.audioDataQueueSize > 0 && node.buffers.length > node.audioDataQueueSize)
						node.buffers.splice((node.buffers.length - 1) / 2, node.buffers.length / 2);
					for (var idx=0; idx<audioBuffers.length; idx++) {
						node.buffers[node.buffers.length] = audioBuffers[idx];
					}
					
				}
			
			} catch (bufferingerror) {
				if (node.audioErrorLogFlag > 0) console.log("audio buffering error: " + bufferingerror);
			}
		},
		function(err) {
			if (node.audioErrorLogFlag > 0) console.log("decode audio error: " + err);
		});
		
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
		for (var channel=0; channel<e.outputs[0].length; channel++) {
			e.outputs[0][channel].set(bufferData[channel]);
		}
	}
};
