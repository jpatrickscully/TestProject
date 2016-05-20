using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestProject.Models
{
    public class File  
    {
        
        public int id { get; set; }
        public int parentId { get; set; }
        public string name { get; set; }
        public long sizeInBytes { get; set; }
        public bool isDirectory { get; set; }
        // if you are your own parent, you are root
        public bool isRoot { get { return (id == parentId); } }
        
        public ICollection<File> children { get; }
       
        public File (int fileId, int fileParentId,string fileName, bool fileIsDirectory)
        {
            id = fileId;
            parentId = fileParentId;
            name = fileName;
            isDirectory = fileIsDirectory;

            if (isDirectory)
            {
                children = new List<File>();
            }

        }
         
    }
}