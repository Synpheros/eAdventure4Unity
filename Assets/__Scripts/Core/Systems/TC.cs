﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class TC
{

    public const int NORMAL_SENTENCE = 1;

    public const int NO_CONDITION_SENTENCE = 2;

    /**
     * Properties set containing the strings.
     */
    private static Dictionary<string, string> guistrings = new Dictionary<string, string>();

    /**
     * Loads the strings of the application from the given XML properties file.
     * 
     * @param languageFile
     *            Name of the file containing the text
     */

    public static void loadstrings(string languageFile)
    {
        guistrings = new Dictionary<string, string>();
        string serializedLangbase = "";
        try
        {
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
            serializedLangbase = File.ReadAllText(languageFile).Replace(Environment.NewLine, "");
#endif
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(
                "One of the language file required was not found, please verify that the language files are on the disk.\nAlthough you may still be able to use the adventure editor, you might get occasional problems or get some unreadable texts.\nIf you get this error again, please try reinstalling the application.",
                "Error loading the language file");
        }

        TCLangbaseRoot LangTexts = TCLangbaseRoot.LoadFromText(serializedLangbase);
        try
        {
            for (int i = 0; i < LangTexts.Labels.Count; i++)
            {

                string textToAdd = LangTexts.Labels[i].TextValue
                    .Replace("\n", "")
                    .Replace("  ", "");

                guistrings.Add(
                    LangTexts.Labels[i].AttributeKey,
                    textToAdd
                    );
            }
        }
        catch
        {
            Console.WriteLine(
                "Critical error: language XML is broken. Ensure that file is a valid XML and there is no duplicates.");
        }
    }

    public static void appendstrings(string languageFile)
    {

        try
        {
            Properties newstrings = new Properties(languageFile);
            foreach (string key in newstrings.getKeyset())
            {
                if (guistrings.ContainsKey(key))
                    guistrings.Remove(key);
                guistrings.Add(key, newstrings.getProperty(key));
            }
        }

        // If the file was not found
        catch (FileNotFoundException e)
        {
            Debug.LogError("Error loading the language file \n\n One of the language file required was not found, please verify that the language files are on the disk.\nAlthough you may still be able to use the adventure editor, you might get occasional problems or get some unreadable texts.\nIf you get this error again, please try reinstalling the application. \n\n ");
        }

        // If there was a I/O exception
        catch (IOException e)
        {
            Debug.LogError(
                "Error loading the language file \n\n There has been an error loading the language file, please check for problem accessing the files.\nAlthough you may still be able to use the adventure editor, you might get occasional problems or get some unreadable texts.\nIf you get this error again, please try reinstalling the application.");
        }
    }

    /**
     * Returns true if the string associated with the given identifier is on the
     * language file.
     * 
     * @param tag
     *            string identifier
     * @return true if found, false otherwise
     */

    public static bool containsText(string tag)
    {

        return guistrings.ContainsKey(tag);
    }

    /**
     * Returns true if the string associated with the given identifier is on the
     * language file.
     * 
     * @param tag
     *            string identifier
     * @param sentence
     *            1 for normal sentence, 2 for no-condition sentence
     * @return true if found, false otherwise
     */

    public static bool containsConditionsContextText(int element, int sentence)
    {

        return containsText("Conditions.Context." + sentence + "." + element);
    }

    /**
     * Returns the element name for the given element identifier.
     * 
     * @param element
     *            Element identifier
     * @return Element identifier, "Error" if the element was not found
     */

    public static string getElement(int element)
    {

        return get("Element.Name" + element);
    }

    /**
     * Returns the element name for the given element identifier.
     * 
     * @param element
     *            Element identifier
     * @param sentence
     *            1 for normal sentence, 2 for no-condition sentence
     * 
     */

    public static string getConditionsContextText(int element, int sentence)
    {

        return get("Conditions.Context." + sentence + "." + element);
    }

    /**
     * Returns the string associated with the given identifier.
     * 
     * @param identifier
     *            Identifier to search for the string
     * @return string retrieved from the text base, "Error" if the text was not
     *         found
     */

    public static string get(string identifier)
    {

        string text = null;

        if (guistrings != null && guistrings.ContainsKey(identifier))
            text = guistrings[identifier];
        else
        {
            text = "Error";
            Console.WriteLine("Identifier \"" + identifier + "\" not found");
        }

        return text;
    }

    /**
     * Returns the string associated with the given identifier. This method also
     * replaces occurrences in the original string with words passed through the
     * call. Every placeholder for the strings have all the form "#n", where
     * <i>n</i> is the index of the string to replace the placeholder in the
     * given array. This method takes only one string to replace.
     * 
     * @param identifier
     *            Identifier to search for the string
     * @param parameter
     *            string to replace the placeholder in the returned string
     * @return string retrieved from the text base, "Error" if the text was not
     *         found
     */

    public static string get(string identifier, string parameter)
    {

        string text = null;

        if (guistrings.ContainsKey(identifier))
        {
            text = guistrings[identifier];
            text = text.Replace("{#0}", parameter);
        }
        else
        {
            text = "Error";
            Console.WriteLine("Identifier \"" + identifier + "\" not found");
        }

        return text;
    }

    /**
     * Returns the string associated with the given identifier. This method also
     * replaces occurrences in the original string with words passed through the
     * call. Every placeholder for the strings have all the form "#n", where
     * <i>n</i> is the index of the string to replace the placeholder in the
     * given array. This method takes an array of strings to replace.
     * 
     * @param identifier
     *            Identifier to search for the string
     * @param parameters
     *            Array of strings to replace the placeholders in the returned
     *            string
     * @return string retrieved from the text base, "Error" if the text was not
     *         found
     */

    public static string get(string identifier, string[] parameters)
    {

        string text = null;

        if (guistrings.ContainsKey(identifier))
        {
            text = guistrings[identifier];
            for (int i = 0; i < parameters.Length; i++)
                text = text.Replace("{#" + i + "}", parameters[i]);
        }
        else
        {
            text = "Error";
            Console.WriteLine("Identifier \"" + identifier + "\" not found");
        }

        return text;
    }
}

[XmlRoot(ElementName = "entry")]
public class LanguageValues
{
    [XmlAttribute(AttributeName = "key")]
    public string AttributeKey { get; set; }

    [XmlText]
    public string TextValue { get; set; }
}

[XmlRoot(ElementName = "properties")]
public class TCLangbaseRoot
{
    [XmlElement(ElementName = "entry")]
    public List<LanguageValues> Labels { get; set; }

    public static TCLangbaseRoot LoadFromText(string text)
    {
        XmlSerializer serializer = new XmlSerializer(typeof (TCLangbaseRoot));
        return serializer.Deserialize(new StringReader(text)) as TCLangbaseRoot;
    }
}