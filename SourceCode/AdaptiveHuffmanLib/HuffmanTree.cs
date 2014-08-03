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
			BitArray result = null;
			List<bool> characterBits = new List<bool>(8);

			TreeNode characterNode = FindCharacterNode(character);

			bool alreadyPresent = ContainsCharacter(character);

			if (!alreadyPresent)
				AddCharacter(character);
			else
				IncreamentCharacter(character);

			_root.UpdateFrequency();

			BalanceTree();
			return result;
		}

		private TreeNode FindCharacterNode(char character)
		{
			TreeNode currentNode = _tail.Prev;

			while (true)
			{
				if (currentNode.IsLeaf)
				{
					if (currentNode.IsEmpty || currentNode.Character == character)
						return currentNode;
				}

				currentNode = currentNode.Prev;
			}
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

		private void AddCharacter(char character)
		{
			// The empty node will become a branch node, so the
			// temp reference we're using is called newBranch.
			// Besides, the left child of the newBranch will become
			// the empty node.
			TreeNode newBranch = EmptyNode;
			newBranch.Left	= new TreeNode();
			newBranch.Right = new TreeNode();

			newBranch.Left.Root			= newBranch;
			newBranch.Right.Right		= newBranch;

			newBranch.Right.Character	= character;
			newBranch.Right.Frequency	= 1;

			newBranch.Left.Next			= newBranch.Right;
			newBranch.Left.Prev			= newBranch.Prev;

			newBranch.Right.Next		= newBranch;
			newBranch.Right.Prev		= newBranch.Left;

			newBranch.Prev	= newBranch.Right;

			TreeNode parent = newBranch;
			while (parent != null)
			{
				parent.Frequency = parent.Left.Frequency + parent.Right.Frequency;
				parent = parent.Root;
			}
		}

		private void IncreamentCharacter(char character)
		{
			TreeNode currentNode = _tail.Prev;

			while (true)
			{
				if (currentNode.IsLeaf && currentNode.Character == character)
					break;

				currentNode = currentNode.Prev;
			}

			currentNode.Frequency++;
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

			public TreeNode Root		{ get; set; }
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
				this.Root		= null;
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
