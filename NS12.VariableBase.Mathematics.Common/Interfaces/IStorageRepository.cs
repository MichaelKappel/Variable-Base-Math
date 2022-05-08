
using System;
using System.Collections.Generic;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Interfaces
{
    public interface IStorageRepository
    {
        void Create(string containerName, string blobName, string value);
        string Get(string containerName, string blobName);
    }
}
