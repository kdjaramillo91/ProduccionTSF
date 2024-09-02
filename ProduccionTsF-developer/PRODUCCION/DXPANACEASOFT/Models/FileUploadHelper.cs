using System;
using System.IO;
using System.Configuration;
using System.Web;

namespace DXPANACEASOFT.Models
{
	public static class FileUploadHelper
	{
		private static string m_UploadDirectory = "~/App_Data/UploadedFiles";
		//private static string m_UploadDirectoryRelative = "~/App_Data/UploadedFiles";
		private static string m_AttachmentDirectory = "~/Attachment";


		public static string UploadDirectoryDefaultTemp
		{
			get
			{
				return m_UploadDirectory;
			}

		}
		private const decimal m_MaxFileUploadSizeMb = 4.0m; // Máximo archivos de 4MB

		public static readonly string[] AllowedFileExtensions = new string[] {
			".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".avi", ".png", ".mp3", ".xml", ".doc", ".pdf"
		};
		public static readonly string[] VirtualScrollingAllowedFileExtensions = new string[] {
			".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".png", ".doc", ".pdf", ".xls", ".xlsx"
		};

		internal static readonly int MaxFileUploadSize;
		internal static readonly string MaxFileSizeErrorText;

		static FileUploadHelper()
		{
			MaxFileUploadSize = Convert.ToInt32(m_MaxFileUploadSizeMb * 1024 * 1024);
			MaxFileSizeErrorText = String.Format("El tamaño máximo permitido es de {0:0,0}MB.", m_MaxFileUploadSizeMb);
		}

		internal static string FileUploadProcess(DevExpress.Web.FileUploadCompleteEventArgs e, string pathServer = "")
		{
			if (e.UploadedFile.IsValid)
			{
				var fileDirectory = pathServer == "" ? GetUploadedFilesDirectory().FullName : pathServer;
				var fileDataName = Guid.NewGuid().ToString("D");

				var fileDescriptor = Path.Combine(fileDirectory, fileDataName + ".da0");
				using (var fileDescriptorStream = new StreamWriter(fileDescriptor))
				{
					fileDescriptorStream.WriteLine(e.UploadedFile.FileName);
					fileDescriptorStream.WriteLine(e.UploadedFile.ContentType);
					fileDescriptorStream.Close();
				}

				var fileBinary = Path.Combine(fileDirectory, fileDataName + ".dat");
				e.UploadedFile.SaveAs(fileBinary);

				var result = new
				{
					guid = fileDataName,
					url = m_UploadDirectory,
					filename = e.UploadedFile.FileName,
				};

				return Newtonsoft.Json.JsonConvert.SerializeObject(result);
			}
			else
			{
				return null;
			}
		}

		internal static string FileUploadProcessAttachment(string partDirectory, string fileName
			, string contentType, byte[] content, out string url, string pathServer = "")
		{
			//if (e.UploadedFile.IsValid)
			//{
			var fileDirectory = pathServer == "" ? GetUploadedFilesDirectory(partDirectory).FullName : pathServer;
			//var fileDirectory = GetAttachmentDirectory(partDirectory).FullName;
			var fileDataName = Guid.NewGuid().ToString("D");

			var fileDescriptor = Path.Combine(fileDirectory, fileDataName + ".da0");
			using (var fileDescriptorStream = new StreamWriter(fileDescriptor))
			{
				fileDescriptorStream.WriteLine(fileName);
				fileDescriptorStream.WriteLine(contentType);
				fileDescriptorStream.Close();
			}

			var fileBinary = Path.Combine(fileDirectory, fileDataName + ".dat");
			//e.UploadedFile.SaveAs(fileBinary);
			File.WriteAllBytes(fileBinary, content ?? new byte[] { });
			//guid = fileDataName;
			url = m_AttachmentDirectory + partDirectory;

			return fileDataName;
			//}
			//else
			//{
			//    return null;
			//}
		}

		internal static byte[] ReadFileUpload(ref string fileDataName
			, out string filename, out string contentType, string pathServer = "")
		{
			var fileDirectory = pathServer == "" ? GetUploadedFilesDirectory().FullName : pathServer;

			fileDataName = Path.GetFileNameWithoutExtension(fileDataName);

			var fileDescriptor = Path.Combine(fileDirectory, fileDataName + ".da0");
			var fileBinary = Path.Combine(fileDirectory, fileDataName + ".dat");

			try
			{
				var fileInfo = File.ReadAllLines(fileDescriptor);

				filename = fileInfo[0];
				contentType = fileInfo[1];

				return File.ReadAllBytes(fileBinary);
			}
			catch (Exception exception)
			{
				throw new Exception("No se pudo procesar el archivo cargado." + exception.Message);
				//OperacionesException("No se pudo procesar el archivo cargado.", exception);
			}
		}

		internal static byte[] ReadFileUpload(ref string fileDataName, ref string directory, out string filename, out string contentType, string pathServer = "")
		{
			var fileDirectory = pathServer == "" ? GetUploadedFilesDirectory(directory).FullName : pathServer;

			fileDataName = Path.GetFileNameWithoutExtension(fileDataName);

			var fileDescriptor = Path.Combine(fileDirectory, fileDataName + ".da0");
			var fileBinary = Path.Combine(fileDirectory, fileDataName + ".dat");

			try
			{
				var fileInfo = File.ReadAllLines(fileDescriptor);

				filename = fileInfo[0];
				contentType = fileInfo[1];

				return File.ReadAllBytes(fileBinary);
			}
			catch (Exception exception)
			{
				throw new Exception("No se pudo procesar el archivo cargado." + exception.Message);
				//OperacionesException("No se pudo procesar el archivo cargado.", exception);
			}
		}

		internal static byte[] ReadFileUpload(string contentId,
			out string fileName, out string contentType, out string fileContentPath)
		{
			// Verificamos si estamos en un contexto válido...
			if (HttpContext.Current == null)
			{
				throw new Exception("No se pudo procesar el archivo cargado: Contexto es inválido.");
			}

			// Cargamos el descriptor y contenido del archivo
			var fileDescriptorPath = GetFileDescriptorPath(contentId);
			fileContentPath = GetFileContentPath(contentId);

			try
			{
				var fileInfo = File.ReadAllLines(fileDescriptorPath);

				fileName = fileInfo[0];
				contentType = fileInfo[1];

				return File.ReadAllBytes(fileContentPath);
			}
			catch (Exception exception)
			{
				throw new Exception("No se pudo procesar el archivo cargado.", exception);
			}
		}

		internal static void CleanUpUploadedFilesDirectory()
		{
			try
			{
				var maxDateTime = DateTime.Now.AddMinutes(-30); // Después de 30 minutos, los archivos temporales ya no serán usados

				foreach (var file in GetUploadedFilesDirectory().EnumerateFiles("*.da?"))
				{
					if (file.CreationTime <= maxDateTime)
					{
						file.Delete();
					}
				}
			}
			catch
			{
			}
		}

		internal static void CleanUpUploadedFiles(string fileDataName)
		{
			try
			{
				foreach (var file in GetUploadedFilesDirectory().EnumerateFiles(fileDataName + ".da?"))
				{
					file.Delete();
				}
			}
			catch
			{
			}
		}

		internal static void CleanUpUploadedFiles(string directory, string fileDataName)
		{
			try
			{
				foreach (var file in GetUploadedFilesDirectory(directory).EnumerateFiles(fileDataName + ".da?"))
				{
					file.Delete();
				}
			}
			catch
			{
			}
		}

		public static DirectoryInfo GetUploadedFilesDirectory()
		{
			var directory = System.Web.HttpContext.Current.Server.MapPath(m_UploadDirectory);
			return Directory.CreateDirectory(directory);
		}

		private static DirectoryInfo GetAttachmentDirectory(string partDirectory)
		{
			var directory = System.Web.HttpContext.Current.Server.MapPath(m_AttachmentDirectory + partDirectory);
			return Directory.CreateDirectory(directory);
		}

		private static DirectoryInfo GetUploadedFilesDirectory(string directory)
		{
			var directoryAux = System.Web.HttpContext.Current.Server.MapPath(directory);
			return Directory.CreateDirectory(directoryAux);
		}

		internal static string GetFileContentPath(string contentId)
		{
			var fileBaseName = Path.Combine(GetUploadedFilesDirectory().FullName, contentId);
			return Path.ChangeExtension(fileBaseName, ".dat");
		}

		internal static string GetFileDescriptorPath(string contentId)
		{
			var fileBaseName = Path.Combine(GetUploadedFilesDirectory().FullName, contentId);
			return Path.ChangeExtension(fileBaseName, ".da0");
		}
	}
}