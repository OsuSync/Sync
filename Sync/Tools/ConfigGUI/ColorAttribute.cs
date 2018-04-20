using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class ColorAttribute : BaseConfigurationAttribute
    {
        //#RRGGBBAA
        public override bool Check(string rgba)
        {
            return rgba.Length == 9
                && rgba[0] == '#'
                && byte.TryParse(rgba.Substring(1, 2), NumberStyles.HexNumber, null, out var _)
                && byte.TryParse(rgba.Substring(3, 2), NumberStyles.HexNumber, null, out var _)
                && byte.TryParse(rgba.Substring(5, 2), NumberStyles.HexNumber, null, out var _)
                && byte.TryParse(rgba.Substring(7, 2), NumberStyles.HexNumber, null, out var _);
        }
    }
}
