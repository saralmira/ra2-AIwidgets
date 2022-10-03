using Library;
using RA2AI_Editor;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;

namespace AIcore
{
    public static class Game
    {
        public enum GameType
        {
            Unknown,
            RA,
            YR
        }

        public class GameTypeClass
        {
            public GameType GameType { get; set; }
            public string Description { get; set; }
            public string Digest { get; set; }
        }

        public static readonly GameTypeClass DefaultUnknownGame = new GameTypeClass() { GameType = GameType.Unknown, Description = null, Digest = null };
        public static readonly GameTypeClass DefaultRAGame = new GameTypeClass() { GameType = GameType.RA, Description = Local.Dictionary("GAME_RA"), Digest = "pciAS/TnHWXQNmZa4GbC9emvP3M=" };
        public static readonly GameTypeClass DefaultYRGame = new GameTypeClass() { GameType = GameType.YR, Description = Local.Dictionary("GAME_YR"), Digest = "m+F1aOs8y7FvcCJjyEsh2JEPw4I=" };

        public static ObservableCollection<GameTypeClass> GameLists =
            new ObservableCollection<GameTypeClass>
                {
                    //new GameTypeClass() { GameType = GameType.TS, Description = RA2AI_Editor.Local.Dictionary("GAME_TS") },
                    DefaultRAGame,
                    DefaultYRGame
                };

        private static GameTypeClass _CurrentGame = DefaultUnknownGame;
        public static GameTypeClass CurrentGame
        {
            get { return _CurrentGame; }
            set
            {
                _CurrentGame = value;
                switch (value.GameType)
                {
                    case GameType.Unknown:
                        CurrentGameDir = "";
                        break;
                    case GameType.RA:
                        CurrentGameDir = @"\ra2";
                        break;
                    case GameType.YR:
                        CurrentGameDir = @"\yr";
                        break;
                }
            }
        }

        public static bool IsCurrentGameYR => _CurrentGame.GameType == GameType.YR;

        public static string CurrentGameDir = "";

        public static GameTypeClass GetGameTypeWithName(string name)
        {
            foreach (GameTypeClass game in GameLists)
                if (game.Description == name)
                    return game;
            return null;
        }

        public static GameTypeClass GetGameTypeWithDigest(string digest)
        {
            foreach (GameTypeClass game in GameLists)
                if (game.Digest == digest)
                    return game;
            return null;
        }

        const string customDir = @"Data\Custom\";

        public static GameTypeClass GetExistedCustomGameType(string name)
        {
            string infopath = Environment.CurrentDirectory + @"\" + customDir + name + @"\info.ini";
            string gname;
            if (!File.Exists(infopath))
                return null;
            GameType gt;
            IniClass customgame = new IniClass(infopath);
            if ((gt = customgame.ReadEnumValue("Info", "GameType", GameType.Unknown)) == GameType.Unknown)
                return null;
            if ((gname = customgame.ReadValueWithoutNotes("Info", "Description")) != name)
                return null;
            return new GameTypeClass { GameType = gt, Description = gname, Digest = customgame.ReadValueWithoutNotes("Info", "Digest") };
        }

        public static GameTypeClass CreateCustomGameType(string name)
        {
            PathClass.CreateDir(Environment.CurrentDirectory + @"\" + customDir + name);
            string infopath = Environment.CurrentDirectory + @"\" + customDir + name + @"\info.ini";
            GameTypeClass gtype = new GameTypeClass { GameType = CurrentGame.GameType, Description = name, Digest = Utils.GetMd5OfString(name) };
            IniClass customgame = new IniClass(infopath);
            customgame.WriteValue("Info", "GameType", (int)gtype.GameType);
            customgame.WriteValue("Info", "Description", gtype.Description);
            customgame.WriteValue("Info", "Digest", gtype.Digest);
            customgame.Save();
            return gtype;
        }

        public static bool IsCustomGameType(GameTypeClass gametype)
        {
            return gametype != DefaultUnknownGame && gametype != DefaultRAGame && gametype != DefaultYRGame;
        }
        public static bool IsCustomGameType()
        {
            return IsCustomGameType(CurrentGame);
        }

        public static string GetGameDirRelative()
        {
            if (IsCustomGameType())
                return customDir + CurrentGame.Description;
            else
                return @"Data\" + App.LanguageCurrent + CurrentGameDir;
        }
    }
}
