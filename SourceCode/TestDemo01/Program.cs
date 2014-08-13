using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unv.AdaptiveHuffmanLib;


namespace TestDemo01
{
	class Program
	{
		static void Main(string[] args)
		{
			bool test1Passed = RunTestBatch01();
		}

		/// <summary>
		/// This is going to run tests on writing to and reading from an empty huffman file.
		/// </summary>
		static bool RunTestBatch01()
		{
			bool worked = true;


			string testFilePath = "testFile1.huf";

			// Create the empty huff file
			using (HuffmanFileWriter writer = new HuffmanFileWriter(testFilePath))
			{
			}

			Thread.Sleep(20);


			// Test the single character read method
			using (HuffmanFileReader reader1 = new HuffmanFileReader(testFilePath))
			{
				char character = reader1.Read();

				if (character != FileReaderWriterBase.EOF_CHARACTER)
					worked = false;

				if (!reader1.EndOfFile)
					worked = false;
			}

			Thread.Sleep(20);


			// Test the read to char buffer method
			using (HuffmanFileReader reader2 = new HuffmanFileReader(testFilePath))
			{
				char[] outputBuffer = new char[15];

				int charactersRead = reader2.Read(outputBuffer, 0, 5);
				if (charactersRead != 0)
					worked = false;

				if (!reader2.EndOfFile)
					worked = false;
			}

			Thread.Sleep(20);


			// Test the ReadLine method
			using (HuffmanFileReader reader3 = new HuffmanFileReader(testFilePath))
			{
				string s = reader3.ReadLine();

				if (string.Empty != s)
					worked = false;

				if (!reader3.EndOfFile)
					worked = false;
			}

			Thread.Sleep(20);


			// Test the ReadToEnd method
			using (HuffmanFileReader reader4 = new HuffmanFileReader(testFilePath))
			{
				string s = reader4.ReadToEnd();

				if (string.Empty != s)
					worked = false;

				if (!reader4.EndOfFile)
					worked = false;
			}


			return worked;
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
