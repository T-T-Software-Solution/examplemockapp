using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TTSW.Utils
{
	/// <summary>
	/// Sigleton class to manage folder with configuration file.
	/// </summary>
	/// <remarks>
	/// Configuration (AppIOConfiguration.json) can be created in advance also be able to create in runtime too.
	/// </remarks>
	public sealed class AppIOConfiguration
	{
		#region Properties and internal classes
		public class FilePathInfo {
			public string RelativePath { get; set; }
        	public FileInfo File { get; set; }
		}

		public class DirectoryPathInfo {
			public string RelativePath {get;set;}
			public DirectoryInfo Directory { get; set; }
		}

		public Dictionary<string, AppIOConfiguration.FilePathInfo> Files { get; }  = new Dictionary<string, AppIOConfiguration.FilePathInfo>();
		public Dictionary<string, AppIOConfiguration.DirectoryPathInfo> Directories { get; } = new Dictionary<string, AppIOConfiguration.DirectoryPathInfo>();
		#endregion

		#region Private Member for Singleton and instantiate
		private static readonly Lazy<AppIOConfiguration> lazy = new Lazy<AppIOConfiguration>(PopulateObject);
		private const string StaticConfigFileName = @"AppIOConfiguration.json";

		public static AppIOConfiguration Instance => lazy.Value;

		private static string configurationFile => Path.Combine(Directory.GetCurrentDirectory(), StaticConfigFileName);
		private static string currentDirectory => Directory.GetCurrentDirectory();
		private AppIOConfiguration(){ }

		private AppIOConfiguration(Dictionary<string, AppIOConfiguration.FilePathInfo> Files, Dictionary<string, AppIOConfiguration.DirectoryPathInfo> Directories) {
			this.Directories = Directories;
			this.Files = Files;
		}
		
		private static AppIOConfiguration PopulateObject() {
			if (File.Exists(configurationFile))
			{
				using (StreamReader file = System.IO.File.OpenText(configurationFile))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Converters.Add(new FileInfoJsonConverter());
					serializer.Converters.Add(new DirectoryInfoJsonConverter());
					return (AppIOConfiguration)serializer.Deserialize(file, typeof(AppIOConfiguration));
				}
			} else {
				return new AppIOConfiguration();
			}
		}

		#endregion
		
		public static void Serialize<T>(string serializeFilePath, T input)
        {
            using (StreamWriter file = System.IO.File.CreateText(serializeFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
				serializer.Converters.Add(new FileInfoJsonConverter());
				serializer.Converters.Add(new DirectoryInfoJsonConverter());
				serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, input, typeof(T));
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

		private static void Save()
        {
            Serialize(configurationFile, Instance);
        }

		/// <summary>
		/// Create Directory and save to configuration file
		/// </summary>
		/// <param name="directoryName">Use as a Key in Directories collection</param>
		/// <param name="parentKeyName">Reference to parent directory referred by Key in Directories collection.(default value is "RootPath")</param>
		/// <param name="relativePath">Alias(fake) path. If value is empty, value will be assigned by combining parent.RelativePath + directoryName</param>
		/// <example>
		/// This is an example for using this method.
		/// <code>
		/// AppIOConfiguration.CreateDirectory("Files");
		/// AppIOConfiguration.CreateDirectory("Logs", relativePath:"/logs");
		/// AppIOConfiguration.CreateDirectory("Log2018", "Logs", "/logs2018");
		/// AppIOConfiguration.CreateDirectory("EIC_OrderList", "Files");
		/// </code>
		/// </example>
		public static void CreateDirectory(string directoryName, string parentKeyName = "RootPath", string relativePath = "")
		{
			if (parentKeyName.Equals("RootPath") && !Instance.Directories.ContainsKey(parentKeyName))
			{
				var appRootDirectory = new DirectoryInfo(currentDirectory);
				var appRootDirectoryPathInfo = new DirectoryPathInfo() {
					Directory = appRootDirectory,
					RelativePath = "/"
				};
				Instance.Directories.Add("RootPath", appRootDirectoryPathInfo);
				Save();
			}

			if (!Instance.Directories.ContainsKey(parentKeyName))
			{
				throw new DirectoryNotFoundException("Parent's keyname is not found");
			}

			if (!Instance.Directories.ContainsKey(directoryName))
			{
				var parentDirectory = Instance.Directories[parentKeyName];
				var parentPath = parentDirectory.Directory.FullName;
				var createdDirectory = new DirectoryInfo(Path.Combine(parentPath, directoryName));
				createdDirectory.Create();
				var createdDirectoryPathInfo = new DirectoryPathInfo() {
					Directory = createdDirectory,
				};

				createdDirectoryPathInfo.RelativePath = string.IsNullOrEmpty(relativePath) ? 
					Path.Combine(parentDirectory.RelativePath, directoryName) :
					relativePath;
				
				if (!createdDirectory.Exists) 
				{
					createdDirectory.Create();
				}
				Instance.Directories.Add(directoryName, createdDirectoryPathInfo);
				Save();
			}
		}
	}
}
