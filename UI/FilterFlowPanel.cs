using Blish_HUD.Controls;
using Blish_HUD.Controls.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.UI
{
	internal class FilterFlowPanel : FlowPanel
	{
		public void Filter(Func<LegendaryImage, bool> filter)
		{
			foreach(FlowPanel panel in Children.Where(_ => _.GetType() == typeof(FlowPanel)))
			{

				panel.FilterChildren(filter);
				var hidden = panel.Children.Where(_ => !_.Visible).ToList();
				if (hidden.Count == panel.Children.Count)
				{
					panel.Visible = false;
				}
				else
				{
					panel.Visible = true;
					foreach(var child in hidden)
					{
						child.Location = panel.Children.Where(_ => _.Visible).First().Location;
					}
				}
			}
			Invalidate();
		}
	}
}
