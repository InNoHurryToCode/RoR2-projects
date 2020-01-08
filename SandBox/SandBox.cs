using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SandBox {
    [BepInPlugin("com.morris1927.sandbox", "Sandbox", "1.0.0")]
    public class SandBox : BaseUnityPlugin {
        Dictionary<string, int> d = new Dictionary<string, int>();

        public void Awake() {
            int count = 0;

            foreach (var item in Assembly.GetAssembly(typeof(On.CSteamID)).GetTypes()) {
                if (d.ContainsKey(item.Namespace)) {
                    d[item.Namespace]++;
                } else {
                    d.Add(item.Namespace, 1);
                }

            }
            foreach (var item in d.OrderByDescending(x => x.Value).ToList()) {
                Debug.Log(item.Value + " " + item.Key);
            }
            Debug.Log(count);

            //var assembly = AssemblyDefinition.ReadAssembly("C:\\Games\\Steam\\steamapps\\common\\Risk of Rain 2\\BepInEx\\plugins\\R2API\\MMHOOK_Assembly-CSharp.dll");
            //foreach (var item in assembly.MainModule.GetTypes()) {
            ////    Debug.Log(item.Name);
            //
            //}

        }

        public void Update() {
        }





    }
}
