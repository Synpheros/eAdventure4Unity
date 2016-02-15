using UnityEngine;
using System.Collections;
using System.Xml;

public interface Condition {
	bool check();

	bool canParseType(string type);
	void parse(XmlNode node);
}
