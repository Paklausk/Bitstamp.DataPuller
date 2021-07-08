using System;
using System.Collections.Generic;

namespace Bitstamp.DataPuller
{
    public class Settings
    {
        static object _instanceMutex = new object();
        static Settings _instance;
        public static Settings Instance
        {
            get
            {
                lock (_instanceMutex)
                {
                    if (_instance == null)
                        _instance = new Settings();
                    return _instance;
                }
            }
        }
        private Settings()
        {
            InitializeValues();
        }
        
        public bool IsDebugVersion
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
        public string ServiceName
        {
            get { return "Bitstamp.DataPuller"; }
        }
        public List<string> ServiceDependencies
        {
            get; private set;
        } = new List<string>();

        void InitializeValues()
        {
            ServiceDependencies.Add("postgresql-x64-9.6");
        }
    }
}
