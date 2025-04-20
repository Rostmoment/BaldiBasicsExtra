using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.Structures
{
    abstract class CustomSwingDoor<T> : MonoBehaviour where T : CustomSwingDoor<T>
    {
        public static GameObject Create()
        {
            if (CachedAssets.customSwingDoors.ContainsKey(typeof(T)))
                return CachedAssets.customSwingDoors[typeof(T)];
            SwingDoor door = Instantiate(Prefabs.SwingDoor);
            door.gameObject.ConvertToPrefab(true);
            T res = door.gameObject.AddComponent<T>();
            res.gameObject.ConvertToPrefab(true);
            res.Initialize();
            res.name = typeof(T).Name;
            CachedAssets.customSwingDoors.Add(typeof(T), res.gameObject);
            return door.gameObject;
        }

        public SwingDoor SwingDoor
        {
            get
            {
                SwingDoor door = gameObject.GetComponent<SwingDoor>();
                if (door != null && BaseGameManager.Instance != null)
                    door.ec = BaseGameManager.Instance.ec;
                return door;
            }
        }
        private Material newOverlay;
        private Material[] originalOverlays;
        public abstract Texture2D MaterialTexture { get; }
        public static List<T> all = new List<T>();

        public virtual void VirtualStart()
        {
            if (SwingDoor == null)
                return;
            originalOverlays = new Material[SwingDoor.overlayLocked.Length];
            newOverlay = Instantiate(AssetsHelper.LoadAsset<CoinDoor>().coinDoorOverlay);
            newOverlay.SetMainTexture(MaterialTexture);
            for (int i = 0; i < SwingDoor.overlayLocked.Length; i++)
            {
                originalOverlays[i] = SwingDoor.overlayLocked[i];
                SwingDoor.overlayLocked[i] = newOverlay;
            }
        }
        public virtual void Initialize()
        {
            all.Add((T)this);
        }
        public virtual void VirtualOnDestroy()
        {
            all.Remove((T)this);
            for (int i = 0; i < SwingDoor.overlayLocked.Length; i++)
            {
                SwingDoor.overlayLocked[i] = originalOverlays[i];
            }
        }
        public virtual void VirtualUpdate()
        {

        }
        private void Update()
        {
            VirtualUpdate();
        }
        private void Start()
        {
            VirtualStart();
        }
        private void OnDesroy()
        {
            VirtualOnDestroy();
        }
    }
}
