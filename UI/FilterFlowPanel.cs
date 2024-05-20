using Blish_HUD.Controls;
using System;
using System.Linq;

namespace LegendaryArmory.UI
{
	internal class FilterFlowPanel : FlowPanel
	{
		public void Filter(Func<LegendaryImage, bool> filter)
		{
			foreach(var control in Children.Where(c => c.GetType() == typeof(FlowPanel)))
			{
				var panel = (FlowPanel)control;

				panel.FilterChildren(filter);
				var hidden = panel.Children.Where(c => !c.Visible).ToList();
				if (hidden.Count == panel.Children.Count)
				{
					panel.Visible = false;
				}
				else
				{
					panel.Visible = true;
					foreach(var child in hidden)
					{
						child.Location = panel.Children.First(c => c.Visible).Location;
					}
				}
			}
			Invalidate();
		}
	}
}
