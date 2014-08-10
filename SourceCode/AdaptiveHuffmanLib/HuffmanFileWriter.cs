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
		public HuffmanFileWriter(string filePath, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.ReadWrite)
		{
			FileStream fileStream = File.Open(filePath, fileMode, fileAccess);

			if (!fileStream.CanWrite)
				throw new ArgumentException("Writing permissions were not given for file.");

			_output = fileStream;
		}

		public HuffmanFileWriter(FileStream outputStream)
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
		private const int	DEFAULT_PREFERED_BUFFER_SIZE = 32;

		private bool		_isDisposed			= false;
		private HuffmanTree _characterEncoder	= new HuffmanTree();
		private List<bool>	_bitBuffer			= new List<bool>(DEFAULT_PREFERED_BUFFER_SIZE * 8);

		private FileStream	_output;
		#endregion


		#region Properties
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

				if (_bitBuffer.Count / 8 > n_preferedBufferSize)
					Flush();
			}
		}
		private int n_preferedBufferSize = DEFAULT_PREFERED_BUFFER_SIZE;
		#endregion


		#region Methods
		public void Flush()
		{
			int byteCount = _bitBuffer.Count / 8;

			if (byteCount == 0)
				return;

			byte[] bytes = new byte[byteCount];
			var flushableBits = _bitBuffer.Take(byteCount * 8);
			BitArray bitCompressor = new BitArray(flushableBits.ToArray());
			bitCompressor.CopyTo(bytes, 0);

			_bitBuffer.RemoveRange(0, byteCount * 8);

			_output.Write(bytes, 0, byteCount);
			_output.Flush();
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

			throw new NotImplementedException();
		}
		#endregion
	}
}
