using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	internal class HuffmanTree
	{
		#region Attributes
		private TreeNode	_root					= null;
		private TreeNode	_head					= null;
		private TreeNode	_tail					= null;

		private TreeNode	_currentDecodePosition	= null;
		private List<bool>	_leftOverBits			= new List<bool>(8);
		#endregion


		#region Properties
		private TreeNode EmptyNode
		{
			get
			{
				return _head.Next;
			}
		}
		#endregion


		#region Constructors
		public HuffmanTree()
		{
			InitializeHuffTree();
		}
		#endregion


		#region Methods
		/// <summary>
		/// This method will insert a 16-bit Unicode character into an Adaptive Huffman 
		/// Tree and the encoded character as a BitArray. As more characters are 
		/// inserted, the BitArrays will start to get shorter (on average).
		/// </summary>
		public BitArray EncodeCharacter(char character)
		{
			// Encoding a character is a two part process.

			// 1. Get the bit code for the character based on the tree's current state.
			TreeNode	characterNode	= FindCharacterNode(character);
			bool[]		encodedBits		= GetCharacterBits(characterNode, character);

			// 2. Update the state of the tree by increaseing the character's frequency,
			//	  while keeping it balanced.
			UpdateTree(characterNode, character);

			BitArray result = new BitArray(encodedBits);
			return result;
		}

		/// <summary>
		/// This method will create 16-bit Unicode characters from an Adaptive Huffman 
		/// Tree. As characters are decoded the tree's state will be updated so that the
		/// more frequently decoded characters will have shorter shorter bit sequences. 
		/// When given an incomplete sequence, the decoding will continue from where it 
		/// left off on the next bit sequence.
		/// </summary>
		public char[] DecodeCharacters(bool[] bits)
		{
			var	charactersFound = new List<char>();
			var	bitIndex		= 0;

			///
			/// We are going to loop through the entire bit sequence that was given in 
			/// bits. Due to the changing nature of the Huffman Tree, it was just easier 
			/// for me to build this method with a forever loop. That being said, it can 
			/// be altered into a regular for loop and broken down into smaller methods.
			/// -FCT
			/// 
			while(true)
			{
				// We are already as far down the tree as we can get.
				if (_currentDecodePosition.IsLeaf)
				{
					// We are looking at the Empty Node. This means that this is the 
					// first use of the character we are about to decode. We will 
					// require 16 bits using little indian bit arrangement to get a 
					// Unicode16 character.
					if (_currentDecodePosition.IsEmpty)
					{
						int charSize = UnicodeEncoding.CharSize * 8;

						// Using any leftover bits from the last decode command (if 
						// there are any), we will continue looking for bits untill we 
						// have enough to form a single character.
						for (int i = 0; _leftOverBits.Count < charSize && bitIndex < bits.Length; i++, bitIndex++)
						{
							_leftOverBits.Add(bits[bitIndex]);
						}

						// If we have enough bits to get a single character, the get 
						// that character. Update the tree so that it now contains a 
						// character node of its own, add the character to the results 
						// of this decode pass, and reposition to the tree's root node.
						if (_leftOverBits.Count == charSize)
						{
							BitArray unicodeBits = new BitArray(_leftOverBits.ToArray());
							byte[] unicodeBytes = new byte[2];
							unicodeBits.CopyTo(unicodeBytes, 0);
							
							char newChar = Encoding.Unicode.GetChars(unicodeBytes)[0];
							charactersFound.Add(newChar);
							_leftOverBits.Clear();
							UpdateTree(_currentDecodePosition, newChar);
							_currentDecodePosition = _root;
						}
						// There weren't enough bits to decode a character. We will 
						// continue from where left off on the next call to decode.
						else
						{
							break;
						}
					}
					// We are looking at a leaf node containing a character that has 
					// been decoded before. Get what we can from the node, update the 
					// tree, move back to the top of the tree to start decoding the next 
					// character.
					else
					{
						charactersFound.Add(_currentDecodePosition.Character);
						UpdateTree(_currentDecodePosition, _currentDecodePosition.Character);
						_currentDecodePosition = _root;
					}
				}
				// We are currently looking at a branch, and there are bit left for use 
				// to decode with. Use the next bit to move down one level of the tree.
				// 
				else if (bitIndex < bits.Length)
				{
					_currentDecodePosition = bits[bitIndex] ? _currentDecodePosition.Right : _currentDecodePosition.Left;
					bitIndex++;
				}
				// We are currently looking at a branch, and we ran out of bits to 
				// decode with. We can't decode anything else until they give us more 
				// bits.
				else
				{
					break;
				}
			}

			// We're out of bits to decode. Return all characters that were found in 
			// this decode call.
			return charactersFound.ToArray();
		}

		/// <summary>
		/// This method returns the node for the given character in the Huffman Tree. If 
		/// the given character is not in the tree, it will return the empty node.
		/// </summary>
		private TreeNode FindCharacterNode(char character)
		{
			// The heigher the node's frequency, the further back is will be in the 
			// list. So, starting at the end of the list, and moving forward will speed 
			// things up (on average).
			TreeNode currentNode = _tail.Prev;

			while (true)
			{
				// The branch nodes contain a Character property and this property is of 
				// type char; char is a struct, so it always contains a value. This 
				// means that there is a slim chance that a branch node will contain the 
				// character we are looking for. Because of this, we first make sure 
				// that the node is a leaf node in the tree.
				// - FCT
				if (currentNode.IsLeaf)
				{
					// The empty node has a frequency of zero; for this reason, it will 
					// always be the first node in the linked list. If we don't find the 
					// node for the requested character, be the time we reach the front 
					// of the list, then it's not in the list.
					if (currentNode.IsEmpty || currentNode.Character == character)
						return currentNode;
				}

				currentNode = currentNode.Prev;
			}
		}

		/// <summary>
		/// Given a TreeNode, this method will return the path to that node from the 
		/// root. Starting from the start of the bool[], a false means go to the left 
		/// child, and true means go to the right child. If the character node is the 
		/// empty node, the character's bit patter will be appended to the end of the 
		/// array. The character will use Unicode with a little indian bit arrangment.
		/// </summary>
		private bool[] GetCharacterBits(TreeNode characterNode, char character, bool encodeCharacter = true)
		{
			List<bool> bits = new List<bool>();

			// Back track from the given node to the root node. We will stop when the 
			// current node is the root (which has no parent).
			TreeNode current	= characterNode;
			TreeNode parent		= characterNode.Parent;

			while (parent != null)
			{
				bits.Add(parent.Right == current);

				current = parent;
				parent	= current.Parent;
			}

			// The bits we have are going from character location to root, we need them 
			// to go the other way.
			bits.Reverse();

			// If the character isn't in the tree yet, we'll use the character's actual 
			// bit pattern to finish off the rest of the bit path.
			if (characterNode.IsEmpty && encodeCharacter)
			{
				byte[] charBytes = Encoding.Unicode.GetBytes(new char[] { character });
				BitArray bitArray = new BitArray(charBytes);
				for (int i = 0; i < bitArray.Length; i++)
					bits.Add(bitArray[i]);
			}

			return bits.ToArray();
		}

		/// <summary>
		/// This method updates and balances the tree based on the new character data 
		/// that is being added.
		/// </summary>
		private void UpdateTree(TreeNode characterNode, char character)
		{
			// Insert the character data into the tree if needed.
			if (characterNode.IsEmpty)
				characterNode = AddCharacterNode(character);

			// Re-balance the tree
			TreeNode nodeToUpdate = characterNode;

			while (nodeToUpdate != null)
			{
				nodeToUpdate.Frequency++;

				if (!SiblingPropertiesHold(nodeToUpdate))
				{
					// restore the sibling properties of the nodes by switching them around
					TreeNode nodeToSwitchPositionWith = FindNodeToSwitchWith(nodeToUpdate);
					SwitchNodePositions(nodeToUpdate, nodeToSwitchPositionWith);
				}

				nodeToUpdate = nodeToUpdate.Parent;
			}
		}

		/// <summary>
		/// The linked list portion of the TreeNodes are required to either increase 
		/// or hold the same frequency when it comes to itterating through the list. 
		/// This means that the relationships of sibling nodes are required to hold 
		/// certain relationships in their property values for the tree to remain 
		/// properly balanced. This method will tell us if those relationship 
		/// requirments are being held to for the given node.
		/// </summary>
		private bool SiblingPropertiesHold(TreeNode node)
		{
			if (node == _root)
				return true;

			if (node.Next == _root)
				return true;

			// If the next node is the list isn't this node's parent then that node 
			// better have frequency that is equal to or heigher than this node's 
			// frequency.
			if (node.Next != node.Parent)
				return node.Frequency <= node.Next.Frequency;

			return node.Frequency <= node.Next.Next.Frequency;
		}

		/// <summary>
		/// Find the TreeNode to switch with that will restore the proper tree 
		/// balence at this startingNode's level of the HuffmanTree.
		/// </summary>
		private TreeNode FindNodeToSwitchWith(TreeNode startingNode)
		{
			TreeNode targetNode = startingNode.Next;

			while (startingNode.Frequency > targetNode.Frequency)
				targetNode = targetNode.Next;

			targetNode = targetNode.Prev;

			if (startingNode.Parent == targetNode)
				targetNode = targetNode.Prev;

			return targetNode;
		}

		/// <summary>
		/// This method will create and return a new TreeNode for the given 
		/// character.
		/// </summary>
		/// <remarks>
		/// If there's a TreeNode for the character already present in the tree, it 
		/// will still create a new TreeNode for the character.
		/// </remarks>
		private TreeNode AddCharacterNode(char character)
		{
			///
			/// Insert data into tree structure
			/// 

			// The empty node will become a branch node, so the temp reference we're 
			// using is called newBranch. Besides, the left child of the newBranch 
			// will become the empty node.
			TreeNode newBranch = EmptyNode;
			newBranch.Left	= new TreeNode();
			newBranch.Right = new TreeNode();

			// Point the children back to their parent
			newBranch.Left.Parent		= newBranch;
			newBranch.Right.Parent		= newBranch;

			// Insert the character data into the new character node.
			newBranch.Right.Character	= character;

			///
			/// Insert data into linked list
			/// 
			// Point the head node and the empty node to eachother.
			_head.Next					= newBranch.Left;
			_head.Next.Prev				= _head;

			// Point the left and right nodes to eachother.
			newBranch.Left.Next			= newBranch.Right;
			newBranch.Right.Prev		= newBranch.Left;

			// Point the right node and parent node to eachother.
			newBranch.Right.Next		= newBranch;
			newBranch.Prev				= newBranch.Right;


			// Return the new character node
			return newBranch.Right;
		}

		/// <summary>
		/// This method will switch the positions of the two given nodes. The nodes 
		/// will have their positions in the tree sturcture switched, and their 
		/// positions in the linked list structure switched.
		/// </summary>
		/// <<remarks>
		/// TreeNode 'a' must come before TreeNode 'b' within the linked list 
		/// structure.
		/// </remarks>
		private void SwitchNodePositions(TreeNode a, TreeNode b)
		{
			/// 
			/// Switch nodes within tree structure
			/// 
			{
				TreeNode aParent = a.Parent;
				TreeNode bParent = b.Parent;

				if (aParent == bParent)
				{
					aParent.Left = b;
					aParent.Right = a;
				}
				else
				{
					if (aParent.Left == a)
						aParent.Left = b;
					else
						aParent.Right = b;

					if (bParent.Left == b)
						bParent.Left = a;
					else
						bParent.Right = a;

					a.Parent = bParent;
					b.Parent = aParent;
				}
			}

			///
			/// Switch nodes within linked list structure
			/// 
			{
				TreeNode aPrevious	= a.Prev;
				TreeNode aNext		= a.Next;
				TreeNode bPrevious	= b.Prev;
				TreeNode bNext		= b.Next;

				aPrevious.Next = b;
				b.Prev = aPrevious;

				if (a.Next == b)
				{
					b.Next = a;
					a.Prev = b;
				}
				else
				{
					b.Next = aNext;
					aNext.Prev = b;

					bPrevious.Next = a;
					a.Prev = bPrevious;
				}

				a.Next = bNext;
				bNext.Prev = a;
			}
		}

		/// <summary>
		/// This method returns an array of strings with the current character, 
		/// frequency, and path of the leaf nodes that make up the tree.
		/// </summary>
		public string[] GetTestReadout(bool includeBranches = false)
		{
			List<string> info = new List<string>();

			TreeNode currentNode = _head.Next;

			while (currentNode != _tail)
			{
				if (currentNode.IsLeaf)
				{
					//
					// Get the node's character data
					//
					info.Add(string.Format("ID: {0}", currentNode.NodeID));
					info.Add("Leaf Node:");
					if (currentNode.IsEmpty)
						info.Add("Empty Node");
					else
						info.Add(string.Format("Character Node: {0}", currentNode.Character));


					//
					// Get the Node's weight value
					//
					info.Add(string.Format("Character Frequency: {0}", currentNode.Frequency));

					//
					// Get the node's path data
					//

					// We just want the path of the given node, but we don't want to 
					// encode the character data if it happens to be the Empty Node.
					bool[] bitPath = GetCharacterBits(currentNode, 'a', false);

					// Convert the the path into a string that is more easily read by 
					// humans
					StringBuilder b = new StringBuilder();
					b.Append("Node Path: ");
					for (int i = 0; i < bitPath.Length; i++)
						b.Append(bitPath[i] ? 'R' : 'L');

					info.Add(b.ToString());

					//
					// Add a empty line for readability
					//
					info.Add(string.Empty);
				}
				else if (includeBranches)
				{
					info.Add(string.Format("ID: {0}", currentNode.NodeID));

					info.Add("Branch Node:");
					info.Add(string.Format("Frequency: {0}", currentNode.Frequency));


					//
					// Get the node's path data
					//

					// We just want the path of the given node, but we don't want to 
					// encode the character data if it happens to be the Empty Node.
					bool[] bitPath = GetCharacterBits(currentNode, 'a', false);

					// Convert the the path into a string that is more easily read by 
					// humans
					StringBuilder b = new StringBuilder();
					b.Append("Node Path: ");
					for (int i = 0; i < bitPath.Length; i++)
						b.Append(bitPath[i] ? 'R' : 'L');

					info.Add(b.ToString());

					info.Add(string.Empty);
				}

				currentNode = currentNode.Next;
			}

			return info.ToArray();
		}

		/// <summary>
		/// This method will resest the HuffmanTree to the point it was when first 
		/// constructed. 
		/// </summary>
		/// <remarks>
		/// The nodes in the tree will continue to increment their IDs from where 
		/// they left off.
		/// </remarks>
		public void ClearTree()
		{
			TreeNode currentNode = _head;

			while (currentNode != null)
			{
				if (currentNode.Prev != null)
					currentNode.Prev.Next = null;

				currentNode.Parent = null;
				currentNode.Left = null;
				currentNode.Right = null;
				currentNode.Prev = null;

				currentNode = currentNode.Next;
			}

			InitializeHuffTree();
		}

		/// <summary>
		/// This method will initialize the tree and linked list structures of the 
		/// HuffmanTree with new nodes. The only piece of data in the HuffmanTree will 
		/// be the Empty Node.
		/// </summary>
		private void InitializeHuffTree()
		{
			_head = new TreeNode();
			_tail = new TreeNode();
			_root = new TreeNode();

			_head.Next = _root;
			_root.Next = _tail;

			_tail.Prev = _root;
			_root.Prev = _head;

			// This node is always the node at the end of the list. This will keep it 
			// constistant with the rules of the linked list.
			_tail.Frequency = int.MaxValue;

			_currentDecodePosition = _root;
		}
		#endregion


		private class TreeNode
		{
			#region Attributes
			private			bool	_characterHasBeenSet = false;
			private			char	_character;

			private static	ushort NextID = 0;
			private			ushort _id;
			#endregion


			#region Properties
			public ushort	NodeID		{ get { return _id; } }

			public TreeNode Next		{ get; set; }
			public TreeNode Prev		{ get; set; }

			public TreeNode Parent		{ get; set; }
			public TreeNode Left		{ get; set; }
			public TreeNode Right		{ get; set; }

			public bool		IsLeaf		{ get { return Right == null && Left == null; } }

			public int		Frequency	{ get; set; }
			public bool		IsEmpty		{ get { return IsLeaf && !_characterHasBeenSet; } }

			public char Character
			{
				get { return _character; }
				set
				{
					_character = value;
					_characterHasBeenSet = true;
				}
			}
			#endregion


			#region Constructors
			public TreeNode()
			{
				this._id = NextID++;

				this._characterHasBeenSet = false;

				this.Frequency	= 0;
				this.Left		= null;
				this.Next		= null;
				this.Prev		= null;

				this.Right		= null;
				this.Parent		= null;
			}
			#endregion
		}
	}
}
