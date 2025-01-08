using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ParsingTool
{
	static public class ZipHelper
	{
		
		public static void FindAllZipFilePath(string sourcePath)
		{
			// 
			if (!Directory.Exists(sourcePath))
			{
				Console.WriteLine("目录不存在: " + sourcePath);
				return;
			}

			// 調用遞迴方法遍歷所有子目錄
			TraverseDirectory(sourcePath);
		}


		private static void TraverseDirectory(string currentDir)
		{
			
			string[] zipFiles = Directory.GetFiles(currentDir, "*.zip");
			foreach(string file in zipFiles)
			{
				Console.WriteLine($"{file}\n");
			}


			string[] subDirectories = Directory.GetDirectories(currentDir);
			foreach (string subdirectory in subDirectories)
			{
				// 递归调用，遍历每个子目录
				TraverseDirectory(subdirectory);
			}
		}



	}
}
