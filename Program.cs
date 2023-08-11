using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using System.Xml;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();
        listener.Start(29294);

        Thread.Sleep(-1);
    }
}
class MyPlayer : Player<MyPlayer>
{
    public bool IsVip;
    
}
class MyGameServer : GameServer<MyPlayer>
{
    bool anyVipPlayer = false;
    public Team VipTeam;
    public override async Task OnRoundStarted()
    {
        SayToChat("Vip is in team: "+VipTeam.ToString());
    }
    public override async Task OnRoundEnded()
    {
    }

    public override async Task OnPlayerConnected(MyPlayer player)
    {
        
        if (!anyVipPlayer)
        {
            foreach (var item in AllPlayers)
            {
                if (item.IsVip)
                {
                    anyVipPlayer = true;
                    break;
                }
            }
        }


        if (!anyVipPlayer)
        {
            player.IsVip = true;
            player.Message("You are the Vip.");
            player.PromoteToSquadLeader();
            player.Kill();
        }
    }

    public override async Task OnAPlayerKilledAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
    {
        if (args.Victim.IsVip)
        {
            await Console.Out.WriteLineAsync("The VIP was killed!");
            Task.Delay(1000);
            ForceEndGame();
        }
    }


    public override async Task<OnPlayerSpawnArguments> OnPlayerSpawning(MyPlayer player, OnPlayerSpawnArguments request)
    {
        if (player.IsVip)
        {
            request.Loadout.PrimaryWeapon = default;
            request.Loadout.SecondaryWeapon = default;
            request.Loadout.LightGadget = null;
            request.Loadout.HeavyGadget = null;
            request.Loadout.Throwable = null;
        }

        return request;
    }
    public override async Task OnPlayerSpawned(MyPlayer player)
    {
        if(player.IsVip)
        {
            player.SetRunningSpeedMultiplier(0.85f);
            player.SetFallDamageMultiplier(1.15f);
            player.SetReceiveDamageMultiplier(1.25f);
            VipTeam = player.Team;
        }
    }
}
