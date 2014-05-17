using PintheCloudWS.Common;
using PintheCloudWS.Converters;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Managers;
using PintheCloudWS.Models;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class ExplorerPage : PtcPage
    {
        //private const string SELECTED_EXPLORER_INDEX_KEY = "SELECTED_EXPLORER_INDEX_KEY";

        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private FileObjectViewModel PickFileObjectViewModel = new FileObjectViewModel();
        private List<FileObjectViewItem> PickSelectedFileList = new List<FileObjectViewItem>();

        private FileObjectViewModel PinFileObjectViewModel = new FileObjectViewModel();
        private List<FileObject> CurrentFileObjectList = new List<FileObject>();
        private List<FileObjectViewItem> PinSelectedFileList = new List<FileObjectViewItem>();

        private CloudModeViewModel CloudModeViewModel = new CloudModeViewModel();

        private SpotViewItem CurrentSpotViewItem = null;
        private SpotObject CurrentSpot = null;



        /// <summary>
        /// 이는 강력한 형식의 뷰 모델로 변경될 수 있습니다.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        public ExplorerPage()
        {
            this.InitializeComponent();

            // Set Datacontext
            uiPickFileList.DataContext = this.PickFileObjectViewModel;
            uiPinFileListGridView.DataContext = this.PinFileObjectViewModel;
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

            // Get Parameter
            this.CurrentSpotViewItem = e.Parameter as SpotViewItem;

            // Set this explorer page.
            uiSpotNameText.Text = this.CurrentSpotViewItem.SpotName;
            uiAccountNameText.Text = this.CurrentSpotViewItem.AccountName;
            Switcher.SetStorageTo(Switcher.GetMainStorage().GetStorageName());
            this.CurrentSpot = App.SpotManager.GetSpotObject(this.CurrentSpotViewItem.SpotId);

            uiExplorerList.SelectedIndex = EventHelper.PICK_PIVOT;
            this.SetPickPivot(AppResources.Loading);
            this.SetPinPivot(AppResources.Loading);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        #region UI Methods

        private void uiExplorerList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // Set View model for dispaly. One time loading.
            App.ApplicationSessions[PIVOT_KEY] = uiExplorerList.SelectedIndex;

            switch (uiExplorerList.SelectedIndex)
            {
                case EventHelper.PICK_PIVOT:
                    // Set Pick Pivot UI
                    uiPickPivotGrid.Visibility = Visibility.Visible;
                    uiPinPivotGrid.Visibility = Visibility.Collapsed;
                    uiPinAppBarButton.Visibility = Visibility.Collapsed;
                    this.SetPickPivot(AppResources.Loading);
                    break;


                case EventHelper.PIN_PIVOT:
                    // Set Pin Pivot UI
                    uiPickPivotGrid.Visibility = Visibility.Collapsed;
                    uiPinPivotGrid.Visibility = Visibility.Visible;
                    IStorageManager iStorageManager = Switcher.GetCurrentStorage();
                    uiPickDeleteAppBarButton.Visibility = Visibility.Collapsed;
                    uiPinAppBarButton.Visibility = Visibility.Visible;

                    // TODO Wait signin and change cloud mode combobox name

                    this.SetPinPivot(AppResources.Loading);
                    break;
            }
        }


        private void uiRefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            switch (uiExplorerList.SelectedIndex)
            {
                case EventHelper.PICK_PIVOT:
                    this.PickFileObjectViewModel.IsDataLoaded = false;
                    this.SetPickPivot(AppResources.Refreshing);
                    break;


                case EventHelper.PIN_PIVOT:
                    this.PinFileObjectViewModel.IsDataLoaded = false;
                    TaskHelper.AddTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName(), StorageExplorer.Refresh());
                    this.SetPinPivot(AppResources.Refreshing);
                    break;
            }
        }


        private void uiPickFileList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            // Get Selected File Obejct
            FileObjectViewItem fileObjectViewItem = e.ClickedItem as FileObjectViewItem;

            // Launch files to other reader app.
            if (NetworkInterface.GetIsNetworkAvailable())
                this.LaunchFileAsync(fileObjectViewItem);
            else
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage, OK_MODE);
        }


        private void uiCloudModeComboBox_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // Get index
            CloudModeViewItem cloudModeViewItem = uiCloudModeComboBox.SelectedItem as CloudModeViewItem;

            if (Switcher.GetCurrentStorage().GetStorageName().Equals(cloudModeViewItem.CloudName)) return;
            if (Switcher.GetCurrentStorage().IsSigningIn()) return;
            Switcher.SetStorageTo(cloudModeViewItem.CloudName);

            // If it is not in current cloud mode, change it.
            uiPinFileCurrentPath.Text = "";
            base.SetProgressRing(uiPinPivotProgressRing, false);
            this.PinFileObjectViewModel.IsDataLoaded = false;
            this.SetPinPivot(AppResources.Loading);
        }


        private void uiPinFileListUpButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!Switcher.GetCurrentStorage().IsSignIn())
            {
                Switcher.GetCurrentStorage().SignIn();
            }
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
                    base.SetProgressRing(uiPinPivotProgressRing, true);
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
                    base.SetProgressRing(uiPinPivotProgressRing, false);
                }
            }
            else
            {
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage, OK_MODE);
            }
        }


        private void uiPickDeleteAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                base.ShowMessageDialog(AppResources.DeleteFileMessage, OK_CANCEL_MODE, () => 
                {
                    uiPickDeleteAppBarButton.IsEnabled = false;
                    foreach (FileObjectViewItem fileObjectViewItem in this.PickSelectedFileList)
                        this.DeleteFileAsync(fileObjectViewItem);
                    this.PickSelectedFileList.Clear();
                });
            }
            else
            {
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage, OK_MODE);
            }
        }


        private void uiPinAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                this.uiPinAppBarButton.IsEnabled = false;
                foreach (FileObjectViewItem fileObjectViewItem in this.PinSelectedFileList)
                    this.PinFileAsync(fileObjectViewItem);
                this.PinSelectedFileList.Clear();
            }
            else
            {
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage, OK_MODE);
            }
        }

        #endregion



        #region Private Methods

        private void SetPickPivot(string message)
        {
            // If internet is on, refresh
            // Otherwise, show internet unavailable message.
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!this.PickFileObjectViewModel.IsDataLoaded)
                    this.SetPickFileListAsync(message);
            }
            else
            {
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = AppResources.InternetUnavailableMessage;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            }
        }


        private async void SetPickFileListAsync(string message)
        {
            // Show Progress Indicator
            base.SetProgressRing(uiPickPivotFileListProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = message;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            });

            try
            {
                // Get files from the spot and set it to list.
                List<FileObject> fileList = await this.CurrentSpot.ListFileObjectsAsync();
                this.PickFileObjectViewModel.IsDataLoaded = true;
                if (fileList.Count > 0)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPickFileList.Visibility = Visibility.Visible;
                        uiPickFileListMessage.Visibility = Visibility.Collapsed;
                        this.PickFileObjectViewModel.SetItems(fileList, false);

                        // Set List Edit View Button
                        if (this.CurrentSpotViewItem.AccountId.Equals(App.AccountManager.GetPtcId()))
                            uiPickFileListEditViewButton.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPickFileList.Visibility = Visibility.Collapsed;
                        uiPickFileListMessage.Text = AppResources.NoFileInSpotMessage;
                        uiPickFileListMessage.Visibility = Visibility.Visible;
                    });
                }
            }
            catch
            {
                this.PickFileObjectViewModel.IsDataLoaded = true;
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = AppResources.BadLoadingFileMessage;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            }


            // Hide Progress Indicator
            base.SetProgressRing(uiPickPivotFileListProgressRing, false);
        }


        private async void LaunchFileAsync(FileObjectViewItem fileObjectViewItem)
        {
            // Show Downloading message
            base.SetProgressRing(uiPickPivotFileListProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                fileObjectViewItem.SelectFileImage = FileObjectViewModel.DOWNLOAD_IMAGE_URI;
                fileObjectViewItem.SelectFileImagePress = false;
            });

            try
            {
                // Download file and Launch files to other reader app.
                await this.CurrentSpot.PreviewFileObjectAsync(this.CurrentSpot.GetFileObject(fileObjectViewItem.Id));
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    fileObjectViewItem.SelectFileImagePress = true;
                });
            }
            catch
            {
                fileObjectViewItem.SelectFileImage = FileObjectViewModel.FAIL_IMAGE_URI;
            }

            // Hide Progress Indicator
            base.SetProgressRing(uiPickPivotFileListProgressRing, false);
        }


        private async void DeleteFileAsync(FileObjectViewItem fileObjectViewItem)
        {
            // Show Deleting message
            base.SetProgressRing(uiPickPivotFileListProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                fileObjectViewItem.SelectFileImage = FileObjectViewModel.ING_IMAGE_URI;
            });


            // Delete
            try
            {
                await App.BlobStorageManager.DeleteFileAsync(fileObjectViewItem.Id);
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PickFileObjectViewModel.Items.Remove(fileObjectViewItem);
                    if (this.PickFileObjectViewModel.Items.Count < 1)
                    {
                        uiPickFileListEditViewButton.Visibility = Visibility.Collapsed;
                        uiPickFileList.Visibility = Visibility.Collapsed;
                        uiPickFileListMessage.Text = AppResources.NoFileInSpotMessage;
                        uiPickFileListMessage.Visibility = Visibility.Visible;
                    }
                });
            }
            catch
            {
                    fileObjectViewItem.SelectFileImage = FileObjectViewModel.FAIL_IMAGE_URI;
            }

            // Hide Progress Indicator
            base.SetProgressRing(uiPickPivotFileListProgressRing, false);
        }


        private async void SetPinPivot(string message)
        {
            // If it wasn't already signed in, show signin button.
            // Otherwise, load files
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            if (!iStorageManager.IsSignIn())  // wasn't signed in.
            {
                this.PinSelectedFileList.Clear();

                uiPinFileListGrid.Visibility = Visibility.Collapsed;
                uiPinFileSignInPanel.Visibility = Visibility.Visible;
            }
            else  // already signed in.
            {
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
            base.SetProgressRing(uiPinPivotProgressRing, true);
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
            base.SetProgressRing(uiPinPivotProgressRing, false);
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


        private async void PinFileAsync(FileObjectViewItem fileObjectViewItem)
        {
            // Show Uploading message and file for good UX
            base.SetProgressRing(uiPinPivotProgressRing, true);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                fileObjectViewItem.SelectFileImage = FileObjectViewModel.ING_IMAGE_URI;
            });

            try
            {
                string blobId = await this.CurrentSpot.AddFileObjectAsync(this.GetCloudStorageFileObjectById(fileObjectViewItem.Id));
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PickFileObjectViewModel.IsDataLoaded = false;
                    fileObjectViewItem.SelectFileImage = FileObjectViewModel.CHECK_NOT_IMAGE_URI;
                });
            }
            catch
            {
                fileObjectViewItem.SelectFileImage = FileObjectViewModel.FAIL_IMAGE_URI;
            }

            // Hide progress message
            base.SetProgressRing(uiPinPivotProgressRing, false);
        }

        #endregion

    }
}
