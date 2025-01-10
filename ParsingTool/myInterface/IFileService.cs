using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingTool.myInterface
{
	public interface IFileService
	{
		IEnumerable<string> GetTxtFiles(string directory);
		void MoveFiles(string sourceDir, string targetDir, IEnumerable<string> files);
		void DeleteFiles(string directory, string searchPattern);
	}
}
