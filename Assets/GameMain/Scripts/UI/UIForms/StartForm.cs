
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class StartForm : UGuiForm
    {
        private ProcedureStart procedureStart;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureStart = (ProcedureStart)userData;
            if (procedureStart == null)
            {
                Log.Warning("ProcedureStart is null.");
                return;
            }

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }

        
        

        
    }
}