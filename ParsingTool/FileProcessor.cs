using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ParsingTool.myInterface;
using System.Threading;

namespace ParsingTool
{
	class FileProcessor
	{
		private readonly IFileService _fileService;
		private readonly IJavaExecuter _javaExecutor;

		private readonly IDirectoryService _directoryService;

		public FileProcessor(IFileService fileService, IJavaExecuter javaExecutor, IDirectoryService directoryService)
		{
			_fileService = fileService;
			_javaExecutor = javaExecutor;
			_directoryService = directoryService;
		}

		public void ProcessFiles(string sourceDir, string javaExecutable, string outputDir)
		{
			object locker = new object();
			while (true)
			{
				// 1. 檢查是否有 .txt 檔案
				var txtFiles = _fileService.GetTxtFiles(sourceDir);
				if (!txtFiles.Any())
				{
					Console.WriteLine("Source 資料夾無 .txt 檔案，結束程序。");
					//break;
					return;
				}

				// 2. 取出前 10 筆檔案移到 Java 執行檔所在目錄。如果不足10筆，取所有檔案
				int count = 10;
				if (txtFiles.Count() < 10)
				{
					count = txtFiles.Count();
				}

				var filesToMove = txtFiles.Take(count).ToList();

				_fileService.MoveFiles(sourceDir, Path.GetDirectoryName(javaExecutable), filesToMove);


				// 3. 執行 Java 程式
				_javaExecutor.RunJavaExecutable(javaExecutable);

				// 4. 刪除 Java 執行檔目錄下 outputFile資料夾中 不符合格式的txt檔案
				string outputFileFolder = Path.Combine(Path.GetDirectoryName(javaExecutable), "outputFile");
				string datePattern = @"^MEC_\d{4}_\d{2}_\d{2}T\d{2}_\d{2}\.txt$";


				_directoryService.DeleteUnmatchFiles(outputFileFolder, datePattern);            ///要完全 Match  pattern
				


				// 5. 移動 outputFile 目錄中的txt 檔案到 target 目錄

				_directoryService.MoveAllFiles(outputFileFolder, outputDir);



				// 6. 清理 Java 目錄的 txt 檔案
				lock (locker)
				{
					_fileService.DeleteFiles(Path.GetDirectoryName(javaExecutable), "*.txt");
				}

			}
		}
	}
}
