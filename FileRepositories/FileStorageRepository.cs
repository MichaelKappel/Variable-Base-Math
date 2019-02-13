using Common.Interfaces;
using System;
using System.IO;

namespace FileRepositories
{
    public class FileStorageRepository: IStorageRepository
    {

        public void Create(String containerName, String blobName, String value)
        {
           if (!Directory.Exists(containerName)){
                Directory.CreateDirectory(containerName);
            }


           var numberString = String.Empty;
            using (FileStream fs = File.Open(String.Format(@"{0}\{1}", containerName, blobName), FileMode.CreateNew))
            {
                using (StreamWriter sr = new StreamWriter(fs))
                {
                        sr.Write(value);
                }
            }
        }

        public String Get(String containerName, String blobName)
        {
            var numberString = String.Empty;
            using (FileStream fs = File.OpenRead(String.Format(@"{0}\{1}", containerName, blobName)))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
