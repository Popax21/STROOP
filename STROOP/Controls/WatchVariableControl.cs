﻿using STROOP.Extensions;
using STROOP.Managers;
using STROOP.Structs;
using STROOP.Structs.Configurations;
using STROOP.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;

namespace STROOP.Controls
{
    public class WatchVariableControl : TableLayoutPanel
    {
        // Main objects
        private readonly WatchVariableControlPrecursor _watchVarPrecursor;
        private readonly WatchVariableWrapper _watchVarWrapper;
        public readonly List<VariableGroup> GroupList;

        // Sub controls
        private readonly Panel _namePanel;
        private readonly TextBox _nameTextBox;
        private readonly PictureBox _lockPictureBox;
        private readonly PictureBox _pinPictureBox;
        private readonly TextBox _valueTextBox;
        private readonly CheckBox _valueCheckBox;
        private readonly ContextMenuStrip _valueTextboxOriginalContextMenuStrip;
        private readonly ContextMenuStrip _nameTextboxOriginalContextMenuStrip;

        // Parent control
        private WatchVariableFlowLayoutPanel _watchVariablePanel;

        public string TextBoxValue
        {
            get { return _valueTextBox.Text; }
            set { _valueTextBox.Text = value; }
        }

        public CheckState CheckBoxValue
        {
            get { return _valueCheckBox.CheckState; }
            set { _valueCheckBox.CheckState = value; }
        }

        private static readonly Pen _borderPen = new Pen(Color.Red, 5);

        public static readonly Color DEFAULT_COLOR = SystemColors.Control;
        public static readonly Color FAILURE_COLOR = Color.Red;
        public static readonly Color ENABLE_CUSTOM_FUNCIONALITY_COLOR = Color.Yellow;
        public static readonly Color ADD_TO_CUSTOM_TAB_COLOR = Color.CornflowerBlue;
        public static readonly Color REORDER_START_COLOR = Color.DarkGreen;
        public static readonly Color REORDER_END_COLOR = Color.LightGreen;
        public static readonly Color REORDER_RESET_COLOR = Color.Black;
        public static readonly Color ADD_TO_VAR_HACK_TAB_COLOR = Color.SandyBrown;
        private static readonly int FLASH_DURATION_MS = 1000;

        private Color _baseColor;
        public Color BaseColor
        {
            get { return _baseColor; }
            set { _baseColor = value; _currentColor = value; }
        }

        private Color _currentColor;
        private bool _isFlashing;
        private DateTime _flashStartTime;
        private Color _flashColor;

        private string _varName;
        public string VarName
        {
            get
            {
                return _varName;
            }
        }

        private bool _showBorder;
        public bool ShowBorder
        {
            get
            {
                return _showBorder;
            }
            set
            {
                if (_showBorder == value)
                    return;

                _showBorder = value;
                Invalidate();
            }
        }

        private bool _editMode;
        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                _valueTextBox.ReadOnly = !_editMode;
                _valueTextBox.BackColor = _editMode ? Color.White : _currentColor;
                _valueTextBox.ContextMenuStrip = _editMode ? _valueTextboxOriginalContextMenuStrip : ContextMenuStrip;
                if (_editMode)
                {
                    _valueTextBox.Focus();
                    _valueTextBox.SelectAll();
                }
            }
        }

        private bool _renameMode;
        public bool RenameMode
        {
            get
            {
                return _renameMode;
            }
            set
            {
                _renameMode = value;
                _nameTextBox.ReadOnly = !_renameMode;
                _nameTextBox.BackColor = _renameMode ? Color.White : _currentColor;
                _nameTextBox.ContextMenuStrip = _renameMode ? _nameTextboxOriginalContextMenuStrip : ContextMenuStrip;
                if (_renameMode)
                {
                    _nameTextBox.Focus();
                    _nameTextBox.SelectAll();
                }
            }
        }

        public List<uint> FixedAddressList;

        private static readonly Image _lockedImage = Properties.Resources.img_lock;
        private static readonly Image _someLockedImage = Properties.Resources.img_lock_grey;
        private static readonly Image _pinnedImage = Properties.Resources.img_pin;

        private static readonly int LOCK_PADDING = 16;
        private static readonly int PIN_OUTER_PADDING = 11;
        private static readonly int PIN_INNER_PADDING = 24;

        public static readonly int DEFAULT_VARIABLE_NAME_WIDTH = 120;
        public static readonly int DEFAULT_VARIABLE_VALUE_WIDTH = 85;
        public static readonly int DEFAULT_VARIABLE_HEIGHT = 20;

        public static int VariableNameWidth = DEFAULT_VARIABLE_NAME_WIDTH;
        public static int VariableValueWidth = DEFAULT_VARIABLE_VALUE_WIDTH;
        public static int VariableHeight = DEFAULT_VARIABLE_HEIGHT;

        private int _variableNameWidth;
        private int _variableValueWidth;
        private int _variableHeight;

        public WatchVariableControl(
            WatchVariableControlPrecursor watchVarPrecursor,
            string name,
            WatchVariable watchVar,
            WatchVariableSubclass subclass,
            Color? backgroundColor,
            bool? useHex,
            bool? invertBool,
            WatchVariableCoordinate? coordinate,
            List<VariableGroup> groupList,
            List<uint> fixedAddresses)
        {
            // Store the precursor
            _watchVarPrecursor = watchVarPrecursor;

            // Initialize main fields
            _varName = name;
            GroupList = groupList;
            _showBorder = false;
            _editMode = false;
            _renameMode = false;
            FixedAddressList = fixedAddresses;

            // Initialize color fields
            _baseColor = backgroundColor ?? DEFAULT_COLOR;
            _currentColor = _baseColor;
            _isFlashing = false;
            _flashStartTime = DateTime.Now;

            // Initialize size fields
            _variableNameWidth = VariableNameWidth;
            _variableValueWidth = VariableValueWidth;
            _variableHeight = VariableHeight;

            // Create controls
            InitializeBase();
            _namePanel = CreateNamePanel();
            _nameTextBox = CreateNameTextBox();
            _lockPictureBox = CreateLockPictureBox();
            _pinPictureBox = CreatePinPictureBox();
            _valueTextBox = CreateValueTextBox();
            _valueCheckBox = CreateValueCheckBox();

            // Add controls to their containers
            base.Controls.Add(_valueTextBox, 1, 0);
            base.Controls.Add(_valueCheckBox, 1, 0);
            base.Controls.Add(_namePanel, 0, 0);
            _namePanel.Controls.Add(_pinPictureBox);
            _namePanel.Controls.Add(_lockPictureBox);
            _namePanel.Controls.Add(_nameTextBox);

            // Create var x
            _watchVarWrapper = WatchVariableWrapper.CreateWatchVariableWrapper(
                watchVar, this, subclass, useHex, invertBool, coordinate);

            // Initialize context menu strip
            _valueTextboxOriginalContextMenuStrip = _valueTextBox.ContextMenuStrip;
            _nameTextboxOriginalContextMenuStrip = _nameTextBox.ContextMenuStrip;
            ContextMenuStrip = _watchVarWrapper.GetContextMenuStrip();
            _nameTextBox.ContextMenuStrip = ContextMenuStrip;
            _valueTextBox.ContextMenuStrip = ContextMenuStrip;

            // Set whether to start as a checkbox
            SetUseCheckbox(_watchVarWrapper.StartsAsCheckbox());

            // Add functions
            _namePanel.Click += (sender, e) => OnNameTextBoxClick();
            _nameTextBox.Click += (sender, e) => OnNameTextBoxClick();
            _nameTextBox.Leave += (sender, e) => { RenameMode = false; };
            _nameTextBox.KeyDown += (sender, e) => OnNameTextValueKeyDown(e);
            _valueTextBox.DoubleClick += (sender, e) => { EditMode = true; };
            _valueTextBox.KeyDown += (sender, e) => OnValueTextValueKeyDown(e);
            _valueTextBox.Leave += (sender, e) => { EditMode = false; };
            _valueCheckBox.Click += (sender, e) => OnCheckboxClick();
        }

        private void InitializeBase()
        {
            base.Size = new Size(_variableNameWidth + _variableValueWidth, _variableHeight + 2);
            base.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            base.RowCount = 1;
            base.ColumnCount = 2;
            base.RowStyles.Clear();
            base.RowStyles.Add(new RowStyle(SizeType.Absolute, _variableHeight));
            base.ColumnStyles.Clear();
            base.Margin = new Padding(0);
            base.Padding = new Padding(0);
            base.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, _variableNameWidth));
            base.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, _variableValueWidth));
            base.BackColor = _currentColor;
        }

        private Panel CreateNamePanel()
        {
            Panel namePanel = new Panel();
            namePanel.Dock = DockStyle.Fill;
            namePanel.Margin = new Padding(0, 0, 0, 0);
            namePanel.BackColor = Color.Transparent;
            return namePanel;
        }

        private TextBox CreateNameTextBox()
        {
            TextBox nameTextBox = new TextBox();
            nameTextBox.Text = VarName;
            nameTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            nameTextBox.ReadOnly = true;
            nameTextBox.BorderStyle = BorderStyle.None;
            nameTextBox.TextAlign = HorizontalAlignment.Left;
            nameTextBox.Anchor = AnchorStyles.Left;
            nameTextBox.Size = new Size(200, 20);
            nameTextBox.Location = new Point(4, _variableHeight / 2 - 7);
            return nameTextBox;
        }

        private PictureBox CreateLockPictureBox()
        {
            PictureBox lockPictureBox = new PictureBox();
            lockPictureBox.Image = Properties.Resources.img_lock;
            lockPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            lockPictureBox.Size = new Size(16, 18);
            lockPictureBox.Margin = new Padding(0, 0, 0, 0);
            lockPictureBox.Anchor = AnchorStyles.Right;
            lockPictureBox.Location =
                new Point(_variableNameWidth - LOCK_PADDING, _variableHeight / 2 - 9);
            lockPictureBox.Visible = false;
            return lockPictureBox;
        }

        private PictureBox CreatePinPictureBox()
        {
            PictureBox lockPictureBox = new PictureBox();
            lockPictureBox.Image = Properties.Resources.img_pin;
            lockPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            lockPictureBox.Size = new Size(10, 17);
            lockPictureBox.Margin = new Padding(0, 0, 0, 0);
            lockPictureBox.Anchor = AnchorStyles.Right;
            lockPictureBox.Location =
                new Point(_variableNameWidth - PIN_OUTER_PADDING, _variableHeight / 2 - 8);
            lockPictureBox.Visible = false;
            return lockPictureBox;
        }

        private TextBox CreateValueTextBox()
        {
            TextBox nameTextBox = new TextBox();
            nameTextBox.ReadOnly = true;
            nameTextBox.BorderStyle = BorderStyle.None;
            nameTextBox.TextAlign = HorizontalAlignment.Right;
            nameTextBox.Margin = new Padding(0, 0, 6, 0);
            nameTextBox.Anchor = AnchorStyles.Right;
            nameTextBox.Size = new Size(200, 20);
            return nameTextBox;
        }

        private CheckBox CreateValueCheckBox()
        {
            CheckBox valueCheckBox = new CheckBox();
            valueCheckBox.CheckState = CheckState.Unchecked;
            valueCheckBox.BackColor = Color.Transparent;
            valueCheckBox.AutoSize = true;
            valueCheckBox.CheckAlign = ContentAlignment.MiddleRight;
            valueCheckBox.Dock = DockStyle.Fill;
            valueCheckBox.Margin = new Padding(0, 0, 5, 0);
            return valueCheckBox;
        }

        public void SetUseCheckbox(bool useCheckbox)
        {
            if (useCheckbox)
            {
                _valueTextBox.Visible = false;
                _valueCheckBox.Visible = true;
            }
            else
            {
                _valueTextBox.Visible = true;
                _valueCheckBox.Visible = false;
            }
        }

        private void OnValueTextValueKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (_editMode)
            {
                if (e.KeyData == Keys.Escape)
                {
                    EditMode = false;
                    this.Focus();
                    return;
                }

                if (e.KeyData == Keys.Enter)
                {
                    EditMode = false;
                    SetValue(_valueTextBox.Text);
                    this.Focus();
                    return;
                }
            }
        }

        private void OnNameTextBoxClick()
        {
            this.Focus();

            bool isCtrlKeyHeld = ModifierKeys == Keys.Control;
            bool isShiftKeyHeld = ModifierKeys == Keys.Shift;
            bool isAltKeyHeld = ModifierKeys == Keys.Alt;
            bool isFKeyHeld = Keyboard.IsKeyDown(Key.F);
            bool isAKeyHeld = Keyboard.IsKeyDown(Key.A);
            bool isHKeyHeld =  Keyboard.IsKeyDown(Key.H);
            bool isLKeyHeld =  Keyboard.IsKeyDown(Key.L);
            bool isRKeyHeld =  Keyboard.IsKeyDown(Key.R);
            bool isDeleteKeyHeld =
                 Keyboard.IsKeyDown(Key.Delete) ||
                 Keyboard.IsKeyDown(Key.Back) ||
                 Keyboard.IsKeyDown(Key.Escape);
            bool isBacktickHeld =  Keyboard.IsKeyDown(Key.OemTilde);
            bool isZHeld = Keyboard.IsKeyDown(Key.Z);
            bool isMinusHeld = Keyboard.IsKeyDown(Key.OemMinus);
            bool isPlusHeld = Keyboard.IsKeyDown(Key.OemPlus);

            if (isCtrlKeyHeld && isFKeyHeld)
            {
                AddToCustomTab(true, false);
                return;
            }

            if (isFKeyHeld)
            {
                ToggleFixedAddress();
                return;
            }

            if (isHKeyHeld)
            {
                _watchVarWrapper.ToggleHighlighted();
                return;
            }

            if (isLKeyHeld)
            {
                _watchVarWrapper.ToggleLocked(FixedAddressList);
                return;
            }

            if (isRKeyHeld)
            {
                RenameMode = true;
                return;
            }

            if (isDeleteKeyHeld)
            {
                DeleteFromPanel();
                return;
            }

            if (isCtrlKeyHeld && isAKeyHeld)
            {
                AddToCustomTab(true, true);
                return;
            }

            if (isCtrlKeyHeld)
            {
                AddToCustomTab(false, false);
                return;
            }

            if (isShiftKeyHeld)
            {
                NotifyPanelOfReodering();
                return;
            }

            if (isAltKeyHeld)
            {
                EnableCustomFunctionality();
                return;
            }

            if (isBacktickHeld)
            {
                AddToVarHackTab();
                return;
            }

            if (isZHeld)
            {
                SetValue("0");
                return;
            }

            if (isMinusHeld)
            {
                AddValue("1", false);
                return;
            }

            if (isPlusHeld)
            {
                AddValue("1", true);
                return;
            }

            // default
            {
                _watchVarWrapper.ShowVarInfo();
                return;
            }
        }

        private void OnNameTextValueKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (_renameMode)
            {
                if (e.KeyData == Keys.Escape)
                {
                    RenameMode = false;
                    _nameTextBox.Text = VarName;
                    this.Focus();
                    return;
                }

                if (e.KeyData == Keys.Enter)
                {
                    _varName = _nameTextBox.Text;
                    RenameMode = false;
                    this.Focus();
                    return;
                }
            }
        }

        private void OnCheckboxClick()
        {
            bool success = _watchVarWrapper.SetCheckStateValue(_valueCheckBox.CheckState, FixedAddressList);
            if (!success) FlashColor(FAILURE_COLOR);
        }

        public void UpdateControl()
        {
            if (!EditMode)
            {
                if (_valueTextBox.Visible) _valueTextBox.Text = _watchVarWrapper.GetValue(true, true, FixedAddressList);
                if (_valueCheckBox.Visible) _valueCheckBox.CheckState = _watchVarWrapper.GetCheckStateValue(FixedAddressList);
            }

            _watchVarWrapper.UpdateItemCheckStates();

            UpdateSize();
            UpdateColor();
            UpdatePictureBoxes();
        }

        private void UpdatePictureBoxes()
        {
            Image currentLockImage = GetImageForCheckState(_watchVarWrapper.GetLockedCheckState(FixedAddressList));
            bool isLocked = currentLockImage != null;
            bool isFixedAddress = FixedAddressList != null;

            if (_lockPictureBox.Image == currentLockImage &&
                _lockPictureBox.Visible == isLocked &&
                _pinPictureBox.Visible == isFixedAddress) return;

            _lockPictureBox.Image = currentLockImage;
            _lockPictureBox.Visible = isLocked;
            _pinPictureBox.Visible = isFixedAddress;

            int pinPadding = isLocked ? PIN_INNER_PADDING : PIN_OUTER_PADDING;
            _pinPictureBox.Location =
                new Point(
                    _variableNameWidth - pinPadding,
                    _pinPictureBox.Location.Y);
        }

        private static Image GetImageForCheckState(CheckState checkState)
        {
            switch (checkState)
            {
                case CheckState.Unchecked:
                    return null;
                case CheckState.Checked:
                    return _lockedImage;
                case CheckState.Indeterminate:
                    return _someLockedImage;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateSize()
        {
            if (_variableNameWidth == VariableNameWidth &&
                _variableValueWidth == VariableValueWidth &&
                _variableHeight == VariableHeight)
                return;

            _variableNameWidth = VariableNameWidth;
            _variableValueWidth = VariableValueWidth;
            _variableHeight = VariableHeight;

            Size = new Size(_variableNameWidth + _variableValueWidth, _variableHeight + 2);
            RowStyles[0].Height = _variableHeight;
            ColumnStyles[0].Width = _variableNameWidth;
            ColumnStyles[1].Width = _variableValueWidth;
        }

        private void UpdateColor()
        {
            if (_isFlashing)
            {
                DateTime currentTime = DateTime.Now;
                double timeSinceFlashStart = currentTime.Subtract(_flashStartTime).TotalMilliseconds;
                if (timeSinceFlashStart < FLASH_DURATION_MS)
                {
                    _currentColor = ColorUtilities.InterpolateColor(
                        _flashColor, _baseColor, timeSinceFlashStart / FLASH_DURATION_MS);
                }
                else
                {
                    _currentColor = _baseColor;
                    _isFlashing = false;
                }
            }

            BackColor = _currentColor;
            if (!_editMode) _valueTextBox.BackColor = _currentColor;
            if (!_renameMode) _nameTextBox.BackColor = _currentColor;
        }

        public void FlashColor(Color color)
        {
            _flashStartTime = DateTime.Now;
            _flashColor = color;
            _isFlashing = true;
        }





        public bool BelongsToGroup(VariableGroup variableGroup)
        {
            return GroupList.Contains(variableGroup);
        }

        public bool BelongsToAnyGroup(List<VariableGroup> variableGroups)
        {
            return variableGroups.Any(varGroup => BelongsToGroup(varGroup));
        }




        public void SetPanel(WatchVariableFlowLayoutPanel panel)
        {
            _watchVariablePanel = panel;
        }

        public void DeleteFromPanel()
        {
            if (_watchVariablePanel == null) return;
            _watchVariablePanel.RemoveVariable(this);
        }

        public void OpenPanelOptions(Point point)
        {
            if (_watchVariablePanel == null) return;
            _watchVariablePanel.ContextMenuStrip.Show(point);
        }

        public void AddToCustomTab(bool useFixedAddress, bool useIndidualAddresses)
        {
            List<uint> addressList = _watchVarWrapper.GetCurrentAddresses();
            List<List<uint>> addressesLists =
                useIndidualAddresses ?
                    addressList.ConvertAll(address => new List<uint>() { address }) :
                    new List<List<uint>>() { addressList };
            for (int i = 0; i < addressesLists.Count; i++)
            {
                string name = VarName;
                if (useIndidualAddresses && addressesLists.Count > 1) name += " " + (i + 1);
                List<uint> constructorAddressList = useFixedAddress ? addressesLists[i] : null;
                WatchVariableControl newControl =
                    _watchVarPrecursor.CreateWatchVariableControl(
                        null, name, constructorAddressList);
                Config.CustomManager.AddVariable(newControl);
            }
            FlashColor(ADD_TO_CUSTOM_TAB_COLOR);
        }

        public void AddToVarHackTab()
        {
            _watchVarWrapper.AddToVarHackTab(FixedAddressList);
            FlashColor(ADD_TO_VAR_HACK_TAB_COLOR);
        }

        public void EnableCustomFunctionality()
        {
            _watchVarWrapper.EnableCustomFunctionality();
            FlashColor(ENABLE_CUSTOM_FUNCIONALITY_COLOR);
        }

        public void NotifyPanelOfReodering()
        {
            _watchVariablePanel.NotifyOfReordering(this);
        }

        public void ToggleFixedAddress()
        {
            if (FixedAddressList == null)
            {
                FixedAddressList = _watchVarWrapper.GetCurrentAddresses();
            }
            else
            {
                FixedAddressList = null;
            }
        }

        public string GetValue(bool useRounding)
        {
            return _watchVarWrapper.GetValue(useRounding);
        }

        public void SetValue(string value)
        {
            bool success = _watchVarWrapper.SetValue(value, FixedAddressList);
            if (!success) FlashColor(FAILURE_COLOR);
        }

        public void AddValue(string value, bool add)
        {
            bool success = _watchVarWrapper.AddValue(value, add, FixedAddressList);
            if (!success) FlashColor(FAILURE_COLOR);
        }

        public XElement ToXml(bool useCurrentState = true)
        {
            Color? color = _baseColor == DEFAULT_COLOR ? (Color?)null : _baseColor;
            if (useCurrentState)
                return _watchVarPrecursor.ToXML(color, VarName, FixedAddressList);
            else
                return _watchVarPrecursor.ToXML();
        }

        public List<string> GetVarInfo()
        {
            return _watchVarWrapper.GetVarInfo();
        }




        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var rec = DisplayRectangle;
            rec.Width -= 1;
            rec.Height -= 1;
            if (_showBorder)
                e.Graphics.DrawRectangle(_borderPen, rec);
        }

        public override string ToString()
        {
            return _watchVarPrecursor.ToString();
        }
    }
}