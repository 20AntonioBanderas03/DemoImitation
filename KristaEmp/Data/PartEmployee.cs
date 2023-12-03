using System;
using System.IO;

namespace KristaEmp.Data
{
    public partial class Employee
    {
        public readonly static string FULL_IMAGES_PATH = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\Resources\\").FullName;
        public readonly string defName = "Default.png";
        public string ImgPath
        {
            get
            {
                string fullPath = $"{FULL_IMAGES_PATH}{ImageName}";
                if (File.Exists(fullPath))
                    return fullPath;
                return FULL_IMAGES_PATH + defName;
            }

            set => ImageName = value;
        }
    }
}
