﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessTodo.Classes
{
    class FileSystem
    {
        public bool deleteTodoListFile(string name)
        {
            bool res = false;
            string tmp = name.Replace("_", "").Replace("[PTD] - ", "") + ".json";

            MessageBox.Show(tmp);

            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            string[] files = Directory.GetFiles(workingDirectory, tmp, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                try
                {
                    File.Delete(files.First());
                    res = true;
                }
                catch
                {
                    res = false;
                }
            }
            return res;
        }
    }
}