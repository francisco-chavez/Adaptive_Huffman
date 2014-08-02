using System;
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
		private bool ContainsCharacter(char character)
		{
			bool		result		= false;
			TreeNode	currentNode = _head.Next;
			
			while (currentNode != _tail)
			{
				if (currentNode.IsEmpty)
				{
					// The empty node doesn't affect the result of this
					// search, so we're not doing anything specific with
					// the empty node.
				}
				else if (currentNode.Character == character)
				{
					result = true;
					break;
				}

				currentNode = currentNode.Next;
			}

			return result;
		}
		#endregion

		private class TreeNode
		{
			#region Attributes
			private char _character;
			private bool _isEmpty	= true;
			#endregion


			#region Properties
			public TreeNode Next		{ get; set; }
			public TreeNode Prev		{ get; set; }
											   	    
			public TreeNode Left		{ get; set; }
			public TreeNode Right		{ get; set; }
			public TreeNode Root		{ get; set; }

			public int		Frequency	{ get; set; }
			public bool		IsEmpty		{ get { return _isEmpty; } }

			public char Character
			{
				get { return _character; }
				set
				{
					_character	= value;
					_isEmpty	= false;
				}
			}
			#endregion
		}
	}
}
