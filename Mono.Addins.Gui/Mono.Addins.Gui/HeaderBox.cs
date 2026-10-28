// 
// HeaderBox.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Gtk;

namespace Mono.Addins.Gui
{
	class HeaderBox: Bin
	{
		Gtk.Widget child;
		int topMargin;
		int bottomMargin;
		int leftMargin;
		int rightMargin;
		
		int topPadding;
		int bottomPadding;
		int leftPadding;
		int rightPadding;
		
		bool useCustomColor;
		Gdk.Color customColor;
		
		public HeaderBox ()
		{
		}
		
		public HeaderBox (int topMargin, int bottomMargin, int leftMargin, int rightMargin)
		{
			SetMargins (topMargin, bottomMargin, leftMargin, rightMargin);
		}
		
		public void Replace (Gtk.Bin parent)
		{
			Gtk.Widget c = parent.Child;
			parent.Remove (c);
			Add (c);
			parent.Add (this);
		}
		
		public void SetMargins (int topMargin, int bottomMargin, int leftMargin, int rightMargin)
		{
			this.topMargin = topMargin;
			this.bottomMargin = bottomMargin;
			this.leftMargin = leftMargin;
			this.rightMargin = rightMargin;
		}
		
		public void SetPadding (int topPadding, int bottomPadding, int leftPadding, int rightPadding)
		{
			this.topPadding = topPadding;
			this.bottomPadding = bottomPadding;
			this.leftPadding = leftPadding;
			this.rightPadding = rightPadding;
		}
		
		public bool GradientBackround { get; set; }
		
		public Gdk.Color BackgroundColor {
			get { return customColor; }
			set { customColor = value; useCustomColor = true; }
		}
		
		public void ResetBackgroundColor ()
		{
			useCustomColor = false;
		}

		protected override void OnAdded (Widget widget)
		{
			base.OnAdded (widget);
			child = widget;
		}

		protected override void OnGetPreferredWidth (out int minimum_width, out int natural_width)
		{
			if (child != null) {
				child.GetPreferredWidth (out minimum_width, out natural_width);
				minimum_width += leftMargin + rightMargin + leftPadding + rightPadding;
				natural_width += leftMargin + rightMargin + leftPadding + rightPadding;
			} else {
				minimum_width = natural_width = 0;
			}
		}

		protected override void OnGetPreferredHeight (out int minimum_height, out int natural_height)
		{
			if (child != null) {
				child.GetPreferredHeight (out minimum_height, out natural_height);
				minimum_height += topMargin + bottomMargin + topPadding + bottomPadding;
				natural_height += topMargin + bottomMargin + topPadding + bottomPadding;
			} else {
				minimum_height = natural_height = 0;
			}
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			if (allocation.Width > leftMargin + rightMargin + leftPadding + rightPadding) {
				allocation.X += leftMargin + leftPadding;
				allocation.Width -= leftMargin + rightMargin + leftPadding + rightPadding;
			}
			if (allocation.Height > topMargin + bottomMargin + topPadding + bottomPadding) {
				allocation.Y += topMargin + topPadding;
				allocation.Height -= topMargin + bottomMargin + topPadding + bottomPadding;
			}
			if (child != null)
				child.SizeAllocate (allocation);
		}

		protected override bool OnDrawn (Cairo.Context cr)
		{
			Gdk.Rectangle rect = Allocation;

			if (GradientBackround) {
				Gdk.RGBA rgba = Parent.StyleContext.GetBackgroundColor (Gtk.StateFlags.Normal);
				HslColor gcol = useCustomColor ? customColor : new Gdk.Color ((byte) (rgba.Red * 65535), (byte) (rgba.Green * 65535), (byte) (rgba.Blue * 65535));

				cr.NewPath ();
				cr.MoveTo (rect.X, rect.Y);
				cr.RelLineTo (rect.Width, 0);
				cr.RelLineTo (0, rect.Height);
				cr.RelLineTo (-rect.Width, 0);
				cr.RelLineTo (0, -rect.Height);
				cr.ClosePath ();
				using (Cairo.Gradient pat = new Cairo.LinearGradient (rect.X, rect.Y, rect.X, rect.Y + rect.Height - 1)) {
					Cairo.Color color1 = gcol;
					pat.AddColorStop (0, color1);
					gcol.L -= 0.1;
					if (gcol.L < 0)
						gcol.L = 0;
					pat.AddColorStop (1, gcol);
					cr.SetSource (pat);
					cr.FillPreserve ();
				}
				cr.NewPath ();
			}

			base.OnDrawn (cr);

			Gdk.RGBA border = Parent.StyleContext.GetColor (Gtk.StateFlags.Normal);
			cr.SetSourceColor (new Cairo.Color (border.Red, border.Green, border.Blue, 1));

			if (topMargin > 0) {
				cr.Rectangle (rect.X, rect.Y, rect.Width, topMargin);
				cr.Fill ();
			}
			if (bottomMargin > 0) {
				cr.Rectangle (rect.X, rect.Bottom - bottomMargin, rect.Width, bottomMargin);
				cr.Fill ();
			}
			if (leftMargin > 0) {
				cr.Rectangle (rect.X, rect.Y, leftMargin, rect.Height);
				cr.Fill ();
			}
			if (rightMargin > 0) {
				cr.Rectangle (rect.Right - rightMargin, rect.Y, rightMargin, rect.Height);
				cr.Fill ();
			}

			return true;
		}
	}
}

