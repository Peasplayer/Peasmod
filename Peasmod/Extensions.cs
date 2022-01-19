using Peasmod.Roles.Impostor;

namespace Peasmod
{
    public static class Extensions
    {
        public static void RpcGoInvisible(this PlayerControl player, bool enable)
        {
            Ninja.RpcGoInvisible(player, enable);
        }

        public static void RpcFreeze(this PlayerControl player, bool enable)
        {
            Glaciater.RpcFreeze(player, enable);
        }

        public static void RpcCreateVent(this PlayerControl player)
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            EvilBuilder.RpcCreateVent(player, pos.x, pos.y, pos.z);
        }

        public static void RpcDragBody(this PlayerControl player, byte bodyId)
        {
            Undertaker.RpcDragBody(player, true, bodyId);
        }
        
        public static void RpcDropBody(this PlayerControl player)
        {
            Undertaker.RpcDragBody(player, false, byte.MaxValue);
        }
    }
}