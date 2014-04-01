﻿using PintheCloudWS.Helpers;
using PintheCloudWS.Managers;
using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Utilities
{
    public static class StorageExplorer
    {
        private static Dictionary<string, FileObject> DictionaryRoot = new Dictionary<string, FileObject>();
        private static Dictionary<string, Stack<FileObject>> DictionaryTree = new Dictionary<string, Stack<FileObject>>();
        //private static Dictionary<string, string> DictionaryKey = new Dictionary<string, string>();
        public static string STORAGE_EXPLORER_SYNC = "STORAGE_EXPLORER_SYNC";
        private static string SYNC_KEYS = "SYNC_KEYS";
        private static string ROOT_ID = "ROOT_ID";

     
        public async static Task<bool> Synchronize(string key)
        {
            IStorageManager Storage = StorageHelper.GetStorageManager(key);
            bool result = await TaskHelper.WaitSignInTask(Storage.GetStorageName());
            if (result == false) return false;
            // Fetching from SQL
            if (false && App.ApplicationSettings.Contains(SYNC_KEYS + key))
            {
                System.Diagnostics.Debug.WriteLine("Fetching From SQL");
                try
                {
                    //var dbpath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, key + "data.db3");
                    //using (var db = new SQLite.SQLiteConnection(dbpath))
                    //{
                    //    // Create the tables if they don't exist
                    //    db.DropTable<FileObjectSQL>();
                    //    db.CreateTable<FileObjectSQL>();
                    //    db.Commit();

                    //    var rootDB = from fos in db.Table<FileObjectSQL>() where fos.ParentId.Equals(ROOT_ID) select fos;

                    //    List<FileObjectSQL> getsqlList = rootDB.ToList<FileObjectSQL>();

                    //    if (getsqlList.Count != 1) System.Diagnostics.Debugger.Break();

                    //    FileObjectSQL rootFos = getsqlList.First<FileObjectSQL>();

                    //    FileObject rootFolder = FileObject.ConvertToFileObject(db, rootFos);

                    //    if (DictionaryRoot.ContainsKey(key))
                    //    {
                    //        DictionaryRoot.Remove(key);
                    //    }
                    //    DictionaryRoot.Add(key, rootFolder);

                    //    Stack<FileObject> stack = new Stack<FileObject>();
                    //    stack.Push(rootFolder);
                    //    if (DictionaryTree.ContainsKey(key))
                    //    {
                    //        DictionaryTree.Remove(key);
                    //    }
                    //    DictionaryTree.Add(key, stack);

                    //    //db.Dispose();
                    //    //db.Close();
                    //}



                    //using (FileObjectDataContext db = new FileObjectDataContext("isostore:/" + key + "_db.sdf"))
                    //{
                    //    if (!db.DatabaseExists())
                    //    {
                    //        App.ApplicationSettings.Remove(SYNC_KEYS + key);
                    //        return await Synchronize(key);
                    //    }
                    //    var rootDB = from FileObjectSQL fos in db.FileItems where fos.ParentId.Equals(ROOT_ID) select fos;

                    //    List<FileObjectSQL> getsqlList = rootDB.ToList<FileObjectSQL>();

                    //    if (getsqlList.Count != 1) System.Diagnostics.Debugger.Break();

                    //    FileObjectSQL rootFos = getsqlList.First<FileObjectSQL>();

                    //    FileObject rootFolder = FileObject.ConvertToFileObject(db, rootFos);

                    //    if (DictionaryRoot.ContainsKey(key))
                    //    {
                    //        DictionaryRoot.Remove(key);
                    //    }
                    //    DictionaryRoot.Add(key, rootFolder);

                    //    Stack<FileObject> stack = new Stack<FileObject>();
                    //    stack.Push(rootFolder);
                    //    if (DictionaryTree.ContainsKey(key))
                    //    {
                    //        DictionaryTree.Remove(key);
                    //    }
                    //    DictionaryTree.Add(key, stack);
                    //}
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    System.Diagnostics.Debugger.Break();
                    return false;
                }
            }
            // Fetching from Server
            else
            {
                System.Diagnostics.Debug.WriteLine("Fetching From Server");
                if (Storage.IsSignIn())
                {
                    FileObject rootFolder = await Storage.Synchronize();
                    if (DictionaryRoot.ContainsKey(key))
                    {
                        DictionaryRoot.Remove(key);
                    }
                    DictionaryRoot.Add(key, rootFolder);

                    Stack<FileObject> stack = new Stack<FileObject>();
                    stack.Push(rootFolder);
                    if (DictionaryTree.ContainsKey(key))
                    {
                        DictionaryTree.Remove(key);
                    }
                    DictionaryTree.Add(key, stack);

                    ////////////////////////////////////////////
                    // Saving to SQL job
                    ////////////////////////////////////////////
                    //try
                    //{
                    //    var dbpath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, key + "data.db3");
                    //    System.Diagnostics.Debug.WriteLine(dbpath);
                    //    using (var db = new SQLite.SQLiteConnection("Data Source=database.db"))
                    //    {
                    //        // Create the tables if they don't exist
                    //        db.DropTable<FileObjectSQL>();
                    //        db.CreateTable<FileObjectSQL>();
                    //        db.Commit();


                    //        List<FileObjectSQL> sqlList = new List<FileObjectSQL>();

                    //        FileObject.ConvertToFileObjectSQL(sqlList, rootFolder, ROOT_ID);

                    //        for (var i = 0; i < sqlList.Count; i++)
                    //        {
                    //            db.Insert(sqlList[i]);
                    //        }
                    //        db.Commit();

                    //    }
                    //    using (FileObjectDataContext db = new FileObjectDataContext("isostore:/" + key + "_db.sdf"))
                    //    {
                    //        if (db.DatabaseExists())
                    //        {
                    //            db.DeleteDatabase();
                    //        }
                    //        db.CreateDatabase();


                    //        List<FileObjectSQL> sqlList = new List<FileObjectSQL>();

                    //        FileObject.ConvertToFileObjectSQL(sqlList, rootFolder, ROOT_ID);

                    //        for (var i = 0; i < sqlList.Count; i++)
                    //        {
                    //            db.FileItems.InsertOnSubmit(sqlList[i]);
                    //        }

                    //        db.SubmitChanges();
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    System.Diagnostics.Debug.WriteLine(e.ToString());
                    //    System.Diagnostics.Debugger.Break();
                    //}

                    App.ApplicationSettings[SYNC_KEYS + key] = true;
                    App.ApplicationSettings.Save();
                    return true;
                }
                return false;
            }
        }

        public async static Task<bool> Refresh()
        {
            string key = Switcher.GetCurrentStorage().GetStorageName();
            App.ApplicationSettings.Remove(SYNC_KEYS + key);
            return await Synchronize(key);
        }

        public static void RemoveKey(string key)
        {
            if (App.ApplicationSettings.Contains(SYNC_KEYS + key))
                App.ApplicationSettings.Remove(SYNC_KEYS + key);
        }

        public static void RemoveAllKeys()
        {
            using (var itr = StorageHelper.GetStorageEnumerator())
            {
                while (itr.MoveNext())
                {
                    string key = itr.Current.GetStorageName();
                    if (App.ApplicationSettings.Contains(SYNC_KEYS + key))
                        App.ApplicationSettings.Remove(SYNC_KEYS + key);
                }
            }

        }

        public static List<FileObject> GetFilesFromRootFolder()
        {
            if (GetCurrentRoot() == null) System.Diagnostics.Debugger.Break();
            if (GetCurrentRoot().FileList == null) System.Diagnostics.Debugger.Break();

            GetCurrentTree().Clear();
            Stack<FileObject> stack = new Stack<FileObject>();
            GetCurrentTree().Push(GetCurrentRoot());
            return GetCurrentRoot().FileList;
        }


        public static List<FileObject> GetTreeForFolder(FileObject folder)
        {
            if (folder.FileList == null) System.Diagnostics.Debugger.Break();
            List<FileObject> list = folder.FileList;
            if (!GetCurrentTree().Contains(folder))
                GetCurrentTree().Push(folder);
            if (list == null) System.Diagnostics.Debugger.Break();
            return list;
        }


        public static List<FileObject> TreeUp()
        {
            if (GetCurrentTree().Count > 1)
            {
                GetCurrentTree().Pop();
                return GetTreeForFolder(GetCurrentTree().First());
            }
            return null;
        }


        public static string GetCurrentPath()
        {
            FileObject[] array = GetCurrentTree().Reverse<FileObject>().ToArray<FileObject>();
            string str = String.Empty;
            foreach (FileObject f in array)
            {
                str = str + f.Name + "/";
                System.Diagnostics.Debug.WriteLine(f.Name);
            }

            return str;
        }


        public static FileObject GetCurrentRoot()
        {
            if (DictionaryRoot.ContainsKey(Switcher.GetCurrentStorage().GetStorageName()))
                return DictionaryRoot[Switcher.GetCurrentStorage().GetStorageName()];
            else
                return null;
        }

        public static Stack<FileObject> GetCurrentTree()
        {
            if (DictionaryTree.ContainsKey(Switcher.GetCurrentStorage().GetStorageName()))
                return DictionaryTree[Switcher.GetCurrentStorage().GetStorageName()];
            else
                return null;
        }
    }
}
