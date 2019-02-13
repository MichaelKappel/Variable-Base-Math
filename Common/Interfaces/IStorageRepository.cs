using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IStorageRepository
    {
        void Create(String containerName, String blobName, String value);
        String Get(String containerName, String blobName);
    }
}
