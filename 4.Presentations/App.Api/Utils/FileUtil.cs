using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTSW.Constant;
using MimeTypes;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace TTSW.Utils
{
    public static class FileUtil
    {
        public static IConfiguration Configuration { get; set; }

        #region Serialize File

        public static string GetRootPath()
        {
            return Directory.GetCurrentDirectory();
        }

        public static void Serialize(string serializeFilePath, object input)
        {
            using (StreamWriter file = System.IO.File.CreateText(serializeFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
				serializer.Converters.Add(new FileInfoJsonConverter());
				serializer.Converters.Add(new DirectoryInfoJsonConverter());
                serializer.Serialize(file, input);
            }
        }
        public static T Deserialize<T>(string serializeFilePath)
        {
            using (StreamReader file = System.IO.File.OpenText(serializeFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
				serializer.Converters.Add(new FileInfoJsonConverter());
				serializer.Converters.Add(new DirectoryInfoJsonConverter());
                return (T)serializer.Deserialize(file, typeof(T));
            }
        }
        public static StaticConfigModel GetStaticConfig()
        {
            StaticConfigModel s = new StaticConfigModel();

            string logfile = MyHelper.GetConfig(Configuration, "Files:LogFile");
            string rootpath = MyHelper.GetConfig(Configuration, "Directories:RootPath");
            string logs = MyHelper.GetConfig(Configuration, "Directories:Logs");
            string files = MyHelper.GetConfig(Configuration, "Directories:Files");

            if (!Directory.Exists(rootpath)) Directory.CreateDirectory(rootpath);
            if (!Directory.Exists(logs)) Directory.CreateDirectory(logs);
            if (!Directory.Exists(files)) Directory.CreateDirectory(files);
            if (!File.Exists(logfile)) File.CreateText(logfile);

            s.Files = new Dictionary<string, FileInfo>();
            s.Directories = new Dictionary<string, DirectoryInfo>();

            s.Files.Add("LogFile", new FileInfo(logfile));
            s.Directories.Add("RootPath", new DirectoryInfo(rootpath));
            s.Directories.Add("Logs", new DirectoryInfo(logs));
            s.Directories.Add("Files", new DirectoryInfo(files));

            return s;
        }

        /// <summary>
        /// Steps
        /// 1. Revise Utils\Models\StaticConfigModel.cs to add more properties
        /// 2. Edit code in this function on StaticConfigModel to declare more properties
        /// 3. Update the config file with by running this function to serialize it again
        /// </summary>
   //     public static void UpdateConfigFile()
   //     {
   //         var configFilePath = FileUtil.GetStaticConfigFilePath();

   //         // Declare file info
   //         var fileInfo = new StaticConfigModel();
			//fileInfo.Files = new System.Collections.Generic.Dictionary<string, FileInfo>();
			//fileInfo.Directories = new System.Collections.Generic.Dictionary<string, DirectoryInfo>();
			//fileInfo.Files.Add("appsetting", new FileInfo("./appsetting.json"));
			//fileInfo.Files.Add("temp", new FileInfo("./tmp.json"));
			//fileInfo.Directories.Add("images", new DirectoryInfo("./images"));
			//fileInfo.Directories.Add("pdf", new DirectoryInfo("./pdf"));

   //         // Serialize file info to config file
   //         FileUtil.Serialize(configFilePath, fileInfo);

   //         // Test getting content int the file
   //         var resultConfig = FileUtil.Deserialize<StaticConfigModel>(configFilePath);
   //     }
        #endregion

        public static string GetLogPath()
        {
            var staticConfig = GetStaticConfig();

            return staticConfig.Directories["Logs"].FullName;
        }

        /// <summary>
        /// Get root path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(string filePath)
        {
            var staticConfig = GetStaticConfig();
            var currentDir = staticConfig.Directories["RootPath"];

            return Path.Combine(currentDir.FullName, filePath);
        }

        /// <summary>
        /// Get root path for the specific dir
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static FilePathInfo GetDirectoryRootPath(FilePathConstant.DirType dir)
        {
            var dirName = StringEnumUtil.GetStringValue(dir);

            var filePathInfo = new FilePathInfo()
            {
                RelativePath = dirName,
                PhysicalPath = FileUtil.GetAbsolutePath(dirName)
            };

            if (!Directory.Exists(filePathInfo.PhysicalPath))
                Directory.CreateDirectory(filePathInfo.PhysicalPath);

            return filePathInfo;
        }

        /// <summary>
        /// Get both relative and physical dir path in format {root dir}/{id}/
        /// </summary>
        /// <param name="dirType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FilePathInfo GetDirectoryInfo(FilePathConstant.DirType dirType, object id)
        {
            var rootPath = GetDirectoryRootPath(dirType);

            var info = new FilePathInfo()
            {
                PhysicalPath = Path.Combine(rootPath.PhysicalPath, id.ToString()),
                RelativePath = Path.Combine(rootPath.RelativePath, id.ToString())
            };

            if (!Directory.Exists(info.PhysicalPath))
                Directory.CreateDirectory(info.PhysicalPath);

            return info;
        }

        /// <summary>
        /// Get both relative and physical file path in format {root dir}/{id}/{temp filename}
        /// </summary>
        /// <param name="dirType"></param>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FilePathInfo GetFileInfo(FilePathConstant.DirType dirType, object id, string fileName)
        {
            var dirInfo = GetDirectoryInfo(dirType, id);

            var info = new FilePathInfo()
            {
                PhysicalPath = Path.Combine(dirInfo.PhysicalPath, fileName),
                RelativePath = Path.Combine(dirInfo.RelativePath, fileName)
            };

            return info;
        }

        /// <summary>
        /// Get temporary path for file uploading
        /// </summary>
        /// <returns></returns>
        public static FilePathInfo GetTempUploadPath()
        {
            // Create temp id for temp dir
            var tempId = Guid.NewGuid();

            var tempDirInfo = GetDirectoryInfo(FilePathConstant.DirType.TempUpload, tempId);

            if (!Directory.Exists(tempDirInfo.PhysicalPath))
                Directory.CreateDirectory(tempDirInfo.PhysicalPath);

            return tempDirInfo;
        }

        /// <summary>
        /// Move file to specific folder in the format {root dir}/{id}/{temp filename}
        /// </summary>
        /// <returns>Null is no file moved. Else return filename</returns>
        public static string MoveTempUploadFileToActualPath(string tempRelativeFilePath, FilePathConstant.DirType dirType, object id)
        {
            string fileName = "";

            var tempFilePath = FileUtil.GetAbsolutePath(tempRelativeFilePath);

            // If the temp file does not exist, exit the function.
            if (File.Exists(tempFilePath))
            {
                fileName = Path.GetFileName(tempFilePath);
                var newFileInfo = GetFileInfo(dirType, id, fileName);

                if (File.Exists(newFileInfo.PhysicalPath) && new FileInfo(tempFilePath).FullName != new FileInfo(newFileInfo.PhysicalPath).FullName)
                    File.Delete(newFileInfo.PhysicalPath);

                File.Move(tempFilePath, newFileInfo.PhysicalPath);

                return fileName;
            }

            return fileName;
        }

        /// <summary>
        /// Move file to specific folder in the format {root dir}/{id}/{temp filename}. It also remove old specific file.
        /// </summary>
        /// <returns>Null is no file moved. Else return filename</returns>
        public static string MoveTempUploadFileToActualPath(string tempRelativeFilePath, FilePathConstant.DirType dirType, object id, string oldFileName)
        {
            //Remove old file first
            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFileInfo = GetFileInfo(dirType, id, oldFileName);

                if (File.Exists(oldFileInfo.PhysicalPath))
                    File.Delete(oldFileInfo.PhysicalPath);
            }            

            return MoveTempUploadFileToActualPath(tempRelativeFilePath, dirType, id);
        }

        /// <summary>
        /// Move file to specific folder in the format {root dir}/{id}/{temp filename}. It also remove old specific file.
        /// </summary>
        /// <returns>Null is no file moved. Else return filename</returns>
        public static FileBase64Info MoveTempUploadFileToBase64File(string tempRelativeFilePath)
        {
           
            var tempFilePath = FileUtil.GetAbsolutePath(tempRelativeFilePath);

            // If the temp file does not exist, exit the function.
            if (File.Exists(tempFilePath))
            {
                var fileName = Path.GetFileName(tempFilePath);
                var fileExtension = Path.GetExtension(tempFilePath);

                var base64Info = new FileBase64Info();

                // Get mine type to use later to produce data url 
                base64Info.FileName = fileName;
                base64Info.MimeType = MimeTypeMap.GetMimeType(fileExtension);

                // Convert file content to base64 string
                Byte[] bytes = File.ReadAllBytes(tempFilePath);
                base64Info.Content = Convert.ToBase64String(bytes);

                //Delete the file from temp
                //File.Delete(tempFilePath);

                return base64Info;
            }

            else
                return null;
        }

        public static bool AreThereAnyFilesInDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return false;

            var files = Directory.GetFiles(directoryPath);
            return (files.Count() > 0) ? true : false;
        }

        public static void DeleteAllFilesInDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return;

            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            var files = dirInfo.GetFiles();

            foreach (var file in files)
                file.Delete();

            return;
        }

        public static void DeleteAllFilesInDirectory(FilePathConstant.DirType dirType, Guid id)
        {
            var dirInfo = GetDirectoryInfo(dirType, id);

            DeleteAllFilesInDirectory(dirInfo.PhysicalPath);
        }

        /// <summary>
        /// Get data url with base64 content in format data:{mineType};base64,{content}
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mineType"></param>
        /// <returns></returns>
        public static FileBase64Info GetBase64Info(string content, string mineType)
        {
            return new FileBase64Info()
            {
                Content = content,
                MimeType = mineType
            };
        }

        // Get file stream content
        public static StreamContent GetFileStream(string filePath)
        {
            var fullPath = GetAbsolutePath(filePath);

            var filestream = File.Open(fullPath, FileMode.Open, FileAccess.Read);

            return new StreamContent(filestream);
        }

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static bool DoesFileExist(string fullPath)
        {
            if (File.Exists(fullPath))
                return true;
            else
                return false;
        }

        public static string GetMineType(string fullPath)
        {
            var fileExtension = Path.GetExtension(fullPath);

            return MimeTypeMap.GetMimeType(fileExtension);
        }


    }
}
