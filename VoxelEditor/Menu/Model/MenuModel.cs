using System;
using System.Numerics;
using VoxelEditor.Common.Enums;
using VoxelEditor.Editor.ViewModel;
using VoxelEditor.Menu.Modelview;
using VoxelEditor.MVCInterfaces;

namespace VoxelEditor.Menu.Model
{
    internal class MenuModel : IModel
    {
        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        public void Update(float absoluteTime, ModelInput input)
        {
            //TODO implement
        }
        private MenuViewModel CreateViewModel()
        {
            return new MenuViewModel();
            //TODO implement
        }



        private void OnModelEvent()
        {
            ModelEvent?.Invoke(null, null);
        }

        private void OnStateChanged(State state, bool temporary)
        {
            StateChanged?.Invoke((int)state, temporary);
        }
    }
}
