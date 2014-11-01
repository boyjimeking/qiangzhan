
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataStorage
{
    public static T LoadData<T>(string key) where T : new()
    {
        if (PlayerPrefs.HasKey(key))
        {
            XmlSerializer serializer = new XmlSerializer(typeof (T));
            StringReader sr = new StringReader(PlayerPrefs.GetString(key));
            return (T) serializer.Deserialize(sr);
        }
        else
        {
            return new T();
        }
    }

    public static void SaveData<T>(string key, T source)
    {
        XmlSerializer serializer = new XmlSerializer(typeof (T));
        StringWriter sw = new StringWriter();
        serializer.Serialize(sw, source);
        PlayerPrefs.SetString(key, sw.ToString());
    }

    public static void ClearData(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
}

