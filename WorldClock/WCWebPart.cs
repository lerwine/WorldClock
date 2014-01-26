using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace WorldClock
{
	/// <summary>
	/// Description for OptionList
	/// </summary>
	public enum OptionList
	{
		[XmlEnum(Name = "Option One")]
		Option_One = 0,
		[XmlEnum(Name = "Option Two")]
		Option_Two = 1,
		[XmlEnum(Name = "Option Three")]
		Option_Three = 2
	};

	/// <summary>
	/// Description for WorldClock.
	/// </summary>
	[DefaultProperty("TimeZoneOffsets"),
		ToolboxData("<{0}:WorldClock runat=server></{0}:WorldClock>"),
		XmlRoot(Namespace = "WorldClock")]
	public class WorldClock : Microsoft.SharePoint.WebPartPages.WebPart, System.Web.UI.INamingContainer
	{
		private int[] timeZoneOffsets;
		private string[] timeZoneNames;
		private bool[] timeDstAdjust;

		#region WEBPART PROPERTIES
		[Browsable(false),
			Category("Miscellaneous"),
			WebPartStorage(Storage.Personal),
			FriendlyName("Time Zone Offsets"),
			Description("Time Zone Offsets Property")]
		public int[] TimeZoneOffsets
		{
			get
			{
				return timeZoneOffsets;
			}

			set
			{
				timeZoneOffsets = (value == null) ? new int[] { -14400, 0, 10800, 14400, 16200 } : value;
			}
		}

		public bool ShouldSerializeTimeZoneOffsets()
		{
			return true;
		}

		[Browsable(false),
			Category("Miscellaneous"),
			WebPartStorage(Storage.Personal),
			FriendlyName("Time Zone Names"),
			Description("Time Zone Names Property")]
		public string[] TimeZoneNames
		{
			get
			{
				return timeZoneNames;
			}

			set
			{
				timeZoneNames = (value == null) ? new string[] { "Tampa", "Zulu", "Iraq/HOA", "Qatar", "Afgn" } : value;
			}
		}

		public bool ShouldSerializeTimeZoneNames()
		{
			return true;
		}

		[Browsable(false),
			Category("Miscellaneous"),
			WebPartStorage(Storage.Personal),
			FriendlyName("Time Zone DST Adjust"),
			Description("Time Zone DST Adjust Property")]
		public bool[] TimeDstAdjust
		{
			get
			{
				return timeDstAdjust;
			}

			set
			{
				timeDstAdjust = (value == null) ? new bool[] { true, false, false, false, false } : value;
			}
		}

		public bool ShouldSerializeTimeDstAdjust()
		{
			return true;
		}
		#endregion

		/// <summary>
		///	Constructor for the class.
		///	</summary>
		public WorldClock()
		{
			this.timeZoneOffsets = new int[] { -14400, 0, 10800, 14400, 16200 };
			this.timeZoneNames = new string[] { "Tampa", "Zulu", "Iraq/HOA", "Qatar", "Afgn" };
			this.timeDstAdjust = new bool[] { true, false, false, false, false };
		}

		protected override void CreateChildControls()
		{
			System.Web.UI.HtmlControls.HtmlTable table;
			System.Web.UI.HtmlControls.HtmlTableRow topRow, bottomRow;
			System.Web.UI.HtmlControls.HtmlTableCell cell;
			System.Web.UI.WebControls.Image image;

			table = new System.Web.UI.HtmlControls.HtmlTable();
			table.Border = 0;
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Width = "100%";
			table.ID = this.Qualifier + "WorldClock";
			table.Attributes.Add("title", "World Clock");
			table.Attributes.Add("summary", "World Time Zones");

			topRow = new System.Web.UI.HtmlControls.HtmlTableRow();
			bottomRow = new System.Web.UI.HtmlControls.HtmlTableRow();

			for (int n = 0; n < this.timeZoneOffsets.Length; n++)
			{
				if (n >= this.timeZoneNames.Length || n >= this.timeDstAdjust.Length)
					continue;

				if (n > 0)
				{
					cell = new System.Web.UI.HtmlControls.HtmlTableCell("th");
					// cell.Attributes.Add("class", "RegularHeaderStyle");
					image = new Image();
					image.BorderWidth = new Unit("0px");
					image.AlternateText = "";
					image.Height = new Unit("4px");
					image.ImageUrl = @"\_layouts\images\DISCBUL.GIF";
					image.Width = new Unit("4px");
					cell.Controls.Add(image);
					topRow.Cells.Add(cell);

					cell = new System.Web.UI.HtmlControls.HtmlTableCell();
					// cell.Attributes.Add("class", "RegularCellStyle");
					cell.InnerHtml = "&nbsp;";
					bottomRow.Cells.Add(cell);
				}

				cell = new System.Web.UI.HtmlControls.HtmlTableCell("th");
				// cell.Attributes.Add("class", "RegularHeaderStyle");
				cell.Align = "center";
				cell.NoWrap = true;
				cell.InnerText = this.timeZoneNames[n];
				topRow.Cells.Add(cell);

				cell = new System.Web.UI.HtmlControls.HtmlTableCell();
				// cell.Attributes.Add("class", "RegularCellStyle");
				cell.Align = "center";
				cell.NoWrap = true;
				bottomRow.Cells.Add(cell);
			}

			table.Rows.Add(topRow);
			table.Rows.Add(bottomRow);

			this.Controls.Add((Control)table);
		}

		/// <summary>
		///	This method gets the custom tool parts for this Web Part by overriding the
		///	GetToolParts method of the WebPart base class. You must implement
		///	custom tool parts in a separate class that derives from 
		///	Microsoft.SharePoint.WebPartPages.ToolPart. 
		///	</summary>
		///<returns>An array of references to ToolPart objects.</returns>
		public override ToolPart[] GetToolParts()
		{
			return new ToolPart[2] { new WCToolPart(), new WebPartToolPart() };
		}

		/// <summary>
		/// Render this Web Part to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void RenderWebPart(HtmlTextWriter output)
		{
			Control control;

			if ((control = this.FindControl(this.Qualifier + "WorldClock")) == null)
				output.Write("Could not find world clock table");
			else
				control.RenderControl(output);

			output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
			output.AddAttribute("Language", "JavaScript1.2");
			output.RenderBeginTag(HtmlTextWriterTag.Script);
			output.Write("<!--\r\n");
			output.Write("var " + this.Qualifier + "TimeZones = [");
			for (int n = 0; n < this.timeZoneOffsets.Length; n++)
			{
				if (n > 0)
					output.Write(", ");
				output.Write(((float)this.timeZoneOffsets[n]).ToString());
			}
			output.Write("];\r\n");
			try
			{
				output.Write(this.GetEmbeddedFileContents("Embedded.js"));
			}
			catch (Exception exc)
			{
				output.Write("/* " + SPEncode.HtmlEncode(exc.Message) + " */");
			}
			output.Write("\r\n" + this.Qualifier + "Init(\"" + control.UniqueID.Replace(":", "_") + "\");\r\n// -->");
			output.RenderEndTag();
		}

		public string GetEmbeddedFileContents(string strResourceFile)
		{
			System.IO.StreamReader sr = null;
			string strText;
			System.IO.Stream stream = null;

			try
			{
				stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldClock." +
					strResourceFile);
				sr = new System.IO.StreamReader(stream);
				strText = this.ReplaceTokens(sr.ReadToEnd());
			}
			catch
			{
				throw;
			}
			finally
			{
				if (sr != null) sr.Close();
				if (stream != null) stream.Close();
			}

			return strText;
		}
	}
}
