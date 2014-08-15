#pragma once

//Francisco Chavez
//Fall 08
//CptS 223
//CharList: This is a stripped down doubly linked list<char>
	//It only conatins the functions that are useful to an
	//AdapHuff object.
#include<stdexcept>
using std::exception;

#ifndef CHAR_LIST_H
#define CHAR_LIST_H

struct CharNode{
	CharNode* next;
	CharNode* prev;
	char data;
};

class CharList{
public:
	class Iterator{
	public:
		Iterator(){
			current = NULL;
		}

		Iterator(CharNode* location){
			current = location;
		}

		//Pre:	Iterator o must exist
		//Post:	current = o.current
		Iterator(const Iterator& o)
			:current(o.current)
		{
		}

		//Pre:	Iterator o must exist
		//Post:	current = o.current
		//Post:	returns reference to this
		const Iterator& operator=(const Iterator& o){
			if(this != &o)
				current = o.current;

			return *this;
		}
		
		//Pre:	ListNode* o must exist
		//Post:	current = o
		//Post:	Returns reference to this
		const Iterator& operator=(CharNode*& o){
			if(o != NULL)
				current = o;

			return *this;
		}
		
		//Pre:	o must exist
		//Post:	returns wither or not the other Iterator points to the same node
		bool operator==(const Iterator& o)const{
			return current == o.current;
		}

		//Pre:	o must exist
		//Post:	returns !operater==(o)
		bool operator!=(const Iterator& o)const{
			return !(*this == o);
		}

		//Pre:	current != last
		//Post:	Moves Iterator to current->next and returns this reference
		//				preincrement
		//Post:	throws exception if current == last
		Iterator& operator++(){
			if(current->next == NULL)
				throw exception();
			
			current = current->next;

			return *this;
		}
		
		//Pre:	current != last
		//Post:	Moves Iterator to current->next after returning reference
		//				postincrement
		//Post:	throws exception if current == last
		Iterator& operator++(int){
			Iterator temp(*this);

			++(*this);

			return temp;
		}

		//Pre:	current != first->next
		//Post:	Moves Iterator to current->prev before returning *this reference
		//				predecricment
		//Post:	throws exception if current == first->prev
		Iterator& operator--(){
			if(current->prev->prev == NULL)
				throw exception();

			current = current->prev;

			return *this;
		}

		//Pre:	current != first->next
		//Post:	Moves Iterator to current->prev after returning *this reference
		//				postdecriment
		//Post:	throws exception if current == first->prev
		Iterator& operator--(int){
			Iterator temp(*this);

			--(*this);

			return temp;
		}

		//Post:	Returns a reference to the data inside of current
		char& operator*(){
			return current->data;
		}

		CharNode* operator&(){
			return current;
		}

		CharNode* current;
	};

	CharList();
	~CharList();


	bool isEmpty() const;
		//Post:	returns (size == 0)

	int getSize() const;
		//Post:	return size

	void makeEmpty();
		//Pre:	none;
		//Post:	Empties all data elements from list.
		//Post:	size = 0;


	char back() const;
		//Pre:	size > 0
		//Post:	Returns copy of last data element
		//Post:	if (size == 0), throws exception


	Iterator begin() const{
		Iterator result(head->next);
		return result;
	}	//end function begin
		//Post:	returns Iterator instance pointing to first element in
			//CharList
		//Note:	I was unable to export this function into the implementation file.

	Iterator end() const{
		Iterator result(tail);
		return result;
	}	//end function end
		//Post:	returns Iterator instance pointing on element past the last element in
			//CharList.
		//Note:	I was unable to export this function to the implentation file.


	void insert(const char& data, Iterator& position);
		//Pre:	Iterator must be initialized to a CharList
		//Pre:	Iterator.current != head
		//Post:	data element is inserted into CharList
		//Post:	Iterator points to inserted element

	void remove(Iterator& position);
		//Pre:	Iterator must be initialized to a CharList
		//Pre:	CharList.size > 0
		//Pre:	Iterator.current != head && Iterator. current != tail
		//Post:	data elemetn at positon Iterator is deleted from CharList
		//Post:	Iterator points the element after the deleted element


	void pushBack(const char& data);
		//Post:	data element is added to the end of CharList
	void popBack();
		//Pre:	size > 0
		//Post:	data element at end of CharList is removed
private:
	CharNode* head;
	CharNode* tail;
	int size;
};

#endif
