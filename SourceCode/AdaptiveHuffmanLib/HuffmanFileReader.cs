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
			while (_characterBuffer.Count < 2)
				ReadFromStream();

			char result = _characterBuffer.Dequeue();

			if (_characterBuffer.Peek() == EOF_CHARACTER)
				_hasReachedEndOfFile = true;

			return result;
		}

		public int Read(char[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		public string ReadLine()
		{
			throw new NotImplementedException();
		}

		public string ReadToEnd()
		{
			throw new NotImplementedException();
		}

		private void ReadFromStream()
		{
			byte[] bufferBytes = new byte[32];
			_fileStream.Read(bufferBytes, 0, 32);
			BitArray  bufferBits = new BitArray(bufferBytes);
			bool[] readableBits = new bool[bufferBits.Count];

			for (int i = 0; i < bufferBits.Count; i++)
				readableBits[i] = bufferBits[i];
			char[] readCharacters = _huffmanTree.DecodeCharacters(readableBits);

			for (int i = 0; i < readCharacters.Length; i++)
				_characterBuffer.Enqueue(readCharacters[i]);
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
