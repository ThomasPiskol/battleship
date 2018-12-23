using System;

namespace Storage
{
    public interface IStorageManager
    {
        Guid InstanceID { get; }
    }
}