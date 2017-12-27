using System;
using System.Collections.Generic;
using VoxelUtils.Enums;

namespace VoxelEditor.ViewModel
{
    internal class GuiKeyActionsEventArgs : EventArgs
    {
        public ICollection<KeyAction> KeyActions { get; private set; }

        public GuiKeyActionsEventArgs(ICollection<KeyAction> keyActions)
        {
            KeyActions = keyActions;
        }
    }
}
