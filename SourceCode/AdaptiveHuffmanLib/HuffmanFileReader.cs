using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public sealed class HuffmanFileReader
		: FileReaderWriterBase
	{
		#region Constructors
		private HuffmanFileReader()
		{
		}

		public HuffmanFileReader(string filePath, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read)
			: this()
		{
			throw new NotImplementedException();
		}

		public HuffmanFileReader(FileStream inputStream)
		{
			throw new NotImplementedException();
		}

		~HuffmanFileReader()
		{
			Dispose();
		}
		#endregion


		#region Attributes
		private Queue<char> _characterBuffer		= new Queue<char>();
		private bool		_hasReachedEndOfFile	= false;
		#endregion


		#region Properties
		public bool EndOfFile { get { return _hasReachedEndOfFile; } }
		#endregion


		#region Methods
		public char Read()
		{
			PreReadCheck();

			while (_characterBuffer.Count < 2)
				ReadFromStream();

			char result = _characterBuffer.Dequeue();

			if (_characterBuffer.Peek() == EOF_CHARACTER)
				_hasReachedEndOfFile = true;

			return result;
		}

		public int Read(char[] buffer, int index, int count)
		{
			PreReadCheck();

			if (index < 0 || index >= buffer.Length)
				throw new IndexOutOfRangeException();

			int charactersRead = 0;
			for (; (charactersRead < count) && ((index + charactersRead) < buffer.Length) && (!EndOfFile); charactersRead++)
				buffer[charactersRead + index] = Read();

			return charactersRead;
		}

		public string ReadLine()
		{
			PreReadCheck();

			string newLine = NewLine;
			int endingLength = newLine.Length;

			StringBuilder builder = new StringBuilder();

			while (true)
			{
				if (EndOfFile)
					break;

				builder.Append(Read());

				if (builder.Length >= endingLength)
				{
					bool endOfLineReached = true;
					for (int i = 0; i < endingLength; i++)
						if (builder[builder.Length - (i + 1)] != newLine[endingLength - (i + 1)])
						{
							endOfLineReached = false;
							break;
						}

					if (endOfLineReached)
					{
						builder.Remove(builder.Length - (1 + endingLength), endingLength);
						break;
					}
				}
			}

			return builder.ToString();
		}

		public string ReadToEnd()
		{
			PreReadCheck();

			StringBuilder builder = new StringBuilder();
			
			while (!EndOfFile)
				builder.Append(Read());

			return builder.ToString();
		}

		private void ReadFromStream()
		{
			byte[] bufferBytes = new byte[32];
			_fileStream.Read(bufferBytes, 0, 32);
			BitArray bufferBits = new BitArray(bufferBytes);
			bool[] readableBits = new bool[bufferBits.Count];

			for (int i = 0; i < bufferBits.Count; i++)
				readableBits[i] = bufferBits[i];
			char[] readCharacters = _huffmanTree.DecodeCharacters(readableBits);

			for (int i = 0; i < readCharacters.Length; i++)
				_characterBuffer.Enqueue(readCharacters[i]);
		}

		private void PreReadCheck()
		{
			if (_isDisposed)
				throw new ObjectDisposedException("Reading is not allowed once the HuffmanFileReader has been disposed.");
			if (EndOfFile)
				throw new NotSupportedException("Reading is not supported once the end of the file has been reached.");
		}

		public override void Dispose()
		{
			if (_isDisposed)
				return;

			_fileStream.Dispose();
			_characterBuffer.Clear();
			_huffmanTree.ClearTree();

			_fileStream			= null;
			_characterBuffer	= null;
			_huffmanTree		= null;

			_isDisposed = true;
		}
		#endregion
	}
}
