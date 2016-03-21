using UnityEditor;
using UnityEngine;
using System.IO;


public class BatchRenamer
{
	[MenuItem("eAdventure4Unity/Rename *.eaa to *.xml %r")]
	public static void RenameOELToXML()
	{
		string[] games = Directory.GetDirectories ("Assets/Resources/");

		foreach (string game in games) 
		{
			DirectoryInfo info = new DirectoryInfo (game + "/assets/animation/");
			FileInfo[] fileInfo = info.GetFiles ();

			foreach (FileInfo file in fileInfo) {
				if (file.Extension.ToLower () == ".eaa") {
					string newname = game + "/assets/animation/" + file.Name.Substring (0, file.Name.Length - 3) + "xml";
					
					FileUtil.DeleteFileOrDirectory (newname);
					FileUtil.CopyFileOrDirectory (file.ToString (), newname);
				}
			}
		}
	}
}