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
			bool test1Passed = RunTest1();
		}

		static bool RunTest1()
		{
			bool result = true;



			return result;
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
