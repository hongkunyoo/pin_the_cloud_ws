using PintheCloudWS.Common;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
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
using Windows.Storage;
using Windows.System;
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
    public sealed partial class PickPage : PtcPage
    {   
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private FileObjectViewModel FileObjectViewModel = new FileObjectViewModel();
        private SpotObject CurrentSpot = null;


        /// <summary>
        /// 이는 강력한 형식의 뷰 모델로 변경될 수 있습니다.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public PickPage()
        {
            this.InitializeComponent();
            uiPickFileList.DataContext = this.FileObjectViewModel;
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

            // Get Parameter.
            SpotViewItem spotViewItem = e.Parameter as SpotViewItem;
            this.CurrentSpot = App.SpotManager.GetSpotObject(spotViewItem.SpotId);
            
            // Set this pick page.
            this.SetPickPage(AppResources.Loading);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        #region UI Methods
        private void uiFileListView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            // Get Selected File Obejct
            FileObjectViewItem fileObjectViewItem = e.ClickedItem as FileObjectViewItem;

            // Launch files to other reader app.
            if (NetworkInterface.GetIsNetworkAvailable())
                this.LaunchFileAsync(fileObjectViewItem);
            else
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage);
        }
        #endregion



        #region Private Methods
        private void SetPickPage(string message)
        {
            // If internet is on, refresh
            // Otherwise, show internet unavailable message.
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!this.FileObjectViewModel.IsDataLoaded)
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
            base.SetProgressRing(uiFileListProgressRing, true);
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
                this.FileObjectViewModel.IsDataLoaded = true;
                if (fileList.Count > 0)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        uiPickFileList.Visibility = Visibility.Visible;
                        uiPickFileListMessage.Visibility = Visibility.Collapsed;
                        this.FileObjectViewModel.SetItems(fileList, false);
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
                this.FileObjectViewModel.IsDataLoaded = true;
                uiPickFileList.Visibility = Visibility.Collapsed;
                uiPickFileListMessage.Text = AppResources.BadLoadingFileMessage;
                uiPickFileListMessage.Visibility = Visibility.Visible;
            }


            // Hide Progress Indicator
            base.SetProgressRing(uiFileListProgressRing, false);
        }


        private async void LaunchFileAsync(FileObjectViewItem fileObjectViewItem)
        {
            // Show Downloading message
            base.SetProgressRing(uiFileListProgressRing, true);
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
            base.SetProgressRing(uiFileListProgressRing, false);
        }
        #endregion

    }
}
