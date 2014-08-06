using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public class HuffmanTree
	{
		#region Attributes
		private TreeNode _root = null;
		private TreeNode _head = null;
		private TreeNode _tail = null;
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
			_head = new TreeNode();
			_tail = new TreeNode();
			_root = new TreeNode();

			_head.Next = _root;
			_root.Next = _tail;

			_tail.Prev = _root;
			_root.Prev = _head;

			// This node is always the node at the end of the list. This will stop 
			// the consitancy checking method for the tree's balance from trying to 
			// switch it with other nodes in the list.
			_tail.Frequency = int.MaxValue;
		}
		#endregion


		#region Methods
		/// <summary>
		/// This method will insert a 16-bit Unicode character into an adaptive 
		/// Huffman Tree and the encoded character as a BitArray. As more characters 
		/// are inserted, the BitArrays will start to get shorter (on average).
		/// </summary>
		public BitArray InsertCharacter(char character)
		{
			TreeNode	characterNode	= FindCharacterNode(character);
			bool[]		encodedBits		= GetCharacterBits(characterNode, character);

			UpdateTree(characterNode, character);

			BitArray result = new BitArray(encodedBits);
			return result;
		}

		/// <summary>
		/// This method returns the node for the given character in the Huffman Tree.
		/// If the given character is not in the tree, it will return the empty node.
		/// </summary>
		private TreeNode FindCharacterNode(char character)
		{
			// The heigher the node's frequency, the further back is will be in the 
			// list. So, starting at the end of the list, and moving forward will 
			// speed things up (on average).
			TreeNode currentNode = _tail.Prev;

			while (true)
			{
				// The branch nodes contain a Character property and this property 
				// is of type char; char is a struct, so it always contains a value. 
				// This means that there is a slim chance that a branch node will 
				// contain the character we are looking for. Because of this, we
				// first make sure that the node is a leaf node in the tree.
				// - FCT
				if (currentNode.IsLeaf)
				{
					// The empty node has a frequency of zero; for this reason, it 
					// will always be the first node in the linked list. If we don't 
					// find the node for the requested character, be the time we 
					// reach the front of the list, then it's not in the list.
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
		/// array. The character will use Unicode with a little indian bit 
		/// arrangment.
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

			// The bits we have are going from character location to root, we need 
			// them to go the other way.
			bits.Reverse();

			// If the character isn't in the tree yet, we'll use the character's 
			// actual bit pattern to finish off the rest of the bit path.
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
			TreeNode nextNode = startingNode;

			do
			{
				nextNode = nextNode.Next;
			} while (nextNode.Frequency < startingNode.Frequency);

			nextNode = nextNode.Prev;

			if (nextNode == startingNode.Parent)
				nextNode = nextNode.Prev;

			return nextNode;
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

			// Point the head node and the empty node to eachother.
			_head.Next					= newBranch.Left;
			newBranch.Left.Prev			= _head;

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
						aParent.Left = b;

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
		#endregion


		private class TreeNode
		{
			#region Attributes
			private bool m_characterHasBeenSet = false;
			private char m_character;
			#endregion


			#region Properties
			public TreeNode Next		{ get; set; }
			public TreeNode Prev		{ get; set; }

			public TreeNode Parent		{ get; set; }
			public TreeNode Left		{ get; set; }
			public TreeNode Right		{ get; set; }

			public bool		IsLeaf		{ get { return Right == null && Left == null; } }

			public int		Frequency	{ get; set; }
			public bool		IsEmpty		{ get { return IsLeaf && !m_characterHasBeenSet; } }

			public char Character
			{
				get { return m_character; }
				set
				{
					m_character = value;
					m_characterHasBeenSet = true;
				}
			}
			#endregion


			#region Constructors
			public TreeNode()
			{
				this.m_characterHasBeenSet = false;

				this.Frequency	= 0;
				this.Left		= null;
				this.Next		= null;
				this.Prev		= null;

				this.Right		= null;
				this.Parent		= null;
			}
			#endregion


			#region Methods
			public void UpdateFrequency()
			{
				if (!IsLeaf)
				{
					Left.UpdateFrequency();
					Right.UpdateFrequency();

					Frequency = Left.Frequency + Right.Frequency;
				}
			}
			#endregion
		}
	}
}
