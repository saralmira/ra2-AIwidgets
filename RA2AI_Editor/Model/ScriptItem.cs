using System.Collections.ObjectModel;

namespace AIcore
{
    public class ScriptItem
    {
        public ScriptItem(int action)
        {
            scriptAction = action;
            Name = action.ToString();
            Summary = Description = "";
            IsAllowedInSkirmish = false;
            ParamAllowed = new ObservableCollection<Parameter>();
        }

        public int scriptAction;
        public string SAction { get { return scriptAction.ToString(); } }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public bool IsAllowedInSkirmish;
        public ObservableCollection<Parameter> ParamAllowed;

        public string GetParamDescription(string param)
        {
            foreach (Parameter p in ParamAllowed)
                if (p.Param == param)
                    return p.Description;
            return "";
        }

        public class Parameter
        {
            public string Param { get; set; }
            public string Description { get; set; }
        }
    }


    /*
    public enum ScriptAction
    {
        Attack = 0,

        AttackWaypoint = 1,
        GoBerzerk = 2,
        MoveToWaypoint = 3,
        MoveToCell = 4,

        Guard = 5,
        JumpToLine = 6,

        PlayerWins = 7,

        Unload = 8,
        Deploy = 9,
        FollowFriendlies = 10,
        DoThis,
        SetGlobal,
        IdleAnim,
        LoadOntoTransport,
        SpyOnBldg,
        PatrolToWaypoint,
        ChangeScript,
        ChangeTeam,
        Panic,
        ChangeHouse,
        Scatter,
        GotoNearbyShroud,
        PlayerLoses,
        PlaySpeech,
        PlaySound,
        PlayMovie,
        PlayMusic,
        ReduceTiberium,
        BeginProduction,
        FireSale,
        SelfDestruct,
        IonStormStartIn,
        IonStornEnd,
        CenterViewOnTeam,
        ReshroudMap,
        RevealMap,
        DeleteTeamMembers,
        ClearGlobal,
        SetLocal,
        ClearLocal,
        Unpanic,
        ForceFacing,
        WaitTillFullyLoaded,
        TruckUnload,
        TruckLoad,
        AttackEnemyBuilding,
        MovetoEnemyBuilding,
        Scout,
        Success,
        Flash,
        PlayAnim,
        TalkBubble,
        GatherAtEnemy,
        GatherAtBase,
        IronCurtainMe,
        ChronoPrepforABwP,
        ChronoPrepforAQ,
        MoveToOwnBuilding,
        //AttackBuildingAtWaypoint,

        SendToGrinder = 60,
        SendToTankBunker,
        SendToBioReactor,
        SendToBunker,
        MoveToNearestCivilBuilding
    }
    */
}
