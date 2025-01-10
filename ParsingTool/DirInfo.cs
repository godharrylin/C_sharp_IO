using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ParsingTool
{
	class DirInfo
	{
		public string FolderName { get; set; }
		public string AbsolutePath { get; set; }
		public string TargetPath { get; set; }
		public PARSING_TYPE Type { get; set; }
		public DirInfo(string absolutePath)
		{
			AbsolutePath = absolutePath;
			FolderName = Path.GetFileName(AbsolutePath);
			Type = SettingType();
		}

		public PARSING_TYPE SettingType()
		{
			Type = PARSING_TYPE.NONE;
			try
			{
				//	找該目錄是否有 .txt檔
				string[] txtfiles = Directory.GetFiles(AbsolutePath);
				if (txtfiles.Length == 0)
				{
					return Type;
				}

				//	找是否有符合格式的 .txt檔
				bool needParse = false;
				foreach (string file in txtfiles)
				{
					string fileName = Path.GetFileName(file);
					if (Regex.IsMatch(fileName, @"^\d{4}_\d{2}_\d{2}T\d{2}_\d{2}\.txt$"))
					{
						needParse = true;
						break;
					}
				}


				//	判斷要使用哪一種parsing method
				if (needParse)
				{
					switch(true)
					{
						case var _ when FolderName.Contains("Parameters"):
							Type = PARSING_TYPE.PARAMETERS;
							break;
						case var _ when FolderName.Contains("Event"):
							Type = PARSING_TYPE.EVENT;
							break;
					}
				}
				else
				{
					return Type = PARSING_TYPE.NONE;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("搜尋過程中發生錯誤：" + ex.Message);
			}

			return Type;
		}

	}
}
