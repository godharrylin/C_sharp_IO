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
				File.Move(file, targetPath);
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
				File.Move(file, targetPath);
				Debug.WriteLine($"已移動檔案: {file} -> {targetPath}");
			}
		}
	}

	public class JavaExecuter : IJavaExecuter
	{
		private Process process;
		private TaskCompletionSource<bool> eventHandled;
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

		private void myProcess_Exited(object sender, EventArgs e)
		{
			Console.WriteLine(
				$"Exit time    : {process.ExitTime}\n" +
				$"Exit code    : {process.ExitCode}\n" +
				$"Elapsed time : {Math.Round((process.ExitTime - process.StartTime).TotalMilliseconds)}");
			eventHandled.TrySetResult(true);
		}
	}
}
