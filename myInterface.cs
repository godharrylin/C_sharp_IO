using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingTool
{
	public interface IFileService
	{
		IEnumerable<string> GetTxtFiles(string directory);
		void MoveFiles(string sourceDir, string targetDir, IEnumerable<string> files);
		void DeleteFiles(string directory, string searchPattern);
	}

	public interface IJavaExecutor
	{
		void RunJavaExecutable(string javaExecutable);
	}

	public interface IDirectoryService
	{
		void DeleteLastNFiles(string directory, int count);
		void MoveAllFiles(string sourceDir, string targetDir);
	}
}
