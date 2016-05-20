using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestProject.Models;

namespace TestProject.Repositories
{
    public abstract class FileSystem
    {
        public File root { get; protected set; }

        public FileSystem ()
        {
            root = new File(GetRootFileId(), GetRootFileId(), "ROOT", true);
            
        }
        
        public abstract void Open();

        public abstract void Close();

        public abstract File GetFile(int id);
        // TODO Search

        public abstract byte[] GetFileContents(int id);
        
        //Create new File
        public File NewFile(int parentId, string name, bool isDirectory)
        {
            File newFile = null;

            File parent = GetFile(parentId);
            bool parentValid = ((parent != null) && (parent.isDirectory));
           
            if ((name == null)||(name.Length ==0))
            {
                name = GenerateNewFileName(parentId, isDirectory);
            }
            bool nameValid = IsNameValid(name);
            if (parentValid && nameValid)
            {
                int id = GetNextFileId();
                newFile = new File(id, parentId, name, isDirectory);
                if (newFile != null)
                {
                    parent.children.Add(newFile);
                    OnNewFile(newFile);
                }
            }
            return newFile;
        }

        abstract public void UpdateFileContents(int fileId, byte[] contents);

        // OnNewSyncFile is different from NewFile in that it is 
        // adding the File record to the model because it already
        // exists on the repository.  NewFile is for creating 
        // new files in the model and the repository. 
        protected File OnNewSyncFile(File parent, string name, bool isDirectory)
        {
            File newFile = null;
            if (parent.isDirectory)
            {
                int id = GetNextFileId();
                newFile = new File(id, parent.id, name, isDirectory);
                if (newFile != null)
                {
                    parent.children.Add(newFile);
                }
            }
            return newFile;
        }


        //delete
        //If the identified file is a directoty, it will recurse through its children.
        public bool DeleteFile(int fileId)
        {
            bool success = false;

            File toDelete = GetFile(fileId);
            if (toDelete != null)
            {
                success = DeleteFile(toDelete);
            }
            return success;
        }

        protected bool DeleteFile (File toDelete)
        {
            bool success = false;
            if (!toDelete.isRoot) // do not delete root
            {
                File parent = GetFile(toDelete.parentId);
                if (parent != null)
                {
                    if (toDelete.isDirectory)
                    {
                        // recure to delete the children
                        // dont use enumerator since we will be removing items as we go
                        while (toDelete.children.Count > 0)
                        {
                            File lastChild = toDelete.children.Last();
                            DeleteFile(lastChild);
                            // even if the child hasn't deleted weel, continue on
                        }
                    }
                    OnDeleteFile(toDelete);
                    parent.children.Remove(toDelete);
                }
            }           
            return success;
        }
        
        protected abstract bool IsNameValid(string name);
        protected abstract string GenerateNewFileName(int parentId, bool isDirectory);
        protected abstract int GetRootFileId();
        protected abstract int GetNextFileId();
        
        protected abstract void OnNewFile(File newFile);
        protected abstract void OnDeleteFile(File oldFile);
    }
}