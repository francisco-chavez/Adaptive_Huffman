using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unv.AdaptiveHuffmanLib;


namespace TestDemo01
{
	class Program
	{
		static void Main(string[] args)
		{
			HuffmanTree tree = new HuffmanTree();
			string testInput = "mississippi river";
			BitArray encodedCharacter;
			string[] treeInfo;

			treeInfo = tree.GetTestReadout();

			for (int i = 0; i < testInput.Length; i++)
			{
				encodedCharacter = tree.InsertCharacter(testInput[i]);
				treeInfo = tree.GetTestReadout();
			}
		}
	}
}
