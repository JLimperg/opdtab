
// This file has been generated by the GUI designer. Do not modify.
namespace OPDtabGui
{
	public partial class Room
	{
		private global::Gtk.Alignment alignment1;

		private global::Gtk.VBox vbox6;

		private global::Gtk.Frame frameBig;

		private global::Gtk.Alignment GtkAlignment;

		private global::Gtk.VBox vbox3;

		private global::Gtk.HBox hbox5;

		private global::Gtk.Frame frameGov;

		private global::Gtk.Alignment cGovBig;

		private global::Gtk.EventBox cGov;

		private global::Gtk.Label GtkLabel1;

		private global::Gtk.Frame frameOpp;

		private global::Gtk.Alignment cOppBig;

		private global::Gtk.EventBox cOpp;

		private global::Gtk.Label GtkLabel2;

		private global::Gtk.HBox hbox8;

		private global::Gtk.VBox vbox2;

		private global::Gtk.Frame frame3;

		private global::Gtk.Alignment cFreeSpeakersBig;

		private global::Gtk.Table cFreeSpeakers;

		private global::Gtk.Label GtkLabel3;

		private global::Gtk.Frame frame5;

		private global::Gtk.Alignment GtkAlignment4;

		private global::Gtk.EventBox ebStatus;

		private global::Gtk.VBox vbox7;

		private global::Gtk.Label lblConflictStatus;

		private global::Gtk.Label lblJudgeStats;

		private global::Gtk.Table tableJudgeStars;

		private global::Gtk.Label GtkLabel5;

		private global::Gtk.VBox vbox1;

		private global::Gtk.Frame frame2;

		private global::Gtk.Alignment cChairBig;

		private global::Gtk.Alignment cChair;

		private global::Gtk.Label GtkLabel4;

		private global::Gtk.Frame frame1;

		private global::Gtk.Alignment cJudgesBig;

		private global::Gtk.Table cJudges;

		private global::Gtk.Label GtkLabel;

		private global::Gtk.Label labelRoomName;

		private global::Gtk.Alignment alignSmall;

		private global::Gtk.Table tableSmall;

		private global::Gtk.EventBox cChairSmall;

		private global::Gtk.EventBox cFreeSpeakersSmall;

		private global::Gtk.EventBox cGovSmall;

		private global::Gtk.EventBox cJudgesSmall;

		private global::Gtk.EventBox cOppSmall;

		private global::Gtk.EventBox eventbox1;

		private global::Gtk.EventBox eventbox2;

		private global::Gtk.EventBox eventbox3;

		private global::Gtk.Label labelRoomNo;

		private global::Gtk.Label labelRoomNoRight;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget OPDtabGui.Room
			global::Stetic.BinContainer.Attach(this);
			this.Name = "OPDtabGui.Room";
			// Container child OPDtabGui.Room.Gtk.Container+ContainerChild
			this.alignment1 = new global::Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
			this.alignment1.Name = "alignment1";
			this.alignment1.TopPadding = ((uint)(5));
			this.alignment1.BottomPadding = ((uint)(5));
			// Container child alignment1.Gtk.Container+ContainerChild
			this.vbox6 = new global::Gtk.VBox();
			this.vbox6.Name = "vbox6";
			// Container child vbox6.Gtk.Box+BoxChild
			this.frameBig = new global::Gtk.Frame();
			this.frameBig.Name = "frameBig";
			this.frameBig.LabelXalign = 0.5F;
			// Container child frameBig.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.BorderWidth = ((uint)(5));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox();
			this.hbox5.Name = "hbox5";
			this.hbox5.Homogeneous = true;
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.frameGov = new global::Gtk.Frame();
			this.frameGov.Name = "frameGov";
			this.frameGov.BorderWidth = ((uint)(2));
			// Container child frameGov.Gtk.Container+ContainerChild
			this.cGovBig = new global::Gtk.Alignment(0F, 0F, 1F, 0F);
			this.cGovBig.Name = "cGovBig";
			this.cGovBig.LeftPadding = ((uint)(12));
			this.cGovBig.RightPadding = ((uint)(6));
			// Container child cGovBig.Gtk.Container+ContainerChild
			this.cGov = new global::Gtk.EventBox();
			this.cGov.Name = "cGov";
			this.cGovBig.Add(this.cGov);
			this.frameGov.Add(this.cGovBig);
			this.GtkLabel1 = new global::Gtk.Label();
			this.GtkLabel1.Name = "GtkLabel1";
			this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString("Government");
			this.frameGov.LabelWidget = this.GtkLabel1;
			this.hbox5.Add(this.frameGov);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.frameGov]));
			w3.Position = 0;
			w3.Expand = false;
			// Container child hbox5.Gtk.Box+BoxChild
			this.frameOpp = new global::Gtk.Frame();
			this.frameOpp.Name = "frameOpp";
			this.frameOpp.BorderWidth = ((uint)(2));
			// Container child frameOpp.Gtk.Container+ContainerChild
			this.cOppBig = new global::Gtk.Alignment(0F, 0F, 1F, 0F);
			this.cOppBig.Name = "cOppBig";
			this.cOppBig.LeftPadding = ((uint)(12));
			this.cOppBig.RightPadding = ((uint)(6));
			// Container child cOppBig.Gtk.Container+ContainerChild
			this.cOpp = new global::Gtk.EventBox();
			this.cOpp.Name = "cOpp";
			this.cOppBig.Add(this.cOpp);
			this.frameOpp.Add(this.cOppBig);
			this.GtkLabel2 = new global::Gtk.Label();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString("Opposition");
			this.frameOpp.LabelWidget = this.GtkLabel2;
			this.hbox5.Add(this.frameOpp);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.frameOpp]));
			w6.Position = 1;
			w6.Expand = false;
			this.vbox3.Add(this.hbox5);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox5]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox8 = new global::Gtk.HBox();
			this.hbox8.Name = "hbox8";
			this.hbox8.Homogeneous = true;
			this.hbox8.Spacing = 6;
			// Container child hbox8.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox();
			this.vbox2.Name = "vbox2";
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame3 = new global::Gtk.Frame();
			this.frame3.Name = "frame3";
			this.frame3.BorderWidth = ((uint)(2));
			// Container child frame3.Gtk.Container+ContainerChild
			this.cFreeSpeakersBig = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.cFreeSpeakersBig.Name = "cFreeSpeakersBig";
			this.cFreeSpeakersBig.LeftPadding = ((uint)(12));
			this.cFreeSpeakersBig.RightPadding = ((uint)(6));
			// Container child cFreeSpeakersBig.Gtk.Container+ContainerChild
			this.cFreeSpeakers = new global::Gtk.Table(((uint)(1)), ((uint)(1)), false);
			this.cFreeSpeakers.Name = "cFreeSpeakers";
			this.cFreeSpeakersBig.Add(this.cFreeSpeakers);
			this.frame3.Add(this.cFreeSpeakersBig);
			this.GtkLabel3 = new global::Gtk.Label();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString("Free Speakers");
			this.frame3.LabelWidget = this.GtkLabel3;
			this.vbox2.Add(this.frame3);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame3]));
			w10.Position = 0;
			w10.Expand = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame5 = new global::Gtk.Frame();
			this.frame5.Name = "frame5";
			// Container child frame5.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			this.GtkAlignment4.LeftPadding = ((uint)(12));
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.ebStatus = new global::Gtk.EventBox();
			this.ebStatus.Name = "ebStatus";
			// Container child ebStatus.Gtk.Container+ContainerChild
			this.vbox7 = new global::Gtk.VBox();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			// Container child vbox7.Gtk.Box+BoxChild
			this.lblConflictStatus = new global::Gtk.Label();
			this.lblConflictStatus.Name = "lblConflictStatus";
			this.lblConflictStatus.Xalign = 0F;
			this.lblConflictStatus.LabelProp = global::Mono.Unix.Catalog.GetString("label5");
			this.vbox7.Add(this.lblConflictStatus);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.lblConflictStatus]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.lblJudgeStats = new global::Gtk.Label();
			this.lblJudgeStats.Name = "lblJudgeStats";
			this.lblJudgeStats.Xalign = 0F;
			this.lblJudgeStats.LabelProp = global::Mono.Unix.Catalog.GetString("label2");
			this.vbox7.Add(this.lblJudgeStats);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.lblJudgeStats]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.tableJudgeStars = new global::Gtk.Table(((uint)(3)), ((uint)(5)), false);
			this.tableJudgeStars.Name = "tableJudgeStars";
			this.tableJudgeStars.RowSpacing = ((uint)(6));
			this.tableJudgeStars.ColumnSpacing = ((uint)(6));
			this.vbox7.Add(this.tableJudgeStars);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.tableJudgeStars]));
			w13.Position = 2;
			w13.Expand = false;
			w13.Fill = false;
			this.ebStatus.Add(this.vbox7);
			this.GtkAlignment4.Add(this.ebStatus);
			this.frame5.Add(this.GtkAlignment4);
			this.GtkLabel5 = new global::Gtk.Label();
			this.GtkLabel5.Name = "GtkLabel5";
			this.GtkLabel5.LabelProp = global::Mono.Unix.Catalog.GetString("Status");
			this.frame5.LabelWidget = this.GtkLabel5;
			this.vbox2.Add(this.frame5);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame5]));
			w17.Position = 1;
			w17.Expand = false;
			this.hbox8.Add(this.vbox2);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox8 [this.vbox2]));
			w18.Position = 0;
			// Container child hbox8.Gtk.Box+BoxChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame();
			this.frame2.Name = "frame2";
			// Container child frame2.Gtk.Container+ContainerChild
			this.cChairBig = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.cChairBig.Name = "cChairBig";
			this.cChairBig.LeftPadding = ((uint)(12));
			this.cChairBig.RightPadding = ((uint)(6));
			// Container child cChairBig.Gtk.Container+ContainerChild
			this.cChair = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.cChair.Name = "cChair";
			this.cChairBig.Add(this.cChair);
			this.frame2.Add(this.cChairBig);
			this.GtkLabel4 = new global::Gtk.Label();
			this.GtkLabel4.Name = "GtkLabel4";
			this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString("Chair");
			this.frame2.LabelWidget = this.GtkLabel4;
			this.vbox1.Add(this.frame2);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frame2]));
			w21.Position = 0;
			w21.Expand = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame();
			this.frame1.Name = "frame1";
			// Container child frame1.Gtk.Container+ContainerChild
			this.cJudgesBig = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.cJudgesBig.Name = "cJudgesBig";
			this.cJudgesBig.LeftPadding = ((uint)(12));
			this.cJudgesBig.RightPadding = ((uint)(6));
			// Container child cJudgesBig.Gtk.Container+ContainerChild
			this.cJudges = new global::Gtk.Table(((uint)(1)), ((uint)(1)), false);
			this.cJudges.Name = "cJudges";
			this.cJudgesBig.Add(this.cJudges);
			this.frame1.Add(this.cJudgesBig);
			this.GtkLabel = new global::Gtk.Label();
			this.GtkLabel.Name = "GtkLabel";
			this.GtkLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Judges");
			this.frame1.LabelWidget = this.GtkLabel;
			this.vbox1.Add(this.frame1);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frame1]));
			w24.Position = 1;
			w24.Expand = false;
			w24.Fill = false;
			this.hbox8.Add(this.vbox1);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.hbox8 [this.vbox1]));
			w25.Position = 1;
			this.vbox3.Add(this.hbox8);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox8]));
			w26.Position = 1;
			w26.Expand = false;
			w26.Fill = false;
			this.GtkAlignment.Add(this.vbox3);
			this.frameBig.Add(this.GtkAlignment);
			this.labelRoomName = new global::Gtk.Label();
			this.labelRoomName.Name = "labelRoomName";
			this.labelRoomName.LabelProp = global::Mono.Unix.Catalog.GetString("<b>RoomName</b>");
			this.labelRoomName.UseMarkup = true;
			this.frameBig.LabelWidget = this.labelRoomName;
			this.vbox6.Add(this.frameBig);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.frameBig]));
			w29.Position = 0;
			// Container child vbox6.Gtk.Box+BoxChild
			this.alignSmall = new global::Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
			this.alignSmall.Name = "alignSmall";
			this.alignSmall.LeftPadding = ((uint)(10));
			// Container child alignSmall.Gtk.Container+ContainerChild
			this.tableSmall = new global::Gtk.Table(((uint)(2)), ((uint)(7)), false);
			this.tableSmall.Name = "tableSmall";
			this.tableSmall.ColumnSpacing = ((uint)(6));
			// Container child tableSmall.Gtk.Table+TableChild
			this.cChairSmall = new global::Gtk.EventBox();
			this.cChairSmall.Name = "cChairSmall";
			this.tableSmall.Add(this.cChairSmall);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.cChairSmall]));
			w30.TopAttach = ((uint)(1));
			w30.BottomAttach = ((uint)(2));
			w30.LeftAttach = ((uint)(4));
			w30.RightAttach = ((uint)(5));
			w30.XOptions = ((global::Gtk.AttachOptions)(4));
			w30.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child tableSmall.Gtk.Table+TableChild
			this.cFreeSpeakersSmall = new global::Gtk.EventBox();
			this.cFreeSpeakersSmall.Name = "cFreeSpeakersSmall";
			this.tableSmall.Add(this.cFreeSpeakersSmall);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.cFreeSpeakersSmall]));
			w31.TopAttach = ((uint)(1));
			w31.BottomAttach = ((uint)(2));
			w31.LeftAttach = ((uint)(3));
			w31.RightAttach = ((uint)(4));
			w31.XOptions = ((global::Gtk.AttachOptions)(0));
			w31.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child tableSmall.Gtk.Table+TableChild
			this.cGovSmall = new global::Gtk.EventBox();
			this.cGovSmall.Name = "cGovSmall";
			this.tableSmall.Add(this.cGovSmall);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.cGovSmall]));
			w32.TopAttach = ((uint)(1));
			w32.BottomAttach = ((uint)(2));
			w32.LeftAttach = ((uint)(1));
			w32.RightAttach = ((uint)(2));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child tableSmall.Gtk.Table+TableChild
			this.cJudgesSmall = new global::Gtk.EventBox();
			this.cJudgesSmall.Name = "cJudgesSmall";
			this.tableSmall.Add(this.cJudgesSmall);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.cJudgesSmall]));
			w33.TopAttach = ((uint)(1));
			w33.BottomAttach = ((uint)(2));
			w33.LeftAttach = ((uint)(5));
			w33.RightAttach = ((uint)(6));
			w33.XOptions = ((global::Gtk.AttachOptions)(0));
			w33.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child tableSmall.Gtk.Table+TableChild
			this.cOppSmall = new global::Gtk.EventBox();
			this.cOppSmall.Name = "cOppSmall";
			this.tableSmall.Add(this.cOppSmall);
			global::Gtk.Table.TableChild w34 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.cOppSmall]));
			w34.TopAttach = ((uint)(1));
			w34.BottomAttach = ((uint)(2));
			w34.LeftAttach = ((uint)(2));
			w34.RightAttach = ((uint)(3));
			w34.XOptions = ((global::Gtk.AttachOptions)(4));
			w34.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child tableSmall.Gtk.Table+TableChild
			this.eventbox1 = new global::Gtk.EventBox();
			this.eventbox1.WidthRequest = 140;
			this.eventbox1.Name = "eventbox1";
			this.tableSmall.Add(this.eventbox1);
			global::Gtk.Table.TableChild w35 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.eventbox1]));
			w35.LeftAttach = ((uint)(1));
			w35.RightAttach = ((uint)(2));
			w35.XOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableSmall.Gtk.Table+TableChild
			this.eventbox2 = new global::Gtk.EventBox();
			this.eventbox2.WidthRequest = 140;
			this.eventbox2.Name = "eventbox2";
			this.tableSmall.Add(this.eventbox2);
			global::Gtk.Table.TableChild w36 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.eventbox2]));
			w36.LeftAttach = ((uint)(2));
			w36.RightAttach = ((uint)(3));
			w36.XOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableSmall.Gtk.Table+TableChild
			this.eventbox3 = new global::Gtk.EventBox();
			this.eventbox3.WidthRequest = 120;
			this.eventbox3.Name = "eventbox3";
			this.tableSmall.Add(this.eventbox3);
			global::Gtk.Table.TableChild w37 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.eventbox3]));
			w37.LeftAttach = ((uint)(4));
			w37.RightAttach = ((uint)(5));
			w37.XOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableSmall.Gtk.Table+TableChild
			this.labelRoomNo = new global::Gtk.Label();
			this.labelRoomNo.Name = "labelRoomNo";
			this.labelRoomNo.Xalign = 0F;
			this.labelRoomNo.LabelProp = global::Mono.Unix.Catalog.GetString("label3");
			this.tableSmall.Add(this.labelRoomNo);
			global::Gtk.Table.TableChild w38 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.labelRoomNo]));
			w38.TopAttach = ((uint)(1));
			w38.BottomAttach = ((uint)(2));
			w38.XOptions = ((global::Gtk.AttachOptions)(4));
			w38.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableSmall.Gtk.Table+TableChild
			this.labelRoomNoRight = new global::Gtk.Label();
			this.labelRoomNoRight.Name = "labelRoomNoRight";
			this.labelRoomNoRight.Xalign = 1F;
			this.labelRoomNoRight.Justify = ((global::Gtk.Justification)(1));
			this.tableSmall.Add(this.labelRoomNoRight);
			global::Gtk.Table.TableChild w39 = ((global::Gtk.Table.TableChild)(this.tableSmall [this.labelRoomNoRight]));
			w39.TopAttach = ((uint)(1));
			w39.BottomAttach = ((uint)(2));
			w39.LeftAttach = ((uint)(6));
			w39.RightAttach = ((uint)(7));
			w39.YOptions = ((global::Gtk.AttachOptions)(0));
			this.alignSmall.Add(this.tableSmall);
			this.vbox6.Add(this.alignSmall);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.alignSmall]));
			w41.Position = 1;
			this.alignment1.Add(this.vbox6);
			this.Add(this.alignment1);
			if ((this.Child != null)) {
				this.Child.ShowAll();
			}
			this.Hide();
		}
	}
}
