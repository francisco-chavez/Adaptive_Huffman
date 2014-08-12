using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public sealed class HuffmanFileWriter
		: FileReaderWriterBase
	{
		#region Constructors
		private HuffmanFileWriter()
		{
			NewLine = Environment.NewLine;
		}

		public HuffmanFileWriter(string filePath, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.ReadWrite)
			: this()
		{
			FileStream fileStream = File.Open(filePath, fileMode, fileAccess);

			if (!fileStream.CanWrite)
				throw new ArgumentException("Writing permissions were not given for file.");

			_fileStream = fileStream;
		}

		public HuffmanFileWriter(FileStream outputStream)
			: this()
		{
			if (outputStream == null)
				throw new ArgumentNullException("No Stream was given to the Huffman File Writer.");

			if (!outputStream.CanWrite)
				throw new ArgumentException("The Stream given to the Huffman File Writer does not support writing permissions.");

			_fileStream = outputStream;
		}

		~HuffmanFileWriter()
		{
			Dispose();
		}
		#endregion


		#region Attributes
		private const int	DEFAULT_PREFERED_BUFFER_SIZE	= 32;

		private List<bool> _bitBuffer = new List<bool>(DEFAULT_PREFERED_BUFFER_SIZE * 8);
		#endregion


		#region Properties
		/// <summary>
		/// This character is used to mark the end of the text to be encoded. If your 
		/// text includes this character, it is recomended to use an escape sequence 
		/// that doesn't contain this character in any text you write. Any text after 
		/// the EOF character will be considered bit filler for making sure the file 
		/// uses the proper number of bits in each byte.
		/// </summary>
		public string EOF { get { return EOF_CHARACTER.ToString(); } }

		/// <summary>
		/// Gets or sets the number of bytes that should be used held in the buffer.
		/// When the buffer starts holding a number of bytes that is greater than this
		/// amount, the buffer will flush any complete bytes into the output stream.
		/// </summary>
		public int PreferedBufferSize
		{
			get { return n_preferedBufferSize; }
			set
			{
				if (value == n_preferedBufferSize)
					return;

				n_preferedBufferSize = value;

				// 1 is the smallest buffer size I'll allow.
				if (value < 1)
					n_preferedBufferSize = 1;

				// If the wanted buffer size was reduced, then we might
				// have more data in it than what the user wants us to
				// have.
				if (ShouldFlush)
					Flush();
			}
		}
		private int n_preferedBufferSize = DEFAULT_PREFERED_BUFFER_SIZE;

		private bool ShouldFlush
		{
			get { return _bitBuffer.Count / 8 >= PreferedBufferSize; }
		}

		public string NewLine
		{
			get { return n_newLine; }
			set { n_newLine = (value == null) ? Environment.NewLine : value; }
		}
		private string n_newLine;
		#endregion


		#region Methods
		/// <summary>
		/// This method will encode and write the given string to the output stream.
		/// </summary>
		/// <param name="formatString">A composite format string.</param>
		/// <param name="inputObjects">An object to write using format.</param>
		public void Write(string formatString, params object[] inputObjects)
		{
			DisposeCheck();
			string input = string.Format(formatString, inputObjects);
			Write(input);
		}

		/// <summary>
		/// This method will encode and write the given string to the output stream.
		/// </summary>
		/// <param name="inputString"></param>
		public void Write(string inputString)
		{
			DisposeCheck();

			// I'll be using a single list getting the encoded bits to reduce
			// the number of array allocations by a bit.
			List<bool> characterBits = new List<bool>();

			// Loop through each character we need to write.
			for (int i = 0; i < inputString.Length; i++)
			{
				// Encode the current character.
				var inputCharacter		= inputString[i];
				var encodedCharacter	= _huffmanTree.EncodeCharacter(inputCharacter);

				// Move the encoded character bits to the buffer.
				for (int j = 0; j < encodedCharacter.Length; j++)
					characterBits.Add(encodedCharacter[j]);

				_bitBuffer.AddRange(characterBits);
				characterBits.Clear();

				// Flush the buffer if we've gone past the number bytes
				// the user wants us to use for the buffer.
				if (ShouldFlush)
					Flush();
			}
		}

		/// <summary>
		/// This method will encode and write the given string to a new line in the output stream.
		/// </summary>
		/// <param name="formatString">A composite format string.</param>
		/// <param name="inputObjects">An object to write using format.</param>
		public void WriteLine(string formatString, params object[] inputObjects)
		{
			DisposeCheck();
			string input = string.Format(formatString, inputObjects);
			WriteLine(input);
		}

		/// <summary>
		/// This method will encode and write the given string to a new line in the output stream.
		/// </summary>
		/// <param name="inputString"></param>
		public void WriteLine(string inputString)
		{
			DisposeCheck();
			Write(inputString);
			Write(NewLine);
		}

		/// <summary>
		/// This method will flush any complete bytes in the buffer to the output stream.
		/// </summary>
		public void Flush()
		{
			int byteCount = _bitBuffer.Count / 8;

			if (byteCount == 0)
				return;

			byte[]		bytes			= new byte[byteCount];
			var			flushableBits	= _bitBuffer.Take(byteCount * 8);
			BitArray	bitCompressor	= new BitArray(flushableBits.ToArray());
			
			bitCompressor.CopyTo(bytes, 0);
			_bitBuffer.RemoveRange(0, byteCount * 8);
			_fileStream.Write(bytes, 0, byteCount);
			_fileStream.Flush();
		}

		public override void Dispose()
		{
			if (_isDisposed)
				return;

			// Encode the end of file character, so that the reader will
			// know when it has reached the end encoded portion of the file.
			Write(EOF);

			// Add fake bits to the end of the buffer so that we can evenly
			// fill out each the last byte.
			while (_bitBuffer.Count % 8 != 0)
				_bitBuffer.Add(true);

			// Flush out anything that's left.
			Flush();

			// Clear out the output stream and the huffman tree.
			_fileStream.Dispose();
			_huffmanTree.ClearTree();

			// Free any items we are holding so that the garbage collector
			// can do its job.
			_bitBuffer		= null;
			_huffmanTree	= null;
			_fileStream		= null;

			// Mark this item as disposed.
			_isDisposed			= true;
		}

		private void DisposeCheck()
		{
			if (_isDisposed)
				throw new InvalidOperationException("This HuffmanFileWriter is no longer able to write.");
		}
		#endregion
	}
}
