using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SerializeFile
{
    private FileStream FS;
    public void Serialize(string fileName, object ob)
    {
        FS = new FileStream(fileName,
            FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(FS, ob);
        FS.Close();
    }
    public object Deserialize(string fileName)
    {
        object ob;
        FS = new FileStream(fileName,
            FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryFormatter bf = new BinaryFormatter();
        ob = bf.Deserialize(FS);
        FS.Close();
        return ob;
    }
}
