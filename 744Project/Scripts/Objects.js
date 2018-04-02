//Nodes
class Node {
	constructor(id, name, ip, x, y, type, region) {
		this.data = {
			id: id,
			name: name,
			ip: ip,
			type: type,
			region: region,
			active: true,
			queue: [],
			node: null
		};

		this.position = {
			x: x,
			y: y
		};
	}

	setNode(theNode) {
		this.data.node = theNode;
	}
}

class Store extends Node {
	constructor(num, name, x, y) {
		var id = "s" + num;
		var ip = "192.168.0." + num;
		super(id, name, ip, x, y, "store");
	}
}

class Router extends Node {
	constructor(num, x, y) {
		var id = "r" + num;
		var name = "Relay Station " + num;
		var ip = "192.168.0." + (num + 100);
		super(id, name, ip, x, y, "router");
	}
}

class ProcessingCenter extends Node {
	constructor(x, y) {
		var id = "pc";
		var name = "Processing Center";
		var ip = "192.168.0.201";
		super(id, name, ip, x, y, "processing_center");
	}
}

//edges
class Edge {
	constructor(source, target, weight) {
		this.data = {
			id: source.data.id + "_" + target.data.id,
			source: source.data.id,
			target: target.data.id,
			weight: weight,
			active: true
		};
	}
}

class Message {
	constructor(id, src, dest, lastNode, payload, isResponse) {
		this.id = id;
		this.src = src;
		this.dest = dest;
		this.lastNode = lastNode;
		this.payload = payload;
		this.isResponse = isResponse;
	}
}

class Response {
	constructor(id, status, date, node) {
		this.id = id;
		this.status = status;
		this.date = date;
		this.storeNode = node;
	}
}

class Transaction {
	constructor(node, amount, cardNumber, name, expMonth, expYear, securityCode, isSelf,
		transactionId, isCredit, date) {
		this.storeNode = node;
		this.amount = amount;
		this.cardNumber = cardNumber;
		this.name = name;
		this.expMonth = expMonth;
		this.expYear = expYear;
		this.securityCode = securityCode;
		this.isSelf = isSelf;
		this.transactionId = transactionId;
		this.isCredit = isCredit;
		this.date = date;
	}
}


