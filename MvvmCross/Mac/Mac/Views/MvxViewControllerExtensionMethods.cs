// MvxViewControllerExtensionMethods.cs

// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
//
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

namespace MvvmCross.Mac.Views
{
    using System.Collections.Generic;

    using global::MvvmCross.Core.Platform;
    using global::MvvmCross.Core.ViewModels;
    using global::MvvmCross.Core.Views;
    using global::MvvmCross.Platform;
    using global::MvvmCross.Platform.Exceptions;
    using global::MvvmCross.Platform.Platform;

    public static class MvxViewControllerExtensionMethods
    {
        public static void OnViewCreate(this IMvxMacView macView)
        {
            //var view = touchView as IMvxView<TViewModel>;
            macView.OnViewCreate(() => { return macView.LoadViewModel(); });
        }

        private static IMvxViewModel LoadViewModel(this IMvxMacView macView)
        {
            if (macView.Request == null)
            {
                MvxTrace.Trace(
                    "Request is null - assuming this is a TabBar type situation where ViewDidLoad is called during construction... patching the request now - but watch out for problems with virtual calls during construction");
                macView.Request = Mvx.Resolve<IMvxCurrentRequest>().CurrentRequest;
            }

            var instanceRequest = macView.Request as MvxViewModelInstanceRequest;
            if (instanceRequest != null)
            {
                return instanceRequest.ViewModelInstance;
            }

            var loader = Mvx.Resolve<IMvxViewModelLoader>();
            var viewModel = loader.LoadViewModel(macView.Request, null /* no saved state on iOS currently */);
            if (viewModel == null)
                throw new MvxException("ViewModel not loaded for " + macView.Request.ViewModelType);
            return viewModel;
        }

        public static IMvxMacView CreateViewControllerFor<TTargetViewModel>(this IMvxMacView view,
                                                                            object parameterObject)
            where TTargetViewModel : class, IMvxViewModel
        {
            return
                view.CreateViewControllerFor<TTargetViewModel>(parameterObject == null
                                                                   ? null
                                                                   : parameterObject.ToSimplePropertyDictionary());
        }

#warning TODO - could this move down to IMvxView level?

        public static IMvxMacView CreateViewControllerFor<TTargetViewModel>(
            this IMvxMacView view,
            IDictionary<string, string> parameterValues = null)
            where TTargetViewModel : class, IMvxViewModel
        {
            var parameterBundle = new MvxBundle(parameterValues);
            var request = new MvxViewModelRequest<TTargetViewModel>(parameterBundle, null);
            return view.CreateViewControllerFor(request);
        }

        public static IMvxMacView CreateViewControllerFor<TTargetViewModel>(
            this IMvxMacView view,
            MvxViewModelRequest request)
            where TTargetViewModel : class, IMvxViewModel
        {
            return Mvx.Resolve<IMvxMacViewCreator>().CreateView(request);
        }

        public static IMvxMacView CreateViewControllerFor(
            this IMvxMacView view,
            MvxViewModelRequest request)
        {
            return Mvx.Resolve<IMvxMacViewCreator>().CreateView(request);
        }

        public static IMvxMacView CreateViewControllerFor(
            this IMvxMacView view,
            IMvxViewModel viewModel)
        {
            return Mvx.Resolve<IMvxMacViewCreator>().CreateView(viewModel);
        }
    }
}