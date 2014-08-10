using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
			string testInput = "mississippi river";
			testInput = testInput + "   " + testInput;

			//HuffmanTree tree = new HuffmanTree();
			//string[] treeInfo;

			//treeInfo = tree.GetTestReadout();
			//List<bool> encodedTestInput = new List<bool>();

			//for (int i = 0; i < testInput.Length; i++)
			//{
			//	char		inputCharacter		= testInput[i];
			//	BitArray	encodedCharacter	= tree.EncodeCharacter(inputCharacter);
			//	treeInfo = tree.GetTestReadout(true);

			//	Console.WriteLine("{0}: {1}", inputCharacter, ReadableBits(encodedCharacter));

			//	for (int j = 0; j < encodedCharacter.Length; j++)
			//		encodedTestInput.Add(encodedCharacter[j]);
			//}

			//HuffmanTree		encodeTree1				= new HuffmanTree();
			//HuffmanTree		decodeTree1				= new HuffmanTree();
			//StringBuilder	decodedStringBuilder	= new StringBuilder();

			//for (int i = 0; i < testInput.Length; i++)
			//{
			//	char		inputCharacter		= testInput[i];
			//	BitArray	encodedCharacter	= encodeTree1.EncodeCharacter(inputCharacter);
			//	bool[]		encodedBits			= Enumerable.Range(0, encodedCharacter.Length).Select(j => { return encodedCharacter[j]; }).ToArray();
			//	char		characterFound		= decodeTree1.DecodeCharacters(encodedBits)[0];
			//	decodedStringBuilder.Append(characterFound);
			//}

			//string decodedTestString = decodedStringBuilder.ToString();

			//if (testInput == decodedTestString)
			//{
			//	Console.WriteLine("It works");
			//}
			//else
			//{
			//	Console.WriteLine("Keep working at it.");
			//}
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
