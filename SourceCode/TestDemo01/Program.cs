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
			testInput = testInput + "   " + testInput;
			BitArray encodedCharacter;
			string[] treeInfo;

			treeInfo = tree.GetTestReadout();
			List<bool> encodedTestInput = new List<bool>();

			for (int i = 0; i < testInput.Length; i++)
			{
				char inputCharacter = testInput[i];
				encodedCharacter = tree.InsertCharacter(inputCharacter);
				treeInfo = tree.GetTestReadout(true);

				Console.WriteLine("{0}: {1}", inputCharacter, ReadableBits(encodedCharacter));

				for (int j = 0; j < encodedCharacter.Length; j++)
					encodedTestInput.Add(encodedCharacter[j]);
			}
		}

		static string ReadableBits(BitArray bitArray)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < bitArray.Length; i++)
				builder.Append(bitArray[i] ? '1' : '0');

			return builder.ToString();
		}
	}
}
