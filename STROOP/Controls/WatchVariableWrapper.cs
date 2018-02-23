﻿using STROOP.Extensions;
using STROOP.Forms;
using STROOP.Managers;
using STROOP.Structs;
using STROOP.Structs.Configurations;
using STROOP.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace STROOP.Controls
{
    public class WatchVariableWrapper
    {
        // Defaults
        protected const int DEFAULT_ROUNDING_LIMIT = 3;
        protected const bool DEFAULT_DISPLAY_AS_HEX = false;
        protected const bool DEFAULT_USE_CHECKBOX = false;

        // Main objects
        protected readonly WatchVariable _watchVar;
        protected readonly WatchVariableControl _watchVarControl;
        protected readonly BetterContextMenuStrip _contextMenuStrip;

        // Main items
        private ToolStripMenuItem _itemHighlight;
        private ToolStripMenuItem _itemLock;
        private ToolStripMenuItem _itemRemoveAllLocks;

        // Custom items
        private ToolStripSeparator _separatorCustom;
        private ToolStripMenuItem _itemFixAddress;
        private ToolStripMenuItem _itemRename;
        private ToolStripMenuItem _itemDelete;

        // Fields
        private readonly bool _startsAsCheckbox;

        public static WatchVariableWrapper CreateWatchVariableWrapper(
            WatchVariable watchVar,
            WatchVariableControl watchVarControl,
            WatchVariableSubclass subclass,
            bool? useHex,
            bool? invertBool,
            WatchVariableCoordinate? coordinate)
        {
            switch (subclass)
            {
                case WatchVariableSubclass.String:
                    return new WatchVariableWrapper(watchVar, watchVarControl);

                case WatchVariableSubclass.Number:
                    return new WatchVariableNumberWrapper(
                        watchVar,
                        watchVarControl,
                        DEFAULT_ROUNDING_LIMIT,
                        useHex,
                        DEFAULT_USE_CHECKBOX,
                        coordinate);

                case WatchVariableSubclass.Angle:
                    return new WatchVariableAngleWrapper(watchVar, watchVarControl);

                case WatchVariableSubclass.Object:
                    return new WatchVariableObjectWrapper(watchVar, watchVarControl);

                case WatchVariableSubclass.Boolean:
                    return new WatchVariableBooleanWrapper(watchVar, watchVarControl, invertBool);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected WatchVariableWrapper(WatchVariable watchVar, WatchVariableControl watchVarControl, bool useCheckbox = false)
        {
            _watchVar = watchVar;
            _watchVarControl = watchVarControl;

            _startsAsCheckbox = useCheckbox;
            _contextMenuStrip = new BetterContextMenuStrip();
            AddContextMenuStripItems();
            AddExternalContextMenuStripItems();
            AddCustomContextMenuStripItems();
        }

        public bool StartsAsCheckbox()
        {
            return _startsAsCheckbox;
        }

        public ContextMenuStrip GetContextMenuStrip()
        {
            return _contextMenuStrip;
        }

        private void AddContextMenuStripItems()
        {
            _itemHighlight = new ToolStripMenuItem("Highlight");
            _itemHighlight.Click += (sender, e) => ToggleHighlighted();
            _itemHighlight.Checked = _watchVarControl.ShowBorder;

            _itemLock = new ToolStripMenuItem("Lock");
            _itemLock.Click += (sender, e) => ToggleLocked(_watchVarControl.FixedAddressList);

            _itemRemoveAllLocks = new ToolStripMenuItem("Remove All Locks");
            _itemRemoveAllLocks.Click += (sender, e) => WatchVariableLockManager.RemoveAllLocks();

            ToolStripMenuItem itemCopyUnrounded = new ToolStripMenuItem("Copy");
            itemCopyUnrounded.Click += (sender, e) => Clipboard.SetText(GetValue(false));

            ToolStripMenuItem itemPaste = new ToolStripMenuItem("Paste");
            itemPaste.Click += (sender, e) => _watchVarControl.SetValue(Clipboard.GetText());

            _contextMenuStrip.AddToBeginningList(_itemHighlight);
            _contextMenuStrip.AddToBeginningList(_itemLock);
            _contextMenuStrip.AddToBeginningList(_itemRemoveAllLocks);
            _contextMenuStrip.AddToBeginningList(itemCopyUnrounded);
            _contextMenuStrip.AddToBeginningList(itemPaste);
        }

        private void AddExternalContextMenuStripItems()
        {
            ToolStripMenuItem itemPanelOptions = new ToolStripMenuItem("Panel Options");
            itemPanelOptions.Click += (sender, e) =>
            {
                _watchVarControl.OpenPanelOptions(_contextMenuStrip.Bounds.Location);
            };

            ToolStripMenuItem itemOpenController = new ToolStripMenuItem("Open Controller");
            itemOpenController.Click += (sender, e) => { ShowVarController(); };

            ToolStripMenuItem itemAddToCustomTab = new ToolStripMenuItem("Add to Custom Tab");
            itemAddToCustomTab.Click += (sender, e) => { _watchVarControl.AddToCustomTab(false, false); };

            _contextMenuStrip.AddToEndingList(new ToolStripSeparator());
            _contextMenuStrip.AddToEndingList(itemPanelOptions);
            _contextMenuStrip.AddToEndingList(itemOpenController);
            _contextMenuStrip.AddToEndingList(itemAddToCustomTab);
        }

        private void AddCustomContextMenuStripItems()
        {
            _separatorCustom = new ToolStripSeparator();
            _separatorCustom.Visible = false;

            _itemFixAddress = new ToolStripMenuItem("Fix Address");
            _itemFixAddress.Click += (sender, e) =>
            {
                _watchVarControl.ToggleFixedAddress();
                _itemFixAddress.Checked = _watchVarControl.FixedAddressList != null;
            };
            _itemFixAddress.Visible = false;

            _itemRename = new ToolStripMenuItem("Rename");
            _itemRename.Click += (sender, e) => { _watchVarControl.RenameMode = true; };
            _itemRename.Visible = false;

            _itemDelete = new ToolStripMenuItem("Delete");
            _itemDelete.Click += (sender, e) => { _watchVarControl.DeleteFromPanel(); };
            _itemDelete.Visible = false;

            _contextMenuStrip.AddToEndingList(_separatorCustom);
            _contextMenuStrip.AddToEndingList(_itemFixAddress);
            _contextMenuStrip.AddToEndingList(_itemRename);
            _contextMenuStrip.AddToEndingList(_itemDelete);
        }

        public void ShowVarInfo()
        {
            VariableViewerForm varInfo =
                new VariableViewerForm(
                    _watchVarControl.VarName,
                    _watchVar.GetTypeDescription(),
                    _watchVar.GetBaseOffsetDescription(),
                    _watchVar.GetRamAddressString(true, _watchVarControl.FixedAddressList),
                    _watchVar.GetProcessAddressString(_watchVarControl.FixedAddressList));
            varInfo.Show();
        }

        public List<string> GetVarInfo()
        {
            return new List<string>()
            {
                _watchVarControl.VarName,
                _watchVar.GetTypeDescription(),
                _watchVar.GetBaseOffsetDescription(),
                _watchVar.GetRamAddressString(true, _watchVarControl.FixedAddressList),
                _watchVar.GetProcessAddressString(_watchVarControl.FixedAddressList),
            };
        }

        public static List<string> GetVarInfoLabels()
        {
            return new List<string>()
            {
                "Name",
                "Type",
                "Base + Offset",
                "N64 Address",
                "Emulator Address",
            };
        }

        public void ShowVarController()
        {
            VariableControllerForm varController =
                new VariableControllerForm(
                    _watchVarControl.VarName,
                    this,
                    _watchVarControl.FixedAddressList);
            varController.Show();
        }

        public CheckState GetLockedCheckState(List<uint> addresses = null)
        {
            return WatchVariableLockManager.ContainsLocksCheckState(_watchVar, addresses);
        }

        public bool GetLockedBool(List<uint> addresses = null)
        {
            return WatchVariableLockManager.ContainsLocksBool(_watchVar, addresses);
        }

        public void UpdateItemCheckStates(List<uint> addresses = null)
        {
            _itemLock.Checked = GetLockedBool(addresses);
            _itemRemoveAllLocks.Visible = WatchVariableLockManager.ContainsAnyLocks();
            _itemFixAddress.Checked = _watchVarControl.FixedAddressList != null;
        }

        public void ToggleHighlighted()
        {
            _watchVarControl.ShowBorder = !_watchVarControl.ShowBorder;
            _itemHighlight.Checked = _watchVarControl.ShowBorder;
        }

        public void ToggleLocked(List<uint> addresses = null)
        {
            if (WatchVariableLockManager.ContainsLocksBool(_watchVar, addresses))
            {
                WatchVariableLockManager.RemoveLocks(_watchVar, addresses);
            }
            else
            {
                WatchVariableLockManager.AddLocks(_watchVar, addresses);
            }
        }




        public string GetValue(
            bool handleRounding = true,
            bool handleFormatting = true,
            List<uint> addresses = null)
        {
            List<string> values = _watchVar.GetValues(addresses);
            (bool meaningfulValue, string value) = CombineValues(values);
            if (!meaningfulValue) return value;

            value = ConvertValue(value, handleRounding, handleFormatting);
            return value;
        }

        private string ConvertValue(
            string value,
            bool handleRounding = true,
            bool handleFormatting = true)
        {
            value = HandleAngleConverting(value);
            if (handleRounding) value = HandleRounding(value);
            value = HandleAngleRoundingOut(value);
            value = HandleNegating(value);
            if (handleFormatting) value = HandleHexDisplaying(value);
            if (handleFormatting) value = HandleObjectDisplaying(value);
            return value;
        }

        public bool SetValue(string value, List<uint> addresses = null)
        {
            value = UnconvertValue(value);
            bool success = _watchVar.SetValue(value, addresses);
            if (success && GetLockedBool(addresses)) WatchVariableLockManager.UpdateLockValues(_watchVar, value, addresses);
            return success;
        }

        public string UnconvertValue(string value)
        {
            value = HandleObjectUndisplaying(value);
            value = HandleHexUndisplaying(value);
            value = HandleUnnegating(value);
            value = HandleAngleUnconverting(value);
            return value;
        }

        public CheckState GetCheckStateValue(List<uint> addresses = null)
        {
            List<string> values = _watchVar.GetValues(addresses);
            List<CheckState> checkStates = values.ConvertAll(value => ConvertValueToCheckState(value));
            CheckState checkState = CombineCheckStates(checkStates);
            return checkState;
        }

        public bool SetCheckStateValue(CheckState checkState, List<uint> addresses = null)
        {
            string value = ConvertCheckStateToValue(checkState);
            bool success = _watchVar.SetValue(value, addresses);
            if (success && GetLockedBool(addresses))
                WatchVariableLockManager.UpdateLockValues(_watchVar, value, addresses);
            return success;
        }

        public bool AddValue(string stringValue, bool add, List<uint> addresses = null)
        {
            double? changeValueNullable = ParsingUtilities.ParseDoubleNullable(stringValue);
            if (!changeValueNullable.HasValue) return false;
            double changeValue = changeValueNullable.Value;

            List<string> currentValuesString = _watchVar.GetValues(addresses);
            List<double?> currentValuesDoubleNullable =
                currentValuesString.ConvertAll(
                    currentStringValue => ParsingUtilities.ParseDoubleNullable(currentStringValue));
            List<string> newValuesString = currentValuesDoubleNullable.ConvertAll(currentValueDoubleNullable =>
            {
                if (!currentValueDoubleNullable.HasValue) return null;
                double currentValueDouble = currentValueDoubleNullable.Value;
                string currentValueString = currentValueDouble.ToString();
                string convertedValueString = ConvertValue(currentValueDouble.ToString(), false, false);
                double convertedValueDouble = ParsingUtilities.ParseDouble(convertedValueString);
                double modifiedValueDouble = convertedValueDouble + changeValue * (add ? +1 : -1);
                string modifiedValueString = modifiedValueDouble.ToString();
                string unconvertedValueString = UnconvertValue(modifiedValueString);
                return unconvertedValueString;
            });

            bool success = _watchVar.SetValues(newValuesString, addresses);
            if (success && GetLockedBool(addresses))
                WatchVariableLockManager.UpdateLockValues(_watchVar, newValuesString, addresses);
            return success;
        }

        public List<uint> GetCurrentAddresses()
        {
            return _watchVar.AddressList;
        }

        public void EnableCustomFunctionality()
        {
            _separatorCustom.Visible = true;
            _itemFixAddress.Visible = true;
            _itemRename.Visible = true;
            _itemDelete.Visible = true;
        }

        public void AddToVarHackTab(List<uint> addresses = null)
        {
            if (_watchVar.BaseAddressType == BaseAddressTypeEnum.Triangle)
            {
                Config.VarHackManager.AddVariable(
                    _watchVarControl.VarName + " ",
                    Config.TriangleManager.TrianglePointerAddress,
                    _watchVar.MemoryType,
                    GetUseHex(),
                    _watchVar.Offset);
            }
            else
            {
                List<uint> addressList = addresses ?? _watchVar.AddressList;
                foreach (uint address in addressList)
                {
                    Config.VarHackManager.AddVariable(
                        _watchVarControl.VarName + " ",
                        address,
                        _watchVar.MemoryType,
                        GetUseHex(),
                        null);
                }
            }
        }





        protected (bool meaningfulValue, string stringValue) CombineValues(List<string> values)
        {
            if (values.Count == 0) return (false, "(none)");
            string firstValue = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] != firstValue) return (false, "(multiple values)");
            }
            return (true, firstValue);
        }

        protected CheckState CombineCheckStates(List<CheckState> checkStates)
        {
            if (checkStates.Count == 0) return CheckState.Unchecked;
            CheckState firstCheckState = checkStates[0];
            for (int i = 1; i < checkStates.Count; i++)
            {
                if (checkStates[i] != firstCheckState) return CheckState.Indeterminate;
            }
            return firstCheckState;
        }



        // Number methods

        protected virtual string HandleRounding(string value)
        {
            return value;
        }

        protected virtual string HandleNegating(string value)
        {
            return value;
        }

        protected virtual string HandleUnnegating(string value)
        {
            return value;
        }

        protected virtual string HandleHexDisplaying(string value)
        {
            return value;
        }

        protected virtual string HandleHexUndisplaying(string value)
        {
            return value;
        }

        // Angle methods

        protected virtual string HandleAngleConverting(string value)
        {
            return value;
        }

        protected virtual string HandleAngleUnconverting(string value)
        {
            return value;
        }

        protected virtual string HandleAngleRoundingOut(string value)
        {
            return value;
        }

        // Object methods

        protected virtual string HandleObjectDisplaying(string value)
        {
            return value;
        }

        protected virtual string HandleObjectUndisplaying(string value)
        {
            return value;
        }

        // Boolean methods

        protected virtual CheckState ConvertValueToCheckState(string value)
        {
            return CheckState.Unchecked;
        }

        protected virtual string ConvertCheckStateToValue(CheckState checkState)
        {
            return "";
        }



        // Getter methods

        protected virtual bool GetUseHex()
        {
            return false;
        }
    }
}