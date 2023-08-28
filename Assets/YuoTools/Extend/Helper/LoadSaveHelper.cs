using System.IO;
using System.Text;

namespace YuoTools.Extend.Helper
{
    public class LoadSaveHelper
    {
        #region Load

        #region StreamReader

        public static string StreamReaderLoad(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        public static string StreamReaderLoad(string path, Encoding encoding)
        {
            StreamReader sr = new StreamReader(path, encoding);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        #endregion

        #region BinaryReader

        public static string BinaryReaderLoad(string path)
        {
            BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open));
            string str = br.ReadString();
            br.Close();
            return str;
        }

        public static string BinaryReaderLoad(string path, Encoding encoding)
        {
            BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open), encoding);
            string str = br.ReadString();
            br.Close();
            return str;
        }

        #endregion

        #region FileStream

        public static string FileStreamLoad(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            string str = br.ReadString();
            br.Close();
            fs.Close();
            return str;
        }

        public static string FileStreamLoad(string path, Encoding encoding)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            string str = br.ReadString();
            br.Close();
            fs.Close();
            return str;
        }

        #endregion

        #region File

        public static string FileLoad(string path)
        {
            var buffer = File.ReadAllBytes(path);
            return Encoding.Default.GetString(buffer);
        }

        public static string FileLoad(string path, Encoding encoding)
        {
            var buffer = File.ReadAllBytes(path);
            return encoding.GetString(buffer);
        }

        #endregion

        #endregion

        #region Save

        #region FileStream

        public static  void FileStreamSave(string path, string content)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            byte[] bs = Encoding.Default.GetBytes(content);
            fs.Write(bs, 0, bs.Length);
            fs.Flush();
            fs.Close();
        }

        public static void FileStreamSave(string path, string content, bool append)
        {
            FileStream fs = new FileStream(path, append ? FileMode.Append : FileMode.Create);
            byte[] bs = Encoding.Default.GetBytes(content);
            fs.Write(bs, 0, bs.Length);
            fs.Flush();
            fs.Close();
        }

        public static void FileStreamSave(string path, string content, Encoding encoding)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            byte[] bs = encoding.GetBytes(content);
            fs.Write(bs, 0, bs.Length);
            fs.Flush();
            fs.Close();
        }

        public static void FileStreamSave(string path, string content, bool append, Encoding encoding)
        {
            FileStream fs = new FileStream(path, append ? FileMode.Append : FileMode.Create);
            byte[] bs = encoding.GetBytes(content);
            fs.Write(bs, 0, bs.Length);
            fs.Flush();
            fs.Close();
        }

        #endregion

        #region StreamWriter

        public static void StreamWriterSave(string path, string str)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
            sw.Write(str);
            sw.Close();
        }

        public static void StreamWriterSave(string path, string str, bool append)
        {
            StreamWriter sw = new StreamWriter(path, append, Encoding.Default);
            sw.Write(str);
            sw.Close();
        }

        public static void StreamWriterSave(string path, string str, Encoding encoding)
        {
            StreamWriter sw = new StreamWriter(path, false, encoding);
            sw.Write(str);
            sw.Close();
        }

        public static void StreamWriterSave(string path, string str, bool append, Encoding encoding)
        {
            StreamWriter sw = new StreamWriter(path, append, encoding);
            sw.Write(str);
            sw.Close();
        }

        #endregion

        #region BinaryWriter

        public static void BinaryWriterSave(string path, byte[] bytes)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create));
            bw.Write(bytes);
            bw.Close();
        }

        public static void BinaryWriterSave(string path, byte[] bytes, bool append)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Append));
            bw.Write(bytes);
            bw.Close();
        }

        public static void BinaryWriterSave(string path, byte[] bytes, Encoding encoding)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create), encoding);
            bw.Write(bytes);
            bw.Close();
        }

        public static void BinaryWriterSave(string path, byte[] bytes, bool append, Encoding encoding)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(path, append ? FileMode.Append : FileMode.Create),
                encoding);
            bw.Write(bytes);
            bw.Close();
        }

        #endregion

        #region File

        public static void FileSave(string path, string content)
        {
            File.WriteAllBytes(path, Encoding.Default.GetBytes(content));
        }

        public static void FileSave(string path, string content, bool append)
        {
            File.WriteAllBytes(path, Encoding.Default.GetBytes(content));
        }

        public static void FileSave(string path, string content, Encoding encoding)
        {
            File.WriteAllBytes(path, encoding.GetBytes(content));
        }

        public static void FileSave(string path, string content, bool append, Encoding encoding)
        {
            File.WriteAllBytes(path, encoding.GetBytes(content));
        }

        #endregion

        #endregion
    }
}