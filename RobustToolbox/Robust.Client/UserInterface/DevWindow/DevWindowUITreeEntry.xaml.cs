﻿using System.Linq;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Client.UserInterface
{
    [GenerateTypedNameReferences]
    public sealed partial class DevWindowUITreeEntry : Control
    {
        private readonly DevWindowTabUI _tab;
        public readonly Control VisControl;

        public DevWindowUITreeEntry(DevWindowTabUI tab, Control visControl)
        {
            _tab = tab;
            VisControl = visControl;

            InitializeComponent();

            var typeName = visControl.GetType().Name;
            ControlName.Text = visControl.Name == null ? typeName : $"{visControl.Name} ({typeName})";

            _tab.EntryAdded(this);
        }

        private void InitializeComponent()
        {
            RobustXamlLoader.Load(this);

            ExpandButton.OnToggled += ExpandButtonOnOnToggled;
            Header.OnKeyBindDown += HeaderOnOnKeyBindDown;
        }

        private void HeaderOnOnKeyBindDown(GUIBoundKeyEventArgs obj)
        {
            if (obj.Function != EngineKeyFunctions.UIClick)
                return;

            obj.Handle();
            _tab.SelectControl(VisControl);
        }

        protected override void EnteredTree()
        {
            base.EnteredTree();

            VisControl.OnChildAdded += VisControlOnChildAdded;
            VisControl.OnChildRemoved += VisControlOnChildRemoved;
            VisControl.OnChildMoved += VisControlOnChildMoved;
            _tab.SelectedControlChanged += TabOnSelectedControlChanged;
        }

        protected override void ExitedTree()
        {
            base.ExitedTree();

            VisControl.OnChildAdded -= VisControlOnChildAdded;
            VisControl.OnChildRemoved -= VisControlOnChildRemoved;
            VisControl.OnChildMoved -= VisControlOnChildMoved;
            _tab.SelectedControlChanged -= TabOnSelectedControlChanged;

            _tab.EntryRemoved(this);
        }

        private void TabOnSelectedControlChanged()
        {
            var isThis = VisControl == _tab.SelectedControl;

            Header.PanelOverride = isThis ? new StyleBoxFlat { BackgroundColor = Color.Blue } : null;
        }

        private void VisControlOnChildAdded(Control child)
        {
            if (!ExpandButton.Pressed)
                return;

            ChildEntryContainer.AddChild(new DevWindowUITreeEntry(_tab, child));
        }

        private void VisControlOnChildRemoved(Control child)
        {
            if (!ExpandButton.Pressed)
                return;

            var entry = ChildEntryContainer.Children.OfType<DevWindowUITreeEntry>().Single(c => c.VisControl == child);
            ChildEntryContainer.RemoveChild(entry);
        }

        private void VisControlOnChildMoved(ControlChildMovedEventArgs eventArgs)
        {
            if (!ExpandButton.Pressed)
                return;

            var entry = ChildEntryContainer.Children
                .OfType<DevWindowUITreeEntry>()
                .Single(c => c.VisControl == eventArgs.Control);
            entry.SetPositionInParent(eventArgs.NewIndex);
        }

        internal void Open()
        {
            DebugTools.Assert(ChildEntryContainer.ChildCount == 0);

            ExpandButton.Pressed = true;

            foreach (var child in VisControl.Children)
            {
                ChildEntryContainer.AddChild(new DevWindowUITreeEntry(_tab, child));
            }
        }

        private void ExpandButtonOnOnToggled(BaseButton.ButtonToggledEventArgs obj)
        {
            if (obj.Pressed)
            {
                Open();
            }
            else
            {
                ChildEntryContainer.RemoveAllChildren();
            }
        }
    }
}
