using System;
using MVCCore.Interfaces;
using VoxelMenu.ViewModel;
using VoxelUtils.Enums;

namespace VoxelMenu.Model
{
    public class MenuModel : IModel
    {
        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        public void Update(float absoluteTime, ModelInput input)
        {
            //TODO implement
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
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
