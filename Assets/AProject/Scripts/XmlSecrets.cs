using System;
using System.Xml.Serialization;


[Serializable]
public class XmlSecrets 
{

    [XmlAttribute("sha")]
    public string SHA_publicKey { get; set; }

    [XmlAttribute("salt")]
    public string Salt_Key { get; set; }

    [XmlAttribute("user_name")]
    public string User_Name { get; set; }

    [XmlAttribute("pswrd")]
    public string Password { get; set; }
}
