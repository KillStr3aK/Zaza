namespace Zaza.Internal
{
    using Zaza;

    public class ZazaInternal : Script
    {
        public ZazaInternal()
        {
            // Beginning of your code
        }

        public override void OnTick()
        {
            ZazaConsole.WriteLine($"{this.GetScriptName()} OnTick()");
        }
    }
}
