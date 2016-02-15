using UnityEngine;
using System.Collections;
using System.Xml;

public interface ConversationNode {

	bool canParseType(string type);
	void parse(XmlElement node);

	bool execute();

	int getChild();
	void resetChild();
}
