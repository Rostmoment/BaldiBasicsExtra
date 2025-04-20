using BBE.Extensions;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace BBE.CustomClasses.FunSettings
{
    class ModdedMirrored : MonoBehaviour
    {
        public static ModdedMirrored Instance => BaseGameManager.Instance.gameObject.GetOrAddComponent<ModdedMirrored>();
        public bool flippedByX = false;
        public bool flippedByY = false;

        private Camera[] cameraToMirror;


        void OnDisable()
        {
            Disable();
        }
        void OnDestroy()
        {
            Disable();
        }
        void Start()
        {
            flippedByX = flippedByY = false;
        }
        public void FlipByX(bool flip)
        {
            if (flip == flippedByY)
                return;
            SetupCamera();
            flippedByY = flip;
            foreach (Camera obj in cameraToMirror)
            {
                Matrix4x4 projectionMatrix = obj.projectionMatrix;
                projectionMatrix *= Matrix4x4.Scale(new Vector3(1f, -1f, 1f));
                obj.projectionMatrix = projectionMatrix;
            }
            if (flip)
            {
                RenderPipelineManager.beginCameraRendering += ReverseCulling;
                RenderPipelineManager.endCameraRendering += ReturnCulling;
            }
            else
            {
                RenderPipelineManager.beginCameraRendering -= ReverseCulling;
                RenderPipelineManager.endCameraRendering -= ReturnCulling;
            }
        }
        public void FlipByX()
        {
            flippedByX = !flippedByX;
            FlipByX(flippedByX);
        }
        public void FlipByY(bool flip)
        {
            if (flip == flippedByY)
                return;
            SetupCamera();
            flippedByY = flip;
            foreach (Camera obj in cameraToMirror)
            {
                Matrix4x4 projectionMatrix = obj.projectionMatrix;
                projectionMatrix *= Matrix4x4.Scale(new Vector3(1f, -1f, 1f));
                obj.projectionMatrix = projectionMatrix;
            }
            if (flip)
            {
                RenderPipelineManager.beginCameraRendering += ReverseCulling;
                RenderPipelineManager.endCameraRendering += ReturnCulling;
            }
            else
            {
                RenderPipelineManager.beginCameraRendering -= ReverseCulling;
                RenderPipelineManager.endCameraRendering -= ReturnCulling;
            }
        }
        public void FlipByY()
        {
            flippedByY = !flippedByY;
            FlipByY(flippedByY);
        }
        public void DisableFlipByY()
        {
            if (!flippedByY)
                return;
            FlipByY();
        }
        public void DisableFlipByX()
        {
            if (!flippedByX) 
                return;
            FlipByX();
        }
        public void Disable()
        {
            DisableFlipByX();
            DisableFlipByY();
        }
        private void ReverseCulling(ScriptableRenderContext context, Camera camera)
        {
            if (camera == cameraToMirror[0])
            {
                GL.invertCulling = true;
            }
        }
        private void ReturnCulling(ScriptableRenderContext context, Camera camera)
        {
            if (camera == cameraToMirror[0] || camera == cameraToMirror[1])
            {
                GL.invertCulling = false;
            }
        }
        private void SetupCamera()
        {
            if (cameraToMirror.EmptyOrNull())
            {
                cameraToMirror = new Camera[] {
                    Singleton<CoreGameManager>.Instance.GetCamera(0).camCom,
                    CoreGameManager.Instance.GetCamera(0).billboardCam
                };
            }
        }
    }
}
