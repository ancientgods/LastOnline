using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using TShockAPI.DB;

namespace Last_Online
{
    [ApiVersion(1, 16)]
    public class LastOnline : TerrariaPlugin
    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Ancientgods"; }
        }
        public override string Name
        {
            get { return "LastOnline"; }
        }

        public override string Description
        {
            get { return "Lets you check when a player was last online"; }
        }

        public LastOnline(Main game)
            : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("lastonline",Check, "lo"));
        }

        private void Check(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! proper syntax: /lo <username>");
                return;
            }
            string name = string.Join(" ", args.Parameters);
            TShockAPI.DB.User DbUser = new UserManager(TShock.DB).GetUserByName(name);
            if (DbUser == null)
            {
                args.Player.SendErrorMessage("Player not found! (Doesn't exist? Also Case Sensitivity is important)");
                return;
            }
            foreach (TSPlayer ts in TShock.Players)
            {
                if (ts != null)
                {
                    if (ts.Name == DbUser.Name)
                    {
                        args.Player.SendErrorMessage("This player is still online!");
                        return;
                    }
                }
            }
            TimeSpan t = DateTime.UtcNow.Subtract(DateTime.Parse(DbUser.LastAccessed));
            args.Player.SendInfoMessage(string.Format("{0} was last online {1} days {2} hours {3} minutes ago.", DbUser.Name, t.Days, t.Hours, t.Minutes));
        }
    }

}
