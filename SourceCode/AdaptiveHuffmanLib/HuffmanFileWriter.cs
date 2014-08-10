using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public class HuffmanFileWriter
		: IDisposable
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

			_output = fileStream;
		}

		public HuffmanFileWriter(FileStream outputStream)
			: this()
		{
			if (outputStream == null)
				throw new ArgumentNullException("No Stream was given to the Huffman File Writer.");

			if (!outputStream.CanWrite)
				throw new ArgumentException("The Stream given to the Huffman File Writer does not support writing permissions.");

			_output = outputStream;
		}

		~HuffmanFileWriter()
		{
			Dispose();
		}
		#endregion


		#region Attributes
		private const int	DEFAULT_PREFERED_BUFFER_SIZE	= 32;
		internal const char EOF_CHARACTER					= (char) 3;

		private bool		_isDisposed			= false;
		private HuffmanTree _characterEncoder	= new HuffmanTree();
		private List<bool>	_bitBuffer			= new List<bool>(DEFAULT_PREFERED_BUFFER_SIZE * 8);

		private FileStream	_output;
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

		public int PreferedBufferSize
		{
			get { return n_preferedBufferSize; }
			set
			{
				if (value == n_preferedBufferSize)
					return;

				n_preferedBufferSize = value;

				if (value < 1)
					n_preferedBufferSize = DEFAULT_PREFERED_BUFFER_SIZE;

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
		public void Write(string formatString, params object[] inputObjects)
		{
			string input = string.Format(formatString, inputObjects);
			Write(input);
		}

		public void Write(string inputString)
		{
			List<bool> characterBits = new List<bool>();

			for (int i = 0; i < inputString.Length; i++)
			{
				var inputCharacter = inputString[i];
				var encodedCharacter = _characterEncoder.EncodeCharacter(inputCharacter);

				for (int j = 0; j < encodedCharacter.Length; j++)
					characterBits.Add(encodedCharacter[j]);

				_bitBuffer.AddRange(characterBits);
				characterBits.Clear();

				if (ShouldFlush)
					Flush();
			}
		}

		public void WriteLine(string formatString, params object[] inputObjects)
		{
			string input = string.Format(formatString, inputObjects);
			WriteLine(input);
		}

		public void WriteLine(string inputString)
		{
			Write(inputString);
			Write(NewLine);
		}

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
			_output.Write(bytes, 0, byteCount);
			_output.Flush();
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

			Write(EOF);
			while (_bitBuffer.Count % 8 != 0)
				_bitBuffer.Add(true);

			Flush();
			_output.Dispose();
		}
		#endregion
	}
}
