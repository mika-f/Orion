﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Microsoft.Toolkit.Uwp.UI.Controls;

using Orion.Shared.Models;
using Orion.UWP.Models;
using Orion.UWP.Mvvm;
using Orion.UWP.Services.Interfaces;
using Orion.UWP.ViewModels.Contents;
using Orion.UWP.ViewModels.Timelines;
using Orion.UWP.Views.Dialogs;

using Prism.Windows.Navigation;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Orion.UWP.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        private readonly IAccountService _accountService;
        private readonly IDialogService _dialogService;
        private readonly ITimelineService _timelineService;

        public ReadOnlyReactiveCollection<TimelineViewModel> Timelines { get; }
        public ReactiveProperty<TimelineViewModel> SelectedTimeline { get; }
        public ReactiveProperty<HamburgerMenuItem> SelectedMiddleItem { get; }
        public ReactiveProperty<HamburgerMenuItem> SelectedOptions { get; }

        public MainPageViewModel(GlobalNotifier globalNotifier, IAccountService accountService, IDialogService dialogService, ITimelineService timelineService)
        {
            _accountService = accountService;
            _dialogService = dialogService;
            _timelineService = timelineService;

            globalNotifier.ObserveProperty(w => w.TimelineAreaHorizontalOffset).Subscribe(w => HorizontalOffset = w).AddTo(this);
            SelectedTimeline = new ReactiveProperty<TimelineViewModel>();
            SelectedTimeline.Where(w => w != null).Subscribe(w =>
            {
                var offset = Timelines.IndexOf(w) * 1 + 1d;
                if (offset <= 1)
                    offset = 0;
                HorizontalOffset = offset;
            }).AddTo(this);
            SelectedMiddleItem = new ReactiveProperty<HamburgerMenuItem>();
            SelectedOptions = new ReactiveProperty<HamburgerMenuItem>();
            SelectedMiddleItem.Merge(SelectedOptions).Where(w => w?.TargetPageType != null).Subscribe(w =>
            {
                if (w.TargetPageType.Name.EndsWith("Dialog"))
                    _dialogService.ShowDialogAsync(w.TargetPageType);
                else if (w.TargetPageType.Name.EndsWith("Page"))
                    NavigationService.Navigate(w.TargetPageType);
            }).AddTo(this);
            Timelines = _timelineService.Timelines.ToReadOnlyReactiveCollection(w =>
            {
                if (w is StatusesTimeline timeline)
                    return new StatusesTimelineViewModel(globalNotifier, dialogService, timelineService, timeline) as TimelineViewModel;
                return null;
            }).AddTo(this);
        }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            if (_accountService.Accounts.Count == 0)
                await _dialogService.ShowDialogAsync<AuthorizationDialog>();
            else
                await _timelineService.RestoreAsync();
            DefaultAccount = new AccountViewModel(_accountService.Accounts.First(w => w.IsMarkAsDefault));
        }

        #region DefaultAccount

        private AccountViewModel _defaultAccount;

        public AccountViewModel DefaultAccount
        {
            get => _defaultAccount;
            set => SetProperty(ref _defaultAccount, value);
        }

        #endregion

        #region HorizontalOffset

        private double _horizontalOffset;

        public double HorizontalOffset
        {
            get => _horizontalOffset;
            set => SetProperty(ref _horizontalOffset, value);
        }

        #endregion
    }
}