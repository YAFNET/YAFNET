// Created by vzrus 2010
namespace YAF.Classes.Utils
{
  using System; 

   /// <summary>
   /// The class gets common system info. Used in data layers other than MSSQL.
   /// </summary>
    public static class Platform
    {
        private static bool inited;
        private static bool isMono;

        public static bool IsMono
        {
            get
            {
                if (!inited)
                    Init();
                return isMono;
            }
        }

        public static string RuntimeName
        {
            get
            {
                if (!inited)
                    Init();
                if (isMono) return "Mono";
                else return ".NET";
            }
        }

        private static void Init()
        {
            Type t = Type.GetType("Mono.Runtime");
            isMono = t != null;
        }

        public static String VersionString
        {
            get
            {
                OperatingSystem os = Environment.OSVersion;
                return os.VersionString;
            }
        }
        public static String RuntimeString
        {
            get
            {
                Version os = Environment.Version;
                return os.ToString();
            }
        }
        public static long AllocatedMemory
        {
            get
            {
                return System.Environment.WorkingSet;
            }
        }
        public static String Processors
        {
            get
            {
                return System.Environment.ProcessorCount.ToString();
            }
        }
    } 
}
