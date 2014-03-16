using PintheCloudWS.Common;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Managers;
using PintheCloudWS.Models;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 기본 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234237에 나와 있습니다.

namespace PintheCloudWS.Pages
{
    /// <summary>
    /// 대부분의 응용 프로그램에 공통되는 특성을 제공하는 기본 페이지입니다.
    /// </summary>
    public sealed partial class PinPage : PtcPage
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private CloudModeViewModel CloudModeViewModel = new CloudModeViewModel();
        private FileObjectViewModel FileObjectViewModel = new FileObjectViewModel();
        public List<FileObjectViewItem> SelectedFile = new List<FileObjectViewItem>();


        /// <summary>
        /// 이는 강력한 형식의 뷰 모델로 변경될 수 있습니다.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        public PinPage()
        {
            this.InitializeComponent();
            uiCloudModeComboBox.DataContext = this.CloudModeViewModel;
            uiCloudModeComboBox.SelectedIndex = Switcher.GetCurrentIndex();
        }


        #region NavigationHelper 등록

        /// 이 섹션에서 제공되는 메서드는 NavigationHelper에서
        /// 페이지의 탐색 메서드에 응답하기 위해 사용됩니다.
        /// 
        /// 페이지별 논리는 다음에 대한 이벤트 처리기에 있어야 합니다.  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// 및 <see cref="GridCS.Common.NavigationHelper.SaveState"/>입니다.
        /// 탐색 매개 변수는 LoadState 메서드 
        /// 및 이전 세션 동안 유지된 페이지 상태에서 사용할 수 있습니다.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region UI Methods

        private void uiCloudModeComboBox_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // Get index
            CloudModeViewItem cloudModeViewItem = uiCloudModeComboBox.SelectedItem as CloudModeViewItem;

            if (Switcher.GetCurrentStorage().GetStorageName().Equals(cloudModeViewItem.CloudName)) return;

            if (Switcher.GetCurrentStorage().IsSigningIn()) return;

            Switcher.SetStorageTo(cloudModeViewItem.CloudName);

            // If it is not in current cloud mode, change it.

            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            this.FileObjectViewModel.IsDataLoaded = false;
            this.SetPinFileList(iStorageManager, App.ResourceLoader.GetString(ResourcesKeys.Loading), false);
        }

        private void uiPinFileListUpButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            if (iStorageManager.GetFolderRootTree().Count > 1)
                this.TreeUp(iStorageManager);
        }

        #endregion

        #region

        private void TreeUp(IStorageManager iStorageManager)
        {
            // Clear trees.
            iStorageManager.GetFolderRootTree().Pop();
            iStorageManager.GetFoldersTree().Pop();
            this.SelectedFile.Clear();

            // Set previous files to list.
            this.FileObjectViewModel.SetItems(iStorageManager.GetFoldersTree().First(), true);
            this.SetCurrentPath(iStorageManager);
        }


        protected void SetPinFileList(IStorageManager iStorageManager, string message, bool load)
        {
            // If it wasn't already signed in, show signin button.
            // Otherwise, load files
            if (!iStorageManager.IsSignIn())  // wasn't signed in.
            {
                iStorageManager.GetFolderRootTree().Clear();
                iStorageManager.GetFoldersTree().Clear();
                this.SelectedFile.Clear();
            }
            else  // already signed in.
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    if (this.FileObjectViewModel.IsDataLoaded)
                    {
                        Stack<FileObjectViewItem> folderRootStack = iStorageManager.GetFolderRootTree();
                        if (folderRootStack.Count > 0)
                            this.SetPinFileListAsync(iStorageManager, message, folderRootStack.First(), load);
                        else
                            this.SetPinFileListAsync(iStorageManager, message, null, true);
                    }
                }
                else
                {
                    base.ShowMessageDialog(App.ResourceLoader.GetString(ResourcesKeys.InternetUnavailableMessage));
                }
            }
        }


        private async void SetPinFileListAsync(IStorageManager iStorageManager,string message, FileObjectViewItem folder, bool load)
        {
            // Set Mutex true and Show Process Indicator
            this.FileObjectViewModel.IsDataLoaded = true;
            base.SetProgressRing(uiFileListProgressRing, true);

            // Wait task
            await App.TaskHelper.WaitSignInTask(iStorageManager.GetStorageName());
            await App.TaskHelper.WaitSignOutTask(iStorageManager.GetStorageName());

            // If it wasn't signed out, set list.
            // Othersie, show sign in grid.
            if (iStorageManager.GetStorageAccount() != null)  // Wasn't signed out.
            {
                // If it has to load, load new files.
                // Otherwise, set existing files to list.
                List<FileObject> fileObjects = new List<FileObject>();
                if (load)  // Load from server
                {
                    // If folder null, set root.
                    if (folder == null)
                    {
                        iStorageManager.GetFolderRootTree().Clear();
                        iStorageManager.GetFoldersTree().Clear();

                        FileObject rootFolder = await iStorageManager.GetRootFolderAsync();
                        folder = new FileObjectViewItem();
                        folder.Id = rootFolder.Id;
                    }

                    // Get files and push to stack tree.
                    fileObjects = await iStorageManager.GetFilesFromFolderAsync(folder.Id);
                    if (!message.Equals(App.ResourceLoader.GetString(ResourcesKeys.Refrshing)))
                    {
                        iStorageManager.GetFoldersTree().Push(fileObjects);
                        if (!iStorageManager.GetFolderRootTree().Contains(folder))
                            iStorageManager.GetFolderRootTree().Push(folder);
                    }
                }
                else  // Set existed file to list
                {
                    fileObjects = iStorageManager.GetFoldersTree().First();
                }


                // If didn't change cloud mode while loading, set it to list.
                if (iStorageManager.GetStorageName().Equals(iStorageManager.GetStorageName()))
                {
                    // Set file list visible and current path.
                    await base.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.SetCurrentPath(iStorageManager);
                        this.FileObjectViewModel.SetItems(fileObjects, true);
                    });

                    // If there exists file, show it.
                    // Otherwise, show no file message.
                    if (fileObjects.Count > 0)
                    {
                        // Set List
                    }
                    else
                    {
                        base.ShowMessageDialog(App.ResourceLoader.GetString(ResourcesKeys.NoFileInFolderMessage));
                    }
                }
            }
            else  // Was signed out.
            {
                // Show Sign Button
            }

            // Set Mutex false and Hide Process Indicator
            base.SetProgressRing(uiFileListProgressRing, false);
        }

        private void SetCurrentPath(IStorageManager iStorageManager)
        {
            FileObjectViewItem[] array = iStorageManager.GetFolderRootTree().Reverse<FileObjectViewItem>().ToArray<FileObjectViewItem>();
            uiPinFileCurrentPathText.Text = String.Empty;
            foreach (FileObjectViewItem f in array)
                uiPinFileCurrentPathText.Text = uiPinFileCurrentPathText.Text + f.Name + "/";
        }

        #endregion
    }
}
