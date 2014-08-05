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
		}
		#endregion


		#region Methods
		public BitArray InsertCharacter(char character)
		{
			TreeNode	characterNode	= FindCharacterNode(character);
			bool[]		encodedBits		= GetCharacterBits(characterNode, character);

			UpdateTree(characterNode, character);

			throw new NotImplementedException();
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
		private bool[] GetCharacterBits(TreeNode characterNode, char character)
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
			if (characterNode.IsEmpty)
			{
				byte[] charBytes = Encoding.Unicode.GetBytes(new char[] { character });
				BitArray bitArray = new BitArray(charBytes);
				for (int i = 0; i < bitArray.Length; i++)
					bits.Add(bitArray[i]);
			}

			return bits.ToArray();
		}

		private void UpdateTree(TreeNode characterNode, char character)
		{
			if (characterNode.IsEmpty)
				characterNode = AddCharacterNode(character);
			else
				characterNode.Frequency++;

			throw new NotImplementedException();
		}

		public bool ContainsCharacter(char character)
		{
			// The most frequent character is going to be  towards
			// the end of the linked list. So, if we start at the
			// back, and moves towards the front, we will find is
			// sooner (on average).
			bool		result		= false;
			TreeNode	currentNode = _tail.Prev;

			while (currentNode != _head)
			{
				if (currentNode.IsEmpty || !currentNode.IsLeaf)
				{
					// If the current node isn't an empty leaf node,
					// then we don't want it to be marked down.
				}
				else if (currentNode.Character == character)
				{
					result = true;
					break;
				}

				currentNode = currentNode.Prev;
			}

			return result;
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
			newBranch.Right.Frequency	= 1;

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

		private void SwitchChildNodes(TreeNode parent)
		{
			TreeNode start	= parent.Left.Prev;
			TreeNode end	= parent.Right.Next;
			TreeNode temp	= parent.Left;

			// Switch childs nodes in tree
			parent.Left = parent.Right;
			parent.Right = temp;

			//
			// Re-arrange nodes in Linked list
			//

			// Connect starting node to left node
			start.Next = parent.Left;
			parent.Left.Prev = start;

			// Connect left node to right node
			parent.Left.Next = parent.Right;
			parent.Right.Prev = parent.Left;

			// Connect right node to ending node
			parent.Right.Next = end;
			end.Prev = parent.Right;
		}

		private void BalanceTree()
		{
			throw new NotImplementedException();
		}
		#endregion


		private class TreeNode
		{
			#region Properties
			public TreeNode Next		{ get; set; }
			public TreeNode Prev		{ get; set; }

			public TreeNode Parent		{ get; set; }
			public TreeNode Left		{ get; set; }
			public TreeNode Right		{ get; set; }

			public bool		IsLeaf		{ get { return Right == null && Left == null; } }

			public int		Frequency	{ get; set; }
			public bool		IsEmpty		{ get { return IsLeaf && Frequency == 0; } }

			public char		Character	{ get; set; }
			#endregion


			#region Constructors
			public TreeNode()
			{
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
