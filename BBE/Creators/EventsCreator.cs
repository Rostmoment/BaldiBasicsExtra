using BBE.Events.HookChaos;
using BBE.Events;
using BBE.Extensions;
using MTM101BaldAPI.ObjectCreation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;
using BBE.CustomClasses;

namespace BBE.Creators
{
    class EventsCreator
    {
        private static void AddToFloors(RandomEvent randomEvent, int F1, int F2, int F3, int END)
        {
            if (F1 > 0)
                FloorData.Get("F1").randomEvent.Add(new WeightedRandomEvent() { selection = randomEvent, weight = F1 });
            if (F2 > 0)
                FloorData.Get("F2").randomEvent.Add(new WeightedRandomEvent() { selection = randomEvent, weight = F1 });
            if (F3 > 0)
                FloorData.Get("F3").randomEvent.Add(new WeightedRandomEvent() { selection = randomEvent, weight = F1 });
            if (END > 0)
                FloorData.Get("END").randomEvent.Add(new WeightedRandomEvent() { selection = randomEvent, weight = F1 });
        }
        public static void CreateEvents()
        {
            
            SoundObject sound = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "Starts", "BBE_TeleportationChaos.wav"), SoundType.Voice, Color.green, -1f, "BBE_Event_TeleportationChaos1");
            sound = sound.AddAdditionalKey("BBE_Event_TeleportationChaos2", 1.2f).AddAdditionalKey("BBE_Event_TeleportationChaos3", 5.2f);
            RandomEvent randomEvent = new RandomEventBuilder<TeleportationChaosEvent>(BasePlugin.Instance.Info)
                .SetMinMaxTime(30, 60)
                .SetEnumAndName(ModdedRandomEvent.TeleportationChaos)
                .SetSound(sound)
                .Build();
            AddToFloors(randomEvent, 0, 30, 60, 60);

            sound = AssetsHelper.LoadAsset<SoundObject>("BAL_Event_Ruler");
            randomEvent = new RandomEventBuilder<SoundEvent>(BasePlugin.Instance.Info)
                .SetMinMaxTime(50, 60)
                .SetEnumAndName(ModdedRandomEvent.SoundEvent)
                .SetSound(sound)
                .Build();
            AddToFloors(randomEvent, 45, 0, 0, 0);
            
            sound = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "Starts", "BBE_HookChaos.wav"), SoundType.Voice, Color.green, -1f, "BBE_Event_HookChaos1");
            sound = sound.AddAdditionalKey("BBE_Event_HookChaos2", 2.7f).AddAdditionalKey("BBE_Event_HookChaos3", 5f);
            randomEvent = new RandomEventBuilder<HookChaosEvent>(BasePlugin.Instance.Info)
                .SetMinMaxTime(90, 120)
                .SetEnumAndName(ModdedRandomEvent.HookChaos)
                .SetSound(sound)
                .Build();
            AddToFloors(randomEvent, 0, 0, 60, 60);
            
            sound = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "Starts", "BBE_ElectricityEvent.wav"), SoundType.Voice, Color.green, -1f, "BBE_Event_Electricity1");
            sound = sound.AddAdditionalKey("BBE_Event_Electricity2", 2.8f).AddAdditionalKey("BBE_Event_Electricity3", 7f);
            randomEvent = new RandomEventBuilder<ElectricityEvent>(BasePlugin.Instance.Info)
                .SetMinMaxTime(60, 90)
                .SetEnumAndName(ModdedRandomEvent.ElectricityEvent)
                .SetSound(sound)
                .Build();
            AddToFloors(randomEvent, 0, 30, 50, 70);
            
            sound = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "Events", "Starts", "BBE_FireEvent.wav"), SoundType.Voice, Color.green, -1f, "BBE_Event_Fire1");
            sound = sound.AddAdditionalKey("BBE_Event_Fire2", 1.3f).AddAdditionalKey("BBE_Event_Fire3", 5f);
            randomEvent = new RandomEventBuilder<FireEvent>(BasePlugin.Instance.Info)
                .SetMinMaxTime(60, 90)
                .SetEnumAndName(ModdedRandomEvent.FireEvent)
                .SetSound(sound)
                .Build();
            AddToFloors(randomEvent, 0, 60, 0, 80);
        }
    }
}
