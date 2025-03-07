using BBE.CustomClasses;
using BBE.Extensions;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Helpers
{
    internal class Prefabs
    {
        public static SwingDoor SwingDoor
        {
            get
            {
                if (swingDoor == null) swingDoor = AssetsHelper.LoadAsset<SwingDoor>("Door_Swinging");
                return swingDoor;
            }
            set
            {
                swingDoor = value;
            }
        }
        public static GenericHallBuilder HallBuilder
        {
            get
            {
                if (hallBuilder == null) hallBuilder = AssetsHelper.LoadAsset<GenericHallBuilder>("ZestyHallBuilder");
                return hallBuilder;
            }
            set
            {
                hallBuilder = value;
            }
        }
        public static SodaMachine SodaMachine
        {
            get
            {
                if (sodaMachine == null) sodaMachine = AssetsHelper.LoadAsset<GameObject>("SodaMachine").GetComponent<SodaMachine>();
                return sodaMachine;
            }
            set
            {
                sodaMachine = value;
            }
        }
        public static MenuToggle MenuToggle
        {
            get
            {
                if (menuToggle == null) menuToggle = AssetsHelper.LoadAsset<MenuToggle>();
                return menuToggle;
            }
            set
            {
                menuToggle = value;
            }
        }
        public static MapIcon MapIcon
        {
            get
            {
                if (mapIcon == null) mapIcon = AssetsHelper.LoadAsset<Notebook>().iconPre;
                return mapIcon;
            }
            set
            {
                mapIcon = value;
            }
        }
        public static CursorController CursorController
        {
            get
            {
                if (cursor == null) cursor = AssetsHelper.LoadAsset<CursorController>("CursorOrigin");
                return cursor;
            }
            set
            {
                cursor = value;
            }
        }
        public static GameObject Canvas
        {
            get
            {
                if (canvas == null) canvas = AssetsHelper.LoadAsset<GameObject>("GumOverlay");
                return canvas;
            }
            set
            {
                canvas = value;
            }
        }
        private static GenericHallBuilder hallBuilder;
        private static SodaMachine sodaMachine;
        private static MenuToggle menuToggle;
        private static MapIcon mapIcon;
        private static GameObject canvas;
        private static CursorController cursor;
        private static SwingDoor swingDoor;
    }
}
