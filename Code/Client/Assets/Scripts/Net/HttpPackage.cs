using UnityEngine;
using System.Text;

public class HttpPackage : NetPackage
{
    public WWW WwwObject { get; set; }

    public string error
    {
        get
        {
            if (IsOverTime)
            {
                return "http request over time";
            }
            else
            {
                return WwwObject.error;
            }
        }
    }

    public byte[] GetResponse()
    {
        return WwwObject.bytes;
    }

}