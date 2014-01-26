using System;
using System.Text;
using Microsoft.SharePoint.WebPartPages;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using Microsoft.SharePoint.Utilities;

namespace WorldClock
{
	class WCToolPart : ToolPart, System.Web.UI.INamingContainer
	{
		private int[] timeZoneOffsets;
		private string[] timeZoneNames;
		private bool[] timeDstAdjust;
		private int editIndex = -1;

		public WCToolPart()
		{
			this.Init += new EventHandler(WCToolPart_Init);
			this.EnableViewState = true;
			this.AllowMinimize = true;
			this.FrameType = FrameType.Default;
			this.Title = "Time Zones";
		}

		private void WCToolPart_Init(object sender, EventArgs e)
		{
			this.timeZoneOffsets = ((WorldClock)(this.ParentToolPane.SelectedWebPart)).TimeZoneOffsets;
			this.timeZoneNames = ((WorldClock)(this.ParentToolPane.SelectedWebPart)).TimeZoneNames;
			this.timeDstAdjust = ((WorldClock)(this.ParentToolPane.SelectedWebPart)).TimeDstAdjust;
		}

		protected override object SaveViewState()
		{
			return new object[] { base.SaveViewState(), this.timeZoneOffsets, this.timeZoneNames, this.timeDstAdjust, this.editIndex };
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(((object[])(savedState))[0]);

			this.timeZoneOffsets = (int[])(((object[])(savedState))[1]);
			this.timeZoneNames = (string[])(((object[])(savedState))[2]);
			this.timeDstAdjust = (bool[])(((object[])(savedState))[3]);
			this.editIndex = (int)(((object[])(savedState))[4]);
		}

		protected override void CreateChildControls()
		{
			System.Web.UI.HtmlControls.HtmlTable table;
			System.Web.UI.WebControls.ImageButton image;
			System.Web.UI.WebControls.TextBox textbox;
			System.Web.UI.WebControls.CheckBox checkbox;

			for (int n = 0; n < this.timeZoneOffsets.Length + 1; n++)
			{
				if (n >= this.timeZoneNames.Length + 1 || n >= this.timeDstAdjust.Length + 1)
					continue;

				image = new ImageButton();
				image.BorderWidth = new Unit("0px");
				image.AlternateText = "Edit";
				image.Height = new Unit("16px");
				image.ImageUrl = @"\_layouts\images\EDIT.GIF";
				image.Width = new Unit("16px");
				image.CommandName = "Edit";
				image.CommandArgument = Convert.ToString(n);
				image.Command += new CommandEventHandler(imageButton_Command);
				image.ID = this.Qualifier + "EditButton" + n.ToString();
				image.Visible = false;
				this.Controls.Add((Control)image);

				if (n > 0)
				{
					image = new ImageButton();
					image.BorderWidth = new Unit("0px");
					image.AlternateText = "Move Up";
					image.Height = new Unit("8px");
					image.ImageUrl = @"\_layouts\images\SORTUP.GIF";
					image.Width = new Unit("5px");
					image.CommandName = "Up";
					image.CommandArgument = Convert.ToString(n);
					image.Command += new CommandEventHandler(imageButton_Command);
					image.ID = this.Qualifier + "UpButton" + n.ToString();
					image.Visible = false;
					image.Attributes.Add("hspace", "4");
					this.Controls.Add((Control)image);
				}

				image = new ImageButton();
				image.BorderWidth = new Unit("0px");
				image.AlternateText = "Move Down";
				image.Height = new Unit("8px");
				image.ImageUrl = @"\_layouts\images\SORTDOWN.GIF";
				image.Width = new Unit("5px");
				image.CommandName = "Down";
				image.CommandArgument = Convert.ToString(n);
				image.Command += new CommandEventHandler(imageButton_Command);
				image.ID = this.Qualifier + "DownButton" + n.ToString();
				image.Visible = false;
				image.Attributes.Add("hspace", "4");
				this.Controls.Add((Control)image);

				image = new ImageButton();
				image.BorderWidth = new Unit("0px");
				image.AlternateText = "Delete Time Zone";
				image.Height = new Unit("16px");
				image.ImageUrl = @"\_layouts\images\DELITEM.GIF";
				image.Width = new Unit("16px");
				image.CommandName = "Delete";
				image.CommandArgument = Convert.ToString(n);
				image.Command += new CommandEventHandler(imageButton_Command);
				image.ID = this.Qualifier + "DeleteButton" + n.ToString();
				image.Attributes.Add("onclick", "return confirm('Are you sure?');");
				image.Visible = false;
				this.Controls.Add((Control)image);
			}

			textbox = new TextBox();
			textbox.ID = this.Qualifier + "OffsetTextBox";
			textbox.MaxLength = 8;
			textbox.Text = "";
			textbox.TextMode = TextBoxMode.SingleLine;
			textbox.Width = new Unit("40px");
			textbox.Visible = false;
			this.Controls.Add((Control)textbox);

			textbox = new TextBox();
			textbox.ID = this.Qualifier + "NameTextBox";
			textbox.MaxLength = 32;
			textbox.Text = "";
			textbox.TextMode = TextBoxMode.SingleLine;
			textbox.Width = new Unit("100px");
			textbox.Visible = false;
			this.Controls.Add((Control)textbox);

			checkbox = new CheckBox();
			checkbox.Checked = false;
			checkbox.ID = this.Qualifier + "DstAdjCheckBox";
			checkbox.Visible = false;
			this.Controls.Add((Control)checkbox);

			image = new ImageButton();
			image.BorderWidth = new Unit("0px");
			image.AlternateText = (this.editIndex == -1) ? "Add" : "Save";
			image.Height = new Unit("16px");
			image.ImageUrl = @"\_layouts\images\SAVEITEM.GIF";
			image.Width = new Unit("16px");
			image.CommandName = "Save";
			image.Command += new CommandEventHandler(imageButton_Command);
			image.ID = this.Qualifier + "SaveButton";
			image.Visible = false;
			this.Controls.Add((Control)image);

			image = new ImageButton();
			image.BorderWidth = new Unit("0px");
			image.AlternateText = "Cancel";
			image.Height = new Unit("13px");
			image.ImageUrl = @"\_layouts\images\PTCLOSE.GIF";
			image.Width = new Unit("13px");
			image.CommandName = "Cancel";
			image.Command += new CommandEventHandler(imageButton_Command);
			image.ID = this.Qualifier + "CancelButton";
			image.Visible = false;
			this.Controls.Add((Control)image);

			table = new System.Web.UI.HtmlControls.HtmlTable();
			table.Width = "100%";
			table.Border = 0;
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Attributes.Add("title", "Time Zones");
			table.Attributes.Add("summary", "Edit Time Zones");
			table.ID = this.Qualifier + "EditTable";
			this.Controls.Add((Control)table);

			base.CreateChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			Control control;
			System.Web.UI.HtmlControls.HtmlTable table;
			System.Web.UI.HtmlControls.HtmlTableRow row;
			System.Web.UI.HtmlControls.HtmlTableCell cell;
			float tzOffset;
			string tzDisp;

			if ((control = this.FindControl(this.Qualifier + "EditTable")) == null)
				return;

			table = (System.Web.UI.HtmlControls.HtmlTable)control;

			for (int n = 0; n < this.timeZoneOffsets.Length; n++)
			{
				if (n >= this.timeZoneNames.Length || n >= this.timeDstAdjust.Length)
					continue;

				row = new System.Web.UI.HtmlControls.HtmlTableRow();

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");

				if (this.editIndex == n)
				{
					cell.Controls.Add(this.createEditTable(this.timeZoneOffsets[n], this.timeZoneNames[n], this.timeDstAdjust[n]));
				}
				else
				{
					if (n < this.timeZoneOffsets.Length)
					{
						tzOffset = (float)(this.timeZoneOffsets[n]) / 3600;
						tzDisp = this.timeZoneNames[n] + " (" + ((tzOffset < 0) ? "" : "+") + tzOffset.ToString();

						tzOffset -= 1;

						if (this.timeDstAdjust[n])
							tzDisp += " / " + ((tzOffset < 0) ? "" : "+") + tzOffset.ToString() + " DST";

						cell.InnerText = tzDisp + ")";
					}
				}
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				if (n == this.editIndex)
				{
					if ((control = this.FindControl(this.Qualifier + "SaveButton")) == null)
						cell.InnerText = "Can't find image";
					else
					{
						((ImageButton)control).CommandArgument = n.ToString();
						cell.Controls.Add(control);
						control.Visible = true;
					}
				}
				else
				{
					if ((control = this.FindControl(this.Qualifier + "EditButton" + n.ToString())) == null)
						cell.InnerText = "Can't find image";
					else
					{
						cell.Controls.Add(control);
						control.Visible = true;
					}
				}
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				if (n == this.editIndex)
				{
					if ((control = this.FindControl(this.Qualifier + "CancelButton")) == null)
						cell.InnerText = "Can't find image";
					else
					{
						cell.Controls.Add(control);
						control.Visible = true;
					}
				}
				else
				{
					if (n > 0)
					{
						if ((control = this.FindControl(this.Qualifier + "UpButton" + n.ToString())) == null)
							cell.InnerText = "Can't find image";
						else
						{
							cell.Controls.Add(control);
							control.Visible = true;
						}
					}
					else
						cell.InnerHtml = "&nbsp;";
				}
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				if (n == this.editIndex)
					cell.InnerHtml = "&nbsp;";
				else
				{
					if (n < this.timeZoneOffsets.Length - 1)
					{
						if ((control = this.FindControl(this.Qualifier + "DownButton" + n.ToString())) == null)
							cell.InnerText = "Can't find image";
						else
						{
							cell.Controls.Add(control);
							control.Visible = true;
						}
					}
					else
						cell.InnerHtml = "&nbsp;";
				}
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				if (n == this.editIndex)
					cell.InnerHtml = "&nbsp;";
				else
				{
					if ((control = this.FindControl(this.Qualifier + "DeleteButton" + n.ToString())) == null)
						cell.InnerText = "Can't find image";
					else
					{
						cell.Controls.Add(control);
						control.Visible = true;
					}
				}
				row.Cells.Add(cell);

				table.Rows.Add(row);
			}

			if (this.editIndex == -1)
			{
				row = new System.Web.UI.HtmlControls.HtmlTableRow();

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				cell.Controls.Add((Control)this.createEditTable(Convert.ToInt32(TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime(2000, 1, 1)).TotalSeconds),
					TimeZone.CurrentTimeZone.StandardName, false));
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				if ((control = this.FindControl(this.Qualifier + "SaveButton")) == null)
					cell.InnerText = "Can't find image";
				else
				{
					cell.Controls.Add(control);
					control.Visible = true;
				}
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				cell.InnerHtml = "&nbsp;";
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				cell.InnerHtml = "&nbsp;";
				row.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				cell.InnerHtml = "&nbsp;";
				row.Cells.Add(cell);

				table.Rows.Add(row);
			}

			base.OnPreRender(e);
		}

		protected override void RenderToolPart(System.Web.UI.HtmlTextWriter output)
		{
			this.EnsureChildControls();

			base.RenderToolPart(output);
		}

		public override void ApplyChanges()
		{
			((WorldClock)(this.ParentToolPane.SelectedWebPart)).TimeZoneOffsets = this.timeZoneOffsets;
			((WorldClock)(this.ParentToolPane.SelectedWebPart)).TimeZoneNames = this.timeZoneNames;
			((WorldClock)(this.ParentToolPane.SelectedWebPart)).TimeDstAdjust = this.timeDstAdjust;
		}

		private System.Web.UI.HtmlControls.HtmlTable createEditTable(int tzOffset, string tzName, bool tzDstAdj)
		{
			Control control;
			System.Web.UI.HtmlControls.HtmlTable table;
			System.Web.UI.HtmlControls.HtmlTableRow topRow, bottomRow;
			System.Web.UI.HtmlControls.HtmlTableCell cell;
			float tzVal;

			table = new System.Web.UI.HtmlControls.HtmlTable();
			table.Width = "100%";
			table.Border = 0;
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Attributes.Add("title", "Time Zone Details");
			table.Attributes.Add("summary", ((this.editIndex == -1) ? "Add New" : "Edit") + " Time Zone");

			topRow = new System.Web.UI.HtmlControls.HtmlTableRow();
			bottomRow = new System.Web.UI.HtmlControls.HtmlTableRow();

			cell = new System.Web.UI.HtmlControls.HtmlTableCell("th");
			// cell.Attributes.Add("class", "RegularHeaderStyle");
			cell.InnerHtml = "Offset<br />(hours)";
			cell.NoWrap = false;
			cell.Align = "center";
			cell.VAlign = "bottom";
			topRow.Cells.Add(cell);

			cell = new System.Web.UI.HtmlControls.HtmlTableCell();
			// cell.Attributes.Add("class", "RegularCellStyle");
			if ((control = this.FindControl(this.Qualifier + "OffsetTextBox")) == null)
				cell.InnerText = "Can't find textbox";
			else
			{
				tzVal = (float)tzOffset / 3600;
				((TextBox)control).Text = tzVal.ToString();
				cell.Controls.Add(control);
				control.Visible = true;
			}
			bottomRow.Cells.Add(cell);

			cell = new System.Web.UI.HtmlControls.HtmlTableCell("th");
			// cell.Attributes.Add("class", "RegularHeaderStyle");
			cell.InnerText = "Name";
			cell.NoWrap = true;
			cell.Align = "center";
			cell.VAlign = "bottom";
			topRow.Cells.Add(cell);

			cell = new System.Web.UI.HtmlControls.HtmlTableCell();
			// cell.Attributes.Add("class", "RegularCellStyle");
			if ((control = this.FindControl(this.Qualifier + "NameTextBox")) == null)
				cell.InnerText = "Can't find textbox";
			else
			{
				((TextBox)control).Text = tzName;
				cell.Controls.Add(control);
				control.Visible = true;
			}
			bottomRow.Cells.Add(cell);

			cell = new System.Web.UI.HtmlControls.HtmlTableCell("th");
			// cell.Attributes.Add("class", "RegularHeaderStyle");
			cell.InnerText = "DST Adj";
			cell.NoWrap = true;
			cell.Align = "center";
			cell.VAlign = "bottom";
			topRow.Cells.Add(cell);

			cell = new System.Web.UI.HtmlControls.HtmlTableCell();
			// cell.Attributes.Add("class", "RegularCellStyle");
			if ((control = this.FindControl(this.Qualifier + "DstAdjCheckBox")) == null)
				cell.InnerText = "Can't find checkbox";
			else
			{
				((CheckBox)control).Checked = tzDstAdj;
				cell.Controls.Add(control);
				control.Visible = true;
			}
			bottomRow.Cells.Add(cell);

			table.Rows.Add(topRow);
			table.Rows.Add(bottomRow);

			return table;
		}

		void imageButton_Command(object sender, CommandEventArgs e)
		{
			int itemIndex, n;
			int[] tzOffset;
			string[] tzName;
			bool[] tzDstAdj;
			Control control;

			switch (e.CommandName)
			{
				case "Edit":
					this.editIndex = Convert.ToInt32(e.CommandArgument);
					break;
				case "Up":

					if ((itemIndex = Convert.ToInt32(e.CommandArgument)) > 0)
					{
						tzOffset = new int[1] { this.timeZoneOffsets[itemIndex] };
						tzName = new string[1] { this.timeZoneNames[itemIndex] };
						tzDstAdj = new bool[1] { this.timeDstAdjust[itemIndex] };
						this.timeZoneOffsets[itemIndex] = this.timeZoneOffsets[itemIndex - 1];
						this.timeZoneNames[itemIndex] = this.timeZoneNames[itemIndex - 1];
						this.timeDstAdjust[itemIndex] = this.timeDstAdjust[itemIndex - 1];
						this.timeZoneOffsets[itemIndex - 1] = tzOffset[0];
						this.timeZoneNames[itemIndex - 1] = tzName[0];
						this.timeDstAdjust[itemIndex - 1] = tzDstAdj[0];
					}
					break;
				case "Down":
					if ((itemIndex = Convert.ToInt32(e.CommandArgument)) < this.timeZoneOffsets.Length - 1)
					{
						tzOffset = new int[1] { this.timeZoneOffsets[itemIndex] };
						tzName = new string[1] { this.timeZoneNames[itemIndex] };
						tzDstAdj = new bool[1] { this.timeDstAdjust[itemIndex] };
						this.timeZoneOffsets[itemIndex] = this.timeZoneOffsets[itemIndex + 1];
						this.timeZoneNames[itemIndex] = this.timeZoneNames[itemIndex + 1];
						this.timeDstAdjust[itemIndex] = this.timeDstAdjust[itemIndex + 1];
						this.timeZoneOffsets[itemIndex + 1] = tzOffset[0];
						this.timeZoneNames[itemIndex + 1] = tzName[0];
						this.timeDstAdjust[itemIndex + 1] = tzDstAdj[0];
					}
					break;
				case "Delete":
					itemIndex = Convert.ToInt32(e.CommandArgument);
					tzOffset = new int[this.timeZoneOffsets.Length - 1];
					tzName = new string[this.timeZoneOffsets.Length - 1];
					tzDstAdj = new bool[this.timeZoneOffsets.Length - 1];
					for (n = 0; n < itemIndex; n++)
					{
						tzOffset[n] = this.timeZoneOffsets[n];
						tzName[n] = this.timeZoneNames[n];
						tzDstAdj[n] = this.timeDstAdjust[n];
					}
					for (n = itemIndex + 1; n < this.timeZoneOffsets.Length; n++)
					{
						tzOffset[n - 1] = this.timeZoneOffsets[n];
						tzName[n - 1] = this.timeZoneNames[n];
						tzDstAdj[n - 1] = this.timeDstAdjust[n];
					}
					this.timeZoneOffsets = tzOffset;
					this.timeZoneNames = tzName;
					this.timeDstAdjust = tzDstAdj;
					break;
				case "Save":
					if (this.editIndex == -1)
					{
						tzOffset = new int[this.timeZoneOffsets.Length + 1];
						tzName = new string[this.timeZoneOffsets.Length + 1];
						tzDstAdj = new bool[this.timeZoneOffsets.Length + 1];
						for (n = 0; n < this.timeZoneOffsets.Length; n++)
						{
							tzOffset[n] = this.timeZoneOffsets[n];
							tzName[n] = this.timeZoneNames[n];
							tzDstAdj[n] = this.timeDstAdjust[n];
						}
						try
						{
							tzOffset[this.timeZoneOffsets.Length] = Convert.ToInt32(Convert.ToDouble(((TextBox)(this.FindControl(this.Qualifier + "OffsetTextBox"))).Text) * 3600);
						}
						catch
						{
							tzOffset[this.timeZoneOffsets.Length] = 0;
						}
						try
						{
							tzName[this.timeZoneOffsets.Length] = ((TextBox)(this.FindControl(this.Qualifier + "NameTextBox"))).Text;
						}
						catch
						{
							tzName[this.timeZoneOffsets.Length] = "Could not find control";
						}
						try
						{
							tzDstAdj[this.timeZoneOffsets.Length] = ((CheckBox)(this.FindControl(this.Qualifier + "DstAdjCheckBox"))).Checked;
						}
						catch
						{
							tzDstAdj[this.timeZoneOffsets.Length] = false;
						}
						this.timeZoneOffsets = tzOffset;
						this.timeZoneNames = tzName;
						this.timeDstAdjust = tzDstAdj;
					}
					else
					{
						if ((control = this.FindControl(this.Qualifier + "OffsetTextBox")) != null)
							this.timeZoneOffsets[this.editIndex] = Convert.ToInt32(Convert.ToDouble(((TextBox)control).Text) * 3600);
						if ((control = this.FindControl(this.Qualifier + "NameTextBox")) != null)
							this.timeZoneNames[this.editIndex] = ((TextBox)control).Text;
						if ((control = this.FindControl(this.Qualifier + "DstAdjCheckBox")) != null)
							this.timeDstAdjust[this.editIndex] = ((CheckBox)control).Checked;
						this.editIndex = -1;
					}
					break;
				case "Cancel":
					this.editIndex = -1;
					break;
			}
		}
	}
}
