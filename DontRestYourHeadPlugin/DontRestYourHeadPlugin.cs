using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BepInEx;
using Bounce.Unmanaged;
using DiceCallbackPlugin;
using DiceCallbackPlugin.Patches;
using RadialUI;
using DontRestYourHeadPlugin.Extensions;
using HarmonyLib;

namespace DontRestYourHeadPlugin
{

    [BepInPlugin(Guid, "HolloFoxes' Dont Rest Your Head Plugin", Version)]
    [BepInDependency(DiceCallbackPlugin.DiceCallbackPlugin.Guid)]
    [BepInDependency(RadialUIPlugin.Guid)]
    public class DontRestYourHeadPlugin : BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.DontRestYourHead";
        private const string Version = "1.0.0.0";

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log("Don't Rest Your Head Plug-in loaded");
            ModdingUtils.Initialize(this, Logger);
            RadialUIPlugin.AddOnRemoveCharacter(Guid,"Enable Torch",ShouldHide);
            
            RadialUIPlugin.AddOnCharacter(Guid,new MapMenu.ItemArgs
            {
                Action = AttackAdv,
                Title = "Roll for Madness",
                CloseMenuOnActivate = true
            }
            , check
            );

            RegisteredCallbacks.Callbacks.Add(Guid,RenderRoll);

            var harmony = new Harmony(Guid);
            harmony.PatchAll();
        }

        private static bool ShouldHide(string menuText, string miniId, string targetId)
        {
            return !LocalPlayer.Rights.CanGm;
        }

        private string RenderRoll(DiceManager.DiceRollResultData data,CallbackType type)
        {
            var name = data.GroupResults[0].Name;
            int count = 0;
            List<short> discipline = null;
            List<short> madness = null;
            List<short> exhaustion = null;
            if (name.Contains("<dis>"))
            {
                discipline = data.GroupResults[count].Dice[0].Results.ToList();
                count++;
            }
            if (name.Contains("<mad>"))
            {
                madness = data.GroupResults[count].Dice[0].Results.ToList();
                count++;
            }
            if (name.Contains("<exh>"))
            {
                exhaustion = data.GroupResults[count].Dice[0].Results.ToList();
            }



            if (discipline == null) discipline = new List<short>();
            if (madness == null) madness = new List<short>();
            if (exhaustion== null) exhaustion = new List<short>();

            discipline.Sort();
            madness.Sort();
            exhaustion.Sort();

            int success = 
                discipline.Count(d => d < 4) +
                madness.Count(d => d < 4) +
                exhaustion.Count(d => d < 4);

            string dom = "";
            while (dom.Length == 0  && (discipline.Any() || madness.Any() || exhaustion.Any()))
            {
                var d = discipline.PopHighest();
                var m = madness.PopHighest();
                var e = exhaustion.PopHighest();
                var highest = d;
                if (highest < m) highest = m;
                if (highest < e) highest = e;

                if (highest > d)
                {
                    discipline.Clear();
                    d = -1;
                }

                if (highest > m)
                {
                    madness.Clear();
                    m = -1;
                }

                if (highest > e)
                {
                    exhaustion.Clear();
                    e = -1;
                }

                if (d > 0 && m == -1 && e == -1)
                {
                    dom = "Discipline";
                } else if (m > 0 && d == -1 && e == -1)
                {
                    dom = "Madness";
                }
                else if (e > 0 && d == -1 && m == -1)
                {
                    dom = "Exhaustion";
                }
                else if (discipline.Count == 0 && madness.Count == 0 && exhaustion.Count == 0)
                {
                    dom = d > 0 ? "Discipline" : "Madness";
                }
            }

            var o = $"{success} Successes, {dom}";
            if (type == CallbackType.UIChat) o += " Dominated";
            return o;
        }

        private static NGuid radialId;

        private bool check(NGuid id1, NGuid id2)
        {
            radialId = id2;
            return true;
        }

        private void AttackAdv(MapMenuItem item, object obj)
        {
            CreatureManager.TryGetCreatureData(new CreatureGuid(radialId), out var creature);

            var d = creature.Stat6.Value;
            var m = creature.Stat1.Value;
            var e = creature.Stat0.Value;

            if (d < creature.Stat6.Max) m += creature.Stat6.Max - d;

            var tags = "";
            

            Debug.Log($"{d}{m}{e}");

            var discipline = new DiceRoller.Dice();
            discipline.Add(DiceRoller.DiceType.d6,(int)d);

            var madness = new DiceRoller.Dice();
            madness.Add(DiceRoller.DiceType.d6, (int)m);

            var exhaustion = new DiceRoller.Dice();
            exhaustion.Add(DiceRoller.DiceType.d6, (int)e);

            var formulas = new List<DiceRoller.Dice>();
            var colors = new List<DiceColor>();

            if (d > 0) 
            {
                tags += "<dis>";
                formulas.Add(discipline);
                colors.Add(new DiceColor(Color.grey));
            }

            if (m > 0)
            {
                tags += "<mad>";
                formulas.Add(madness);
                colors.Add(new DiceColor(Color.red));
            }

            if (e > 0)
            {
                tags += "<exh>";
                formulas.Add(exhaustion);
                colors.Add(new DiceColor(Color.yellow));
            }

            DiceRoller.RollDice($"for Madness{tags}",formulas.ToArray(),$"{Guid}",colors.ToArray());
        }

    }
}
