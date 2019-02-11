using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Emulators.Shortcuts
{
    class WiiUChecker
    {
        public static string CheckForWiiUFolder(string path)
        {
            var checkForCode = Directory.Exists($"{path}/code");
            if (checkForCode)
            {
                foreach (string file in Directory.GetFiles($"{path}/code"))
                {
                    if ((Path.GetExtension(file).Equals(".rpx") || Path.GetExtension(file).Equals(".wud") || Path.GetExtension(file).Equals(".wux") || Path.GetExtension(file).Equals(".wad") || Path.GetExtension(file).Equals(".elf")))
                    {
                        Debug.WriteLine(file);
                        return file;
                    }
                }
            }
            return null;
        }
    }
}
