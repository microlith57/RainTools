using System.Xml;

namespace Celeste.Mod.RainTools.DecalRegistryProperties {
    public static class PutImageHere {

        public static void HandlePutImageHere(Decal decal, XmlAttributeCollection attrs) {
            decal.image = new Decal.DecalImage();
            decal.Add(decal.image);
        }

        internal static void Load() {
            DecalRegistry.AddPropertyHandler("raintools_put_decal_image_here", HandlePutImageHere);
        }

    }
}
