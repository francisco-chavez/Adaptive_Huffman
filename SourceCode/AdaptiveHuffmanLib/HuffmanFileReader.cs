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
		public HuffmanFileReader(string filePath, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read)
			: base()
		{
			FileStream fileStream = File.Open(filePath, fileMode, fileAccess);

			if (!fileStream.CanRead)
				throw new ArgumentException("Reading permissions were not given for file.");

			_fileStream = fileStream;
		}

		public HuffmanFileReader(FileStream inputStream)
			: base()
		{
			if (inputStream == null)
				throw new ArgumentNullException("No Stream was given to read from.");

			if (!inputStream.CanRead)
				throw new ArgumentException("Reading permissions were not given for the FileStream.");

			_fileStream = inputStream;
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
		/// <summary>
		/// This property will return a true value when the end of the file has
		/// been reached.
		/// </summary>
		public bool EndOfFile { get { return _hasReachedEndOfFile; } }
		#endregion


		#region Methods
		/// <summary>
		/// This method will read the next character from the given file.
		/// </summary>
		/// <remarks>
		/// This is the only public read method that may return the End Of File character; this
		/// should only happen on the first read of an empty Huffman file.
		/// </remarks>
		public char Read()
		{
			// Run the standard pre-read checks.
			PreReadCheck();

			// Make sure we have at least two characters in the buffer.
			while (_characterBuffer.Count < 2)
			{
				// If the file was empty, grabing two characters will not be possible.
				// This will let us exit the loop when this happens.
				if(_characterBuffer.Count != 0 && _characterBuffer.Peek() == EOF_CHARACTER)
					break;

				ReadFromStream();
			}

			// Grab the first character in the buffer.
			char result = _characterBuffer.Dequeue();

			// Check for end of file.
			if ((result == EOF_CHARACTER) || (_characterBuffer.Peek() == EOF_CHARACTER))
				_hasReachedEndOfFile = true;

			// Return the character that was read.
			return result;
		}

		/// <summary>
		/// This method will read characters from the file and insert them into the buffer. The
		/// insertion will start at the given index; it will not exceed the given count.
		/// </summary>
		/// <param name="buffer">This is where the read characters will be inserted.</param>
		/// <param name="index">This tells us where to start inserting characters in the <i>buffer</i>.</param>
		/// <param name="count">This tells us the max number of characters to insert into the <i>buffer.</i>.</param>
		/// <returns>The number of characters that were inserted into the buffer.</returns>
		public int Read(char[] buffer, int index, int count)
		{
			// Run the standard pre-read checks.
			PreReadCheck();

			if (index < 0 || index >= buffer.Length)
				throw new IndexOutOfRangeException();

			int charactersRead = 0;
			for (; (charactersRead < count) && ((index + charactersRead) < buffer.Length) && (!EndOfFile); charactersRead++)
			{
				char character = Read();

				if(character == EOF_CHARACTER)
					break;

				buffer[charactersRead + index] = character;
			}

			return charactersRead;
		}

		/// <summary>
		/// Returns the next line of text from the file.
		/// </summary>
		public string ReadLine()
		{
			// Run the standard pre-read checks.
			PreReadCheck();

			string newLine = NewLine;
			int endingLength = newLine.Length;

			StringBuilder builder = new StringBuilder();

			while (true)
			{
				// Stop reading text when we reach the end of the file.
				if (EndOfFile)
					break;

				// Add the next character.
				builder.Append(Read());

				// Run a check for the end of line string.
				// If found remove it from the string builder and stop reading the file (for now).
				if (builder.Length >= endingLength)
				{
					// In order to prove that the end of the line has not been reached,
					// we are goning to say that is has been reached.
					bool endOfLineReached = true;

					// Since the end of the line has been reached, then the last characters of the
					// line will match up with the NewLine string.
					for (int i = 0; i < endingLength; i++)
						if (builder[builder.Length - (i + 1)] != newLine[endingLength - (i + 1)])
						{
							// Oh look, one of the characters doesn't match, I guess that means
							// our assumption that then end of the line has been reached was 
							// wrong
							endOfLineReached = false;
							break;
						}

					// Now that we know for sure that the end of the line has been reached,
					// we will remove it from the results and stop reading.
					if (endOfLineReached)
					{
						builder.Remove(builder.Length - endingLength, endingLength);
						break;
					}
				}
			}

			// If the first characer is the End Of File character, then we shouldn't be 
			// returning anything.
			if (builder.Length == 0 || builder[0] == EOF_CHARACTER)
				return string.Empty;

			// Return our results.
			return builder.ToString();
		}

		/// <summary>
		/// This method returns the entire contents of the file in a single string object.
		/// </summary>
		public string ReadToEnd()
		{
			// Run the standard pre-read checks.
			PreReadCheck();

			StringBuilder builder = new StringBuilder();

			while (!EndOfFile)
				builder.Append(Read());

			// Making sure we don't return anything beyound the EOF character (or the EOF character).
			if (builder.Length == 0 || EOF_CHARACTER == builder[0])
				return string.Empty;

			if (builder.Length != 0 && builder[builder.Length - 1] == EOF_CHARACTER)
				builder.Remove(builder.Length - 1, 1);

			return builder.ToString();
		}

		/// <summary>
		/// This method will decode characters from the file stream with the
		/// help of the HuffmanTree and place them into the character buffer.
		/// </summary>
		private void ReadFromStream()
		{
			// Read the bytes from the file stream
			byte[] bufferBytes = new byte[32];
			int bytesRead = _fileStream.Read(bufferBytes, 0, 32);

			// Split the bytes into their individual bits for
			// file decoding.
			BitArray bufferBits = new BitArray(bufferBytes.Take(bytesRead).ToArray());
			bool[] readableBits = new bool[bufferBits.Count];

			// Adding a check that should help with files that are truely empty.
			if (bytesRead == 0)
			{
				_characterBuffer.Enqueue(EOF_CHARACTER);
				return;
			}

			for (int i = 0; i < bytesRead * 8; i++)
				readableBits[i] = bufferBits[i];

			// Decode the bits into characters
			char[] readCharacters = _huffmanTree.DecodeCharacters(readableBits);

			// Move the characters into the buffer.
			for (int i = 0; i < readCharacters.Length; i++)
				_characterBuffer.Enqueue(readCharacters[i]);
		}

		/// <summary>
		/// This method runs a series of checks at the beginning of a Read method before doing any
		/// reading. If any of these checks fail, an appropriate execption will be thrown.
		/// </summary>
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
