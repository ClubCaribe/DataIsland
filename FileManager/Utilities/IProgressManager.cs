using System;
using System.Collections.Generic;
namespace FileManager.Utilities
{
    public interface IProgressManager
    {
        Dictionary<string, object> Properties { get; }
        void Increment();
        void CancelOperation();
        bool IsCanceled();
    }
}
