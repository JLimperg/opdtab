
// This file has been generated by the GUI designer. Do not modify.
namespace OPDtabGui
{
	public partial class ImportCSV
	{
		private global::Gtk.VBox vbox2;

		private global::Gtk.Frame frame1;

		private global::Gtk.Alignment GtkAlignment;

		private global::Gtk.VBox vbox1;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Button btnChooseFile;

		private global::Gtk.Label labelFile;

		private global::Gtk.HBox hbox2;

		private global::Gtk.CheckButton cbHasHeaders;

		private global::Gtk.CheckButton cbOverwrite;

		private global::Gtk.Label GtkLabel3;

		private global::Gtk.Table table2;

		private global::Gtk.CheckButton cbAge;

		private global::Gtk.CheckButton cbBirthday;

		private global::Gtk.CheckButton cbBlackList;

		private global::Gtk.CheckButton cbCity;

		private global::Gtk.CheckButton cbExtraInfo;

		private global::Gtk.CheckButton cbRole;

		private global::Gtk.Entry entryBdayFormat;

		private global::Gtk.Entry entryExtraInfoDefault;

		private global::Gtk.HBox hbox3;

		private global::Gtk.RadioButton rbTeamMember;

		private global::Gtk.RadioButton rbJudge;

		private global::Gtk.Label hdAge;

		private global::Gtk.Label hdBirthday;

		private global::Gtk.Label hdBlackList;

		private global::Gtk.Label hdCity;

		private global::Gtk.Label hdClub;

		private global::Gtk.Label hdExtraInfo;

		private global::Gtk.Label hdFirstName;

		private global::Gtk.Label hdLastName;

		private global::Gtk.Label hdRole;

		private global::Gtk.Label label2;

		private global::Gtk.Label label3;

		private global::Gtk.Label label4;

		private global::Gtk.Label label7;

		private global::Gtk.Label label8;

		private global::Gtk.Label labelHeader;

		private global::Gtk.SpinButton sbAge;

		private global::Gtk.SpinButton sbBirthday;

		private global::Gtk.SpinButton sbBlackList;

		private global::Gtk.SpinButton sbCity;

		private global::Gtk.SpinButton sbClub;

		private global::Gtk.SpinButton sbExtraInfo;

		private global::Gtk.SpinButton sbFirstName;

		private global::Gtk.SpinButton sbLastName;

		private global::Gtk.SpinButton sbRole;

		private global::Gtk.Button btnImportCSV;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget OPDtabGui.ImportCSV
			this.Name = "OPDtabGui.ImportCSV";
			this.Title = "ImportCSV";
			this.Icon = global::Gdk.Pixbuf.LoadFromResource("OPDtabGui.AppIcon.png");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.Modal = true;
			// Container child OPDtabGui.ImportCSV.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame();
			this.frame1.Name = "frame1";
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.hbox1.BorderWidth = ((uint)(3));
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnChooseFile = new global::Gtk.Button();
			this.btnChooseFile.CanFocus = true;
			this.btnChooseFile.Name = "btnChooseFile";
			this.btnChooseFile.UseUnderline = true;
			this.btnChooseFile.Label = "Choose...";
			this.hbox1.Add(this.btnChooseFile);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnChooseFile]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.labelFile = new global::Gtk.Label();
			this.labelFile.Name = "labelFile";
			this.labelFile.Ellipsize = ((global::Pango.EllipsizeMode)(2));
			this.hbox1.Add(this.labelFile);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.labelFile]));
			w2.Position = 1;
			this.vbox1.Add(this.hbox1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.cbHasHeaders = new global::Gtk.CheckButton();
			this.cbHasHeaders.CanFocus = true;
			this.cbHasHeaders.Name = "cbHasHeaders";
			this.cbHasHeaders.Label = "Headers in first line";
			this.cbHasHeaders.Active = true;
			this.cbHasHeaders.DrawIndicator = true;
			this.cbHasHeaders.UseUnderline = true;
			this.hbox2.Add(this.cbHasHeaders);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.cbHasHeaders]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.cbOverwrite = new global::Gtk.CheckButton();
			this.cbOverwrite.CanFocus = true;
			this.cbOverwrite.Name = "cbOverwrite";
			this.cbOverwrite.Label = "Overwrite existing";
			this.cbOverwrite.DrawIndicator = true;
			this.cbOverwrite.UseUnderline = true;
			this.hbox2.Add(this.cbOverwrite);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.cbOverwrite]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.vbox1.Add(this.hbox2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox2]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			this.GtkAlignment.Add(this.vbox1);
			this.frame1.Add(this.GtkAlignment);
			this.GtkLabel3 = new global::Gtk.Label();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = "File";
			this.frame1.LabelWidget = this.GtkLabel3;
			this.vbox2.Add(this.frame1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.frame1]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.Table(((uint)(10)), ((uint)(4)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.cbAge = new global::Gtk.CheckButton();
			this.cbAge.CanFocus = true;
			this.cbAge.Name = "cbAge";
			this.cbAge.Label = "Age";
			this.cbAge.DrawIndicator = true;
			this.cbAge.UseUnderline = true;
			this.table2.Add(this.cbAge);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table2[this.cbAge]));
			w10.TopAttach = ((uint)(5));
			w10.BottomAttach = ((uint)(6));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbBirthday = new global::Gtk.CheckButton();
			this.cbBirthday.CanFocus = true;
			this.cbBirthday.Name = "cbBirthday";
			this.cbBirthday.Label = "Birthday";
			this.cbBirthday.DrawIndicator = true;
			this.cbBirthday.UseUnderline = true;
			this.table2.Add(this.cbBirthday);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2[this.cbBirthday]));
			w11.TopAttach = ((uint)(6));
			w11.BottomAttach = ((uint)(7));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbBlackList = new global::Gtk.CheckButton();
			this.cbBlackList.CanFocus = true;
			this.cbBlackList.Name = "cbBlackList";
			this.cbBlackList.Label = "BlackList";
			this.cbBlackList.DrawIndicator = true;
			this.cbBlackList.UseUnderline = true;
			this.table2.Add(this.cbBlackList);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2[this.cbBlackList]));
			w12.TopAttach = ((uint)(9));
			w12.BottomAttach = ((uint)(10));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbCity = new global::Gtk.CheckButton();
			this.cbCity.CanFocus = true;
			this.cbCity.Name = "cbCity";
			this.cbCity.Label = "City";
			this.cbCity.DrawIndicator = true;
			this.cbCity.UseUnderline = true;
			this.table2.Add(this.cbCity);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2[this.cbCity]));
			w13.TopAttach = ((uint)(4));
			w13.BottomAttach = ((uint)(5));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbExtraInfo = new global::Gtk.CheckButton();
			this.cbExtraInfo.CanFocus = true;
			this.cbExtraInfo.Name = "cbExtraInfo";
			this.cbExtraInfo.Label = "ExtraInfo";
			this.cbExtraInfo.DrawIndicator = true;
			this.cbExtraInfo.UseUnderline = true;
			this.table2.Add(this.cbExtraInfo);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2[this.cbExtraInfo]));
			w14.TopAttach = ((uint)(8));
			w14.BottomAttach = ((uint)(9));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbRole = new global::Gtk.CheckButton();
			this.cbRole.CanFocus = true;
			this.cbRole.Name = "cbRole";
			this.cbRole.Label = "Role";
			this.cbRole.Active = true;
			this.cbRole.DrawIndicator = true;
			this.cbRole.UseUnderline = true;
			this.table2.Add(this.cbRole);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2[this.cbRole]));
			w15.TopAttach = ((uint)(7));
			w15.BottomAttach = ((uint)(8));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryBdayFormat = new global::Gtk.Entry();
			this.entryBdayFormat.CanFocus = true;
			this.entryBdayFormat.Name = "entryBdayFormat";
			this.entryBdayFormat.Text = "dd.MM.yyyy";
			this.entryBdayFormat.IsEditable = true;
			this.entryBdayFormat.InvisibleChar = '●';
			this.table2.Add(this.entryBdayFormat);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2[this.entryBdayFormat]));
			w16.TopAttach = ((uint)(6));
			w16.BottomAttach = ((uint)(7));
			w16.LeftAttach = ((uint)(3));
			w16.RightAttach = ((uint)(4));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryExtraInfoDefault = new global::Gtk.Entry();
			this.entryExtraInfoDefault.CanFocus = true;
			this.entryExtraInfoDefault.Name = "entryExtraInfoDefault";
			this.entryExtraInfoDefault.IsEditable = true;
			this.entryExtraInfoDefault.InvisibleChar = '●';
			this.table2.Add(this.entryExtraInfoDefault);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table2[this.entryExtraInfoDefault]));
			w17.TopAttach = ((uint)(8));
			w17.BottomAttach = ((uint)(9));
			w17.LeftAttach = ((uint)(3));
			w17.RightAttach = ((uint)(4));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hbox3 = new global::Gtk.HBox();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.rbTeamMember = new global::Gtk.RadioButton("Team Member/Parse");
			this.rbTeamMember.CanFocus = true;
			this.rbTeamMember.Name = "rbTeamMember";
			this.rbTeamMember.Active = true;
			this.rbTeamMember.DrawIndicator = true;
			this.rbTeamMember.UseUnderline = true;
			this.rbTeamMember.Group = new global::GLib.SList(global::System.IntPtr.Zero);
			this.hbox3.Add(this.rbTeamMember);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.rbTeamMember]));
			w18.Position = 0;
			// Container child hbox3.Gtk.Box+BoxChild
			this.rbJudge = new global::Gtk.RadioButton("Judge");
			this.rbJudge.CanFocus = true;
			this.rbJudge.Name = "rbJudge";
			this.rbJudge.DrawIndicator = true;
			this.rbJudge.UseUnderline = true;
			this.rbJudge.Group = this.rbTeamMember.Group;
			this.hbox3.Add(this.rbJudge);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.rbJudge]));
			w19.Position = 1;
			this.table2.Add(this.hbox3);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table2[this.hbox3]));
			w20.TopAttach = ((uint)(7));
			w20.BottomAttach = ((uint)(8));
			w20.LeftAttach = ((uint)(3));
			w20.RightAttach = ((uint)(4));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdAge = new global::Gtk.Label();
			this.hdAge.Name = "hdAge";
			this.hdAge.Xalign = 0F;
			this.hdAge.LabelProp = "label6";
			this.table2.Add(this.hdAge);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table2[this.hdAge]));
			w21.TopAttach = ((uint)(5));
			w21.BottomAttach = ((uint)(6));
			w21.LeftAttach = ((uint)(2));
			w21.RightAttach = ((uint)(3));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdBirthday = new global::Gtk.Label();
			this.hdBirthday.Name = "hdBirthday";
			this.hdBirthday.Xalign = 0F;
			this.hdBirthday.LabelProp = "label7";
			this.table2.Add(this.hdBirthday);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table2[this.hdBirthday]));
			w22.TopAttach = ((uint)(6));
			w22.BottomAttach = ((uint)(7));
			w22.LeftAttach = ((uint)(2));
			w22.RightAttach = ((uint)(3));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdBlackList = new global::Gtk.Label();
			this.hdBlackList.Name = "hdBlackList";
			this.hdBlackList.Xalign = 0F;
			this.hdBlackList.LabelProp = "label8";
			this.table2.Add(this.hdBlackList);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table2[this.hdBlackList]));
			w23.TopAttach = ((uint)(9));
			w23.BottomAttach = ((uint)(10));
			w23.LeftAttach = ((uint)(2));
			w23.RightAttach = ((uint)(3));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdCity = new global::Gtk.Label();
			this.hdCity.Name = "hdCity";
			this.hdCity.Xalign = 0F;
			this.hdCity.LabelProp = "label5";
			this.table2.Add(this.hdCity);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table2[this.hdCity]));
			w24.TopAttach = ((uint)(4));
			w24.BottomAttach = ((uint)(5));
			w24.LeftAttach = ((uint)(2));
			w24.RightAttach = ((uint)(3));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdClub = new global::Gtk.Label();
			this.hdClub.Name = "hdClub";
			this.hdClub.Xalign = 0F;
			this.hdClub.LabelProp = "label4";
			this.table2.Add(this.hdClub);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table2[this.hdClub]));
			w25.TopAttach = ((uint)(3));
			w25.BottomAttach = ((uint)(4));
			w25.LeftAttach = ((uint)(2));
			w25.RightAttach = ((uint)(3));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdExtraInfo = new global::Gtk.Label();
			this.hdExtraInfo.Name = "hdExtraInfo";
			this.hdExtraInfo.Xalign = 0F;
			this.hdExtraInfo.LabelProp = "label8";
			this.table2.Add(this.hdExtraInfo);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table2[this.hdExtraInfo]));
			w26.TopAttach = ((uint)(8));
			w26.BottomAttach = ((uint)(9));
			w26.LeftAttach = ((uint)(2));
			w26.RightAttach = ((uint)(3));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdFirstName = new global::Gtk.Label();
			this.hdFirstName.Name = "hdFirstName";
			this.hdFirstName.Xalign = 0F;
			this.hdFirstName.LabelProp = "label2";
			this.table2.Add(this.hdFirstName);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table2[this.hdFirstName]));
			w27.TopAttach = ((uint)(1));
			w27.BottomAttach = ((uint)(2));
			w27.LeftAttach = ((uint)(2));
			w27.RightAttach = ((uint)(3));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdLastName = new global::Gtk.Label();
			this.hdLastName.Name = "hdLastName";
			this.hdLastName.Xalign = 0F;
			this.hdLastName.LabelProp = "label3";
			this.table2.Add(this.hdLastName);
			global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.table2[this.hdLastName]));
			w28.TopAttach = ((uint)(2));
			w28.BottomAttach = ((uint)(3));
			w28.LeftAttach = ((uint)(2));
			w28.RightAttach = ((uint)(3));
			w28.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hdRole = new global::Gtk.Label();
			this.hdRole.Name = "hdRole";
			this.hdRole.Xalign = 0F;
			this.hdRole.LabelProp = "label8";
			this.table2.Add(this.hdRole);
			global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.table2[this.hdRole]));
			w29.TopAttach = ((uint)(7));
			w29.BottomAttach = ((uint)(8));
			w29.LeftAttach = ((uint)(2));
			w29.RightAttach = ((uint)(3));
			w29.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = "FirstName";
			this.table2.Add(this.label2);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.table2[this.label2]));
			w30.TopAttach = ((uint)(1));
			w30.BottomAttach = ((uint)(2));
			w30.XOptions = ((global::Gtk.AttachOptions)(4));
			w30.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = "LastName";
			this.table2.Add(this.label3);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table2[this.label3]));
			w31.TopAttach = ((uint)(2));
			w31.BottomAttach = ((uint)(3));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = "Club";
			this.table2.Add(this.label4);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.table2[this.label4]));
			w32.TopAttach = ((uint)(3));
			w32.BottomAttach = ((uint)(4));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label();
			this.label7.Name = "label7";
			this.label7.LabelProp = "Field";
			this.table2.Add(this.label7);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.table2[this.label7]));
			w33.XOptions = ((global::Gtk.AttachOptions)(4));
			w33.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label();
			this.label8.Name = "label8";
			this.label8.LabelProp = "Column";
			this.table2.Add(this.label8);
			global::Gtk.Table.TableChild w34 = ((global::Gtk.Table.TableChild)(this.table2[this.label8]));
			w34.LeftAttach = ((uint)(1));
			w34.RightAttach = ((uint)(2));
			w34.XOptions = ((global::Gtk.AttachOptions)(4));
			w34.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.labelHeader = new global::Gtk.Label();
			this.labelHeader.Name = "labelHeader";
			this.labelHeader.Xalign = 0F;
			this.labelHeader.LabelProp = "Header";
			this.table2.Add(this.labelHeader);
			global::Gtk.Table.TableChild w35 = ((global::Gtk.Table.TableChild)(this.table2[this.labelHeader]));
			w35.LeftAttach = ((uint)(2));
			w35.RightAttach = ((uint)(3));
			w35.XOptions = ((global::Gtk.AttachOptions)(4));
			w35.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbAge = new global::Gtk.SpinButton(0, 100, 1);
			this.sbAge.CanFocus = true;
			this.sbAge.Name = "sbAge";
			this.sbAge.Adjustment.PageIncrement = 10;
			this.sbAge.ClimbRate = 1;
			this.sbAge.Numeric = true;
			this.sbAge.Value = 4;
			this.table2.Add(this.sbAge);
			global::Gtk.Table.TableChild w36 = ((global::Gtk.Table.TableChild)(this.table2[this.sbAge]));
			w36.TopAttach = ((uint)(5));
			w36.BottomAttach = ((uint)(6));
			w36.LeftAttach = ((uint)(1));
			w36.RightAttach = ((uint)(2));
			w36.XOptions = ((global::Gtk.AttachOptions)(4));
			w36.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbBirthday = new global::Gtk.SpinButton(0, 100, 1);
			this.sbBirthday.CanFocus = true;
			this.sbBirthday.Name = "sbBirthday";
			this.sbBirthday.Adjustment.PageIncrement = 10;
			this.sbBirthday.ClimbRate = 1;
			this.sbBirthday.Numeric = true;
			this.sbBirthday.Value = 5;
			this.table2.Add(this.sbBirthday);
			global::Gtk.Table.TableChild w37 = ((global::Gtk.Table.TableChild)(this.table2[this.sbBirthday]));
			w37.TopAttach = ((uint)(6));
			w37.BottomAttach = ((uint)(7));
			w37.LeftAttach = ((uint)(1));
			w37.RightAttach = ((uint)(2));
			w37.XOptions = ((global::Gtk.AttachOptions)(4));
			w37.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbBlackList = new global::Gtk.SpinButton(0, 100, 1);
			this.sbBlackList.CanFocus = true;
			this.sbBlackList.Name = "sbBlackList";
			this.sbBlackList.Adjustment.PageIncrement = 10;
			this.sbBlackList.ClimbRate = 1;
			this.sbBlackList.Numeric = true;
			this.sbBlackList.Value = 8;
			this.table2.Add(this.sbBlackList);
			global::Gtk.Table.TableChild w38 = ((global::Gtk.Table.TableChild)(this.table2[this.sbBlackList]));
			w38.TopAttach = ((uint)(9));
			w38.BottomAttach = ((uint)(10));
			w38.LeftAttach = ((uint)(1));
			w38.RightAttach = ((uint)(2));
			w38.XOptions = ((global::Gtk.AttachOptions)(4));
			w38.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbCity = new global::Gtk.SpinButton(0, 100, 1);
			this.sbCity.CanFocus = true;
			this.sbCity.Name = "sbCity";
			this.sbCity.Adjustment.PageIncrement = 10;
			this.sbCity.ClimbRate = 1;
			this.sbCity.Numeric = true;
			this.sbCity.Value = 3;
			this.table2.Add(this.sbCity);
			global::Gtk.Table.TableChild w39 = ((global::Gtk.Table.TableChild)(this.table2[this.sbCity]));
			w39.TopAttach = ((uint)(4));
			w39.BottomAttach = ((uint)(5));
			w39.LeftAttach = ((uint)(1));
			w39.RightAttach = ((uint)(2));
			w39.XOptions = ((global::Gtk.AttachOptions)(4));
			w39.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbClub = new global::Gtk.SpinButton(0, 100, 1);
			this.sbClub.CanFocus = true;
			this.sbClub.Name = "sbClub";
			this.sbClub.Adjustment.PageIncrement = 10;
			this.sbClub.ClimbRate = 1;
			this.sbClub.Numeric = true;
			this.sbClub.Value = 2;
			this.table2.Add(this.sbClub);
			global::Gtk.Table.TableChild w40 = ((global::Gtk.Table.TableChild)(this.table2[this.sbClub]));
			w40.TopAttach = ((uint)(3));
			w40.BottomAttach = ((uint)(4));
			w40.LeftAttach = ((uint)(1));
			w40.RightAttach = ((uint)(2));
			w40.XOptions = ((global::Gtk.AttachOptions)(4));
			w40.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbExtraInfo = new global::Gtk.SpinButton(0, 100, 1);
			this.sbExtraInfo.CanFocus = true;
			this.sbExtraInfo.Name = "sbExtraInfo";
			this.sbExtraInfo.Adjustment.PageIncrement = 10;
			this.sbExtraInfo.ClimbRate = 1;
			this.sbExtraInfo.Numeric = true;
			this.sbExtraInfo.Value = 7;
			this.table2.Add(this.sbExtraInfo);
			global::Gtk.Table.TableChild w41 = ((global::Gtk.Table.TableChild)(this.table2[this.sbExtraInfo]));
			w41.TopAttach = ((uint)(8));
			w41.BottomAttach = ((uint)(9));
			w41.LeftAttach = ((uint)(1));
			w41.RightAttach = ((uint)(2));
			w41.XOptions = ((global::Gtk.AttachOptions)(4));
			w41.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbFirstName = new global::Gtk.SpinButton(0, 100, 1);
			this.sbFirstName.CanFocus = true;
			this.sbFirstName.Name = "sbFirstName";
			this.sbFirstName.Adjustment.PageIncrement = 10;
			this.sbFirstName.ClimbRate = 1;
			this.sbFirstName.Numeric = true;
			this.table2.Add(this.sbFirstName);
			global::Gtk.Table.TableChild w42 = ((global::Gtk.Table.TableChild)(this.table2[this.sbFirstName]));
			w42.TopAttach = ((uint)(1));
			w42.BottomAttach = ((uint)(2));
			w42.LeftAttach = ((uint)(1));
			w42.RightAttach = ((uint)(2));
			w42.XOptions = ((global::Gtk.AttachOptions)(4));
			w42.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbLastName = new global::Gtk.SpinButton(0, 100, 1);
			this.sbLastName.CanFocus = true;
			this.sbLastName.Name = "sbLastName";
			this.sbLastName.Adjustment.PageIncrement = 10;
			this.sbLastName.ClimbRate = 1;
			this.sbLastName.Numeric = true;
			this.sbLastName.Value = 1;
			this.table2.Add(this.sbLastName);
			global::Gtk.Table.TableChild w43 = ((global::Gtk.Table.TableChild)(this.table2[this.sbLastName]));
			w43.TopAttach = ((uint)(2));
			w43.BottomAttach = ((uint)(3));
			w43.LeftAttach = ((uint)(1));
			w43.RightAttach = ((uint)(2));
			w43.XOptions = ((global::Gtk.AttachOptions)(4));
			w43.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbRole = new global::Gtk.SpinButton(0, 100, 1);
			this.sbRole.CanFocus = true;
			this.sbRole.Name = "sbRole";
			this.sbRole.Adjustment.PageIncrement = 10;
			this.sbRole.ClimbRate = 1;
			this.sbRole.Numeric = true;
			this.sbRole.Value = 6;
			this.table2.Add(this.sbRole);
			global::Gtk.Table.TableChild w44 = ((global::Gtk.Table.TableChild)(this.table2[this.sbRole]));
			w44.TopAttach = ((uint)(7));
			w44.BottomAttach = ((uint)(8));
			w44.LeftAttach = ((uint)(1));
			w44.RightAttach = ((uint)(2));
			w44.XOptions = ((global::Gtk.AttachOptions)(4));
			w44.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add(this.table2);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.table2]));
			w45.Position = 1;
			// Container child vbox2.Gtk.Box+BoxChild
			this.btnImportCSV = new global::Gtk.Button();
			this.btnImportCSV.CanFocus = true;
			this.btnImportCSV.Name = "btnImportCSV";
			this.btnImportCSV.UseUnderline = true;
			this.btnImportCSV.Label = "Import CSV";
			this.vbox2.Add(this.btnImportCSV);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.btnImportCSV]));
			w46.Position = 2;
			w46.Expand = false;
			w46.Fill = false;
			this.Add(this.vbox2);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 543;
			this.DefaultHeight = 494;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this.btnChooseFile.Clicked += new global::System.EventHandler(this.OnBtnChooseFileClicked);
			this.cbHasHeaders.Toggled += new global::System.EventHandler(this.OnCbHasHeadersToggled);
			this.sbRole.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbLastName.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbFirstName.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbExtraInfo.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbClub.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbCity.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbBlackList.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbBirthday.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.sbAge.Changed += new global::System.EventHandler(this.OnSbChanged);
			this.btnImportCSV.Clicked += new global::System.EventHandler(this.OnBtnImportCSVClicked);
		}
	}
}
