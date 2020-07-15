using System.IO;

namespace Library
{
    /// <summary>
    /// 路径操作类
    /// </summary>
    public static class PathClass
    {
        /// <summary>
        /// 获取不冲突的新文件路径
        /// </summary>
        /// <param name="filepath">输入原始文件路径</param>
        /// <returns>返回新文件的路径</returns>
        public static string NewFilePath(string filepath)
        {
            int i = 2;
            string ext = Path.GetExtension(filepath),
                name = Path.GetDirectoryName(filepath) + "\\" +
                Path.GetFileNameWithoutExtension(filepath), ret;
            if (File.Exists(ret = (name + ext)))
            {
                while (File.Exists(ret = (name + " (" + i.ToString() + ")" + ext)))
                {
                    i++;
                }
            }
            return ret;
        }

        /// <summary>
        /// 如果文件不存在则创建文件及其所有根目录，否则忽略
        /// </summary>
        /// <param name="fpath">创建的文件路径</param>
        public static void CreateFile(string fpath, bool overwrite = false)
        {
            if (File.Exists(fpath))
            {
                if (overwrite)
                    File.Delete(fpath);
                else
                    return;
            }
            string dir = Path.GetDirectoryName(fpath);
            if (!Directory.Exists(dir))
				CreateDir(dir);
            File.Create(fpath).Close();
        }

        /// <summary>
        /// 如果文件不存在则创建文件及其所有根目录，否则忽略，返回指向该文件的流。
        /// </summary>
        /// <param name="fpath">创建的文件路径</param>
        /// <returns>返回指向该文件的流</returns>
        public static FileStream CreateFileStream(string fpath)
        {
            if (File.Exists(fpath))
                return new FileStream(fpath, FileMode.Open, FileAccess.ReadWrite);
            string dir = Path.GetDirectoryName(fpath);
            if (!Directory.Exists(dir))
                CreateDir(dir);
            return File.Create(fpath);
        }

        /// <summary>
        /// 如果文件夹不存在则创建文件夹及其所有根目录，否则忽略
        /// </summary>
        /// <param name="dir">创建的文件夹路径</param>
        public static void CreateDir(string dir)
        {
            string ddir = Path.GetDirectoryName(dir);
            if (!Directory.Exists(ddir))
				CreateDir(ddir);
            Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// 同目录下修改文件名
        /// </summary>
        /// <param name="fullpath">原文件路径</param>
        /// <param name="newname">新文件名</param>
        /// <returns>返回新文件完整路径</returns>
        public static string FileRename(string fullpath, string newname)
        {
            if (File.Exists(fullpath))
            {
                string dir = Path.GetDirectoryName(fullpath);
                if (!(dir.EndsWith("\\") || dir.EndsWith("/")))
                    dir += "\\";
                return dir + newname;
            }
            return string.Empty;
        }
    }
}
