using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
namespace Qisi.General
{
	public class CommonMethods
	{
		public static string LastErrorMessage
		{
			get;
			set;
		}
		public static DataSet Readxlsx(string Path, bool HDR = true)
		{
			DataSet dataSet = new DataSet();
			string text;
			if (HDR)
			{
				text = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=Yes;IMEX=1\";";
			}
			else
			{
				text = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=No;IMEX=1\";";
			}
			OleDbConnection oleDbConnection = new OleDbConnection(text);
			try
			{
				oleDbConnection.Open();
				DataTable oleDbSchemaTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[]
				{
					null,
					null,
					null,
					"TABLE"
				});
				foreach (DataRow dataRow in oleDbSchemaTable.Rows)
				{
					string text2 = dataRow["TABLE_NAME"].ToString().Trim();
					if (text2.EndsWith("$"))
					{
						OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + text2 + "]", text);
						oleDbDataAdapter.Fill(dataSet, text2);
					}
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
			}
			finally
			{
				oleDbConnection.Close();
				oleDbConnection.Dispose();
			}
			return dataSet;
		}
		public static DataTable Readxlsx(string Path, string TableName, bool HDR = true)
		{
			DataTable dataTable = new DataTable();
			string text;
			if (HDR)
			{
				text = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=Yes;IMEX=1\";";
			}
			else
			{
				text = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=No;IMEX=1\";";
			}
			OleDbConnection oleDbConnection = new OleDbConnection(text);
			try
			{
				oleDbConnection.Open();
				OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + TableName + "$]", text);
				oleDbDataAdapter.Fill(dataTable);
				oleDbDataAdapter.Dispose();
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
			}
			finally
			{
				oleDbConnection.Close();
				oleDbConnection.Dispose();
			}
			return dataTable;
		}
		public static DataSet Readxlsx(string Path, string[] TableNames, bool HDR = true)
		{
			DataSet dataSet = new DataSet();
			string text;
			if (HDR)
			{
				text = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=Yes;IMEX=1\";";
			}
			else
			{
				text = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=No;IMEX=1\";";
			}
			OleDbConnection oleDbConnection = new OleDbConnection(text);
			try
			{
				oleDbConnection.Open();
				for (int i = 0; i < TableNames.Length; i++)
				{
					string text2 = TableNames[i];
					OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + text2 + "$]", text);
					oleDbDataAdapter.Fill(dataSet, text2);
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
			}
			finally
			{
				oleDbConnection.Close();
				oleDbConnection.Dispose();
			}
			return dataSet;
		}
		public static DataSet Readxls(string Path, bool HDR = true)
		{
			DataSet dataSet = new DataSet();
			string text;
			if (HDR)
			{
				text = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";";
			}
			else
			{
				text = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=No;IMEX=1\";";
			}
			OleDbConnection oleDbConnection = new OleDbConnection(text);
			try
			{
				oleDbConnection.Open();
				DataTable oleDbSchemaTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[]
				{
					null,
					null,
					null,
					"TABLE"
				});
				foreach (DataRow dataRow in oleDbSchemaTable.Rows)
				{
					string text2 = dataRow["TABLE_NAME"].ToString().Trim();
					if (text2.EndsWith("$"))
					{
						OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + text2 + "]", text);
						oleDbDataAdapter.Fill(dataSet, text2);
					}
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
			}
			return dataSet;
		}
		public static DataTable Readxls(string Path, string TableName, bool HDR)
		{
			DataTable dataTable = new DataTable();
			string text;
			if (HDR)
			{
				text = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";";
			}
			else
			{
				text = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=No;IMEX=1\";";
			}
			OleDbConnection oleDbConnection = new OleDbConnection(text);
			try
			{
				oleDbConnection.Open();
				OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + TableName + "$]", text);
				oleDbDataAdapter.Fill(dataTable);
				oleDbDataAdapter.Dispose();
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
			}
			finally
			{
				oleDbConnection.Close();
				oleDbConnection.Dispose();
			}
			return dataTable;
		}
		public static DataSet Readxls(string Path, string[] TableNames, bool HDR = true)
		{
			DataSet dataSet = new DataSet();
			string text;
			if (HDR)
			{
				text = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";";
			}
			else
			{
				text = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=No;IMEX=1\";";
			}
			OleDbConnection oleDbConnection = new OleDbConnection(text);
			try
			{
				oleDbConnection.Open();
				for (int i = 0; i < TableNames.Length; i++)
				{
					string text2 = TableNames[i];
					OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + text2 + "$]", text);
					oleDbDataAdapter.Fill(dataSet, text2);
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
			}
			finally
			{
				oleDbConnection.Close();
				oleDbConnection.Dispose();
			}
			return dataSet;
		}
		public static void CopyFolder(string sourceFolder, string destFolder, bool overwrite = true)
		{
			if (Directory.Exists(sourceFolder))
			{
				if (!Directory.Exists(destFolder))
				{
					Directory.CreateDirectory(destFolder);
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(sourceFolder);
				FileInfo[] files = directoryInfo.GetFiles();
				FileInfo[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					FileInfo fileInfo = array[i];
					string path = fileInfo.Name;
					string text = Path.Combine(destFolder, path);
					if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					{
						fileInfo.Attributes = FileAttributes.Normal;
					}
					try
					{
						fileInfo.CopyTo(text, overwrite);
					}
					catch (Exception ex)
					{
						CommonMethods.LastErrorMessage = ex.Message;
					}
				}
				string[] directories = Directory.GetDirectories(sourceFolder);
				string[] array2 = directories;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					string path = Path.GetFileName(text2);
					string text = Path.Combine(destFolder, path);
					CommonMethods.CopyFolder(text2, text, overwrite);
				}
			}
		}
		public static void CopyFolder(string sourceFolder, string filter, string destFolder)
		{
			string[] array = filter.Split(new char[]
			{
				'|'
			});
			if (array.Length == 1)
			{
				if (!Directory.Exists(destFolder))
				{
					Directory.CreateDirectory(destFolder);
				}
				string[] files = Directory.GetFiles(sourceFolder, filter);
				string[] array2 = files;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					string fileName = Path.GetFileName(text);
					string text2 = Path.Combine(destFolder, fileName);
					try
					{
						File.Copy(text, text2, true);
					}
					catch (Exception ex)
					{
						CommonMethods.LastErrorMessage = ex.Message;
					}
				}
				string[] directories = Directory.GetDirectories(sourceFolder);
				array2 = directories;
				for (int i = 0; i < array2.Length; i++)
				{
					string text3 = array2[i];
					string fileName = Path.GetFileName(text3);
					string text2 = Path.Combine(destFolder, fileName);
					CommonMethods.CopyFolder(text3, filter, text2);
				}
			}
			else
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string filter2 = array2[i];
					CommonMethods.CopyFolder(sourceFolder, filter2, destFolder);
				}
			}
		}
		public static bool Ency(string filePath)
		{
			FileStream fileStream = new FileInfo(filePath).OpenRead();
			byte[] bytes;
			bool result;
			if (CommonMethods.Ency(fileStream, out bytes))
			{
				File.WriteAllBytes(filePath, bytes);
				fileStream.Close();
				fileStream.Dispose();
				result = true;
			}
			else
			{
				fileStream.Close();
				fileStream.Dispose();
				result = false;
			}
			return result;
		}
		public static bool Ency(FileStream stream, out byte[] file)
		{
			file = new byte[stream.Length];
			bool result;
			try
			{
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, Convert.ToInt32(stream.Length));
				stream.Close();
				stream.Dispose();
				byte[] array2 = mD5CryptoServiceProvider.ComputeHash(array);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array2.Length; i++)
				{
					stringBuilder.Append(array2[i].ToString("x2"));
				}
				byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
				file = new byte[array.Length + 2 * bytes.Length];
				for (int i = 0; i < bytes.Length; i++)
				{
					file[i] = bytes[i];
				}
				for (int i = 0; i < array.Length; i++)
				{
					file[i + bytes.Length] = array[array.Length - 1 - i];
				}
				for (int i = 0; i < bytes.Length; i++)
				{
					file[i + bytes.Length + array.Length] = bytes[i];
				}
				result = true;
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
				result = false;
			}
			return result;
		}
		public static bool Ency(FileStream stream, string filePath)
		{
			byte[] bytes;
			bool result;
			if (CommonMethods.Ency(stream, out bytes))
			{
				File.WriteAllBytes(filePath, bytes);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		private static bool ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName)
		{
			bool flag = true;
			ZipEntry zipEntry = null;
			FileStream fileStream = null;
			string[] array;
			try
			{
				string[] files = Directory.GetFiles(FolderToZip);
				array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string path = array[i];
					fileStream = File.OpenRead(path);
					byte[] array2 = new byte[fileStream.Length];
					fileStream.Read(array2, 0, array2.Length);
					zipEntry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(path)));
					zipEntry.DateTime = DateTime.Now;
					zipEntry.Size = fileStream.Length;
					fileStream.Close();
					fileStream.Dispose();
					s.PutNextEntry(zipEntry);
					s.Write(array2, 0, array2.Length);
					s.Flush();
				}
			}
			catch
			{
				flag = false;
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
					fileStream = null;
				}
				if (zipEntry != null)
				{
					zipEntry = null;
				}
				GC.Collect();
				GC.Collect(1);
			}
			string[] directories = Directory.GetDirectories(FolderToZip);
			array = directories;
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (!CommonMethods.ZipFileDictory(text, s, Path.Combine(ParentFolderName, Path.GetFileName(text))))
				{
					result = false;
					return result;
				}
			}
			result = flag;
			return result;
		}
		private static bool ZipFileDictory(string FolderToZip, string ZipedFile, string Password)
		{
			bool result;
			if (!Directory.Exists(FolderToZip))
			{
				result = false;
			}
			else
			{
				ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(ZipedFile));
				zipOutputStream.SetLevel(6);
				zipOutputStream.Password = Password;
				bool flag = CommonMethods.ZipFileDictory(FolderToZip, zipOutputStream, "");
				zipOutputStream.Finish();
				zipOutputStream.Close();
				zipOutputStream.Dispose();
				result = flag;
			}
			return result;
		}
		private static bool ZipFile(string FileToZip, string ZipedFile, string Password)
		{
			bool result;
			if (!File.Exists(FileToZip))
			{
				CommonMethods.LastErrorMessage = "指定要压缩的文件: " + FileToZip + " 不存在!";
				result = false;
			}
			else
			{
				FileStream fileStream = null;
				ZipOutputStream zipOutputStream = null;
				ZipEntry zipEntry = null;
				bool flag = true;
				try
				{
					try
					{
						fileStream = File.OpenRead(FileToZip);
					}
					catch (Exception ex)
					{
						CommonMethods.LastErrorMessage = ex.Message;
						result = false;
						return result;
					}
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
					fileStream.Close();
					fileStream.Dispose();
					fileStream = File.Create(ZipedFile);
					zipOutputStream = new ZipOutputStream(fileStream);
					zipOutputStream.Password = Password;
					zipEntry = new ZipEntry(Path.GetFileName(FileToZip));
					zipOutputStream.PutNextEntry(zipEntry);
					zipOutputStream.SetLevel(6);
					zipOutputStream.Write(array, 0, array.Length);
					zipOutputStream.Flush();
				}
				catch
				{
					flag = false;
				}
				finally
				{
					if (zipEntry != null)
					{
						zipEntry = null;
					}
					if (zipOutputStream != null)
					{
						zipOutputStream.Finish();
						zipOutputStream.Close();
						zipOutputStream.Dispose();
					}
					if (fileStream != null)
					{
						fileStream.Close();
						fileStream.Dispose();
						fileStream = null;
					}
					GC.Collect();
					GC.Collect(1);
				}
				result = flag;
			}
			return result;
		}
		public static bool Zip(string FileToZip, string ZipedFile, string Password)
		{
			bool result;
			if (Directory.Exists(FileToZip))
			{
				result = CommonMethods.ZipFileDictory(FileToZip, ZipedFile, Password);
			}
			else
			{
				result = (File.Exists(FileToZip) && CommonMethods.ZipFile(FileToZip, ZipedFile, Password));
			}
			return result;
		}
		public static bool Decy(string filePath)
		{
			FileStream fileStream = new FileInfo(filePath).OpenRead();
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, Convert.ToInt32(fileStream.Length));
			byte[] array2 = new byte[fileStream.Length - 64L];
			fileStream.Close();
			for (int i = 0; i < array.Length - 64; i++)
			{
				array2[i] = array[array.Length - 33 - i];
			}
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array3 = mD5CryptoServiceProvider.ComputeHash(array2);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array3.Length; i++)
			{
				stringBuilder.Append(array3[i].ToString("x2"));
			}
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			bool result;
			for (int i = 0; i < 32; i++)
			{
				if (bytes[i] != array[i])
				{
					result = false;
					return result;
				}
			}
			File.WriteAllBytes(filePath, array2);
			result = true;
			return result;
		}
		public static bool Decy(string filePath, out byte[] filebuffer)
		{
			FileStream fileStream = new FileInfo(filePath).OpenRead();
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, Convert.ToInt32(fileStream.Length));
			filebuffer = new byte[fileStream.Length - 64L];
			fileStream.Close();
			for (int i = 0; i < array.Length - 64; i++)
			{
				filebuffer[i] = array[array.Length - 33 - i];
			}
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array2 = mD5CryptoServiceProvider.ComputeHash(filebuffer);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array2.Length; i++)
			{
				stringBuilder.Append(array2[i].ToString("x2"));
			}
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			bool result;
			for (int i = 0; i < 32; i++)
			{
				if (bytes[i] != array[i])
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public static bool Decy(byte[] buffer2, out byte[] filebuffer)
		{
			filebuffer = new byte[buffer2.Length - 64];
			for (int i = 0; i < buffer2.Length - 64; i++)
			{
				filebuffer[i] = buffer2[buffer2.Length - 33 - i];
			}
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(filebuffer);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			bool result;
			for (int i = 0; i < 32; i++)
			{
				if (bytes[i] != buffer2[i])
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public static bool Unzip(string filePath, string path, string password)
		{
			bool flag = true;
			FileStream fileStream = new FileInfo(filePath).OpenRead();
			byte[] buffer = new byte[fileStream.Length];
			fileStream.Read(buffer, 0, Convert.ToInt32(fileStream.Length));
			Stream stream = new MemoryStream(buffer);
			ZipInputStream zipInputStream = new ZipInputStream(stream);
			ZipEntry zipEntry = null;
			zipInputStream.Password = password;
			FileStream fileStream2 = null;
			try
			{
				while ((zipEntry = zipInputStream.GetNextEntry()) != null)
				{
					string text = Path.Combine(path, zipEntry.Name);
					if (zipEntry.Name.Contains("\\"))
					{
						int length = zipEntry.Name.LastIndexOf("\\");
						Directory.CreateDirectory(Path.Combine(path, zipEntry.Name.Substring(0, length)));
					}
					if (text != string.Empty)
					{
						byte[] array = new byte[2048];
						fileStream2 = File.Create(text);
						while (true)
						{
							int num = zipInputStream.Read(array, 0, array.Length);
							if (num <= 0)
							{
								break;
							}
							fileStream2.Write(array, 0, num);
						}
						fileStream2.Flush();
					}
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
				flag = false;
			}
			finally
			{
				if (fileStream2 != null)
				{
					fileStream2.Close();
					fileStream2 = null;
				}
				if (stream != null)
				{
					stream.Close();
					stream = null;
				}
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream = null;
				}
				if (zipEntry != null)
				{
					zipEntry = null;
				}
				if (zipInputStream != null)
				{
					zipInputStream.Close();
					zipInputStream = null;
				}
				GC.Collect();
				GC.Collect(1);
			}
			return !flag || flag;
		}
		public static bool Unzip(byte[] buffer2, string path, string password)
		{
			bool flag = true;
			Stream stream = new MemoryStream(buffer2);
			ZipInputStream zipInputStream = new ZipInputStream(stream);
			ZipEntry zipEntry = null;
			zipInputStream.Password = password;
			FileStream fileStream = null;
			try
			{
				while ((zipEntry = zipInputStream.GetNextEntry()) != null)
				{
					string text = Path.Combine(path, zipEntry.Name);
					if (zipEntry.Name.Contains("\\"))
					{
						int length = zipEntry.Name.LastIndexOf("\\");
						Directory.CreateDirectory(Path.Combine(path, zipEntry.Name.Substring(0, length)));
					}
					if (text != string.Empty)
					{
						byte[] array = new byte[2048];
						fileStream = File.Create(text);
						while (true)
						{
							int num = zipInputStream.Read(array, 0, array.Length);
							if (num <= 0)
							{
								break;
							}
							fileStream.Write(array, 0, num);
						}
						fileStream.Flush();
						fileStream.Close();
					}
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
				flag = false;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream = null;
				}
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream = null;
				}
				if (zipEntry != null)
				{
					zipEntry = null;
				}
				if (zipInputStream != null)
				{
					zipInputStream.Close();
					zipInputStream = null;
				}
				GC.Collect();
				GC.Collect(1);
			}
			return !flag || flag;
		}
		public static bool Unzip(byte[] buffer2, string password)
		{
			bool flag = true;
			Stream stream = new MemoryStream(buffer2);
			ZipInputStream zipInputStream = new ZipInputStream(stream);
			ZipEntry zipEntry = null;
			zipInputStream.Password = password;
			try
			{
				while ((zipEntry = zipInputStream.GetNextEntry()) != null)
				{
					string fileName = Path.GetFileName(zipEntry.Name);
					if (fileName != string.Empty)
					{
						byte[] array = new byte[2048];
						Stream stream2 = new MemoryStream();
						while (true)
						{
							int num = zipInputStream.Read(array, 0, array.Length);
							if (num <= 0)
							{
								break;
							}
							stream2.Write(array, 0, num);
						}
						byte[] array2 = new byte[stream2.Length];
						stream2.Seek(0L, SeekOrigin.Begin);
						stream2.Read(array2, 0, array2.Length);
						stream2.Close();
					}
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
				flag = false;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream = null;
				}
				if (zipEntry != null)
				{
					zipEntry = null;
				}
				if (zipInputStream != null)
				{
					zipInputStream.Close();
					zipInputStream = null;
				}
				GC.Collect();
				GC.Collect(1);
			}
			return !flag || flag;
		}
		public static bool Unzip(byte[] buffer2, out List<byte[]> StreamList, out List<string> FilenameList, string ext, string password)
		{
			bool flag = true;
			StreamList = new List<byte[]>();
			FilenameList = new List<string>();
			Stream baseInputStream = new MemoryStream(buffer2);
			ZipInputStream zipInputStream = new ZipInputStream(baseInputStream);
			ZipEntry zipEntry = null;
			zipInputStream.Password = password;
			byte[] array = null;
			try
			{
				while ((zipEntry = zipInputStream.GetNextEntry()) != null)
				{
					string fileName = Path.GetFileName(zipEntry.Name);
					if (fileName != string.Empty)
					{
						byte[] array2 = new byte[2048];
						Stream stream = new MemoryStream();
						while (true)
						{
							int num = zipInputStream.Read(array2, 0, array2.Length);
							if (num <= 0)
							{
								break;
							}
							stream.Write(array2, 0, num);
						}
						array = new byte[stream.Length];
						stream.Seek(0L, SeekOrigin.Begin);
						stream.Read(array, 0, array.Length);
						stream.Close();
					}
					if (Path.GetExtension(fileName) == ext)
					{
						StreamList.Add(array);
						FilenameList.Add(Path.GetFileNameWithoutExtension(fileName));
					}
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
				flag = false;
			}
			finally
			{
				if (zipEntry != null)
				{
					zipEntry = null;
				}
				if (zipInputStream != null)
				{
					zipInputStream.Close();
					zipInputStream = null;
				}
				GC.Collect();
				GC.Collect(1);
			}
			return !flag || flag;
		}
		public static bool Unzip(byte[] buffer2, out List<byte[]> StreamList, out List<string> FilenameList, string password)
		{
			bool flag = true;
			StreamList = new List<byte[]>();
			FilenameList = new List<string>();
			Stream baseInputStream = new MemoryStream(buffer2);
			ZipInputStream zipInputStream = new ZipInputStream(baseInputStream);
			ZipEntry zipEntry = null;
			zipInputStream.Password = password;
			byte[] array = null;
			try
			{
				while ((zipEntry = zipInputStream.GetNextEntry()) != null)
				{
					string fileName = Path.GetFileName(zipEntry.Name);
					if (fileName != string.Empty)
					{
						byte[] array2 = new byte[2048];
						Stream stream = new MemoryStream();
						while (true)
						{
							int num = zipInputStream.Read(array2, 0, array2.Length);
							if (num <= 0)
							{
								break;
							}
							stream.Write(array2, 0, num);
						}
						array = new byte[stream.Length];
						stream.Seek(0L, SeekOrigin.Begin);
						stream.Read(array, 0, array.Length);
						stream.Close();
					}
					StreamList.Add(array);
					FilenameList.Add(Path.GetFileName(fileName));
				}
			}
			catch (Exception ex)
			{
				CommonMethods.LastErrorMessage = ex.Message;
				flag = false;
			}
			finally
			{
				if (zipEntry != null)
				{
					zipEntry = null;
				}
				if (zipInputStream != null)
				{
					zipInputStream.Close();
					zipInputStream = null;
				}
				GC.Collect();
				GC.Collect(1);
			}
			return !flag || flag;
		}
		public static bool ClearDirectory(string Path)
		{
			bool result = true;
			DirectoryInfo directoryInfo = new DirectoryInfo(Path);
			if (directoryInfo.Exists)
			{
				DirectorySecurity accessControl = Directory.GetAccessControl(Path, AccessControlSections.All);
				InheritanceFlags inheritanceFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
				FileSystemAccessRule rule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inheritanceFlags, PropagationFlags.None, AccessControlType.Allow);
				bool flag;
				accessControl.ModifyAccessRule(AccessControlModification.Add, rule, out flag);
				directoryInfo.SetAccessControl(accessControl);
				try
				{
					FileInfo[] files = directoryInfo.GetFiles();
					for (int i = 0; i < files.Length; i++)
					{
						FileInfo fileInfo = files[i];
						if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
						{
							fileInfo.Attributes = FileAttributes.Normal;
						}
						try
						{
							fileInfo.Delete();
						}
						catch
						{
							result = false;
						}
					}
				}
				catch
				{
					result = false;
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					DirectoryInfo directoryInfo2 = directories[i];
					if (!CommonMethods.ClearDirectory(directoryInfo2.FullName))
					{
						result = false;
					}
					try
					{
						directoryInfo2.Delete(true);
					}
					catch
					{
						result = false;
					}
				}
			}
			return result;
		}
	}
}
