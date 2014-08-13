using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public abstract class FileReaderWriterBase
		: IDisposable
	{
		#region Attributes
		protected const char EOF_CHARACTER = (char) 3;

		protected bool			_isDisposed		= false;
		protected HuffmanTree	_huffmanTree	= new HuffmanTree();
		protected FileStream	_fileStream		= null;
		#endregion


		#region Constructors
		protected FileReaderWriterBase()
		{
			NewLine = Environment.NewLine;
		}
		#endregion


		#region Properties
		public string NewLine
		{
			get { return n_newLine; }
			set { n_newLine = (value == null) ? Environment.NewLine : value; }
		}
		private string n_newLine;
		#endregion


		#region Methods
		/// <summary>
		/// This method will close and dispose of all resources used by this class instance.
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		public abstract void Dispose();
		#endregion
	}
}
