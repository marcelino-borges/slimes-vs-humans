﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVLoader 
{
    //Reference to the file
    private TextAsset csvFile;
    private char lineSeparator = '\n';
    private char surround = '"';
    private string[] fieldSeparator = { "\",\"" };

    public void LoadCSV() {
        csvFile = Resources.Load<TextAsset>("Localisation/localisation");
    }

    public Dictionary<string,string> GetDictionaryValues(string attributeId) {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] lines = csvFile.text.Split(lineSeparator);
        int attributeIndex = -1;

        string[] headers = lines[0].Split(fieldSeparator, StringSplitOptions.None);        

        for (int i = 0; i < headers.Length; i++) {
            if(headers[i].Contains(attributeId)) {
                attributeIndex = i;
                break;
            }
        }

        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for(int i = 1; i < lines.Length; i++) {
            string line = lines[i];

            string[] fields = CSVParser.Split(line);

            for(int f = 0; f < fields.Length; f++) {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].Replace(surround.ToString(), "");
            }

            if(fields.Length > attributeIndex) {
                var key = fields[0];
                if(dictionary.ContainsKey(key)) { continue; }
                var value = fields[attributeIndex];                
                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }

#if UNITY_EDITOR
    public void Add (string key, string value) {
        string appended = string.Format("\n\"{0}\"{1}\",\"\"", key, value);
        File.AppendAllText("Assets/Resources/Localisation/localisation.csv", appended);

        UnityEditor.AssetDatabase.Refresh();
    }

    public void Remove(string key) {
        string[] lines = csvFile.text.Split(lineSeparator);

        string[] keys = new string[lines.Length];

        for(int i = 0; i < lines.Length; i++) {
            string line = lines[i];
            keys[i] = line.Split(fieldSeparator, StringSplitOptions.None)[0];
        }

        int index = -1; 

        for(int i = 0; i < keys.Length; i++) {
            if (keys[i].Contains(key)) {
                index = i;
                break;
            }
        }

        if(index > -1) {
            string[] newLines;
            newLines = lines.Where(w => w != lines[index]).ToArray();

            string replaced = string.Join(lineSeparator.ToString(), newLines);
            File.WriteAllText("Assets/Resources/Localisation/localisation.csv", replaced);
        }
    }

    public void Edit(string key, string value) {
        Remove(key);
        Add(key, value);
    }
#endif
}
