#include "stdafx.h"


#include "AdaptiveHuffman.h"
#include "CharList.h"
#include <string>
#include <iostream>
#include <cstdlib>

using namespace std;

AdaptHuff::AdaptHuff(){
	root = NULL;
}	//end AdaptHuff constructor

AdaptHuff::~AdaptHuff(){
	deleteHuffTree();
}	//end ~AdaptHuff destructor

void AdaptHuff::deleteHuffTree(){
	for(HuffNode* i = root; i != NULL; i = root){
		root = i->prev;
		delete i;
	}
}	//end function deleteSubHuffTree

string AdaptHuff::encode(char* message, char* alphabet){
	//set up AdaptHuff for next use
	string result = "";
	setUpZeroNode(alphabet);
	HuffNode* node = root;

	for(unsigned int i = 0; i < strlen(message); i++, node = root){
		findLetterEncode(message[i], result, node);
		updateHuffTree(node, message[i]);
	}

	return result;
}	//end encode

string AdaptHuff::decode(char* message, char* alphabet){
	//set up AdapHuff for next use
	string result = "";
	string currentLetter = "";	//I know it's not neccessary, but it makes debuggin easier
	setUpZeroNode(alphabet);
	HuffNode* node = root;

	for(unsigned int i = 0; i < strlen(message); result = result + currentLetter, currentLetter = "", node = root){
		findLetterDecode(i, message, currentLetter, node);
		updateHuffTree(node, (currentLetter[0]));
	}

	return result;
}	//end function decode

void AdaptHuff::emptyHuff(){
	deleteHuffTree();
	znd.makeEmpty();
}	//end function emptyHuffTree

void AdaptHuff::setUpZeroNode(char* alphabet){
	emptyHuff();

	for(unsigned int i = 0; i < strlen(alphabet); i++){
		znd.pushBack(alphabet[i]);
	}

	root = new HuffNode;

	root->left = NULL;
	root->right = NULL;
	root->parent = NULL;
	root->next = NULL;
	root->prev = NULL;
	root->letter = 0;
	root->count = 0;
}	//end function setUpZeroNode

void AdaptHuff::findLetterEncode(const char& letter, string& result, HuffNode*& nodeToUpdate){
	bool found = false;

	//loot until you find letter
	while(!found){
		if(nodeToUpdate->count == 0){		//if letter is in ZeroNode, look in znd (ZeroNode data)
			CharList::Iterator trace = znd.begin();

			while(trace != znd.end())
				if((*trace) == letter){
					result = result + "10";
					trace = znd.end();
				}else{
					result = result + "1";
					trace++;
				}

			found = true;
		}else if(nodeToUpdate->letter == letter){
			found = true;
		}else if(hasLetter(letter, nodeToUpdate->left)){
			nodeToUpdate = nodeToUpdate->left;
			result = result + "0";
		}else if(hasLetter(letter, nodeToUpdate->right)){
			nodeToUpdate = nodeToUpdate->right;
			result = result + "1";
		}else{
			cerr << "Letter '" << letter << "' might not be in the alphabet.\n"
				<< "Skipping and moving on to next letter." << endl;
			found = true;
		}
	}
}	//end function findLetterEncode

void AdaptHuff::findLetterDecode(unsigned int& i, char*& message, string& result, HuffNode*& node){
	bool found = false;

	//loop until you find a leaf
	while(!found){
		if(node->count == 0){	//if in ZeroNode
			CharList::Iterator trace = znd.begin();

			do{
				trace++;
				i++;
			}while(message[i] != '0');
			trace--;		//backup one spot

			result = result + (*trace);
			found = true;
		}else if(node->letter != 0){	//if in node with Letter
			result = result + (node->letter);
			i--;	//i++ at end pushes one spot too far without this
			found = true;
		}else if(message[i] == '0')
			node = node->left;
		else
			node = node->right;

		i++;
	}
}	//end function findLetterDecode

bool AdaptHuff::hasLetter(char letter, HuffNode*& node){
	if(node == NULL){
		return false;
	}else if(node->count == 0){	//if in ZeroNode
		for(CharList::Iterator i = znd.begin(); i != znd.end(); i++)
			if((*i) == letter)
				return true;

		return false;
	}else if(node->letter == letter){
		return true;
	}else{
		return (hasLetter(letter, node->left) || hasLetter(letter, node->right));	//check children
	}
}	//end function hasLetter

void AdaptHuff::updateHuffTree(HuffNode* node, char letter){
	//if starting in ZeroNode
	if(node->count == 0)
		updateZeroNode(node, letter);	

	//update each node, check the sib. prop., restore sib. prop. if needed,
	//continue with node's parent, repeat until there's nothing left to update
	while(node != NULL){
		(node->count)++;

		if(!sibPropHolds(node))
			restoreSibProp(node);

		node = node->parent;
	}
}	//end function updateHuffTree

void AdaptHuff::updateZeroNode(HuffNode*& node, const char& letter){
	node->left = new HuffNode;
	node->right = new HuffNode;
	node->prev = node->right;

	//setup new ZeroNode
	node->left->left = NULL;
	node->left->right = NULL;
	node->left->parent = node;
	node->left->next = node->right;
	node->left->prev = NULL;
	node->left->letter = 0;
	node->left->count = 0;

	//setup new leaf
	node->right->left = NULL;
	node->right->right = NULL;
	node->right->parent = node;
	node->right->next = node;
	node->right->prev = node->left;
	node->right->letter = letter;
	node->right->count = 1;
	//finished spliting zero node

	//update zero node's data
	if(znd.back() != letter){
		CharList::Iterator trace = znd.begin();
		bool found = false;

		while(!found){
			if((*trace) == letter)
				found = true;
			else
				trace++;
		}

		//replace letter with copy of last element
		znd.insert(znd.back(), trace);
		trace++;
		znd.remove(trace);
	}

	//pop off last element
	znd.popBack();

	(node->count)++;
	node = node->parent;
}	//end function updateZeroNode

bool AdaptHuff::sibPropHolds(HuffNode* node){
	if(node == root)
		return true;
	//if(root->right == node)	//this test didn't work for some reason
		//return true;
	if(node->next == root)
		return true;
	if(node->next != node->parent){
		if(node->count <= node->next->count)
			return true;
		return false;
	}
	if(node->count <= node->next->next->count)
		return true;
	return false;
}	//end function sibPropHolds

void AdaptHuff::restoreSibProp(HuffNode* node){
	HuffNode* leaderPtr = NULL;

	getLeaderPtr(node, leaderPtr);	//find node to swap places with
	swapNodes(node, leaderPtr);		//swap place with that node
}	//end restoreSibProp

void AdaptHuff::getLeaderPtr(HuffNode*& node, HuffNode*& leaderPtr){
	leaderPtr = node;

	do{
		leaderPtr = leaderPtr->next;
	}while(leaderPtr->count < node->count);

	leaderPtr = leaderPtr->prev;

	if(leaderPtr == node->parent)
		leaderPtr = leaderPtr->prev;
}	//end function getLeaderPtr

void AdaptHuff::swapNodes(HuffNode*& node1, HuffNode*& node2){
	HuffNode* temp = NULL;

	if(node1->parent != node2->parent){	//if node1->parent == node2->parent the nodes
		//switch parent's children			//would be switched over twice, leaving them
		if(node1->parent->left == node1)	//in their original positions in the Huff Tree
			node1->parent->left = node2;
		else
			node1->parent->right = node2;

		if(node2->parent->left == node2)
			node2->parent->left = node1;
		else
			node2->parent->right = node1;

		//switch children's parents
		temp = node1->parent;
		node1->parent = node2->parent;
		node2->parent = temp;
	}else{
		if(node1->parent->left == node1){
			node1->parent->left = node2;
			node1->parent->right = node1;
		}else{
			node1->parent->left = node1;
			node1->parent->right = node2;
		}
	}

	//switch nodes in linked list
	node1->prev->next = node2;
	node2->next->prev = node1;

	if(node1->next == node2){
		node1->next = node2->next;
		node2->prev = node1->prev;

		node1->prev = node2;
		node2->next = node1;
	}else{
		node1->next->prev = node2;
		node2->prev->next = node1;

		temp = node2->next;
		node2->next = node1->next;
		node1->next = temp;

		temp = node2->prev;
		node2->prev = node1->prev;
		node1->prev = temp;
	}
}	//end function swapNodes
