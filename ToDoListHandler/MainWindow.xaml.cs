﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ToDoListHandler.Classes;
using ToDoListHandler.Classes.JSON;

namespace ToDoListHandler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string callingProcess;

        private Data_userInterface data_UserInterface;
        private Data_handler data_Handler;

        private string jsonPath;

        private DataTable dataTable = new DataTable();

        public MainWindow()
        {
            try
            {
                callingProcess = Environment.GetCommandLineArgs().Last().Replace("caller=", "");
            }
            catch { }

            InitializeComponent();

            initializeGrid();

            this.jsonPath = findJsonfile();
            this.data_Handler = new Data_handler();
            this.data_UserInterface = new Data_userInterface(dataTable, this.jsonPath);

            label_todoList_title.Content = callingProcess;

            updateGrid();
        }

        private void updateGrid()
        {
            this.dataTable = data_UserInterface.updateDataTable();
            
        }

        private void saveGrid()
        {
            //Get the List from the grid
            List<TodoListClass> rows = new List<TodoListClass>();
            DataRowCollection r = dataTable.Rows;


            for (int i = 0; i < r.Count; i++)
            {
                TodoListClass todoListClass = new TodoListClass();
                todoListClass.processFullPath = r[i][0].ToString();
                todoListClass.todoItem = r[i][1].ToString();
                rows.Add(todoListClass);
            }
            
            data_Handler.saveJsonObject(rows, this.jsonPath);
        }

        private void initializeGrid()
        {
            dataTable.Columns.Add("Process", typeof(string));
            dataTable.Columns.Add("Item", typeof(string));

            dataGrid_todo.DataContext = dataTable.DefaultView;
        }

        private string findJsonfile()
        {
            string callingProcessClean = callingProcess.Replace("\\", "").Replace(":", "");
            string tmpFileName = $"{callingProcessClean}.json";
            string res = "";
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(workingDirectory, tmpFileName, SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                res = files.First();
            }
            else
            {
                //No jsonFile yet. Create a new one:
                Directory.CreateDirectory(workingDirectory + "Data");
                res = workingDirectory + $"Data\\{tmpFileName}";
            }
            return res;
        }

        private void dataGrid_todo_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            saveGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveGrid();
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        private void button_maximize_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void button_minimize_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

    }
}