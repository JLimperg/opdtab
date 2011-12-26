using System;
using OPDtabData;
using Gtk;
using System.Collections.Generic;
namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class RoundResultSheet : Gtk.Bin
	{
		RoomData roomData;
		
		/* intLabels: 
		 * 0, 2, 8 are Avg. for Gov (order kept even when drag-dropped)
		 * 1, 3, 7 are Avg. for Opp (order kept even when drag-dropped)
		 * 4, 5, 6 are Avg. for Free
		 * 9, 10   are Avg. for TeamGov/TeamOpp
		 * 11, 12  are SpeakerSum for Gov/Opp
		 * 13, 14  are Totals for Gov/Opp
		 * */		
		List<MyIntLabel> intLabels;		
		
		/* dataWidgets: 
		 * 0 - 8    are DebaterWidgets (ordered as intLabels),
		 * 9 - 10   are Teams
		 * 11 - inf are DebaterWidgets for judges...
		 * */
		List<IResultDataWidget> dataWidgets;
		
		/* bestOnes are enhanced RadioButtons
		 * 0 - 8    for best non-Team breaking Speaker (ordered as intLabels)
		 * 9 - 10   for best Team (Gov and Opp)
		 * lblWin and lblBest are global for convenience (don't need to search in table)
		 * */
		List<RoundResultSetBest> bestOnes;
		Label lblBestTeam;
		Label lblBestSpeaker;
		
		/* results:
		 * 0 - 8         are RoundResults for Debaters (ordered as intLabels)
		 * 8 - 8+nJudges are RoundResults for Judges
		 * */		
		List<RoundResultData> results;
		List<RoundResultData> resultsJudges;
		
		// global layout vars
		static uint judgesOffset = 2;
		static uint[] debaterRows = new uint[] {2, 3, 4, 5, 7, 8, 9, 11, 12, 16, 17}; 
		
		Gdk.Color[] colBg = new Gdk.Color[] {new Gdk.Color(213,229,255), // blueish for Gov
			new Gdk.Color(215,244,215), // greenish for Opp,
			new Gdk.Color(255,238,170) // yellowish for freeSpeakers
		};
		Gdk.Color colBgNone = new Gdk.Color(180,180,180); // darker gray
		Gdk.Color colBlack = new Gdk.Color(0,0,0);	
		
		// Some more handy arrays
		static int[] posGov = new int[] {0, 2, 8};
		static int[] posOpp = new int[] {1, 3, 7};
			
		public RoundResultSheet ()
		{
			this.Build ();
			SetupTable();
			Realized += delegate(object sender, EventArgs e) {
				SetRoomData(null, null);
			};							
		}	
		
		public void SetRoomData(string rN, RoomData rd) {
			// before updating roomData, do some stuff
			if(roomData != null) {
				RemoveTableColumns(judgesOffset, (uint)roomData.Judges.Count);
				// this ensures that the rows are in standard order
				// it's necessary for UpdateRoomData()
				RepositionTeamRowsBack(posGov);
				RepositionTeamRowsBack(posOpp);
			}
			roomData = rd;
			if(roomData != null) 
				UpdateRoomData();
			MiscHelpers.SetIsShown(lblNoRoomData, roomData == null);
			MiscHelpers.SetIsShown(table, roomData != null);			
		}
				
		void UpdateRoomData() {			
			dataWidgets = new List<IResultDataWidget>();
			results = new List<RoundResultData>();
			
			// 9 rows for speakers, 2 rows for both teams
			// the containers are always in standard order
			Container[] c = new Container[] {cGov1, cOpp1, cGov2, cOpp2,
				cFree1, cFree2, cFree3, cOpp3, cGov3, cTeamGov, cTeamOpp};
			
			List<object> o = roomData.AsOrderedObjects();
			for(int i=0;i<11;i++) {
				SetupTeamRow(c[i], o[i], i);
			}
						
			InsertTableColumns(judgesOffset, (uint)roomData.Judges.Count);			
			// JUDGES
			// clear all avg parents (9 Speakers, 2 Teams = 11) first
			for(int i=0;i<11;i++) 
				intLabels[i].ClearParents();
			
			// Setup Judge Column (fills resultsJudges)
			resultsJudges = new List<RoundResultData>();
			for(int i=0;i<roomData.Judges.Count;i++)
				SetupJudgeColumn(i);
			
			// CHECK CONSISTENCY
			CheckPositions();
			
			// GUI COL POSITIONS
			// (this needs to be done at the end!)
			RepositionJudgeCols();
			
			// GUI ROW POSITIONS
			RepositionTeamRows(posGov);
			RepositionTeamRows(posOpp);
			
			// the above changes FrameLabels, so undo it.
			SetSpeakerFrameLabels();
			
			// force total sum updates
			intLabels[intLabels.Count-1].ForceUpdateInt();
			intLabels[intLabels.Count-2].ForceUpdateInt();					
			// is complete?
			CheckCompleteFlags();			
			// update bestOnes...
			UpdateBestOnes();
			UpdateBestSpeaker();
		}
		
		void UpdateBestOnes() {
			// hide all
			MiscHelpers.SetIsShown(lblBestTeam, false);
			MiscHelpers.SetIsShown(lblBestSpeaker, false);
			for(int i=0;i<11;i++)
				MiscHelpers.SetIsShown(bestOnes[i], false);
			// determine best team
			int pGov = intLabels[13].GetInt();
			int pOpp = intLabels[14].GetInt();
			// is one of the teams incomplete?
			if(pGov<0 || pOpp<0) 
				return;
			if(pGov>pOpp) {
				bestOnes[9].Active = true;
				MiscHelpers.SetIsShown(bestOnes[9], true);
			}
			else if(pGov<pOpp) {
				bestOnes[10].Active = true;
				MiscHelpers.SetIsShown(bestOnes[10], true);
			}
			else {
				// conflict
				MiscHelpers.SetIsShown(lblBestTeam, true);
				// use saved information, if available
				if(roomData.BestTeam==1)
					bestOnes[10].Active = true;
				else
					bestOnes[9].Active = true;						
				// show both
				MiscHelpers.SetIsShown(bestOnes[9], true);
				MiscHelpers.SetIsShown(bestOnes[10], true);
			}
			// UpdateBestSpeaker is called by updating setting bestOnes.Active 
		}
		
		void UpdateBestSpeaker() {
			// hide best speaker stuff
			MiscHelpers.SetIsShown(lblBestSpeaker, false);
			for(int i=0;i<9;i++)
				MiscHelpers.SetIsShown(bestOnes[i], false);
			// continue only if teams are complete 
			if(intLabels[13].GetInt()<0 || intLabels[14].GetInt()<0) 
				return;
			// determine max points of non-teambreaking speaker
			int max = -1;
			List<int> exclude = new List<int>(roomData.BestTeam==1?posOpp:posGov);
			for(int i=0;i<9;i++) {
				int n = intLabels[i].GetInt();
				// abort if one is incomplete
				if(n<0)
					return;
				// exclude winning team
				if(exclude.Contains(i))
					continue;
				// set max
				if(n>max)
					max = n;
			}
			// show all buttons
			List<int> bestSpeakers = new List<int>();
			for(int i=0;i<9;i++) {
				int n = intLabels[i].GetInt();
				if(exclude.Contains(i))
					continue;
				if(n==max) {
					bestSpeakers.Add(i);
					MiscHelpers.SetIsShown(bestOnes[i], true);
					//bestOnes[i].Active = true;
				}
			}
			// do we have a valid saved result?
			if(bestSpeakers.Contains(roomData.BestSpeaker)) {
				bestOnes[roomData.BestSpeaker].Active = true;			
			}
			else {
				// default
				bestOnes[bestSpeakers[0]].Active = true;
			}
			// more than one speaker could break?
			if(bestSpeakers.Count>1) 
				MiscHelpers.SetIsShown(lblBestSpeaker, true);
			
		}
		
		void CheckCompleteFlags() {
			bool ok = true;
			List<int> toBeChecked = new List<int>();
			for(int i=4;i<7;i++)
				if(dataWidgets[i].HasResult)
					toBeChecked.Add(i);
			// Opp and Gov should always be checked..
			toBeChecked.Add(13);
			toBeChecked.Add(14);
			
			foreach(int i in toBeChecked)
				ok = intLabels[i].GetInt() >= 0  && ok;
			if(roomData!=null)
				roomData.ItemCompleted = ok;
			// update combobox at parent
			Widget w = GetAncestor(Window.GType);
			if(w is EditRoundResults)
				(w as EditRoundResults).UpdateComboboxes();
		}
		
		void CheckPositions() {			
			// speakers first
			bool ok = true;
			for(int i=0;i<results.Count;i++) {
				if(results[i] == null)
					continue; 
				ok = ok && (uint)results[i].Role == RoundResultData.PosToRoleType[i];
				if(results[i].Role == RoundResultData.RoleType.Free)
					// always parse free speaker pos silently
					results[i].ParsePosition(i);
			}
			// judges
			// possible indices...
			List<int> judgeIndex = new List<int>();
			for(int i=0;i<roomData.Judges.Count;i++) 
				judgeIndex.Add(i);
			for(int i=0;i<resultsJudges.Count;i++) {
				ok = ok && resultsJudges[i].Role == RoundResultData.RoleType.Judge 
					&& judgeIndex.Remove(resultsJudges[i].Index);
			}
			
			if(!ok) {
				Console.WriteLine("WARNING: Re-set positions and roles for judges." +
				 	" Data may be lost!");
				for(int i=0;i<results.Count;i++) {
					if(results[i] == null)
						continue; 
					results[i].ParsePosition(i);
					results[i].SetNJudges(roomData.Judges.Count);
				}
				for(int i=0;i<resultsJudges.Count;i++) {
					resultsJudges[i].Index = i;			
					resultsJudges[i].ClearJudgeData();
				}
			}
		}
		
		
		#region Setup Team
		void SetupTeamRow(Container c, object data, int i) {				
			if(i>8) {
				// a team is easy
				Team t = new Team((TeamData)data);
				MiscHelpers.AddToContainer(c, t);
				dataWidgets.Add(t);
			}
			else {
				DebaterWidget dw = new DebaterWidget((RoundDebater)data);
				dataWidgets.Add(dw);
				if(dw.HasResult)
					results.Add(dw.Debater.
					            GetRoundResult(roomData.RoundName, i, roomData.Judges.Count));
				else
					results.Add(null);
				
				uint type = RoundResultData.PosToRoleType[i];
				if(type != (uint)RoundResultData.RoleType.Free) {
					string section = type == 0 ? "OnlyGov" : "OnlyOpp";
					// Setup Drag Drop					
					dw.SetupDragDropSource(section, i);
					// Container ATTACH
					MiscHelpers.AddToContainer(c, dw);
					// DragDrop DEST
					DragDropHelpers.DestSet(dw,
		        		    	 DestDefaults.All,
		       				     DragDropHelpers.TgtFromString(section),
		        	    		 Gdk.DragAction.Move);
				
					dw.DragDataReceived += delegate(object s, DragDataReceivedArgs args) {
						int j = (int)DragDropHelpers.Deserialize(args.SelectionData);
						// Swap Rows
						SwapTableRows(debaterRows[results[i].GetPosition()],
				    	          debaterRows[results[j].GetPosition()]);
						// backswap the labels of the frames...
						SwapSpeakerFrameLabels(i, j);
						// save this in results						
						int tmp = results[i].Index;
						results[i].Index = results[j].Index;  
						results[j].Index = tmp; 
						// update infos
						dataWidgets[i].UpdateInfo();
						dataWidgets[j].UpdateInfo();
					};
				}
				else {
					MiscHelpers.AddToContainer(c, dw);
				}
			}		
		}
		
		void RepositionTeamRowsBack(int[] pos) {
			// checking the first should be enough
			if(results[pos[0]] == null)
				return;
			table.NRows += (uint)pos.Length;
			// move at end
			for(int i=0;i<pos.Length;i++) {
				MoveTableRow(debaterRows[results[pos[i]].GetPosition()],
				             (uint)(table.NRows-1-i));
			}
			// move to saved position
			for(int i=0;i<pos.Length;i++) {
				MoveTableRow((uint)(table.NRows-1-i), 
				             debaterRows[pos[i]]);
			}
			table.NRows -= (uint)pos.Length;	
		}
		
		void RepositionTeamRows(int[] pos) {
			// checking the first should be enough
			if(results[pos[0]] == null)
				return;
			table.NRows += (uint)pos.Length;
			// move at end
			for(int i=0;i<pos.Length;i++) {
				MoveTableRow(debaterRows[pos[i]], (uint)(table.NRows-1-i));
			}
			// move to saved position
			for(int i=0;i<pos.Length;i++) {
				MoveTableRow((uint)(table.NRows-1-i), 
				             debaterRows[results[pos[i]].GetPosition()]);
			}
			table.NRows -= (uint)pos.Length;
		}
		#endregion
			
		#region Setup Judge (including Result updating)
		void RepositionJudgeCols() {
			uint n = (uint)roomData.Judges.Count;
			table.NColumns += n;
			// move at end
			for(int i=0;i<n;i++)
				MoveTableCol((uint)(i+judgesOffset), 
				             (uint)(table.NColumns-1-i));
			// move to saved position
			for(int i=0;i<n;i++)
				MoveTableCol((uint)(table.NColumns-1-i), 
				             (uint)(resultsJudges[i].Index+judgesOffset));				
			table.NColumns -= n;
		}
		
		void SetupJudgeColumn(int i) {
			// Setup small DebaterWidget (keep colsize small...)
			DebaterWidget dw = DebaterWidget.Small(roomData.Judges[(int)i], false); 
			// add to results, nJudges doesnt matter for judges
			resultsJudges.Add(dw.Debater.GetRoundResult(roomData.RoundName, i, -1));
			dataWidgets.Add(dw);
			// default position
			uint pos = (uint)(i+judgesOffset);
			
			// THE FOLLOWING ORDER FOR DRAG DROP IS IMPORTANT, see DragDropHelpers
			
			// DragDrop SOURCE 
			// we send always the fixed index i
			
			if(roomData.Judges.Count>1)
				dw.SetupDragDropSource("Judge", i);
				//DragDropHelpers.SourceSet(dw, "Judge", i); 
			
			// Container ATTACH
			table.Attach(dw, pos, pos+1, 0, 1,
			             AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			
			// DragDrop DEST
			if(roomData.Judges.Count>1) {
				DragDropHelpers.DestSet(dw,
			            	 DestDefaults.All,
			        	     DragDropHelpers.TgtFromString("Judge"),
			    	         Gdk.DragAction.Move);
				
				dw.DragDataReceived += delegate(object o, DragDataReceivedArgs args) {
					int j = (int)DragDropHelpers.Deserialize(args.SelectionData);
					// Swap Columns
					SwapTableCols((uint)(resultsJudges[i].Index+judgesOffset),
					              (uint)(resultsJudges[j].Index+judgesOffset));
					// save this in results
					int tmp = resultsJudges[i].Index;
					resultsJudges[i].Index = resultsJudges[j].Index;  
					resultsJudges[j].Index = tmp; 
					// update infos
					dataWidgets[i+11].UpdateInfo();
					dataWidgets[j+11].UpdateInfo();
				};
			}
			
			// Setup Spinbuttons			
			int k = 0;
			foreach(uint row in debaterRows) {
				Alignment al = null;
				if(dataWidgets[k].HasResult) {
					// k denotes speaker position
					// k=9, k=10 are team points
					// i denotes judgeIndex					 
					MySpinButton sb = k<9 ? SetupSbSpeaker(i, k) : 
						SetupSbTeam(i, k);		
					// spinbuttons are parents for avg labels..
					intLabels[k].AddParent(sb);
					// nice alignment
					al = new Alignment(0f,0f,1f,1f);
					al.LeftPadding = 6;
					al.RightPadding = 6;
					al.Add(sb);
				}				
				table.Attach(MiscHelpers.MakeBackground(al, colBg[RoundResultData.PosToRoleType[k]]), 
				             pos, pos+1, row, row+1, 
				             AttachOptions.Fill, AttachOptions.Fill, 0, 0);
				k++;
			}
		}	
				
		MySpinButton SetupSbSpeaker(int i, int k) {
			MySpinButton sb = new MySpinButton(-1, 100, 1, i, k);
			sb.ValueChanged += MySpinbuttonChangedSpeaker;
			sb.Value = results[k].SpeakerScores[i];	
			return sb;		
		}
		
		void MySpinbuttonChangedSpeaker(object s, EventArgs e) {
			MySpinButton sb = (MySpinButton)s;
			int k = sb.DebaterIndex;
			int i = sb.JudgeIndex;
			// save at speaker
			results[k].SpeakerScores[i] = sb.ValueAsInt;
			// save at judge
			resultsJudges[i].SpeakerScores[k] = sb.ValueAsInt;
			// update info
			dataWidgets[k].UpdateInfo();
			dataWidgets[11+i].UpdateInfo();
		}		
		
		MySpinButton SetupSbTeam(int i, int k) {
			MySpinButton sb = new MySpinButton(-1, 200, 1, i, k);
			sb.ValueChanged += MySpinbuttonChangedTeam;
			if(k == 9)
				sb.Value = GetTeamScore(i, posGov);
			else if(k == 10)
				sb.Value = GetTeamScore(i, posOpp);
			else
				throw new IndexOutOfRangeException();
			return sb;	
		}
		
		int GetTeamScore(int i, int[] pos) {
			int val = results[pos[0]].TeamScores[i];
			// consistency check: all Debaters in one team 
			// should have same team points...
			for(int j=1;j<pos.Length;j++)
				if(val != results[pos[j]].TeamScores[i]) {
					Console.WriteLine("WARNING: Inconsistent team scores!");
					return -1;
				}
			return val;
		}		
		
		void MySpinbuttonChangedTeam(object s, EventArgs e) {
			MySpinButton sb = (MySpinButton)s;
			int k = sb.DebaterIndex;
			int i = sb.JudgeIndex;
			if(k==9) {
				// Gov
				// all speakers
				foreach(int p in posGov) 
					results[p].TeamScores[i] = sb.ValueAsInt;
			}
			else if(k==10) {
				// Opp
				// all speakers
				foreach(int p in posOpp) 
					results[p].TeamScores[i] = sb.ValueAsInt;
			}
			else
				throw new IndexOutOfRangeException();
			// judges
			resultsJudges[i].TeamScores[k-9] = sb.ValueAsInt;
			// UpdateInfo of affected Widgets
			dataWidgets[k].UpdateInfo();
			dataWidgets[11+i].UpdateInfo();
		}
		#endregion
		
		
		#region Table Helpers		
		void SwapTableCols(uint col1, uint col2) {
			MiscHelpers.SwapTableCols(table, col1, col2);			
		}
		
		void SwapTableRows(uint row1, uint row2) {
			MiscHelpers.SwapTableRows(table, row1, row2);
		}
		
		void MoveTableCol(uint source, uint dest) {
			MiscHelpers.MoveTableCol(table, source, dest);
		}
		
		void MoveTableRow(uint source, uint dest) {
			MiscHelpers.MoveTableRow(table, source, dest);
		}
		
		void InsertTableColumns(uint start, uint n) {
			MiscHelpers.InsertTableColumns(table, start, n);
		}
		
		void RemoveTableColumns(uint start, uint n) {
			MiscHelpers.RemoveTableColumns(table, start, n);
		}
		#endregion
		
		#region Gui Table Helpers
		
		void SetSpeakerFrameLabels() {
			foreach(int i in new int[] {0,1,2,3,7,8}) {
				DebaterWidget w = dataWidgets[i] as DebaterWidget;
				int j = i;
				if(w.HasResult)
					j = results[i].GetPosition();
				Frame f = w.GetAncestor(Frame.GType) as Frame;
				RoundResultData.RoleType role;
				int index;
				RoundResultData.ParsePosition(j, out role, out index);
				(f.LabelWidget as Label).Markup = "<i>"+(index+1)+". "+role+"</i>";
			}
		}
		
		void SwapSpeakerFrameLabels(int i1, int i2) {
			DebaterWidget w1 = dataWidgets[i1] as DebaterWidget;
			DebaterWidget w2 = dataWidgets[i2] as DebaterWidget;
			Frame f1 = w1.GetAncestor(Frame.GType) as Frame;
			Frame f2 = w2.GetAncestor(Frame.GType) as Frame;
			// do it the hard way...
			string text1 = f1.Label;
			string text2 = f2.Label;
			(f1.LabelWidget as Label).Markup = "<i>"+text2+"</i>";
			(f2.LabelWidget as Label).Markup = "<i>"+text1+"</i>";
		}
		
		void SetupTable() {
			// set size
			table.NColumns = 11;
			
			// INTEGER LABELS
			// Headers
			uint avgColOff = table.NColumns-8; // starting column offset
			
			AttachHeaderLabel("Gov", avgColOff);
			AttachHeaderLabel("Opp", avgColOff+1);
			AttachHeaderLabel("Free", avgColOff+2);
			lblBestSpeaker = AttachConflictLabel("?", avgColOff+3, 0);
			
			// averages for speakers
			intLabels = new List<MyIntLabel>();
			bestOnes = new List<RoundResultSetBest>();
			for(int i=0;i<debaterRows.Length;i++) {
				for(uint k=0;k<3;k++) {
					uint col = avgColOff+k;
					Gdk.Color color = colBgNone;
					MyIntLabel lbl = null;
					if(k==RoundResultData.PosToRoleType[i]) {
						lbl = new MyIntLabel(true);
						intLabels.Add(lbl);
						color = colBg[k];
					}						
					table.Attach(MiscHelpers.MakeBackground(lbl, color), 
					             col, col+1, debaterRows[i], debaterRows[i]+1,
				    	         AttachOptions.Fill, AttachOptions.Fill,0,0);
				}
				// add also radiobuttons, they are one group
				// but only for speakers, not for teams
				if(i<9) {
					RoundResultSetBest rb = new RoundResultSetBest(
						bestOnes.Count>0 ? bestOnes[0] : null, i);
					rb.Toggled += delegate(object sender, EventArgs e) {
						if(rb.Active && roomData != null)
							roomData.BestSpeaker = (sender as RoundResultSetBest).Index;
					};
					bestOnes.Add(rb);
					table.Attach(rb,
						avgColOff+3, avgColOff+4,
						debaterRows[i], debaterRows[i]+1,
						AttachOptions.Shrink, AttachOptions.Shrink,0,0);
				}
			}
			
			// PLUS
			AttachBigLabel("+", table.NColumns-5, table.NRows-2);
			AttachBigLabel("+", table.NColumns-5, table.NRows-1);
						
			// SPEAKER SUM
			AttachSumLabel(table.NColumns-4, table.NRows-2, colBg[0], posGov);
			AttachSumLabel(table.NColumns-4, table.NRows-1, colBg[1], posOpp);
			
			// EQUALS
			AttachBigLabel("=", table.NColumns-3, table.NRows-2);
			AttachBigLabel("=", table.NColumns-3, table.NRows-1);
			
			// TOTAL
			AttachSumLabel(table.NColumns-2, table.NRows-2, colBg[0], 9, 11);
			AttachSumLabel(table.NColumns-2, table.NRows-1, colBg[1], 10, 12);
			
			
			// connect to intLabels to update completeFlags
			foreach(int i in new int[] {4,5,6,13,14})
				intLabels[i].IntChanged += delegate(object sender, EventArgs e) {
					CheckCompleteFlags();
				};
			
			// Win? Label
			lblBestTeam = AttachConflictLabel("?", table.NColumns-1, table.NRows-4);
			
			// complete bestOnes with Team buttons
			RoundResultSetBest rbGov = new RoundResultSetBest(null, 9);
			RoundResultSetBest rbOpp = new RoundResultSetBest(rbGov, 10);
			rbGov.Toggled += delegate(object sender, EventArgs e) {
				if(rbGov.Active && roomData != null) {
					roomData.BestTeam = 0;
					UpdateBestSpeaker();
				}
			};
			rbOpp.Toggled += delegate(object sender, EventArgs e) {
				if(rbOpp.Active && roomData != null) {
					roomData.BestTeam = 1;
					UpdateBestSpeaker();
				}
			};
			bestOnes.Add(rbGov);
			bestOnes.Add(rbOpp);			
			
			table.Attach(rbGov, 
				table.NColumns-1, table.NColumns,
				table.NRows-2, table.NRows-1, 
			             AttachOptions.Shrink, AttachOptions.Shrink,
			             12, 0);
			table.Attach(rbOpp, 
				table.NColumns-1, table.NColumns,
				table.NRows-1, table.NRows,
			             AttachOptions.Shrink, AttachOptions.Shrink,
			             12, 0);
			
			// connect to intLabels to update bestOnes
			for(int i=0;i<9;i++) {
				intLabels[i].IntChanged += delegate(object sender, EventArgs e) {
					UpdateBestSpeaker();
				};
			}
			intLabels[13].IntChanged += delegate(object sender, EventArgs e) {
				UpdateBestOnes();
			};
			intLabels[14].IntChanged += delegate(object sender, EventArgs e) {
				UpdateBestOnes();
			};
			
			
			// COLOR DEBATER BACKGROUNDS			
			ebGov1.ModifyBg(StateType.Normal, colBg[0]);
			ebGov2.ModifyBg(StateType.Normal, colBg[0]);
			ebGov3.ModifyBg(StateType.Normal, colBg[0]);
			ebTeamGov.ModifyBg(StateType.Normal, colBg[0]);
			
			ebOpp1.ModifyBg(StateType.Normal, colBg[1]);
			ebOpp2.ModifyBg(StateType.Normal, colBg[1]);
			ebOpp3.ModifyBg(StateType.Normal, colBg[1]);
			ebTeamOpp.ModifyBg(StateType.Normal, colBg[1]);
			
			ebFree1.ModifyBg(StateType.Normal, colBg[2]);
			ebFree2.ModifyBg(StateType.Normal, colBg[2]);
			ebFree3.ModifyBg(StateType.Normal, colBg[2]);
			
			// GRIDLINES			
			// vertical
			MyGridLine.V(table, 1, 1, colBlack);
			MyGridLine.V(table, table.NColumns-9, 1, colBlack);
						
			// horizontal
			MyGridLine.H(table, 1, table.NColumns-5, 2, colBlack);
			MyGridLine.H(table, table.NRows-3, table.NColumns-5, 2, colBlack);			
		}		
		
		void AttachHeaderLabel(string text, uint col) {
			Label lbl = new Label();
			lbl.Markup = "<b>"+text+"</b>";
			table.Attach(lbl, col, col+1, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 6, 0);
		}
		
		void AttachSumLabel(uint col, uint row, Gdk.Color color, params int[] w) {
			MyIntLabel sumSpeaker = new MyIntLabel(false); 
			foreach(int i in w) 
				sumSpeaker.AddParent(intLabels[i]);
			
			intLabels.Add(sumSpeaker);
			table.Attach(MiscHelpers.MakeBackground(sumSpeaker, color),
			             col, col+1, 
			             row, row+1,
			             AttachOptions.Fill, AttachOptions.Fill,
			             0,0);
		}
		
		void AttachBigLabel(string text, uint col, uint row) {
			AttachLabel("<big><big><big>"+text+"</big></big></big>", col, row);
		}
		
		Label AttachConflictLabel(string text, uint col, uint row) {
			return AttachLabel("<big><big><span foreground=\"red\">"+text+"</span></big></big>", col, row);	
		}
		
		Label AttachLabel(string text, uint col, uint row) {
			Label lbl = new Label();
			lbl.Markup = text;
			table.Attach(lbl, col, col+1, row, row+1, 
			             AttachOptions.Shrink, AttachOptions.Shrink,
			             12, 0);
			return lbl;
		}
		
		#endregion
	}
}

