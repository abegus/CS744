/*var graphStopped = false;


function toggleFlow() {
	if (graphStopped) {
		resumeFlow();
		//$("#flowBtn").html("Stop");
	} else {
		stopFlow();
		//$("#flowBtn").html("Resume");
	}
}

function stopFlow() {

	// Stop new transactions from moving on the graph.
	graphStopped = true;

	// Stop transactions from moving on the graph.
	Object.keys(elementQueues).forEach(function (elementId) {
		if (elementQueues[elementId].queue.length > 0) {
			clearTimeout(elementQueues[elementId].queue[0].timeoutObj.timeout);
		}
	});
}

function resumeFlow() {

	// Allow new transactions to move on the graph.
	graphStopped = false;

	// Start transactions to move on graph.
	var sendTransactions = []
	Object.keys(elementQueues).forEach(function (elementId) {
		if (elementQueues[elementId].queue.length > 0 && !elementQueues[elementId].queue[0].destinationReached) {
			sendTransactions.push(elementQueues[elementId].queue[0]);
		}
	});

	sendTransactions.forEach(function (sendTransaction) {
		sendTransaction.timeoutObj.sendFunc(sendTransaction.timeoutObj.fromNode, sendTransaction.timeoutObj.toNode, sendTransaction);
	})
}*/


/* IF the 
async function sendToEdge(node, msg) {
	while (msg !== node.data("queue")[0] || isPaused) {
		await sleep(1000);
	}
	msg.lastNode = node;
	var path = CalculateRoute(node, msg);
	// Check if pathfinding could reach destination.
	if (path.length === 1) {
		node.data("queue").shift();
		return;
	}

	while (path[1].animated() || isPaused) {
		await sleep(1000);
	}
	node.data("queue").shift();
	StartAnimation(path[1], msg);
}

async function sendToNode(edge, msg) {
	var path = CalculateRoute(msg.lastNode, msg);
	// Check if pathfinding could reach destination.
	if (path.length === 1) {
		return;
	}
	// Check if next node went down.
	if (edge !== path[1]) {
		addMessageToNode(msg.lastNode, msg);
		return;
	}

	while (path[1].animated() || isPaused) {
		await sleep(1000);
	}
	// Check if next node went down again.
	if (edge !== path[1]) {
		addMessageToNode(msg.lastNode, msg);
		return;
	}

	// Send to next node.
	addMessageToNode(path[2], msg);
}





























/*//CALL TO START FROM NODE
function submitNewTransaction(node, transactionId, pc) {
	console.log("IN OTHER JAVASCRIPT FILE");
	console.log(node);
	//closeTransactionModal();
	//var transactionInfo = getTransactionInfo(node);
	//var pcId = getProcessingCenterId();
	var pcId = pc;
	initiateAnimation(node, pcId, transactionId);
}


//JAVASCRIPT TO WORK THE QUEUE AND ANIMATIONS
var isPaused = false;
var currentAnimations = [];
var delay = 1000;
var ANIM_SETTINGS = {
	nodeHighlight: {
		style: {
			'width': 50,
			'height': 50
		},
		queue: true,
		duration: delay
	},
	nodeRevert: {
		style: {
			'width': 30,
			'height': 30
		},
		queue: true,
		duration: delay
	},
	edgeHighlight: {
		style: {
			'width': 20
		},
		queue: true,
		duration: delay
	},
	edgeRevert: {
		style: {
			'width': 3
		},
		queue: true,
		duration: delay
	}
};

function SendMessage(node, msg) {
	msg.location = node.data.id;
	console.log(msg.id + ": " + msg.location);
}

function initiateAnimation(node, dest, transaction) {
	var message = CreateMessage(node, dest, transaction);
	addMessageToNode(node, message);
	//ShowPlayPauseButtons();
}

function CreateMessage(node, dest, transaction) {
	return new Message(transaction.transactionId, node.data("id"), dest, node.data("id"), transaction, false);
}

//function CreateResponse(msg, el) {
//	//$.ajax({
//	//    type: "POST",
//	//    url: '/Accounts/EditAccount',
//	//    data: JSON.stringify(data),
//	//    contentType: "application/json; charset=utf-8",
//	//    dataType: "json",
//	//    success: successFunc,
//	//    error: errorFunc
//	//});

//	var card = GetCardInfo(msg.payload);
//	var tran = GetTranInfo(msg.payload);

//	var data = {};
//	data.card = card;
//	data.tran = tran;

//	$.ajax({
//		type: "POST",
//		url: '/Accounts/AddTransaction',
//		data: JSON.stringify(data),
//		contentType: "application/json; charset=utf-8",
//		dataType: "json",
//		success: function (data) { SendResponse(data, el, msg); },
//		error: function () { console.log('error :('); }
//	});


//	//var response = new Response(msg.id, true, Date.now(), msg.payload.storeNode);
//	//return new Message(msg.id, msg.dest, msg.src, msg.dest, response, true);
//}

//function GetCardInfo(tran) {
//	var card = {};
//	var splitName = tran.name.split(" ");

//	card.first_name = (splitName.length >= 1) ? splitName[0] : "";
//	card.last_name = (splitName.length >= 2) ? splitName[1] : "";
//	card.number = tran.cardNumber;
//	card.expiration = tran.expMonth + "/" + tran.expYear;
//	card.cvv = tran.securityCode;

//	return card;
//}

function GetTranInfo(tran) {
	var newTran = {};
	newTran.id = tran.transactionId;
	newTran.amount = tran.amount;
	newTran.credit = tran.isCredit;
	newTran.merchant = (tran.isSelf) ? "SELF" : tran.storeNode.data.name;
	newTran.card_number = tran.cardNumber;

	return newTran;
}

function SendResponse(data, el, msg) {
	var response = data;
	response.id = msg.id;
	response.storeNode = msg.storeNode;

	var returnMsg = new Message(msg.id, msg.dest, msg.src, msg.dest, response, true);
	addMessageToNode(el, returnMsg);
}

async function addMessageToNode(node, msg) {
	node.data("queue").push(msg);
	while (node.animated() || isPaused) {
		await sleep(1000);
	}
	StartAnimation(node, msg);
}

function StartAnimation(el, msg) {
	var highlightType;
	var revertType;
	if (el.isNode()) {
		highlightType = ANIM_SETTINGS.nodeHighlight;
		revertType = ANIM_SETTINGS.nodeRevert;
	} else {
		highlightType = ANIM_SETTINGS.edgeHighlight;
		revertType = ANIM_SETTINGS.edgeRevert;
	}

	var animation = el.animation(highlightType);
	currentAnimations.push(animation);
	animation.play().promise().then(function () {
		// Remove animation from currentAnimations.
		currentAnimations.splice(currentAnimations.indexOf(animation), 1);

		animation = el.delayAnimation(delay);
		currentAnimations.push(animation);
		animation.play().promise().then(function () {
			// Remove animation from currentAnimations.
			currentAnimations.splice(currentAnimations.indexOf(animation), 1);

			animation = el.animation(revertType);
			currentAnimations.push(animation);
			animation.play().promise().then(async function () {
				// Remove animation from currentAnimations.
				currentAnimations.splice(currentAnimations.indexOf(animation), 1);

				if (el.data("id") === msg.dest) {
					// Remove the message from the queue so it's not permanently there.
					while (msg !== el.data("queue")[0] || isPaused) {
						await sleep(1000);
					}
					el.data("queue").shift();

					if (el.data("type") === "processing_center") {
						var responseMessage = CreateResponse(msg, el);
					} else {
						AttemptHidePlayPauseButtons();
					}
				} else if (el.isNode()) {
					sendToEdge(el, msg);
				} else {
					sendToNode(el, msg);
				}
			});
		});
	});
}

async function sendToEdge(node, msg) {
	while (msg !== node.data("queue")[0] || isPaused) {
		await sleep(1000);
	}
	msg.lastNode = node;
	var path = CalculateRoute(node, msg);
	// Check if pathfinding could reach destination.
	if (path.length === 1) {
		node.data("queue").shift();
		return;
	}

	while (path[1].animated() || isPaused) {
		await sleep(1000);
	}
	node.data("queue").shift();
	StartAnimation(path[1], msg);
}

async function sendToNode(edge, msg) {
	var path = CalculateRoute(msg.lastNode, msg);
	// Check if pathfinding could reach destination.
	if (path.length === 1) {
		return;
	}
	// Check if next node went down.
	if (edge !== path[1]) {
		addMessageToNode(msg.lastNode, msg);
		return;
	}

	while (path[1].animated() || isPaused) {
		await sleep(1000);
	}
	// Check if next node went down again.
	if (edge !== path[1]) {
		addMessageToNode(msg.lastNode, msg);
		return;
	}

	// Send to next node.
	addMessageToNode(path[2], msg);
}

function CalculateRoute(node, msg) {
	var eles = cy.elements('node[?active][type != "store"], node[id = "' + msg.dest + '"], node[id = "' + msg.src + '"], edge[?active]');
	console.log("PATH: ");
	console.log(eles);
	var dijkstra = eles.dijkstra(node, function (edge) {
		return edge.data('weight');
	});

	var dest = cy.getElementById(msg.dest);
	var path = dijkstra.pathTo(dest);

	return path;
}

function PauseAnimations() {
	isPaused = true;
	currentAnimations.forEach(function (el) {
		el.pause();
	});
}

function ResumeAnimations() {
	isPaused = false;
	currentAnimations.forEach(function (el) {
		el.play();
	});
}

//function ShowPlayPauseButtons() {
//	document.getElementById('buttonContainer').style.display = "inline-block";
//}

//function AttemptHidePlayPauseButtons() {
//	if (currentAnimations.length === 0) {
//		document.getElementById('buttonContainer').style.display = "none";
//	}
//}

function sleep(ms) {
	return new Promise(resolve => setTimeout(resolve, ms));
}
*/