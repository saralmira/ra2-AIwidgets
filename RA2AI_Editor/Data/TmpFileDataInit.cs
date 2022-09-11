using AIcore;
using Library;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace RA2AI_Editor.Data
{
    public class TmpFileDataInit
    {
        private const string defdir = @"\Temp";

        public TmpFileDataInit(IniClass config)
        {
            cfg = config;
            FilePaths = new ObservableCollection<FileInfoClass>();
            if (Directory.Exists(Environment.CurrentDirectory + defdir))
            {
                foreach (string file in Directory.GetFiles(Environment.CurrentDirectory + defdir))
                {
                    string md5 = Path.GetFileName(file);
                    if (config.IsKeyExist("TmpFiles", md5))
                    {
                        FilePaths.Add(new FileInfoClass
                        {
                            FileOriginalPath = config.ReadValueWithoutNotes("TmpFiles", md5),
                            FilePath = file,
                            FileLastWriteTime = new FileInfo(file).LastWriteTime,
                            MD5 = md5
                        });
                    }
                }
            }
        }

        public void Add(string path)
        {
            if (File.Exists(path))
            {
                string md5 = Utils.GetMd5(path);
                if (!Contains(md5))
                {
                    string savepath = Environment.CurrentDirectory + defdir + @"\" + md5;
                    Utils.FileCopy(path, savepath, true);
                    FilePaths.Insert(0, new FileInfoClass
                    {
                        FileOriginalPath = path,
                        FilePath = savepath,
                        FileLastWriteTime = new FileInfo(path).LastWriteTime,
                        MD5 = md5
                    });
                }
            }
        }

        public void Delete(string md5)
        {
            if (FilePaths.Count > 0)
            {
                FileInfoClass fc = null;
                foreach (FileInfoClass c in FilePaths)
                {
                    if (c.MD5 == md5)
                    {
                        fc = c;
                        if (File.Exists(c.FilePath))
                            Utils.FileDelete(c.FilePath);
                        break;
                    }
                }
                if (fc != null)
                    FilePaths.Remove(fc);
            }
        }

        public void Recover(string md5, string path)
        {
            FileInfoClass fclass = null;
            foreach (FileInfoClass c in FilePaths)
            {
                if (c.MD5 == md5)
                {
                    fclass = c;
                    if (File.Exists(c.FilePath))
                    {
                        Utils.FileCopy(c.FilePath, path, true);
                        Utils.FileDelete(c.FilePath);
                    }
                }
            }
            if (fclass != null)
                FilePaths.Remove(fclass);
        }

        public void Save()
        {
            cfg.WriteValue("TmpFiles", null, null);
            foreach (FileInfoClass f in FilePaths)
            {
                if (File.Exists(f.FilePath))
                    cfg.WriteValue("TmpFiles", f.MD5, f.FileOriginalPath);
            }
        }

        private bool Contains(string md5)
        {
            if (FilePaths != null && FilePaths.Count > 0)
            {
                foreach (FileInfoClass fic in FilePaths)
                    if (fic.MD5 == md5)
                        return true;
            }
            return false;
        }

        public ObservableCollection<FileInfoClass> FilePaths { get; set; }
        private readonly IniClass cfg;
    }
}
