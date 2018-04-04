
var currentAnimations = [];
var resumeAnimationList = [];

//should be this but nans code is bugged...
//function submitNewTransaction(nodeId, transactionId, pc){
function submitNewTransaction(nodeId, transactionId, pc, path) {

	/*var graph = makeAGraph();
	console.log("IN javascript file");
	console.log("Node: " + nodeId + ", transaction: " + transactionId + ", pc: " + pc);
	console.log(graph);*/

	//var result = shortestPath(graph, nodeId, pc);
	//console.log("Nans path");
	//console.log(result);
	//if (result.err != null) {
	//	alert("No path available");
	//}

	console.log("=========Sending new transaction: " + transactionId + "===========");
	console.log(path);
	var transObj = TransactionList[transactionId + ""];
	
	transObj.path = path;
	console.log(transObj);
	SendTransaction(transactionId, transObj, nodeId);
	console.log("================================");
}


//This function initiates the sending of a Transaction
function SendTransaction(transactionId, transObj, storeIp) {

	SendToNode(storeIp, 0, transactionId, transObj, null);
}

//this function handles the physical sending of a transaction to a node
function SendToNode(currentIp, pathIndex, transactionId, transObj, lastIp) {
	var result = CheckIfQueueIsFull(currentIp);
	//base case, the queue is full and the transaction is dropped
	if (result) {
		alert("Dropped transaction, queue is full");
		return;
	}
	//Alternate case, the element is actually inactive, so the transaction should be dropped


	//Change state, add to queue...
	AddTransactionToQueue(transactionId, currentIp);

	//Continue Case, this element is the only thing on the queue and if the current element is not being animated, and animation is runnign.
	var isHead = GetHeadOfQueue(currentIp) == transactionId;
	var isAnimated = IsElementAnimated(currentIp);
	console.log("result: " + isHead + ", animated?: " + isAnimated);
	if (isHead  && !isAnimated ) { //still check the next To-Queue if it's open??? can it take it?
		console.log("animate the item");
		AnimateItem(transactionId, currentIp, transObj, pathIndex);
	}
}

function WakeUpNodeHead(nodeIp) {
	var node = cy.getElementById(nodeIp);
	var head = GetHeadOfQueue(nodeIp);
	console.log("===============Wake Up ===========");
	console.log(node);
	//if there is nothing on the queue, then do nothing and return.
	if (head == null) {
		return;
	}
	//else, animate the first thing.
	else {
		var transObj = TransactionList[head + ""];
		console.log("===============TransactionObject on wake up ===========");
		console.log(transObj);
		console.log("=================doe with wakeup, sending to animation=======");
		AnimateItem(head, nodeIp, transObj, transObj["curIndex"]);

	}
}

function AnimateItem(transId, currentIp, transObj, pathIndex) {
	if (!isAnimated) {
		AddToResumeAnimationList(transId, currentIp, transObj, pathIndex);
		return;
	}

	var ann = cy.getElementById(currentIp).animation({
		style: {
			'background-color': 'white',
			'line-color': 'white',
			'width': 50,
			'height': 50
		},
		duration: 1000
	});

	currentAnimations.push(ann);

	ann.play().promise('completed').then(function () {
		ann.reverse().rewind().play().promise('completed').then(function () {
			currentAnimations.pop();
			TransactionList[transId + ""]["curIndex"]++;//transObj["curIndex"]++;
			pathIndex++;
			var nextIp = transObj.path[pathIndex];
			SendToNode(nextIp, pathIndex, transId, transObj, pathIndex, currentIp);
			RemoveTransactionFromQueue(transId, currentIp);
			WakeUpNodeHead(currentIp);
		});
	});
}


//adds an object to the list of animations to wake up upon pressing the play button.
function AddToResumeAnimationList(transId, currentIp, transObj, pathIndex) {
	var obj = [transId, currentIp, transObj, pathIndex];
	resumeAnimationList.push(obj);
}

function ResumeFromAnimationList() {
	console.log("==========================RESUMING =====================");
	console.log(resumeAnimationList);
	for (var i in resumeAnimationList) {
		console.log(resumeAnimationList[i][0] );
		AnimateItem(resumeAnimationList[i][0], resumeAnimationList[i][1], resumeAnimationList[i][2], resumeAnimationList[i][3],)
	}
	resumeAnimationList = [];
}

//removes the leement from the queue
function RemoveTransactionFromQueue(transId, currentIp) {
	console.log("-------------Remove FROM QUEUE------------" + currentIp);
	var removed = false;
	var node = cy.getElementById(currentIp);
	var queue = node._private.data.queue;
	var tempQueue = [];
	for (var i in queue) {
		if (!removed) {
			if (queue[i] != transId) {
				tempQueue.push(queue[i]);
			}
			else {
				removed = true;
			}
		}
		else {
			tempQueue.push(queue[i]);
		}
	}
	//console.log(tempQueue);
	//console.log("node change queue");
	node.data('queue', tempQueue);
	console.log("-------------REMOVED------------");
	//console.log(node);
}

//adds the element to the queue
function AddTransactionToQueue(transId, nodeIp) {
	var node = cy.getElementById(nodeIp);
	var queue = node._private.data.queue;
	console.log("-------------ADDING TO QUEUE------------" + nodeIp);
	var tempQueue = [];
	for (var i in queue) {
		console.log("EXISTING QUEUE");
		console.log(queue);
		tempQueue.push(queue[i]); //was tempQueue.push(i);
	}
	tempQueue.push(transId);
	//console.log(tempQueue);
	//console.log("node change queue");
	node.data('queue', tempQueue);
	console.log("-------------DONE------------");
	//console.log(node);
}
//checks if the current element (ip) is being animated
function IsElementAnimated(nodeIp) {
	var node = cy.getElementById(nodeIp);
	return node.animated();
}
//checks if the quee of the given IP is full.
function CheckIfQueueIsFull(nodeIp) {
	console.log(nodeIp);
	var node = cy.getElementById(nodeIp);
	console.log("found elemnt: ");
	console.log(node);
	//check if limit has not been reached
	if (node._private.data.queue.length < node._private.data.limit - 1) {
		return false;
	}
	else {
		return true;
	}
}
//checks the head of the queue. Returns transactionId or null
function GetHeadOfQueue(nodeIp) {
	var node = cy.getElementById(nodeIp);
	console.log("contents of queue, getting head");
	console.log(node._private.data.queue);
	if (node._private.data.queue.length < 1) {
		return null;
	}
	else {
		console.log(node._private.data.queue[0]);
		return node._private.data.queue[0];
	}
}

function GetQueueList(nodeIp) {
	var node = cy.getElementById(nodeIp);
	console.log("contents of queue");
	console.log(node._private.data.queue);
	return node._private.data.queue;
}




