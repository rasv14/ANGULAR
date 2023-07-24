using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace canvasApiTest
{
	class Program
	{
		static void Main(string[] args)
		{
			clsApiTest apiTest = new clsApiTest(args);

			apiTest.execute();
			while (apiTest.working)
			{
				Thread.Sleep(100);
			}
			Console.WriteLine("Testing complete.");
			Console.ReadKey();
		}
	}
}
