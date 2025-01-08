using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingTool.myInterface
{
	interface IDirectoryService
	{
		void DeleteUnmatchFiles(string directory, string regexPattern);
		void MoveAllFiles(string sourceDir, string targetDir);
	}
}
