using System.Collections.Generic;
using UnityEngine;

public class LocalisationSystem : MonoBehaviour {

    public static Language language = Language.English;

    private static Dictionary<string, string> localisedBR;
    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedES;
    private static Dictionary<string, string> localisedJA;
    private static Dictionary<string, string> localisedZHCN;
    private static Dictionary<string, string> localisedKO;
    private static Dictionary<string, string> localisedIT;
    private static Dictionary<string, string> localisedRU;
    private static Dictionary<string, string> localisedTH;
    private static Dictionary<string, string> localisedFR;
    private static Dictionary<string, string> localisedDE;

    public static bool isInit;

    public static CSVLoader csvLoader;

    public static void Init() {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        UpdateDictionaries();

        isInit = true;
    }

    public static void UpdateDictionaries()
    {
        localisedBR = csvLoader.GetDictionaryValues("pt");
        localisedEN = csvLoader.GetDictionaryValues("en");
        localisedES = csvLoader.GetDictionaryValues("es");
        localisedJA = csvLoader.GetDictionaryValues("ja");
        localisedZHCN = csvLoader.GetDictionaryValues("zh-CN");
        localisedKO = csvLoader.GetDictionaryValues("ko");
        localisedIT = csvLoader.GetDictionaryValues("it");
        localisedRU = csvLoader.GetDictionaryValues("ru");
        localisedTH = csvLoader.GetDictionaryValues("th");
        localisedFR = csvLoader.GetDictionaryValues("fr");
        localisedDE = csvLoader.GetDictionaryValues("de");
    }

    public static Dictionary<string, string> GetDictionaryForEditor() {
        if(!isInit) { Init(); }
        return localisedEN;
    }

    public static string GetLocalisedValue(string key) {
        if(!isInit) { Init(); }

        string value = key;

        switch(language) {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.Portuguese:
                localisedBR.TryGetValue(key, out value);
                break;
            case Language.Spanish:
                localisedES.TryGetValue(key, out value);
                break;
            case Language.Japonese:
                localisedJA.TryGetValue(key, out value);
                break;
            case Language.Chinese:
                localisedZHCN.TryGetValue(key, out value);
                break;
            case Language.Korean:
                localisedKO.TryGetValue(key, out value);
                break;
            case Language.Italian:
                localisedIT.TryGetValue(key, out value);
                break;
            case Language.Russian:
                localisedRU.TryGetValue(key, out value);
                break;
            case Language.Thai:
                localisedTH.TryGetValue(key, out value);
                break;
            case Language.French:
                localisedFR.TryGetValue(key, out value);
                break;
            case Language.Deutsch:
                localisedDE.TryGetValue(key, out value);
                break;
        }

        return value;
    }

#if UNITY_EDITOR
    public static void Add(string key, string value) {
        if(value.Contains("\"")) {
            value.Replace('"', '\"');
        }

        if(csvLoader == null) {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Add(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Replace(string key, string value) {
        if (value.Contains("\"")) {
            value.Replace('"', '\"');
        }

        if (csvLoader == null) {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Edit(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Remove(string key) {        
        if (csvLoader == null) {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Remove(key);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }
#endif
}
