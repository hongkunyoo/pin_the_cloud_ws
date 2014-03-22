using PintheCloudWS.Common;
using PintheCloudWS.Locale;
using PintheCloudWS.Models;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
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
    public sealed partial class SpotListPage : PtcPage
    {
        // Instances
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public SpotViewModel NearSpotViewModel = new SpotViewModel();



        /// <summary>
        /// 이는 강력한 형식의 뷰 모델로 변경될 수 있습니다.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        public SpotListPage()
        {
            this.InitializeComponent();
            uiSpotGridView.DataContext = this.NearSpotViewModel;
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

            // Remove Splash Page from Back Stack
            if(this.Frame.BackStack.Count > 0)
                this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
            this.SetSpotGridView(AppResources.Loading);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region UI Methods

        private void uiSpotGridView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            // Get Selected Spot View Item
            SpotViewItem spotViewItem = e.ClickedItem as SpotViewItem;

            //if (spotViewItem.IsPrivateImage.Equals(FileObjectViewModel.IS_PRIVATE_IMAGE_URI))
            //{
            //    SubmitSpotPasswordPopup submitSpotPasswordPopup =
            //        new SubmitSpotPasswordPopup(this.SubmitSpotPasswordParentPopup, spotViewItem.SpotId, spotViewItem.SpotPassword,
            //            uiPickPivot.ActualWidth, uiPickPivot.ActualHeight, uiPivotTitleGrid.ActualHeight);
            //    this.SubmitSpotPasswordParentPopup.Child = submitSpotPasswordPopup;
            //    this.SubmitSpotPasswordParentPopup.IsOpen = true;
            //    this.SubmitSpotPasswordParentPopup.Closed += (senderObject, args) =>
            //    {
            //        if (((SubmitSpotPasswordPopup)((Popup)senderObject).Child).result)
            //            this.NavigateToFileListPage(spotViewItem);
            //    };
            //}
            //else
            //{
            //    this.NavigateToFileListPage(spotViewItem);
            //}
            this.Frame.Navigate(typeof(ExplorerPage), spotViewItem);
        }


        private void uiSettingButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        #endregion


        #region Private Methods

        private void SetSpotGridView(string message)
        {
            // If Internet available, Set spot list
            // Otherwise, show internet bad message
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!this.NearSpotViewModel.IsDataLoaded)  // Mutex check
                    this.SetSpotGridViewItemAsync(message);
            }
            else
            {
                base.ShowMessageDialog(AppResources.InternetUnavailableMessage);
            }
        }


        private async void SetSpotGridViewItemAsync(string message)
        {
            // Show Progress Indicator
            base.SetProgressRing(uiSpotListProgressRing, true);

            // Check whether GPS works well or not
            try
            {
                Geoposition currentGeoposition = await App.Geolocator.GetGeopositionAsync();
                if (currentGeoposition != null)  // GPS works well
                {
                    // If there is near spots, Clear and Add spots to list
                    // Otherwise, Show none message.
                    List<SpotObject> spots = await App.SpotManager.GetNearSpotListAsync(currentGeoposition);

                    if (spots != null)
                    {
                        if (spots.Count > 0)  // There are near spots
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                this.NearSpotViewModel.IsDataLoaded = true;
                                this.NearSpotViewModel.SetItems(spots);
                            });
                        }
                        else  // No near spots
                        {
                            this.NearSpotViewModel.IsDataLoaded = true;
                            base.ShowMessageDialog(AppResources.NoNearSpotMessage);
                        }
                    }
                    else
                    {
                        base.ShowMessageDialog(AppResources.BadLoadingSpotMessage);
                    }
                }
                else  // GPS works bad
                {
                    base.ShowMessageDialog(AppResources.BadLocationServiceMessage);
                }
            }
            catch (UnauthorizedAccessException)
            {
                base.ShowGeolocatorStatusMessageDialog();
            }
            catch (Exception)
            {
                base.ShowGeolocatorStatusMessageDialog();
            }

            // Hide progress indicator
            base.SetProgressRing(uiSpotListProgressRing, false);
        }



        #endregion
    }
}
