using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class Chapter {
	public string path = "";
	public string title;
	public string description;

	public Chapter(XmlNode chapter){
		this.path = chapter.Attributes["path"].Value;

		XmlNode title = chapter.SelectSingleNode("title");
		if (title != null)
			this.title = title.InnerText;

		XmlNode description = chapter.SelectSingleNode("description");
		if (description != null)
			this.description = description.InnerText;
	}
}

public class Descriptor {
	public string title;
	public string description;

	//CONFIGURATION TO-DO

	public List<Chapter> chapters = new List<Chapter>();

	public Descriptor(XmlNode descriptor){
		XmlNode title = descriptor.SelectSingleNode("title");
		if (title != null)
			this.title = title.InnerText;

		XmlNode description = descriptor.SelectSingleNode("description");
		if (description != null)
			this.description = description.InnerText;

		XmlNode contents = descriptor.SelectSingleNode("contents");
		if (contents != null)
			foreach (XmlNode chapter in contents.ChildNodes)
				chapters.Add (new Chapter (chapter));
				
	}
}