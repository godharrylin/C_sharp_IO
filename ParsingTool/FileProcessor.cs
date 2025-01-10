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
		private string _fileNamePattern;
		private string _srcfilePattern;
		private string _srcRoot;
		private string _tarRoot;
		private string _exeDir;
		private Dictionary<PARSING_TYPE, string> _exeDict = new Dictionary<PARSING_TYPE, string>();
		private List<string> _srcDir;
		private List<DirInfo> _pDirList = new List<DirInfo>();

		public FileProcessor(IFileService fileService, IJavaExecuter javaExecutor, IDirectoryService directoryService)
		{
			_fileService = fileService;
			_javaExecutor = javaExecutor;
			_directoryService = directoryService;
			_fileNamePattern = @"^MEC_\d{4}_\d{2}_\d{2}T\d{2}_\d{2}\.txt$";
			_srcfilePattern = @"d{4}_\d{2}_\d{2}T\d{2}_\d{2}\.txt$";
			_srcDir = new List<string>();
		}

		private void ProcessFiles(string sourceDir, string javaExecutable, string outputDir)
		{
			object locker = new object();
			while (true)
			{
				// 1. 檢查是否有 .txt 檔案且符合格式-------------------------- Not finished
				var txtFiles = _fileService.GetTxtFiles(sourceDir);
				if (!txtFiles.Any())
				{
					Console.WriteLine("Source 資料夾無 .txt 檔案，結束Java程式。");
					break;
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
				_directoryService.DeleteUnmatchFiles(outputFileFolder, _fileNamePattern);

				// 5. 移動 outputFile 目錄中的txt 檔案到 target 目錄

				_directoryService.MoveAllFiles(outputFileFolder, outputDir);



				// 6. 清理 Java 目錄的 txt 檔案
				lock (locker)
				{
					_fileService.DeleteFiles(Path.GetDirectoryName(javaExecutable), "*.txt");
				}

			}
			Console.WriteLine($"資料產生完畢: {outputDir}");
		}

		public void Setting(string sourceDir, string exeDir, string targetDir)
		{
			_srcRoot = sourceDir;
			_tarRoot = targetDir;
			_exeDir = exeDir;
			SetParsingFolderInfo(_srcRoot, _tarRoot);
			SetExePath(exeDir);

			//TraverseDir(_tarRoot);		// 查看target目錄的結構是否和source目錄一樣

			Console.WriteLine("Setting Finished");

		}

		public void Setting()
		{
			string srcDir = @"C:\Users\ASUS PC\Desktop\TraveseDir\Sample\Before";
			string exeDir = @"C:\Users\ASUS PC\Desktop\Work\java_build_Use";
			string tarDir = @"C:\Users\ASUS PC\Desktop\TraveseDir\Sample\After";
			Setting(srcDir, exeDir, tarDir);
		}

		public void Start()
		{
			foreach (DirInfo item in _pDirList)
			{
				if (item.Type != PARSING_TYPE.NONE)
				{
					ProcessFiles(item.AbsolutePath, _exeDict[item.Type], item.TargetPath);
				}
				else
				{
					CreateTarDirDependsOnSrcDir(item.TargetPath);
				}
			}
		}
		public List<string> TraverseDir(string path, int level = 0)
		{
			List<string> directories = new List<string>();
			Console.WriteLine(new string(' ', (level) * 3) + "|___" + Path.GetFileName(path));
			try
			{
				foreach (string Dir in Directory.GetDirectories(path))
				{
					directories.Add(Dir);
					directories.AddRange(TraverseDir(Dir, level + 1));
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Console.WriteLine($"無法訪問目錄: {path}. 錯誤: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"處理目錄時發生錯誤: {path}. 錯誤: {ex.Message}");
			}

			return directories;
		}

		private void SetParsingFolderInfo(string srcRoot, string tarRoot)
		{
			List<string> _srcDir = TraverseDir(srcRoot);
			foreach (string dir in _srcDir)
			{
				_pDirList.Add(new DirInfo(dir) { TargetPath = dir.Replace(srcRoot, tarRoot) });
			}
			Console.WriteLine("===============");
		}
		private void SetExePath(string exeDirPath)
		{
			_exeDict.Clear();
			string[] paths = Directory.GetFiles(exeDirPath, "*.exe");
			
			foreach (string path in paths)
			{
				if (_exeDict.Count == 2)
				{
					break;
				}

				string fileName = Path.GetFileName(path);
				if (fileName.Contains("Parameters"))
				{
					_exeDict.Add(PARSING_TYPE.PARAMETERS, path);
				}
				else
				{
					_exeDict.Add(PARSING_TYPE.EVENT, path);
				}
			}
		}

		private void CreateTarDirDependsOnSrcDir(string path)
		{
			try
			{
				path = path.Replace(_srcRoot, _tarRoot);
				Directory.CreateDirectory(path);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		

	}
}
