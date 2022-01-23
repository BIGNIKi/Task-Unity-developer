using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class Util
{
    private static long timePassed;

    public static void SetTimePassed(long val)
    {
        timePassed = val;
    }

    public static long GetTimePassed()
    {
        return timePassed;
    }

    /// <summary>
    /// convert object to JSON and create in root directory new file with name nameOfFile
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public static void ToJsonAndCreateFile<T>(T obj, string nameOfFile)
    {
        string str = JsonUtility.ToJson(obj, true);
        StreamWriter writer = new StreamWriter(nameOfFile, false); //false - to rewrite an existing file
        writer.WriteLine(str);
        writer.Close();
    }

    /// <summary>
    /// Serialize obj and save it to nameOfFile
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public static void ToBinaryAndCreateFile<T>(T obj, string nameOfFile)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using FileStream fs = new FileStream(nameOfFile, FileMode.OpenOrCreate);
        formatter.Serialize(fs, obj);
    }

    public static T LoadFromJson<T>(string nameOfFile)
    {
        if(File.Exists(nameOfFile))
        {
            StreamReader sr = new StreamReader(nameOfFile);
            if(sr != null)
            {
                string txt = sr.ReadToEnd();
                sr.Close();
                T newOb = JsonUtility.FromJson<T>(txt);
                return newOb;
            }
        }
        Debug.LogError("Couldn't find a file.");
        return default(T);
    }

    public static int getChildCountActive(Transform obTransform)
    {
        int count = 0;
        foreach(Transform child in obTransform)
        {
            if(child.gameObject.activeSelf == true)
            {
                count++;
            }
        }
        return count;
    }

}
