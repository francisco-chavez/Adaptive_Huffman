#pragma once

//Francisco Chavez
//Fall 08: CptS 223
//Program 04
//Adaptive Huffman:
//
//This class is used to encode and decode messages using Adaptive Huffman
#ifndef ADAPTIVE_HUFF_H
#define ADAPTIVE_HUFF_H

#include "CharList.h"
#include <string>

using namespace std;

struct HuffNode{
	HuffNode* left;
	HuffNode* right;
	HuffNode* parent;
	HuffNode* next;	//pointer to sibling that is greater than or equal to this node
	HuffNode* prev;	//pointer to sibling that is less then or equal to this node
	char letter;
	unsigned int count;
};

class AdaptHuff{
public:
	AdaptHuff();
	~AdaptHuff();

	string encode(char* message, char* alphabet);
		//Pre:	Each character in message should show up in alphabet only once
		//Post:	Returns a sting with the AdaptiveHuffman encoding for the givin
			//		message and alphabet
		//Post:	Any letter in the message that isn't found in the alphabet will be
			//		skipped.
		//Post:	Multiple instances of the same character inside the alphabet may
			//		result in invalid results. (There is a small chance that the output will
			//		be correct).
	string decode(char* message, char* alphabet);
		//Pre:	Each character in alphabet should show up only once.
		//Pre:	The giving aphabet should be the same as the alphabet that was used
			//		to encode the message.
		//Post:	Returns a string that represents the original message  before it was encoded.
		//Post:	Multiple instance of the same character inside the alphabet may result in invalid
			//		results.
private:
	void deleteHuffTree();
		//Pre:	none
		//Post:	deletes all the nodes found in the HuffTree
		//Post:	root = NULL
	void emptyHuff();
		//Pre:	none
		//Post:	calls deleteHuffTree and empties znd (ZeroNode data)
	void setUpZeroNode(char* alphabet);
		//Pre:	(root == NULL) && (znd.isEmpty())
		//Post:	root = Zero Node && znd = alphabet

	void findLetterEncode(const char& letter, string& result, HuffNode*& nodeToUpdate);
		//Pre:	letter has the letter you are looking for in HuffTree, nodeToUpdate starts with root
		//Post:	result += path to letter && nodeToUpdate = node containing letter
		//findLetterEncode tranverses the HuffTree looking for the letter you want to encode. In the process
			//of finding the letter, it appends the path to the letter at the end of result. It also leaves 
			//nodeToUpdate pointing to the node containing the letter as a starting point when updateing the
			//HuffTree
	void findLetterDecode(unsigned int& i, char*& message, string& result, HuffNode*& nodetoUpdate);
		//Pre:	root == nodeToUpdate
		//Post:	i is increased so that message[i] points to the encoding for finding the next letter,
			//		letter found is appended to end of result, nodeToUpdate points to the node containing
			//		the letter that was just decoded.
		//findLetterDecode is the equivalent to findLetterEncode and you want to decode a letter.
	bool hasLetter(char letter, HuffNode*& node);
		//Pre:	none
		//Post:	returns true if node or any of node's decendents has letter, returns false other wise
		//hasLetter is used to find which path to take when seeking out a letter

	void updateHuffTree(HuffNode* node, char letter);
		//Pre:	node points to node containing last letter encoded/decoded
		//Pre:	you shouldn't call this if the HuffTree is already up to date
		//Post:	HuffTree is updated
		//updateHuffTree updates the Adaptive Huffman structure starting from the node contaning the last
			//letter that was read. It also makes sure that the siblining property is restored whenever
			//it's broken.
	void updateZeroNode(HuffNode*& node, const char& letter);
		//Pre:	last letter found is in znd (ZeroNode data) && node points to ZeroNode
		//Post:	node->left = new ZeroNode, node->right = new leaf (with letter) && letter (in znd) is 
			//		overwritten with last element in znd before popping off last element in znd
		//updateZeroNode splits the ZeroNode

	bool sibPropHolds(HuffNode* node);
		//Pre:	node points to last node to have it's count increased
		//Post:	if sib. prop. holds for node, returns true, else, returns false
	void restoreSibProp(HuffNode* node);
		//Pre:	!sibPropHolds(node)
		//Post:	sibPropHolds(node)
		//This function restores the sibling property when it is violated.
	void getLeaderPtr(HuffNode*& node, HuffNode*& leaderPtr);
		//Pre:	!sibPropHolds(node)
		//Post:	leaderPtr = the leader for node (execpt for node's parent) before node's count was
			//		incremented
	void swapNodes(HuffNode*& node1, HuffNode*& node2);
		//Pre:	node1 < node2 (node1 != node2->next)
		//Pre:	node1 != NULL && node2 != NULL
		//Post:	node1 takes node2's postion in the HuffTree, and node2 takes node1's position in
			//		the HuffTree

	HuffNode* root;
	CharList znd;	//zero node data
};

#endif
