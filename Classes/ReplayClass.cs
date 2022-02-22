using System;
using System.Collections.Generic;
using System.Text;

namespace ReplayHandler.Classes
{
    public class ReplayClass
    {
        public List<ReplayTick> Ticks { get; set; }

        public ReplayClass()
        {
            Ticks = new List<ReplayTick>();
        }
    }

    public class ReplayTick
    {
        public bool GameOver { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public int Time { get; set; }
        public int GoalMessageTimer { get; set; }
        public int Period { get; set; }
        public List<ReplayPuck> Pucks { get; set; }
        public List<ReplayPlayer> Players { get; set; }
        public List<ReplayMessage> Messages { get; set; }

        public ReplayTick()
        {
            Pucks = new List<ReplayPuck>();
            Players = new List<ReplayPlayer>();
            Messages = new List<ReplayMessage>();
        }
    }

    public class ReplayPuck
    {
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public decimal PosZ { get; set; }
        public decimal RotX { get; set; }
        public decimal RotY { get; set; }
    }

    public class ReplayPlayer
    {
        public int PlayerIndex { get; set; }
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public decimal PosZ { get; set; }
        public decimal RotX { get; set; }
        public decimal RotY { get; set; }
        public decimal StickPosX { get; set; }
        public decimal StickPosY { get; set; }
        public decimal StickPosZ { get; set; }
        public decimal StickRotX { get; set; }
        public decimal StickRotY { get; set; }
        public decimal HeadTurn { get; set; }
        public decimal BodyLean { get; set; }
    }

    public class ReplayMessage
    {
        public ReplayMessageType ReplayMessageType { get; set; }

        //Chat
        public int PlayerIndex { get; set; }
        public string Message { get; set; }

        //Goal
        public int GoalIndex { get; set; }
        public int AssistIndex { get; set; }

        //PlayerUpdate
        public int UpdatePlayerIndex { get; set; }
        public string PlayerName { get; set; }
        public bool InServer { get; set; }
        public ReplayTeam Team { get; set; }

    }

    public enum ReplayMessageType
    {
        Chat,
        Goal,
        PlayerUpdate
    }

    public enum ReplayTeam
    {
        Red,
        Blue,
        Spectator
    }
}
