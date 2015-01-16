using System.Threading.Tasks;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class SceneExtensions
    {
        public static KlikAanKlikUitClient KlikAanKlikUitClient(this Scene dev)
        {
            return new KlikAanKlikUitClient(dev.TpcUri);
        }

        public static Task Activate(this Scene scene)
        {
            return scene.KlikAanKlikUitClient().ActivateScene(scene.SceneNo);
        }
    }
}