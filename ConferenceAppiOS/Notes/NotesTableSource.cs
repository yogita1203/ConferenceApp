using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS.Notes
{
	internal class NotesTableSource : UITableViewSource
	{
		static nfloat RowHeight = 50;
		NotesTableController notesTableController;
		List<BuiltNotes> notes;
		string noData = AppTheme.NoNotesData;
		NSString cellIdentifier = new NSString("NoteCell");
		NSString deafaultCellIdentifier = new NSString("DefaultNoteCell");

		public NSIndexPath selectedIndex;

		public NotesTableSource(NotesTableController notesTableController, List<BuiltNotes> notes)
		{
			this.notesTableController = notesTableController;
			this.notes = notes;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			if (notes.Count > 0) {
				NotesTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as NotesTableCell;
				if (cell == null) cell = new NotesTableCell(cellIdentifier);
				var item = notes[indexPath.Row];
				cell.UpdateCell(item);
				return cell;
			} else {
				UITableViewCell cell = tableView.DequeueReusableCell(deafaultCellIdentifier) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, deafaultCellIdentifier);

				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.TextLabel.TextAlignment = UITextAlignment.Center;
				cell.TextLabel.Text = noData;
				cell.TextLabel.TextColor = AppTheme.NTnotesCellTitleColor;
				cell.TextLabel.BackgroundColor = UIColor.Clear;
				cell.TextLabel.Font = AppFonts.ProximaNovaRegular(16);

				return cell;
			}

		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (notes.Count > 0) {
				selectedIndex = indexPath;

				if (notesTableController.NoteChangedHandler != null)
					notesTableController.NoteChangedHandler (notes [indexPath.Row]);
			} 
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			if (notes.Count > 0)
				return notes.Count;
			else
				return 1;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
            if (notes.Count > 0)
            {
                var item = notes[indexPath.Row];

                if (item.created_at != null && item.created_at.Length > 0)
                {
                    return AppTheme.DHcellRowHeight;
                }
                else
                {
                    return RowHeight;
                }
            }
            else
            {
                return RowHeight;
            }
		}

		internal void UpdateSource(List<BuiltNotes> notes)
		{
			this.notes = notes;
		}
	}
}