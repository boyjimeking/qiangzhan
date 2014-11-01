using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class WDBAsset : ScriptableObject
{
    [SerializeField]
    private byte[] m_Stream;
	
	public WDBAsset(byte[] stream)
	{
	}
    public WDBData GetData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream serializationStream = new MemoryStream(this.m_Stream);
        WDBData data = formatter.Deserialize(serializationStream) as WDBData;
        if (data != null)
        {
            data.Init();
        }
        return data;
    }

    public void InitStream(WDBData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream serializationStream = new MemoryStream();
        formatter.Serialize(serializationStream, data);
        this.m_Stream = serializationStream.ToArray();
    }
}

