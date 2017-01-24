﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BungieWebClient.Model.Membership;

namespace BungieWebClient.Model.Character
{
    public class CharacterEndpoint
    {
        public CharacterResponse Response { get; set; }
        public int ErrorCode { get; set; }
        public int ThrottleSeconds { get; set; }
        public string ErrorStatus { get; set; }
        public string Message { get; set; }
        public MessageData MessageData { get; set; }
    }

    public class CharacterResponse : Response
    {
        public Data data { get; set; }
    }

    public class STATDEFENSE
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATINTELLECT
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATDISCIPLINE
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATSTRENGTH
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATLIGHT
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATARMOR
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATAGILITY
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATRECOVERY
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class STATOPTICS
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class Stats
    {
        public STATDEFENSE STAT_DEFENSE { get; set; }
        public STATINTELLECT STAT_INTELLECT { get; set; }
        public STATDISCIPLINE STAT_DISCIPLINE { get; set; }
        public STATSTRENGTH STAT_STRENGTH { get; set; }
        public STATLIGHT STAT_LIGHT { get; set; }
        public STATARMOR STAT_ARMOR { get; set; }
        public STATAGILITY STAT_AGILITY { get; set; }
        public STATRECOVERY STAT_RECOVERY { get; set; }
        public STATOPTICS STAT_OPTICS { get; set; }
    }

    public class Equipment
    {
        public long itemHash { get; set; }
        public List<object> dyes { get; set; }
    }

    public class PeerView
    {
        public List<Equipment> equipment { get; set; }
    }

    public class CharacterBase
    {
        public string membershipId { get; set; }
        public int membershipType { get; set; }
        public string characterId { get; set; }
        public string dateLastPlayed { get; set; }
        public string minutesPlayedThisSession { get; set; }
        public string minutesPlayedTotal { get; set; }
        public int powerLevel { get; set; }
        public long raceHash { get; set; }
        public long genderHash { get; set; }
        public long classHash { get; set; }
        public long currentActivityHash { get; set; }
        public long lastCompletedStoryHash { get; set; }
        public Stats stats { get; set; }
        public int grimoireScore { get; set; }
        public PeerView peerView { get; set; }
        public int genderType { get; set; }
        public int classType { get; set; }
        public long buildStatGroupHash { get; set; }
    }

    public class LevelProgression
    {
        public int dailyProgress { get; set; }
        public int weeklyProgress { get; set; }
        public int currentProgress { get; set; }
        public int level { get; set; }
        public int step { get; set; }
        public int progressToNextLevel { get; set; }
        public int nextLevelAt { get; set; }
        public long progressionHash { get; set; }
    }

    public class Character
    {
        public CharacterBase characterBase { get; set; }
        public LevelProgression levelProgression { get; set; }
        public string emblemPath { get; set; }
        public string backgroundPath { get; set; }
        public long emblemHash { get; set; }
        public int characterLevel { get; set; }
        public int baseCharacterLevel { get; set; }
        public bool isPrestigeLevel { get; set; }
        public double percentToNextLevel { get; set; }
    }

    public class PrimaryStat
    {
        public long statHash { get; set; }
        public int value { get; set; }
        public int maximumValue { get; set; }
    }

    public class Item
    {
        public long itemHash { get; set; }
        public string itemId { get; set; }
        public int quantity { get; set; }
        public int damageType { get; set; }
        public long damageTypeHash { get; set; }
        public bool isGridComplete { get; set; }
        public int transferStatus { get; set; }
        public int state { get; set; }
        public int characterIndex { get; set; }
        public long bucketHash { get; set; }
        public PrimaryStat primaryStat { get; set; }
    }

    public class Currency
    {
        public long itemHash { get; set; }
        public int value { get; set; }
    }

    public class Inventory
    {
        public List<Item> items { get; set; }
        public List<Currency> currencies { get; set; }
    }

    public class Data
    {
        public string membershipId { get; set; }
        public int membershipType { get; set; }
        public List<Character> characters { get; set; }
        public Inventory inventory { get; set; }
        public int grimoireScore { get; set; }
        public int versions { get; set; }
    }
}
