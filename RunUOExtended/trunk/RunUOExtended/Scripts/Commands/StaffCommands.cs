/*************
-- Please do not edit this Header --

.Control originally scripted by Quick_silver
for www.Welt-von-Midgrd.ch
and everyone who want to use it.
 * *********************/


using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using System.Reflection;
using Server.Items;
using System.Collections;
using Server.Mobiles;
using Server.Commands;

namespace Server.Commands
{
    public class StaffCommands
    {
        public static AccessLevel accessLevel = AccessLevel.GameMaster;
        private static List<Mobile> m_HearAll = new List<Mobile>();

        public static void Initialize()
        {
            CommandSystem.Register("HearAll", accessLevel, new CommandEventHandler(HearAll_OnCommand));
            CommandSystem.Register("SayThis", accessLevel, new CommandEventHandler(SayThis_OnCommand));
            CommandSystem.Register("GmMe", accessLevel, new CommandEventHandler(GmMe_OnCommand));
            CommandSystem.Register("Refresh", accessLevel, new CommandEventHandler(Refresh_OnCommand));
            CommandSystem.Register("Clone", accessLevel, new CommandEventHandler(Clone_OnCommand));
            CommandSystem.Register("CloneMe", accessLevel, new CommandEventHandler(CloneMe_OnCommand));
            CommandSystem.Register("Control", accessLevel, new CommandEventHandler(Control_OnCommand));
            EventSink.Speech += new SpeechEventHandler(HearAllOnSpeech);
        }

        public static void HearAllOnSpeech(SpeechEventArgs e)
        {
            if (m_HearAll.Count > 0)
            {
                string msg = String.Format("({0}): {1}", e.Mobile.RawName, e.Speech);

                for (int i = 0; i < m_HearAll.Count; ++i)
                {
                    m_HearAll[i].SendMessage(msg);
                }
            }
        }
        [Usage("HearAll")]
        [Description("Enable or Disable hearing everything in the world.")]
        public static void HearAll_OnCommand(CommandEventArgs e)
        {
            if (m_HearAll.Contains(e.Mobile))
            {
                m_HearAll.Remove(e.Mobile);
                e.Mobile.SendMessage("HearAll deactivated.");
            }
            else
            {
                m_HearAll.Add(e.Mobile);
                e.Mobile.SendMessage("HearAll enabled.");
            }
        }
        [Usage("SayThis <text>")]
        [Description("Forces Target to Say <text>.")]
        public static void SayThis_OnCommand(CommandEventArgs e)
        {
            string toSay = e.ArgString.Trim();

            if (toSay.Length > 0)
                e.Mobile.Target = new SayThisTarget(toSay);
            else
                e.Mobile.SendMessage("Format: SayThis \"<text>\"");
        }
        [Usage("GmMe")]
        [Description("Helps senior staff members set their body to GM style.")]
        public static void GmMe_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            CommandLogging.WriteLine(from, "{0} {1} is assuming a GM body", from.AccessLevel, CommandLogging.Format(from));
            from.Blessed = true;
            DisRobe(from, Layer.Shoes);
            DisRobe(from, Layer.Pants);
            DisRobe(from, Layer.Shirt);
            DisRobe(from, Layer.Helm);
            DisRobe(from, Layer.Gloves);
            DisRobe(from, Layer.Neck);
            DisRobe(from, Layer.Hair);
            DisRobe(from, Layer.Waist);
            DisRobe(from, Layer.InnerTorso);
            DisRobe(from, Layer.MiddleTorso);
            DisRobe(from, Layer.Arms);
            DisRobe(from, Layer.Cloak);
            DisRobe(from, Layer.OuterTorso);
            DisRobe(from, Layer.OuterLegs);
            from.AddItem(new GMRobe());

            for (int i = 0; i < from.Skills.Length; ++i)
                from.Skills[i].Base = 120;
        }
        [Usage("Refresh")]
        [Description("Sets all targets stats to full.")]
        public static void Refresh_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new freshTarget();
        }

        [Usage("Clone")]
        [Description("Assume the form of another Player or Creature.")]
        public static void Clone_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new CloneTarget();
        }

        [Usage("CloneMe")]
        [Description("Makes an exact duplicate of you at your present location and hides you")]
        public static void CloneMe_OnCommand(CommandEventArgs e)
        {
            BaseCreature m = new BaseCreature(AIType.AI_None, FightMode.None, 10, 1, 0.2, 0.4);
            m.InitStats(10, 10, 10);
            m.SetSkill(SkillName.Cooking, 65, 88);
            m.SetSkill(SkillName.Snooping, 65, 88);
            m.SetSkill(SkillName.Stealing, 65, 88);
            if (m.Female = Utility.RandomBool())
            {
                m.Body = 0x1A6;
                m.Name = NameList.RandomName("female");


            }
            else
            {
                m.Body = 0x1A4;
                m.Name = NameList.RandomName("male");

            }
            e.Mobile.Hidden = true;
            m.Dex = e.Mobile.Dex;
            m.Int = e.Mobile.Int;
            m.Str = e.Mobile.Str;
            m.Fame = e.Mobile.Fame;
            m.Karma = e.Mobile.Karma;
            m.NameHue = e.Mobile.NameHue;
            m.SpeechHue = e.Mobile.SpeechHue;
            m.Criminal = e.Mobile.Criminal;
            m.Name = e.Mobile.Name;
            m.Title = e.Mobile.Title;
            m.Female = e.Mobile.Female;
            m.Body = e.Mobile.Body;
            m.Hue = e.Mobile.Hue;
            m.Hits = e.Mobile.HitsMax;
            m.Mana = e.Mobile.ManaMax;
            m.Stam = e.Mobile.StamMax;
            m.BodyMod = e.Mobile.Body;
            m.Map = e.Mobile.Map;
            m.Location = e.Mobile.Location;
            m.Direction = e.Mobile.Direction;
            m.HairItemID = e.Mobile.HairItemID;
            m.FacialHairItemID = e.Mobile.FacialHairItemID;
            m.HairHue = e.Mobile.HairHue;
            m.FacialHairHue = e.Mobile.FacialHairHue;

            for (int i = 0; i < e.Mobile.Skills.Length; i++)
                m.Skills[i].Base = e.Mobile.Skills[i].Base;

            ArrayList items = new ArrayList(e.Mobile.Items);
            for (int i = 0; i < items.Count; i++)
            {
                Item item = (Item)items[i]; //my favorite line of code, ever. 

                if (((item != null) && (item.Parent == e.Mobile) && (item != e.Mobile.Backpack)))
                {
                    Type t = item.GetType();
                    ConstructorInfo c = t.GetConstructor(Type.EmptyTypes);
                    if (c != null)
                    {
                        try
                        {
                            object o = c.Invoke(null);
                            if (o != null && o is Item)
                            {
                                Item newItem = (Item)o;
                                CopyProperties(newItem, item);
                                item.OnAfterDuped(newItem);
                                newItem.Parent = null;
                                m.AddItem(newItem);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        [Usage("Control [target]")]
        [Description("Permet de controler un mobile.")]
        //"Let you control a NPC."
        private static void Control_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            e.GetInt32(0);

            if (from != null)
            {
                from.SendMessage("Ciblez le mobile � controler ?");
                //"Choose the target to control..."

                from.Target = new ControlTarget(e.Arguments);
            }
        }

        private class SayThisTarget : Target
        {
            private string m_toSay;

            public SayThisTarget(string s)
                : base(-1, false, TargetFlags.None)
            {
                m_toSay = s;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {

                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;
                    targ.Say(m_toSay);

                }
                else if (targeted is Item)
                {
                    Item objet = targeted as Item;
                    objet.PublicOverheadMessage(MessageType.Regular, 0, false, "" + m_toSay + "");
                }

            }
        }

        private static void DisRobe(Mobile m_from, Layer layer)
        {
            if (m_from.FindItemOnLayer(layer) != null)
            {
                Item item = m_from.FindItemOnLayer(layer);
                m_from.PlaceInBackpack(item); // Place in a bag first? 
            }
        }
        public class freshTarget : Target
        {
            public freshTarget()
                : base(12, false, TargetFlags.None)
            {
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;
                    if (!from.CanSee(targ))
                    {
                        from.SendMessage("The target is not in your line of sight!");
                    }
                    else
                    {
                        targ.Hits = targ.HitsMax;
                        targ.Mana = targ.ManaMax;
                        targ.Stam = targ.StamMax;
                    }
                }
            }
        }
        private class CloneTarget : Target
        {
            bool real = true;
            bool location = false;
            bool statsskills = false;
            public CloneTarget()
                : base(-1, false, TargetFlags.None)
            {
            }
            public void SimulateTarget(Mobile from, object targeted, bool loc)
            {
                real = false;
                location = loc;
                OnTarget(from, targeted);
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;

                    if (from != targ && (!real || from.AccessLevel > targ.AccessLevel))
                    {
                        CommandLogging.WriteLine(from, "{0} {1} is cloning {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targ));

                        from.Dex = targ.Dex;
                        from.Int = targ.Int;
                        from.Str = targ.Str;
                        from.Fame = targ.Fame;
                        from.Karma = targ.Karma;
                        from.NameHue = targ.NameHue;
                        from.SpeechHue = targ.SpeechHue;

                        from.Name = targ.Name;
                        from.Title = targ.Title;
                        from.Female = targ.Female;
                        from.Body = targ.Body;
                        from.Hue = targ.Hue;

                        from.Hits = from.HitsMax;
                        from.Mana = from.ManaMax;
                        from.Stam = from.StamMax;

                        if (location)
                        {
                            from.Location = targ.Location;
                            from.Direction = targ.Direction;
                            from.Map = targ.Map;
                        }

                        from.HairItemID = targ.HairItemID;
                        from.FacialHairItemID = targ.FacialHairItemID;
                        from.HairHue = targ.HairHue;
                        from.FacialHairHue = targ.FacialHairHue;

                        if (!targ.Player)
                            from.BodyMod = targ.Body;
                        else
                            from.BodyMod = 0;

                        for (int i = 0; i < from.Skills.Length; i++)
                            from.Skills[i].Base = targ.Skills[i].Base;


                        ArrayList m_items = new ArrayList(from.Items);
                        for (int i = 0; i < m_items.Count; i++)
                        {
                            Item item = (Item)m_items[i];
                            if (((item.Parent == from) && (item != from.Backpack)))
                                item.Delete();
                        }


                        ArrayList items = new ArrayList(targ.Items);
                        for (int i = 0; i < items.Count; i++)
                        {
                            Item item = (Item)items[i]; //my favorite line of code, ever. 

                            if (((item != null) && (item.Parent == targ) && (item != targ.Backpack)))
                            {
                                Type t = item.GetType();
                                ConstructorInfo c = t.GetConstructor(Type.EmptyTypes);
                                if (c != null)
                                {
                                    try
                                    {
                                        object o = c.Invoke(null);
                                        if (o != null && o is Item)
                                        {
                                            Item newItem = (Item)o;
                                            CopyProperties(newItem, item);
                                            item.OnAfterDuped(newItem);
                                            newItem.Parent = null;
                                            from.AddItem(newItem);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                        if( !real)
                            CopyProps(from, targ, true, true, location);
                    }
                }
            }
        }
        private static void CopyProperties(Item dest, Item src)
        {
            PropertyInfo[] props = src.GetType().GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                try
                {
                    if (props[i].CanRead && props[i].CanWrite)
                    {

                        // These properties must not be copied during the dupe, they get set implicitely by placing
                        // items properly using "DropItem()" etc. .
                        switch (props[i].Name)
                        {
                            case "Parent":
                            case "TotalWeight":
                            case "TotalItems":
                            case "TotalGold":
                                break;
                            default:
                                props[i].SetValue(dest, props[i].GetValue(src, null), null);
                                break;
                        }
                        // end exceptions 
                    }
                }
                catch
                {
                }

                // BaseArmor, BaseClothing, BaseJewel, BaseWeapon: copy nested classes
                // ToDo: If someone knows something about dynamic casting these 4 blocks
                //       could be integrated into one...
                if (src is BaseWeapon)
                {
                    object src_obj = ((BaseWeapon)src).Attributes;
                    object dest_obj = ((BaseWeapon)dest).Attributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseWeapon)src).SkillBonuses;
                    dest_obj = ((BaseWeapon)dest).SkillBonuses;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseWeapon)src).WeaponAttributes;
                    dest_obj = ((BaseWeapon)dest).WeaponAttributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseWeapon)src).AosElementDamages;
                    dest_obj = ((BaseWeapon)dest).AosElementDamages;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);
                }
                else if (src is BaseArmor)
                {
                    object src_obj = ((BaseArmor)src).Attributes;
                    object dest_obj = ((BaseArmor)dest).Attributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseArmor)src).SkillBonuses;
                    dest_obj = ((BaseArmor)dest).SkillBonuses;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseArmor)src).ArmorAttributes;
                    dest_obj = ((BaseArmor)dest).ArmorAttributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);
                }
                else if (src is BaseJewel)
                {
                    object src_obj = ((BaseJewel)src).Attributes;
                    object dest_obj = ((BaseJewel)dest).Attributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseJewel)src).SkillBonuses;
                    dest_obj = ((BaseJewel)dest).SkillBonuses;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseJewel)src).Resistances;
                    dest_obj = ((BaseJewel)dest).Resistances;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                }
                else if (src is BaseClothing)
                {
                    object src_obj = ((BaseClothing)src).Attributes;
                    object dest_obj = ((BaseClothing)dest).Attributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseClothing)src).SkillBonuses;
                    dest_obj = ((BaseClothing)dest).SkillBonuses;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseClothing)src).ClothingAttributes;
                    dest_obj = ((BaseClothing)dest).ClothingAttributes;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                    src_obj = ((BaseClothing)src).Resistances;
                    dest_obj = ((BaseClothing)dest).Resistances;

                    if (src_obj != null && dest_obj != null)
                        CopyProperties(dest_obj, src_obj);

                }
                // end copying nested classes

            }
        }
        //Duplicates props between two items of same type 
        private static void CopyProperties(object dest, object src)
        {
            PropertyInfo[] props = src.GetType().GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                try
                {
                    if (props[i].CanRead && props[i].CanWrite)
                    {
                        props[i].SetValue(dest, props[i].GetValue(src, null), null);
                    }
                }
                catch
                {
                }
            }
        }
        private class ControlTarget : Target
        {
            string[] m_parameter;

            public ControlTarget(params string[] parameter)
                : base(-1, true, TargetFlags.None)
            {
                m_parameter = parameter;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                bool stats = true;
                bool skills = true;
                bool items = true;

                for (int i = 0; i < m_parameter.Length; i++)
                    if (string.Compare(m_parameter[i], "NoStats", true) == 0)
                        stats = false;

                for (int i = 0; i < m_parameter.Length; i++)
                    if (string.Compare(m_parameter[i], "NoSkills", true) == 0)
                        skills = false;

                for (int i = 0; i < m_parameter.Length; i++)
                    if (string.Compare(m_parameter[i], "NoItems", true) == 0)
                        items = false;


                DoControl(from, targeted, stats, skills, items);
            }
        }

        /*Find the Control item of the Mobile from*/
        public static ControlItem GetControlItem(Mobile from)
        {
            Item result = SearchItemInCont(typeof(ControlItem), from.Backpack);

            if (result != null && result is ControlItem)
                return (ControlItem)result;
            else
                return null;
        }

        private static Item SearchItemInCont(Type targetType, Container cont)
        {
            Item item;

            if (cont != null && !cont.Deleted)
            {
                for (int i = 0; i < cont.Items.Count; i++)
                {
                    item = (Item)cont.Items[i];
                    // recursively search containers
                    if (item != null && !item.Deleted)
                    {
                        if (item.GetType() == targetType)
                            return item;
                        else if (item is Container)
                            item = SearchItemInCont(targetType, (Container)item);

                        if (item != null && item.GetType() == targetType)
                            return item;
                    }
                }
            }

            return null;
        }

        public static void DoControl(Mobile from, object targeted, bool stats, bool skills, bool items)
        {
            Mobile target;

            if (from is PlayerMobile && targeted is Mobile)
            {
                if (targeted is PlayerMobile && ((PlayerMobile)targeted).Player)
                {
                    from.SendMessage("Vous ne pouvez pas controler un joueur.");
                    //"You cant control players"
                    return;
                }

                target = (Mobile)targeted;
                ControlItem controlItem = GetControlItem(from);

                if (controlItem == null)
                {
                    from.SendMessage("Stats: {0} Skills: {1} Items: {2}", stats, skills, items);
                    StartControl(from, target, stats, skills, items);
                }
                else
                {
                    from.SendMessage("Stats: {0} Skills: {1} Items: {2}", controlItem.Stats, controlItem.Skills, controlItem.Items);
                    ChangeControl(target, controlItem, controlItem.Stats, controlItem.Skills, controlItem.Items);
                }
            }
            else if (from is PlayerMobile && targeted is ControlItem)
                ((Item)targeted).Delete();
        }


        private static void StartControl(Mobile from, Mobile target, bool stats, bool skills, bool items)
        {
            from.SendMessage("You leave your Body and control {0}, {1}", target.Name, target.Title);
            //"You leave your Body and control {0}, {1}"

            //Clone Player
            PlayerMobile playerClone = (PlayerMobile)DupeMobile(from);
            new CloneTarget().SimulateTarget(playerClone, from, false);

            //Create ControlItem
            ControlItem controlItem = new ControlItem(from, playerClone, target, stats, skills, items);
            from.Backpack.DropItem(controlItem);

            /*
            //Props target -> player
            CopyProps(from, target, stats, skills);

            //Backup Equip
            //Equip from target to player
            MoveEquip(target, from, items);
            */
            new CloneTarget().SimulateTarget(from, target, true);
            from.Hidden = target.Hidden;

            target.Internalize();
            playerClone.Internalize();

        }


        private static void ChangeControl(Mobile target, ControlItem controlItem, bool stats, bool skills, bool items)
        {
            Mobile from = controlItem.Owner;
            PlayerMobile oldPlayer = controlItem.Player;
            Mobile oldNPC = controlItem.NPC;

            if (oldNPC != null)
            {
                //NPC Wiederherstellen
                if (!oldNPC.Deleted)
                {
                    //Props immer �bernehmen bei der R�ckverwandlung?
                    //ja, weil sich hits etc �ndern
                    //Props from -> oldNPC
                    new CloneTarget().SimulateTarget(oldNPC, from, true);
                }
                else
                {
                    from.SendMessage("Votre corps d'origine a �t� d�truit.");
                    oldNPC.Delete();
                }
            }

            //Neuen Mobile �bernehmen oder Control abbrechen
            if (target != oldPlayer && target != null && !target.Deleted) //Neues Mobile �bernehmen
            {
                from.SendMessage("You Control  {0}, {1}", target.Name, target.Title);
                //"You Control  {0}, {1}"

                //Update ControlItem
                controlItem.NPC = target;
                controlItem.Stats = stats;
                controlItem.Skills = skills;
                controlItem.Items = items;
                new CloneTarget().SimulateTarget(from, target, true);

                target.Internalize();
            }
            else if (target == oldPlayer && !target.Deleted)
            {
                controlItem.Delete();
            }
        }

        public static void EndControl(ControlItem controlItem, bool stats, bool skills, bool items)
        {
            Mobile from = controlItem.Owner;
            PlayerMobile oldPlayer = controlItem.Player;
            Mobile oldNPC = controlItem.NPC;

            if (from == null)
                return;

            from.SendMessage("Vous �tes dans votre corps d'origine.");
            //"You are in your original Body"

            //NPC wiederherstellen
            if (oldNPC != null && !oldNPC.Deleted)
            {
                new CloneTarget().SimulateTarget(oldNPC, from, true);
            }
            else
            {
                from.SendMessage("Votre corps original a �t� d�truit.");
                //"The original NPC was deleted. Maybe because a manual respawn"
                oldNPC.Delete();
            }

            if (oldPlayer != null && !oldPlayer.Deleted)
            {
                //Spieler Wiederherstellen (100%)
                //Props: oldPlayer -> player
                //CopyProps(from, oldPlayer, true, true);
                new CloneTarget().SimulateTarget(from, oldPlayer, false);
                //Equip: oldPlayer -> player
                //MoveEquip(oldPlayer, from, true);

                oldPlayer.Delete();
            }
        }

        //Return true if the base.OnBeforeDeath should be executed and false if not.
        public static bool UncontrolDeath(Mobile from)
        {
            if (from.AccessLevel < accessLevel)
                return true;

            ControlItem controlItem = GetControlItem(from);

            if (controlItem != null)
            {
                //Backup NPC
                Mobile NPC = (Mobile)controlItem.NPC;

                //Release GM
                controlItem.Delete();
                from.Hits = from.HitsMax;
                from.Stam = from.StamMax;
                from.Mana = from.StamMax;

                //Kill NPC as normal
                NPC.Kill();

                return false; //GM stirbt nicht ;)
            }

            return true;
        }

        //With items for DupeCommand?
        public static Mobile DupeMobile(object mobile)
        {
            Type t = mobile.GetType();
            object o = Construct(t);

            if (o == null)
            {
                Console.WriteLine("Unable to dupe {0}. Mobile must have a 0 parameter constructor.", t.Name);
                return null;
            }

            if (o is Mobile)
            {
                Mobile newMobile = (Mobile)o;
                Mobile srcMobile = (Mobile)mobile;
                //CopyProperties( o, mobile, t, "Parent", "NetState" );

                //CopyProps(newMobile, srcMobile, true, true);

                //CopyProps didn't copy the AccessLevel, but we need it for some items the GM is wearing.
                newMobile.AccessLevel = srcMobile.AccessLevel;

                newMobile.Player = false;
                newMobile.UpdateTotals();
                return newMobile;
            }

            return null;
        }

        private static object Construct(Type type, params object[] constructParams)
        {
            bool constructed = false;
            object toReturn = null;
            ConstructorInfo[] info = type.GetConstructors();

            foreach (ConstructorInfo c in info)
            {
                if (constructed) break;
                ParameterInfo[] paramInfo = c.GetParameters();

                if (paramInfo.Length == constructParams.Length)
                {
                    try
                    {
                        object o = c.Invoke(constructParams);

                        if (o != null)
                        {
                            constructed = true;
                            toReturn = o;
                        }
                    }
                    catch
                    {
                        toReturn = null;
                    }
                }
            }
            return toReturn;
        }


        /*copy the poropertys from one Mobile to another*/
        private static void CopyProps(Mobile target, Mobile from, bool stats, bool skills, bool location)
        {
            try
            {
                if (from.Map == Map.Internal)
                    from.MoveToWorld(target.Location, target.Map);

                if (stats)
                    CopyMobileProps(target, from, location, "Parent", "NetState", "Player", "AccessLevel");
                else
                    CopyMobileProps(target, from, location, "Parent", "NetState", "Player", "AccessLevel", "RawStr", "Str", "RawDex", "Dex", "RawInt", "Int", "Hits", "Mana", "Stam");

                if (skills)
                    //Console.WriteLine("Copy {2} Skills from {0} to {1}", from, target, target.Skills.Length);
                    for (int i = 0; i < target.Skills.Length; ++i)
                    {
                        //Console.WriteLine("Skill {0} old Value = {1} new Value = {2}", i, target.Skills[i].Base, from.Skills[i].Base);
                        target.Skills[i].Base = from.Skills[i].Base;

                    }
            }
            catch
            {
                Console.WriteLine("Error in Control.cs -> CopyProps(Mobile from, Mobile target, bool stats, bool skills)");
                return;
            }
        }

        private static void CopyMobileProps(Mobile dest, Mobile src, bool location, params string[] oProps)
        {
            //Type type = src.GetType(); didn't work correct
            List<String> omitProps = new List<String>(oProps);
            if (!location)
                omitProps.AddRange(new String[] { "Direction", "Location", "Map" });
            Type type = typeof(Mobile);

            PropertyInfo[] props = type.GetProperties();

            bool omit = false;
            //Console.WriteLine("----- COPPY PROPS ------");
            //Console.WriteLine("From: {0} to {1}", src.Name, dest.Name);
            for (int i = 0; i < props.Length; i++)
            {
                try
                {
                    for (int j = 0; j < omitProps.Count; j++)
                    {
                        if (string.Compare(omitProps[j], props[i].Name, true) == 0)
                        {
                            omit = true;
                            //Console.WriteLine("Skip Value {0} @ {1} = {2}", props[i].Name, dest.Name, props[i].GetValue( src, null )); 
                            break;
                        }
                    }

                    if (props[i].CanRead && props[i].CanWrite && !omit)
                    {
                        //Setzte am Ziel 
                        //Console.WriteLine("SetValue {0} @ {1} = {2}", props[i].Name, dest.Name, props[i].GetValue( src, null )); 
                        //dest.SendMessage("SetValue {0}", props[i].Name); 
                        props[i].SetValue(dest, props[i].GetValue(src, null), null);
                        //Console.WriteLine("-> {0}", props[i].GetValue( dest, null ));
                    }

                    omit = false; //Weiter kopieren
                }
                catch
                {
                    Console.WriteLine("Can't copy property: Control.cs");
                }
            }
        }

        private static bool CompareType(object o, Type type)
        {
            if (o.GetType() == type || o.GetType().IsSubclassOf(type))
                return true;
            else
                return false;
        }
    }
}
namespace Server.Items
{
    public class ControlItem : Item
    {
        private Mobile m_Owner;
        private Mobile m_Player;
        private Mobile m_NPC;

        private bool m_Stats;
        private bool m_Skills;
        private bool m_Items;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner
        {
            get
            {
                if (m_Player is PlayerMobile)
                    return (PlayerMobile)m_Owner;
                else return null;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Player
        {
            get
            {
                if (m_Player is PlayerMobile)
                    return (PlayerMobile)m_Player;
                else return null;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile NPC
        {
            get { return m_NPC; }
            set { m_NPC = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Stats
        {
            get { return m_Stats; }
            set { m_Stats = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Skills
        {
            get { return m_Skills; }
            set { m_Skills = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        new public bool Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }


        public ControlItem(Mobile owner, Mobile player, Mobile npc, bool stats, bool skills, bool items)
            : base(0x2106)
        {
            m_Owner = owner;
            m_Player = player;
            m_NPC = npc;

            m_Stats = stats;
            m_Skills = skills;
            m_Items = items;

            Name = "Control Item";
            LootType = LootType.Blessed;
        }

        public ControlItem(Mobile owner, Mobile player, Mobile npc)
            : base(0x2106)
        {
            m_Owner = owner;
            m_Player = player;
            m_NPC = npc;

            m_Stats = true;
            m_Skills = true;
            m_Items = true;

            Name = "Control Item";
            LootType = LootType.Blessed;
        }


        public ControlItem(Serial serial)
            : base(serial)
        {
        }


        public override void OnDoubleClick(Mobile from)
        {
            if (from == m_Owner)
                Delete();

            base.OnDoubleClick(from);
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (RootParent != m_Owner)
                Delete();
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            Delete();


            return false;
            //return base.DropToWorld( from, p );
        }


        public override void OnDelete()
        {
            StaffCommands.EndControl(this, m_Stats, m_Skills, m_Items);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            //Version 1
            writer.Write((bool)m_Stats);
            writer.Write((bool)m_Skills);
            writer.Write((bool)m_Items);

            //Version 0
            writer.Write((Mobile)m_Owner);
            writer.Write((Mobile)m_Player);
            writer.Write((Mobile)m_NPC);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Stats = reader.ReadBool();
                        m_Skills = reader.ReadBool();
                        m_Items = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Owner = reader.ReadMobile();
                        m_Player = reader.ReadMobile();
                        m_NPC = reader.ReadMobile();
                        break;
                    }
            }

        }




    }
}