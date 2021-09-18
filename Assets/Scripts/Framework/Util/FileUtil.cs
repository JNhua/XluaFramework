using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FileUtil
{
    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsExists(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }

    public static void WriteFile(string path,byte[] data)
    {
        path = PathUtil.GetStandardPath(path);
        // Get the path of the folder
        string folder = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            file.Delete();
        }
        try
        {
            using (FileStream fs = new FileStream(path,FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
}