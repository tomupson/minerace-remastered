using Unity.Netcode.Components;

namespace MineRace.Utils.Netcode
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
