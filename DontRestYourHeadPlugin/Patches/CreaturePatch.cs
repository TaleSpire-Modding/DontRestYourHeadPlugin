using HarmonyLib;
using UnityEngine;

namespace DontRestYourHeadPlugin.Patches
{
    [HarmonyPatch(typeof(Creature), "GetStatByIndex")]
    public class CreaturePatchGetStatByIndex
    {
        static bool Prefix(int index, CreatureDataV2 ____mostRecentCreatureData, CreatureStat __result)
        {
            switch (index)
            {
                case -1:
                    __result = ____mostRecentCreatureData.Hp;
                    break;
                case 0:
                    __result = ____mostRecentCreatureData.Stat0;
                    break;
                case 1:
                    __result = ____mostRecentCreatureData.Stat1;
                    break;
                case 2:
                    __result = ____mostRecentCreatureData.Stat2;
                    break;
                case 3:
                    __result = ____mostRecentCreatureData.Stat3;
                    break;
                case 4:
                    __result = ____mostRecentCreatureData.Stat4;
                    break;
                case 5:
                    __result = ____mostRecentCreatureData.Stat5;
                    break;
                case 6:
                    __result = ____mostRecentCreatureData.Stat6;
                    break;
                case 7:
                    __result = ____mostRecentCreatureData.Stat7;
                    break;
                default:
                    Debug.LogError((object)("Invalid state index:" + index.ToString()));
                    __result = new CreatureStat();
                    break;
            }

            return false;
        }
    }
}
