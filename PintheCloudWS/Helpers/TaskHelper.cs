﻿using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Helpers
{
    public static class TaskHelper
    {
        // Tasks
        private static IDictionary<string, Task<bool>> Tasks = new Dictionary<string, Task<bool>>();
        private static Dictionary<string, Task<bool>> SignInTasks = new Dictionary<string, Task<bool>>();

        public const string STORAGE_EXPLORER_SYNC = "STORAGE_EXPLORER_SYNC";



        public static void AddTask(string name, Task<bool> task)
        {
            if (!Tasks.ContainsKey(name))
                Tasks.Add(name, task);
        }


        public static async Task<bool> WaitTask(string name)
        {
            if (Tasks.ContainsKey(name))
            {
                bool result = await Tasks[name];
                Tasks.Remove(name);
                if (result)
                    return result;
                else
                    throw new Exception();
            }
            else
            {
                return true;
            }
        }


        public static void AddSignInTask(string key, Task<bool> task)
        {
            if (!SignInTasks.ContainsKey(key))
                SignInTasks.Add(key, task);
        }


        public static async Task<bool> WaitSignInTask(string key)
        {
            if (SignInTasks.ContainsKey(key))
            {
                bool result = await SignInTasks[key];
                SignInTasks.Remove(key);
                if (result)
                    return result;
                else
                    throw new Exception();
            }
            else
            {
                return true;
            }
        }


        public static async Task<bool> WaitForAllSignIn()
        {
            bool result = true;
            using (var itr = SignInTasks.GetEnumerator())
            {
                while (itr.MoveNext())
                    result &= await itr.Current.Value;
            }
            return result;
        }
    }
}
