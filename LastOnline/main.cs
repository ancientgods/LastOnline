using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using TShockAPI.DB;
using System.Text;
using System.Collections.Generic;

namespace Last_Online
{
    [ApiVersion(1, 20)]
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
            Commands.ChatCommands.Add(new Command("lastonline.check", Check, "lo"));
        }

        private bool isOnline(string name)
        {
            foreach (TSPlayer ts in TShock.Players)
            {
                if (ts == null)
                    continue;

                if (ts.User.Name == name)
                {
                   
                    return true;
                }
            }

            return false;
        }

        private void Check(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                //   args.Player.SendErrorMessage("Invalid syntax! proper syntax: /lo <username>");

                List<User> users = new UserManager(TShock.DB).GetUsers();

                string output = "";
                foreach(User u in users)
                {
                    TimeSpan time = DateTime.UtcNow.Subtract(DateTime.Parse(u.LastAccessed));
                    if (isOnline(u.Name))
                    {
                        output += string.Format("{0} is online for {1}", u.Name, GetTimeFormat(time));
                    }
                    else {
                        output += string.Format("{0} was last seen online {1} ago", u.Name, GetTimeFormat(time));
                    }

                    output += "\n";
                }

                args.Player.SendInfoMessage(output);

                return;
            }
            string name = string.Join(" ", args.Parameters);
            TShockAPI.DB.User DbUser = new UserManager(TShock.DB).GetUserByName(name);
            if (DbUser == null)
            {
                args.Player.SendErrorMessage("Player not found! (Doesn't exist? Also Case Sensitivity is important)");
                return;
            }
            if (isOnline(DbUser.Name))
            {
                args.Player.SendInfoMessage(DbUser.Name + " is still online!");
                return;
            }

            
            TimeSpan t = DateTime.UtcNow.Subtract(DateTime.Parse(DbUser.LastAccessed));
            args.Player.SendInfoMessage(string.Format("{0} was last seen online {1} ago", DbUser.Name, GetTimeFormat(t)));
        }

        public string GetTimeFormat(TimeSpan ts)
        {
            StringBuilder sb = new StringBuilder();
            bool add = false;
            if (ts.Days > 0)
            {
                sb.Append(string.Format("{0} day{1}", ts.Days, ts.Days > 1 ? "s" : ""));
                add = true;
            }
            if (add || ts.Hours > 0)
            {
                sb.Append(string.Format("{0}{1} hour{2}", add ? " " : "", ts.Hours, ts.Hours > 1 ? "s" : ""));
                add = true;
            }
            if (add || ts.Minutes > 0)
            {
                sb.Append(string.Format("{0}{1} minute{2}", add ? " " : "", ts.Minutes, ts.Minutes > 1 ? "s" : ""));
                add = true;
            }
            if (add || ts.Seconds > 0)
            {
                sb.Append(string.Format("{0}{1} second{2}", add ? " " : "", ts.Seconds, ts.Seconds > 1 ? "s" : ""));
                add = true;
            }
            return sb.ToString();
        }
    }
}
