using PintheCloudWS.Common;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Managers;
using PintheCloudWS.Models;
using PintheCloudWS.Utilities;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private FileObjectViewModel PinFileObjectViewModel = new FileObjectViewModel();
        private List<FileObjectViewItem> PinSelectedFileList = new List<FileObjectViewItem>();

        private List<FileObject> CurrentFileObjectList = new List<FileObject>();
        private SpotObject CurrentSpot = null;


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
            uiPinFileListGridView.DataContext = this.PinFileObjectViewModel;
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
            SpotViewItem spotViewItem = e.Parameter as SpotViewItem;
            this.CurrentSpot = App.SpotManager.GetSpotObject(spotViewItem.SpotId);
            this.SetPinPage(AppResources.Loading);

            // TODO Wait signin and change cloud mode combobox name
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
            uiPinFileCurrentPath.Text = "";
            base.SetProgressRing(uiFileListProgressRing, false);
            this.PinFileObjectViewModel.IsDataLoaded = false;
            this.SetPinPage(AppResources.Loading);
        }


        private void uiPinFileListUpButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (StorageExplorer.GetCurrentTree() != null)
                this.TreeUp();
        }


        private async void uiPinFileSignInButton_Click(object sender, RoutedEventArgs e)
        {
            // If Internet available, Set pin list with root folder file list.
            // Otherwise, show internet bad message
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // Show Loading message and save is login true for pivot moving action while sign in.
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    uiPinFileListGridView.Visibility = Visibility.Collapsed;
                    uiPinFileMessage.Text = AppResources.DoingSignIn;
                    uiPinFileMessage.Visibility = Visibility.Visible;

                    uiPinFileListGrid.Visibility = Visibility.Visible;
                    uiPinFileSignInPanel.Visibility = Visibility.Collapsed;
                });

                try
                {
                    // Sign in and await that task.
                    // If sign in success, set list.
                    base.SetProgressRing(uiFileListProgressRing, true);
                    IStorageManager iStorageManager = Switcher.GetCurrentStorage();
                    if (!iStorageManager.IsSigningIn())
                        TaskHelper.AddSignInTask(iStorageManager.GetStorageName(), iStorageManager.SignIn());
                    await TaskHelper.WaitSignInTask(iStorageManager.GetStorageName());
                    this.SetPinFileListAsync(iStorageManager, AppResources.Loading, null);
                }
                catch
                {
                    base.ShowMessageDialog(AppResources.BadSignInMessage, OK_MODE);
                    uiPinFileListGrid.Visibility = Visibility.Collapsed;
                    uiPinFileSignInPanel.Visibility = Visibility.Visible;
                    base.SetProgressRing(uiFileListProgressRing, false);
                }
            }
            else
            {
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage, OK_MODE);
            }
        }

        #endregion



        #region Private Methods

        private async void TreeUp()
        {
            try
            {
                // Wait Sync work
                await TaskHelper.WaitTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName());

                // If message is visible, set collapsed.
                if (uiPinFileMessage.Visibility == Visibility.Visible && !uiPinFileMessage.Text.Equals(AppResources.Refreshing))
                    uiPinFileMessage.Visibility = Visibility.Collapsed;

                // Do tree up work and set items to list
                List<FileObject> fileList = StorageExplorer.TreeUp();
                this.CurrentFileObjectList = fileList;
                this.PinFileObjectViewModel.SetItems(this.CurrentFileObjectList, true);
                uiPinFileCurrentPath.Text = StorageExplorer.GetCurrentPath();

                // Clear trees.
                //this.PinFileAppBarButton.IsEnabled = false;
                this.PinSelectedFileList.Clear();
            }
            catch
            {
                return;
            }
        }


        private async void SetPinPage(string message)
        {
            // If it wasn't already signed in, show signin button.
            // Otherwise, load files
            System.Diagnostics.Debug.WriteLine("NOT HERE?");
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            if (!iStorageManager.IsSignIn())  // wasn't signed in.
            {
                this.PinSelectedFileList.Clear();

                uiPinFileListGrid.Visibility = Visibility.Collapsed;
                uiPinFileSignInPanel.Visibility = Visibility.Visible;
            }
            else  // already signed in.
            {
                System.Diagnostics.Debug.WriteLine("In HERE!");
                uiPinFileListGrid.Visibility = Visibility.Visible;
                uiPinFileSignInPanel.Visibility = Visibility.Collapsed;

                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    if (!this.PinFileObjectViewModel.IsDataLoaded)
                        this.SetPinFileListAsync(iStorageManager, message, null);
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPinFileListGridView.Visibility = Visibility.Collapsed;
                        uiPinFileMessage.Text = AppResources.InternetUnavailableMessage;
                        uiPinFileMessage.Visibility = Visibility.Visible;
                    });
                }
            }
        }


        private async void SetPinFileListAsync(IStorageManager iStorageManager, string message, FileObjectViewItem folder)
        {
            // Set Mutex true and Show Process Indicator
            // Clear selected file and set pin button false.
            base.SetProgressRing(uiFileListProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                uiPinFileListGridView.Visibility = Visibility.Collapsed;
                uiPinFileMessage.Text = message;
                uiPinFileMessage.Visibility = Visibility.Visible;
            });
            this.PinSelectedFileList.Clear();

            try
            {
                // Wait Signin and Sync task
                await TaskHelper.WaitSignInTask(iStorageManager.GetStorageName());
                await TaskHelper.WaitTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName());

                // Get files from current folder in the cloud.
                // If it is not null, set items.
                // Otherwise, show message
                if (folder == null)
                    this.CurrentFileObjectList = StorageExplorer.GetFilesFromRootFolder();
                else
                    this.CurrentFileObjectList = StorageExplorer.GetTreeForFolder(this.GetCloudStorageFileObjectById(folder.Id));
                if (this.CurrentFileObjectList != null)
                {
                    // If didn't change cloud mode while loading, set it to list.
                    // Set file list visible and current path.
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.PinFileObjectViewModel.IsDataLoaded = true;
                        uiPinFileListGridView.Visibility = Visibility.Visible;
                        uiPinFileCurrentPath.Text = StorageExplorer.GetCurrentPath();
                        this.PinFileObjectViewModel.SetItems(this.CurrentFileObjectList, true);
                    });


                    // If there exists file, show it.
                    // Otherwise, show no file message.
                    if (this.CurrentFileObjectList.Count > 0)
                    {
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            uiPinFileMessage.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            uiPinFileMessage.Text = AppResources.NoFileInFolderMessage;
                        });
                    }
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPinFileListGridView.Visibility = Visibility.Collapsed;
                        uiPinFileMessage.Text = AppResources.BadLoadingFileMessage;
                        uiPinFileMessage.Visibility = Visibility.Visible;
                    });
                }
            }
            catch
            {
                uiPinFileListGridView.Visibility = Visibility.Collapsed;
                uiPinFileMessage.Text = AppResources.BadLoadingFileMessage;
                uiPinFileMessage.Visibility = Visibility.Visible;
            }
            base.SetProgressRing(uiFileListProgressRing, false);
        }


        private FileObject GetCloudStorageFileObjectById(string fileId)
        {
            if (fileId == null) return null;
            for (int i = 0; i < this.CurrentFileObjectList.Count; i++)
            {
                if (this.CurrentFileObjectList[i] == null) return null;
                if (this.CurrentFileObjectList[i].Id == null) return null;
                if (this.CurrentFileObjectList[i].Id.Equals(fileId)) return this.CurrentFileObjectList[i];
            }
            return null;
        }

        #endregion Private Methods
    }
}
