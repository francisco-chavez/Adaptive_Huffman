#include "stdafx.h"


//Francisco Chavez
//Fall 08: CptS 223
//
//This file holds the implementation for the CharList class
#include <stdexcept>
using std::exception;

#include "CharList.h"

CharList::CharList(){
	head = new CharNode;
	tail = new CharNode;
	size = 0;

	head->next = tail;
	head->prev = NULL;

	tail->next = NULL;
	tail->prev = head;
}	//end CharList constructor

CharList::~CharList(){
	makeEmpty();

	delete head;
	delete tail;
}	//end ~CharList destructor

bool CharList::isEmpty() const{
	return (size == 0);
}	//end function isEmpty

int CharList::getSize() const{
	return size;
}	//end function getSize

void CharList::makeEmpty(){
	for(CharNode* temp = head->next; temp != tail; temp = head->next){
		head->next = temp->next;
		delete temp;
	}

	tail->prev = head;
	size = 0;
}	//end function makeEmpty

char CharList::back() const{
	if(size == 0)
		throw exception();

	return (tail->prev->data);
}	//end function back

void CharList::insert(const char& data, Iterator& position){
	if((&position) == NULL)
		throw exception();

	CharNode* node = new CharNode;

	//set up node
	node->next = (&position);
	node->prev = node->next->prev;
	node->data = data;

	//insert node into list;
	node->next->prev = node;
	node->prev->next = node;

	position--;	//move position to original position
	size++;
}	//end function insert

void CharList::remove(Iterator& position){
	if(size == 0 || (&position) == NULL || position == end())
		throw exception();

	CharNode* node = (&position);
	position++;		//return position to original position

	//remove node from list
	node->next->prev = node->prev;
	node->prev->next = node->next;

	//remove list from node
	node->next = NULL;
	node->prev = NULL;

	delete node;
	size--;
}	//end function remove

void CharList::pushBack(const char& data){
	CharNode* node = new CharNode;

	//set up node
	node->next = tail;
	node->prev = tail->prev;
	node->data = data;

	//insert node into list
	tail->prev = node;
	node->prev->next = node;

	size++;
}	//end function pushBack

void CharList::popBack(){
	if(size == 0)
		throw exception();

	CharNode* temp = tail->prev;

	//remove temp from list
	temp->prev->next = temp->next;
	temp->next->prev = temp->prev;

	//remove list from temp
	temp->next = NULL;
	temp->prev = NULL;

	size--;
	delete temp;
}	//end function popBack
