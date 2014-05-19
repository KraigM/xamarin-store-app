using System;
using MonoTouch.UIKit;

namespace XamarinStore.iOS
{
	public interface IRootViewController
	{
		void PushViewController(UIViewController viewController, bool animated);
		void PopViewController(bool animated);
		UIViewController[] PopToRootViewController (bool animated);
	}

	public class PhoneRootViewController : UINavigationController, IRootViewController
	{
		public void PopViewController(bool animated)
		{
			base.PopViewControllerAnimated (animated);
		}

		public override UIViewController[] PopToRootViewController (bool animated)
		{
			return base.PopToRootViewController (animated);
		}
	}

	public class PadRootViewController : UISplitViewController, IRootViewController
	{
		private readonly UINavigationController _Master = new UINavigationController();
		private readonly UINavigationController _Detail = new UINavigationController();
		private readonly UINavigationController _PopUp = new UINavigationController();

		private UINavigationController _Main;

		public PadRootViewController()
		{
			ViewControllers = new UIViewController[] { _Master, _Detail };

			_Main = _Detail;
		}

		public void PushViewController (UIViewController viewController, bool animated)
		{
			if (viewController is ProductListViewController) 
			{
				_Master.PushViewController (viewController, animated);
			}
			else if (viewController is BasketViewController) {
				_Main = _PopUp;
				_PopUp.PushViewController (viewController, false);
				viewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, (s,e) =>
					{
						ClosePopUp(true);
					});
				this.PresentViewControllerAsync (_PopUp, animated);
			}
			else if (viewController is ProductDetailViewController) {
				_Main.ViewControllers = new UIViewController[] { };
				_Main.PushViewController (viewController, animated);
			}
			else
			{
				_Main.PushViewController (viewController, animated);
			}
		}

		private async void ClosePopUp(bool animated)
		{
			if (_PopUp.ViewControllers.Length > 0) {
				_Main = _Detail;
				await DismissViewControllerAsync (true);
				_PopUp.PopViewControllerAnimated (false);
			}
		}

		public void PopViewController (bool animated)
		{
			_Detail.PopViewControllerAnimated (animated);
		}

		public UIViewController[] PopToRootViewController (bool animated)
		{
			ClosePopUp (animated);
			var vcs = _Detail.ViewControllers;
			_Detail.ViewControllers = new UIViewController[]{};
			return vcs;
		}
	}
}

