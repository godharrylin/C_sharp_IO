using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ParsingTool.myInterface;

namespace ParsingTool
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("請輸入 Source Dir 路徑：");
			string sourceDirectory = Console.ReadLine();

			Console.WriteLine("請輸入 Java 執行檔路徑：");
			string javaExecutable = Console.ReadLine();

			Console.WriteLine("請輸入 Output Dir 路徑：");
			string outputDirectory = Console.ReadLine();

			try
			{
				// 建立依賴
				IFileService fileService = new FilesService();
				IJavaExecuter javaExecutor = new JavaExecuter();
				IDirectoryService directoryService = new DirectoryService();

				// 啟動處理程序
				var fileProcessor = new FileProcessor(fileService, javaExecutor, directoryService);
				fileProcessor.ProcessFiles(sourceDirectory, javaExecutable, outputDirectory);

				Console.WriteLine("所有資料處理完成。");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"執行過程中發生錯誤: {ex.Message}");
			}



			Console.ReadLine();
		}

	}
}
