﻿using System;
using System.Collections.Generic;
using System.IO;
using MVCCore.Interfaces;
using OpenTK;
using VoxelEditor.ViewModel;
using VoxelUtils.Enums;
using VoxelUtils.Registry.View;
using Zenseless.Application;
using Zenseless.Base;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace VoxelEditor.View
{
    public class EditorView : ViewRegistryContainer, IView
    {
        private readonly ViewRegistry _registry;
        private readonly EditorVisual _visual;
        private readonly EditorSound _sound;

        private bool _cursorVisible;

        public event GameWindowEventHandler GameWindowEvent;

        public EditorView(IViewRegistry registry) : base(registry)
        {
            _registry = (ViewRegistry)registry;
            _visual = new EditorVisual(_registry);
            _sound = new EditorSound();

            _cursorVisible = false;
        }

        public void ProcessModelEvent(EventArgs e)
        {
            if (e is GuiKeyActionsEventArgs)
            {
                HandleKeyActions(((GuiKeyActionsEventArgs)e).KeyActions);
            }
        }

        public void LoadResources(ResourceManager resourceManager)
        {
            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";

            if (ReferenceEquals(null, resourceManager.GetShader(_visual.VoxelShaderName)))
            {
                resourceManager.AddShader(_visual.VoxelShaderName, dir + "voxelVertex.glsl", dir + "voxelFragment.glsl"
                    , Resourcen.voxelVertex, Resourcen.voxelFragment);
            }
            if (ReferenceEquals(null, resourceManager.GetShader(_visual.RaytraceShaderName)))
            {
                resourceManager.AddShader(_visual.RaytraceShaderName, dir + "raytraceVertex.glsl", dir + "raytraceFragment.glsl"
                    , Resourcen.raytraceVertex, Resourcen.raytraceFragment);
            }
            if (ReferenceEquals(null, resourceManager.GetShader(_visual.AddShaderName)))
            {
                resourceManager.AddShader(_visual.AddShaderName, dir + "screenQuadVertex.glsl", dir + "addFragment.glsl"
                    , Resourcen.screenQuadVertex, Resourcen.addFragment);
            }
            if (ReferenceEquals(null, resourceManager.GetShader(_visual.SsaoShaderName)))
            {
                resourceManager.AddShader(_visual.SsaoShaderName, dir + "screenQuadVertex.glsl", dir + "ssaoFragment.glsl"
                    , Resourcen.screenQuadVertex, Resourcen.ssaoFragment);
            }
            if (ReferenceEquals(null, resourceManager.GetShader(_visual.DepthShaderName)))
            {
                resourceManager.AddShader(_visual.DepthShaderName, dir + "depthVertex.glsl", dir + "depthFragment.glsl"
                    , Resourcen.depthVertex, Resourcen.depthFragment);
            }
        }

        public void ShaderChanged(string name, IShader shader)
        {
            _visual.ShaderChanged(name, shader);
        }

        public void Render(IViewModel viewModel)
        {
            SetCursorVisible();
            _visual.Render((EditorViewModel)viewModel);
        }

        public void Resize(int width, int height)
        {
            _visual.Resize(width, height);
        }

        private void HandleKeyActions(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.Exit))
            {
                ExitGame();
            }
            if (keyActions.Contains(KeyAction.ToggleFullscreen))
            {
                ToggleFullscreen();
            }
            if (keyActions.Contains(KeyAction.ToggleCursorVisible))
            {
                _cursorVisible = !_cursorVisible;
            }
        }

        private void ExitGame()
        {
            GameWindowEvent?.Invoke(gameWindow => gameWindow.Exit());
        }

        private void ToggleFullscreen()
        {
            GameWindowEvent?.Invoke(gameWindow => gameWindow.WindowState = WindowState.Fullscreen == gameWindow.WindowState ? WindowState.Normal : WindowState.Fullscreen);
        }

        private void SetCursorVisible()
        {
            GameWindowEvent?.Invoke(gameWindow => gameWindow.CursorVisible = _cursorVisible);
        }
    }
}
