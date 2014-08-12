using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public class HuffmanFileReader
		: IDisposable
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
		internal const char EOF_CHARACTER = HuffmanFileWriter.EOF_CHARACTER;

		private bool _isDisposed = false;
		private HuffmanTree _characterDecoder = new HuffmanTree();
		private FileStream _input;
		#endregion


		#region Properties
		public bool EndOfFile { get { throw new NotImplementedException(); } }

		public string NewLine
		{
			get { return n_newLine; }
			set { n_newLine = (value == null) ? Environment.NewLine : value; }
		}
		private string n_newLine;
		#endregion


		#region Methods
		public char Read()
		{
			throw new NotImplementedException();
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

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
