using ReplayHandler.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReplayHandler
{
    public class ReplayHandler
    {
        public static ReplayClass ParseReplay(byte[] bytes)
        {
            var replayClass = new ReplayClass();

            var reader = new HQMMessageReader(bytes);
            var current_msg_pos = 0;
            var _ = reader.ReadU32Aligned();
            var _bytes = reader.ReadU32Aligned();

            while (reader.Pos < bytes.Length)
            {
                var replayTick = new ReplayTick();

                reader.ReadByteAligned();

                replayTick.GameOver = reader.ReadBits(1) == 1;
                replayTick.RedScore = reader.ReadBits(8);
                replayTick.BlueScore = reader.ReadBits(8);
                replayTick.Time = reader.ReadBits(16);
                replayTick.GoalMessageTimer = reader.ReadBits(16);
                replayTick.Period = reader.ReadBits(8);

                var objects = ReadObjects(reader);

                foreach (var obj in objects)
                {
                    if (obj is ReplayPlayer)
                    {
                        replayTick.Players.Add(obj as ReplayPlayer);
                    }
                    else if (obj is ReplayPuck)
                    {
                        replayTick.Pucks.Add(obj as ReplayPuck);
                    }
                }

                var messageNum = reader.ReadBits(16);
                var msgPos = reader.ReadBits(16);

                for (int i = 0; i < messageNum; i++)
                {
                    var msg_pos_of_this_message = msgPos + i;
                    var msg = ReadMessage(reader);
                    if (msg_pos_of_this_message >= current_msg_pos) {
                        replayTick.Messages.Add(msg);
                    }
                }

                current_msg_pos = msgPos + messageNum;

                replayClass.Ticks.Add(replayTick);

                reader.Next();
            }

            return replayClass;
        }

        public static ReplayMessage ReadMessage(HQMMessageReader reader)
        {
            var message = new ReplayMessage();
            var messageType = reader.ReadBits(6);

            if (messageType == 0)
            {
                message.ReplayMessageType = ReplayMessageType.PlayerUpdate;

                message.PlayerIndex = reader.ReadBits(6);
                message.InServer = reader.ReadBits(1) == 1;
                var team = reader.ReadBits(2);

                switch (team)
                {
                    case 0:
                        message.Team = ReplayTeam.Red;
                        break;
                    case 1:
                        message.Team = ReplayTeam.Blue;
                        break;
                    case 3:
                        message.Team = ReplayTeam.Spectator;
                        break;
                }

                var objectIndex = reader.ReadBits(6);

                var bytes = new List<byte>();
                for (int i = 0; i < 31; i++)
                {
                    bytes.Add(Convert.ToByte(reader.ReadBits(7)));
                }

                message.PlayerName = Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);

            }
            else if (messageType == 1)
            {
                message.ReplayMessageType = ReplayMessageType.Goal;

                var team = reader.ReadBits(2);

                switch (team)
                {
                    case 0:
                        message.Team = ReplayTeam.Red;
                        break;
                    case 1:
                        message.Team = ReplayTeam.Blue;
                        break;
                    case 2:
                        message.Team = ReplayTeam.Spectator;
                        break;
                }

                message.GoalIndex = reader.ReadBits(6);
                message.AssistIndex = reader.ReadBits(6);
            }
            else if (messageType == 2)
            {
                message.ReplayMessageType = ReplayMessageType.Chat;

                message.PlayerIndex = reader.ReadBits(6);

                var size = reader.ReadBits(6);

                var bytes = new List<byte>();
                for (int i = 0; i < size; i++)
                {
                    bytes.Add(Convert.ToByte(reader.ReadBits(7)));
                }

                message.Message = Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
            }

            return message;
        }

        private static List<dynamic> ReadObjects(HQMMessageReader reader)
        {
            var objects = new List<dynamic>();

            var currentPacketNum = reader.ReadU32Aligned();
            var previousPacketNum = reader.ReadU32Aligned();

            for (int i = 0; i < 32; i++)
            {
                var isObject = reader.ReadBits(1) == 1;

                if (isObject)
                {
                    var objectType = reader.ReadBits(2);

                    if (objectType == 0)
                    {
                        var replayPlayer = new ReplayPlayer();

                        replayPlayer.PosX = reader.ReadPos(17);
                        replayPlayer.PosY = reader.ReadPos(17);
                        replayPlayer.PosZ = reader.ReadPos(17);

                        replayPlayer.RotX = reader.ReadPos(31);
                        replayPlayer.RotY = reader.ReadPos(31);

                        replayPlayer.StickPosX = reader.ReadPos(13);
                        replayPlayer.StickPosY = reader.ReadPos(13);
                        replayPlayer.StickPosZ = reader.ReadPos(13);

                        replayPlayer.StickRotX = reader.ReadPos(25);
                        replayPlayer.StickRotY = reader.ReadPos(25);

                        replayPlayer.HeadTurn = reader.ReadPos(16);
                        replayPlayer.BodyLean = reader.ReadPos(16);

                        objects.Add(replayPlayer);
                    }
                    else if (objectType == 1)
                    {
                        var replayPuck = new ReplayPuck();

                        replayPuck.PosX = reader.ReadPos(17);
                        replayPuck.PosY = reader.ReadPos(17);
                        replayPuck.PosZ = reader.ReadPos(17);

                        replayPuck.RotX = reader.ReadPos(31);
                        replayPuck.RotY = reader.ReadPos(31);

                        objects.Add(replayPuck);
                    }
                }
            }

            return objects;
        }

    }
}
