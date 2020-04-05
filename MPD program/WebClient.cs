/* Function WebClient.HttpDownload downloads file located by url.
 * This function is called by MPDclient.
 */
using System;
using System.IO;
using System.Net;

namespace MPD
{
    public class WebClient
    {
        public WebClient()
        {
        }
        public static bool HttpDownload(string url, string path)
        {
            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            System.IO.Directory.CreateDirectory(tempPath);  
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; 
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    
            }
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
               
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
               
                Stream responseStream = response.GetResponseStream();
                
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                System.IO.File.Move(tempFile, path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}