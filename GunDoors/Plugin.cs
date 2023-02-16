using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

namespace GunDoors
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "GunDoors";
        public override string Name => "GunDoors";
        public override string Author => "Rysik5318";
        public static Plugin plugin;
        public override Version Version { get; } = new Version(1, 0, 0);

        internal List<Door> DoorsInProgress = new List<Door>();

        public override void OnEnabled()
        {
            plugin = this;
            Exiled.Events.Handlers.Player.Shooting += OnPlayerShooting;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Log.Info("" +
                "\nAuthor Plugin - Rysik5318#7967" +
                "\nCo-Authors - EdrenBaton Team" +
                "\nEdrenBaton Team -" +
                "\nR2Kip#3812" +
                "\nMariki#0001" +
                "\nRysik5318#7967" +
                "\nmoseechev#6235");
            base.OnEnabled();
        }

        public override void OnDisabled() 
        { 
            plugin = null;
            Exiled.Events.Handlers.Player.Shooting -= OnPlayerShooting;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            base.OnDisabled();
        }

        public void OnPlayerShooting(ShootingEventArgs ev)
        {
            Physics.Raycast(new Ray(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward), out RaycastHit raycastHit);

            if (Player.Get(raycastHit.transform.GetComponentInParent<ReferenceHub>()) == ev.Player)
                return;
            string ObjectName = raycastHit.transform.name;
            if (ObjectName.Contains("TouchScreenPanel") || ObjectName.Contains("collider"))
            {
                Door door = Door.Get(raycastHit.transform.GetComponentInParent<DoorVariant>());
                if (!door.IsBroken && !door.IsLocked && !DoorsInProgress.Contains(door))
                {
                    if (door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.None && ev.Player.CustomInfo != "SCP-343")
                    {
                        door.Base.NetworkTargetState = !door.Base.NetworkTargetState;
                        DoorsInProgress.Add(door);
                        Timing.CallDelayed(1.5f, () => DoorsInProgress.Remove(door));
                    }
                    else
                        door.PlaySound(DoorBeepType.PermissionDenied);
                }
            }
        }
        private void OnWaitingForPlayers()
        {
            DoorsInProgress.Clear();
        }
    }
}
