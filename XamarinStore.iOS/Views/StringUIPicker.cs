using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Drawing;

namespace XamarinStore
{
	public class StringUIPicker : UIPickerView
	{
		public event EventHandler SelectedItemChanged;
		string[] items;
		public IEnumerable<string> Items
		{
			get{ return items; }
			set{ 
				items = value.ToArray ();
				Model = new PickerModel {
					Items = items,
					Parent = this,
				};
			}
		}
		int currentIndex;
		public int SelectedIndex {
			get{ return currentIndex; }
			set{
				if (currentIndex == value)
					return;
				currentIndex = value;
				this.Select (currentIndex, 0, true);
				if (SelectedItemChanged != null)
					SelectedItemChanged (this, EventArgs.Empty);
			}
		}
		public string SelectedItem
		{
			get { 
				return items.Length <= currentIndex ? "" : items [currentIndex];
			}
			set {
				if(!items.Contains(value))
					return;
				currentIndex = Array.IndexOf (items, value);
			}
		}

		UIActionSheet sheet;
		UIPopoverController popover;
		public void ShowPicker(UIView viewForPicker, RectangleF location)
		{
			var toolbar = new UIToolbar (new RectangleF (0, 0, 320, 44)) {
				Items = new[] {
					new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace), 
					new UIBarButtonItem (UIBarButtonSystemItem.Done, (s, e) => 
						{
							if (sheet != null) {
								sheet.DismissWithClickedButtonIndex (0, true);
								sheet = null;
							}
							if (popover != null) {
								popover.Dismiss(true);
								popover = null;
							}
						})
				},
				BarStyle = UIBarStyle.Default,
			};
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				sheet = new UIActionSheet ();
				sheet.AddSubview (this);
				sheet.AddSubviews (toolbar);
				sheet.BackgroundColor = UIColor.Clear;
				sheet.ShowInView (viewForPicker);
				UIView.Animate (.25, () => sheet.Bounds = new RectangleF (0, 0, viewForPicker.Frame.Width, 485));
			} else {
				this.Frame = new RectangleF (0, 44, 320, 216);
				var view = new UIView ();
				view.Add (this);
				view.Add (toolbar);
				var vc = new UIViewController ();
				vc.View = view;
				vc.PreferredContentSize = new SizeF (320, 260);
				popover = new UIPopoverController (vc);
				popover.PresentFromRect (location, viewForPicker, UIPopoverArrowDirection.Down, true);
			}
		}


		class PickerModel : UIPickerViewModel
		{
			public StringUIPicker Parent { get; set; }
			public string[] Items = new string[0];

			public override int GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override int GetRowsInComponent (UIPickerView picker, int component)
			{
				return Items.Length;
			}

			public override string GetTitle (UIPickerView picker, int row, int component)
			{

				return Items [row];
			}

			public override void Selected (UIPickerView picker, int row, int component)
			{
				if (Parent != null)
					Parent.SelectedIndex = row;
			}

		}

	}
}

