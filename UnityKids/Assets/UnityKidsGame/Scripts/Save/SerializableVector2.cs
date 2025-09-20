using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Класс для сериализации Vector2 в JSON и из него 
/// </summary>
[Serializable]
public class SerializableVector2{
    public float x;
    public float y;

    [JsonIgnore]
    public Vector2 UnityVector => new(x, y);

    public SerializableVector2(Vector2 v){
        x = v.x;
        y = v.y;
    }

    public static List<SerializableVector2> GetSerializableList(List<Vector3> vList){
        var list = new List<SerializableVector2>(vList.Count);
        for(int i = 0 ; i < vList.Count ; i++){
            list.Add(new SerializableVector2(vList[i]));
        }
        return list;
    }

    public static List<Vector3> GetSerializableList(List<SerializableVector2> vList){
        var list = new List<Vector3>(vList.Count);
        for(int i = 0 ; i < vList.Count ; i++){
            list.Add(vList[i].UnityVector);
        }
        return list;
    }
}