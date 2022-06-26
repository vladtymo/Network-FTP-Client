using System;
using System.IO;
using System.Net;

namespace FtpDemo18
{
    /*
    AppendFile: добавляет в запрос команду APPE, которая используется для присоединения файла к существующему файлу на FTP-сервере

    DeleteFile: добавляет в запрос команду DELE, которая используется для удаления файла на FTP-сервере

    DownloadFile: добавляет команду RETR, которая используется для загрузки файла

    GetDateTimestamp: представляет команду MDTM, которая применяется для получения даты и времени из файла

    GetFileSize: команда SIZE, получение размера файла

    ListDirectory: команда NLIST, возвращает краткий список файлов на сервере

    ListDirectoryDetails: команда LIST, возвращает подробный список файлов на FTP-сервере

    MakeDirectory: команда MKD, создает каталог на FTP-сервере

    PrintWorkingDirectory: команда PWD, отображает имя текущего рабочего каталога

    RemoveDirectory: команда RMD, удаляет каталог

    Rename: команда RENAME, переименовывает каталог

    UploadFile: команда STOR, загружает файл на FTP-сервер

    UploadFileWithUniqueName: команда STOU, загружает файл с уникальным именем на FTP-сервер
    */
    class Program
    {
        static void Main(string[] args)
        {
            string localIP = "10.0.1.9";
            NetworkCredential credential = new NetworkCredential("ftp_user", "1");

            ////////////////// working with FTP server
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{localIP}/");
            request.Credentials = credential;

            // commands
            #region ListDirectoryDetails
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine(content);
            }
            
            Console.WriteLine($"Status: {response.StatusDescription}");
            response.Close();
            #endregion

            #region MakeDirectory
            string folderName = "monday";
            request = (FtpWebRequest)WebRequest.Create($"ftp://{localIP}/" + folderName);
            request.Credentials = credential;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            //request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            request.GetResponse();
            #endregion

            #region Upload file
            string file = "Hello world!! file";
            string path = "hello.txt";
            File.WriteAllText(path, file);

            request = (FtpWebRequest)WebRequest.Create($"ftp://{localIP}/monday/" + path);
            request.Credentials = credential;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
            #endregion

            #region Download file
            request = (FtpWebRequest)WebRequest.Create($"ftp://{localIP}/" + "text_blabla.txt");
            request.Credentials = credential;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            response = (FtpWebResponse)request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string content = reader.ReadToEnd();
                File.WriteAllText("downloaded.txt", content);
            }

            response.Close();
            #endregion

            #region Delete File
            request = (FtpWebRequest)WebRequest.Create($"ftp://{localIP}/" + "text_blabla.txt");
            request.Credentials = credential;
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.GetResponse();
            #endregion
        }
    }
}
