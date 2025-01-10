using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using ParsingTool.myInterface;

namespace ParsingTool
{
	public class FilesService: IFileService
	{
		public IEnumerable<string> GetTxtFiles(string directory)
		{
			return Directory.GetFiles(directory, "*.txt");
		}

		public void MoveFiles(string sourceDir, string targetDir, IEnumerable<string> files)
		{
			foreach (string file in files)
			{
				string targetPath = Path.Combine(targetDir, Path.GetFileName(file));
				try
				{
					File.Move(file, targetPath);
				}
				catch (Exception e)
				{
					Console.WriteLine($"{e}, when move {sourceDir}");
				}

				Debug.WriteLine($"已移動檔案: {file} -> {targetDir}");
			}
		}

		public void DeleteFiles(string directory, string searchPattern)
		{
			var files = Directory.GetFiles(directory, searchPattern);
			foreach (var file in files)
			{
				File.Delete(file);
				Debug.WriteLine($"已刪除檔案: {file}");
			}
		}
	}

	public class DirectoryService : IDirectoryService
	{
		public void DeleteUnmatchFiles(string directory, string datePattern)
		{
			var files = Directory.GetFiles(directory).ToList();

			foreach (var file in files)
			{
				string fileName = Path.GetFileName(file);
				if (!Regex.IsMatch(fileName, datePattern))
				{
					File.Delete(file);
					Debug.WriteLine($"已刪除檔案: {fileName}");
				}
				Debug.WriteLine("清理完成！");
			}
		}
		public void MoveAllFiles(string sourceDir, string targetDir)
		{

			var files = Directory.GetFiles(sourceDir);
			foreach (var file in files)
			{
				string targetPath = Path.Combine(targetDir, Path.GetFileName(file));

				//	確保目錄存在
				string destinationDirectory = Path.GetDirectoryName(targetPath);
				if (!Directory.Exists(destinationDirectory))
				{
					Directory.CreateDirectory(destinationDirectory);
				}

				try
				{
					File.Move(file, targetPath);
				}
				catch(Exception e)
				{
					Console.WriteLine($"{e}, in function MoveAllFiles");
				}
				Debug.WriteLine($"已移動檔案: {file} -> {targetPath}");
			}
		}
	}

	public class JavaExecuter : IJavaExecuter
	{
		private Process process;
		public void RunJavaExecutable(string javaExecutable)
		{
			if (!File.Exists(javaExecutable))
			{
				throw new Exception($"找不到 Java 執行檔: {javaExecutable}");
			}

			var processStartInfo = new ProcessStartInfo
			{
				FileName = javaExecutable,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (process = new Process { StartInfo = processStartInfo })
			{
				process.Start();
				Console.WriteLine("Java 程式已啟動。");
				process.WaitForExit();


				//string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();

				//if (!string.IsNullOrEmpty(output))
				//{
				//	Debug.WriteLine($"輸出: {output}");
				//}

				if (!string.IsNullOrEmpty(error))
				{
					Debug.WriteLine($"錯誤: {error}");
				}

			}
		}
	}
}
