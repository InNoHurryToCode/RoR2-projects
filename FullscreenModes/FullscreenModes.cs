using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FullscreenModes {

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.morris1927.FullscreenModes", "FullscreenModes", "1.0.0")]
    public class FullscreenModes : BaseUnityPlugin {

        public static bool cursorLocked = false;

        public void Awake() {
            On.RoR2.Console.Awake += (orig, self) => {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };
            On.RoR2.RoR2Application.Update += (orig, self) => {
                orig(self);
                Cursor.lockState = ((MPEventSystemManager.kbmEventSystem.isCursorVisible || MPEventSystemManager.combinedEventSystem.isCursorVisible) ? (cursorLocked ? CursorLockMode.Confined : CursorLockMode.None) : CursorLockMode.Locked);
            };

        }

        [ConCommand(commandName = "mod_confine_cursor", flags = ConVarFlags.Engine, helpText = "Sets confine cursor")]
        private static void CCConfineCursor(ConCommandArgs args) {
            cursorLocked = !cursorLocked;
        }


        [ConCommand(commandName = "mod_fullscreen_mode", flags = ConVarFlags.Engine, helpText = "Sets fullscreen mode")]
        private static void CCFullscreenMode(ConCommandArgs args) {
            string modeString = ArgsHelper.GetValue(args.userArgs, 0);

            FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
            if (Enum.TryParse<FullScreenMode>(modeString, out mode)) {
                Screen.fullScreenMode = mode;
            }
        }
    }
    //CommandHelper written by Wildbook
    public class CommandHelper {
        public static void RegisterCommands(RoR2.Console self) {
            var types = typeof(CommandHelper).Assembly.GetTypes();
            var catalog = self.GetFieldValue<IDictionary>("concommandCatalog");

            foreach (var methodInfo in types.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))) {
                var customAttributes = methodInfo.GetCustomAttributes(false);
                foreach (var attribute in customAttributes.OfType<ConCommandAttribute>()) {
                    var conCommand = Reflection.GetNestedType<RoR2.Console>("ConCommand").Instantiate();
                    Debug.Log(conCommand);
                    conCommand.SetFieldValue("flags", attribute.flags);
                    conCommand.SetFieldValue("helpText", attribute.helpText);
                    conCommand.SetFieldValue("action", (RoR2.Console.ConCommandDelegate)Delegate.CreateDelegate(typeof(RoR2.Console.ConCommandDelegate), methodInfo));

                    catalog[attribute.commandName.ToLower()] = conCommand;
                }
            }
        }
    }
    public class ArgsHelper {

        public static string GetValue(List<string> args, int index) {
            if (index < args.Count && index >= 0) {
                return args[index];
            }

            return "";
        }
    }

}
