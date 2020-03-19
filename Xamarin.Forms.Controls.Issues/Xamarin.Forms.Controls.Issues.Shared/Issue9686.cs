﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 9686, "[Bug, CollectionView,iOS] Foundation.Monotouch Exception in Grouped CollectionView",
			PlatformAffected.iOS)]
	public class Issue9686 : TestContentPage
	{
		const string Success = "Success";
		const string Test9686 = "9686";

		protected override void Init()
		{
			var layout = new StackLayout();

			var cv = new CollectionView
			{
				IsGrouped = true
			};

			BindingContext = new _9686ViewModel();

			cv.ItemTemplate = new DataTemplate(() => {
				var label = new Label() { Margin = new Thickness(5, 0, 0, 0) };
				label.SetBinding(Label.TextProperty, new Binding("Name"));
				return label;
			});

			cv.GroupHeaderTemplate = new DataTemplate(() => {
				var label = new Label();
				label.SetBinding(Label.TextProperty, new Binding("GroupName"));

				var tgr = new TapGestureRecognizer();
				tgr.Command = ((_9686ViewModel)BindingContext).ShowOrHideCommand;
				tgr.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding("."));

				label.GestureRecognizers.Add(tgr);

				return label;
			});

			cv.SetBinding(ItemsView.ItemsSourceProperty, new Binding(nameof(_9686ViewModel.Groups)));

			var instructions = new Label { Text = $"Tap the '{Test9686}' header once, then again. The application doesn't crash, this test has passed." };

			var result = new Label { };

			layout.Children.Add(instructions);
			layout.Children.Add(result);
			layout.Children.Add(cv);

			Content = layout;
		}

		public class _9686Item
		{
			public string Name { get; set; }
		}

		public class _9686Group : List<_9686Item>
		{
			public string GroupName { get; set; }

			public _9686Group(string groupName, ObservableCollection<_9686Item> items) : base(items)
			{
				GroupName = groupName;
			}
		}

		public class _9686ViewModel
		{
			public ICommand ShowOrHideCommand { get; set; }
			public ObservableCollection<_9686Group> Groups { get; set; }

			public _9686Group PreviousGroup { get; set; }

			public _9686ViewModel()
			{
				ShowOrHideCommand = new Command<_9686Group>((group) => ShowOrHideItems(group));

				Groups = new ObservableCollection<_9686Group>();

				Groups.Add(new _9686Group($"{Test9686}", new ObservableCollection<_9686Item>()));
				Groups.Add(new _9686Group("Group 2", new ObservableCollection<_9686Item>()));
				Groups.Add(new _9686Group("Group 3", new ObservableCollection<_9686Item>()));
				Groups.Add(new _9686Group("Group 4", new ObservableCollection<_9686Item>()));
				Groups.Add(new _9686Group("Group 5", new ObservableCollection<_9686Item>()));
				Groups.Add(new _9686Group("Group 6", new ObservableCollection<_9686Item>()));
			}

			void ShowOrHideItems(_9686Group group)
			{
				if (PreviousGroup == group)
				{
					if (PreviousGroup.Any())
					{
						PreviousGroup.Clear();
					}
					else
					{
						PreviousGroup.AddRange(new List<_9686Item>
						{
							new _9686Item
							{
								Name = "Item 1"
							},
							new _9686Item
							{
								Name = "Item 2"
							},
							new _9686Item
							{
								Name = "Item 3"
							},
							new _9686Item
							{
								Name = "Item 4"
							},
						});
					}

					UpdateCollection(PreviousGroup);
				}
				else
				{
					if (PreviousGroup != null)
					{
						PreviousGroup.Clear();
						UpdateCollection(PreviousGroup);
					}

					group.AddRange(new List<_9686Item>
					{
						new _9686Item
						{
							Name = "Item 1"
						},
						new _9686Item
						{
							Name = "Item 2"
						},
						new _9686Item
						{
							Name = "Item 3"
						},
						new _9686Item
						{
							Name = "Item 4"
						},
					});

					UpdateCollection(group);
					PreviousGroup = group;
				}
			}

			void UpdateCollection(_9686Group group)
			{
				var index = Groups.IndexOf(group);
				Groups.Remove(group);
				if (group.Count == 0)
				{
					group.GroupName = Success;
				}
				Groups.Insert(index, group);
			}
		}

#if UITEST
		[Category(UITestCategories.CollectionView)]
		[Test]
		public void AddRemoveEmptyGroupsShouldNotCrashOnInsert()
		{
			RunningApp.WaitForElement(Test9686);
			RunningApp.Tap(Test9686);
			RunningApp.WaitForElement("Item 1");
			RunningApp.Tap(Test9686);
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
