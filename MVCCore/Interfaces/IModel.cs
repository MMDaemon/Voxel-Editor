using System;
using OpenTK;

namespace MVCCore.Interfaces
{
	public delegate void StateChangedHandler(int state, bool temporary);
    public delegate void GameWindowEventHandler(Action<GameWindow> gameWindowCall);

	public interface IModel
	{
		IViewModel ViewModel { get; }

		void Update(float absoluteTime, ModelInput input);
	    void Resize(int width, int height);

        event EventHandler ModelEvent;
		event StateChangedHandler StateChanged;
	    event GameWindowEventHandler GameWindowEvent;
	}
}
