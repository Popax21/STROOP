﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using STROOP.Controls;

namespace STROOP.Structs
{
    public class MapGui
    {
        public GLControl GLControl;
        public Label QpuValueLabel;
        public Label PuValueLabel;
        public Label MapIdLabel;
        public Label MapNameLabel;
        public Label MapSubNameLabel;
        public TrackBar MapIconSizeTrackbar;
        public TrackBar MapZoomTrackbar;
        public CheckBox MapShowMario;
        public CheckBox MapShowInactiveObjects;
        public CheckBox MapShowHolp;
        public CheckBox MapShowIntendedNextPosition;
        public CheckBox MapShowCamera;
        public CheckBox MapShowFloorTriangle;
        public CheckBox MapShowCeilingTriangle;

        public Button MapBoundsUpButton;
        public Button MapBoundsDownButton;
        public Button MapBoundsLeftButton;
        public Button MapBoundsRightButton;
        public Button MapBoundsUpLeftButton;
        public Button MapBoundsUpRightButton;
        public Button MapBoundsDownLeftButton;
        public Button MapBoundsDownRightButton;
        public BetterTextbox MapBoundsPositionTextBox;

        public Button MapBoundsZoomInButton;
        public Button MapBoundsZoomOutButton;
        public BetterTextbox MapBoundsZoomTextBox;

        public BetterTextbox MapArtificialMarioYLabelTextBox;

        public RadioButton RadioButtonScaleCourseDefault;
        public RadioButton RadioButtonScaleMaxCourseSize;
        public RadioButton RadioButtonScaleCustom;
        public BetterTextbox TextBoxScaleCustom;

        public BetterTextbox TextBoxScaleChange;
        public Button ButtonCenterScaleChangeMinus;
        public Button ButtonCenterScaleChangePlus;

        public RadioButton RadioButtonCenterBestFit;
        public RadioButton RadioButtonCenterOrigin;
        public RadioButton RadioButtonCenterCustom;
        public BetterTextbox TextBoxCenterCustom;

        public BetterTextbox TextBoxCenterChange;
        public Button ButtonCenterChangeUp;
        public Button ButtonCenterChangeDown;
        public Button ButtonCenterChangeLeft;
        public Button ButtonCenterChangeRight;
        public Button ButtonCenterChangeUpLeft;
        public Button ButtonCenterChangeUpRight;
        public Button ButtonCenterChangeDownLeft;
        public Button ButtonCenterChangeDownRight;

        public RadioButton RadioButtonAngle0;
        public RadioButton RadioButtonAngle16384;
        public RadioButton RadioButtonAngle32768;
        public RadioButton RadioButtonAngle49152;
        public RadioButton RadioButtonAngleCustom;
        public BetterTextbox TextBoxAngleCustom;

        public BetterTextbox TextBoxAngleChange;
        public Button ButtonAngleChangeCounterclockwise;
        public Button ButtonAngleChangeClockwise;

        public MapTrackerFlowLayoutPanel MapTrackerFlowLayoutPanel;
        public Button ButtonAdd;
        public Button ButtonClear;
    }
}
