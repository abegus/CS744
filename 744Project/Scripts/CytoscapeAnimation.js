
var currentAnimations = [];
var resumeAnimationList = [];

//Entry function of sending a transaction.
function submitNewTransaction(nodeId, transactionId, pc, path) {
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
	//base base case where the pathIndex is out of bounds (called on completion of route).
	//if it is back to the store it started at, then remove it from the queue and end.
	if (pathIndex >= transObj.path.length && transObj.processed) {
		RemoveTransactionFromQueue(transactionId, transObj.storeIp);
		RemoveFakeTransactionFromQueue(transactionId, transObj.storeIp);
		UpdateTransactionStatus("Returned To Source", transactionId)
		return;
	} 
	//if the node is inactive, drop the transaction
	if (!GetActiveStatus(currentIp)) {
		RemoveTransactionFromQueue(transactionId, transObj.storeIp);
		RemoveFakeTransactionFromQueue(transactionId, transObj.lastRelay);
		UpdateTransactionStatus("Dropped Transaction", transactionId);
		return;
	}

	var result = CheckIfQueueIsFull(currentIp);
	//base case, the queue is full and the transaction is dropped
	if (result) {
		RemoveTransactionFromQueue(transactionId, transObj.storeIp);
		RemoveFakeTransactionFromQueue(transactionId, transObj.lastRelay);
		UpdateTransactionStatus("Dropped Transaction (full Queue)", transactionId);
		return;
	}

	//Change state, add to queue...
	AddTransactionToQueue(transactionId, currentIp);

	//Continue Case, this element is the only thing on the queue and if the current element is not being animated, and animation is runnign.
	var isHead = GetHeadOfQueue(currentIp) == transactionId;
	var isAnimated = IsElementAnimated(currentIp);
	console.log("result: " + isHead + ", animated?: " + isAnimated);
	if (isHead  && !isAnimated ) { //still check the next To-Queue if it's open??? can it take it?
		console.log("animate the item");
		AnimateItem(transactionId, currentIp, transObj, pathIndex);
		return;
	}

	//case where it is puased and just sent, if it makes it to here, add it to resume animation stuff
	if (!isAnimated && pathIndex <= 1) {
		AddToResumeAnimationList(transactionId, currentIp, transObj, pathIndex);
		return;
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

	//bug case on wakeup? might affect transactions coming from 2 ways ...
	if (IsElementAnimated(currentIp)) {
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




/********************************************************************************************************************
 *							HELPER METHODS 
 ********************************************************************************************************************/

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

	//remove from actual happens every time
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

function RemoveFakeTransactionFromQueue(transId, currentIp) {
	var transObj = TransactionList[transId + ""];

	var removed = false;
	var node = cy.getElementById(currentIp);
	//getting fake queue (which is only used to show what elements are at what queue)
	var queue = node._private.data.lastQueue;

	//remove from actual happens every time
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
	node.data('lastQueue', tempQueue);
	console.log("-------------REMOVED------------");
	//console.log(node);
}

//adds the element to the queue
function AddTransactionToQueue(transId, nodeIp) {

	//basic add, happens every time.
	var node = cy.getElementById(nodeIp);
	var queue = node._private.data.queue;
	var tempQueue = [];
	for (var i in queue) {
	//	console.log("EXISTING QUEUE");
		tempQueue.push(queue[i]); //was tempQueue.push(i);
	}
	tempQueue.push(transId);
	node.data('queue', tempQueue);

	// fake add which updates the lastQueue. Also has to update the transObj
	var ipAsString = nodeIp + "";
	if (ipAsString.length < 16) {
		var transObj = TransactionList[transId + ""];
		var resetIp = transObj.lastRelay; // the ip that needs to remove this transaction from its lastQueue
		if (resetIp != null) {// it could be null in the initial base case
			RemoveFakeTransactionFromQueue(transId, resetIp);
		}
		//now update the object and the new relay's lastQueue
		AddFakeTransactionToQueue(transId, nodeIp);
		transObj["lastRelay"] = nodeIp;
	}
	
}
//adds the element to the lastQueue (fake representation queue for UI)
function AddFakeTransactionToQueue(transId, nodeIp) {

	//basic add, happens every time.
	var node = cy.getElementById(nodeIp);
	var queue = node._private.data.lastQueue;
	var tempQueue = [];
	for (var i in queue) {
		//	console.log("EXISTING QUEUE");
		tempQueue.push(queue[i]); //was tempQueue.push(i);
	}
	tempQueue.push(transId);
	node.data('lastQueue', tempQueue);

}

//checks if the current element (ip) is being animated
function IsElementAnimated(nodeIp) {
	var node = cy.getElementById(nodeIp);
	return node.animated();
}
//checks if the quee of the given IP is full.
function CheckIfQueueIsFull(nodeIp) {
	//console.log(nodeIp);
	var node = cy.getElementById(nodeIp);
	//console.log("found elemnt: ");
	//console.log(node);
	//check if limit has not been reached
	if (node._private.data.queue.length < node._private.data.limit ) {
		return false;
	}
	else {
		return true;
	}
}
//checks the head of the queue. Returns transactionId or null
function GetHeadOfQueue(nodeIp) {
	var node = cy.getElementById(nodeIp);
	//console.log("contents of queue, getting head");
	//console.log(node._private.data.queue);
	if (node._private.data.queue.length < 1) {
		return null;
	}
	else {
	//	console.log(node._private.data.queue[0]);
		return node._private.data.queue[0];
	}
}

function GetQueueList(nodeIp) {
	var node = cy.getElementById(nodeIp);
	console.log("contents of queue");
	console.log(node._private.data.queue);
	return node._private.data.queue;
}

function GetActiveStatus(nodeIp) {
	var node = cy.getElementById(nodeIp);
	//console.log("contents of queue");
	//console.log(node._private.data.queue);
	return node._private.data.isActive;
}


// update the innerHtml of the given transaction
function UpdateTransactionStatus(Message, transactionId){
	//var div = $(".status#" + transactionId).html(Message + "");
	$("td#" + transactionId + ".status").html(Message + "");
}

function UpdateTransactionProcessed(Message, transactionId) {
	//var div =
		$("td#"+transactionId+".pstatus").html(Message + "");
}


