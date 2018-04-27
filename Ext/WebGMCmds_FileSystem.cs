using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUDLR;
using System.IO;

namespace WebGM.FileSystem
{
    public enum RootDirType
    {
        PersistentDataPath,
        TempDataPath,
    }

    public class GMFilePath
    {
        private RootDirType rootType;
        public RootDirType RootType
        {
            get
            {
                return rootType;
            }
        }

        private string rootPath {
            get
            {
                if (rootType == RootDirType.PersistentDataPath)
                {
                    return Application.persistentDataPath;
                }
                else
                {
                    //temp
                    return Application.temporaryCachePath;
                }
            }
        }

        private string relativePath;
        public string GetFadePath()
        {
            return "[" + rootType + "]/" + relativePath;
        }

        public GMFilePath()
        {
            rootType = RootDirType.PersistentDataPath;
            relativePath = string.Empty;
        }

        public string AbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(relativePath))
                {
                    return rootPath;
                }
                else
                {
                    return rootPath + "/" + relativePath;
                }
                
            }
        }

        public bool IsRootPath(string abPath)
        {
            return abPath.TrimEnd('/') == rootPath;
        }

        public string GetAbsolutePath(string rPath, string figureRelativePath = null)
        {
            if (figureRelativePath == null)
            {
                figureRelativePath = this.relativePath;
            }

            if (rPath == ".")
            {
                return rootPath + "/" + figureRelativePath;
            }
            
            if (rPath == "..")
            {
                rPath = "../";
            }

            if (string.IsNullOrEmpty(rPath))
            {
                if (string.IsNullOrEmpty(figureRelativePath))
                {
                    return rootPath;
                }
                else
                {
                    return rootPath + "/" + figureRelativePath;
                }
            }
            else if (rPath.StartsWith("/"))
            {
                return rootPath + rPath;
            }
            else if (rPath.StartsWith("./"))
            {
                return GetAbsolutePath(rPath.Substring(2));
            }
            else if (rPath.StartsWith("../"))
            {
                var parentDir = GetParentDir(figureRelativePath);
                return GetAbsolutePath(rPath.Substring(3), parentDir);
            }
            else
            {
                return rootPath + "/" + figureRelativePath + "/" + rPath;
            }
        }

        private string GetParentDir(string rPath)
        {
            var idx = rPath.LastIndexOf('/');
            if (idx < 0)
            {
                return string.Empty;
            }
            else
            {
                return rPath.Remove(idx);
            }
        }

        public void Cd(string abPath)
        {
            if (abPath.StartsWith(rootPath))
            {
                var rPath = abPath.Substring(rootPath.Length);
                rPath = rPath.Trim('/');

                SetRelativePath(rPath);
            }
            else
            {
                Debug.LogError("change root not imp yet");
            }
        }

        private void SetRelativePath(string rPath)
        {
            relativePath = rPath;
        }

    }

    /// <summary>
    /// 模拟ls, cd, rm, mkdir, pwd等命令
    /// </summary>
    public static class WebGMCmds_FileSystem
    {

        private static GMFilePath pwd = new GMFilePath();

        #region ls
        [Command("ls", "列出当前目录的文件")]
        public static void Ls(string[] args)
        {
            string path = null;
            if (args == null || args.Length == 0)
            {
                path = pwd.AbsolutePath;
            }
            else
            {
                var arg = args[0];
                path = pwd.GetAbsolutePath(arg);
            }

            string err = null;
            if (!_Ls(path, out err))
            {
                Console.Log(err);
            }

        }

        private static bool _Ls(string path, out string err)
        {
            err = null;

            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {

                var dirs = dir.GetDirectories();
                foreach (var d in dirs)
                {
                    var dname = d.Name;
                    Console.Log(dname);
                }

                var files = dir.GetFiles();
                foreach (var f in files)
                {
                    var fname = f.Name;
                    Console.Log(fname);
                }

                return true;
            }
            else
            {
                err = "path is not a directory";
                return false;
            }
            
        }
        #endregion

        #region cd
        [Command("cd", "修改当前目录")]
        public static void Cd(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Log("invalid parameter.");
            }
            else
            {
                var toPath = args[0];
                var abPath = pwd.GetAbsolutePath(toPath);

                string err = null;
                if (_Cd(abPath, out err))
                {
                    //do nothing
                }
                else
                {
                    Console.Log(err);
                }
            }
        }

        private static bool _Cd(string path, out string err)
        {
            err = null;

            if (Directory.Exists(path))
            {
                pwd.Cd(path);
                Console.Log("pwd:" + pwd.GetFadePath());

                return true;
            }
            else
            {
                Console.Log("dir not exist.");
                return false;
            }
        }
        #endregion

        #region pwd
        [Command("pwd", "当前目录位置")]
        public static void Pwd()
        {
            var str = pwd.GetFadePath();
            Console.Log(str);
        }
        #endregion

        #region mkdir
        [Command("mkdir", "创建文件夹")]
        public static void Mkdir(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Log("invalid argument");
                return;
            }

            var arg = args[0];
            var dirPath = pwd.GetAbsolutePath(arg);

            if (Directory.Exists(dirPath))
            {
                Console.Log("target dir exist.");
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (System.IO.IOException ioe)
                {
                    Console.Log("mkdir failed.");
                }
            }

        }
        #endregion

        #region rm
        [Command("rm", "删除文件或文件夹")]
        public static void Rm(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Log("invalid argument.");
            }
            else
            {
                var arg = args[0];

                var abPath = pwd.GetAbsolutePath(arg);
                if (pwd.IsRootPath(abPath))
                {
                    Console.Log("can not remove root path.");
                }
                else
                {
                    if (File.Exists(abPath))
                    {
                        try
                        {
                            File.Delete(abPath);
                        }
                        catch (System.IO.IOException)
                        {
                            Console.Log("remove file error.");
                        }
                    }
                    else
                    {
                        if (Directory.Exists(abPath))
                        {
                            try
                            {
                                Directory.Delete(abPath, true);
                            }
                            catch (System.IO.IOException)
                            {
                                Console.Log("remove directory error.");
                            }
                        }
                        else
                        {
                            Console.Log("path not exist.");
                        }
                    }
                }
            }
        }
        #endregion

            #region echo
        [Command("echo", "echo")]
        public static void Echo(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                //do nothing
            }
            else
            {
                var arg = args[0];
                Console.Log(arg);
            }
        }
        #endregion

        #region wget

        #endregion
    }

}
