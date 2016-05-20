using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TestProject.Models;

namespace TestProject.Repositories
{
    public class FileSystemOnDrive : FileSystem
    {

        // This index contains all of the files and directories
        // This will be used to optimize searches without
        // having to walk the directory tree 
        //
        protected class FileMetaData
        {
            public string path { get; set; }
        }
        protected class FileRecord
        {
            public File modelFile { get; set; }
            public FileMetaData metaData {get;set;}

        }
        protected Dictionary <int, FileRecord> fileIndex = new Dictionary<int, FileRecord>();

        protected string rootHome;

        public FileSystemOnDrive (string rootOnFileSystem)
        {
            rootHome = rootOnFileSystem;
        }

        public override void Open()
        {
            if (System.IO.Directory.Exists(rootHome))
            {
                //Setup up the root index and meta data
                NewSyncRoot();
                
            }
            else
            {
                throw new System.IO.DirectoryNotFoundException(rootHome + " not found "); 
            }
        }

        // The current implementation only syncs on start up
        // Would be good to add listeners and update the model
        // if the file system changes from a source other 
        // than this service.

        protected void NewSyncRoot ()
        {
            fileIndex[root.id] = new FileRecord();
            fileIndex[root.id].modelFile = root;
            fileIndex[root.id].metaData = new FileMetaData();
            fileIndex[root.id].metaData.path = rootHome;

            NewSyncChildren(fileIndex[root.id]);
        }

        protected void NewSyncChildren (FileRecord parent)
        {
            foreach (string dirPath in System.IO.Directory.GetDirectories(parent.metaData.path))
            {
                NewSyncDirectory(parent, dirPath);
            }

            foreach (string filePath in System.IO.Directory.GetFiles (parent.metaData.path))
            {
                NewSyncFile(parent, filePath);   
            }

        }
        protected void NewSyncFile (FileRecord parent, string path)
        {
            FileRecord newFileRecord = new FileRecord();
            newFileRecord.modelFile = OnNewSyncFile(parent.modelFile, System.IO.Path.GetFileName(path), false);
            
            newFileRecord.metaData = new FileMetaData();
            newFileRecord.metaData.path = path;
            fileIndex[newFileRecord.modelFile.id] = newFileRecord;

            System.IO.FileInfo info = new System.IO.FileInfo(path);
            newFileRecord.modelFile.sizeInBytes = info.Length;
            
        }

        protected void NewSyncDirectory (FileRecord parent, string path)
        {
            FileRecord newFileRecord = new FileRecord();
            newFileRecord.modelFile = OnNewSyncFile(parent.modelFile, System.IO.Path.GetFileName(path), true);

            newFileRecord.metaData = new FileMetaData();
            newFileRecord.metaData.path = path;
            fileIndex[newFileRecord.modelFile.id] = newFileRecord;

            NewSyncChildren(newFileRecord);
        }

        public override void Close()
        {

        }

        override public void UpdateFileContents(int fileId, byte[] contents)
        {
            string path = fileIndex[fileId].metaData.path;
            System.IO.File.WriteAllBytes(path, contents);
            fileIndex[fileId].modelFile.sizeInBytes = contents.Length;
        }

        public override File GetFile(int id)
        {
            File returnFile = null;
            if (fileIndex.ContainsKey(id))
            {
                returnFile = fileIndex[id].modelFile;
            }
            return returnFile;
        }

        public override byte[] GetFileContents(int id)
        {
            byte[] returnBytes = null;
            File file = GetFile(id);
            if (file != null)
            {
                // TODO handle large files 
                returnBytes = new byte[file.sizeInBytes];

                if (file.sizeInBytes >0)
                {
                    // TODO asyncronous I/O
                    returnBytes = System.IO.File.ReadAllBytes(fileIndex[id].metaData.path);
                }
            }
            return returnBytes;
        }


        private int nextFieldId = 1;
        protected override int GetRootFileId()
        {
            return 0;
        }
        protected override int GetNextFileId()
        {
            return nextFieldId ++;
        }

        protected override bool IsNameValid(string name)
        {
            //TODO validate names
            return true;
        }

        protected override void OnNewFile(File newFile)
        {
            FileRecord parentRecord = fileIndex[newFile.parentId];

            FileRecord newRecord = new FileRecord ();
            newRecord.modelFile = newFile;
            newRecord.metaData = new FileMetaData();
            newRecord.metaData.path = System.IO.Path.Combine(parentRecord.metaData.path, newFile.name);

            if (newFile.isDirectory)
            {
                System.IO.DirectoryInfo newInfo = new System.IO.DirectoryInfo(newRecord.metaData.path);
                if (newInfo.Exists)
                {
                    throw new System.IO.IOException(newRecord.metaData.path + " already exists");
                }
                else
                {
                    newInfo.Create();
                    fileIndex[newFile.id] = newRecord;
                }
            }
            else
            {
                System.IO.FileInfo newInfo = new System.IO.FileInfo(newRecord.metaData.path);
                if (newInfo.Exists)
                {
                    throw new System.IO.IOException(newRecord.metaData.path + " already exists");
                }
                else
                {
                    newInfo.Create().Dispose(); // Dispose to close the file stream
                    newRecord.modelFile.sizeInBytes = 0;
                    fileIndex[newFile.id] = newRecord;
                }
            }
        }

        protected override void OnDeleteFile(File oldFile)
        {
            FileRecord oldRecord = fileIndex[oldFile.id];
            
            if (oldFile.isDirectory)
            {
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(oldRecord.metaData.path);
                if (info.Exists)
                {
                    info.Delete();
                }
            }
            else
            {
                System.IO.FileInfo info = new System.IO.FileInfo(oldRecord.metaData.path);
                if (info.Exists)
                {
                    info.Delete();
                }
            }
            fileIndex.Remove(oldFile.id);
            
        }

        protected override string GenerateNewFileName(int parentId, bool isDirectory)
        {
            // TODO generate nicer looking unique namee like "NewFile" "NewFile1"
            return System.IO.Path.GetRandomFileName();
        }
    }
}