using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDemo01
{
	class Program
	{
		static void Main(string[] args)
		{
			byte[] charBytes = Encoding.Unicode.GetBytes(new char[] { 'd' });
			BitArray bitArray = new BitArray(charBytes);

		}
	}
}
