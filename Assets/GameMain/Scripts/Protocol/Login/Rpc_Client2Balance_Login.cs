using UnityGameFramework.Runtime;

/* Generate Time: 2024/1/19 20:38:38 */

namespace GameMain
{
    public partial class Rpc_Client2Balance_Login
    {
        protected override void OnReply()
        {
            Log.Error($"process login test {m_Rsp.Account}");
        }
    }
}