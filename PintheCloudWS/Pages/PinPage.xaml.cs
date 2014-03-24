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
            uiPinFileCurrentPath.Text = "";
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            this.PinFileObjectViewModel.IsDataLoaded = false;
            this.SetPinPivot(AppResources.Loading);
        }

        private void uiPinFileListUpButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            if (StorageExplorer.GetCurrentTree() != null)
                this.TreeUp();
        }

        #endregion

        #region

        private async void TreeUp()
        {
            if (!await TaskHelper.WaitTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName())) return;

            // If message is visible, set collapsed.
            if (uiPinFileMessage.Visibility == Visibility.Visible && !uiPinFileMessage.Text.Equals(AppResources.Refrshing))
                uiPinFileMessage.Visibility = Visibility.Collapsed;

            // Clear trees.
            //this.PinFileAppBarButton.IsEnabled = false;
            
            this.PinSelectedFileList.Clear();

            // Set previous files to list.
            List<FileObject> fileList = StorageExplorer.TreeUp();
            if (fileList == null) return;

            this.CurrentFileObjectList = fileList;
            this.PinFileObjectViewModel.SetItems(this.CurrentFileObjectList, true);
            uiPinFileCurrentPath.Text = StorageExplorer.GetCurrentPath();
        }


        private void SetPinPivot(string message)
        {
            // If it wasn't already signed in, show signin button.
            // Otherwise, load files
            IStorageManager iStorageManager = Switcher.GetCurrentStorage();
            if (!iStorageManager.IsSignIn())  // wasn't signed in.
            {
                this.PinSelectedFileList.Clear();
                //this.PinFileAppBarButton.IsEnabled = false;

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
                    base.SetListUnableAndShowMessage(uiPinFileList, uiPinFileMessage, AppResources.InternetUnavailableMessage);
                }
            }
        }


        private async void SetPinFileListAsync(IStorageManager iStorageManager, string message, FileObjectViewItem folder)
        {
            // Set Mutex true and Show Process Indicator            
            base.SetListUnableAndShowMessage(uiPinFileList, uiPinFileMessage, message);
            //base.SetProgressIndicator(true);

            base.SetProgressRing(uiFileListProgressRing, true);

            // Clear selected file and set pin button false.
            this.PinSelectedFileList.Clear();
            //base.Dispatcher.BeginInvoke(() =>
            //{
            //    this.PinFileAppBarButton.IsEnabled = false;
            //});
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.PinFileAppBarButton.IsEnabled = false;
            });
            // Wait task
            //await TaskHelper.WaitTask(STORAGE_EXPLORER_SYNC);
            await TaskHelper.WaitSignOutTask(iStorageManager.GetStorageName());

            // If it wasn't signed out, set list.
            // Othersie, show sign in grid.
            if (await iStorageManager.GetStorageAccountAsync() == null)  // Wasn't signed out.
            {
                //base.Dispatcher.BeginInvoke(() =>
                //{
                //    uiPinFileListGrid.Visibility = Visibility.Collapsed;
                //    uiPinFileSignInPanel.Visibility = Visibility.Visible;
                //});
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    uiPinFileListGrid.Visibility = Visibility.Collapsed;
                    uiPinFileSignInPanel.Visibility = Visibility.Visible;
                });

                //base.SetProgressIndicator(false);
                base.SetProgressRing(uiFileListProgressRing, false);
                return;
            }

            // Get files and push to stack tree.
            Debug.WriteLine("waiting sync : " + TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName());
            bool result = await TaskHelper.WaitTask(TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName());
            Debug.WriteLine("finished sync : " + TaskHelper.STORAGE_EXPLORER_SYNC + Switcher.GetCurrentStorage().GetStorageName());
            //fileObjects = null;
            if (!result) return;
            if (folder == null)
            {
                this.CurrentFileObjectList = StorageExplorer.GetFilesFromRootFolder();
            }
            else
            {
                if (folder == null) System.Diagnostics.Debugger.Break();
                this.CurrentFileObjectList = StorageExplorer.GetTreeForFolder(this.GetCloudStorageFileObjectById(folder.Id));
            }


            //////////////////////////////////////////////////////////////////////////
            // TODO : Check Logical Error
            //////////////////////////////////////////////////////////////////////////
            if (this.CurrentFileObjectList == null) System.Diagnostics.Debugger.Break();


            // If didn't change cloud mode while loading, set it to list.
            // Set file list visible and current path.
            //base.Dispatcher.BeginInvoke(() =>
            //{
            //    this.PinFileObjectViewModel.IsDataLoaded = true;
            //    uiPinFileList.Visibility = Visibility.Visible;
            //    uiPinFileCurrentPath.Text = StorageExplorer.GetCurrentPath();
            //    this.PinFileObjectViewModel.SetItems(fileObjects, true);
            //});
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.PinFileObjectViewModel.IsDataLoaded = true;
                uiPinFileList.Visibility = Visibility.Visible;
                uiPinFileCurrentPath.Text = StorageExplorer.GetCurrentPath();
                this.PinFileObjectViewModel.SetItems(fileObjects, true);
            });

            // If there exists file, show it.
            // Otherwise, show no file message.
            if (this.CurrentFileObjectList.Count > 0)
            {
                //base.Dispatcher.BeginInvoke(() =>
                //{
                //    uiPinFileMessage.Visibility = Visibility.Collapsed;
                //});
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    uiPinFileMessage.Visibility = Visibility.Collapsed;
                });
            }
            else
            {
                //base.Dispatcher.BeginInvoke(() =>
                //{
                //    uiPinFileMessage.Text = AppResources.NoFileInFolderMessage;
                //});
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    uiPinFileMessage.Text = AppResources.NoFileInFolderMessage;
                });
            }

            // Set Mutex false and Hide Process Indicator
            //base.SetProgressIndicator(false);
            base.SetProgressRing(uiFileListProgressRing, true);
        }

        //private void SetCurrentPath(IStorageManager iStorageManager)
        //{
        //    FileObjectViewItem[] array = iStorageManager.GetFolderRootTree().Reverse<FileObjectViewItem>().ToArray<FileObjectViewItem>();
        //    uiPinFileCurrentPathText.Text = String.Empty;
        //    foreach (FileObjectViewItem f in array)
        //        uiPinFileCurrentPathText.Text = uiPinFileCurrentPathText.Text + f.Name + "/";
        //}

        private async void uiPinFileSignInButton_Click(object sender, RoutedEventArgs e)
        {
            // If Internet available, Set pin list with root folder file list.
            // Otherwise, show internet bad message
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // Show Loading message and save is login true for pivot moving action while sign in.
                base.SetListUnableAndShowMessage(uiPinFileList, uiPinFileMessage, AppResources.DoingSignIn);
                //base.Dispatcher.BeginInvoke(() =>
                //{
                //    uiPinFileListGrid.Visibility = Visibility.Visible;
                //    uiPinFileSignInPanel.Visibility = Visibility.Collapsed;
                //});
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    uiPinFileListGrid.Visibility = Visibility.Visible;
                    uiPinFileSignInPanel.Visibility = Visibility.Collapsed;
                });

                // Sign in and await that task.
                IStorageManager iStorageManager = Switcher.GetCurrentStorage();
                if (!iStorageManager.IsSigningIn())
                    TaskHelper.AddSignInTask(iStorageManager.GetStorageName(), iStorageManager.SignIn());
                bool result = await TaskHelper.WaitSignInTask(iStorageManager.GetStorageName());

                // If sign in success, set list.
                // Otherwise, show bad sign in message box.
                //base.SetProgressIndicator(true);
                base.SetProgressRing(uiFileListProgressRing, true);
                if (result)
                {
                    this.SetPinFileListAsync(iStorageManager, AppResources.Loading, null);
                }
                else
                {
                    //base.Dispatcher.BeginInvoke(() =>
                    //{
                    //    MessageBox.Show(AppResources.BadSignInMessage, AppResources.BadSignInCaption, MessageBoxButton.OK);
                    //    uiPinFileListGrid.Visibility = Visibility.Collapsed;
                    //    uiPinFileSignInPanel.Visibility = Visibility.Visible;
                    //});
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPinFileListGrid.Visibility = Visibility.Collapsed;
                        uiPinFileSignInPanel.Visibility = Visibility.Visible;
                    });
                }

                //base.SetProgressIndicator(false);
                base.SetProgressRing(uiFileListProgressRing, false);
            }
            else
            {
                //MessageBox.Show(AppResources.InternetUnavailableMessage, AppResources.InternetUnavailableCaption, MessageBoxButton.OK);
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage);
            }
        }

        #endregion
    }
}
