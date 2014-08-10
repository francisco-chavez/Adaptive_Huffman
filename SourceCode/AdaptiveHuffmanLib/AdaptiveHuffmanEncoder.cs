using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unv.AdaptiveHuffmanLib
{
	public class AdaptiveHuffmanEncoder
		: IDisposable
	{
		#region Constructors
		~AdaptiveHuffmanEncoder()
		{
			if (!_isDisposed)
				this.Dispose();
		}
		#endregion


		#region Attributes
		private HuffmanTree _encoder	= new HuffmanTree();
		private List<bool>	_bitBuffer	= new List<bool>(32);
		private bool		_isDisposed = false;
		#endregion


		#region Properties
		#endregion


		#region Methods
		private void Encode(IEnumerable<char> characters)
		{
			foreach (var character in characters)
			{
				var encodedCharacter = _encoder.EncodeCharacter(character);
				for (int i = 0; i < encodedCharacter.Length; i++)
					_bitBuffer.Add(encodedCharacter[i]);
			}
		}

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

			// ToDo: Move the data in bytes into the encoder's output object;
			throw new NotImplementedException();
		}

		private void FlushAllBits()
		{
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
