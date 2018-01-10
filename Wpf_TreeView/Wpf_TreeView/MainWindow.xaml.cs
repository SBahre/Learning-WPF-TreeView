using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace Wpf_TreeView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        /// <summary>
        /// when the application first opens
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            var t = new TreeViewItem();
        }
        #endregion

        #region On Loaded
        /// <summary>
        /// when th application first opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Get every logical drive on the machine
            foreach (var drive in Directory.GetLogicalDrives())
            {
                //Create a new item for it
                var item = new TreeViewItem()
                {
                    //Set the header 
                    Header = drive,
                    //and the full path
                    Tag = drive
            };

                
                //add the dummy item
                item.Items.Add(null);

                //Listen out for the item being expanded
                item.Expanded += Folder_Expanded;

                //add it to the main-tree view
                FolderView.Items.Add(item);
            }
        }

        #endregion

        #region Folder Expanded

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks
            var item = (TreeViewItem)sender;

            //if item contains only dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            //Clear Dummy data
            item.Items.Clear();

            //Get full Path
            var fullPath = (string)item.Tag;
            #endregion

            #region Get Folders

            //create a blank list of directories
            var directories = new List<string>();

            //try and get directories from the folder
            //ignore any issues doing so
            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                {
                    directories.AddRange(dirs);
                }
            }
            catch { }

            //For each directory...
            directories.ForEach(directoryPath =>
            {
                //create directory item
                var subItem = new TreeViewItem()
                {
                    //set header as full name
                    Header = GetFileFolderName(directoryPath),
                    //tag as full path
                    Tag = directoryPath
                };

                //Add dummy items so that we can expand folder
                subItem.Items.Add(null);

                subItem.Expanded += Folder_Expanded;

                //Add this item to the parent
                item.Items.Add(subItem);

            });
            #endregion

            #region Get Files
            //create a blank list of directories
            var files = new List<string>();

            //try and get files from the folder
            //ignore any issues doing doing so
            try
            {
                var oneFile = Directory.GetFiles(fullPath);

                if (oneFile.Length > 0)
                {
                    files.AddRange(oneFile);
                }
            }
            catch { }

            //For each file...
            files.ForEach(filePath =>
            {
                //create file item
                var subItem = new TreeViewItem()
                {
                    //set header as file name
                    Header = GetFileFolderName(filePath),
                    //tag as full path
                    Tag = filePath
                };

                //Add this item to the parent
                item.Items.Add(subItem);

            });
            #endregion
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Finds the file of foldername from a given full path
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            // C:\something\a folder
            // C:\something\a file.png
            // aNewFile.png

            //if we have no path return empty
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            //make all slashes back slashes
            var normalisedPath = path.Replace('/','\\');

            //Find the last backslash in the path
            var lastIndex = normalisedPath.LastIndexOf('\\');

            //if we don;t find the backslash retun the path itself
            if (lastIndex <= 0)
                return path;

            return path.Substring(lastIndex+1);
        }
        #endregion

    }
}
