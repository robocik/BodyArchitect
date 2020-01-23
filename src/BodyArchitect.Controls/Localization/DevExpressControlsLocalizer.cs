using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls
{
    class DevExpressControlsLocalizer : Localizer
    {
        public override string GetLocalizedString(StringId id)
        {
            switch (id)
            {
                case StringId.PictureEditMenuCopy:
                    return ControlsStrings.TextEditCopyMenu;
                case StringId.PictureEditMenuCut:
                    return ControlsStrings.TextEditCutMenu;
                case StringId.PictureEditMenuDelete:
                    return ControlsStrings.TextEditDeleteMenu;
                case StringId.PictureEditMenuPaste:
                    return ControlsStrings.TextEditPasteMenu;
                case StringId.PictureEditMenuSave:
                    return ControlsStrings.SaveMenu;
                case StringId.PictureEditMenuLoad:
                    return ControlsStrings.LoadMenu;
                case StringId.PictureEditSaveFileTitle:
                    return ControlsStrings.SaveAsMenu;
                case StringId.DateEditToday:
                    return ControlsStrings.DateEditToday;
                case StringId.DateEditClear:
                    return ControlsStrings.DateEditClear;
                case StringId.TextEditMenuCopy:
                    return ControlsStrings.TextEditCopyMenu;
                case StringId.TextEditMenuCut:
                    return ControlsStrings.TextEditCutMenu;
                case StringId.TextEditMenuPaste:
                    return ControlsStrings.TextEditPasteMenu;
                case StringId.TextEditMenuUndo:
                    return ControlsStrings.TextEditUndoMenu;
                case StringId.TextEditMenuSelectAll:
                    return ControlsStrings.TextEditSelectAllMenu;
                case StringId.TextEditMenuDelete:
                    return ControlsStrings.TextEditDeleteMenu;
                case StringId.Apply:
                    return ControlsStrings.OKButton;
                case StringId.Cancel:
                    return ControlsStrings.CancelButton;
                case StringId.XtraMessageBoxNoButtonText:
                    return ControlsStrings.NoButton;
                case StringId.XtraMessageBoxYesButtonText:
                    return ControlsStrings.YesButton;
                case StringId.XtraMessageBoxOkButtonText:
                    return ControlsStrings.OKButton;
                case StringId.XtraMessageBoxCancelButtonText:
                    return ControlsStrings.CancelButton;
            }
            return base.GetLocalizedString(id);
        }
    }
}
