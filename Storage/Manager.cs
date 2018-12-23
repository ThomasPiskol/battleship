using System;

namespace Storage
{
    public class StorageManager : IStorageManager
    {
        private static Guid s_InstanceID;
        public Guid InstanceID
        {
            get
            {
                if(s_InstanceID == null)
                {
                    s_InstanceID = Guid.NewGuid();
                }
                return s_InstanceID;
            }
        }
    }
}
