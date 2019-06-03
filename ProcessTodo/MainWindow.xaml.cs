﻿using ProcessTodo.Classes;
using ProcessTodo.Classes.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32.TaskScheduler;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ProcessTodo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 

    //TODO: Scale all UI elements up

    

    public partial class MainWindow : Window
    {
        private readonly TaskSched_Handler t_handler;
        public MainWindow()
        {
            InitializeComponent();

            Check_auditpol();

            t_handler = new TaskSched_Handler();
            UpdateList();
        }

        private void Check_auditpol()
        {
            AuditPol_HND auditPol_HND = new AuditPol_HND();

            if (!auditPol_HND.IsTrackingPolicySet())
            {
                //Need to set auditpol!
                new window_auditpol().ShowDialog();
            }
            else
            {
                //auditpol allready set
            }

        }

        private void UpdateList()
        {
            List<Task> tasks = t_handler.getTasks();

            CheckBox cb = new CheckBox();
            cb.SetResourceReference(Control.StyleProperty, "CheckBoxStyle_Dark");

            List<CheckedItem> items = new List<CheckedItem>();
            foreach (Task task in tasks)
            {
                //format the name
                string tmpName = "lb_task_item_" + task.Name.Replace("[PTD]", "").Replace(" ", "").Replace("-", "").Split('.').First();
                //items.Add(new CheckedItem() { Text = task.Name, IsChecked = false });
                cb.Content = task.Name;
                cb.IsChecked = false;
            }

            listBox_Tasks.Items.Add(cb);
            //listBox_Tasks.ItemsSource = items;
        }

        private void Button_reg_new_process_Click(object sender, RoutedEventArgs e)
        {
            //TODO: implement, einschalten der prozessüberwachung als GUI.
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Executables (*.exe)|*.exe|All Files (*.*)|*.*";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string processToRegister = dlg.FileName;
                bool exec = t_handler.CreateTask(processToRegister, "[PTD] - " + processToRegister.Replace("\\", "_").Replace(":", "_"));
                if (exec)
                {
                    //Goood
                }
                else
                {
                    MessageBox.Show("Task Registering Failed.");
                }
            }
            UpdateList();
        }

        private void Button_delete_selected_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedTaskNames = new List<string>();

            foreach (CheckedItem ci in listBox_Tasks.Items)
            {
                if (ci.IsChecked)
                    selectedTaskNames.Add(ci.Text);
            }
            int counter = 0;
            foreach (string t in selectedTaskNames)
            {
                t_handler.DeleteTask(t);

                if (new FileSystem().DeleteTodoListFile(t))
                    counter++;

            }
            MessageBox.Show($"Deleted {counter} Todo-Lists");
            UpdateList();
        }

        private void Button_reg_new_process_Copy_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBox_Tasks.Items)
            {
                CheckedItem tmp = (CheckedItem)item;
                if (tmp.IsChecked)
                {
                    if (!t_handler.RunTask(tmp.Text))
                    {
                        MessageBox.Show("Task could not be executed");
                    }
                }
            }
        }

        private void Switch_window_size()
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


        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        private void Button_maximize_Click(object sender, RoutedEventArgs e)
        {
            Switch_window_size();
        }

        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void Button_minimize_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                Switch_window_size();

        }
    }
}
