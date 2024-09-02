
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DXPANACEASOFT.Auxiliares
{
	internal static class FileExcelUploadHelper
	{
		private const string m_UploadDirectory = "~/App_Data/UploadedFiles";
		private const decimal m_MaxFileUploadSizeMb = 100.0m; // Máximo archivos de 4MB

		private static long m_LastExpiredFilesCleanUp;

		internal static readonly int MaxFileUploadSize;
		internal static readonly string MaxFileSizeErrorText;

		static FileExcelUploadHelper()
		{
			MaxFileUploadSize = Convert.ToInt32(m_MaxFileUploadSizeMb * 1024 * 1024);
			MaxFileSizeErrorText = String.Format("El tamaño máximo permitido es de {0:0.0} MB.", m_MaxFileUploadSizeMb);
		}

		internal static string FileUploadProcess(DevExpress.Web.FileUploadCompleteEventArgs e)
		{
			// Verificamos si estamos en un contexto válido, con un archivo válido...
			if (HttpContext.Current == null)
			{
				return null;
			}

			if (!e.UploadedFile.IsValid)
			{
				return null;
			}

			// Generamos el nombre base del archivo
			var contentId = Guid.NewGuid().ToString("D");

			// Guardamos el descriptor del archivo
			var fileDescriptorPath = GetFileDescriptorPath(contentId);

			using (var fileDescriptorStream = new StreamWriter(fileDescriptorPath))
			{
				fileDescriptorStream.WriteLine(e.UploadedFile.FileName);
				fileDescriptorStream.WriteLine(e.UploadedFile.ContentType);
				fileDescriptorStream.Close();
			}

			// Guardamos el contenido del archivo
			var fileContentPath = GetFileContentPath(contentId);
			e.UploadedFile.SaveAs(fileContentPath);

			// Devolvemos el resultado de la carga
			return contentId;
		}

		internal static string WriteFileUpload(string fileName, string contentType, byte[] fileContent)
		{
			// Verificamos si estamos en un contexto válido...
			if (HttpContext.Current == null)
			{
				return null;
			}

			// Generamos el nombre base del archivo
			var contentId = Guid.NewGuid().ToString("D");

			// Guardamos el descriptor del archivo
			var fileDescriptorPath = GetFileDescriptorPath(contentId);

			using (var fileDescriptorStream = new StreamWriter(fileDescriptorPath))
			{
				fileDescriptorStream.WriteLine(fileName);
				fileDescriptorStream.WriteLine(contentType);
				fileDescriptorStream.Close();
			}

			// Guardamos el contenido del archivo
			var fileContentPath = GetFileContentPath(contentId);
			File.WriteAllBytes(fileContentPath, fileContent);

			// Devolvemos el resultado de la carga
			return contentId;
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

		internal static void CleanUpUploadedFile(string fileId)
		{
			// Verificamos si estamos en un contexto válido...
			if (HttpContext.Current == null)
			{
				return;
			}

			var filesPattern = Path.GetFileNameWithoutExtension(fileId) + ".da?";
			var files = GetUploadedFilesDirectory().EnumerateFiles(filesPattern);
			Task.Run(() => PerformUploadedFilesCleanUp(files, false));
		}

		internal static void CleanUpUploadedFilesDirectory()
		{
			// Verificamos si estamos en un contexto válido...
			if (HttpContext.Current == null)
			{
				return;
			}

			// Recuperamos la última hora de ejecución
			var currentTime = DateTime.Now.Ticks;
			var nextCleanUp = Interlocked.Read(ref m_LastExpiredFilesCleanUp)
				+ (new TimeSpan(0, 5, 0)).Ticks; // Limpieza se ejecutará cada 5 minutos...

			if (currentTime >= nextCleanUp)
			{
				Interlocked.Exchange(ref m_LastExpiredFilesCleanUp, currentTime);

				var files = GetUploadedFilesDirectory().EnumerateFiles("*.da?");
				Task.Run(() => PerformUploadedFilesCleanUp(files, true));
			}
		}

		private static void PerformUploadedFilesCleanUp(IEnumerable<FileInfo> files, bool expiredOnly)
		{
			// Archivos de contenido expiran en 30 minutos desde su última modificación
			var expiredDateTime = DateTime.Now.AddMinutes(-30);

			foreach (var file in files)
			{
				try
				{
					if (expiredOnly)
					{
						// Se actualiza el campo con el tiempo de última limpieza de expirados
						Interlocked.Exchange(ref m_LastExpiredFilesCleanUp, DateTime.Now.Ticks);

						if (expiredDateTime >= file.LastWriteTime)
						{
							file.Delete();
						}
					}
					else
					{
						file.Delete();
					}
				}
				catch
				{
				}
			}
		}

		private static DirectoryInfo GetUploadedFilesDirectory()
		{
			var directory = HttpContext.Current.Server.MapPath(m_UploadDirectory);
			return Directory.CreateDirectory(directory);
		}
	}
}